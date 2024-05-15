// <copyright file="PackageComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Language.Debugging;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using static Hl7.Fhir.Model.VerificationResult;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.PackageComparer;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.ComparisonUtils;
using System.Runtime.InteropServices.JavaScript;
using Hl7.Fhir.Rest;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using System.Security.AccessControl;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class PackageComparer
{
    private IFhirPackageClient _cache;

    private DefinitionCollection _left;
    private DefinitionCollection _right;

    private CrossVersionMapCollection? _crossVersion = null;

    private HashSet<FhirArtifactClassEnum> _leftOnlyClasses = [];

    private Dictionary<string, CMR> _typeRelationships = [];


    private string _leftRLiteral;
    private string _rightRLiteral;

    private const string _leftTableLiteral = "Source";
    private const string _rightTableLiteral = "Target";

    private readonly JsonSerializerOptions _firelySerializerOptions;

    private ConfigCompare _config;

    private HttpClient? _httpClient = null;
    private Uri? _ollamaUri = null;

    private Dictionary<string, List<ValueSetComparison>> _vsComparisons = [];

    public record class PackagePathRenames
    {
        public required string PackageDirectiveLeft { get; init; }
        public required string PackageDirectiveRight { get; init; }
        public required Dictionary<string, string> LeftRightPath { get; init; }
    }

    internal static readonly HashSet<string> _exclusionSet =
    [
        /* UCUM is used as a required binding in a codeable concept. Since we do not
         * use enums in this situation, it is not useful to generate this valueset
         */
        "http://hl7.org/fhir/ValueSet/ucum-units",

        /* R5 made Resource.language a required binding to all-languages, which contains
         * all of bcp:47 and is listed as infinite. This is not useful to generate.
         * Note that in R5, many elements that are required to all-languages also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/all-languages",

        /* MIME types are infinite, so we do not want to generate these.
         * Note that in R5, many elements that are required to MIME type also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/mimetypes",

        /* ISO 3166 is large and has not changed since used in this context - do not map it
         * This should not need to be in the list - it is a system not a value set
         */
        //"urn:iso:std:iso:3166",
        //"urn:iso:std:iso:3166:-2",
    ];

    public PackageComparer(ConfigCompare config, IFhirPackageClient cache, DefinitionCollection left, DefinitionCollection right)
    {
        _config = config;
        _cache = cache;
        _left = left;
        _right = right;

        _leftRLiteral = left.FhirSequence.ToRLiteral();
        //_leftShortVersion = left.FhirSequence.ToShortVersion();
        _rightRLiteral = right.FhirSequence.ToRLiteral();
        //_rightShortVersion = right.FhirSequence.ToShortVersion();

        _firelySerializerOptions = new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector).Pretty();

        //if (!string.IsNullOrEmpty(config.OllamaUrl) &&
        //    !string.IsNullOrEmpty(config.OllamaModel))
        //{
        //    _httpClient = new HttpClient();
        //    _ollamaUri = config.OllamaUrl.EndsWith("generate", StringComparison.OrdinalIgnoreCase)
        //        ? new Uri(config.OllamaUrl)
        //        : new Uri(new Uri(config.OllamaUrl), "api/generate");
        //}
    }

    public PackageComparison Compare()
    {
        Console.WriteLine(
            $"Comparing {_left.MainPackageId}#{_left.MainPackageVersion}" +
            $" and {_right.MainPackageId}#{_right.MainPackageVersion}");

        // check for loading cross-version maps
        if (!string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            _crossVersion = new(_cache, _left, _right);

            if (!_crossVersion.TryLoadConceptMaps(_config.CrossVersionMapSourcePath))
            {
                throw new Exception("Failed to load requested cross-version maps");
            }
        }

        // check if we are saving cross version maps and did not load any
        if ((_crossVersion == null) && (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None))
        {
            // create our cross-version map collection
            _crossVersion = new(_cache, _left, _right);
        }

        string outputDir = string.IsNullOrEmpty(_config.CrossVersionMapDestinationPath)
            ? Path.Combine(_config.OutputDirectory, $"{_leftRLiteral}_{_rightRLiteral}")
            : Path.Combine(_config.CrossVersionMapDestinationPath, $"{_leftRLiteral}_{_rightRLiteral}");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string pageDir = Path.Combine(outputDir, "pages");
        if (!Directory.Exists(pageDir))
        {
            Directory.CreateDirectory(pageDir);
        }

        string conceptMapDir = Path.Combine(outputDir, "maps");
        if (!Directory.Exists(conceptMapDir))
        {
            Directory.CreateDirectory(conceptMapDir);
        }

        // build our filename
        string mdFilename = "overview.md";

        string mdFullFilename = Path.Combine(pageDir, mdFilename);

        using ExportStreamWriter? mdWriter = _config.NoOutput ? null : CreateMarkdownWriter(mdFullFilename);

        // need to expand every value set for comparison
        Dictionary<string, ValueSet> vsLeft = GetValueSets(_left);
        _vsComparisons = CompareValueSets(vsLeft, GetValueSets(_right));
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Value Sets", _vsComparisons);

            string mdSubDir = Path.Combine(pageDir, "ValueSets");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<ValueSetComparison> vcs in _vsComparisons.Values)
            {
                foreach (ValueSetComparison c in vcs)
                {
                    //string name = GetName(c.Left, c.Right);
                    //string filename = Path.Combine(subDir, $"{name}.md");
                    string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                    using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                    {
                        WriteComparisonFile(writer, string.Empty, c);
                    }
                }
            }

            // write out the value set maps
            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                string mapSubDir = Path.Combine(conceptMapDir, "ValueSets");
                if (!Directory.Exists(mapSubDir))
                {
                    Directory.CreateDirectory(mapSubDir);
                }

                WriteValueSetMaps(mapSubDir, _vsComparisons.Values.SelectMany(vl => vl.Select(v => v)));
            }
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec>> primitives = ComparePrimitives(FhirArtifactClassEnum.PrimitiveType, _left.PrimitiveTypesByName, _right.PrimitiveTypesByName);
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Primitive Types", primitives.Values);

            string mdSubDir = Path.Combine(pageDir, "PrimitiveTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (ComparisonRecord<StructureInfoRec> c in primitives.Values)
            {
                string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes = CompareStructures(FhirArtifactClassEnum.ComplexType, _left.ComplexTypesByName, _right.ComplexTypesByName);
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Complex Types", complexTypes.Values);

            string mdSubDir = Path.Combine(pageDir, "ComplexTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in complexTypes.Values)
            {
                string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }

            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                string mapSubDir = Path.Combine(conceptMapDir, "ComplexTypes");
                if (!Directory.Exists(mapSubDir))
                {
                    Directory.CreateDirectory(mapSubDir);
                }

                WriteStructureMaps(mapSubDir, complexTypes.Values);
            }

            // write out the data type map
            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                WriteDataTypeMap(conceptMapDir, primitives.Values, complexTypes.Values);
            }
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources = CompareStructures(FhirArtifactClassEnum.Resource, _left.ResourcesByName, _right.ResourcesByName);
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Resources", resources.Values);

            string mdSubDir = Path.Combine(pageDir, "Resources");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in resources.Values)
            {
                //string name = GetName(c.Left, c.Right);
                //string filename = Path.Combine(subDir, $"{name}.md");
                string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }

            // write out the resource type map
            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                WriteResourceTypeMap(conceptMapDir, resources.Values);

                string mapSubDir = Path.Combine(conceptMapDir, "Resources");
                if (!Directory.Exists(mapSubDir))
                {
                    Directory.CreateDirectory(mapSubDir);
                }

                WriteStructureMaps(mapSubDir, resources.Values);
            }
        }

        // TODO(ginoc): Logical models are tracked by URL in collections, but structure mapping is done by name.
        //Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> logical = Compare(FhirArtifactClassEnum.LogicalModel, _left.LogicalModelsByUrl, _right.LogicalModelsByUrl);
        //if (mdWriter != null)
        //{
        //    WriteComparisonOverview(mdWriter, "Logical Models", logical.Values);

        //    string mdSubDir = Path.Combine(pageDir, "LogicalModels");
        //    if (!Directory.Exists(mdSubDir))
        //    {
        //        Directory.CreateDirectory(mdSubDir);
        //    }

        //    foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in logical.Values)
        //    {
        //        string name = GetName(c.Left, c.Right);
        //        string filename = Path.Combine(mdSubDir, $"{name}.md");

        //        using ExportStreamWriter writer = CreateMarkdownWriter(filename);
        //        {
        //            WriteComparisonFile(writer, string.Empty, c);
        //        }
        //    }
        //}

        PackageComparison packageComparison = new()
        {
            LeftPackageId = _left.MainPackageId,
            LeftPackageVersion = _left.MainPackageVersion,
            RightPackageId = _right.MainPackageId,
            RightPackageVersion = _right.MainPackageVersion,
            ValueSets = _vsComparisons,
            PrimitiveTypes = primitives,
            ComplexTypes = complexTypes,
            Resources = resources,
            //LogicalModels = logical,
        };

        if (mdWriter != null)
        {
            mdWriter.Flush();
            mdWriter.Close();
            mdWriter.Dispose();
        }

        if (_config.SaveComparisonResult)
        {
            string jsonFilename = Path.Combine(outputDir, "comparison.json");

            using FileStream jsonFs = new(jsonFilename, FileMode.Create, FileAccess.Write);
            using Utf8JsonWriter jsonWriter = new(jsonFs, new JsonWriterOptions() { Indented = false, });
            {
                JsonSerializer.Serialize(jsonWriter, packageComparison);
            }
        }

        return packageComparison;
    }

    private void WriteResourceTypeMap(
        string outputDir,
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources)
    {
        if (_crossVersion == null)
        {
            return;
        }

        ConceptMap? cm = _crossVersion.GetSourceResourceTypeConceptMap(resources);
        if (cm == null)
        {
            return;
        }

        string filename = Path.Combine(outputDir, $"ConceptMap-{cm.Id}.json");

        try
        {
            using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
            using Utf8JsonWriter writer = new(fs, new JsonWriterOptions() { Indented = true, });
            {
                JsonSerializer.Serialize(writer, cm, _firelySerializerOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing {filename}: {ex.Message} {ex.InnerException?.Message}");
        }
    }

    private void WriteDataTypeMap(
        string outputDir,
        IEnumerable<ComparisonRecord<StructureInfoRec>> primitiveTypes,
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes)
    {
        if (_crossVersion == null)
        {
            return;
        }

        ConceptMap? cm = _crossVersion.GetSourceDataTypesConceptMap(primitiveTypes, complexTypes);
        if (cm == null)
        {
            return;
        }
        string filename = Path.Combine(outputDir, $"ConceptMap-{cm.Id}.json");

        try
        {
            using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
            using Utf8JsonWriter writer = new(fs, new JsonWriterOptions() { Indented = true, });
            {
                JsonSerializer.Serialize(writer, cm, _firelySerializerOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing {filename}: {ex.Message} {ex.InnerException?.Message}");
        }
    }


    private void WriteStructureMaps(string outputDir, IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> values)
    {
        if (_crossVersion == null)
        {
            return;
        }

        foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in values)
        {
            ConceptMap? cm = _crossVersion.TryGetSourceStructureElementConceptMap(c);
            if (cm == null)
            {
                continue;
            }

            string filename = Path.Combine(outputDir, $"ConceptMap-{cm.Id}.json");

            try
            {
                using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
                using Utf8JsonWriter writer = new(fs, new JsonWriterOptions() { Indented = true, });
                {
                    JsonSerializer.Serialize(writer, cm, _firelySerializerOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing {filename}: {ex.Message} {ex.InnerException?.Message}");
            }
        }
    }

    private void WriteValueSetMaps(string outputDir, IEnumerable<ValueSetComparison> values)
    {
        if (_crossVersion == null)
        {
            return;
        }

        foreach (ValueSetComparison c in values)
        {
            ConceptMap? cm = _crossVersion.GetSourceValueSetConceptMap(c);
            if (cm == null)
            {
                continue;
            }

            string filename = Path.Combine(outputDir, $"ConceptMap-{cm.Id}.json");

            try
            {
                using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
                using Utf8JsonWriter writer = new(fs, new JsonWriterOptions() { Indented = true, });
                {
                    JsonSerializer.Serialize(writer, cm, _firelySerializerOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing {filename}: {ex.Message} {ex.InnerException?.Message}");
            }
        }
    }



    private Dictionary<string, ValueSet> GetValueSets(DefinitionCollection dc, Dictionary<string, ValueSet>? other = null)
    {
        other ??= [];

        Dictionary<string, ValueSet> valueSets = [];

        HashSet<string> mappedSets = _crossVersion?.GetAllReferencedValueSetUrls() ?? [];

        foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            if (_exclusionSet.Contains(unversionedUrl))
            {
                continue;
            }

            // only use the latest version
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // only check bindings if we do not have a map and it was not in the other set
            if ((!mappedSets.Contains(unversionedUrl)) && (!other.ContainsKey(unversionedUrl)))
            {
                IEnumerable<StructureElementCollection> coreBindingsVersioned = dc.CoreBindingsForVs(versionedUrl);
                Hl7.Fhir.Model.BindingStrength? strongestBindingV = dc.StrongestBinding(coreBindingsVersioned);

                IEnumerable<StructureElementCollection> coreBindingsUnversioned = dc.CoreBindingsForVs(unversionedUrl);
                Hl7.Fhir.Model.BindingStrength? strongestBindingU = dc.StrongestBinding(coreBindingsUnversioned);

                if ((strongestBindingV != Hl7.Fhir.Model.BindingStrength.Required) &&
                    (strongestBindingU != Hl7.Fhir.Model.BindingStrength.Required))
                {
                    continue;
                }
            }

            // always expand based on the versioned
            if (!dc.TryExpandVs(versionedUrl, out ValueSet? vs))
            {
                continue;
            }

            valueSets.Add(unversionedUrl, vs);
        }

        return valueSets;
    }

    private void WriteComparisonSummaryTableDict(
        ExportStreamWriter writer,
        IEnumerable<IComparisonRecord> values)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();

        // build summary data
        foreach (IComparisonRecord c in values)
        {
            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        writer.WriteLine("| Status | Count |");
        writer.WriteLine("| ------ | ----- |");
        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            writer.WriteLine($"{status} | {count} |");
        }
        writer.WriteLine();
        writer.WriteLine();
    }

    private void WriteComparisonSummaryTableDict(
        ExportStreamWriter writer,
        Dictionary<string, List<ValueSetComparison>> values)
    {
        Dictionary<string, int> counts = [];

        // build summary data
        foreach (List<ValueSetComparison> vcs in values.Values)
        {
            foreach (ValueSetComparison c in vcs)
            {
                string status = c.GetStatusString();
                if (!counts.TryGetValue(status, out int count))
                {
                    count = 0;
                }

                counts[status] = count + 1;
            }
        }

        writer.WriteLine("| Status | Count |");
        writer.WriteLine("| ------ | ----- |");
        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            writer.WriteLine($"{status} | {count} |");
        }
        writer.WriteLine();
        writer.WriteLine();
    }

    private void WriteComparisonOverview(
        ExportStreamWriter writer,
        string header,
        Dictionary<string, List<ValueSetComparison>> values)
    {
        writer.WriteLine("## " + header);

        WriteComparisonSummaryTableDict(writer, values);

        writer.WriteLine("<details>");
        writer.WriteLine("<summary>Entry details</summary>");

        writer.WriteLine();

        writer.WriteLine("| Name | Source | Dest | Status | Message |");
        writer.WriteLine("| ---- | ------ | ---- | ------ | ------- |");

        foreach (ValueSetComparison c in values.Values.SelectMany(vc => vc).OrderBy(c => c.CompositeName))
        {
            writer.WriteLine($"| {c.CompositeName} | {c.SourceUrl} | {c.TargetUrl} | {c.GetStatusString()} | {c.Message} |");
        }

        writer.WriteLine();

        writer.WriteLine("</details>");
        writer.WriteLine();
    }

    private void WriteComparisonOverview(
        ExportStreamWriter writer,
        string header,
        IEnumerable<IComparisonRecord> values)
    {
        writer.WriteLine("## " + header);

        WriteComparisonSummaryTableDict(writer, values);

        writer.WriteLine("<details>");
        writer.WriteLine("<summary>Entry details</summary>");

        writer.WriteLine();

        writer.WriteLine("| Key | Source | Dest | Status | Message |");
        writer.WriteLine("| --- | ------ | ---- | ------ | ------- |");

        foreach (IComparisonRecord c in values.OrderBy(cr => cr.Key))
        {
            writer.WriteLine("| " + string.Join(" | ", c.GetComparisonRow()) + " |");
        }

        writer.WriteLine();

        writer.WriteLine("</details>");
        writer.WriteLine();
    }

    private void WriteComparisonRecDataTable<T>(ExportStreamWriter writer, IComparisonRecord<T> cRec)
    {
        string[] tdHeader;
        string[][] tdLeft;
        string[][] tdRight;

        switch (cRec)
        {
            case IComparisonRecord<ConceptInfoRec> conceptInfoRecs:
                {
                    tdHeader = ["Side", "System", "Code", "Description"];
                    tdLeft = conceptInfoRecs.Left.Select(ci => (string[])[_leftTableLiteral, ci.System, ci.Code, ci.Description.ForMdTable()]).ToArray();
                    tdRight = conceptInfoRecs.Right.Select(ci => (string[])[_rightTableLiteral, ci.System, ci.Code, ci.Description.ForMdTable()]).ToArray();
                }
                break;

            case IComparisonRecord<ValueSetInfoRec> valueSetInfoRecs:
                {
                    tdHeader = ["Side", "Url", "Name", "Title", "Description"];
                    tdLeft = valueSetInfoRecs.Left
                        .Select(vsi => (string[])[_leftTableLiteral, vsi.Url, vsi.Name, vsi.Title.ForMdTable(), vsi.Description.ForMdTable()])
                        .ToArray();
                    tdRight = valueSetInfoRecs.Right
                        .Select(vsi => (string[])[_rightTableLiteral, vsi.Url, vsi.Name, vsi.Title.ForMdTable(), vsi.Description.ForMdTable()])
                        .ToArray();
                }
                break;

            case IComparisonRecord<ElementTypeInfoRec> elementTypeInfoRecs:
                {
                    tdHeader = ["Side", "Name", "Profiles", "Target Profiles"];
                    tdLeft = elementTypeInfoRecs.Left
                        .Select(eti => (string[])[_leftTableLiteral, eti.Name, string.Join(", ", eti.Profiles), string.Join(", ", eti.TargetProfiles)])
                        .ToArray();
                    tdRight = elementTypeInfoRecs.Right
                        .Select(eti => (string[])[_rightTableLiteral, eti.Name, string.Join(", ", eti.Profiles), string.Join(", ", eti.TargetProfiles)])
                        .ToArray();
                }
                break;

            case IComparisonRecord<ElementInfoRec> elementInfoRecs:
                {
                    tdHeader = ["Side", "Name", "Path", "Short", "Definition", "Card", "Binding"];
                    tdLeft = elementInfoRecs.Left
                        .Select(ei => (string[])[
                            _leftTableLiteral,
                            ei.Name,
                            ei.Path,
                            ei.Short.ForMdTable(),
                            ei.Definition.ForMdTable(),
                            $"{ei.MinCardinality}..{ei.MaxCardinalityString}",
                            $"{ei.ValueSetBindingStrength} {ei.BindingValueSet}"])
                        .ToArray();
                    tdRight = elementInfoRecs.Right
                        .Select(ei => (string[])[
                            _rightTableLiteral,
                            ei.Name,
                            ei.Path,
                            ei.Short.ForMdTable(),
                            ei.Definition.ForMdTable(),
                            $"{ei.MinCardinality}..{ei.MaxCardinalityString}",
                            $"{ei.ValueSetBindingStrength} {ei.BindingValueSet}"])
                        .ToArray();
                }
                break;

            case IComparisonRecord<StructureInfoRec> structureInfoRecs:
                {
                    tdHeader = ["Side", "Name", "Title", "Description", "Snapshot", "Differential"];
                    tdLeft = structureInfoRecs.Left
                        .Select(si => (string[])[
                            _leftTableLiteral,
                            si.Name,
                            si.Title.ForMdTable(),
                            si.Description.ForMdTable(),
                            si.SnapshotCount.ToString(),
                            si.DifferentialCount.ToString()])
                        .ToArray();
                    tdRight = structureInfoRecs.Right
                        .Select(si => (string[])[
                            _rightTableLiteral,
                            si.Name,
                            si.Title.ForMdTable(),
                            si.Description.ForMdTable(),
                            si.SnapshotCount.ToString(),
                            si.DifferentialCount.ToString()])
                        .ToArray();
                }
                break;

            default:
                throw new Exception($"Unknown comparison record type: {cRec.GetType().Name}");
        }

        if (tdLeft.Length == 0)
        {
            tdLeft = [[_leftTableLiteral, .. Enumerable.Repeat("-", tdHeader.Length).ToArray()]];
        }

        if (tdRight.Length == 0)
        {
            tdRight = [[_rightTableLiteral, .. Enumerable.Repeat("-", tdHeader.Length).ToArray()]];
        }

        writer.WriteLine("| " + string.Join(" | ", tdHeader) + " |");
        writer.WriteLine("| " + string.Join(" | ", Enumerable.Repeat("---", tdHeader.Length)) + " |");

        foreach (string[] td in tdLeft)
        {
            writer.WriteLine("| " + string.Join(" | ", td) + " |");
        }

        foreach (string[] td in tdRight)
        {
            writer.WriteLine("| " + string.Join(" | ", td) + " |");
        }

        writer.WriteLine();
    }

    private void WriteComparisonChildDetails(ExportStreamWriter writer, IComparisonRecord cRec, bool inLeft = true, bool inRight = true, bool useDetails = true)
    {
        writer.WriteLine();
        if (useDetails)
        {
            writer.WriteLine("<details>");
            writer.WriteLine("<summary>Content details</summary>");
            writer.WriteLine();
        }

        IEnumerable<string[]> rows = cRec.GetChildComparisonRows(inLeft, inRight);

        writer.WriteLine("| Key | Source | Dest | Status | Message |");
        writer.WriteLine("| --- | ------ | ---- | ------ | ------- |");

        foreach (string[] row in rows)
        {
            writer.WriteLine("| " + string.Join(" | ", row) + " |");
        }

        writer.WriteLine();

        if (useDetails)
        {
            writer.WriteLine("</details>");
            writer.WriteLine();
        }
    }

    //private void WriteComparisonTypeConversions(ExportStreamWriter writer, IComparisonRecord cRec)
    //{
    //    //if ((cRec.AdditionalSerializations == null) ||
    //    //    (cRec.AdditionalSerializations.Count == 0))
    //    //{
    //    //    return;
    //    //}

    //    writer.WriteLine();
    //    writer.WriteLine($"#### Conversions");
    //    writer.WriteLine();
    //    //writer.WriteLine("<details>");
    //    //writer.WriteLine("<summary>Additional Serializations</summary>");
    //    //writer.WriteLine();

    //    writer.WriteLine("| Target | Relationship | Message |");
    //    writer.WriteLine("| ------ | ------------ | ------- |");

    //    foreach (SerializationMapInfo smi in cRec.AdditionalSerializations.Values.OrderBy(smi => smi.Target))
    //    {
    //        writer.WriteLine($"| {smi.Target} | {smi.Relationship?.ToString() ?? "-"} | {smi.Message} |");
    //    }

    //    //writer.WriteLine();
    //    //writer.WriteLine("</details>");
    //    writer.WriteLine();
    //}


    private void WriteComparisonRecResult(ExportStreamWriter writer, IComparisonRecord cRec)
    {
        writer.WriteLine();
        writer.WriteLine($"#### Comparison Result: {cRec.GetStatusString()}");
        writer.WriteLine();
    }

    private void WriteComparisonRecStatusTable(ExportStreamWriter writer, IComparisonRecord cRec, bool inLeft = true, bool inRight = true)
    {
        Dictionary<string, int> counts = cRec.GetStatusCounts(inLeft, inRight);

        writer.WriteLine("| Status | Count |");
        writer.WriteLine("| ------ | ----- |");

        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            writer.WriteLine($"{status} | {count} |");
        }

        writer.WriteLine();
    }

    private void WriteComparisonFile<T>(
        ExportStreamWriter writer,
        string header,
        IComparisonRecord<T> cRec)
    {
        if (!string.IsNullOrEmpty(header))
        {
            writer.WriteLine("## " + header);
        }

        WriteComparisonRecDataTable(writer, cRec);
        WriteComparisonRecResult(writer, cRec);

        if (cRec.ComparisonArtifactType == FhirArtifactClassEnum.PrimitiveType)
        {
            writer.WriteLine();
            writer.WriteLine($"### Primitive type mapping");
            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine("| Source | Target | Relationship | Message |");
            writer.WriteLine("| ------ | ------ | ------------ | ------- |");

            if ((cRec.TypeSerializationInfo == null) || (cRec.TypeSerializationInfo.Count == 0))
            {
                writer.WriteLine($"| {cRec.Key} | {cRec.Key} | {cRec.Relationship?.ToString() ?? "-"} | {cRec.Message}");
            }
            else
            {
                foreach (SerializationMapInfo smi in cRec.TypeSerializationInfo.Values.OrderBy(smi => smi.Target))
                {
                    writer.WriteLine($"| {smi.Source} | {smi.Target} | {smi.Relationship} | {smi.Message} |");
                }
            }
        }
        else
        {
            writer.WriteLine();
            writer.WriteLine($"### Union of {_leftRLiteral} and {_rightRLiteral}");
            writer.WriteLine();
            WriteComparisonRecStatusTable(writer, cRec, inLeft: true, inRight: true);
            WriteComparisonChildDetails(writer, cRec, inLeft: true, inRight: true, useDetails: false);

            writer.WriteLine();
            writer.WriteLine($"### {_leftRLiteral} Detail");
            writer.WriteLine();
            WriteComparisonRecStatusTable(writer, cRec, inLeft: true, inRight: false);
            WriteComparisonChildDetails(writer, cRec, inLeft: true, inRight: false, useDetails: false);

            writer.WriteLine();
            writer.WriteLine($"### {_rightRLiteral} Detail");
            writer.WriteLine();
            WriteComparisonRecStatusTable(writer, cRec, inLeft: false, inRight: true);
            WriteComparisonChildDetails(writer, cRec, inLeft: false, inRight: true, useDetails: false);
        }
    }

    private void WriteComparisonRecDataTable(ExportStreamWriter writer, ValueSetComparison v)
    {
        string[] tdHeader = ["Side", "Url", "Name", "Title", "Description"];
        string[] tdLeft = [_leftTableLiteral, v.SourceUrl, v.SourceName, v.SourceTitle.ForMdTable(), v.SourceDescription.ForMdTable()];
        string[] tdRight = [_rightTableLiteral, v.TargetUrl, v.TargetName, v.TargetTitle.ForMdTable(), v.TargetDescription.ForMdTable()];

        writer.WriteLine("| " + string.Join(" | ", tdHeader) + " |");
        writer.WriteLine("| " + string.Join(" | ", Enumerable.Repeat("---", tdHeader.Length)) + " |");
        writer.WriteLine("| " + string.Join(" | ", tdLeft) + " |");
        writer.WriteLine("| " + string.Join(" | ", tdRight) + " |");

        writer.WriteLine();
    }

    private void WriteComparisonRecResult(ExportStreamWriter writer, string statusString)
    {
        writer.WriteLine();
        writer.WriteLine($"Comparison Result: {statusString}");
        writer.WriteLine();
    }

    private void WriteComparisonRecStatusTable(ExportStreamWriter writer, ValueSetComparison v)
    {
        Dictionary<string, int> counts = [];

        // build summary data
        foreach (ConceptComparison c in v.ConceptComparisons.Values)
        {
            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        writer.WriteLine("| Status | Count |");
        writer.WriteLine("| ------ | ----- |");

        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            writer.WriteLine($"{status} | {count} |");
        }

        writer.WriteLine();
    }

    private void WriteComparisonChildDetails(ExportStreamWriter writer, ValueSetComparison cRec, bool useDetails = true)
    {
        writer.WriteLine();
        if (useDetails)
        {
            writer.WriteLine("<details>");
            writer.WriteLine("<summary>Content details</summary>");
            writer.WriteLine();
        }

        writer.WriteLine("| Key | Source | Dest | Status | Message |");
        writer.WriteLine("| --- | ------ | ---- | ------ | ------- |");

        foreach ((string code, ConceptComparison cc) in cRec.ConceptComparisons)
        {
            if (cc.TargetMappings.Count == 0)
            {
                writer.WriteLine($"| {cc.Source.Code} | {cc.Source.Code} | - | {cc.GetStatusString()} | {cc.Message} |");
                continue;
            }

            foreach (ConceptComparisonDetails cd in cc.TargetMappings)
            {
                writer.WriteLine($"| {cc.Source.Code} | {cc.Source.Code} | {cd.Target.Code} | {cd.GetStatusString()} | {cd.Message} |");
            }
        }

        writer.WriteLine();

        if (useDetails)
        {
            writer.WriteLine("</details>");
            writer.WriteLine();
        }
    }

    private void WriteComparisonFile(
        ExportStreamWriter writer,
        string header,
        ValueSetComparison cRec)
    {
        if (!string.IsNullOrEmpty(header))
        {
            writer.WriteLine("## " + header);
        }

        WriteComparisonRecDataTable(writer, cRec);
        WriteComparisonRecResult(writer, cRec.GetStatusString());

        writer.WriteLine();
        writer.WriteLine("### Mapping details");
        writer.WriteLine();
        WriteComparisonRecStatusTable(writer, cRec);
        WriteComparisonChildDetails(writer, cRec, useDetails: false);

    }


    private ExportStreamWriter CreateMarkdownWriter(string filename, bool writeGenerationHeader = true)
    {
        ExportStreamWriter writer = new(filename);

        if (writeGenerationHeader)
        {
            writer.WriteLine($"Comparison of {_left.MainPackageId}#{_left.MainPackageVersion} and {_right.MainPackageId}#{_right.MainPackageVersion}");
            writer.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
            writer.WriteLine();
        }

        return writer;
    }


    private (CMR initialRelationship, int maxIndex) GetInitialRelationship<T, U>(List<T> lSource, List<U> rSource)
    {
        if (lSource.Count == 1 && rSource.Count == 1)
        {
            return (CMR.Equivalent, 1);
        }

        if (lSource.Count > 1)
        {
            return (CMR.SourceIsNarrowerThanTarget, lSource.Count);
        }

        return (CMR.SourceIsBroaderThanTarget, rSource.Count);
    }


    /// <summary>
    /// Tries to compare the type information for two elements and returns a comparison record.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="typeName">     The name of the element.</param>
    /// <param name="lSource">      The left element type info to compare.</param>
    /// <param name="rSource">      The right element type info to compare.</param>
    /// <param name="c">            [out] The comparison record of the elements.</param>
    /// <param name="artifactClass">The artifact class.</param>
    /// <returns>True if the comparison is successful, false otherwise.</returns>
    private bool TryCompareElementType(
        FhirArtifactClassEnum artifactClass,
        string typeName,
        List<ElementTypeInfoRec> lSource,
        List<ElementTypeInfoRec> rSource,
        [NotNullWhen(true)] out ComparisonRecord<ElementTypeInfoRec>? c)
    {
        if ((lSource.Count == 0) && (rSource.Count == 0))
        {
            c = null;
            return false;
        }

        if ((lSource.Count > 1) && (rSource.Count > 1))
        {
            throw new Exception("Cannot compare multiple source to multiple destination records!");
        }

        if (lSource.Count == 0)
        {
            if (_leftOnlyClasses.Contains(artifactClass))
            {
                c = null;
                return false;
            }

            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = typeName,
                CompositeName = GetName(lSource, rSource),
                Left = [],
                KeyInLeft = false,
                Right = rSource,
                KeyInRight = true,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} added type {typeName}",
            };
            return true;
        }

        if (rSource.Count == 0)
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = typeName,
                CompositeName = GetName(lSource, rSource),
                Left = lSource,
                KeyInLeft = true,
                Right = [],
                KeyInRight = false,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} removed type {typeName}",
            };
            return true;
        }

        bool keyInLeft = lSource.Any(i => i.Name == typeName);
        bool keyInRight = rSource.Any(i => i.Name == typeName);

        if (_leftOnlyClasses.Contains(artifactClass) && !keyInLeft)
        {
            c = null;
            return false;
        }

        // initial relationship is based on the number of comparison records
        (CMR relationship, int maxIndex) = GetInitialRelationship(lSource, rSource);

        List<string> messages = [];

        for (int sourceIndex = 0; sourceIndex < maxIndex; sourceIndex++)
        {
            ElementTypeInfoRec left = lSource.Count == 1 ? lSource[0] : lSource[sourceIndex];
            ElementTypeInfoRec right = rSource.Count == 1 ? rSource[0] : rSource[sourceIndex];

            // once we know the types are the same, we want to determine all differences in profiles and target profiles
            List<string> addedProfiles = [];
            List<string> removedProfiles = [];

            HashSet<string> scratch = right.Profiles.ToHashSet();

            foreach (string lp in left.Profiles)
            {
                if (scratch.Contains(lp))
                {
                    scratch.Remove(lp);
                    continue;
                }

                removedProfiles.Add(lp);
            }

            addedProfiles.AddRange(scratch);

            List<string> addedTargets = [];
            List<string> removedTargets = [];

            scratch = right.TargetProfiles.ToHashSet();

            foreach (string lp in left.TargetProfiles)
            {
                if (scratch.Contains(lp))
                {
                    scratch.Remove(lp);
                    continue;
                }

                removedTargets.Add(lp);
            }

            addedTargets.AddRange(scratch);

            if (addedProfiles.Any())
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                messages.Add($"{right.Name} added profiles: {string.Join(", ", addedProfiles)}");
            }

            if (removedProfiles.Any())
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                messages.Add($"{right.Name} removed profiles: {string.Join(", ", removedProfiles)}");
            }

            if (addedTargets.Any())
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                messages.Add($"{right.Name} added target profiles: {string.Join(", ", addedTargets)}");
            }

            if (removedTargets.Any())
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                messages.Add($"{right.Name} removed target profiles: {string.Join(", ", removedTargets)}");
            }
        }

        string message;

        if (relationship == CMR.Equivalent)
        {
            message = keyInLeft
                ? $"{_rightRLiteral} type {rSource[0].Name} is equivalent to the {_leftRLiteral} type {typeName}"
                : $"{_rightRLiteral} new type {typeName} is equivalent to the {_leftRLiteral} type {lSource[0].Name}";
        }
        else if (messages.Count == 0)
        {
            message = keyInLeft
                ? $"{_leftRLiteral} type {typeName} maps as: {relationship} for {_rightRLiteral}"
                : $"{_rightRLiteral} new type {typeName} maps as: {relationship} for {_leftRLiteral}";
        }
        else
        {
            message = keyInLeft
                ? $"{_leftRLiteral} type {typeName} maps as: {relationship} for {_rightRLiteral} because {string.Join(" and ", messages)}"
                : $"{_rightRLiteral} new type {typeName} maps as: {relationship} for {_leftRLiteral} because {string.Join(" and ", messages)}";

        }

        c = new()
        {
            ComparisonArtifactType = artifactClass,
            Key = typeName,
            CompositeName = GetName(lSource, rSource),
            Left = lSource,
            KeyInLeft = keyInLeft,
            Right = rSource,
            KeyInRight = keyInRight,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
        };
        return true;
    }

    private string GetName(List<ElementTypeInfoRec> l, List<ElementTypeInfoRec> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        return
            (l.Count == 0 ? _leftRLiteral : $"{_leftRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _rightRLiteral : $"{_rightRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}");
    }

    private CMR ApplyRelationship(CMR existing, CMR? change) => existing switch
    {
        CMR.Equivalent => change ?? CMR.Equivalent,
        CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : existing,
        CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
        CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
        CMR.NotRelatedTo => change ?? existing,
        _ => change ?? existing,
    };

    private string GetName(List<ValueSetInfoRec> l, List<ValueSetInfoRec> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        return
            (l.Count == 0 ? _leftRLiteral : $"{_leftRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _rightRLiteral : $"{_rightRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}");
    }

    private bool TryGetVsComparison(string sourceUrl, string targetUrl, [NotNullWhen(true)] out ValueSetComparison? valueSetComparison)
    {
        if (!_vsComparisons.TryGetValue(sourceUrl, out List<ValueSetComparison>? vcs))
        {
            valueSetComparison = null;
            return false;
        }

        valueSetComparison = vcs.FirstOrDefault(vc => vc.TargetUrl == targetUrl);
        return valueSetComparison != null;
    }


    private bool TryCompareElement(
        FhirArtifactClassEnum artifactClass,
        string edPath,
        List<ElementInfoRec> lSource,
        List<ElementInfoRec> rSource,
        Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> typeComparison,
        CMR? mappedRelationship,
        [NotNullWhen(true)] out ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>? c)
    {
        if ((lSource.Count == 0) && (rSource.Count == 0))
        {
            c = null;
            return false;
        }

        if ((lSource.Count > 1) && (rSource.Count > 1))
        {
            throw new Exception("Cannot compare multiple source to multiple destination records!");
        }

        if (lSource.Count == 0)
        {
            if (_leftOnlyClasses.Contains(artifactClass))
            {
                c = null;
                return false;
            }

            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = edPath,
                CompositeName = GetName(lSource, rSource),
                Left = [],
                KeyInLeft = false,
                Right = rSource,
                KeyInRight = true,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} added element {edPath}",
                Children = typeComparison,
            };
            return true;
        }

        if (rSource.Count == 0)
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = edPath,
                CompositeName = GetName(lSource, rSource),
                Left = lSource,
                KeyInLeft = true,
                Right = [],
                KeyInRight = false,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} removed element {edPath}",
                Children = typeComparison,
            };
            return true;
        }

        bool keyInLeft = lSource.Any(i => i.Path == edPath);
        bool keyInRight = rSource.Any(i => i.Path == edPath);

        if (_leftOnlyClasses.Contains(artifactClass) && !keyInLeft)
        {
            c = null;
            return false;
        }

        // initial relationship is based on the number of comparison records
        (CMR relationship, int maxIndex) = GetInitialRelationship(lSource, rSource);

        // check for an existing relationship
        if (mappedRelationship != null)
        {
            relationship = mappedRelationship.Value;
        }

        List<string> messages = [];

        for (int sourceIndex = 0; sourceIndex < maxIndex; sourceIndex++)
        {
            ElementInfoRec left = lSource.Count == 1 ? lSource[0] : lSource[sourceIndex];
            ElementInfoRec right = rSource.Count == 1 ? rSource[0] : rSource[sourceIndex];

            // check for optional becoming mandatory
            if ((left.MinCardinality == 0) && (right.MinCardinality != 0))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                messages.Add($"{right.Name} made the element mandatory");
            }

            // check for source allowing fewer than destination requires
            if (left.MinCardinality < right.MinCardinality)
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                messages.Add($"{right.Name} increased the minimum cardinality from {left.MinCardinality} to {right.MinCardinality}");
            }

            // check for element being constrained out
            if ((left.MaxCardinality != 0) && (right.MaxCardinality == 0))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                messages.Add($"{right.Name} constrained the element out (max cardinality of 0)");
            }

            // check for changing from scalar to array
            if ((left.MaxCardinality == 1) && (right.MaxCardinality != 1))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                messages.Add($"{right.Name} changed from scalar to array (max cardinality from {left.MaxCardinalityString} to {right.MaxCardinalityString})");
            }

            // check for changing from array to scalar
            if ((left.MaxCardinality != 1) && (right.MaxCardinality == 1))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                messages.Add($"{right.Name} changed from array to scalar (max cardinality from {left.MaxCardinalityString} to {right.MaxCardinalityString})");
            }

            // check for source allowing more than destination allows
            if ((right.MaxCardinality != -1) &&
                (left.MaxCardinality > right.MaxCardinality))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                messages.Add($"{right.Name} allows more repetitions (max cardinality from {left.MaxCardinalityString} to {right.MaxCardinalityString})");
            }

            // check to see if there was not a required binding and now there is
            if ((left.ValueSetBindingStrength != null) || (right.ValueSetBindingStrength != null))
            {
                if ((left.ValueSetBindingStrength != BindingStrength.Required) && (right.ValueSetBindingStrength == BindingStrength.Required))
                {
                    relationship = ApplyRelationship(relationship, CMR.RelatedTo);

                    if (left.ValueSetBindingStrength == null)
                    {
                        messages.Add($"{right.Name} added a required binding to {right.BindingValueSet}");
                    }
                    else
                    {
                        messages.Add($"{right.Name} made the binding required (from {left.ValueSetBindingStrength}) for {right.BindingValueSet}");
                    }
                }
                else if (left.ValueSetBindingStrength != right.ValueSetBindingStrength)
                {
                    relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                    if (left.ValueSetBindingStrength == null)
                    {
                        messages.Add($"{right.Name} added a binding requirement - {right.ValueSetBindingStrength} {right.BindingValueSet}");
                    }
                    else if (right.ValueSetBindingStrength == null)
                    {
                        messages.Add($"{right.Name} removed a binding requirement - {left.ValueSetBindingStrength} {left.BindingValueSet}");
                    }
                    else
                    {
                        messages.Add($"{right.Name} changed the binding strength from {left.ValueSetBindingStrength} to {right.ValueSetBindingStrength}");
                    }
                }

                // check to see if we need to lookup a binding comparison
                if ((left.ValueSetBindingStrength == BindingStrength.Required) && (right.ValueSetBindingStrength == BindingStrength.Required))
                {
                    // TODO(ginoc): For sanity right now, we assume that the value sets are from the matching releases
                    // at some point, we need to check specific versions in case there are explicit references

                    string unversionedLeft = left.BindingValueSet.Split('|')[0];
                    string unversionedRight = right.BindingValueSet.Split('|')[0];

                    // if the types are code, we only need to compare codes
                    if (typeComparison.ContainsKey("code"))
                    {
                        // look for the value set comparison
                        if (TryGetVsComparison(unversionedLeft, unversionedRight, out ValueSetComparison? boundVsInfo))
                        {
                            // we are okay with equivalent and narrower
                            if (boundVsInfo.Relationship == CMR.Equivalent ||
                                boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget)
                            {
                                relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                                messages.Add($"{right.Name} has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet} ({boundVsInfo.Relationship})");
                            }

                            // check to see if the codes are the same but the systems are different (ok in codes)
                            else if (boundVsInfo.ConceptComparisons.Values.All(cc => cc.TargetMappings.Any(tc => tc.Target.Code == cc.Source.Code)))
                            {
                                relationship = ApplyRelationship(relationship, CMR.Equivalent);
                                messages.Add($"{right.Name} has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet} (codes match, though systems are different)");
                            }
                            else
                            {
                                relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                                messages.Add($"{right.Name} has INCOMPATIBLE required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                            }
                        }
                        else if (_exclusionSet.Contains(unversionedRight))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{right.Name} using {unversionedRight} is exempted and assumed equivalent");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"({right.Name} failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                        }
                    }

                    // check for any non-code types (need to match system)
                    if (typeComparison.Any(t => t.Key != "code"))
                    {
                        // check for same value set (non-code type)
                        if (TryGetVsComparison(unversionedLeft, unversionedRight, out ValueSetComparison? boundVsInfo))
                        {
                            if ((boundVsInfo.Relationship == CMR.Equivalent) ||
                                (boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget))
                            {
                                // we are okay with equivalent and narrower
                                relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                                messages.Add($"{right.Name} has compatible required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet} ({boundVsInfo.Relationship})");
                            }
                            else
                            {
                                relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                                messages.Add($"{right.Name} has INCOMPATIBLE required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                            }
                        }
                        else if (_exclusionSet.Contains(unversionedRight))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{right.Name} using {unversionedRight} is exempted and assumed equivalent");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"({right.Name} failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                        }
                    }
                }
            }

            // process our type comparisons and promote messages
            foreach (ComparisonRecord<ElementTypeInfoRec> tc in typeComparison.Values)
            {
                // skip equivalent types
                if (tc.Relationship == CMR.Equivalent)
                {
                    continue;
                }

                relationship = ApplyRelationship(relationship, tc.Relationship);
                messages.Add($"{right.Name} has change due to type change: {tc.Message}");
            }
        }

        // build our message
        //string message = $"{_rightRLiteral} element {edPath} is {relationship}" +
        //    (messages.Count == 0 ? string.Empty : (" because " + string.Join(" and ", messages.Distinct())));

        string message;

        if (relationship == CMR.Equivalent)
        {
            message = keyInLeft
                ? $"{_rightRLiteral} element {rSource[0].Path} is equivalent to the {_leftRLiteral} element {edPath}"
                : $"{_rightRLiteral} new element {edPath} is equivalent to the {_leftRLiteral} element {lSource[0].Path}";
        }
        else if (messages.Count == 0)
        {
            message = keyInLeft
                ? $"{_leftRLiteral} element {edPath} maps as: {relationship} for {_rightRLiteral}"
                : $"{_rightRLiteral} new element {edPath} maps as: {relationship} for {_leftRLiteral}";
        }
        else
        {
            message = keyInLeft
                ? $"{_leftRLiteral} element {edPath} maps as: {relationship} for {_rightRLiteral} because {string.Join(" and ", messages)}"
                : $"{_rightRLiteral} new element {edPath} maps as: {relationship} for {_leftRLiteral} because {string.Join(" and ", messages)}";
        }

        // return our info
        c = new()
        {
            ComparisonArtifactType = artifactClass,
            Key = edPath,
            CompositeName = GetName(lSource, rSource),
            Left = lSource,
            KeyInLeft = keyInLeft,
            Right = rSource,
            KeyInRight = keyInRight,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            Children = typeComparison,
        };
        return true;
    }

    private string GetName(List<ElementInfoRec> l, List<ElementInfoRec> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        return
            (l.Count == 0 ? _leftRLiteral : $"{_leftRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _rightRLiteral : $"{_rightRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}");
    }


    private bool TryCompareStructure(
        FhirArtifactClassEnum artifactClass,
        string sdName,
        List<(StructureDefinition sd, StructureInfoRec si)> lSource,
        List<(StructureDefinition sd, StructureInfoRec si)> rSource,
        Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> elementComparison,
        [NotNullWhen(true)] out ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>? c)
    {
        if ((lSource.Count == 0) && (rSource.Count == 0))
        {
            c = null;
            return false;
        }

        if ((lSource.Count > 1) && (rSource.Count > 1))
        {
            throw new Exception("Cannot compare multiple source to multiple destination records!");
        }

        if (lSource.Count == 0)
        {
            if (_leftOnlyClasses.Contains(artifactClass))
            {
                c = null;
                return false;
            }

            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = sdName,
                CompositeName = GetName(lSource, rSource),
                Left = [],
                KeyInLeft = false,
                Right = rSource.Select(s => s.si).ToList(),
                KeyInRight = true,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} added {sdName}",
                Children = elementComparison,
            };
            return true;
        }

        if (rSource.Count == 0)
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = sdName,
                CompositeName = GetName(lSource, rSource),
                Left = lSource.Select(s => s.si).ToList(),
                KeyInLeft = true,
                Right = [],
                KeyInRight = false,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} removed {sdName}",
                Children = elementComparison,
            };
            return true;
        }

        bool keyInLeft = lSource.Any(s => s.si.Name == sdName);
        bool keyInRight = rSource.Any(s => s.si.Name == sdName);

        if (_leftOnlyClasses.Contains(artifactClass) && !keyInLeft)
        {
            c = null;
            return false;
        }

        // check for all elements being the same
        if (elementComparison.Values.All(ec => ec.Relationship == CMR.Equivalent))
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = sdName,
                CompositeName = GetName(lSource, rSource),
                Left = lSource.Select(s => s.si).ToList(),
                KeyInLeft = keyInLeft,
                Right = rSource.Select(s => s.si).ToList(),
                KeyInRight = keyInRight,
                NamedMatch = true,
                Relationship = CMR.Equivalent,
                Message = $"{_rightRLiteral}:{sdName} is equivalent",
                Children = elementComparison,
            };
            return true;
        }

        // initial relationship is based on the number of comparison records
        (CMR relationship, _) = GetInitialRelationship(lSource, rSource);

        foreach (ComparisonRecord<ElementInfoRec, ElementTypeInfoRec> ec in elementComparison.Values)
        {
            if (ec.Relationship == CMR.Equivalent)
            {
                continue;
            }

            if (ec.Left.Count == 0)
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                continue;
            }

            if (ec.Right.Count == 0)
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                continue;
            }

            relationship = ApplyRelationship(relationship, ec.Relationship);
        }

        //string message = relationship switch
        //{
        //    CMR.Equivalent => $"{_rightRLiteral}:{sdName} is equivalent",
        //    CMR.RelatedTo => $"{_rightRLiteral}:{sdName} is related to {_leftRLiteral}:{sdName} (see elements for details)",
        //    CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral}:{sdName} subsumes {_leftRLiteral}:{sdName}",
        //    CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral}:{sdName} is subsumed by {_leftRLiteral}:{sdName}",
        //    _ => $"{_rightRLiteral}:{sdName} is related to {_leftRLiteral}:{sdName} (see elements for details)",
        //};


        string message;

        if (keyInLeft)
        {
            message = relationship switch
            {
                CMR.Equivalent => $"{_rightRLiteral} structure {rSource[0].si.Name} is equivalent to the {_leftRLiteral} structure {sdName}",
                CMR.RelatedTo => $"{_rightRLiteral} structure {rSource[0].si.Name} is related to {_leftRLiteral} structure {sdName} (see elements for details)",
                CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral} structure {rSource[0].si.Name} subsumes {_leftRLiteral} structure {sdName}",
                CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral} structure {rSource[0].si.Name} is subsumed by {_leftRLiteral} structure {sdName}",
                _ => $"{_rightRLiteral} structure {rSource[0].si.Name} is related to {_leftRLiteral} structure {sdName} (see elements for details)",
            };
        }
        else
        {
            message = relationship switch
            {
                CMR.Equivalent => $"{_rightRLiteral} new structure {sdName} is equivalent to the {_leftRLiteral} structure {lSource[0].si.Name}",
                CMR.RelatedTo => $"{_rightRLiteral} new structure {sdName} is related to {_leftRLiteral} structure {lSource[0].si.Name} (see elements for details)",
                CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral} new structure {sdName} subsumes {_leftRLiteral} structure {lSource[0].si.Name}",
                CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral} new structure {sdName} is subsumed by {_leftRLiteral} structure {lSource[0].si.Name}",
                _ => $"{_rightRLiteral} new structure {sdName} is related to {_leftRLiteral} structure {lSource[0].si.Name} (see elements for details)",
            };
        }

        c = new()
        {
            ComparisonArtifactType = artifactClass,
            Key = sdName,
            CompositeName = GetName(lSource, rSource),
            Left = lSource.Select(s => s.si).ToList(),
            KeyInLeft = keyInLeft,
            Right = rSource.Select(s => s.si).ToList(),
            KeyInRight = keyInRight,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            Children = elementComparison,
        };
        return true;
    }

    private string GetName(List<StructureInfoRec> l, List<StructureInfoRec> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        if (l.Count == 0)
        {
            return $"{_rightRLiteral}_{r[0].Name.ForName()}";
        }

        if (r.Count == 0)
        {
            return $"{_leftRLiteral}_{l[0].Name.ForName()}";
        }

        if (l.Count == 1 && r.Count == 1)
        {
            return $"{_leftRLiteral}_{l[0].Name.ForName()}_{_rightRLiteral}_{r[0].Name.ForName()}";
        }

        return
            $"{_leftRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}" +
            $"_{_rightRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}";
    }

    private string GetName(List<(StructureDefinition sd, StructureInfoRec si)> l, List<(StructureDefinition sd, StructureInfoRec si)> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        if (l.Count == 0)
        {
            return $"{_rightRLiteral}_{r[0].si.Name.ForName()}";
        }

        if (r.Count == 0)
        {
            return $"{_leftRLiteral}_{l[0].si.Name.ForName()}";
        }

        if (l.Count == 1 && r.Count == 1)
        {
            return $"{_leftRLiteral}_{l[0].si.Name.ForName()}_{_rightRLiteral}_{r[0].si.Name.ForName()}";
        }

        return
            $"{_leftRLiteral}_{string.Join('_', l.Select(s => s.si.Name.ForName()).Order())}" +
            $"_{_rightRLiteral}_{string.Join('_', r.Select(s => s.si.Name.ForName()).Order())}";
    }

    private string GetName(List<(StructureDefinition sd, StructureInfoRec si)> left, List<(StructureDefinition sd, StructureInfoRec si, CMR? r)> right)
    {
        if (left.Count == 0 && right.Count == 0)
        {
            return string.Empty;
        }

        if (left.Count == 0)
        {
            return $"{_rightRLiteral}_{right[0].si.Name.ForName()}";
        }

        if (right.Count == 0)
        {
            return $"{_leftRLiteral}_{left[0].si.Name.ForName()}";
        }

        if (left.Count == 1 && right.Count == 1)
        {
            return $"{_leftRLiteral}_{left[0].si.Name.ForName()}_{_rightRLiteral}_{right[0].si.Name.ForName()}";
        }

        int equivalentIndex = right.FindIndex(t => t.r == CMR.Equivalent);

        if (equivalentIndex == -1)
        {
            return $"{_rightRLiteral}_{string.Join('_', right.Select(s => s.si.Name.ForName()).Order())}";

            //return
            //    $"{_leftRLiteral}_{string.Join('_', left.Select(s => s.si.Name.ForName()).Order())}" +
            //    $"_{_rightRLiteral}_{string.Join('_', right.Select(s => s.si.Name.ForName()).Order())}";
        }

        string eqName = right[equivalentIndex].si.Name;

        if (right.Count == 1)
        {
            return
                $"{_leftRLiteral}_{string.Join('_', left.Select(s => s.si.Name.ForName()).Order())}" +
                $"_{_rightRLiteral}_{eqName.ForName()}";
        }

        // we want equivalent listed first
        return
            $"{_leftRLiteral}_{string.Join('_', left.Select(s => s.si.Name.ForName()).Order())}" +
            $"_{_rightRLiteral}_{eqName.ForName()}_{string.Join('_', right.Where(s => s.si.Name != eqName).Select(s => s.si.Name.ForName()).Order())}";
    }

    /// <summary>Compare a set of  value sets.</summary>
    /// <param name="sourceValueSets">Sets the source value belongs to.</param>
    /// <param name="targetValueSets">Sets the target value belongs to.</param>
    /// <returns>A dictionary indexed by source value set URL, containing a list of mappings that exist.  Most commonly a single mapping.</returns>
    private Dictionary<string, List<ValueSetComparison>> CompareValueSets(
        IReadOnlyDictionary<string, ValueSet> sourceValueSets,
        IReadOnlyDictionary<string, ValueSet> targetValueSets)
    {
        Dictionary<string, List<ValueSetComparison>> results = [];

        // loop over the source value sets
        foreach ((string sourceUrl, ValueSet sourceVs) in sourceValueSets)
        {
            HashSet<string> testedTargetUrls = [];

            if (!results.TryGetValue(sourceUrl, out List<ValueSetComparison>? comparisons))
            {
                comparisons = [];
                results.Add(sourceUrl, comparisons);
            }

            // check to see if we have any maps for this source value set
            if ((_crossVersion != null) &&
                _crossVersion.TryGetMapsForVs(sourceVs.Url, out List<ConceptMap>? conceptMaps))
            {
                // traverse our list of concept maps
                foreach (ConceptMap cm in conceptMaps)
                {
                    // check to see if we have a target value set
                    if ((cm.TargetScope is Canonical targetCanonical) &&
                        targetValueSets.TryGetValue(targetCanonical.Uri ?? string.Empty, out ValueSet? mappedTargetVs))
                    {
                        testedTargetUrls.Add(targetCanonical.Uri!);

                        // test this mapping
                        if (TryCompareValueSetConcepts(sourceVs, mappedTargetVs, cm, out ValueSetComparison? mappedComparison))
                        {
                            comparisons.Add(mappedComparison);
                        }
                    }
                }
            }

            // make sure that we tested direct source -> target if it exists
            if (!testedTargetUrls.Contains(sourceUrl) &&
                targetValueSets.TryGetValue(sourceUrl, out ValueSet? targetVs))
            {
                if (TryCompareValueSetConcepts(sourceVs, targetVs, null, out ValueSetComparison? directComparison))
                {
                    comparisons.Add(directComparison);
                }
            }
        }

        return results;
    }

    private bool TryCompareValueSetConcepts(
        ValueSet sourceVs,
        ValueSet targetVs,
        ConceptMap? vsConceptMap,
        [NotNullWhen(true)] out ValueSetComparison? comparison)
    {
        Dictionary<string, FhirConcept> sourceConcepts = sourceVs.cgGetFlatConcepts(_left).ToDictionary(c => c.Code);
        Dictionary<string, FhirConcept> targetConcepts = targetVs.cgGetFlatConcepts(_right).ToDictionary(c => c.Code);

        Dictionary<string, ConceptComparison> conceptComparisons = [];

        Dictionary<string, Dictionary<string, List<ConceptMap.SourceElementComponent>>> mapsByTargetSystemBySourceSystemAndCode = [];

        // build a mapping lookup if we have one
        if (vsConceptMap != null)
        {
            // traverse the groups in our map - each group represents a system
            foreach (ConceptMap.GroupComponent cmGroup in vsConceptMap.Group)
            {
                string groupSourceSystem = cmGroup.Source ?? UnversionedUrl(sourceVs.Url);
                string groupTargetSystem = cmGroup.Target ?? UnversionedUrl(targetVs.Url);

                // add all the elements from this group to our lookup
                foreach (ConceptMap.SourceElementComponent cmElement in cmGroup.Element)
                {
                    string sourceKey = $"{groupSourceSystem}#{cmElement.Code}";

                    if (!mapsByTargetSystemBySourceSystemAndCode.TryGetValue(sourceKey, out Dictionary<string, List<ConceptMap.SourceElementComponent>>? targetMaps))
                    {
                        targetMaps = [];
                        mapsByTargetSystemBySourceSystemAndCode.Add(sourceKey, targetMaps);
                    }

                    if (!targetMaps.TryGetValue(groupTargetSystem, out List<ConceptMap.SourceElementComponent>? elementMaps))
                    {
                        elementMaps = [];
                        targetMaps.Add(groupTargetSystem, elementMaps);
                    }

                    elementMaps.Add(cmElement);
                }
            }
        }

        // traverse the source concepts to do comparison tests
        foreach (FhirConcept sourceConcept in sourceConcepts.Values)
        {
            ConceptInfoRec sourceConceptInfo = GetInfo(sourceConcept);
            List<ConceptComparisonDetails> conceptComparisonDetails = [];

            string sourceKey = $"{sourceConcept.System}#{sourceConcept.Code}";

            // check to see if we have a map for this source concept
            if (mapsByTargetSystemBySourceSystemAndCode.TryGetValue(sourceKey, out Dictionary<string, List<ConceptMap.SourceElementComponent>>? targetMaps))
            {
                foreach ((string targetSystem, List<ConceptMap.SourceElementComponent> mapSourceElements) in targetMaps)
                {
                    foreach (ConceptMap.SourceElementComponent mapSourceElement in mapSourceElements)
                    {
                        foreach (ConceptMap.TargetElementComponent mapTargetElement in mapSourceElement.Target)
                        {
                            CMR? relationship = GetDefaultRelationship(mapTargetElement, mapSourceElement.Target);
                            string message = string.IsNullOrEmpty(mapTargetElement.Comment)
                                ? MessageForConceptRelationship(relationship, mapSourceElement, mapTargetElement)
                                : mapTargetElement.Comment;

                            if (targetConcepts.TryGetValue(mapTargetElement.Code, out FhirConcept? targetConceptFromMap))
                            {
                                conceptComparisonDetails.Add(new()
                                {
                                    Target = GetInfo(targetConceptFromMap),
                                    Relationship = relationship,
                                    Message = message,
                                    IsPreferred = conceptComparisonDetails.Count == 0,
                                });
                            }
                            else
                            {
                                conceptComparisonDetails.Add(new()
                                {
                                    Target = new()
                                    {
                                        System = targetSystem,
                                        Code = mapTargetElement.Code,
                                        Description = string.Empty,
                                    },
                                    Relationship = relationship,
                                    Message = message,
                                    IsPreferred = conceptComparisonDetails.Count == 0,
                                });
                            }
                        }
                    }
                }
            }
            else if (targetConcepts.TryGetValue(sourceConcept.Code, out FhirConcept? targetConcept))
            {
                // create a 'default' comparison state
                conceptComparisonDetails.Add(new()
                {
                    Target = GetInfo(targetConcept),
                    Relationship = CMR.Equivalent,
                    Message = "Name equality with no explicit maps is assumed to be equivalent",
                    IsPreferred = true,
                });
            }

            ConceptComparison cc = new()
            {
                Source = sourceConceptInfo,
                TargetMappings = conceptComparisonDetails,
                Relationship = RelationshipForDetails(conceptComparisonDetails),
                Message = MessageForDetails(conceptComparisonDetails, sourceConcept, targetVs),
            };

            conceptComparisons.Add(sourceConcept.Code, cc);
        }

        string sourceName = sourceVs.Name.ToPascalCase();
        string targetName = targetVs.Name.ToPascalCase();

        CMR? vsRelationship = RelationshipForComparisons(conceptComparisons);

        comparison = new()
        {
            SourceUrl = UnversionedUrl(sourceVs.Url),
            SourceName = sourceName,
            SourceTitle = sourceVs.Title,
            SourceDescription = sourceVs.Description,
            TargetUrl = UnversionedUrl(targetVs.Url),
            TargetName = targetName,
            TargetTitle = targetVs.Title,
            TargetDescription = targetVs.Description,
            CompositeName = $"{_leftRLiteral}-{sourceName}-{_rightRLiteral}-{targetName}",
            ConceptComparisons = conceptComparisons,
            Relationship = vsRelationship,
            Message = MessageForComparisonRelationship(vsRelationship, sourceVs, targetVs),
        };
        return true;

        CMR? GetDefaultRelationship(ConceptMap.TargetElementComponent mapTargetElement, List<ConceptMap.TargetElementComponent> targets) => targets.Count switch
        {
            0 => mapTargetElement.Relationship ?? CMR.NotRelatedTo,
            1 => mapTargetElement.Relationship ?? CMR.Equivalent,
            _ => ApplyRelationship(mapTargetElement.Relationship ?? CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        };

        CMR? RelationshipForDetails(List<ConceptComparisonDetails> details) => details.Count switch
        {
            0 => CMR.NotRelatedTo,
            1 => CMR.Equivalent,
            _ => CMR.SourceIsBroaderThanTarget,
        };

        CMR? RelationshipForComparisons(Dictionary<string, ConceptComparison> comparisons) => comparisons.Count switch
        {
            0 => CMR.NotRelatedTo,
            1 => comparisons.First().Value.Relationship,
            _ => comparisons.Any(kvp => kvp.Value.Relationship != CMR.Equivalent)
                ? (comparisons.Any(kvp => !IsEquivalentOrBroader(kvp.Value)) ? CMR.RelatedTo : CMR.SourceIsBroaderThanTarget)
                : CMR.Equivalent,
        };

        bool IsEquivalentOrBroader(ConceptComparison cc) =>
            cc.Relationship == CMR.Equivalent ||
            cc.Relationship == CMR.SourceIsBroaderThanTarget ||
            cc.TargetMappings.Count == 0;

        string MessageForConceptRelationship(CMR? r, ConceptMap.SourceElementComponent se, ConceptMap.TargetElementComponent te) => r switch
        {
            null => $"{_leftRLiteral} `{se.Code}` has no mapping into {_rightRLiteral} {targetVs.Url}.",
            CMR.Equivalent => $"{_leftRLiteral} `{se.Code}` is equivalent to {_rightRLiteral} `{te.Code}`.",
            CMR.SourceIsBroaderThanTarget => $"{_leftRLiteral} `{se.Code}` is broader than {_rightRLiteral} {te.Code} and is compatible for conversion. `{se.Code}` maps to {string.Join(" and ", se.Target.Select(t => $"`{t.Code}`"))}.",
            _ => $"{_leftRLiteral} `{se.Code}` maps as {r} to the target {_rightRLiteral} `{te.Code}`.",
        };

        string MessageForDetails(List<ConceptComparisonDetails> details, FhirConcept sourceConcept, ValueSet targetVs) => details.Count switch
        {
            0 => $"{_leftRLiteral} `{sourceConcept.Code}` does not appear in the target and has no mapping for {targetVs.Url}.",
            1 => details[0].Message,
            _ => $"{_leftRLiteral} `{sourceConcept.Code}` maps to multiple concepts in {targetVs.Url}.",
        };

        string MessageForComparisonRelationship(CMR? r, ValueSet sourceVs, ValueSet targetVs) => r switch
        {
            null => $"There is no mapping from {_leftRLiteral} {sourceVs.Url} to {_rightRLiteral} {targetVs.Url}.",
            CMR.Equivalent => $"{_leftRLiteral} {sourceVs.Url} is equivalent to {_rightRLiteral} {targetVs.Url}.",
            CMR.SourceIsBroaderThanTarget => $"{_leftRLiteral} {sourceVs.Url} is broader than {_rightRLiteral} {targetVs.Url} and is compatible for conversion.",
            _ => $"{_leftRLiteral} {sourceVs.Url} maps as {r} to {_rightRLiteral} {targetVs.Url}.",
        };
    }

    private string UnversionedUrl(string url) => url.Contains('|') ? url.Split('|')[0] : url;


    private Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> CompareElementTypes(
        FhirArtifactClassEnum artifactClass,
        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> leftInput,
        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> rightInput)
    {
        Dictionary<string, ElementTypeInfoRec> left = leftInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, ElementTypeInfoRec> right = rightInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        // add our comparisons
        foreach (string typeName in keys)
        {
            List<ElementTypeInfoRec> leftInfoSource = left.TryGetValue(typeName, out ElementTypeInfoRec? leftInfo) ? [leftInfo] : [];
            List<ElementTypeInfoRec> rightInfoSource = right.TryGetValue(typeName, out ElementTypeInfoRec? rightInfo) ? [rightInfo] : [];

            if (TryCompareElementType(artifactClass, typeName, leftInfoSource, rightInfoSource, out ComparisonRecord<ElementTypeInfoRec>? c))
            {
                comparison.Add(typeName, c);
            }
        }

        return comparison;
    }

    private Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> CompareElements(
        FhirArtifactClassEnum artifactClass,
        List<(StructureDefinition sd, StructureInfoRec si)> leftSource,
        List<(StructureDefinition sd, StructureInfoRec si)> rightSource,
        Dictionary<string, ElementConceptMapTarget> elementMappings)
    {
        Dictionary<string, ElementDefinition> leftElements = [];
        Dictionary<string, ElementDefinition> rightElements = [];

        // need to build up the set of elements for each side
        foreach ((StructureDefinition leftSd, StructureInfoRec leftSi) in leftSource)
        {
            foreach (ElementDefinition ed in leftSd.cgElements())
            {
                leftElements.Add(ed.Path, ed);
            }
        }

        foreach ((StructureDefinition rightSd, StructureInfoRec rightSi) in rightSource)
        {
            foreach (ElementDefinition ed in rightSd.cgElements())
            {
                rightElements.Add(ed.Path, ed);
            }
        }

        Dictionary<string, ElementInfoRec> leftInfoDict = leftElements.ToDictionary(kvp => kvp.Value.Path, kvp => GetInfo(kvp.Value));
        Dictionary<string, ElementInfoRec> rightInfoDict = rightElements.ToDictionary(kvp => kvp.Value.Path, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> comparison = [];

        IEnumerable<string> keys = leftInfoDict.Keys.Union(rightInfoDict.Keys).Distinct();

        // add our matches
        foreach (string edPath in keys)
        {
            string rightPath = edPath;

            IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> leftTypes;
            IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> rightTypes;

            if (leftElements.TryGetValue(edPath, out ElementDefinition? leftEd))
            {
                leftTypes = leftEd.cgTypes();
            }
            else
            {
                leftTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
            }

            CMR? mappedRelationship = null;

            // check for an element mapping
            if (elementMappings.TryGetValue(edPath, out ElementConceptMapTarget mapped))
            {
                if (rightElements.TryGetValue(mapped.target, out ElementDefinition? rightEd))
                {
                    rightTypes = rightEd.cgTypes();
                    mappedRelationship = mapped.relationship;
                    rightPath = mapped.target;
                }
                else
                {
                    rightTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
                }
            }
            else if (rightElements.TryGetValue(edPath, out ElementDefinition? rightEd))
            {
                rightTypes = rightEd.cgTypes();
            }
            else
            {
                rightTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
            }

            List<ElementInfoRec> leftInfoSource = leftInfoDict.TryGetValue(edPath, out ElementInfoRec? leftInfo) ? [leftInfo] : [];
            List<ElementInfoRec> rightInfoSource = rightInfoDict.TryGetValue(rightPath, out ElementInfoRec? rightInfo) ? [rightInfo] : [];

            // perform type comparison
            Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> typeComparison = CompareElementTypes(artifactClass, leftTypes, rightTypes);

            if (TryCompareElement(artifactClass, edPath, leftInfoSource, rightInfoSource, typeComparison, mappedRelationship, out ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>? c))
            {
                comparison.Add(edPath, c);
            }
        }

        return comparison;
    }

    //private Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> Compare(
    //    FhirArtifactClassEnum artifactClass,
    //    IReadOnlyDictionary<string, ElementDefinition> leftDict,
    //    IReadOnlyDictionary<string, ElementDefinition> rightDict)
    //{
    //    Dictionary<string, ElementInfoRec> leftInfoDict = leftDict.ToDictionary(kvp => kvp.Value.Path, kvp => GetInfo(kvp.Value));
    //    Dictionary<string, ElementInfoRec> rightInfoDict = rightDict.ToDictionary(kvp => kvp.Value.Path, kvp => GetInfo(kvp.Value));

    //    Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> comparison = [];

    //    IEnumerable<string> keys = leftInfoDict.Keys.Union(rightInfoDict.Keys).Distinct();

    //    // add our matches
    //    foreach (string edPath in keys)
    //    {
    //        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> leftTypes;
    //        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> rightTypes;

    //        if (leftDict.TryGetValue(edPath, out ElementDefinition? leftEd))
    //        {
    //            leftTypes = leftEd.cgTypes();
    //        }
    //        else
    //        {
    //            leftTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
    //        }

    //        if (rightDict.TryGetValue(edPath, out ElementDefinition? rightEd))
    //        {
    //            rightTypes = rightEd.cgTypes();
    //        }
    //        else
    //        {
    //            rightTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
    //        }

    //        List<ElementInfoRec> leftInfoSource = leftInfoDict.TryGetValue(edPath, out ElementInfoRec? leftInfo) ? [leftInfo] : [];
    //        List<ElementInfoRec> rightInfoSource = rightInfoDict.TryGetValue(edPath, out ElementInfoRec? rightInfo) ? [rightInfo] : [];

    //        // perform type comparison
    //        Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> typeComparison = Compare(artifactClass, leftTypes, rightTypes);

    //        if (TryCompare(artifactClass, edPath, leftInfoSource, rightInfoSource, typeComparison, out ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>? c))
    //        {
    //            comparison.Add(edPath, c);
    //        }
    //    }

    //    return comparison;
    //}

    private Dictionary<string, ComparisonRecord<StructureInfoRec>> ComparePrimitives(
        FhirArtifactClassEnum artifactClass,
        IReadOnlyDictionary<string, StructureDefinition> leftInput,
        IReadOnlyDictionary<string, StructureDefinition> rightInput)
    {
        if (artifactClass != FhirArtifactClassEnum.PrimitiveType)
        {
            throw new Exception("Can only compare primitive types with this method");
        }

        Dictionary<string, StructureInfoRec> left = leftInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, StructureInfoRec> right = rightInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<StructureInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        HashSet<string> usedCompositeNames = [];

        // add our matches
        foreach (string sdName in keys)
        {
            List<(StructureDefinition sd, StructureInfoRec si)> leftSource;  // = left.TryGetValue(sdName, out StructureInfoRec? leftInfo) ? [leftInfo] : [];
            List<(StructureDefinition sd, StructureInfoRec si, CMR? r)> rightSource; // = right.TryGetValue(sdName, out StructureInfoRec? rightInfo) ? [rightInfo] : [];

            ConceptMap? cm = artifactClass switch
            {
                FhirArtifactClassEnum.PrimitiveType => _crossVersion?.DataTypeMap,
                _ => null
            };

            // prefer using a map if we have one
            if (cm != null)
            {
                HashSet<string> usedSourceNames = [];
                HashSet<string> usedTargetNames = [];

                leftSource = [];
                rightSource = [];

                // check to see if the source element has a map
                ConceptMap.SourceElementComponent? sourceMap = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Code == sdName).FirstOrDefault();

                // if we have a mapping from the current source, we want to use the target mappings
                if (sourceMap != null)
                {
                    // pull information about our mapped source concept
                    if (!left.TryGetValue(sdName, out StructureInfoRec? mapSourceInfo))
                    {
                        Console.WriteLine($"Removing {sdName} from the concept map, it does not actually exist...");
                        cm!.Group[0].Element.Remove(sourceMap);

                        //throw new Exception($"Structure {sdName} is mapped as a source but not defined in the left set");
                    }
                    else
                    {
                        leftSource.Add((leftInput[sdName], mapSourceInfo));
                        usedSourceNames.Add(sdName);

                        // traverse the map targets to pull target information
                        foreach (ConceptMap.TargetElementComponent te in sourceMap.Target)
                        {
                            // check if already added
                            if (usedTargetNames.Contains(te.Code))
                            {
                                continue;
                            }

                            if (!right.TryGetValue(te.Code, out StructureInfoRec? mappedTargetInfo))
                            {
                                throw new Exception($"Structure {te.Code} is mapped as a target but not defined in right set");
                            }

                            rightSource.Add((rightInput[te.Code], mappedTargetInfo, te.Relationship));
                            usedTargetNames.Add(te.Code);
                        }
                    }
                }

                // if we did not find a source mapping from the source, still add as a target structure if it is in the right set
                if ((usedTargetNames.Count == 0) &&
                    right.TryGetValue(sdName, out StructureInfoRec? rightStructureInfo))
                {
                    rightSource.Add((rightInput[sdName], rightStructureInfo, leftSource.Count == 0 ? null : CMR.Equivalent));
                    usedTargetNames.Add(sdName);
                }

                // only pull target mappings if we are in a left-hand (source) comparison for primitives
                if (!left.ContainsKey(sdName))
                {
                    // also pull the list of target mappings to see if this code is mapped *from* any other source
                    List<ConceptMap.SourceElementComponent> targetMaps = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Target.Any(t => usedTargetNames.Contains(t.Code)))?.ToList() ?? [];

                    // traverse all mappings that this target appears in
                    foreach (ConceptMap.SourceElementComponent mapElement in targetMaps)
                    {
                        // check if this has already been added
                        if (!usedSourceNames.Contains(mapElement.Code))
                        {
                            // pull information about our mapped source concept
                            if (!left.TryGetValue(mapElement.Code, out StructureInfoRec? mapSourceInfo))
                            {
                                throw new Exception($"Structure {mapElement.Code} is mapped as a source but not defined in the left set");
                            }

                            leftSource.Add((leftInput[mapElement.Code], mapSourceInfo));
                            usedSourceNames.Add(mapElement.Code);
                        }

                        // traverse the map targets to pull target information
                        foreach (ConceptMap.TargetElementComponent te in mapElement.Target)
                        {
                            if (usedTargetNames.Contains(te.Code))
                            {
                                continue;
                            }

                            if (!right.TryGetValue(te.Code, out StructureInfoRec? mappedTargetInfo))
                            {
                                throw new Exception($"Structure {te.Code} is mapped as a target but not defined in right set");
                            }

                            rightSource.Add((rightInput[te.Code], mappedTargetInfo, te.Relationship));
                            usedTargetNames.Add(te.Code);
                        }
                    }
                }
            }
            else
            {
                // without a map, just try to get the matching source and destination codes
                leftSource = left.TryGetValue(sdName, out StructureInfoRec? leftInfo) ? [(leftInput[sdName], leftInfo)] : [];
                rightSource = right.TryGetValue(sdName, out StructureInfoRec? rightInfo) ? [(rightInput[sdName], rightInfo, null)] : [];
            }

            if (TryComparePrimitive(sdName, leftSource, rightSource, out ComparisonRecord<StructureInfoRec>? c))
            {
                if (!usedCompositeNames.Contains(c.CompositeName))
                {
                    comparison.Add(sdName, c);
                    usedCompositeNames.Add(c.CompositeName);
                }

                foreach (SerializationMapInfo smi in c.TypeSerializationInfo?.Values ?? Enumerable.Empty<SerializationMapInfo>())
                {
                    string trName = $"{smi.Source}_{smi.Target}";
                    if (!_typeRelationships.ContainsKey(trName))
                    {
                        _typeRelationships.Add(trName, smi.Relationship!);
                    }
                }
            }
        }

        return comparison;
    }

    private bool TryComparePrimitive(
        string sdName,
        List<(StructureDefinition sd, StructureInfoRec si)> lSource,
        List<(StructureDefinition sd, StructureInfoRec si, CMR? r)> rSource,
        [NotNullWhen(true)] out ComparisonRecord<StructureInfoRec>? c)
    {
        if ((lSource.Count == 0) && (rSource.Count == 0))
        {
            c = null;
            return false;
        }

        bool keyInLeft = lSource.Any(s => s.si.Name == sdName);
        bool keyInRight = rSource.Any(s => s.si.Name == sdName);

        if ((lSource.Count > 1) && (rSource.Count > 1))
        {
            // if we are in primitives, we can filter out multiple source to multiple destination records
            if (keyInRight && !keyInLeft)
            {
                rSource = rSource.Where(rSource => rSource.si.Name == sdName).ToList();
            }
            else
            {
                throw new Exception("Cannot compare multiple source to multiple destination records!");
            }
        }

        FhirArtifactClassEnum artifactClass = FhirArtifactClassEnum.PrimitiveType;

        if (lSource.Count == 0)
        {
            if (_leftOnlyClasses.Contains(artifactClass))
            {
                c = null;
                return false;
            }

            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = sdName,
                CompositeName = GetName(lSource, rSource),
                Left = [],
                KeyInLeft = keyInLeft,
                Right = rSource.Select(s => s.si).ToList(),
                KeyInRight = keyInRight,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} added {sdName}",
            };
            return true;
        }

        if (rSource.Count == 0)
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = sdName,
                CompositeName = GetName(lSource, rSource),
                Left = lSource.Select(s => s.si).ToList(),
                KeyInLeft = keyInLeft,
                Right = [],
                KeyInRight = keyInRight,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} removed {sdName}",
            };
            return true;
        }

        if (_leftOnlyClasses.Contains(artifactClass) && !keyInLeft)
        {
            c = null;
            return false;
        }

        (CMR relationship, int maxIndex) = GetInitialRelationship(lSource, rSource);

        if ((rSource.Count == 1) &&
            (rSource[0].r != null))
        {
            relationship = rSource[0].r!.Value;
        }

        string message = string.Empty;

        Dictionary<string, SerializationMapInfo> serializations = [];

        // primitive types are always compared 1:1
        for (int sourceIndex = 0; sourceIndex < maxIndex; sourceIndex++)
        {
            (StructureDefinition leftSd, StructureInfoRec leftSi) = lSource.Count == 1 ? lSource[0] : lSource[sourceIndex];
            (StructureDefinition rightSd, StructureInfoRec rightSi, CMR? rightRelationship) = rSource.Count == 1 ? rSource[0] : rSource[sourceIndex];

            // check for being the same type
            if (leftSi.Name == rightSi.Name)
            {
                // check for extra mapping record that is not a match
                if (sdName != leftSd.Name)
                {
                    continue;
                }

                // for primitives, a name-match needs to represent equivalence for sanity elsewhere
                relationship = CMR.Equivalent;
                message = $"{_rightRLiteral} primitive {rightSi.Name} is equivalent to the {_leftRLiteral} primitive {leftSi.Name}";

                // also add a serialization info
                serializations.Add(leftSi.Name, new()
                {
                    Source = leftSi.Name,
                    Target = rightSi.Name,
                    Relationship = rightRelationship ?? CMR.SourceIsBroaderThanTarget,
                    Message = $"{_rightRLiteral} primitive {rightSi.Name} is equivalent to the {_leftRLiteral} primitive {leftSi.Name}",
                });

                continue;
            }

            if (rightRelationship == null)
            {
                // when adding types, the source is broader than whatever it is serializing to
                rightRelationship = CMR.SourceIsBroaderThanTarget;
            }

            // add a serialization map if we have another type here
            SerializationMapInfo serializationInfo = new()
            {
                Source = leftSi.Name,
                Target = rightSi.Name,
                Relationship = rightRelationship ?? CMR.SourceIsBroaderThanTarget,
                Message = $"{_rightRLiteral} new type {rightSi.Name} has a serialization mapping from {_leftRLiteral} type {leftSi.Name}",
            };

            if (keyInLeft)
            {
                serializations.Add(rightSi.Name, serializationInfo);
            }
            else if (keyInRight)
            {
                serializations.Add(leftSi.Name, serializationInfo);
            }
        }

        if (string.IsNullOrEmpty(message))
        {
            if (keyInLeft)
            {
                message = relationship switch
                {
                    CMR.Equivalent => $"{_rightRLiteral} type {rSource[0].si.Name} is equivalent to the {_leftRLiteral} type {sdName}",
                    CMR.RelatedTo => $"{_rightRLiteral} type {rSource[0].si.Name} is related to {_leftRLiteral} type {sdName}",
                    CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral} type {rSource[0].si.Name} subsumes {_leftRLiteral} type {sdName}",
                    CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral} type {rSource[0].si.Name} is subsumed by {_leftRLiteral} type {sdName}",
                    _ => $"{_rightRLiteral} type {rSource[0].si.Name} is related to {_leftRLiteral} type {sdName}",
                };
            }
            else
            {
                message = relationship switch
                {
                    CMR.Equivalent => $"{_rightRLiteral} new type {sdName} is equivalent to the {_leftRLiteral} type {lSource[0].si.Name}",
                    CMR.RelatedTo => $"{_rightRLiteral} new type {sdName} is related to {_leftRLiteral} type {lSource[0].si.Name}",
                    CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral} new type {sdName} subsumes {_leftRLiteral} type {lSource[0].si.Name}",
                    CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral} new type {sdName} is subsumed by {_leftRLiteral} type {lSource[0].si.Name}",
                    _ => $"{_rightRLiteral} new type {sdName} is related to {_leftRLiteral} type {lSource[0].si.Name}",
                };
            }
        }

        c = new()
        {
            ComparisonArtifactType = artifactClass,
            Key = sdName,
            CompositeName = GetName(lSource, rSource),
            Left = lSource.Select(s => s.si).ToList(),
            KeyInLeft = keyInLeft,
            Right = rSource.Select(s => s.si).ToList(),
            KeyInRight = keyInRight,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            TypeSerializationInfo = serializations.Count == 0 ? null : serializations,
        };
        return true;
    }


    private Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> CompareStructures(
        FhirArtifactClassEnum artifactClass,
        IReadOnlyDictionary<string, StructureDefinition> leftInput,
        IReadOnlyDictionary<string, StructureDefinition> rightInput)
    {
        if (artifactClass == FhirArtifactClassEnum.PrimitiveType)
        {
            throw new Exception("Primitive types cannot be compared as generic structures!");
        }

        Dictionary<string, StructureInfoRec> left = leftInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, StructureInfoRec> right = rightInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        HashSet<string> usedCompositeNames = [];

        // traverse all of the structures we know about
        foreach (string sdName in keys)
        {
            List<(StructureDefinition sd, StructureInfoRec si)> leftSource;  // = left.TryGetValue(sdName, out StructureInfoRec? leftInfo) ? [leftInfo] : [];
            List<(StructureDefinition sd, StructureInfoRec si)> rightSource; // = right.TryGetValue(sdName, out StructureInfoRec? rightInfo) ? [rightInfo] : [];

            ConceptMap? cm = artifactClass switch
            {
                FhirArtifactClassEnum.ComplexType => _crossVersion?.DataTypeMap,
                FhirArtifactClassEnum.Resource => _crossVersion?.ResourceTypeMap,
                _ => null
            };

            // prefer using a map if we have one
            if (cm != null)
            {
                HashSet<string> usedSourceNames = [];
                HashSet<string> usedTargetNames = [];

                leftSource = [];
                rightSource = [];

                // check to see if the source element has a map
                ConceptMap.SourceElementComponent? sourceMap = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Code == sdName).FirstOrDefault();

                // if we have a mapping from the current source, we want to use the target mappings
                if (sourceMap != null)
                {
                    // pull information about our mapped source concept
                    if (!left.TryGetValue(sdName, out StructureInfoRec? mapSourceInfo))
                    {
                        Console.WriteLine($"Removing {sdName} from the concept map, it does not actually exist...");
                        cm!.Group[0].Element.Remove(sourceMap);
                        
                        //throw new Exception($"Structure {sdName} is mapped as a source but not defined in the left set");
                    }
                    else
                    {
                        leftSource.Add((leftInput[sdName], mapSourceInfo));
                        usedSourceNames.Add(sdName);

                        // traverse the map targets to pull target information
                        foreach (ConceptMap.TargetElementComponent te in sourceMap.Target)
                        {
                            // check if already added
                            if (usedTargetNames.Contains(te.Code))
                            {
                                continue;
                            }

                            if (!right.TryGetValue(te.Code, out StructureInfoRec? mappedTargetInfo))
                            {
                                // check if this is an invalid 1:1 mapping
                                if (sourceMap.Target.Count == 1)
                                {
                                    Console.WriteLine($"Removing {sdName} from the concept map, it is a map for a target that does not actually exist...");
                                    cm!.Group[0].Element.Remove(sourceMap);

                                    // do not attempt to use this map
                                    leftSource = [];
                                    usedSourceNames.Clear();
                                    mapSourceInfo = null;
                                }
                                else
                                {
                                    throw new Exception($"Structure {te.Code} is mapped as a target but not defined in right set");
                                }
                            }
                            else
                            {
                                rightSource.Add((rightInput[te.Code], mappedTargetInfo));
                                usedTargetNames.Add(te.Code);
                            }
                        }
                    }
                }

                // if we did not find a source mapping from the source, still add as a target structure if it is in the right set
                if ((usedTargetNames.Count == 0) &&
                    right.TryGetValue(sdName, out StructureInfoRec? rightStructureInfo))
                {
                    rightSource.Add((rightInput[sdName], rightStructureInfo));
                    usedTargetNames.Add(sdName);
                }

                // also pull the list of target mappings to see if this code is mapped *from* any other source
                List<ConceptMap.SourceElementComponent> targetMaps = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Target.Any(t => usedTargetNames.Contains(t.Code)))?.ToList() ?? [];

                // traverse all mappings that this target appears in
                foreach (ConceptMap.SourceElementComponent mapElement in targetMaps)
                {
                    // check if this has already been added
                    if (!usedSourceNames.Contains(mapElement.Code))
                    {
                        // pull information about our mapped source concept
                        if (!left.TryGetValue(mapElement.Code, out StructureInfoRec? mapSourceInfo))
                        {
                            throw new Exception($"Structure {mapElement.Code} is mapped as a source but not defined in the left set");
                        }

                        leftSource.Add((leftInput[mapElement.Code], mapSourceInfo));
                        usedSourceNames.Add(mapElement.Code);
                    }

                    // traverse the map targets to pull target information
                    foreach (ConceptMap.TargetElementComponent te in mapElement.Target)
                    {
                        if (usedTargetNames.Contains(te.Code))
                        {
                            continue;
                        }

                        if (!right.TryGetValue(te.Code, out StructureInfoRec? mappedTargetInfo))
                        {
                            throw new Exception($"Structure {te.Code} is mapped as a target but not defined in right set");
                        }

                        rightSource.Add((rightInput[te.Code], mappedTargetInfo));
                        usedTargetNames.Add(te.Code);
                    }
                }
            }
            else
            {
                // without a map, just try to get the matching source and destination codes
                leftSource = left.TryGetValue(sdName, out StructureInfoRec? leftInfo) ? [(leftInput[sdName], leftInfo)] : [];
                rightSource = right.TryGetValue(sdName, out StructureInfoRec? rightInfo) ? [(rightInput[sdName], rightInfo)] : [];
            }

            //// if we have relationships from a map, we want to look for extra relationships that we can filter
            //if ((targetRelationships.Count != 0) &&
            //    targetRelationships.Any(kvp => IsEquivalenceMapping(kvp.Value)) &&
            //    targetRelationships.Any(kvp => !IsEquivalenceMapping(kvp.Value)))
            //{
            //    // remove the non-equivalence mappings
            //    foreach (string key in targetRelationships.Where(kvp => !IsEquivalenceMapping(kvp.Value)).Select(kvp => kvp.Key))
            //    {
            //        targetRelationships.Remove(key);
            //    }
            //}

            // for now, we have a global element map.  Once we have resource-specific ones, load those per structure in the loop
            ConceptMap? elementMap = _crossVersion?.ElementTypeMap(sdName);

            Dictionary<string, ElementConceptMapTarget> elementMappings = [];

            if (elementMap != null)
            {
                // if this appears more than once, we need to evaluate for each source record
                foreach (ConceptMap.SourceElementComponent sourceElement in elementMap.Group.FirstOrDefault()?.Element ?? [])
                {
                    foreach (ConceptMap.TargetElementComponent targetElement in sourceElement.Target)
                    {
                        elementMappings.Add(sourceElement.Code, new(targetElement.Code, targetElement.Relationship ?? CMR.RelatedTo));
                    }
                }
            }

            // perform element comparison
            Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> elementComparison = CompareElements(artifactClass, leftSource, rightSource, elementMappings);

            if (TryCompareStructure(artifactClass, sdName, leftSource, rightSource, elementComparison, out ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>? c))
            {
                if (!usedCompositeNames.Contains(c.CompositeName))
                {
                    comparison.Add(sdName, c);
                    usedCompositeNames.Add(c.CompositeName);
                }

                // add type mappings
                foreach (StructureInfoRec leftRec in c.Left)
                {
                    foreach (StructureInfoRec rightRec in c.Right)
                    {
                        string trName = $"{leftRec.Name}_{rightRec.Name}";
                        if (!_typeRelationships.ContainsKey(trName))
                        {
                            _typeRelationships.Add(trName, c.Relationship ?? CMR.RelatedTo);
                        }
                    }
                }
            }
        }

        return comparison;
    }

    private record struct ElementConceptMapTarget(string target, CMR relationship);

    private bool IsEquivalenceMapping(CMR? relationship) => relationship switch
    {
        CMR.Equivalent => true,
        CMR.SourceIsNarrowerThanTarget => true,
        CMR.SourceIsBroaderThanTarget => true,
        CMR.RelatedTo => false,
        CMR.NotRelatedTo => false,
        _ => false,
    };

    private ConceptInfoRec GetInfo(FhirConcept c)
    {
        return new()
        {
            System = c.System,
            Code = c.Code,
            Description = c.Display,
        };
    }

    private ValueSetInfoRec GetInfo(ValueSet vs)
    {
        return new()
        {
            Url = vs.Url,
            Name = vs.Name,
            Title = vs.Title,
            Description = vs.Description,
            ConceptCount = 0,       // will be filled in later
        };
    }

    private ElementTypeInfoRec GetInfo(ElementDefinition.TypeRefComponent tr)
    {
        return new()
        {
            Name = tr.cgName(),
            Profiles = tr.Profile?.ToList() ?? [],
            TargetProfiles = tr.TargetProfile?.ToList() ?? [],
        };
    }

    private ElementInfoRec GetInfo(ElementDefinition ed)
    {
        return new()
        {
            Name = ed.cgName(),
            Path = ed.Path,
            Short = ed.Short,
            Definition = ed.Definition,
            MinCardinality = ed.cgCardinalityMin(),
            MaxCardinality = ed.cgCardinalityMax(),
            MaxCardinalityString = ed.Max,
            ValueSetBindingStrength = ed.Binding?.Strength,
            BindingValueSet = ed.Binding?.ValueSet ?? string.Empty,
        };
    }

    private StructureInfoRec GetInfo(StructureDefinition sd)
    {
        return new StructureInfoRec()
        {
            Name = sd.Name,
            Url = sd.Url,
            Title = sd.Title,
            Description = sd.Description,
            Purpose = sd.Purpose,
            SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
            DifferentialCount = sd.Differential?.Element.Count ?? 0,
        };
    }

    private class OllamaQuery
    {
        /// <summary>Required: the model name.</summary>
        [JsonPropertyName("model")]
        public required string Model { get; set; }

        /// <summary>The prompt to generate a response for.</summary>
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }

        /// <summary>Optional list of base64-encoded images (for multimodal models such as llava)</summary>
        [JsonPropertyName("images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Images { get; set; } = null;

        /// <summary>The format to return a response in. Currently the only accepted value is json</summary>
        [JsonPropertyName("format")]
        public string Format { get; } = "json";

        /// <summary>Additional model parameters listed in the documentation for the Model file such as temperature</summary>
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Options { get; set; } = null;

        /// <summary>System message to (overrides what is defined in the Model file).</summary>
        [JsonPropertyName("system")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? System { get; set; } = null;

        /// <summary>The prompt template to use (overrides what is defined in the Model file)</summary>
        [JsonPropertyName("template")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Template { get; set; } = null;

        /// <summary>The context parameter returned from a previous request to /generate, this can be used to keep a short conversational memory</summary>
        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? Context { get; set; } = null;

        /// <summary>If false the response will be returned as a single response object, rather than a stream of objects.</summary>
        [JsonPropertyName("stream")]
        public required bool Stream { get; set; }

        /// <summary>If true no formatting will be applied to the prompt. You may choose to use the raw parameter if you are specifying a full templated prompt in your request to the API</summary>
        [JsonPropertyName("raw")]
        public bool Raw { get; set; } = false;

        /// <summary>Controls how long the model will stay loaded into memory following the request (default: 5m)</summary>
        [JsonPropertyName("keep_alive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? KeepAliveDuration { get; set; } = null;
    }


    public class OllamaResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("done")]
        public bool Done { get; set; } = false;

        [JsonPropertyName("context")]
        public int[] Context { get; set; } = [];

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; } = 0;

        [JsonPropertyName("load_duration")]
        public int LoadDuration { get; set; } = 0;

        [JsonPropertyName("prompt_eval_count")]
        public int PromptEvalCount { get; set; } = 0;

        [JsonPropertyName("prompt_eval_duration")]
        public int PromptEvalDuration { get; set; } = 0;

        [JsonPropertyName("eval_count")]
        public int EvalCount { get; set; } = 0;

        [JsonPropertyName("eval_duration")]
        public long EvalDuration { get; set; } = 0;
    }


    private bool TryAskOllama(
        StructureInfoRec known,
        IEnumerable<StructureInfoRec> possibleMatches,
        [NotNullWhen(true)] out StructureInfoRec? guess,
        [NotNullWhen(true)] out int? confidence)
    {
        if (_httpClient == null)
        {
            guess = null;
            confidence = 0;
            return false;
        }

        try
        {
            string system = """
                You are a modeling expert comparing definitions between model structures.
                When presented with a model definition and a set of possible matches, you are asked to select the most likely match.
                You are to respond with ONLY the json representation of the model that you believe is the best match.
                The model definition is a structure with the following properties: Name, Title, Description, and Purpose.
                """;

            //string prompt =
            //    $"Given the following definition:\n{System.Web.HttpUtility.JavaScriptStringEncode(JsonSerializer.Serialize(known))}\n" +
            //    $"Please select the most likely match from the following definitions:\n{System.Web.HttpUtility.JavaScriptStringEncode(JsonSerializer.Serialize(possibleMatches))}." +
            //    $" Respond in JSON with only the definition in the same format.";

            string prompt = $$$"""
                Given the following definition:
                {{{JsonSerializer.Serialize(known)}}}
                Please select the most likely match from the following definitions:
                {{{JsonSerializer.Serialize(possibleMatches)}}}
                """;

                //$"Given the following definition:\n{JsonSerializer.Serialize(known)}\n" +
                //$"Please select the most likely match from the following definitions:\n{JsonSerializer.Serialize(possibleMatches)}." +
                //$" Respond in JSON with only the definition in the same format.";

            // build our prompt
            OllamaQuery query = new()
            {
                Model = _config.OllamaModel,
                System = system,
                Prompt = prompt,
                Stream = false,
            };

            Console.WriteLine("--------------");
            Console.WriteLine($"query:\n{JsonSerializer.Serialize(query)}");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json"),
                RequestUri = _ollamaUri,
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json"),
                    },
                },
            };

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            System.Net.HttpStatusCode statusCode = response.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Request to {request.RequestUri} failed! Returned: {response.StatusCode}");
                guess = null;
                confidence = 0;
                return false;
            }

            string json = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"Request to {request.RequestUri} returned empty body!");
                guess = null;
                confidence = 0;
                return false;
            }

            Console.WriteLine($"response:\n{json}");
            Console.WriteLine("--------------");


            OllamaResponse? olResponse = JsonSerializer.Deserialize<OllamaResponse>(json);

            if (olResponse == null )
            {
                Console.WriteLine($"Failed to deserialize response: {json}");
                guess = null;
                confidence = 0;
                return false;
            }

            if (string.IsNullOrEmpty(olResponse.Response))
            {
                Console.WriteLine($"Ollama response is empty: {json}");
                guess = null;
                confidence = 0;
                return false;
            }

            guess = JsonSerializer.Deserialize<StructureInfoRec>(olResponse.Response);
            if (guess == null)
            {
                Console.WriteLine($"Failed to deserialize response property: {olResponse.Response}");
                guess = null;
                confidence = 0;
                return false;
            }

            confidence = -1;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryAskOllama <<< caught: {ex.Message}{(string.IsNullOrEmpty(ex.InnerException?.Message) ? string.Empty : ex.InnerException.Message)}");
        }

        guess = null;
        confidence = 0;
        return false;
    }
}
