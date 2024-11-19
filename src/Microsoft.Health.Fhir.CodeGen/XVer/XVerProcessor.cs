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
using System.Linq;
using System.Data.Common;
using System.Collections.Concurrent;




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

            // comparers are bidirectional
            _comparisonCache.Add((left.Key, right.Key), comparer);
            //_comparisonCache.Add((right.Key, left.Key), comparer);
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
        if (string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            return;
        }

        string docDir = Path.Combine(_config.CrossVersionMapSourcePath, "docs");
        if (!Directory.Exists(docDir))
        {
            Directory.CreateDirectory(docDir);
        }

        ValueSetGraph vsGraph = new()
        {
            Definitions = _definitions,
            Resources = buildValueSetNodes(),
            Edges = buildValueSetEdges(),
        };

        // walk the definitions to write comparisons
        foreach (DefinitionCollection dc in _definitions)
        {
            string versionDir = Path.Combine(docDir, dc.FhirSequence.ToRLiteral());

            // check for the directory already existing
            if (Directory.Exists(versionDir))
            {
                // remove the directory and contents (start clean)
                Directory.Delete(versionDir, true);
            }

            Directory.CreateDirectory(versionDir);

            // write the contents of our value sets
            writeMarkdownValueSets(versionDir, dc, vsGraph);
        }
    }

    private HashSet<ValueSet> buildValueSetNodes()
    {
        // build our set of value sets if necessary
        if (_vsUrlsToInclude.Count == 0)
        {
            _vsUrlsToInclude = getValueSetsToCompare();
        }

        HashSet<ValueSet> vsNodes = [];

        foreach (DefinitionCollection dc in _definitions)
        {
            foreach (string vsUrl in _vsUrlsToInclude[dc.Key])
            {
                if (!dc.TryExpandVs(vsUrl, out ValueSet? vs) &&
                    !dc.TryGetValueSet(vsUrl, out vs))
                {
                    continue;
                }

                vsNodes.Add(vs);
            }
        }

        return vsNodes;
    }

    private Dictionary<ValueSet, List<ResourceGraphEdge<ValueSet>>> buildValueSetEdges()
    {
        Dictionary<ValueSet, List<ResourceGraphEdge<ValueSet>>> vsEdges = [];

        foreach (FhirCoreComparer coreComparer in _comparisonCache.Values)
        {
            // iterate over the paired maps in this comparison
            foreach ((ValueSet leftVs, ValueSet rightVs, ConceptMap? up, ConceptMap? down) in coreComparer.GetPairedValueSetMaps())
            {
                if (up != null)
                {
                    vsEdges.AddToValue(leftVs, new()
                    {
                        Direction = ComparisonDirection.Up,
                        Source = leftVs,
                        Target = rightVs,
                        Up = up,
                        Down = down,
                    });
                }

                if (down != null)
                {
                    vsEdges.AddToValue(rightVs, new()
                    {
                        Direction = ComparisonDirection.Down,
                        Source = rightVs,
                        Target = leftVs,
                        Up = up,
                        Down = down,
                    });
                }
            }
        }

        return vsEdges;
    }

    private void writeMarkdownValueSets(string dir, DefinitionCollection dc, ValueSetGraph vsGraph)
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

        ConcurrentBag<string> overviewEntries = [];

        // iterate over our value sets from this version
        Parallel.ForEach(_vsUrlsToInclude[dc.Key], (vsUrl, cancellationToken) =>
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
                    return;
                }
            }

            // build the projection for this value set
            List<ValueSetGraphCell?[]> projection = expanded ? vsGraph.Project(dc, vs) : [];

            // add our overview entry
            overviewEntries.Add(getMdOverviewEntry(vs, dc, projection, expanded, expandMessage));

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
        });

        // write our overview file
        foreach (string line in overviewEntries.Order())
        {
            overviewWriter.WriteLineIndented(line);
        }
    }


    private void writeMdOverviewIntroValueSets(ExportStreamWriter writer, DefinitionCollection dc)
    {
        writer.Write($"""
            Keyed off: {dc.Key}
            Canonical: {dc.MainPackageCanonical}
            
            ## Value Set Overview

            """);

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

    private string getMdOverviewEntry(
        ValueSet vs,
        DefinitionCollection dc,
        List<ValueSetGraphCell?[]> projection,
        bool expanded,
        string? expandFailureMessage)
    {
        string vsName = vs.Name.ToPascalCase();

        List<string> mapsTo = [];
        for (int i = 0; i < _definitions.Length; i++)
        {
            if (i == _definitionIndexes[dc.Key])
            {
                continue;
            }
            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : "");
        }

        string expandCell = expanded ? "✔" : $"✘ {expandFailureMessage}";

        return
            $"| [{vs.Name.ForMdTable()}]({getVsFilename(vsName)})" +
            $" | `{vs.Url.ForMdTable()}`" +
            $" | {vs.Description.ForMdTable()}" +
            $" | {expandCell}" +
            $" | {string.Join(" | ", mapsTo)} |";
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
        List<ValueSetGraphCell?[]> projection,
        bool expanded,
        string? expandMessage)
    {
        int keyColumn = Array.IndexOf(_definitions, keyDc);

        writer.WriteLine($"""
            ### {keyVs.Name}

            |      |     |
            | ---: | --- |
            | Package | {keyDc.Key} |
            | Name | {keyVs.Name.ForMdTable()} |
            | URL | `{keyVs.Url.ForMdTable()}` |
            | Version | {keyVs.Version.ForMdTable()} |
            | Description | {keyVs.Description.ForMdTable()} |
            """);

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
                    writer.WriteLine($"| `{binding.Structure.Url}` | {ed.Path} | `{ed.Binding.ValueSet}` | {ed.Binding.Strength} |");
                }
            }
        }

        writer.WriteLine();

        if (!expanded)
        {
            writer.WriteLine($"""
                ### Expansion Failure

                Failed to expand this value set: {expandMessage}
                """);
            return;
        }

        // if there are no mappings, we are done writing this file
        if (projection.Count == 0)
        {
            writer.WriteLine($"""
                ### Empty Projection

                This Value Set resulted in no projection.
                """);
            return;
        }

        string vsName = keyVs.Name.ToPascalCase();

        string[] vsRootUrlsByVersion = _definitions.Select(dc => $"/docs/{dc.FhirSequence.ToRLiteral()}/ValueSets").ToArray();

        (string key, bool hasMapping)[] allKeys = _definitions.Select((dc, i) => (dc.Key, projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Maps | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        foreach (ValueSetGraphCell?[] row in projection)
        {
            int column = -1;
            // traverse columns
            foreach (ValueSetGraphCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Resource.Name.ForMdTable()}]({vsRootUrlsByVersion[column]}/{getVsFilename(cell.Resource.Name.ToPascalCase(), includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Resource.Url}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string toRight, string fromRight) = getConceptMapMdLinks(cell, ComparisonDirection.Up);

                // write mapping notes
                writer.Write(
                    $"| →→→→→→→ <br/> {toRight} <br/> →→→→→→→ " +
                    $"<hr/>" +
                    $"←←←←←←← <br/> {fromRight} <br/> ←←←←←←← ");
            }
        }
        writer.WriteLine();


        // write a section for the code table
        writer.WriteLine("### Code Mappings");
        writer.WriteLine();

        int mapGroupIndex = 0;

        foreach (ValueSetGraphCell?[] valueSetRow in projection)
        {
            if (valueSetRow[keyColumn] == null)
            {
                continue;
            }

            writer.WriteLine();
            writer.WriteLine("#### Map Group " + mapGroupIndex++);
            writer.WriteLine();
            writer.WriteLine($"This group is centered on the Value Set {valueSetRow[keyColumn]!.Resource.Name} from {valueSetRow[keyColumn]!.DC.Key} (column {keyColumn}).");
            writer.WriteLine("All codes from this value set are listed while other value sets only show contents that have relationships with those codes.");
            writer.WriteLine();

            // write the table header
            for (int col = 0; col < _definitions.Length; col++)
            {
                if (col > 0)
                {
                    writer.Write("| Relationship ");
                }

                ValueSetGraphCell? cell = valueSetRow[col];

                if (cell == null)
                {
                    writer.Write("| *No Map* ");
                    continue;
                }

                if (col == keyColumn)
                {
                    writer.Write($"| {cell.DC.Key} {cell.Resource.Name.ForMdTable()}");
                }
                else
                {
                    writer.Write($"| [{cell.DC.Key} {cell.Resource.Name.ForMdTable()}]({vsRootUrlsByVersion[col]}/{getVsFilename(cell.Resource.Name.ToPascalCase(), includeRelativeDir: false)})");
                }
            }
            writer.WriteLine();
            writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

            // build a code map graph
            ValueSetComponentGraph codeMapGraph = new()
            {
                ValueSetRow = valueSetRow,
            };

            HashSet<string>[] codesPerVs = _definitions.Select(_ => new HashSet<string>()).ToArray();

            // iterate over the components in the key value set
            foreach (ValueSet.ContainsComponent component in valueSetRow[keyColumn]!.Resource.cgGetFlatContains())
            {
                bool hasMap = false;

                // project this component
                foreach (ValueSetComponentGraphCell?[] componentRow in codeMapGraph.Project(valueSetRow[keyColumn]!, component))
                {
                    hasMap = true;
                    int column = -1;

                    // traverse columns
                    foreach (ValueSetComponentGraphCell? cell in componentRow)
                    {
                        column++;

                        if (cell == null)
                        {
                            writer.Write("| | ");
                            continue;
                        }

                        codesPerVs[column].Add(cell.Component.cgKey());

                        if (column == keyColumn)
                        {
                            writer.Write($"| **`{cell.Component.Code.ForMdTable()}`**");
                        }
                        else
                        {
                            writer.Write($"| `{cell.Component.Code.ForMdTable()}`");
                        }

                        if (column == (componentRow.Length - 1))
                        {
                            continue;
                        }

                        if (cell.RightEdge == null)
                        {
                            writer.Write("| ");
                        }
                        else
                        {
                            if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
                                (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
                            {
                                writer.Write("| == ");
                            }
                            else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                                     (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
                            {
                                writer.Write("| > ");
                            }
                            else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                                     (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
                            {
                                writer.Write("| < ");
                            }
                            else
                            {
                                // write mapping notes
                                writer.Write(
                                    $"| → {cell.RightEdge?.UpTarget?.Relationship} → " +
                                    $"<hr/>" +
                                    $"← {cell.RightEdge?.DownTarget?.Relationship} ← ");
                            }
                        }
                    }

                    writer.WriteLine();
                }

                // check for unmapped concepts
                if (!hasMap)
                {
                    for (int i = 0; i < valueSetRow.Length; i++)
                    {
                        if (i == keyColumn)
                        {
                            writer.Write($"| **`{component.Code.ForMdTable()}`**");
                        }
                        else
                        {
                            writer.Write("| ");
                        }
                    }
                    writer.WriteLine();
                }
            }

            // check for unused codes in value sets
            for (int i = 0; i < valueSetRow.Length; i++)
            {
                if (i != 0)
                {
                    writer.Write("| ");
                }

                if (valueSetRow[i] == null)
                {
                    writer.Write("| ");
                }
                else
                {
                    writer.Write($"| *{codesPerVs[i].Count} of {valueSetRow[i]!.UniqueCodeCount} codes used* ");
                }
            }
            writer.WriteLine();

            writer.WriteLine();
        }

        return;
            
        //bool isRelated(ConceptDomainRelationshipCodes? relationship) =>
        //    (relationship == ConceptDomainRelationshipCodes.Equivalent) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.Related);
    }

    private void writeTableColumns(ExportStreamWriter writer, string value, int count, bool appendNewline = true)
    {
        for (int i = 0; i < count; i++)
        {
            if (appendNewline && (i == count - 1))
            {
                writer.WriteLine(" | " + value);
            }
            else if (i == 0)
            {
                writer.Write("| " + value);
            }
            else
            {
                writer.Write(" | " + value);
            }
        }
    }


    private string getVsFilename(string sourceVsName, bool includeRelativeDir = true)
    {
        return includeRelativeDir
            ? $"ValueSets/{sourceVsName}.md"
            : sourceVsName + ".md";

        //return includeRelativeDir
        //    ? $"ValueSets/{sourceVsName}_{cd.TargetDefinition.FhirSequence.ToRLiteral()}_{cd.Target?.Name.ToPascalCase()}"
        //    : $"{sourceVsName}_{cd.TargetDefinition.FhirSequence.ToRLiteral()}_{cd.Target?.Name.ToPascalCase()}";
    }

    private (string to, string from) getConceptMapMdLinks(ValueSetGraphCell cell, ComparisonDirection direction)
    {
        if (direction == ComparisonDirection.Up)
        {
            if ((cell.RightCell == null) ||
                (cell.RightEdge?.Up == null) ||
                (cell.RightEdge?.Down == null))
            {
                return ("*no map*", "*no map*");
            }

            return (
                $"[{cell.RightEdge.Up.Name.ForMdTable()}]" +
                $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{cell.RightCell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.RightEdge.Up.Name}.json)",
                $"[{cell.RightEdge.Down.Name.ForMdTable()}]" +
                $"(/input/codes_v2/{cell.RightCell.DC.FhirSequence.ToRLiteral()}to{cell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.RightEdge.Down.Name}.json)");
        }

        if ((cell.LeftCell == null) ||
            (cell.LeftEdge?.Up == null) ||
            (cell.LeftEdge?.Down == null))
        {
            return ("*no map*", "*no map*");
        }

        return (
            $"[{cell.LeftEdge.Down.Name.ForMdTable()}]" +
            $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{cell.LeftCell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.LeftEdge.Down.Name}.json)",
            $"[{cell.LeftEdge.Up.Name.ForMdTable()}]" +
            $"(/input/codes_v2/{cell.LeftCell.DC.FhirSequence.ToRLiteral()}to{cell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.LeftEdge.Up.Name}.json)");
    }


    private ExportStreamWriter createMarkdownWriter(string filename, bool writeGenerationHeader = true, bool includeGenerationTime = false)
    {
        ExportStreamWriter writer = new(filename);

        if (writeGenerationHeader)
        {
            writer.WriteLine($"Comparison of {string.Join(", ", _definitions.Select(dc => dc.Key))}");

            if (includeGenerationTime)
            {
                writer.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
            }

            writer.WriteLine();
        }

        return writer;
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

}
