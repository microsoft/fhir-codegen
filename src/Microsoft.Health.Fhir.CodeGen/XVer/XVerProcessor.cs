// <copyright file="XVerProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using System.CommandLine;




#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif


namespace Microsoft.Health.Fhir.CodeGen.XVer;

internal static partial class XVerProcessorLogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to load requested cross-version maps for {cvMapKey}! Processing will be only algorithmic!")]
    internal static partial void LogMapsNotFound(this ILogger logger, string cvMapKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to expand ValueSet {url} for comparison: {details}")]
    internal static partial void LogValueSetNotExpanded(this ILogger logger, string url, string? details);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve ValueSet {url} from {dcKey}")]
    internal static partial void LogValueSetNotFound(this ILogger logger, string url, string dcKey);
}

internal static class XVerExtensions
{
    internal static string ForMdTable(this string value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");

    internal static string ComparisonKey(this ValueSet vs, string graphId) => graphId + "_" + vs.Name.ToPascalCase();

    internal static IEnumerable<(string packageVersion, ValueSetComparisonDetails? comparison, ValueSet vs)> iterate(this Dictionary<string, List<ValueSetComparisonDetails>> comparisons, string keyVersion, ValueSet keyVs)
    {
        foreach ((string version, List<ValueSetComparisonDetails> details) in comparisons)
        {
            // check for being the 'key' value set (details will be empty)
            if ((details.Count == 0) &&
                (version == keyVersion))
            {
                yield return (version, null, keyVs);
            }

            foreach (ValueSetComparisonDetails detail in details)
            {
                if (detail.Target == null)
                {
                    continue;
                }

                yield return (version, detail, detail.Target);
            }
        }
    }

    internal static List<Dictionary<string, ValueSetMapAnnotationCell>> project(this ValueSet keyVs, ValueSetComparisonAnnotation keyAnnotation, DefinitionCollection keyDc)
    {
        List<List<ValueSetMapAnnotationCell>> up = [];
        List<List<ValueSetMapAnnotationCell>> down = [];

        // start with the main valueset and expand up and down
        expand(keyVs, keyAnnotation, keyDc, ComparisonDirection.Up, null, null, [], ref up);

        // iterate over the rows in the up direction and expand them downwards
        foreach (List<ValueSetMapAnnotationCell> row in up)
        {
            expand(keyVs, keyAnnotation, keyDc, ComparisonDirection.Down, null, null, row, ref down);
        }

        // normalize into dictionary for columnar access
        List<Dictionary<string, ValueSetMapAnnotationCell>> projection = down.Select(row => row.ToDictionary(n => n.PackageVersion)).ToList();

        // return our projection
        return projection;

        void expand(
            ValueSet vs,
            ValueSetComparisonAnnotation ca,
            DefinitionCollection dc,
            ComparisonDirection direction,
            ValueSetMapAnnotationCell? lastCell,
            ValueSetComparisonDetails? lastComparison,
            List<ValueSetMapAnnotationCell> row,
            ref List<List<ValueSetMapAnnotationCell>> results)

        {
            ValueSetMapAnnotationCell cell;

            List<ValueSetComparisonDetails> forward;
            List<ValueSetComparisonDetails> reverse;

            // grab our chain
            if (direction == ComparisonDirection.Up)
            {
                forward = ca.ToNext;
                reverse = ca.ToPrev;
            }
            else
            {
                forward = ca.ToPrev;
                reverse = ca.ToNext;
            }

            // check for starting a different direction in the existing row
            if ((lastCell == null) && (row.Count != 0))
            {
                cell = direction == ComparisonDirection.Up ? row[^1] : row[0];
            }
            else
            {
                // create our cell
                if (direction == ComparisonDirection.Up)
                {
                    ValueSetComparisonDetails? toLeft = reverse.FirstOrDefault(d => d.Target == lastCell?.VS);

                    cell = new()
                    {
                        PackageVersion = dc.MainPackageVersion,
                        VS = vs,
                        ComparisonAnnotation = ca,
                        ToLeftCell = toLeft,
                        FromLeftCell = lastComparison,
                    };

                    if (lastCell != null)
                    {
                        lastCell.FromRightCell = toLeft;
                    }

                    row.Add(cell);
                }
                else
                {
                    ValueSetComparisonDetails? toRight = reverse.FirstOrDefault(d => d.Target == lastCell?.VS);

                    cell = new()
                    {
                        PackageVersion = dc.MainPackageVersion,
                        VS = vs,
                        ComparisonAnnotation = ca,
                        ToRightCell = toRight,
                        FromRightCell = lastComparison,
                    };

                    if (lastCell != null)
                    {
                        lastCell.FromLeftCell = toRight;
                    }

                    row.Insert(0, cell);
                }
            }

            // check for no further links in this direction
            if (forward.Count == 0)
            {
                results.Add(row);
                return;
            }

            // iterate over our links in this direction
            foreach (ValueSetComparisonDetails detail in forward)
            {
                if (detail.Target == null)
                {
                    continue;
                }

                if ((!detail.Target.TryGetAnnotation(out ValueSetComparisonAnnotation? detailCA)) ||
                    (detailCA == null))
                {
                    continue;
                }

                // set the 'to next'
                if (direction == ComparisonDirection.Up)
                {
                    cell.ToRightCell = detail;
                }
                else
                {
                    cell.ToLeftCell = detail;
                }

                expand(detail.Target, detailCA, detail.TargetDefinition, direction, cell, detail, [..row], ref results);
            }
        }
    }
}

public class XVerProcessor
{
    internal static readonly ComparisonDirection[] _directions = [ComparisonDirection.Up, ComparisonDirection.Down];

    internal static readonly HashSet<string> _exclusionSet =
    [
        "http://hl7.org/fhir/ValueSet/ucum-units",
        "http://hl7.org/fhir/ValueSet/all-languages",
        "http://hl7.org/fhir/ValueSet/mimetypes",
    ];

    internal static readonly HashSet<string> _escapeValveCodes = [
        "OTHER",
        "Other",
        "other",
        "OTH",      // v3 Null Flavor of other
        "UNKNOWN",
        "Unknown",
        "unknown",
        "UNK",      // v3 Null Flavor of Unknown
        //"NI",       // v3 Null Flavor of No Information
        ];

    private ConfigXVer _config;
    private ILogger _logger;
    private DefinitionCollection[] _definitions;
    private Dictionary<string, int> _definitionIndexes = [];
    private Dictionary<(string left, string right), FhirCoreComparer> _comparisonCache;
    private Dictionary<string, HashSet<string>> _vsUrlsToInclude = [];

    public XVerProcessor(ConfigXVer config, IEnumerable<DefinitionCollection> definitions)
    {
        _config = config;
        _logger = config.LogFactory.CreateLogger<XVerProcessor>();
        _definitions = [.. definitions];
        _definitions.ForEach((DefinitionCollection dc, int i) => { _definitionIndexes.Add(dc.Key, i); return true; });
        _comparisonCache = [];
    }

    public void ProcessCommand(string? command)
    {
        switch (command)
        {
            case "update-maps":
                Load(preferV1Maps: true);
                Compare(saveUpdates: true);
                break;

            case "build-docs":
                Load(preferV1Maps: false);
                WriteComparisonDocs();
                break;

            default:
                Load(preferV1Maps: true);
                Compare(saveUpdates: true);
                WriteComparisonDocs();
                break;
        }
    }

