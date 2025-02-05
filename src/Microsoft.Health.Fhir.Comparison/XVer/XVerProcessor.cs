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
using Microsoft.Health.Fhir.Comparison.CompareTool;
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
using Microsoft.Health.Fhir.CodeGenCommon.Models;



namespace Microsoft.Health.Fhir.Comparison.XVer;

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

    private string _dbPath;

    public XVerProcessor(ConfigXVer config, IEnumerable<DefinitionCollection> definitions)
    {
        _config = config;
        _logger = config.LogFactory.CreateLogger<XVerProcessor>();
        _definitions = [.. definitions];
        _definitions.ForEach((DefinitionCollection dc, int i) => { _definitionIndexes.Add(dc.Key, i); return true; });
        _comparisonCache = [];

        // TODO(ginoc): need to figure out how to determine in-place vs. output
        _dbPath = Path.Combine(_config.CrossVersionMapSourcePath, "db");
    }

    public void ProcessCommand(string? command)
    {
        switch (command)
        {
            case "convert-from-maps":
                LoadDiffDatabases();
                break;

            case "update-maps":
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                Compare(saveUpdates: true);
                break;

            case "update-vs-maps":
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                Compare(saveUpdates: true, artifactFilter: FhirArtifactClassEnum.ValueSet);
                break;

            case "update-type-maps":
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                Compare(saveUpdates: true, artifactFilter: FhirArtifactClassEnum.PrimitiveType);
                break;

            case "update-resource-maps":
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                Compare(saveUpdates: true, artifactFilter: FhirArtifactClassEnum.Resource);
                break;

            case "build-docs":
                LoadFhirCrossVersionMaps(preferV1Maps: false);
                WriteComparisonDocs();
                break;

            case "build-vs-docs":
                LoadFhirCrossVersionMaps(preferV1Maps: false);
                WriteComparisonDocs(artifactFilter: FhirArtifactClassEnum.ValueSet);
                break;

            case "build-type-docs":
                LoadFhirCrossVersionMaps(preferV1Maps: false);
                WriteComparisonDocs(artifactFilter: FhirArtifactClassEnum.PrimitiveType);
                break;

            case "build-resource-docs":
                LoadFhirCrossVersionMaps(preferV1Maps: false);
                WriteComparisonDocs(artifactFilter: FhirArtifactClassEnum.Resource);
                break;

            default:
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                Compare(saveUpdates: true);
                WriteComparisonDocs();
                break;
        }
    }

    /// <summary>
    /// Loads the definitions and initializes the comparison cache.
    /// </summary>
    /// <remarks>
    /// TODO(ginoc): this is only used to convert origin maps into the database.
    /// </remarks>
    /// <param name="preferV1Maps">Indicates whether to prefer version 1 maps.</param>
    /// <exception cref="InvalidOperationException">Thrown when there are less than two definitions available for comparison.</exception>
    public void LoadFhirCrossVersionMaps(bool preferV1Maps)
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

    //private void createDbsForCollections(
    //    DefinitionCollection left,
    //    DefinitionCollection right)
    //{
    //    FhirCoreComparer comparer = new(
    //        left,
    //        right,
    //        _config.LogFactory,
    //        _config.CrossVersionMapSourcePath);

    //    _ = comparer.GetInitialCrossVersionMaps(true);

    //    // create base difference trackers
    //    DifferenceTracker diffsLeftToRight = new(left, right, _dbPath);
    //    DifferenceTracker diffsRightToLeft = new(right, left, _dbPath);

    //    diffsLeftToRight.InitDb(out bool createdNew);
    //    if (comparer.LeftToRight != null)
    //    {
    //        diffsLeftToRight.LoadFromCrossVersionMaps(comparer.LeftToRight);
    //    }

    //    diffsRightToLeft.InitDb();
    //    if (comparer.RightToLeft != null)
    //    {
    //        diffsRightToLeft.LoadFromCrossVersionMaps(comparer.RightToLeft);
    //    }

    //    _diffCache[(left.Key, right.Key)] = diffsLeftToRight;
    //    _diffCache[(right.Key, left.Key)] = diffsRightToLeft;
    //}

    public void LoadDiffDatabases()
    {
        if (_definitions.Length < 2)
        {
            throw new InvalidOperationException("At least two definitions are required for comparison.");
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

            comparer.Init(_config.CrossVersionMapSourcePath);

            // comparers are bidirectional so only create left-to-right key for sanity
            _comparisonCache.Add((left.Key, right.Key), comparer);
        }
    }


    public void Compare(bool? saveUpdates = null, FhirArtifactClassEnum? artifactFilter = null)
    {
        if (_definitions.Length < 2)
        {
            throw new InvalidOperationException("At least two definitions are required to compare.");
        }

        // load the current cross version maps if necessary
        if (_comparisonCache.Count == 0)
        {
            LoadFhirCrossVersionMaps(preferV1Maps: false);
        }

        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.ValueSet) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            // discover the set of value sets that we want to compare across all selected versions
            _vsUrlsToInclude = getValueSetsToCompare();
        }

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

            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.ValueSet) ||
                (artifactFilter == FhirArtifactClassEnum.Resource))
            {
                // register our filtered sets of value sets
                comparer.RegisterValueSetFilters(_vsUrlsToInclude[left.Key], _vsUrlsToInclude[right.Key]);
            }

            // run the comparison (bi-directional)
            comparer.Compare(artifactFilter);

            // save our results if necessary
            if (saveUpdates ?? _config.SaveComparisonResult)
            {
                comparer.Save(artifactFilter);
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

    public void WriteComparisonDocs(FhirArtifactClassEnum? artifactFilter = null)
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

        ValueSetGraph? vsGraph = null;

        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.ValueSet) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            vsGraph = new()
            {
                Definitions = _definitions,
            };

            vsGraph.Build(_comparisonCache.Values);
        }

        StructureDefinitionGraph? primitiveGraph = null;
        StructureDefinitionGraph? complexGraph = null;

        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            primitiveGraph = new()
            {
                Definitions = _definitions,
                ArtifactType = FhirArtifactClassEnum.PrimitiveType,
            };

            primitiveGraph.Build(_comparisonCache.Values);

            complexGraph = new()
            {
                Definitions = _definitions,
                ArtifactType = FhirArtifactClassEnum.ComplexType,
            };

            complexGraph.Build(_comparisonCache.Values);
        }

        // if we are writing primitives, put the overall mapping doc in the root
        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            writeMarkdownRootPrimitiveMaps(docDir);
        }

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
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.ValueSet))
            {
                writeMarkdownValueSets(versionDir, dc, vsGraph!);
            }

            // write the contents of our types
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource))
            {
                writeMarkdownStructureDefinitions(versionDir, dc, primitiveGraph, FhirArtifactClassEnum.PrimitiveType);
                writeMarkdownStructureDefinitions(versionDir, dc, complexGraph, FhirArtifactClassEnum.ComplexType);
            }
        }
    }


    private void writeMarkdownRootPrimitiveMaps(string dir)
    {
        string overviewFilename = Path.Combine(dir, "PrimitiveTypes.md");

        using ExportStreamWriter writer = createMarkdownWriter(overviewFilename, true, true);

        writer.Write($"""
            ## Primitive Type Mappings

            Primitive types are mapped across all versions using the following table.

            Note that in this table, "concept" refers to the FHIR concept domain and "value" refers to the FHIR value domain.

            The statement: "`typeA` and `typeB` are conceptually interchangeable where appropriate" means that when an *element* provides
            the appropriate context, the concepts are not so disparate that they cannot be used.  For example, an element that
            was defined as a `id` in one version could be defined as a `code` in another version. While the types do not
            *inherently* have a conceptual overlap, the context of the element allows the substitution.  This is different than if
            an element was defined as an `boolean` and changed to a `dateTime`, which do not have the ability to be conceptually mapped.

            | Source Type | Target Type | Concept Relationship | Concept Comment | Value Relationship | Value Comment |
            | --- | --- | --- | --- | --- | --- |

            """);

        foreach (FhirTypeMappings.CodeGenTypeMapping mapping in FhirTypeMappings.PrimitiveMappings)
        {
            writer.WriteLine(
                $"| `{mapping.SourceType}` " +
                $"| `{mapping.TargetType}` " +
                $"| `{mapping.ConceptDomainRelationship}` " +
                $"| {mapping.ConceptDomainComment} " +
                $"| `{mapping.ValueDomainRelationship}` " +
                $"| {mapping.ValueDomainComment} " +
                $"|");
        }
    }

    private void writeMarkdownStructureDefinitions(string dir, DefinitionCollection dc, StructureDefinitionGraph? graph, FhirArtifactClassEnum artifactClass)
    {
        if (graph == null)
        {
            return;
        }

        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string artifactLower = artifactPascal.ToLowerInvariant();

        string artifactDir = Path.Combine(dir, artifactPascal);
        if (!Directory.Exists(artifactDir))
        {
            Directory.CreateDirectory(artifactDir);
        }

        string overviewFilename = Path.Combine(dir, $"{artifactPascal}.md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        writeMdOverviewIntroStructureDefinitions(overviewWriter, dc, artifactClass);

        ConcurrentBag<string> overviewEntries = [];

        IReadOnlyDictionary<string, StructureDefinition> structureDict = dc.GetStructureIndexDict(artifactClass);

        // iterate over our value sets from this version
        Parallel.ForEach(structureDict, (kvp, cancellationToken) =>
        {
            // build the projection for this value set
            List<StructureDefinitionGraphCell?[]> projection = graph.Project(dc, kvp.Value);

            // add our overview entry
            overviewEntries.Add(getMdOverviewEntry(kvp.Value, artifactClass, dc, projection));

            string filename = Path.Combine(artifactDir, getSdFilename(kvp.Value.Name.ToPascalCase(), artifactClass, includeRelativeDir: false));
            using (ExportStreamWriter artifactWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailed(artifactWriter, kvp.Value, artifactClass, dc, projection);

                //// check for failures - write a stub file with information about the structure
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

    private void writeMarkdownValueSets(string dir, DefinitionCollection dc, ValueSetGraph? graph)
    {
        if (graph == null)
        {
            return;
        }

        string vsDir = Path.Combine(dir, "ValueSets");
        if (!Directory.Exists(vsDir))
        {
            Directory.CreateDirectory(vsDir);
        }

        string overviewFilename = Path.Combine(dir, "ValueSets.md");

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
            List<ValueSetGraphCell?[]> projection = expanded ? graph.Project(dc, vs) : [];

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


    private string getMdOverviewEntry(
        StructureDefinition sd,
        FhirArtifactClassEnum artifactClass,
        DefinitionCollection dc,
        List<StructureDefinitionGraphCell?[]> projection)
    {
        string name = sd.Name.ToPascalCase();

        List<string> mapsTo = [];
        for (int i = 0; i < _definitions.Length; i++)
        {
            if (i == _definitionIndexes[dc.Key])
            {
                continue;
            }
            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : "");
        }

        return
            $"| [{sd.Name.ForMdTable()}]({getSdFilename(name, artifactClass)})" +
            $" | `{sd.Url.ForMdTable()}`" +
            $" | {sd.Description.ForMdTable()}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }

    private void writeMdOverviewIntroStructureDefinitions(ExportStreamWriter writer, DefinitionCollection dc, FhirArtifactClassEnum artifactClass)
    {
        string artifactDisplay = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "Primitive Type",
            FhirArtifactClassEnum.ComplexType => "Complex Type",
            FhirArtifactClassEnum.Resource => "Resource",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        writer.Write($"""
            Keyed off: {dc.Key}
            Canonical: {dc.MainPackageCanonical}
            
            ## {artifactDisplay} Overview

            """);

        List<string> headers = ["Canonical", "Name", "Description"];
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

    private void writeMdDetailed(
        ExportStreamWriter writer,
        StructureDefinition keySd,
        FhirArtifactClassEnum artifactClass,
        DefinitionCollection keyDc,
        List<StructureDefinitionGraphCell?[]> projection)
    {
        string artifactDisplay = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "Primitive Type",
            FhirArtifactClassEnum.ComplexType => "Complex Type",
            FhirArtifactClassEnum.Resource => "Resource",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        int keyColumn = Array.IndexOf(_definitions, keyDc);

        writer.WriteLine($"""
            ### {keySd.Name}

            |      |     |
            | ---: | --- |
            | Package | {keyDc.Key} |
            | Name | {keySd.Name.ForMdTable()} |
            | URL | `{keySd.Url.ForMdTable()}` |
            | Version | {keySd.Version.ForMdTable()} |
            | Description | {keySd.Description.ForMdTable()} |
            """);

        // if there are no mappings, we are done writing this file
        if (projection.Count == 0)
        {
            writer.WriteLine($"""
                ### Empty Projection

                This {artifactDisplay} resulted in no projection.
                """);
            return;
        }

        string sdName = keySd.Name.ToPascalCase();

        string[] sdRootUrlsByVersion = _definitions.Select(dc => $"/docs/{dc.FhirSequence.ToRLiteral()}/{artifactPascal}").ToArray();

        (string key, bool hasMapping)[] allKeys = _definitions.Select((dc, i) => (dc.Key, projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Maps | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        foreach (StructureDefinitionGraphCell?[] row in projection)
        {
            int column = -1;
            // traverse columns
            foreach (StructureDefinitionGraphCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Resource.Name.ForMdTable()}]({sdRootUrlsByVersion[column]}/{getSdFilename(cell.Resource.Name.ToPascalCase(), artifactClass, includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Resource.Url}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string overviewToRight, string toRight, string overviewFromRight, string fromRight) = getConceptMapMdLinks(cell, ComparisonDirection.Up, artifactClass);

                // primitives do not have artifact level maps
                if (artifactClass == FhirArtifactClassEnum.PrimitiveType)
                {
                    // write mapping notes
                    writer.Write(
                        $"| →→→→→→→ <br/>Overview: {overviewToRight}<br/> →→→→→→→ " +
                        $"<hr/>" +
                        $"←←←←←←← <br/>Overview: {overviewFromRight}<br/> ←←←←←←← ");
                }
                else
                {
                    // write mapping notes
                    writer.Write(
                        $"| →→→→→→→ <br/>Overview: {overviewToRight}<br/>Artifact: {toRight}<br/> →→→→→→→ " +
                        $"<hr/>" +
                        $"←←←←←←← <br/>Overview: {overviewFromRight}<br/>Artifact: {fromRight}<br/> ←←←←←←← ");
                }
            }
        }
        writer.WriteLine();


        //// write a section for the code table
        //writer.WriteLine("### Code Mappings");
        //writer.WriteLine();

        //int mapGroupIndex = 0;

        //foreach (ValueSetGraphCell?[] valueSetRow in projection)
        //{
        //    if (valueSetRow[keyColumn] == null)
        //    {
        //        continue;
        //    }

        //    writer.WriteLine();
        //    writer.WriteLine("#### Map Group " + mapGroupIndex++);
        //    writer.WriteLine();
        //    writer.WriteLine($"This group is centered on the Value Set {valueSetRow[keyColumn]!.Resource.Name} from {valueSetRow[keyColumn]!.DC.Key} (column {keyColumn}).");
        //    writer.WriteLine("All codes from this value set are listed while other value sets only show contents that have relationships with those codes.");
        //    writer.WriteLine();

        //    // write the table header
        //    for (int col = 0; col < _definitions.Length; col++)
        //    {
        //        if (col > 0)
        //        {
        //            writer.Write("| Relationship ");
        //        }

        //        ValueSetGraphCell? cell = valueSetRow[col];

        //        if (cell == null)
        //        {
        //            writer.Write("| *No Map* ");
        //            continue;
        //        }

        //        if (col == keyColumn)
        //        {
        //            writer.Write($"| {cell.DC.Key} {cell.Resource.Name.ForMdTable()}");
        //        }
        //        else
        //        {
        //            writer.Write($"| [{cell.DC.Key} {cell.Resource.Name.ForMdTable()}]({sdRootUrlsByVersion[col]}/{getVsFilename(cell.Resource.Name.ToPascalCase(), includeRelativeDir: false)})");
        //        }
        //    }
        //    writer.WriteLine();
        //    writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        //    // build a code map graph
        //    ValueSetComponentGraph codeMapGraph = new()
        //    {
        //        SourceRow = valueSetRow,
        //    };

        //    HashSet<string>[] codesPerVs = _definitions.Select(_ => new HashSet<string>()).ToArray();

        //    // iterate over the components in the key value set
        //    foreach (ValueSet.ContainsComponent component in valueSetRow[keyColumn]!.Resource.cgGetFlatContains())
        //    {
        //        bool hasMap = false;

        //        // project this component
        //        foreach (ValueSetComponentGraphCell?[] componentRow in codeMapGraph.Project(valueSetRow[keyColumn]!, component))
        //        {
        //            hasMap = true;
        //            int column = -1;

        //            // traverse columns
        //            foreach (ValueSetComponentGraphCell? cell in componentRow)
        //            {
        //                column++;

        //                if (cell == null)
        //                {
        //                    writer.Write("| | ");
        //                    continue;
        //                }

        //                codesPerVs[column].Add(cell.Component.cgKey());

        //                if (column == keyColumn)
        //                {
        //                    writer.Write($"| **`{cell.Component.Code.ForMdTable()}`**");
        //                }
        //                else
        //                {
        //                    writer.Write($"| `{cell.Component.Code.ForMdTable()}`");
        //                }

        //                if (column == (componentRow.Length - 1))
        //                {
        //                    continue;
        //                }

        //                if (cell.RightEdge == null)
        //                {
        //                    writer.Write("| ");
        //                }
        //                else
        //                {
        //                    if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
        //                        (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
        //                    {
        //                        writer.Write("| == ");
        //                    }
        //                    else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
        //                             (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
        //                    {
        //                        writer.Write("| > ");
        //                    }
        //                    else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
        //                             (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
        //                    {
        //                        writer.Write("| < ");
        //                    }
        //                    else
        //                    {
        //                        // write mapping notes
        //                        writer.Write(
        //                            $"| → {cell.RightEdge?.UpTarget?.Relationship} → " +
        //                            $"<hr/>" +
        //                            $"← {cell.RightEdge?.DownTarget?.Relationship} ← ");
        //                    }
        //                }
        //            }

        //            writer.WriteLine();
        //        }

        //        // check for unmapped concepts
        //        if (!hasMap)
        //        {
        //            for (int i = 0; i < valueSetRow.Length; i++)
        //            {
        //                if (i == keyColumn)
        //                {
        //                    writer.Write($"| **`{component.Code.ForMdTable()}`**");
        //                }
        //                else
        //                {
        //                    writer.Write("| ");
        //                }
        //            }
        //            writer.WriteLine();
        //        }
        //    }

        //    // check for unused codes in value sets
        //    for (int i = 0; i < valueSetRow.Length; i++)
        //    {
        //        if (i != 0)
        //        {
        //            writer.Write("| ");
        //        }

        //        if (valueSetRow[i] == null)
        //        {
        //            writer.Write("| ");
        //        }
        //        else
        //        {
        //            writer.Write($"| *{codesPerVs[i].Count} of {valueSetRow[i]!.UniqueCodeCount} codes used* ");
        //        }
        //    }
        //    writer.WriteLine();

        //    writer.WriteLine();
        //}

        return;

        //bool isRelated(ConceptDomainRelationshipCodes? relationship) =>
        //    (relationship == ConceptDomainRelationshipCodes.Equivalent) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.Related);
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
                SourceRow = valueSetRow,
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

    private string getSdFilename(string sourceName, FhirArtifactClassEnum artifactClass, bool includeRelativeDir = true)
    {
        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        return includeRelativeDir
            ? $"{artifactPascal}/{sourceName}.md"
            : sourceName + ".md";
    }

    private (string overviewTo, string artifactTo, string overviewFrom, string artifactFrom) getConceptMapMdLinks(
        StructureDefinitionGraphCell cell,
        ComparisonDirection direction,
        FhirArtifactClassEnum artifactClass)
    {
        StructureDefinitionGraphCell? targetCell = direction == ComparisonDirection.Up ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("*no map*", "*no map*", "*no map*", "*no map*");
        }

        string overviewRoot = artifactClass == FhirArtifactClassEnum.Resource ? "resources" : "types";

        StructureDefinitionGraphEdge? edge = direction == ComparisonDirection.Up ? cell.RightEdge : cell.LeftEdge;
        ConceptMap? overviewMapTo = direction == ComparisonDirection.Up ? edge?.OverviewUp : edge?.OverviewDown;
        ConceptMap? mapTo = direction == ComparisonDirection.Up ? edge?.Up : edge?.Down;
        ConceptMap? overviewMapFrom = direction == ComparisonDirection.Up ? edge?.OverviewDown : edge?.OverviewUp;
        ConceptMap? mapFrom = direction == ComparisonDirection.Up ? edge?.Down : edge?.Up;

        return (
            getOverviewLink(overviewMapTo, targetCell),
            getArtifactLink(mapTo, targetCell),
            getOverviewLink(overviewMapFrom, targetCell),
            getArtifactLink(mapFrom, targetCell));

        string getOverviewLink(ConceptMap? map, StructureDefinitionGraphCell? target)
        {
            if ((map == null) || (target == null))
            {
                return "*no map*";
            }

            return $"[{map.Name.ForMdTable()}]" +
                $"(/input/{overviewRoot}_v2/ConceptMap-{map.Name}.json)";
        }

        string getArtifactLink(ConceptMap? map, StructureDefinitionGraphCell? target)
        {
            if ((map == null) || (target == null))
            {
                return "*no map*";
            }

            return $"[{map.Name.ForMdTable()}]" +
                $"(/input/{overviewRoot}_v2/{cell.DC.FhirSequence.ToRLiteral()}to{target.DC.FhirSequence.ToRLiteral()}/ConceptMap-{map.Name}.json)";
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
        ValueSetGraphCell? targetCell = direction == ComparisonDirection.Up ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("*no map*", "*no map*");
        }

        ValueSetGraphEdge? edge = direction == ComparisonDirection.Up ? cell.RightEdge : cell.LeftEdge;
        ConceptMap? mapTo = direction == ComparisonDirection.Up ? edge?.Up : edge?.Down;
        ConceptMap? mapFrom = direction == ComparisonDirection.Up ? edge?.Down : edge?.Up;

        return (getLink(mapTo, targetCell), getLink(mapFrom, targetCell));

        //if (direction == ComparisonDirection.Up)
        //{
        //    if ((cell.RightCell == null) ||
        //        (cell.RightEdge?.Up == null) ||
        //        (cell.RightEdge?.Down == null))
        //    {
        //        return ("*no map*", "*no map*");
        //    }

        //    return (
        //        $"[{cell.RightEdge.Up.Name.ForMdTable()}]" +
        //        $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{cell.RightCell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.RightEdge.Up.Name}.json)",
        //        $"[{cell.RightEdge.Down.Name.ForMdTable()}]" +
        //        $"(/input/codes_v2/{cell.RightCell.DC.FhirSequence.ToRLiteral()}to{cell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.RightEdge.Down.Name}.json)");
        //}

        //if ((cell.LeftCell == null) ||
        //    (cell.LeftEdge?.Up == null) ||
        //    (cell.LeftEdge?.Down == null))
        //{
        //    return ("*no map*", "*no map*");
        //}

        //return (
        //    $"[{cell.LeftEdge.Down.Name.ForMdTable()}]" +
        //    $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{cell.LeftCell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.LeftEdge.Down.Name}.json)",
        //    $"[{cell.LeftEdge.Up.Name.ForMdTable()}]" +
        //    $"(/input/codes_v2/{cell.LeftCell.DC.FhirSequence.ToRLiteral()}to{cell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.LeftEdge.Up.Name}.json)");

        string getLink(ConceptMap? map, ValueSetGraphCell? target)
        {
            if ((map == null) || (target == null))
            {
                return "*no map*";
            }

            return $"[{map.Name.ForMdTable()}]" +
                $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{target.DC.FhirSequence.ToRLiteral()}/ConceptMap-{map.Name}.json)";
        }
    }


    private ExportStreamWriter createMarkdownWriter(string filename, bool writeGenerationHeader = true, bool includeGenerationTime = false)
    {
        for (int i = 0; i < 3; i++)
        {
            try
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
            catch (IOException)
            {
                // wait a bit and try again
                Thread.Sleep(1000);
            }
        }

        throw new IOException("Failed to create file after 3 attempts.");
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
