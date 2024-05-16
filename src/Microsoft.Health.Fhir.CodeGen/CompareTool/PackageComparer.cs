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

    private DefinitionCollection _source;
    private DefinitionCollection _target;

    private CrossVersionMapCollection? _crossVersion = null;

    private HashSet<FhirArtifactClassEnum> _leftOnlyClasses = [];

    private Dictionary<string, CMR> _typeRelationships = [];


    private string _sourceRLiteral;
    private string _targetRLiteral;

    private const string _sourceTableLiteral = "Source";
    private const string _targetTableLiteral = "Target";

    private readonly JsonSerializerOptions _firelySerializerOptions;

    private ConfigCompare _config;

    private HttpClient? _httpClient = null;
    private Uri? _ollamaUri = null;

    private Dictionary<string, List<ValueSetComparison>> _vsComparisons = [];

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

    public PackageComparer(ConfigCompare config, IFhirPackageClient cache, DefinitionCollection source, DefinitionCollection target)
    {
        _config = config;
        _cache = cache;
        _source = source;
        _target = target;

        _sourceRLiteral = source.FhirSequence.ToRLiteral();
        _targetRLiteral = target.FhirSequence.ToRLiteral();

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
            $"Comparing {_source.MainPackageId}#{_source.MainPackageVersion}" +
            $" and {_target.MainPackageId}#{_target.MainPackageVersion}");

        // check for loading cross-version maps
        if (!string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            _crossVersion = new(_cache, _source, _target);

            if (!_crossVersion.TryLoadConceptMaps(_config.CrossVersionMapSourcePath))
            {
                throw new Exception("Failed to load requested cross-version maps");
            }
        }

        // check if we are saving cross version maps and did not load any
        if ((_crossVersion == null) && (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None))
        {
            // create our cross-version map collection
            _crossVersion = new(_cache, _source, _target);
        }

        string outputDir = string.IsNullOrEmpty(_config.CrossVersionMapDestinationPath)
            ? Path.Combine(_config.OutputDirectory, $"{_sourceRLiteral}_{_targetRLiteral}")
            : Path.Combine(_config.CrossVersionMapDestinationPath, $"{_sourceRLiteral}_{_targetRLiteral}");

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
        Dictionary<string, ValueSet> vsLeft = GetValueSets(_source);
        _vsComparisons = CompareValueSets(vsLeft, GetValueSets(_target));
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

        Dictionary<string, List<PrimitiveTypeComparison>> primitiveComparisons = ComparePrimitives(_source.PrimitiveTypesByName, _target.PrimitiveTypesByName);
        _crossVersion?.UpdateDataTypeMap(primitiveComparisons);
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Primitive Types", primitiveComparisons);

            string mdSubDir = Path.Combine(pageDir, "PrimitiveTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<PrimitiveTypeComparison> cs in primitiveComparisons.Values)
            {
                foreach (PrimitiveTypeComparison c in cs)
                {
                    string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                    using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                    {
                        WriteComparisonFile(writer, string.Empty, c);
                    }
                }
            }
        }

        //Dictionary<string, ComparisonRecord<StructureInfoRec>> oldPrimitives = ComparePrimitives(FhirArtifactClassEnum.PrimitiveType, _source.PrimitiveTypesByName, _target.PrimitiveTypesByName);
        //Dictionary<string, ComparisonRecord<StructureInfoRec>> primitives = ComparePrimitives(FhirArtifactClassEnum.PrimitiveType, _source.PrimitiveTypesByName, _target.PrimitiveTypesByName);
        //if (mdWriter != null)
        //{
        //    WriteComparisonOverview(mdWriter, "Primitive Types", primitives.Values);

        //    string mdSubDir = Path.Combine(pageDir, "PrimitiveTypes");
        //    if (!Directory.Exists(mdSubDir))
        //    {
        //        Directory.CreateDirectory(mdSubDir);
        //    }

        //    foreach (ComparisonRecord<StructureInfoRec> c in primitives.Values)
        //    {
        //        string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

        //        using ExportStreamWriter writer = CreateMarkdownWriter(filename);
        //        {
        //            WriteComparisonFile(writer, string.Empty, c);
        //        }
        //    }
        //}

        Dictionary<string, List<StructureComparison>> complexTypeComparisons = CompareStructures(_source.ComplexTypesByName, _target.ComplexTypesByName);
        _crossVersion?.UpdateDataTypeMap(complexTypeComparisons);
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Complex Types", complexTypeComparisons);

            string mdSubDir = Path.Combine(pageDir, "ComplexTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<StructureComparison> vcs in complexTypeComparisons.Values)
            {
                foreach (StructureComparison c in vcs)
                {
                    string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                    using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                    {
                        WriteComparisonFile(writer, string.Empty, c);
                    }
                }
            }

            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                string mapSubDir = Path.Combine(conceptMapDir, "ComplexTypes");
                if (!Directory.Exists(mapSubDir))
                {
                    Directory.CreateDirectory(mapSubDir);
                }

                WriteStructureMaps(mapSubDir, complexTypeComparisons.Values.SelectMany(l => l.Select(s => s)));
            }

            // write out the data type map
            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                WriteDataTypeMap(conceptMapDir, primitiveComparisons, complexTypeComparisons);
            }
        }

        //Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes = CompareStructures(FhirArtifactClassEnum.ComplexType, _source.ComplexTypesByName, _target.ComplexTypesByName);
        //if (mdWriter != null)
        //{
        //    WriteComparisonOverview(mdWriter, "Complex Types", complexTypes.Values);

        //    string mdSubDir = Path.Combine(pageDir, "ComplexTypes");
        //    if (!Directory.Exists(mdSubDir))
        //    {
        //        Directory.CreateDirectory(mdSubDir);
        //    }

        //    foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in complexTypes.Values)
        //    {
        //        string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

        //        using ExportStreamWriter writer = CreateMarkdownWriter(filename);
        //        {
        //            WriteComparisonFile(writer, string.Empty, c);
        //        }
        //    }

        //    if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
        //    {
        //        string mapSubDir = Path.Combine(conceptMapDir, "ComplexTypes");
        //        if (!Directory.Exists(mapSubDir))
        //        {
        //            Directory.CreateDirectory(mapSubDir);
        //        }

        //        WriteStructureMaps(mapSubDir, complexTypes.Values);
        //    }

        //    // write out the data type map
        //    if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
        //    {
        //        WriteDataTypeMap(conceptMapDir, oldPrimitives.Values, complexTypes.Values);
        //    }
        //}

        Dictionary<string, List<StructureComparison>> resources = CompareStructures(_source.ResourcesByName, _target.ResourcesByName);
        if (mdWriter != null)
        {
            WriteComparisonOverview(mdWriter, "Resources", resources);

            string mdSubDir = Path.Combine(pageDir, "Resources");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<StructureComparison> vcs in resources.Values)
            {
                foreach (StructureComparison c in vcs)
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

            // write out the resource type map
            if (_config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
            {
                WriteResourceTypeMap(conceptMapDir, resources);

                string mapSubDir = Path.Combine(conceptMapDir, "Resources");
                if (!Directory.Exists(mapSubDir))
                {
                    Directory.CreateDirectory(mapSubDir);
                }

                WriteStructureMaps(mapSubDir, resources.Values.SelectMany(l => l.Select(s => s)));
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
            LeftPackageId = _source.MainPackageId,
            LeftPackageVersion = _source.MainPackageVersion,
            RightPackageId = _target.MainPackageId,
            RightPackageVersion = _target.MainPackageVersion,
            ValueSets = _vsComparisons,
            PrimitiveTypes = primitiveComparisons,
            ComplexTypes = complexTypeComparisons,
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
        Dictionary<string, List<StructureComparison>> resources)
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
        Dictionary<string, List<PrimitiveTypeComparison>> primitiveTypes,
        Dictionary<string, List<StructureComparison>> complexTypes)
        {
            if (_crossVersion == null)
            {
                return;
            }

            ConceptMap? cm = _crossVersion.DataTypeMap ?? _crossVersion.GetSourceDataTypesConceptMap(primitiveTypes, complexTypes);
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

    //private void WriteDataTypeMap(
    //    string outputDir,
    //    IEnumerable<ComparisonRecord<StructureInfoRec>> primitiveTypes,
    //    IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes)
    //{
    //    if (_crossVersion == null)
    //    {
    //        return;
    //    }

    //    ConceptMap? cm = _crossVersion.GetSourceDataTypesConceptMap(primitiveTypes, complexTypes);
    //    if (cm == null)
    //    {
    //        return;
    //    }
    //    string filename = Path.Combine(outputDir, $"ConceptMap-{cm.Id}.json");

    //    try
    //    {
    //        using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
    //        using Utf8JsonWriter writer = new(fs, new JsonWriterOptions() { Indented = true, });
    //        {
    //            JsonSerializer.Serialize(writer, cm, _firelySerializerOptions);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error writing {filename}: {ex.Message} {ex.InnerException?.Message}");
    //    }
    //}


    //private void WriteStructureMaps(string outputDir, IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> values)
    //{
    //    if (_crossVersion == null)
    //    {
    //        return;
    //    }

    //    foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in values)
    //    {
    //        ConceptMap? cm = _crossVersion.TryGetSourceStructureElementConceptMap(c);
    //        if (cm == null)
    //        {
    //            continue;
    //        }

    //        string filename = Path.Combine(outputDir, $"ConceptMap-{cm.Id}.json");

    //        try
    //        {
    //            using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
    //            using Utf8JsonWriter writer = new(fs, new JsonWriterOptions() { Indented = true, });
    //            {
    //                JsonSerializer.Serialize(writer, cm, _firelySerializerOptions);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error writing {filename}: {ex.Message} {ex.InnerException?.Message}");
    //        }
    //    }
    //}

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

    private void WriteStructureMaps(string outputDir, IEnumerable<StructureComparison> values)
    {
        if (_crossVersion == null)
        {
            return;
        }

        foreach (StructureComparison c in values)
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

    private void WriteComparisonSummaryTableDict(
        ExportStreamWriter writer,
        Dictionary<string, List<StructureComparison>> values)
    {
        Dictionary<string, int> counts = [];

        // build summary data
        foreach (List<StructureComparison> vcs in values.Values)
        {
            foreach (StructureComparison c in vcs)
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

    private void WriteComparisonSummaryTableDict(
        ExportStreamWriter writer,
        Dictionary<string, List<PrimitiveTypeComparison>> values)
    {
        Dictionary<string, int> counts = [];

        // build summary data
        foreach (List<PrimitiveTypeComparison> vcs in values.Values)
        {
            foreach (PrimitiveTypeComparison c in vcs)
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
            writer.WriteLine($"| {c.CompositeName} | {c.Source.Url} | {c.Target?.Url} | {c.GetStatusString()} | {c.Message} |");
        }

        writer.WriteLine();

        writer.WriteLine("</details>");
        writer.WriteLine();
    }

    private void WriteComparisonOverview(
        ExportStreamWriter writer,
        string header,
        Dictionary<string, List<StructureComparison>> values)
    {
        writer.WriteLine("## " + header);

        WriteComparisonSummaryTableDict(writer, values);

        writer.WriteLine("<details>");
        writer.WriteLine("<summary>Entry details</summary>");

        writer.WriteLine();

        writer.WriteLine("| Name | Source | Dest | Status | Message |");
        writer.WriteLine("| ---- | ------ | ---- | ------ | ------- |");

        foreach (StructureComparison c in values.Values.SelectMany(vc => vc).OrderBy(c => c.CompositeName))
        {
            writer.WriteLine($"| {c.CompositeName} | {c.Source.Url} | {c.Target?.Url} | {c.GetStatusString()} | {c.Message} |");
        }

        writer.WriteLine();

        writer.WriteLine("</details>");
        writer.WriteLine();
    }


    private void WriteComparisonOverview(
        ExportStreamWriter writer,
        string header,
        Dictionary<string, List<PrimitiveTypeComparison>> values)
    {
        writer.WriteLine("## " + header);

        WriteComparisonSummaryTableDict(writer, values);

        writer.WriteLine("<details>");
        writer.WriteLine("<summary>Entry details</summary>");

        writer.WriteLine();

        writer.WriteLine("| Name | Source | Dest | Status | Message |");
        writer.WriteLine("| ---- | ------ | ---- | ------ | ------- |");

        foreach (PrimitiveTypeComparison c in values.Values.SelectMany(vc => vc).OrderBy(c => c.CompositeName))
        {
            writer.WriteLine($"| {c.CompositeName} | {c.Source.Name} | {c.Target?.Name ?? "-"} | {c.GetStatusString()} | {c.Message} |");
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
                    tdLeft = conceptInfoRecs.Left.Select(ci => (string[])[_sourceTableLiteral, ci.System, ci.Code, ci.Description.ForMdTable()]).ToArray();
                    tdRight = conceptInfoRecs.Right.Select(ci => (string[])[_targetTableLiteral, ci.System, ci.Code, ci.Description.ForMdTable()]).ToArray();
                }
                break;

            case IComparisonRecord<ValueSetInfoRec> valueSetInfoRecs:
                {
                    tdHeader = ["Side", "Url", "Name", "Title", "Description"];
                    tdLeft = valueSetInfoRecs.Left
                        .Select(vsi => (string[])[_sourceTableLiteral, vsi.Url, vsi.Name, vsi.Title.ForMdTable(), vsi.Description.ForMdTable()])
                        .ToArray();
                    tdRight = valueSetInfoRecs.Right
                        .Select(vsi => (string[])[_targetTableLiteral, vsi.Url, vsi.Name, vsi.Title.ForMdTable(), vsi.Description.ForMdTable()])
                        .ToArray();
                }
                break;

            case IComparisonRecord<ElementTypeInfoRec> elementTypeInfoRecs:
                {
                    tdHeader = ["Side", "Name", "Profiles", "Target Profiles"];
                    tdLeft = elementTypeInfoRecs.Left
                        .Select(eti => (string[])[_sourceTableLiteral, eti.Name, string.Join(", ", eti.Profiles), string.Join(", ", eti.TargetProfiles)])
                        .ToArray();
                    tdRight = elementTypeInfoRecs.Right
                        .Select(eti => (string[])[_targetTableLiteral, eti.Name, string.Join(", ", eti.Profiles), string.Join(", ", eti.TargetProfiles)])
                        .ToArray();
                }
                break;

            case IComparisonRecord<ElementInfoRec> elementInfoRecs:
                {
                    tdHeader = ["Side", "Name", "Path", "Short", "Definition", "Card", "Binding"];
                    tdLeft = elementInfoRecs.Left
                        .Select(ei => (string[])[
                            _sourceTableLiteral,
                            ei.Name,
                            ei.Path,
                            ei.Short.ForMdTable(),
                            ei.Definition.ForMdTable(),
                            $"{ei.MinCardinality}..{ei.MaxCardinalityString}",
                            $"{ei.ValueSetBindingStrength} {ei.BindingValueSet}"])
                        .ToArray();
                    tdRight = elementInfoRecs.Right
                        .Select(ei => (string[])[
                            _targetTableLiteral,
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
                            _sourceTableLiteral,
                            si.Name,
                            si.Title.ForMdTable(),
                            si.Description.ForMdTable(),
                            si.SnapshotCount.ToString(),
                            si.DifferentialCount.ToString()])
                        .ToArray();
                    tdRight = structureInfoRecs.Right
                        .Select(si => (string[])[
                            _targetTableLiteral,
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
            tdLeft = [[_sourceTableLiteral, .. Enumerable.Repeat("-", tdHeader.Length).ToArray()]];
        }

        if (tdRight.Length == 0)
        {
            tdRight = [[_targetTableLiteral, .. Enumerable.Repeat("-", tdHeader.Length).ToArray()]];
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
            writer.WriteLine($"### Union of {_sourceRLiteral} and {_targetRLiteral}");
            writer.WriteLine();
            WriteComparisonRecStatusTable(writer, cRec, inLeft: true, inRight: true);
            WriteComparisonChildDetails(writer, cRec, inLeft: true, inRight: true, useDetails: false);

            writer.WriteLine();
            writer.WriteLine($"### {_sourceRLiteral} Detail");
            writer.WriteLine();
            WriteComparisonRecStatusTable(writer, cRec, inLeft: true, inRight: false);
            WriteComparisonChildDetails(writer, cRec, inLeft: true, inRight: false, useDetails: false);

            writer.WriteLine();
            writer.WriteLine($"### {_targetRLiteral} Detail");
            writer.WriteLine();
            WriteComparisonRecStatusTable(writer, cRec, inLeft: false, inRight: true);
            WriteComparisonChildDetails(writer, cRec, inLeft: false, inRight: true, useDetails: false);
        }
    }

    private void WriteComparisonRecDataTable(ExportStreamWriter writer, ValueSetComparison v)
    {
        string[] tdHeader = ["Side", "Url", "Name", "Title", "Description"];
        string[] tdLeft = [_sourceTableLiteral, v.Source.Url, v.Source.Name, v.Source.Title.ForMdTable(), v.Source.Description.ForMdTable()];
        string[] tdRight = [_targetTableLiteral, v.Target?.Url ?? "-", v.Target?.Name ?? "-", v.Target?.Title.ForMdTable() ?? "-", v.Target?.Description.ForMdTable() ?? "-"];

        writer.WriteLine("| " + string.Join(" | ", tdHeader) + " |");
        writer.WriteLine("| " + string.Join(" | ", Enumerable.Repeat("---", tdHeader.Length)) + " |");
        writer.WriteLine("| " + string.Join(" | ", tdLeft) + " |");
        writer.WriteLine("| " + string.Join(" | ", tdRight) + " |");

        writer.WriteLine();
    }

    private void WriteComparisonRecDataTable(ExportStreamWriter writer, PrimitiveTypeComparison v)
    {
        string[] tdHeader = ["Side", "Url", "Name", "Description"];
        string[] tdLeft = [_sourceTableLiteral, v.Source.Url, v.Source.Name, v.Source.Description.ForMdTable()];
        string[] tdRight = [_targetTableLiteral, v.Target?.Url ?? "-", v.Target?.Name ?? "-", v.Target?.Description.ForMdTable() ?? "-"];

        writer.WriteLine("| " + string.Join(" | ", tdHeader) + " |");
        writer.WriteLine("| " + string.Join(" | ", Enumerable.Repeat("---", tdHeader.Length)) + " |");
        writer.WriteLine("| " + string.Join(" | ", tdLeft) + " |");
        writer.WriteLine("| " + string.Join(" | ", tdRight) + " |");

        writer.WriteLine();
    }

    private void WriteComparisonRecDataTable(ExportStreamWriter writer, StructureComparison s)
    {
        string[] tdHeader = ["Side", "Name", "Title", "Description", "Snapshot", "Differential"];
        string[] tdLeft = [_sourceTableLiteral, s.Source.Name, s.Source.Title.ForMdTable(), s.Source.Description.ForMdTable(), s.Source.SnapshotCount.ToString(), s.Source.DifferentialCount.ToString()];
        string[] tdRight = [
            _targetTableLiteral,
            s.Target?.Name ?? "-",
            s.Target?.Title.ForMdTable() ?? "-",
            s.Target?.Description.ForMdTable() ?? "-",
            s.Target?.SnapshotCount.ToString() ?? "-",
            s.Target?.DifferentialCount.ToString() ?? "-"];

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

    private void WriteComparisonRecStatusTable(ExportStreamWriter writer, StructureComparison s)
    {
        Dictionary<string, int> counts = [];

        // build summary data
        foreach (ElementComparison c in s.ElementComparisons.Values)
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

        writer.WriteLine("| Source | Target | Status | Message |");
        writer.WriteLine("| ------ | ------ | ------ | ------- |");

        foreach ((string code, ConceptComparison cc) in cRec.ConceptComparisons.OrderBy(kvp => kvp.Key))
        {
            if (cc.TargetMappings.Count == 0)
            {
                writer.WriteLine($"| {cc.Source.Code} | - | {cc.GetStatusString()} | {cc.Message} |");
                continue;
            }

            foreach (ConceptComparisonDetails cd in cc.TargetMappings)
            {
                writer.WriteLine($"| {cc.Source.Code} | {cd.Target.Code} | {cd.GetStatusString()} | {cd.Message} |");
            }
        }

        writer.WriteLine();

        if (useDetails)
        {
            writer.WriteLine("</details>");
            writer.WriteLine();
        }
    }

    private void WriteComparisonChildDetails(ExportStreamWriter writer, StructureComparison cRec, bool useDetails = true)
    {
        writer.WriteLine();
        if (useDetails)
        {
            writer.WriteLine("<details>");
            writer.WriteLine("<summary>Content details</summary>");
            writer.WriteLine();
        }

        writer.WriteLine("| Source | Target | Status | Message |");
        writer.WriteLine("| ------ | ------ | ------ | ------- |");

        foreach ((string path, ElementComparison ec) in cRec.ElementComparisons.OrderBy(kvp => kvp.Key))
        {
            if (ec.TargetMappings.Count == 0)
            {
                writer.WriteLine($"| {ec.Source.Path} | - | {ec.GetStatusString()} | {ec.Message} |");
                continue;
            }

            foreach (ElementComparisonDetails cd in ec.TargetMappings)
            {
                writer.WriteLine($"| {ec.Source.Path} | {cd.Target.Path} | {cd.GetStatusString()} | {cd.Message} |");
            }
        }

        writer.WriteLine();

        if (useDetails)
        {
            writer.WriteLine("</details>");
            writer.WriteLine();
        }
    }

    private void WriteComparisonChildDetails(ExportStreamWriter writer, PrimitiveTypeComparison cRec, bool useDetails = true)
    {
        writer.WriteLine();
        if (useDetails)
        {
            writer.WriteLine("<details>");
            writer.WriteLine("<summary>Content details</summary>");
            writer.WriteLine();
        }

        writer.WriteLine("| Source | Target | Status | Message |");
        writer.WriteLine("| ------ | ------ | ------ | ------- |");

        writer.WriteLine($"| {cRec.Source.Name} | {cRec.Target?.Name ?? "-"} | {cRec.GetStatusString()} | {cRec.Message.ForMdTable()} |");

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

    private void WriteComparisonFile(
        ExportStreamWriter writer,
        string header,
        PrimitiveTypeComparison cRec)
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
        WriteComparisonChildDetails(writer, cRec, useDetails: false);
    }

    private void WriteComparisonFile(
        ExportStreamWriter writer,
        string header,
        StructureComparison cRec)
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
            writer.WriteLine($"Comparison of {_source.MainPackageId}#{_source.MainPackageVersion} and {_target.MainPackageId}#{_target.MainPackageVersion}");
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
                Message = $"{_targetRLiteral} added type {typeName}",
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
                Message = $"{_targetRLiteral} removed type {typeName}",
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
                ? $"{_targetRLiteral} type {rSource[0].Name} is equivalent to the {_sourceRLiteral} type {typeName}"
                : $"{_targetRLiteral} new type {typeName} is equivalent to the {_sourceRLiteral} type {lSource[0].Name}";
        }
        else if (messages.Count == 0)
        {
            message = keyInLeft
                ? $"{_sourceRLiteral} type {typeName} maps as {relationship} for {_targetRLiteral}"
                : $"{_targetRLiteral} new type {typeName} maps as {relationship} for {_sourceRLiteral}";
        }
        else
        {
            message = keyInLeft
                ? $"{_sourceRLiteral} type {typeName} maps as {relationship} for {_targetRLiteral} because {string.Join(" and ", messages)}"
                : $"{_targetRLiteral} new type {typeName} maps as {relationship} for {_sourceRLiteral} because {string.Join(" and ", messages)}";

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
            (l.Count == 0 ? _sourceRLiteral : $"{_sourceRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _targetRLiteral : $"{_targetRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}");
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
            (l.Count == 0 ? _sourceRLiteral : $"{_sourceRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _targetRLiteral : $"{_targetRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}");
    }

    private bool TryGetVsComparison(string sourceUrl, string targetUrl, [NotNullWhen(true)] out ValueSetComparison? valueSetComparison)
    {
        if (!_vsComparisons.TryGetValue(sourceUrl, out List<ValueSetComparison>? vcs))
        {
            valueSetComparison = null;
            return false;
        }

        valueSetComparison = vcs.FirstOrDefault(vc => vc.Target.Url == targetUrl);
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
                Message = $"{_targetRLiteral} added element {edPath}",
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
                Message = $"{_targetRLiteral} removed element {edPath}",
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
                ? $"{_targetRLiteral} element {rSource[0].Path} is equivalent to the {_sourceRLiteral} element {edPath}"
                : $"{_targetRLiteral} new element {edPath} is equivalent to the {_sourceRLiteral} element {lSource[0].Path}";
        }
        else if (messages.Count == 0)
        {
            message = keyInLeft
                ? $"{_sourceRLiteral} element {edPath} maps as {relationship} for {_targetRLiteral}"
                : $"{_targetRLiteral} new element {edPath} maps as {relationship} for {_sourceRLiteral}";
        }
        else
        {
            message = keyInLeft
                ? $"{_sourceRLiteral} element {edPath} maps as {relationship} for {_targetRLiteral} because {string.Join(" and ", messages)}"
                : $"{_targetRLiteral} new element {edPath} maps as {relationship} for {_sourceRLiteral} because {string.Join(" and ", messages)}";
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
            (l.Count == 0 ? _sourceRLiteral : $"{_sourceRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _targetRLiteral : $"{_targetRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}");
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
                Message = $"{_targetRLiteral} added {sdName}",
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
                Message = $"{_targetRLiteral} removed {sdName}",
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
                Message = $"{_targetRLiteral}:{sdName} is equivalent",
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
                CMR.Equivalent => $"{_targetRLiteral} structure {rSource[0].si.Name} is equivalent to the {_sourceRLiteral} structure {sdName}",
                CMR.RelatedTo => $"{_targetRLiteral} structure {rSource[0].si.Name} is related to {_sourceRLiteral} structure {sdName} (see elements for details)",
                CMR.SourceIsNarrowerThanTarget => $"{_targetRLiteral} structure {rSource[0].si.Name} subsumes {_sourceRLiteral} structure {sdName}",
                CMR.SourceIsBroaderThanTarget => $"{_targetRLiteral} structure {rSource[0].si.Name} is subsumed by {_sourceRLiteral} structure {sdName}",
                _ => $"{_targetRLiteral} structure {rSource[0].si.Name} is related to {_sourceRLiteral} structure {sdName} (see elements for details)",
            };
        }
        else
        {
            message = relationship switch
            {
                CMR.Equivalent => $"{_targetRLiteral} new structure {sdName} is equivalent to the {_sourceRLiteral} structure {lSource[0].si.Name}",
                CMR.RelatedTo => $"{_targetRLiteral} new structure {sdName} is related to {_sourceRLiteral} structure {lSource[0].si.Name} (see elements for details)",
                CMR.SourceIsNarrowerThanTarget => $"{_targetRLiteral} new structure {sdName} subsumes {_sourceRLiteral} structure {lSource[0].si.Name}",
                CMR.SourceIsBroaderThanTarget => $"{_targetRLiteral} new structure {sdName} is subsumed by {_sourceRLiteral} structure {lSource[0].si.Name}",
                _ => $"{_targetRLiteral} new structure {sdName} is related to {_sourceRLiteral} structure {lSource[0].si.Name} (see elements for details)",
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
            return $"{_targetRLiteral}_{r[0].Name.ForName()}";
        }

        if (r.Count == 0)
        {
            return $"{_sourceRLiteral}_{l[0].Name.ForName()}";
        }

        if (l.Count == 1 && r.Count == 1)
        {
            return $"{_sourceRLiteral}_{l[0].Name.ForName()}_{_targetRLiteral}_{r[0].Name.ForName()}";
        }

        return
            $"{_sourceRLiteral}_{string.Join('_', l.Select(i => i.Name.ForName()).Order())}" +
            $"_{_targetRLiteral}_{string.Join('_', r.Select(i => i.Name.ForName()).Order())}";
    }

    private string GetName(List<(StructureDefinition sd, StructureInfoRec si)> l, List<(StructureDefinition sd, StructureInfoRec si)> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        if (l.Count == 0)
        {
            return $"{_targetRLiteral}_{r[0].si.Name.ForName()}";
        }

        if (r.Count == 0)
        {
            return $"{_sourceRLiteral}_{l[0].si.Name.ForName()}";
        }

        if (l.Count == 1 && r.Count == 1)
        {
            return $"{_sourceRLiteral}_{l[0].si.Name.ForName()}_{_targetRLiteral}_{r[0].si.Name.ForName()}";
        }

        return
            $"{_sourceRLiteral}_{string.Join('_', l.Select(s => s.si.Name.ForName()).Order())}" +
            $"_{_targetRLiteral}_{string.Join('_', r.Select(s => s.si.Name.ForName()).Order())}";
    }

    private string GetName(List<(StructureDefinition sd, StructureInfoRec si)> left, List<(StructureDefinition sd, StructureInfoRec si, CMR? r)> right)
    {
        if (left.Count == 0 && right.Count == 0)
        {
            return string.Empty;
        }

        if (left.Count == 0)
        {
            return $"{_targetRLiteral}_{right[0].si.Name.ForName()}";
        }

        if (right.Count == 0)
        {
            return $"{_sourceRLiteral}_{left[0].si.Name.ForName()}";
        }

        if (left.Count == 1 && right.Count == 1)
        {
            return $"{_sourceRLiteral}_{left[0].si.Name.ForName()}_{_targetRLiteral}_{right[0].si.Name.ForName()}";
        }

        int equivalentIndex = right.FindIndex(t => t.r == CMR.Equivalent);

        if (equivalentIndex == -1)
        {
            return $"{_targetRLiteral}_{string.Join('_', right.Select(s => s.si.Name.ForName()).Order())}";

            //return
            //    $"{_leftRLiteral}_{string.Join('_', left.Select(s => s.si.Name.ForName()).Order())}" +
            //    $"_{_rightRLiteral}_{string.Join('_', right.Select(s => s.si.Name.ForName()).Order())}";
        }

        string eqName = right[equivalentIndex].si.Name;

        if (right.Count == 1)
        {
            return
                $"{_sourceRLiteral}_{string.Join('_', left.Select(s => s.si.Name.ForName()).Order())}" +
                $"_{_targetRLiteral}_{eqName.ForName()}";
        }

        // we want equivalent listed first
        return
            $"{_sourceRLiteral}_{string.Join('_', left.Select(s => s.si.Name.ForName()).Order())}" +
            $"_{_targetRLiteral}_{eqName.ForName()}_{string.Join('_', right.Where(s => s.si.Name != eqName).Select(s => s.si.Name.ForName()).Order())}";
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
                _crossVersion.TryGetMapsForSource(sourceVs.Url, out List<ConceptMap>? conceptMaps))
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
        Dictionary<string, FhirConcept> sourceConcepts = sourceVs.cgGetFlatConcepts(_source).ToDictionary(c => c.Code);
        Dictionary<string, FhirConcept> targetConcepts = targetVs.cgGetFlatConcepts(_target).ToDictionary(c => c.Code);

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

        Dictionary<string, HashSet<string>> targetsMappedToSources = [];

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
                            CMR? relationship = mapTargetElement.Relationship ?? CMR.Equivalent;        // GetDefaultRelationship(mapTargetElement, mapSourceElement.Target);
                            string message = string.IsNullOrEmpty(mapTargetElement.Comment)
                                ? MessageForConceptRelationship(relationship, mapSourceElement, mapTargetElement)
                                : mapTargetElement.Comment;

                            string targetCombined = $"{targetSystem}#{mapTargetElement.Code}";
                            if (!targetsMappedToSources.TryGetValue(targetCombined, out HashSet<string>? sourceCodes))
                            {
                                sourceCodes = new();
                                targetsMappedToSources.Add(targetCombined, sourceCodes);
                            }

                            sourceCodes.Add(sourceKey);

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
                string targetCombined = $"{targetConcept.System}#{targetConcept.Code}";
                if (!targetsMappedToSources.TryGetValue(targetCombined, out HashSet<string>? sourceCodes))
                {
                    sourceCodes = new();
                    targetsMappedToSources.Add(targetCombined, sourceCodes);
                }

                sourceCodes.Add(sourceKey);

                // create a 'default' comparison state
                conceptComparisonDetails.Add(new()
                {
                    Target = GetInfo(targetConcept),
                    Relationship = CMR.Equivalent,
                    Message = $"{_sourceRLiteral} `{sourceConcept.Code}` is assumed equivalent to {_targetRLiteral} `{targetConcept.Code}` (no map, but codes match)",
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

        // check our target -> sources to see if we need to mark items as narrower
        foreach ((string targetCombined, HashSet<string> sourceKeys) in targetsMappedToSources)
        {
            if (string.IsNullOrEmpty(targetCombined))
            {
                throw new Exception();
            }

            if (sourceKeys.Count == 1)
            {
                continue;
            }

            string targetCode = targetCombined.Split('#')[^1];

            foreach (string sourceKey in sourceKeys)
            {
                string sourceConcept = sourceKey.Split('#')[^1];

                if (conceptComparisons.TryGetValue(sourceConcept, out ConceptComparison? cc) &&
                    (cc.Relationship == CMR.Equivalent))
                {
                    string msg = $"{_sourceRLiteral} `{cc.Source.Code}` is narrower than {_targetRLiteral} `{targetCode}` and is compatible." +
                            $" `{targetCode}` is mapped from {string.Join(" and ", sourceKeys.Order().Select(sk => $"`{sk.Split('#')[^1]}`"))}.";

                    ConceptComparison updated = cc with
                    {
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = msg,
                        TargetMappings = cc.TargetMappings
                            .Select(tc => tc.Target.Code != targetCode
                                ? tc
                                : tc with
                                {
                                    Relationship = CMR.SourceIsNarrowerThanTarget,
                                    Message = msg,
                                })
                            .ToList(),
                    };

                    conceptComparisons[sourceConcept] = updated;
                }
            }
        }

        string sourceName = sourceVs.Name.ToPascalCase();
        string targetName = targetVs.Name.ToPascalCase();

        CMR? vsRelationship = RelationshipForComparisons(conceptComparisons);

        comparison = new()
        {
            Source = GetInfo(sourceVs),
            Target = GetInfo(targetVs),
            CompositeName = $"{_sourceRLiteral}-{sourceName}-{_targetRLiteral}-{targetName}",
            ConceptComparisons = conceptComparisons,
            Relationship = vsRelationship,
            Message = MessageForComparisonRelationship(vsRelationship, sourceVs, targetVs),
        };
        return true;


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
            _ => comparisons.All(kvp => IsEquivalentOrNotPresent(kvp.Value))
                ? CMR.Equivalent
                : comparisons.All(kvp => IsEquivalentOrBroader(kvp.Value))
                ? CMR.SourceIsBroaderThanTarget
                : comparisons.All(kvp => IsEquivalentOrNarrower(kvp.Value))
                ? CMR.SourceIsNarrowerThanTarget
                : CMR.RelatedTo,
        };

        bool IsEquivalentOrNotPresent(ConceptComparison cc) =>
            cc.Relationship == CMR.Equivalent ||
            cc.TargetMappings.Count == 0;

        bool IsEquivalentOrBroader(ConceptComparison cc) =>
            cc.Relationship == CMR.Equivalent ||
            cc.Relationship == CMR.SourceIsBroaderThanTarget ||
            cc.TargetMappings.Count == 0;

        bool IsEquivalentOrNarrower(ConceptComparison cc) =>
            cc.Relationship == CMR.Equivalent ||
            cc.Relationship == CMR.SourceIsNarrowerThanTarget ||
            cc.TargetMappings.Count == 0;

        string MessageForConceptRelationship(CMR? r, ConceptMap.SourceElementComponent se, ConceptMap.TargetElementComponent te) => r switch
        {
            null => $"{_sourceRLiteral} `{se.Code}` has no mapping into {_targetRLiteral} {targetVs.Url}.",
            CMR.Equivalent => $"{_sourceRLiteral} `{se.Code}` is equivalent to {_targetRLiteral} `{te.Code}`.",
            CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} `{se.Code}` is broader than {_targetRLiteral} {te.Code} and requires mapping choice. `{se.Code}` maps to {string.Join(" and ", se.Target.Select(t => $"`{t.Code}`"))}.",
            _ => $"{_sourceRLiteral} `{se.Code}` maps as {r} to the target {_targetRLiteral} `{te.Code}`.",
        };

        string MessageForDetails(List<ConceptComparisonDetails> details, FhirConcept sourceConcept, ValueSet targetVs) => details.Count switch
        {
            0 => $"{_sourceRLiteral} `{sourceConcept.Code}` does not appear in the target and has no mapping for {targetVs.Url}.",
            1 => details[0].Message,
            _ => $"{_sourceRLiteral} `{sourceConcept.Code}` maps to multiple concepts in {targetVs.Url}.",
        };

        string MessageForComparisonRelationship(CMR? r, ValueSet sourceVs, ValueSet targetVs) => r switch
        {
            null => $"There is no mapping from {_sourceRLiteral} {sourceVs.Url} to {_targetRLiteral} {targetVs.Url}.",
            CMR.Equivalent => $"{_sourceRLiteral} {sourceVs.Url} is equivalent to {_targetRLiteral} {targetVs.Url}.",
            CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} {sourceVs.Url} is broader than {_targetRLiteral} {targetVs.Url} and requires mapping choices for conversion.",
            _ => $"{_sourceRLiteral} {sourceVs.Url} maps as {r} to {_targetRLiteral} {targetVs.Url}.",
        };
    }

    private CMR? GetDefaultRelationship(ConceptMap.TargetElementComponent mapTargetElement, List<ConceptMap.TargetElementComponent> targets) => targets.Count switch
    {
        0 => mapTargetElement.Relationship ?? CMR.NotRelatedTo,
        1 => mapTargetElement.Relationship ?? CMR.Equivalent,
        _ => ApplyRelationship(mapTargetElement.Relationship ?? CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
    };

    string MessageForPrimitiveRelationship(CMR? r, ConceptMap.SourceElementComponent se, ConceptMap.TargetElementComponent te) => r switch
    {
        null => $"{_sourceRLiteral} `{se.Code}` has no mapping into {_targetRLiteral} primitives.",
        CMR.Equivalent => $"{_sourceRLiteral} `{se.Code}` is equivalent to {_targetRLiteral} `{te.Code}`.",
        CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} `{se.Code}` is broader than {_targetRLiteral} {te.Code} and requires mapping choice. `{se.Code}` maps to {string.Join(" and ", se.Target.Select(t => $"`{t.Code}`"))}.",
        _ => $"{_sourceRLiteral} `{se.Code}` maps as {r} to the target {_targetRLiteral} `{te.Code}`.",
    };


    string MessageForComparisonRelationship(CMR? r, StructureDefinition sourceSd, StructureDefinition targetSd) => r switch
    {
        null => $"There is no mapping from {_sourceRLiteral} `{sourceSd.Name}` to {_targetRLiteral} `{targetSd.Name}`.",
        CMR.Equivalent => $"{_sourceRLiteral} `{sourceSd.Name}` is equivalent to {_targetRLiteral} `{targetSd.Name}`.",
        CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} `{sourceSd.Name}` is broader than {_targetRLiteral} `{targetSd.Name}` and requires mapping choices for conversion.",
        _ => $"{_sourceRLiteral} `{sourceSd.Name}` maps as {r} to {_targetRLiteral} `{targetSd.Name}`.",
    };

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

    private Dictionary<string, List<PrimitiveTypeComparison>> ComparePrimitives(
        IReadOnlyDictionary<string, StructureDefinition> sourcePrimitives,
        IReadOnlyDictionary<string, StructureDefinition> targetPrimitives)
    {
        Dictionary<string, List<PrimitiveTypeComparison>> results = [];

        Dictionary<string, Dictionary<string, List<ConceptMap.SourceElementComponent>>> mapsByTargetNameBySourceName = [];

        // check to see if we have a primitive type map
        if ((_crossVersion != null) &&
            (_crossVersion?.DataTypeMap is ConceptMap cm))
        {
            // build a mappings dictionary
            foreach (ConceptMap.SourceElementComponent primitiveSE in cm.Group.SelectMany(g => g.Element))
            {
                string sourceKey = primitiveSE.Code;

                if (!mapsByTargetNameBySourceName.TryGetValue(sourceKey, out Dictionary<string, List<ConceptMap.SourceElementComponent>>? targetMaps))
                {
                    targetMaps = [];
                    mapsByTargetNameBySourceName.Add(sourceKey, targetMaps);
                }

                foreach (ConceptMap.TargetElementComponent primitiveTE in primitiveSE.Target)
                {
                    if (!targetMaps.TryGetValue(primitiveTE.Code, out List<ConceptMap.SourceElementComponent>? elementMaps))
                    {
                        elementMaps = [];
                        targetMaps.Add(primitiveTE.Code, elementMaps);
                    }

                    elementMaps.Add(primitiveSE);
                }
            }
        }

        // loop over the source primitive types
        foreach ((string sourceCode, StructureDefinition sourceSd) in sourcePrimitives)
        {
            StructureInfoRec sourceInfo = GetInfo(sourceSd);

            HashSet<string> testedTargetNames = [];

            if (!results.TryGetValue(sourceCode, out List<PrimitiveTypeComparison>? comparisons))
            {
                comparisons = [];
                results.Add(sourceCode, comparisons);
            }

            // check to see if we have a map for this source type
            if (mapsByTargetNameBySourceName.TryGetValue(sourceCode, out Dictionary<string, List<ConceptMap.SourceElementComponent>>? targetMaps))
            {
                foreach ((string targetCode, List<ConceptMap.SourceElementComponent> mapSourceElements) in targetMaps)
                {
                    foreach (ConceptMap.SourceElementComponent mapSourceElement in mapSourceElements)
                    {
                        foreach (ConceptMap.TargetElementComponent mapTargetElement in mapSourceElement.Target)
                        {
                            if (testedTargetNames.Contains(mapTargetElement.Code))
                            {
                                continue;
                            }
                            testedTargetNames.Add(mapTargetElement.Code);

                            CMR? relationship = mapTargetElement.Relationship ?? CMR.Equivalent;        //  GetDefaultRelationship(mapTargetElement, mapSourceElement.Target);
                            string message = string.IsNullOrEmpty(mapTargetElement.Comment)
                                ? MessageForPrimitiveRelationship(relationship, mapSourceElement, mapTargetElement)
                                : mapTargetElement.Comment;

                            if (targetPrimitives.TryGetValue(mapTargetElement.Code, out StructureDefinition? targetSd))
                            {
                                comparisons.Add(new()
                                {
                                    SourceTypeLiteral = sourceCode,
                                    Source = sourceInfo,
                                    TargetTypeLiteral = mapTargetElement.Code,
                                    Target = GetInfo(targetSd),
                                    CompositeName = $"{_sourceRLiteral}-{sourceSd.Name.ToCamelCase()}-{_targetRLiteral}-{targetSd.Name.ToCamelCase()}",
                                    Relationship = relationship,
                                    Message = message,
                                });
                            }
                            else
                            {
                                comparisons.Add(new()
                                {
                                    SourceTypeLiteral = sourceCode,
                                    Source = sourceInfo,
                                    TargetTypeLiteral = mapTargetElement.Code,
                                    Target = new()
                                    {
                                        Name = mapTargetElement.Code,
                                        Url = string.Empty,
                                        Title = string.Empty,
                                        Description = string.Empty,
                                        Purpose = string.Empty,
                                        SnapshotCount = 0,
                                        DifferentialCount = 0,
                                    },
                                    CompositeName = $"{_sourceRLiteral}-{sourceSd.Name.ToCamelCase()}-{_targetRLiteral}-{mapTargetElement.Code.ToCamelCase()}",
                                    Relationship = relationship,
                                    Message = message,
                                });
                            }
                        }
                    }
                }
            }
            else if (targetPrimitives.TryGetValue(sourceCode, out StructureDefinition? targetSd))
            {
                comparisons.Add(new()
                {
                    SourceTypeLiteral = sourceCode,
                    Source = sourceInfo,
                    TargetTypeLiteral = sourceCode,
                    Target = GetInfo(targetSd),
                    CompositeName = $"{_sourceRLiteral}-{sourceSd.Name.ToCamelCase()}-{_targetRLiteral}-{targetSd.Name.ToCamelCase()}",
                    Relationship = CMR.Equivalent,
                    Message = $"{_sourceRLiteral} `{sourceCode}` is assumed equivalent to {_targetRLiteral} `{sourceCode}` (no map, but names match)",
                });
            }
        }

        return results;
    }


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
                Message = $"{_targetRLiteral} added {sdName}",
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
                Message = $"{_targetRLiteral} removed {sdName}",
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
                message = $"{_targetRLiteral} primitive {rightSi.Name} is equivalent to the {_sourceRLiteral} primitive {leftSi.Name}";

                // also add a serialization info
                serializations.Add(leftSi.Name, new()
                {
                    Source = leftSi.Name,
                    Target = rightSi.Name,
                    Relationship = rightRelationship ?? CMR.SourceIsBroaderThanTarget,
                    Message = $"{_targetRLiteral} primitive {rightSi.Name} is equivalent to the {_sourceRLiteral} primitive {leftSi.Name}",
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
                Message = $"{_targetRLiteral} new type {rightSi.Name} has a serialization mapping from {_sourceRLiteral} type {leftSi.Name}",
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
                    CMR.Equivalent => $"{_targetRLiteral} type {rSource[0].si.Name} is equivalent to the {_sourceRLiteral} type {sdName}",
                    CMR.RelatedTo => $"{_targetRLiteral} type {rSource[0].si.Name} is related to {_sourceRLiteral} type {sdName}",
                    CMR.SourceIsNarrowerThanTarget => $"{_targetRLiteral} type {rSource[0].si.Name} subsumes {_sourceRLiteral} type {sdName}",
                    CMR.SourceIsBroaderThanTarget => $"{_targetRLiteral} type {rSource[0].si.Name} is subsumed by {_sourceRLiteral} type {sdName}",
                    _ => $"{_targetRLiteral} type {rSource[0].si.Name} is related to {_sourceRLiteral} type {sdName}",
                };
            }
            else
            {
                message = relationship switch
                {
                    CMR.Equivalent => $"{_targetRLiteral} new type {sdName} is equivalent to the {_sourceRLiteral} type {lSource[0].si.Name}",
                    CMR.RelatedTo => $"{_targetRLiteral} new type {sdName} is related to {_sourceRLiteral} type {lSource[0].si.Name}",
                    CMR.SourceIsNarrowerThanTarget => $"{_targetRLiteral} new type {sdName} subsumes {_sourceRLiteral} type {lSource[0].si.Name}",
                    CMR.SourceIsBroaderThanTarget => $"{_targetRLiteral} new type {sdName} is subsumed by {_sourceRLiteral} type {lSource[0].si.Name}",
                    _ => $"{_targetRLiteral} new type {sdName} is related to {_sourceRLiteral} type {lSource[0].si.Name}",
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

    private Dictionary<string, List<StructureComparison>> CompareStructures(
        IReadOnlyDictionary<string, StructureDefinition> sourceStructures,
        IReadOnlyDictionary<string, StructureDefinition> targetStructures)
    {
        Dictionary<string, List<StructureComparison>> results = [];

        // loop over the source structures
        foreach ((string sourceSdName, StructureDefinition sourceSd) in sourceStructures)
        {
            HashSet<string> testedTargetNames = [];

            if (!results.TryGetValue(sourceSdName, out List<StructureComparison>? comparisons))
            {
                comparisons = [];
                results.Add(sourceSdName, comparisons);
            }

            // check to see if we have any maps for this source value set
            if ((_crossVersion != null) &&
                _crossVersion.TryGetMapsForSource(sourceSd.Url, out List<ConceptMap>? conceptMaps))
            {
                // traverse our list of concept maps
                foreach (ConceptMap cm in conceptMaps)
                {
                    // check to see if we have a target value set
                    if ((cm.TargetScope is Canonical targetCanonical) &&
                        targetStructures.TryGetValue(targetCanonical.Uri ?? string.Empty, out StructureDefinition? mappedTargetSd))
                    {
                        testedTargetNames.Add(targetCanonical.Uri!);

                        // test this mapping
                        if (TryCompareStructureElements(sourceSd, mappedTargetSd, cm, out StructureComparison? mappedComparison))
                        {
                            comparisons.Add(mappedComparison);
                        }
                    }
                }
            }

            // make sure that we tested direct source -> target if it exists
            if (!testedTargetNames.Contains(sourceSdName) &&
                targetStructures.TryGetValue(sourceSdName, out StructureDefinition? targetSd))
            {
                if (TryCompareStructureElements(sourceSd, targetSd, null, out StructureComparison? directComparison))
                {
                    comparisons.Add(directComparison);
                }
            }
        }

        return results;
    }

    private bool TryCompareStructureElements(
        StructureDefinition sourceSd,
        StructureDefinition targetSd,
        ConceptMap? sdConceptMap,
        [NotNullWhen(true)] out StructureComparison? comparison)
    {
        Dictionary<string, ElementDefinition> sourceElements = sourceSd.cgElements().ToDictionary(c => c.Path);
        Dictionary<string, ElementDefinition> targetElements = targetSd.cgElements().ToDictionary(c => c.Path);

        Dictionary<string, ElementComparison> elementComparisons = [];

        Dictionary<string, Dictionary<string, List<ConceptMap.SourceElementComponent>>> mapsByTargetPathBySourcePath = [];

        // build a mapping lookup if we have one
        if (sdConceptMap != null)
        {
            // traverse the groups in our map - each group represents a system
            foreach (ConceptMap.GroupComponent cmGroup in sdConceptMap.Group)
            {
                string groupSourceSystem = cmGroup.Source ?? UnversionedUrl(sourceSd.Url);
                string groupTargetSystem = cmGroup.Target ?? UnversionedUrl(targetSd.Url);

                // add all the elements from this group to our lookup
                foreach (ConceptMap.SourceElementComponent cmElement in cmGroup.Element)
                {
                    string sourceKey = cmElement.Code;

                    if (!mapsByTargetPathBySourcePath.TryGetValue(sourceKey, out Dictionary<string, List<ConceptMap.SourceElementComponent>>? targetMaps))
                    {
                        targetMaps = [];
                        mapsByTargetPathBySourcePath.Add(sourceKey, targetMaps);
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

        Dictionary<string, HashSet<string>> targetsMappedToSources = [];

        // be optimistic
        CMR elementRelationship = CMR.Equivalent;

        // traverse the source elements to do comparison tests
        foreach (ElementDefinition sourceEd in sourceElements.Values)
        {
            ElementInfoRec sourceEdInfo = GetInfo(sourceEd);
            List<ElementComparisonDetails> elementComparisonDetails = [];

            string sourceKey = sourceEd.Path;

            // check to see if we have a map for this source element
            if (mapsByTargetPathBySourcePath.TryGetValue(sourceKey, out Dictionary<string, List<ConceptMap.SourceElementComponent>>? targetMaps))
            {
                foreach ((string targetSystem, List<ConceptMap.SourceElementComponent> mapSourceElements) in targetMaps)
                {
                    foreach (ConceptMap.SourceElementComponent mapSourceElement in mapSourceElements)
                    {
                        foreach (ConceptMap.TargetElementComponent mapTargetElement in mapSourceElement.Target)
                        {
                            CMR relationship = mapTargetElement.Relationship ?? CMR.Equivalent;     //  GetDefaultRelationship(mapTargetElement, mapSourceElement.Target) ?? CMR.Equivalent;
                            string message = string.IsNullOrEmpty(mapTargetElement.Comment)
                                ? MessageForElementRelationship(relationship, mapSourceElement, mapTargetElement)
                                : mapTargetElement.Comment;

                            string targetCombined = mapTargetElement.Code;
                            if (!targetsMappedToSources.TryGetValue(targetCombined, out HashSet<string>? sourceCodes))
                            {
                                sourceCodes = new();
                                targetsMappedToSources.Add(targetCombined, sourceCodes);
                            }

                            sourceCodes.Add(sourceKey);

                            if (targetElements.TryGetValue(mapTargetElement.Code, out ElementDefinition? targetElementFromMap))
                            {
                                ElementComparisonDetails ecd = CompareElement(sourceEd, targetElementFromMap, mapTargetElement.Relationship);
                                relationship = ApplyRelationship(relationship, ecd.Relationship);
                                elementRelationship = ApplyRelationship(elementRelationship, relationship);
                                elementComparisonDetails.Add(ecd);
                            }
                            else
                            {
                                throw new Exception("Target element does not exist!");
                                //elementComparisonDetails.Add(new()
                                //{
                                //    Target = new()
                                //    {
                                //        System = targetSystem,
                                //        Code = mapTargetElement.Code,
                                //        Description = string.Empty,
                                //    },
                                //    Relationship = relationship,
                                //    Message = message,
                                //    IsPreferred = conceptComparisonDetails.Count == 0,
                                //});
                            }
                        }
                    }
                }
            }
            else if (targetElements.TryGetValue(sourceKey, out ElementDefinition? targetElement))
            {
                string targetCombined = targetElement.Path;
                if (!targetsMappedToSources.TryGetValue(targetCombined, out HashSet<string>? sourceElementPaths))
                {
                    sourceElementPaths = new();
                    targetsMappedToSources.Add(targetCombined, sourceElementPaths);
                }

                sourceElementPaths.Add(sourceKey);

                ElementComparisonDetails ecd = CompareElement(sourceEd, targetElement, CMR.Equivalent);
                elementRelationship = ApplyRelationship(elementRelationship, ecd.Relationship);
                elementComparisonDetails.Add(ecd);

                //// create a 'default' comparison state
                //elementComparisonDetails.Add(new()
                //{
                //    Target = GetInfo(targetElement),
                //    Relationship = CMR.Equivalent,
                //    Message = $"{_sourceRLiteral} `{sourceEd.Path}` is assumed equivalent to {_targetRLiteral} `{targetElement.Path}` (no map, but paths match)",
                //});
            }

            ElementComparison cc = new()
            {
                Source = sourceEdInfo,
                TargetMappings = elementComparisonDetails,
                Relationship = elementRelationship,         //  RelationshipForDetails(elementComparisonDetails),
                Message = MessageForDetails(elementComparisonDetails, sourceEdInfo, targetSd),
            };

            elementComparisons.Add(sourceEd.Path, cc);
        }

        // check our target -> sources to see if we need to mark items as narrower
        foreach ((string targetPath, HashSet<string> sourceKeys) in targetsMappedToSources)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new Exception();
            }

            if (sourceKeys.Count == 1)
            {
                continue;
            }

            foreach (string sourceKey in sourceKeys)
            {
                string sourceConcept = sourceKey.Split('#')[^1];

                if (elementComparisons.TryGetValue(sourceConcept, out ElementComparison? ec) &&
                    (ec.Relationship == CMR.Equivalent))
                {
                    string msg = $"{_sourceRLiteral} `{ec.Source.Path}` is narrower than {_targetRLiteral} `{targetPath}` and is compatible." +
                            $" `{targetPath}` is mapped from {string.Join(" and ", sourceKeys.Order().Select(sk => $"`{sk}`"))}.";

                    ElementComparison updated = ec with
                    {
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = msg,
                        TargetMappings = ec.TargetMappings
                            .Select(tc => tc.Target?.Path != targetPath
                                ? tc
                                : tc with
                                {
                                    Relationship = CMR.SourceIsNarrowerThanTarget,
                                    Message = msg,
                                })
                            .ToList(),
                    };

                    elementComparisons[sourceConcept] = updated;
                }
            }
        }

        string sourceName = sourceSd.Name.ToPascalCase();
        string targetName = targetSd.Name.ToPascalCase();

        CMR? sdRelationship = RelationshipForComparisons(elementComparisons);

        comparison = new()
        {
            Source = GetInfo(sourceSd),
            Target = GetInfo(targetSd),
            CompositeName = $"{_sourceRLiteral}-{sourceName}-{_targetRLiteral}-{targetName}",
            ElementComparisons = elementComparisons,
            Relationship = sdRelationship,
            Message = MessageForComparisonRelationship(sdRelationship, sourceSd, targetSd),
        };
        return true;


        string MessageForElementRelationship(CMR? r, ConceptMap.SourceElementComponent se, ConceptMap.TargetElementComponent te) => r switch
        {
            null => $"{_sourceRLiteral} {sourceSd.Name} `{se.Code}` has no mapping into {_targetRLiteral} {targetSd.Name}.",
            CMR.Equivalent => $"{_sourceRLiteral} {sourceSd.Name} `{se.Code}` is equivalent to {_targetRLiteral} `{te.Code}`.",
            CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} {sourceSd.Name} `{se.Code}` is broader than {_targetRLiteral} {te.Code} and requires mapping choice. `{se.Code}` maps to {string.Join(" and ", se.Target.Select(t => $"`{t.Code}`"))}.",
            _ => $"{_sourceRLiteral} {sourceSd.Name} `{se.Code}` maps as {r} to the target {_targetRLiteral} {targetSd.Name} `{te.Code}`.",
        };

        //CMR? RelationshipForDetails(List<ElementComparisonDetails> details) => details.Count switch
        //{
        //    0 => CMR.NotRelatedTo,
        //    1 => CMR.Equivalent,
        //    _ => CMR.SourceIsBroaderThanTarget,
        //};

        string MessageForDetails(List<ElementComparisonDetails> details, ElementInfoRec sourceInfo, StructureDefinition targetSd) => details.Count switch
        {
            0 => $"{_sourceRLiteral} `{sourceInfo.Path}` does not appear in the target and has no mapping for `{targetSd.Name}`.",
            1 => details[0].Message,
            _ => $"{_sourceRLiteral} `{sourceInfo.Path}` maps to multiple elements in `{targetSd.Name}`.",
        };

        string MessageForComparisonRelationship(CMR? r, StructureDefinition sourceSd, StructureDefinition targetSd) => r switch
        {
            null => $"There is no mapping from {_sourceRLiteral} `{sourceSd.Name}` to {_targetRLiteral} `{targetSd.Name}`.",
            CMR.Equivalent => $"{_sourceRLiteral} `{sourceSd.Name}` is equivalent to {_targetRLiteral} `{targetSd.Name}`.",
            CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} `{sourceSd.Name}` is broader than {_targetRLiteral} `{targetSd.Name}` and requires mapping choices for conversion.",
            _ => $"{_sourceRLiteral} `{sourceSd.Name}` maps as {r} to {_targetRLiteral} `{targetSd.Name}`.",
        };

        CMR? RelationshipForComparisons(Dictionary<string, ElementComparison> comparisons) => comparisons.Count switch
        {
            0 => CMR.NotRelatedTo,
            1 => comparisons.First().Value.Relationship,
            _ => comparisons.All(kvp => IsEquivalent(kvp.Value))
                ? CMR.Equivalent
                : comparisons.All(kvp => IsEquivalentOrBroader(kvp.Value))
                ? CMR.SourceIsBroaderThanTarget
                : comparisons.All(kvp => IsEquivalentOrNarrower(kvp.Value))
                ? CMR.SourceIsNarrowerThanTarget
                : CMR.RelatedTo,
        };

        bool IsEquivalent(ElementComparison cc) => cc.Relationship == CMR.Equivalent;

        bool IsEquivalentOrBroader(ElementComparison cc) =>
            cc.Relationship == CMR.Equivalent ||
            cc.Relationship == CMR.SourceIsBroaderThanTarget ||
            cc.TargetMappings.Count == 0;

        bool IsEquivalentOrNarrower(ElementComparison cc) =>
            cc.Relationship == CMR.Equivalent ||
            cc.Relationship == CMR.SourceIsNarrowerThanTarget;
    }

    private ElementComparisonDetails CompareElement(
        ElementDefinition sourceEd,
        ElementDefinition? targetEd,
        CMR? initialRelationship)
    {
        if (targetEd == null)
        {
            return new()
            {
                Target = null,
                Relationship = null,
                TypeComparisons = [],
                Message = $"{_sourceRLiteral} `{sourceEd.Path}` does not exist in {_targetRLiteral} and no mapping is available.",
            };
        }

        // be optimistic if we don't have a mapped value
        CMR relationship = initialRelationship ?? CMR.Equivalent;

        ElementInfoRec sourceInfo = GetInfo(sourceEd);
        ElementInfoRec targetInfo = GetInfo(targetEd);

        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> sourceTypes = sourceEd.cgTypes();
        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> targetTypes = targetEd.cgTypes();

        List<string> messages = [];

        // check for optional becoming mandatory
        if ((sourceInfo.MinCardinality == 0) && (targetInfo.MinCardinality != 0))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"{targetInfo.Name} made the element mandatory");
        }

        // check for source allowing fewer than destination requires
        if (sourceInfo.MinCardinality < targetInfo.MinCardinality)
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"{targetInfo.Name} increased the minimum cardinality from {sourceInfo.MinCardinality} to {targetInfo.MinCardinality}");
        }

        // check for element being constrained out
        if ((sourceInfo.MaxCardinality != 0) && (targetInfo.MaxCardinality == 0))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"{targetInfo.Name} constrained the element out (max cardinality of 0)");
        }

        // check for changing from scalar to array
        if ((sourceInfo.MaxCardinality == 1) && (targetInfo.MaxCardinality != 1))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"{targetInfo.Name} changed from scalar to array (max cardinality from {sourceInfo.MaxCardinalityString} to {targetInfo.MaxCardinalityString})");
        }

        // check for changing from array to scalar
        if ((sourceInfo.MaxCardinality != 1) && (targetInfo.MaxCardinality == 1))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"{targetInfo.Name} changed from array to scalar (max cardinality from {sourceInfo.MaxCardinalityString} to {targetInfo.MaxCardinalityString})");
        }

        // check for source allowing more than destination allows
        if ((targetInfo.MaxCardinality != -1) &&
            (sourceInfo.MaxCardinality > targetInfo.MaxCardinality))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"{targetInfo.Name} allows more repetitions (max cardinality from {sourceInfo.MaxCardinalityString} to {targetInfo.MaxCardinalityString})");
        }

        // check to see if there was not a required binding and now there is
        if ((sourceInfo.ValueSetBindingStrength != null) || (targetInfo.ValueSetBindingStrength != null))
        {
            if ((sourceInfo.ValueSetBindingStrength != BindingStrength.Required) && (targetInfo.ValueSetBindingStrength == BindingStrength.Required))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);

                if (sourceInfo.ValueSetBindingStrength == null)
                {
                    messages.Add($"{targetInfo.Name} added a required binding to {targetInfo.BindingValueSet}");
                }
                else
                {
                    messages.Add($"{targetInfo.Name} made the binding required (from {sourceInfo.ValueSetBindingStrength}) for {targetInfo.BindingValueSet}");
                }
            }
            else if (sourceInfo.ValueSetBindingStrength != targetInfo.ValueSetBindingStrength)
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                if (sourceInfo.ValueSetBindingStrength == null)
                {
                    messages.Add($"{targetInfo.Name} added a binding requirement - {targetInfo.ValueSetBindingStrength} {targetInfo.BindingValueSet}");
                }
                else if (targetInfo.ValueSetBindingStrength == null)
                {
                    messages.Add($"{targetInfo.Name} removed a binding requirement - {sourceInfo.ValueSetBindingStrength} {sourceInfo.BindingValueSet}");
                }
                else
                {
                    messages.Add($"{targetInfo.Name} changed the binding strength from {sourceInfo.ValueSetBindingStrength} to {targetInfo.ValueSetBindingStrength}");
                }
            }

            // check to see if we need to lookup a binding comparison
            if ((sourceInfo.ValueSetBindingStrength == BindingStrength.Required) && (targetInfo.ValueSetBindingStrength == BindingStrength.Required))
            {
                // TODO(ginoc): For sanity right now, we assume that the value sets are from the matching releases
                // at some point, we need to check specific versions in case there are explicit references

                string unversionedLeft = sourceInfo.BindingValueSet.Split('|')[0];
                string unversionedRight = targetInfo.BindingValueSet.Split('|')[0];

                // if there is a code type, we need to perform a code-only comparison
                if (sourceTypes.ContainsKey("code"))
                {
                    // look for the value set comparison
                    if (TryGetVsComparison(unversionedLeft, unversionedRight, out ValueSetComparison? boundVsInfo))
                    {
                        // we are okay with equivalent and narrower
                        if (boundVsInfo.Relationship == CMR.Equivalent ||
                            boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget)
                        {
                            relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                            messages.Add($"{targetInfo.Name} has compatible required binding for code type: {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet} ({boundVsInfo.Relationship})");
                        }

                        // check to see if the codes are the same but the systems are different (ok in codes)
                        else if (boundVsInfo.ConceptComparisons.Values.All(cc => cc.TargetMappings.Any(tc => tc.Target.Code == cc.Source.Code)))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{targetInfo.Name} has compatible required binding for code type: {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet} (codes match, though systems are different)");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                            messages.Add($"{targetInfo.Name} has INCOMPATIBLE required binding for code type: {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet}");
                        }
                    }
                    else if (_exclusionSet.Contains(unversionedRight))
                    {
                        relationship = ApplyRelationship(relationship, CMR.Equivalent);
                        messages.Add($"{targetInfo.Name} using {unversionedRight} is exempted and assumed equivalent");
                    }
                    else
                    {
                        relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                        messages.Add($"({targetInfo.Name} failed to compare required binding of {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet})");
                    }
                }

                // if there are any non-code types, we need to perform a code+system comparison
                if (sourceTypes.Any(t => t.Key != "code"))
                {
                    // check for same value set (non-code type)
                    if (TryGetVsComparison(unversionedLeft, unversionedRight, out ValueSetComparison? boundVsInfo))
                    {
                        if ((boundVsInfo.Relationship == CMR.Equivalent) ||
                            (boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget))
                        {
                            // we are okay with equivalent and narrower
                            relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                            messages.Add($"{targetInfo.Name} has compatible required binding for non-code type: {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet} ({boundVsInfo.Relationship})");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                            messages.Add($"{targetInfo.Name} has INCOMPATIBLE required binding for code type: {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet}");
                        }
                    }
                    else if (_exclusionSet.Contains(unversionedRight))
                    {
                        relationship = ApplyRelationship(relationship, CMR.Equivalent);
                        messages.Add($"{targetInfo.Name} using {unversionedRight} is exempted and assumed equivalent");
                    }
                    else
                    {
                        relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                        messages.Add($"({targetInfo.Name} failed to compare required binding of {sourceInfo.BindingValueSet} and {targetInfo.BindingValueSet})");
                    }
                }
            }
        }

        // perform element type comparison
        Dictionary<string, ElementTypeComparison> etComparisons = CompareElementTypes(sourceInfo, targetInfo);

        // process our type comparisons and promote messages
        foreach (ElementTypeComparison etc in etComparisons.Values)
        {
            // skip equivalent types
            if (etc.Relationship == CMR.Equivalent)
            {
                continue;
            }

            relationship = ApplyRelationship(relationship, etc.Relationship);
            messages.Add($"{targetInfo.Name} has change due to type change: {etc.Message}");
        }


        return new()
        {
            Target = targetInfo,
            Relationship = relationship,
            TypeComparisons = etComparisons,
            Message = $"{_sourceRLiteral} `{sourceEd.Path}` maps as {relationship} to {_targetRLiteral} `{targetInfo.Path}`"
                + (messages.Count == 0 ? string.Empty : (" - " + string.Join("; ", messages))),
        };
    }

    private Dictionary<string, ElementTypeComparison> CompareElementTypes(
        ElementInfoRec sourceInfo,
        ElementInfoRec? targetInfo)
    {
        if (targetInfo == null)
        {
            // do not bother with comparing against a target that does not exist
            return [];
        }

        Dictionary<string, ElementTypeComparison> results = [];

        Dictionary<string, ConceptMap.SourceElementComponent> dataTypeMaps =
            _crossVersion?.DataTypeMap?.Group.FirstOrDefault()?.Element.ToDictionary(e => e.Code) ?? [];

        HashSet<string> usedTargetTypes = [];

        List<string> elementMessages = [];

        // traverse source types
        foreach ((string sourceTypeName, ElementTypeInfoRec sourceTypeInfo) in sourceInfo.Types)
        {
            ElementTypeInfoRec? targetTypeInfo = null;

            // get any type mappings for this source type
            Dictionary<string, List<(string typeName, CMR? relationship, string comment)>> mappedTargetTypes = [];

            if (dataTypeMaps.TryGetValue(sourceTypeName, out ConceptMap.SourceElementComponent? sourceMap))
            {
                foreach (ConceptMap.TargetElementComponent tec in sourceMap.Target)
                {
                    if (mappedTargetTypes.TryGetValue(tec.Code, out List<(string typeName, CMR? relationship, string comment)>? mttList))
                    {
                        mttList.Add((tec.Code, tec.Relationship, tec.Comment));
                    }
                    else
                    {
                        mappedTargetTypes.Add(tec.Code, [(tec.Code, tec.Relationship, tec.Comment)]);
                    }
                }
            }

            //Dictionary<string, (string typeName, CMR? relationship, string comment)> mappedTargetTypes = dataTypeMaps.TryGetValue(sourceTypeName, out ConceptMap.SourceElementComponent? sourceMap)
            //    ? sourceMap.Target.ToDictionary(t => t.Code, t => (t.Code, t.Relationship, t.Comment))
            //    : [];

            // if there is no type mapping, check for a direct match
            if ((mappedTargetTypes.Count == 0) &&
                (!targetInfo.Types.TryGetValue(sourceTypeName, out targetTypeInfo)))
            {
                string msg = $"{_sourceRLiteral} type {sourceTypeName} does not exist in {_targetRLiteral}";
                elementMessages.Add(msg);

                // this type cannot be mapped
                results.Add(sourceTypeName, new()
                {
                    Source = sourceTypeInfo,
                    TargetTypes = [],
                    Relationship = CMR.SourceIsBroaderThanTarget,
                    Message = msg,
                });

                continue;
            }

            // be optimistic
            CMR sourceTypeRelationship = CMR.Equivalent;

            if (targetTypeInfo != null)
            {
                string msg = $"{_targetRLiteral} {targetTypeInfo.Name} is assumed equivalent to {_sourceRLiteral} {sourceTypeInfo.Name}";

                // be optimistic if we don't have a mapped relationship
                mappedTargetTypes.Add(targetTypeInfo.Name, [(targetTypeInfo.Name, CMR.Equivalent, msg)]);
            }

            List<ElementTypeComparisonDetails> details = [];

            // traverse possible target types
            foreach ((string targetTypeName, CMR? typeRelationship, string comment) in mappedTargetTypes.Values.SelectMany(v => v))
            {
                if (!targetInfo.Types.TryGetValue(targetTypeName, out targetTypeInfo))
                {
                    // hope another valid type exists
                    continue;
                }

                _ = usedTargetTypes.Add(targetTypeName);

                List<string> addedProfiles = [];
                List<string> removedProfiles = [];

                HashSet<string> scratch = targetTypeInfo.Profiles.ToHashSet();

                foreach (string sp in sourceTypeInfo.Profiles)
                {
                    if (scratch.Contains(sp))
                    {
                        scratch.Remove(sp);
                        continue;
                    }

                    removedProfiles.Add(sp);
                }

                addedProfiles.AddRange(scratch);

                List<string> addedTargets = [];
                List<string> removedTargets = [];

                scratch = targetTypeInfo.TargetProfiles.ToHashSet();

                foreach (string sp in sourceTypeInfo.TargetProfiles)
                {
                    if (scratch.Contains(sp))
                    {
                        scratch.Remove(sp);
                        continue;
                    }

                    removedTargets.Add(sp);
                }

                addedTargets.AddRange(scratch);

                // be optimistic if we don't have a mapped relationship
                CMR relationship = typeRelationship ?? CMR.Equivalent;
                List<string> typeMessages = [];

                if (addedProfiles.Any())
                {
                    relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                    typeMessages.Add($"{targetTypeInfo.Name} added profiles: {string.Join(", ", addedProfiles)}");
                }

                if (removedProfiles.Any())
                {
                    relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                    typeMessages.Add($"{targetTypeInfo.Name} removed profiles: {string.Join(", ", removedProfiles)}");
                }

                if (addedTargets.Any())
                {
                    relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                    typeMessages.Add($"{targetTypeInfo.Name} added target profiles: {string.Join(", ", addedTargets)}");
                }

                if (removedTargets.Any())
                {
                    relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                    typeMessages.Add($"{targetTypeInfo.Name} removed target profiles: {string.Join(", ", removedTargets)}");
                }

                // apply our ending relationship to the next level up
                sourceTypeRelationship = ApplyRelationship(sourceTypeRelationship, relationship);

                // add our details
                details.Add(new ()
                {
                    Target = targetTypeInfo,
                    Relationship = relationship,
                    Message = comment + (typeMessages.Count == 0 ? string.Empty : $" - {string.Join("; ", typeMessages)})"),
                });
            }

            if (details.Count == 0)
            {
                string msg = $"{_sourceRLiteral} {sourceInfo.Name} {sourceTypeName} has no equivalent or mapped type in {_targetRLiteral} {targetInfo.Name}";
                elementMessages.Add(msg);

                //elementRelationship = ApplyRelationship(sourceTypeRelationship, CMR.SourceIsBroaderThanTarget);

                // this type cannot be mapped
                results.Add(sourceTypeName, new()
                {
                    Source = sourceTypeInfo,
                    TargetTypes = [],
                    Relationship = CMR.SourceIsBroaderThanTarget,
                    Message = msg,
                });

                continue;
            }

            //elementRelationship = ApplyRelationship(sourceTypeRelationship, elementRelationship);

            string eMsg = $"{_sourceRLiteral} `{sourceInfo.Name}` `{sourceTypeName}` maps as {sourceTypeRelationship} for {_targetRLiteral} `{targetInfo.Name}`";
            elementMessages.Add(eMsg);

            results.Add(sourceTypeName, new()
            {
                Source = sourceTypeInfo,
                TargetTypes = details,
                Relationship = sourceTypeRelationship,
                Message = eMsg,
            });
        }

        return results;
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
            ConceptMap? elementMap = null;// _crossVersion?.ElementTypeMap(sdName);

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
            Url = UnversionedUrl(vs.Url),
            Name = vs.Name,
            NamePascal = vs.Name.ToPascalCase(),
            Title = vs.Title,
            Description = vs.Description,
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
            Types = ed.Type.Select(GetInfo).ToDictionary(i => i.Name),
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