    /// <summary>
    /// Loads the definitions and initializes the comparison cache.
    /// </summary>
    /// <remarks>
    /// TODO(ginoc): once the v2 maps are merged into the main branch, all the v1 handling can be removed
    /// </remarks>
    /// <param name="preferV1Maps">Indicates whether to prefer version 1 maps.</param>
    /// <exception cref="InvalidOperationException">Thrown when there are less than two definitions available for comparison.</exception>
    public void Load(bool preferV1Maps)
    {
        if (_definitions.Length < 2)
        {
            throw new InvalidOperationException("At least two definitions are required to compare.");
        }

        _comparisonCache.Clear();

        // create our comparison objects
        for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
        {
            DefinitionCollection left = _definitions[definitionIndex - 1];
            DefinitionCollection right = _definitions[definitionIndex];

            if (_comparisonCache.ContainsKey((left.Key, right.Key)))
            {
                continue;
            }

            FhirCoreComparer comparer = new(
                left,
                right,
                _config.LogFactory,
                _config.CrossVersionMapSourcePath);

            _ = comparer.GetInitialCrossVersionMaps(preferV1Maps);

            _comparisonCache.Add((left.Key, right.Key), comparer);
        }
    }

    public void Compare(bool? saveUpdates = null)
    {
        if (_definitions.Length < 2)
        {
            throw new InvalidOperationException("At least two definitions are required to compare.");
        }

        // load the current cross version maps if necessary
        if (_comparisonCache.Count == 0)
        {
            Load(preferV1Maps: false);
        }

        // discover the set of value sets that we want to compare across all selected versions
        _vsUrlsToInclude = getValueSetsToCompare();

        // walk the definitions to run the comparisons between each version pair
        for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
        {
            DefinitionCollection left = _definitions[definitionIndex - 1];
            DefinitionCollection right = _definitions[definitionIndex];

            // grab the comparer for this pair (the same comparer will exist for either direction of the pair)
            if (!_comparisonCache.TryGetValue((left.Key, right.Key), out FhirCoreComparer? comparer))
            {
                _logger.LogMapsNotFound($"{left.Key} -> {right.Key}");
                continue;
            }

            // register our filtered sets of value sets
            comparer.RegisterValueSetFilters(_vsUrlsToInclude[left.Key], _vsUrlsToInclude[right.Key]);

            // run the comparison (bi-directional)
            comparer.Compare();

            // save our results if necessary
            if (saveUpdates ?? _config.SaveComparisonResult)
            {
                comparer.Save();
            }
        }
    }

    /// <summary>
    /// Retrieves a dictionary of value sets to compare, based on required bindings and mappings between definition collections.
    /// </summary>
    /// <returns>A dictionary where the key is the definition collection key and the value is a set of unversioned value set URLs to include in the comparison.</returns>
    private Dictionary<string, HashSet<string>> getValueSetsToCompare()
    {
        Dictionary<string, HashSet<string>> vsUrlsToInclude = [];

        // first pass - find value sets that have required bindings
        foreach (DefinitionCollection dc in _definitions)
        {
            // iterate over the value sets in the first definition collection
            foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
            {
                // skip value sets we know we will not process
                if (_exclusionSet.Contains(unversionedUrl))
                {
                    continue;
                }

                // only compare on the highest version in this package
                string vsVersion = versions.OrderDescending().First();
                string versionedUrl = unversionedUrl + "|" + vsVersion;

                // we only need to process value sets that have a required binding
                if (dc.cgHasRequiredBinding(versionedUrl, unversionedUrl))
                {
                    vsUrlsToInclude.AddToValue(dc.Key, unversionedUrl);
                }
            }
        }

        // second pass - find value sets that have a map from a neighbor and were not already included, iterate until we do not find anything new
        bool addedVs = false;
        do
        {
            addedVs = false;

            for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
            {
                DefinitionCollection left = _definitions[definitionIndex - 1];
                DefinitionCollection right = _definitions[definitionIndex];

                // grab the comparer for this pair (always left to right)
                if (!_comparisonCache.TryGetValue((left.Key, right.Key), out FhirCoreComparer? comparer))
                {
                    _logger.LogMapsNotFound($"{left.Key} -> {right.Key}");
                    continue;
                }

                HashSet<string> leftValueSets = vsUrlsToInclude[left.Key];
                HashSet<string> rightValueSets = vsUrlsToInclude[right.Key];

                // iterate over all the currently-selected value sets in the left collection
                foreach (string leftVsUrl in leftValueSets)
                {
                    // get all the map targets for this value set (left to right)
                    List<string> targets = comparer.LeftToRight?.GetMapTargetsForVs(leftVsUrl) ?? [];

                    // make sure all these targets exist in the right set
                    foreach (string target in targets)
                    {
                        if (!rightValueSets.Contains(target))
                        {
                            rightValueSets.Add(target);
                            addedVs = true;
                        }
                    }
                }

                // check value set targets from right to left
                foreach (string rightVsUrl in rightValueSets)
                {
                    // get all the map targets for this value set (left to right)
                    List<string> targets = comparer.RightToLeft?.GetMapTargetsForVs(rightVsUrl) ?? [];

                    // make sure all these targets exist in the right set
                    foreach (string target in targets)
                    {
                        if (!leftValueSets.Contains(target))
                        {
                            leftValueSets.Add(target);
                            addedVs = true;
                        }
                    }
                }
            }
        } while (addedVs);

        return vsUrlsToInclude;
    }

    public void WriteComparisonDocs()
    {
        // check for no output location
        if (string.IsNullOrEmpty(_config.OutputDirectory))
        {
            return;
        }

        // walk the definitions to write comparisons
        foreach (DefinitionCollection dc in _definitions)
        {
            string versionDir = Path.Combine(_config.OutputDirectory, dc.FhirSequence.ToRLiteral());

            // check for the directory already existing
            if (Directory.Exists(versionDir))
            {
                // remove the directory and contents (start clean)
                Directory.Delete(versionDir, true);
            }

            Directory.CreateDirectory(versionDir);

            // write the contents of our value sets
            writeMarkdownValueSets(versionDir, dc);
        }
    }

    private void writeMarkdownValueSets(string dir, DefinitionCollection dc)
    {
        string vsDir = Path.Combine(dir, "ValueSets");
        if (!Directory.Exists(vsDir))
        {
            Directory.CreateDirectory(vsDir);
        }

        string overviewFilename = Path.Combine(dir, "_valuesets.md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        writeMdOverviewIntroValueSets(overviewWriter, dc);

        // build our set of value sets if necessary
        if (_vsUrlsToInclude.Count == 0)
        {
            _vsUrlsToInclude = getValueSetsToCompare();
        }

        // iterate over our value sets from this version
        foreach (string vsUrl in _vsUrlsToInclude[dc.Key].Order())
        {
            bool expanded = true;

            // resolve this value set
            if (!dc.TryExpandVs(vsUrl, out ValueSet? vs, out string? expandMessage))
            {
                _logger.LogValueSetNotFound(vsUrl, expandMessage ?? "failed to expand");
                expanded = false;

                // check to see if we can get an unexpanded one for the overview
                if (!dc.TryGetValueSet(vsUrl, out vs))
                {
                    _logger.LogValueSetNotFound(vsUrl, dc.Key);
                    continue;
                }
            }

            // build the projection for this value set
            List<Dictionary<string, ValueSetMappingCell>> projection = projectComparisons(vs, dc);

            // add our overview entry
            writeMdOverviewEntry(overviewWriter, vs, dc, projection, expanded, expandMessage);

            string filename = Path.Combine(vsDir, getVsFilename(vs.Name.ToPascalCase(), includeRelativeDir: false));
            using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailed(vsWriter, vs, dc, projection, expanded, expandMessage);

                //// check for failures - write a stub file with information about the value set
                //if (ca.FailureCode != null)
                //{
                //    writeMdComparisonFailed(vsWriter, vs);
                //    continue;
                //}
            }

        }


        //foreach (ValueSet vs in dc.ValueSetsByVersionedUrl.Values.OrderBy(vs => vs.Name))
        //{
        //    // skip value sets without a comparison annotation
        //    if ((!vs.TryGetAnnotation(out ValueSetComparisonAnnotation? ca)) ||
        //        (ca == null))
        //    {
        //        continue;
        //    }

        //    // add our overview entry
        //    writeMdOverviewEntry(overviewWriter, vs, ca);

        //    string filename = Path.Combine(vsDir, getVsFilename(vs.Name.ToPascalCase(), includeRelativeDir: false));
        //    using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
        //    {
        //        writeMdDetailed(vsWriter, dc, vs, ca);

        //        // check for failures - write a stub file with information about the value set
        //        if (ca.FailureCode != null)
        //        {
        //            writeMdComparisonFailed(vsWriter, vs);
        //            continue;
        //        }
        //    }
        //}
    }

