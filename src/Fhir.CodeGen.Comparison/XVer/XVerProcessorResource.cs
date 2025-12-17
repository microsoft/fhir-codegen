using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Lib.Configuration;
using Fhir.CodeGen.Lib.FhirExtensions;
using Fhir.CodeGen.Lib.Language;
using Fhir.CodeGen.Lib.Models;
using Fhir.CodeGen.Common.Extensions;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using System.CommandLine;
using System.Linq;
using System.Data.Common;
using System.Collections.Concurrent;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Comparison.Models;
using System.Xml.Linq;
using System.Data;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using static System.Net.Mime.MediaTypeNames;
using Hl7.FhirPath.Sprache;
using Tasks = System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Formats.Tar;

namespace Fhir.CodeGen.Comparison.XVer;

public partial class XVerProcessor
{
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
                        $"| ->->->->->->-> <br/>Overview: {overviewToRight}<br/> ->->->->->->-> " +
                        $"<hr/>" +
                        $"←←←←←←← <br/>Overview: {overviewFromRight}<br/> ←←←←←←← ");
                }
                else
                {
                    // write mapping notes
                    writer.Write(
                        $"| ->->->->->->-> <br/>Overview: {overviewToRight}<br/>Artifact: {toRight}<br/> ->->->->->->-> " +
                        $"<hr/>" +
                        $"←←←←←←← <br/>Overview: {overviewFromRight}<br/>Artifact: {fromRight}<br/> ←←←←←←← ");
                }
            }
        }
        writer.WriteLine();

        return;
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

        List<string> headers = ["Canonical", "Name", "Description",];
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


    private void writeMdOverviewIntroValueSets(ExportStreamWriter writer, DefinitionCollection dc)
    {
        writer.Write($"""
            Keyed off: {dc.Key}
            Canonical: {dc.MainPackageCanonical}
            
            ## Value Set Overview

            """);

        List<string> headers = ["Canonical", "Name", "Description", "Expands"];
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

            ### Bindings

            | Source | Element | Binding | Strength |
            | ------ | ------- | ------- | -------- |
            """);

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
                    $"| ->->->->->->-> <br/> {toRight} <br/> ->->->->->->-> " +
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
                                    $"| -> {cell.RightEdge?.UpTarget?.Relationship} -> " +
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

}