    private List<Dictionary<string, ValueSetMappingCell>> projectComparisons(ValueSet keyVs, DefinitionCollection keyDc)
    {
        List<Dictionary<string, ValueSetMappingCell>> projection = [];

        // build our initial cell
        ValueSetMappingCell keyCell = new()
        {
            DC = keyDc,
            VS = keyVs,
        };

        // start a row
        Dictionary<string, ValueSetMappingCell> initialRow = [];
        initialRow[keyDc.Key] = keyCell;

        // expand to the right first
        List<Dictionary<string, ValueSetMappingCell>> rightProjection = project(keyCell, ComparisonDirection.Up, initialRow);

        // iterate over the rows in the up direction and expand them downwards
        foreach (Dictionary<string, ValueSetMappingCell> row in rightProjection)
        {
            List<Dictionary<string, ValueSetMappingCell>> downProjection = project(row[keyDc.Key], ComparisonDirection.Down, row);

            // add to our current set of results
            projection.AddRange(downProjection);
        }

        // return our projection
        return projection;

        List<Dictionary<string, ValueSetMappingCell>> project(ValueSetMappingCell startingCell, ComparisonDirection direction, Dictionary<string, ValueSetMappingCell> row)
        {
            List<Dictionary<string, ValueSetMappingCell>> results = [];

            // get the neighbors to the right
            List<ValueSetMappingCell> neighbors = getMappingNeighbors(startingCell, direction);

            if (neighbors.Count == 0)
            {
                return [row];
            }

            // iterate over all the neighbors, create necessary cells, and recurse
            foreach (ValueSetMappingCell neighbor in neighbors)
            {
                // duplicate the existing row (if necessary)
                Dictionary<string, ValueSetMappingCell> newRow;

                if (neighbors.Count == 1)
                {
                    newRow = row;
                }
                else
                {
                    newRow = [];
                    foreach ((string rowKey, ValueSetMappingCell rowCell) in row)
                    {
                        newRow.Add(rowKey, rowCell with { });
                    }
                }

                // get the key cell for this row (may be a copy)
                ValueSetMappingCell cell = newRow[startingCell.DC.Key];

                // add our neighbor cell
                newRow[neighbor.DC.Key] = neighbor;

                // set the mappings from content in the new cell
                if (direction == ComparisonDirection.Up)
                {
                    cell.RightCell = neighbor;
                    cell.ToRightCell = neighbor.FromLeftCell;
                    cell.FromRightCell = neighbor.ToLeftCell;
                }
                else
                {
                    cell.LeftCell = neighbor;
                    cell.ToLeftCell = neighbor.FromRightCell;
                    cell.FromLeftCell = neighbor.ToRightCell;
                }

                // recurse
                List<Dictionary<string, ValueSetMappingCell>> completedRows = completedRows = project(neighbor, direction, newRow);

                // add to our current set of results
                results.AddRange(completedRows);
            }

            return results;
        }

        List<ValueSetMappingCell> getMappingNeighbors(ValueSetMappingCell cell, ComparisonDirection direction)
        {
            int dcIndex = _definitionIndexes[cell.DC.Key];

            int targetIndex = direction == ComparisonDirection.Up
                ? dcIndex + 1
                : dcIndex - 1;

            if ((targetIndex >= _definitions.Length) ||
                (targetIndex < 0))
            {
                return [];
            }

            DefinitionCollection targetDc = _definitions[targetIndex];
            if (!_comparisonCache.TryGetValue((cell.DC.Key, targetDc.Key), out FhirCoreComparer? comparer))
            {
                return [];
            }

            List<ConceptMap> forwardMaps = direction == ComparisonDirection.Up
                ? comparer.LeftToRight?.GetMapsForSource(cell.VS.Url) ?? []
                : comparer.RightToLeft?.GetMapsForTarget(cell.VS.Url) ?? [];

            List<ConceptMap> reverseMaps = direction == ComparisonDirection.Up
                ? comparer.RightToLeft?.GetMapsForTarget(cell.VS.Url) ?? []
                : comparer.LeftToRight?.GetMapsForSource(cell.VS.Url) ?? [];

            List<ValueSetMappingCell> cells = [];

            // iterate over the forward direction
            foreach (ConceptMap cm in forwardMaps)
            {
                // get the target Value Set
                if (!targetDc.TryGetValueSet(cm.cgTargetScope() ?? string.Empty, out ValueSet? targetVs))
                {
                    continue;
                }

                // create our cell - we know the link back to the current one

                if (direction == ComparisonDirection.Up)
                {
                    cells.Add(new()
                    {
                        DC = targetDc,
                        VS = targetVs,
                        LeftCell = cell,
                        FromLeftCell = cm,
                        ToLeftCell = reverseMaps.FirstOrDefault(r => r.cgSourceScope() == targetVs.Url),
                    });
                }
                else
                {
                    cells.Add(new()
                    {
                        DC = targetDc,
                        VS = targetVs,
                        RightCell = cell,
                        FromRightCell = cm,
                        ToRightCell = reverseMaps.FirstOrDefault(r => r.cgSourceScope() == targetVs.Url),
                    });
                }
            }

            return cells;
        }
    }

    
    private void writeMdOverviewIntroValueSets(ExportStreamWriter writer, DefinitionCollection dc)
    {
        writer.Write($"""
            Keyed off: {dc.MainPackageId}@{dc.MainPackageVersion}
            Canonical: {dc.MainPackageCanonical}
            
            ## Value Set Overview

            """);

        // | Name | Canonical | Description | Maps to Lower | Maps to Higher | Errors |
        // | ---- | --------- | ----------- | ------------- | -------------- | ------ |

        List<string> headers = [ "Canonical", "Name", "Description", "Expands" ];
        foreach (DefinitionCollection targetDc in _definitions)
        {
            if (targetDc.Key == dc.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetDc.Key}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }

    private void writeMdOverviewEntry(
        ExportStreamWriter writer,
        ValueSet vs,
        DefinitionCollection dc,
        List<Dictionary<string, ValueSetMappingCell>> projection,
        bool expanded,
        string? expandFailureMessage)
    {
        string vsName = vs.Name.ToPascalCase();

        List<string> mapsTo = [];
        foreach (DefinitionCollection targetDc in _definitions)
        {
            if (targetDc.Key == dc.Key)
            {
                continue;
            }

            mapsTo.Add(projection.Any(r => r.ContainsKey(targetDc.Key)) ? "✔" : "");
        }

        string expandCell = expanded ? "✔" : $"✘ {expandFailureMessage}";

        writer.WriteLine(
            $"| {vs.Url.ForMdTable()}" +
            $" | [{vs.Name.ForMdTable()}]({getVsFilename(vsName)})" +
            $" | {vs.Description.ForMdTable()}" +
            $" | {expandCell}" +
            $" | {string.Join(" | ", mapsTo)} |");

        return;
    }

    /// <summary>
    /// Writes a detailed markdown with information about this value set, keyed from this version.
    /// </summary>
    /// <remarks>
    /// Note this function is currently too long and very inefficient - will fix once output is
    /// finalized.
    /// </remarks>
    /// <param name="writer">       The writer.</param>
    /// <param name="keyVs">        The key vs.</param>
    /// <param name="keyDc">        The key device-context.</param>
    /// <param name="projection">   The projection.</param>
    /// <param name="expanded">     True if expanded.</param>
    /// <param name="expandMessage">Message describing the expand.</param>
    private void writeMdDetailed(
        ExportStreamWriter writer,
        ValueSet keyVs,
        DefinitionCollection keyDc,
        List<Dictionary<string, ValueSetMappingCell>> projection,
        bool expanded,
        string? expandMessage)
    {
        writer.WriteLine($"""
            ### {keyVs.Name}

            |      |     |
            | ---: | --- |
            | Package | {keyDc.MainPackageId}@{keyDc.MainPackageVersion} |
            | Name | {keyVs.Name.ForMdTable()} |
            | URL | {keyVs.Url.ForMdTable()} |
            | Version | {keyVs.Version.ForMdTable()} |
            | Description | {keyVs.Description.ForMdTable()} |
            """);

        //if (keyComparison?.FailureCode != null)
        //{
        //    writer.WriteLine($"""
        //        | Failure | {keyComparison.FailureCode} {keyComparison.FailureMessage?.ForMdTable()} |
        //        """);
        //    return;
        //}

        writer.WriteLine("### Bindings");
        writer.WriteLine();
        writer.WriteLine("| Source | Element | Binding | Strength |");
        writer.WriteLine("| ------ | ------- | ------- | -------- |");

        // get the elements with bindings
        {
            IEnumerable<StructureElementCollection> bindings = keyDc.AllBindingsForVs(keyVs.Url);
            foreach (StructureElementCollection binding in bindings)
            {
                foreach (ElementDefinition ed in binding.Elements)
                {
                    writer.WriteLine($"| {binding.Structure.Url} | {ed.Path} | {ed.Binding.ValueSet} | {ed.Binding.Strength} |");
                }
            }
        }

        writer.WriteLine();

        // if there are no mappings, we are done writing this file
        if (projection.Count == 0)
        {
            return;
        }

        string vsName = keyVs.Name.ToPascalCase();

        Dictionary<string, string> vsRootUrlsByVersion = [];
        foreach (DefinitionCollection dc in _definitions)
        {
            vsRootUrlsByVersion.Add(dc.Key, $"/{dc.FhirSequence.ToRLiteral()}/ValueSets");
        }

        (string key, bool hasMapping)[] allKeys = _definitions.Select(dc => (dc.Key, projection.Any(r => r.ContainsKey(dc.Key)))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Details | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        foreach (Dictionary<string, ValueSetMappingCell> row in projection)
        {
            // traverse columns
            foreach ((string key, _) in allKeys)
            {
                if (!row.TryGetValue(key, out ValueSetMappingCell? cell))
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.VS.Name.ForMdTable()}]({vsRootUrlsByVersion[key]}/{getVsFilename(cell.VS.Name.ToPascalCase(), includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" {cell.VS.Url} ");

                // write mapping notes
                writer.Write(
                    $"| →→→→→→→ <br/> {cell.ToRightCell?.Name ?? "*no map*"} <br/> →→→→→→→ " +
                    $"<hr/>" +
                    $"←←←←←←← <br/> {cell.FromRightCell?.Name ?? "*no map*"} <br/> ←←←←←←← ");
            }

            writer.WriteLine();
        }
        writer.WriteLine();


        //writer.WriteLine("### Mapping Chart");
        //writer.WriteLine();

        //// generate mermaid flow chart showing the mappings between FHIR versions
        //writeMdMermaidOpenFlowchart(writer);

        //// write sub-graphs for each version with the relevant value sets as nodes
        //foreach ((string version, ValueSetComparisonDetails? detail, ValueSet vs) in comparisonsByVersion.iterate(keyDc.MainPackageVersion, keyVs))
        //{
        //    string graphId = getChartGraphId(version);
        //    writer.WriteLineIndented($"subgraph {graphId}[\"{version}\"]");
        //    writer.IncreaseIndent();

        //    string? id;
        //    string? name;

        //    (id, name) = getChartNodeInfo(graphId, version, vs);
        //    if (id != null)
        //    {
        //        writer.WriteLineIndented($"{id}[\"{name}\"]");
        //    }

        //    writer.DecreaseIndent();
        //    writer.WriteLineIndented("end");
        //    writer.WriteLine();
        //}

        //// write invisible links between our sub-graphs to force ordering
        //for (int i = 1; i < comparisonsByVersion.Count; i++)
        //{
        //    writer.WriteLineIndented($"{getChartGraphId(_definitions[i - 1].MainPackageVersion)} ~~~ {getChartGraphId(_definitions[i].MainPackageVersion)}");
        //}
        //writer.WriteLine();

        //// write entries in both directions
        //writeChartLinkageRecursive(writer, keyDc, keyVs, keyComparison, ComparisonDirection.Down);
        //writeChartLinkageRecursive(writer, keyDc, keyVs, keyComparison, ComparisonDirection.Up);

        //writer.WriteLine("```");
        //writer.DecreaseIndent();




        // write a section for the code table
        writer.WriteLine("### Code Mappings");
        writer.WriteLine();

        //// iterate over our projection to write a code map for each pair
        //foreach (Dictionary<string, ValueSetMapProjectionCell> row in projection)
        //{
        //    // write a summary table for this combined set
        //    writer.WriteLine("#### Code Mappings - Set Summary");
        //    writer.WriteLine();
        //    writer.WriteLine("| " + string.Join(" | Mapping | ", row.Select(kvp => kvp.Key + ": " + kvp.Value.VS.Name)));
        //    writeTableColumns(writer, "---", (row.Count * 2) - 1, appendNewline: true);

        //    List<List<string>> codeTable = [];


        //    // iterate across columns to write pairwise tables

        //    writer.WriteLine("##### Pair: ");

        //    writer.WriteLine();
        //    writer.WriteLine("| " + string.Join(" | Mapping | ", comparisonsByVersion.Keys));
        //    writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        //    // traverse columns
        //    foreach (string key in comparisonsByVersion.Keys)
        //    {
        //        if (row.TryGetValue(key, out ValueSetMapProjectionCell? cell))
        //        {
        //            writer.Write(
        //                $"| [{cell.VS.Name.ForMdTable()}]({vsRootUrlsByVersion[key]}/{getVsFilename(cell.VS.Name.ToPascalCase(), includeRelativeDir: false)})" +
        //                $"<br/>" +
        //                $" - {cell.VS.Url} ");

        //            // write mapping notes
        //            writer.Write(
        //                $"| → {cell.ToRightCell?.ExplicitMappingSource?.Split('/')[^1] ?? "*no map*"} → " +
        //                $"<br/>" +
        //                $"← {cell.FromRightCell?.ExplicitMappingSource?.Split('/')[^1] ?? "*no map*"} ← ");
        //        }
        //        else
        //        {
        //            writer.Write("| | ");
        //        }
        //    }

        //    writer.WriteLine();
        //}

        //// write pairwise code mapping tables
        //{
        //    List<string> subgraphs = [];

        //    // iterate over our projection - need one chart for each
        //    foreach (Dictionary<string, ValueSetMapProjectionCell> row in projection)
        //    {
        //        string[] cols = _definitions.Where(dc => row.Keys.Contains(dc.MainPackageVersion)).Select(dc => dc.MainPackageVersion).ToArray();

        //        writeMdMermaidOpenFlowchart(writer);

        //        // iterate over our row
        //        foreach ((string version, ValueSetMapProjectionCell node) in row)
        //        {
        //            // use the version for the subgraph to avoid name collisions
        //            string graphId = getChartGraphId(version);
        //            subgraphs.Add(graphId);

        //            writer.WriteLineIndented($"subgraph {graphId}[\"{node.VS.Name} ({version})\"]");
        //            writer.IncreaseIndent();

        //            // iterate over the codes in this set
        //            node.VS.cgGetFlatContains().ForEach(fc =>
        //            {
        //                writer.WriteLineIndented($"{graphId}_{FhirSanitizationUtils.SanitizeForProperty(fc.Code)}[\"{fc.Code}\"]");
        //            });

        //            writer.DecreaseIndent();
        //            writer.WriteLineIndented("end");
        //        }

        //        // add invisible links between subgraphs to force ordering
        //        for (int i = 1; i < subgraphs.Count; i++)
        //        {
        //            writer.WriteLineIndented(subgraphs[i - 1] + " ~~~ " + subgraphs[i]);
        //        }
        //        writer.WriteLine();

        //        // write the links between the codes
        //        for (int i = 1; i < row.Count; i++)
        //        {
        //            ValueSetMapProjectionCell leftCell = row[cols[i-1]];
        //            ValueSetMapProjectionCell rightCell = row[cols[i]];

        //            if ((leftCell.ComparisonAnnotation == null) ||
        //                (rightCell.ComparisonAnnotation == null))
        //            {
        //                continue;
        //            }

        //            ValueSetComparisonAnnotation leftCA = leftCell.ComparisonAnnotation;
        //            ValueSetComparisonAnnotation rightCA = rightCell.ComparisonAnnotation;

        //            ValueSetComparisonDetails? leftToRight = leftCA.ToNext.FirstOrDefault(cd => cd.Target == rightCell.VS);
        //            ValueSetComparisonDetails? rightToLeft = rightCA.ToPrev.FirstOrDefault(cd => cd.Target == leftCell.VS);

        //            string leftGraphId = getChartGraphId(leftCell.PackageVersion);
        //            string rightGraphId = getChartGraphId(rightCell.PackageVersion);

        //            // check left to right
        //            if (leftToRight != null)
        //            {
        //                foreach (ValueSetConceptComparisonDetails codeComparison in leftToRight.ValueSetConcepts?.SelectMany(kvp => kvp.Value) ?? [])
        //                {
        //                    if (codeComparison.Target == null)
        //                    {
        //                        continue;
        //                    }

        //                    string leftCode = FhirSanitizationUtils.SanitizeForProperty(codeComparison.Source.Code);
        //                    string rightCode = FhirSanitizationUtils.SanitizeForProperty(codeComparison.Target.Code);


        //                    switch (codeComparison.ConceptDomain?.Relationship)
        //                    {
        //                        case ConceptDomainRelationshipCodes.Equivalent:
        //                        case ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget:
        //                        case ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget:
        //                        case ConceptDomainRelationshipCodes.Related:
        //                            writer.WriteLineIndented($"{leftGraphId}_{leftCode} --> {rightGraphId}_{rightCode}");
        //                            break;

        //                        case ConceptDomainRelationshipCodes.SourceIsNew:
        //                        case ConceptDomainRelationshipCodes.SourceIsDeprecated:
        //                        case ConceptDomainRelationshipCodes.NotMapped:
        //                        case ConceptDomainRelationshipCodes.NotRelated:
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //            }

        //            // check right to left
        //            if (rightToLeft != null)
        //            {
        //                foreach (ValueSetConceptComparisonDetails codeComparison in rightToLeft.ValueSetConcepts?.SelectMany(kvp => kvp.Value) ?? [])
        //                {
        //                    if (codeComparison.Target == null)
        //                    {
        //                        continue;
        //                    }

        //                    string rightCode = FhirSanitizationUtils.SanitizeForProperty(codeComparison.Source.Code);
        //                    string leftCode = FhirSanitizationUtils.SanitizeForProperty(codeComparison.Target.Code);

        //                    switch (codeComparison.ConceptDomain?.Relationship)
        //                    {
        //                        case ConceptDomainRelationshipCodes.Equivalent:
        //                        case ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget:
        //                        case ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget:
        //                        case ConceptDomainRelationshipCodes.Related:
        //                            writer.WriteLineIndented($"{rightGraphId}_{rightCode} --> {leftGraphId}_{leftCode}");
        //                            break;

        //                        case ConceptDomainRelationshipCodes.SourceIsNew:
        //                        case ConceptDomainRelationshipCodes.SourceIsDeprecated:
        //                        case ConceptDomainRelationshipCodes.NotMapped:
        //                        case ConceptDomainRelationshipCodes.NotRelated:
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //            }
        //        }


        //        writer.DecreaseIndent();
        //        writer.WriteLine("```");
        //    }
        //}


        return;
            
        bool isRelated(ConceptDomainRelationshipCodes? relationship) =>
            (relationship == ConceptDomainRelationshipCodes.Equivalent) ||
            (relationship == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget) ||
            (relationship == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget) ||
            (relationship == ConceptDomainRelationshipCodes.Related);

        string getChartGraphId(string version) => FhirSanitizationUtils.SanitizeForProperty("v_" + version);

        (string? id, string? name) getChartNodeInfo(string graphId, string version, ValueSet? chartVs)
        {
            if (chartVs == null)
            {
                return (null, null);
            }

            return ($"{graphId}_{chartVs.Name.ToPascalCase()}", chartVs.Name);
        }

        //void addVersionDetails(List<ValueSetComparisonDetails>? details, ComparisonDirection direction)
        //{
        //    foreach (ValueSetComparisonDetails detail in details ?? [])
        //    {
        //        if (!comparisonsByVersion.TryGetValue(detail.TargetDefinition.MainPackageVersion, out List<ValueSetComparisonDetails>? mapList))
        //        {
        //            continue;
        //        }

        //        mapList.Add(detail);

        //        if (detail.Target == null)
        //        {
        //            continue;
        //        }

        //        if ((!detail.Target.TryGetAnnotation(out ValueSetComparisonAnnotation? detailCA)) ||
        //            (detailCA == null))
        //        {
        //            continue;
        //        }

        //        if (direction == ComparisonDirection.Up)
        //        {
        //            addVersionDetails(detailCA.ToNext, ComparisonDirection.Up);
        //        }

        //        if (direction == ComparisonDirection.Down)
        //        {
        //            addVersionDetails(detailCA.ToPrev, ComparisonDirection.Down);
        //        }
        //    }
        //}

        void writeChartLinkageRecursive(
            ExportStreamWriter writer,
            DefinitionCollection currentDc,
            ValueSet currentVs,
            ValueSetComparisonAnnotation? ca,
            ComparisonDirection direction)
        {
            if (ca == null)
            {
                return;
            }

            string graphId = getChartGraphId(currentDc.MainPackageVersion);
            (string? id, _) = getChartNodeInfo(graphId, currentDc.MainPackageVersion, currentVs);

            if (id == null)
            {
                return;
            }

            foreach (ValueSetComparisonDetails detail in (direction == ComparisonDirection.Up ? ca?.ToNext : ca?.ToPrev) ?? [])
            {
                if (detail.Target == null)
                {
                    continue;
                }

                if ((!detail.Target.TryGetAnnotation(out ValueSetComparisonAnnotation? detailCA)) ||
                    (detailCA == null))
                {
                    continue;
                }

                string targetGraphId = getChartGraphId(detail.TargetDefinition.MainPackageVersion);
                (string? targetId, _) = getChartNodeInfo(targetGraphId, detail.TargetDefinition.MainPackageVersion, detail.Target);

                string link;

                if (direction == ComparisonDirection.Up)
                {
                    // check for reverse direction
                    if (detailCA.ToPrev.Any(prev => (prev.Target?.Url == currentVs.Url) && (prev.Target?.Version == currentVs.Version)))
                    {
                        link = "<-->";
                    }
                    else
                    {
                        link = "-->";
                    }
                }
                else
                {
                    // check for reverse direction
                    if (detailCA.ToNext.Any(next => (next.Target?.Url == currentVs.Url) && (next.Target?.Version == currentVs.Version)))
                    {
                        link = "<-->";
                    }
                    else
                    {
                        link = "<--";
                    }
                }

                // write this entry
                writer.WriteLineIndented($"{id} {link} {targetId}");

                // recurse up
                if (direction == ComparisonDirection.Up)
                {
                    writeChartLinkageRecursive(writer, detail.TargetDefinition, detail.Target, detailCA, ComparisonDirection.Up);
                }

                // recurse down
                if (direction == ComparisonDirection.Down)
                {
                    writeChartLinkageRecursive(writer, detail.TargetDefinition, detail.Target, detailCA, ComparisonDirection.Down);
                }
            }
        }
    }

    private void writeMdMermaidOpenFlowchart(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            ```mermaid
            flowchart LR

            """);

        //writer.WriteLine("""
        //    ```mermaid
        //    %%{init: {"flowchart": {"defaultRenderer": "elk", "width": "100%"}} }%%
        //    flowchart LR

        //    """);
        writer.IncreaseIndent();
    }

    private void writeTableColumns(ExportStreamWriter writer, string value, int count, bool appendNewline = true)
    {
        for (int i = 0; i < count; i++)
        {
            if (appendNewline && (i == count - 1))
            {
                writer.WriteLine(" | " + value);
            }
            else
            {
                writer.Write(" | " + value);
            }
        }
    }

    private string getOverviewTableCell(string sourceName, List<ValueSetComparisonDetails>? details)
    {
        if ((details == null) ||
            (details.Count == 0))
        {
            return " - ";
        }

        return string.Join("<br/>", details.Select(cd => cd.Target == null ? withoutTarget(cd) : withTarget(cd)));

        string withoutTarget(ValueSetComparisonDetails cd)
        {
            return $"{cd.TargetDefinition.FhirSequence.ToRLiteral()} - Not Mapped";
        }

        string withTarget(ValueSetComparisonDetails cd)
        {
            return $"[" +
            $"{cd.TargetDefinition.FhirSequence.ToRLiteral()} " +
            $"{cd.Target?.Name.ForMdTable()} " +
            $" - {cd.ConceptDomain?.Relationship}" +
            $"]({getVsFilename(sourceName, cd)})";
        }
    }

    private string getVsFilename(string sourceVsName, ValueSetComparisonDetails? cd = null, bool includeRelativeDir = true)
    {
        if (cd?.Target == null)
        {
            return includeRelativeDir
                ? $"ValueSets/{sourceVsName}.md"
                : sourceVsName + ".md";
        }

        return includeRelativeDir
            ? $"ValueSets/{sourceVsName}_{cd.TargetDefinition.FhirSequence.ToRLiteral()}_{cd.Target?.Name.ToPascalCase()}"
            : $"{sourceVsName}_{cd.TargetDefinition.FhirSequence.ToRLiteral()}_{cd.Target?.Name.ToPascalCase()}";
    }

    private void writeMdComparisonFailed(ExportStreamWriter writer, ValueSet vs)
    {
        // build a filename for this vs only
        //string filename = Path.Combine(dir, getVsFilename(vs.Name.ToPascalCase(), includeRelativeDir: false));

        // write a stub file with info
        //using ExportStreamWriter writer = createMarkdownWriter(filename, true, true);


    }

    private void writeMdObjectInfo(ExportStreamWriter writer, ValueSet vs)
    {

    }


    private ExportStreamWriter createMarkdownWriter(string filename, bool writeGenerationHeader = true, bool includeGenerationTime = false)
    {
        ExportStreamWriter writer = new(filename);

        if (writeGenerationHeader)
        {
            writer.WriteLine($"Comparison of {string.Join(", ", _definitions.Select(dc => dc.MainPackageId + "@" + dc.MainPackageVersion))}");

            if (includeGenerationTime)
            {
                writer.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
            }

            writer.WriteLine();
        }

        return writer;
    }


    /// <summary>
    /// Compares the value sets between the source and target definition collections.
    /// </summary>
    /// <param name="dcSource">The source definition collection.</param>
    /// <param name="dcTarget">The target definition collection.</param>
    /// <param name="vsUrlsToInclude">The set of value set URLs to include in the comparison.</param>
    /// <param name="cvMap">The cross-version map collection.</param>
    /// <param name="direction">The direction of the comparison.</param>
    private void compareValueSets(
            DefinitionCollection dcSource,
            DefinitionCollection dcTarget,
            HashSet<string> vsUrlsToInclude,
            CrossVersionMapCollection cvMap,
            ComparisonDirection direction)
    {
        // iterate over the value sets in the first definition collection
        foreach ((string unversionedUrl, string[] versions) in dcSource.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // only process value sets we have already determined should be compared
            if (!vsUrlsToInclude.Contains(unversionedUrl))
            {
                continue;
            }

            // only compare on the highest version in this package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // we can only process value sets we can expand
            if (!dcSource.TryExpandVs(versionedUrl, out ValueSet? vs, out string? expandMessage))
            {
                // get the unexpanded value set object
                if (dcSource.ValueSetsByVersionedUrl.TryGetValue(versionedUrl, out vs))
                {
                    if ((!vs.TryGetAnnotation(out ValueSetComparisonAnnotation? ca)) ||
                        (ca == null))
                    {
                        ca = new();
                        vs.AddAnnotation(ca);
                    }

                    ca.FailureCode = ComparisonFailureCodes.CannotExpand;
                    ca.FailureMessage = $"Failed to expand value set {versionedUrl} for comparison: {expandMessage}.";
                }

                _logger.LogValueSetNotExpanded(versionedUrl, expandMessage);
                continue;
            }

            // get or create the comparison annotation for this VS
            if ((!vs.TryGetAnnotation(out ValueSetComparisonAnnotation? comparisonAnnotation)) ||
                (comparisonAnnotation == null))
            {
                comparisonAnnotation = new()
                {
                    EscapeValveCodes = getEscapeValveCodes(vs),
                };
                vs.AddAnnotation(comparisonAnnotation);
            }

            List<ValueSetComparisonDetails> detailsList = direction == ComparisonDirection.Up
                ? comparisonAnnotation.ToNext
                : comparisonAnnotation.ToPrev;

            // get any mappings for this value set (use the versioned URL to get the versioned and unversioned maps)
            List<ConceptMap> vsConceptMaps = cvMap.GetMapsForSource(versionedUrl);
            foreach (ConceptMap cm in vsConceptMaps)
            {
                string cmTarget = cm.TargetScope is Canonical targetCanonical
                    ? targetCanonical.Value ?? targetCanonical.Uri ?? string.Empty
                    : cm.TargetScope is FhirUri targetUri
                    ? targetUri.Value ?? string.Empty
                    : string.Empty;

                if (string.IsNullOrEmpty(cmTarget))
                {
                    continue;
                }

                // check for already being processed
                if (detailsList.Any(cd => cd.Target?.Url == cmTarget))
                {
                    continue;
                }

                // check to see if we have an expandable target value set
                if (!dcTarget.TryExpandVs(cmTarget, out ValueSet? mappedTargetVs))
                {
                    detailsList.Add(new()
                    {
                        TargetDefinition = dcTarget,
                        Target = null,
                        FailureCode = ComparisonFailureCodes.UnresolvedTarget,
                        FailureMessage = $"Failed to resolve target scope for value set {versionedUrl} from {cm.Url}.",
                        ExplicitMappingSource = cm.Url,
                        ConceptDomain = null,
                        ValueSetConcepts = null,
                    });

                    continue;
                }

                // run this comparison and add our results
                detailsList.Add(compareValueSet(vs, mappedTargetVs, dcTarget, cm));
            }

            // check for this valueset exactly in the target collection
            if (!detailsList.Any(cd => cd.Target?.Url == unversionedUrl) &&
                !detailsList.Any(cd => cd.Target?.Url == versionedUrl) &&
                dcTarget.TryExpandVs(unversionedUrl, out ValueSet? unversionedVs))
            {
                detailsList.Add(compareValueSet(vs, unversionedVs, dcTarget, null));
            }
        }
    }

    /// <summary>
    /// Applies the relationship between existing and change concept domain relationship codes.
    /// </summary>
    /// <param name="existing">The existing concept domain relationship code.</param>
    /// <param name="change">The change concept domain relationship code.</param>
    /// <returns>The resulting concept domain relationship code.</returns>
    private ConceptDomainRelationshipCodes applyRelationship(ConceptDomainRelationshipCodes? existing, ConceptDomainRelationshipCodes? change) => existing switch
    {
        ConceptDomainRelationshipCodes.Unknown => change ?? ConceptDomainRelationshipCodes.Unknown,
        ConceptDomainRelationshipCodes.Equivalent => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : change ?? ConceptDomainRelationshipCodes.Equivalent,
        ConceptDomainRelationshipCodes.SourceIsNew => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsDeprecated => cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.NotMapped => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget => cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.Related => (change == ConceptDomainRelationshipCodes.NotRelated)
            ? ConceptDomainRelationshipCodes.NotRelated
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.NotRelated => change ?? ConceptDomainRelationshipCodes.NotRelated,
        _ => change ?? existing ?? ConceptDomainRelationshipCodes.Unknown,
    };

    /// <summary>
    /// Determines if the given ConceptDomainRelationshipCodes is narrower.
    /// </summary>
    /// <param name="cdr">The ConceptDomainRelationshipCodes to check.</param>
    /// <returns>True if the ConceptDomainRelationshipCodes is narrower; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool cdrCodeIsNarrower(ConceptDomainRelationshipCodes? cdr) =>
        cdr == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget ||
        cdr == ConceptDomainRelationshipCodes.SourceIsDeprecated;

    /// <summary>
    /// Determines if the given ConceptDomainRelationshipCodes is broader.
    /// </summary>
    /// <param name="cdr">The ConceptDomainRelationshipCodes to check.</param>
    /// <returns>True if the ConceptDomainRelationshipCodes is broader; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool cdrCodeIsBroader(ConceptDomainRelationshipCodes? cdr) =>
        cdr == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget ||
        cdr == ConceptDomainRelationshipCodes.SourceIsNew ||
        cdr == ConceptDomainRelationshipCodes.NotMapped;

    /// <summary>Compares two ValueSets and returns the comparison details.</summary>
    /// <param name="sourceVs">The source ValueSet.</param>
    /// <param name="targetVs">The target ValueSet.</param>
    /// <param name="dcTarget">The target definition collection.</param>
    /// <param name="cm">      The ConceptMap for mapping concepts between the ValueSets.</param>
    /// <returns>The comparison details of the ValueSets.</returns>
    private ValueSetComparisonDetails compareValueSet(
        ValueSet sourceVs,
        ValueSet targetVs,
        DefinitionCollection dcTarget,
        ConceptMap? cm)
    {
        // build our concept comparison dictionary
        Dictionary<string, ValueSetConceptComparisonDetails[]>? vsConceptComparisons = compareValueSetConcepts(sourceVs, targetVs, cm);

        // start optimistically
        ConceptDomainRelationshipCodes vsRelationship = ConceptDomainRelationshipCodes.Equivalent;

        // iterate over our concept comparisons to determine the overall relationship
        foreach (ValueSetConceptComparisonDetails vscDetails in vsConceptComparisons?.Values.SelectMany(v => v) ?? [])
        {
            vsRelationship = applyRelationship(vsRelationship, vscDetails.ConceptDomain?.Relationship);
        }

        return new()
        {
            TargetDefinition = dcTarget,
            Target = targetVs,
            ExplicitMappingSource = cm?.Url,
            ConceptDomain = new()
            {
                Relationship = vsRelationship,
            },
            ValueSetConcepts = vsConceptComparisons,
        };
    }

    /// <summary>
    /// Retrieves the escape valve codes from the specified ValueSet.
    /// </summary>
    /// <param name="vs">The ValueSet to retrieve the escape valve codes from.</param>
    /// <returns>An array of escape valve codes.</returns>
    private List<string> getEscapeValveCodes(
        ValueSet vs)
    {
        List<string> assumedEscapeValveCodes = [];

        // check all our codes to see if there is an 'escape valve' code
        foreach (ValueSet.ContainsComponent source in vs.cgGetFlatContains())
        {
            if (!_escapeValveCodes.Contains(source.Code))
            {
                continue;
            }

            // add this code to our assumed set
            assumedEscapeValveCodes.Add(source.cgKey());
        }

        return assumedEscapeValveCodes;
    }

    /// <summary>
    /// Compares the concepts of two value sets and generates a dictionary of comparison details.
    /// </summary>
    /// <param name="sourceVs">The source value set.</param>
    /// <param name="targetVs">The target value set.</param>
    /// <param name="cm">The concept map.</param>
    /// <returns>A dictionary containing the comparison details for each concept in the source value set.</returns>
    private Dictionary<string, ValueSetConceptComparisonDetails[]>? compareValueSetConcepts(
        ValueSet sourceVs,
        ValueSet targetVs,
        ConceptMap? cm)
    {
        HashSet<string> escapeValveKeys = sourceVs.TryGetAnnotation(typeof(ValueSetComparisonAnnotation), out object? annotation)
            ? new HashSet<string>(((ValueSetComparisonAnnotation)annotation).EscapeValveCodes ?? [])
            : [];

        Dictionary<string, ValueSetConceptComparisonDetails[]> retVal = [];

        // build a dictionary of target keys so that we can determine if something exists
        Dictionary<string, ValueSet.ContainsComponent> targetContainsDict = targetVs.cgGetFlatContains().ToDictionary(c => c.System + "#" + c.Code);

        HashSet<string> noMaps;
        Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey;

        (noMaps, mapTargetsByKeyBySourceKey) = processValueSetConceptMap(sourceVs.Url, targetVs.Url, cm);

        ValueSet.ContainsComponent[] sourceFlat = sourceVs.cgGetFlatContains().ToArray();

        // iterate over the source expansion and build our comparisons
        foreach (ValueSet.ContainsComponent source in sourceFlat)
        {
            string sourceKey = source.cgKey();
            List<ValueSetConceptComparisonDetails> vscDetails = [];

            // if we have a no-map, use that first
            if (noMaps.Contains(sourceKey))
            {
                vscDetails.Add(new()
                {
                    Source = source,
                    Target = null,
                    ExplicitMappingSource = cm?.Url,
                    ConceptDomain = new()
                    {
                        Relationship = ConceptDomainRelationshipCodes.NotMapped,
                    },
                    ValueDomain = new()
                    {
                        ConceptRelationship = ValueSetConceptRelationshipFlags.Removed,
                        Messages = [$"{sourceKey} explicitly not mapped in {cm?.Url}"],
                    },
                });
            }

            // if we have mappings, use those
            if (mapTargetsByKeyBySourceKey.TryGetValue(sourceKey, out Dictionary<string, ConceptMap.TargetElementComponent>? mapTargetsByKey))
            {
                // iterate over the targets for this source
                foreach ((string targetKey, ConceptMap.TargetElementComponent cmTarget) in mapTargetsByKey)
                {
                    // check for the target in the target value set
                    if (!targetContainsDict.TryGetValue(targetKey, out ValueSet.ContainsComponent? mappedTarget))
                    {
                        vscDetails.Add(new()
                        {
                            Source = source,
                            Target = null,
                            FailureCode = ComparisonFailureCodes.UnresolvedTarget,
                            FailureMessage = $"Failed to resolve target scope for value set { sourceVs.Url} from { cm!.Url} - expected relationship of {cmTarget.Relationship}.",
                            ExplicitMappingSource = cm?.Url,
                            ConceptDomain = null,
                            ValueDomain = null,
                        });

                        continue;
                    }

                    // start with whatever was mapped
                    ConceptDomainRelationshipCodes conceptDomain = cmTarget.Relationship.ToDomainRelationship();

                    vscDetails.Add(new()
                    {
                        Source = source,
                        Target = mappedTarget,
                        ExplicitMappingSource = cm?.Url,
                        ConceptDomain = new()
                        {
                            Relationship = conceptDomain,
                        },
                        ValueDomain = new()
                        {
                            ConceptRelationship = valueDomainForVsConcept(source.System, source.Code, targetKey),
                            Messages = [
                                $"{sourceKey} mapped with relationship {cmTarget.Relationship} to {targetKey} via {cm?.Url}"
                                ],
                        },
                    });
                }
            }

            // if we have nothing by this point, try to compare literals
            if ((vscDetails.Count == 0) &&
                targetContainsDict.TryGetValue(sourceKey, out ValueSet.ContainsComponent? matchedTarget))
            {
                vscDetails.Add(new()
                {
                    Source = source,
                    Target = matchedTarget,
                    ExplicitMappingSource = cm?.Url,
                    ConceptDomain = new()
                    {
                        Relationship = ConceptDomainRelationshipCodes.Equivalent,
                    },
                    ValueDomain = new()
                    {
                        ConceptRelationship = ValueSetConceptRelationshipFlags.Equivalent,
                        Messages = [
                            $"{sourceKey} found exact match to literal with no map - assumed equivalent in {targetVs.Url}"
                            ],
                    },
                });
            }

            // finally, if we have not found anything, it is an implicit no map
            if (vscDetails.Count == 0)
            {
                vscDetails.Add(new()
                {
                    Source = source,
                    Target = null,
                    ExplicitMappingSource = cm?.Url,
                    ConceptDomain = new()
                    {
                        Relationship = ConceptDomainRelationshipCodes.NotMapped,
                    },
                    ValueDomain = new()
                    {
                        ConceptRelationship = ValueSetConceptRelationshipFlags.Removed,
                        Messages = [$"{sourceKey} not mapped - no mapping found and a matching literal was not found in {targetVs.Url}"],
                    },
                });
            }

            // if this is an escape-valve code, we want to check equivalency
            if (escapeValveKeys.Contains(sourceKey))
            {
                List<KeyValuePair<ValueSetConceptComparisonDetails, ValueSetConceptComparisonDetails>> toReplace = [];

                // loop over the existing details and check the relationships
                foreach (ValueSetConceptComparisonDetails vscDetail in vscDetails)
                {
                    if (vscDetail.ConceptDomain?.Relationship != ConceptDomainRelationshipCodes.Equivalent)
                    {
                        continue;
                    }

                    // check the number of codes in the source and target value sets
                    if (sourceFlat.Length != targetContainsDict.Count)
                    {
                        // this should not be equivalent
                        ConceptDomainRelationshipCodes r = sourceFlat.Length > targetContainsDict.Count
                            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget     // more source codes means that other is a narrower concept
                            : ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget;     // more target codes means that other is a broader concept

                        List<string> messages = vscDetail.ConceptDomain.Messages;
                        messages.Add(
                            $"Modified escape-type relationship based on concept domains covered:" +
                            $" source ({sourceKey}) has {sourceFlat.Length} concepts and" +
                            $" target ({targetVs.Url}|{targetVs.Version}) has {targetContainsDict.Count}. ");

                        toReplace.Add(new(
                            vscDetail,
                            vscDetail with
                            {
                                ConceptDomain = vscDetail.ConceptDomain with
                                {
                                    Relationship = r,
                                    Messages = messages,
                                },
                            }));
                    }
                }

                foreach ((ValueSetConceptComparisonDetails original, ValueSetConceptComparisonDetails updated) in toReplace)
                {
                    vscDetails.Remove(original);
                    vscDetails.Add(updated);
                }
            }

            retVal.Add(sourceKey, vscDetails.ToArray());
        }

        return retVal;
    }

    /// <summary>
    /// Determines the relationship between a source value set concept and a target value set concept.
    /// </summary>
    /// <param name="sourceSystem">The system of the source value set concept.</param>
    /// <param name="sourceCode">The code of the source value set concept.</param>
    /// <param name="targetKey">The key of the target value set concept.</param>
    /// <returns>The relationship between the source and target value set concepts.</returns>
    private ValueSetConceptRelationshipFlags valueDomainForVsConcept(
        string sourceSystem,
        string sourceCode,
        string? targetKey)
    {
        if (string.IsNullOrEmpty(targetKey) || (targetKey == "#"))
        {
            return ValueSetConceptRelationshipFlags.Removed;
        }

        ValueSetConceptRelationshipFlags retVal = ValueSetConceptRelationshipFlags.None;

        string[] targetComponents = targetKey!.Split('#');
        string targetSystem = targetComponents[0];
        string targetCode = targetComponents.Length > 1 ? targetComponents[1] : string.Empty;

        if (sourceSystem != targetSystem)
        {
            retVal |= ValueSetConceptRelationshipFlags.SystemChanged;
        }

        if (sourceCode != targetCode)
        {
            retVal |= ValueSetConceptRelationshipFlags.Renamed;
        }

        return retVal;
    }

    /// <summary>
    /// Extracts the unversioned URL from the given URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The unversioned URL.</returns>
    private string getUnversionedUrl(string url) => url.Contains('|') ? url.Split('|')[0] : url;

    /// <summary>
    /// Processes the concept map for the value set.
    /// </summary>
    /// <param name="sourceVsUrl">The URL of the source value set.</param>
    /// <param name="targetVsUrl">The URL of the target value set.</param>
    /// <param name="cm">The concept map.</param>
    /// <returns>A tuple containing the mappings between source and target value set concepts.</returns>
    private (HashSet<string> noMaps, Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey) processValueSetConceptMap(
        string sourceVsUrl,
        string targetVsUrl,
        ConceptMap? cm)
    {
        if (cm == null)
        {
            return ([], []);
        }

        HashSet<string> noMaps = [];

        // build a map of our concept map to simplify lookups
        Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey = [];

        // traverse the groups in our map - each group represents a system
        foreach (ConceptMap.GroupComponent cmGroup in cm.Group)
        {
            string groupSourceSystem = cmGroup.Source ?? getUnversionedUrl(sourceVsUrl);
            string groupTargetSystem = cmGroup.Target ?? getUnversionedUrl(targetVsUrl);

            // add all the elements from this group to our lookup
            foreach (ConceptMap.SourceElementComponent cmElement in cmGroup.Element)
            {
                string sourceKey = $"{groupSourceSystem}#{cmElement.Code}";

                // check for sources without targets
                if ((cmElement.NoMap == true) || (cmElement.Target.Count == 0))
                {
                    if (!noMaps.Contains(sourceKey))
                    {
                        noMaps.Add(sourceKey);
                    }

                    continue;
                }

                // grab the targets for this source
                if (!mapTargetsByKeyBySourceKey.TryGetValue(sourceKey, out Dictionary<string, ConceptMap.TargetElementComponent>? mapTargets))
                {
                    mapTargets = [];
                    mapTargetsByKeyBySourceKey.Add(sourceKey, mapTargets);
                }

                // add our targets
                foreach (ConceptMap.TargetElementComponent cmTarget in cmElement.Target)
                {
                    string targetKey = $"{groupTargetSystem}#{cmTarget.Code}";
                    //mapTargets.Add(targetKey, cmTarget);
                    mapTargets[targetKey] = cmTarget;
                }
            }
        }

        return (noMaps, mapTargetsByKeyBySourceKey);
    }
}
