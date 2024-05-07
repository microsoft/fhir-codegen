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

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class PackageComparer
{
    private IFhirPackageClient _cache;

    private DefinitionCollection _left;
    private DefinitionCollection _right;

    private HashSet<FhirArtifactClassEnum> _leftOnlyClasses = [];

    private DefinitionCollection? _maps;
    private Dictionary<string, List<string>> _valueSetConceptMaps = [];
    private Dictionary<string, ConceptMap> _elementConceptMaps = [];
    private string _mapCanonical = string.Empty;

    private Dictionary<string, CMR> _typeRelationships = [];

    private ConceptMap? _dataTypeMap = null;
    private ConceptMap? _resourceTypeMap = null;
    private ConceptMap? _elementMapV1 = null;

    private string _leftRLiteral;
    //private string _leftShortVersion;
    private string _rightRLiteral;
    //private string _rightShortVersion;

    private const string _leftTableLiteral = "Source";
    private const string _rightTableLiteral = "Destination";

    private readonly JsonSerializerOptions _firelySerializerOptions;

    private ConfigCompare _config;

    private HttpClient? _httpClient = null;
    private Uri? _ollamaUri = null;

    private Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> _vsComparisons = [];

    public record class PackagePathRenames
    {
        public required string PackageDirectiveLeft { get; init; }
        public required string PackageDirectiveRight { get; init; }
        public required Dictionary<string, string> LeftRightPath { get; init; }
    }

    private static readonly HashSet<string> _exclusionSet =
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

        if (!string.IsNullOrEmpty(_config.CrossVersionRepoPathV1))
        {
            if (!TryLoadFhirCrossVersionMaps())
            {
                throw new Exception("Failed to load requested cross-version maps");
            }
        }

        string outputDir = string.IsNullOrEmpty(_config.CrossVersionRepoPathV2)
            ? Path.Combine(_config.OutputDirectory, $"{_leftRLiteral}_{_rightRLiteral}")
            : Path.Combine(_config.CrossVersionRepoPathV2, $"{_leftRLiteral}_{_rightRLiteral}");

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
        _vsComparisons = CompareValueSets(FhirArtifactClassEnum.ValueSet, vsLeft, GetValueSets(_right, vsLeft));
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Value Sets", _vsComparisons.Values);

            string mdSubDir = Path.Combine(pageDir, "ValueSets");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (ComparisonRecord<ValueSetInfoRec, ConceptInfoRec> c in _vsComparisons.Values)
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
            if (mdWriter is not null)
            {
                WriteValueSetMap(conceptMapDir, _vsComparisons.Values);
            }

            string mapSubDir = Path.Combine(conceptMapDir, "ValueSets");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteValueSetMaps(mapSubDir, _vsComparisons.Values);
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec>> primitives = ComparePrimitives(FhirArtifactClassEnum.PrimitiveType, _left.PrimitiveTypesByName, _right.PrimitiveTypesByName);
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Primitive Types", primitives.Values);

            string mdSubDir = Path.Combine(pageDir, "PrimitiveTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (ComparisonRecord<StructureInfoRec> c in primitives.Values)
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

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes = CompareStructures(FhirArtifactClassEnum.ComplexType, _left.ComplexTypesByName, _right.ComplexTypesByName);
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Complex Types", complexTypes.Values);

            string mdSubDir = Path.Combine(pageDir, "ComplexTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in complexTypes.Values)
            {
                //string name = GetName(c.Left, c.Right);
                //string filename = Path.Combine(subDir, $"{name}.md");
                string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }

            string mapSubDir = Path.Combine(conceptMapDir, "ComplexTypes");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteStructureMaps(mapSubDir, complexTypes.Values);
        }

        // write out the data type map
        if (mdWriter is not null)
        {
            WriteDataTypeMap(conceptMapDir, primitives.Values, complexTypes.Values);
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources = CompareStructures(FhirArtifactClassEnum.Resource, _left.ResourcesByName, _right.ResourcesByName);
        if (mdWriter is not null)
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
            if (mdWriter is not null)
            {
                WriteResourceTypeMap(conceptMapDir, resources.Values);
            }

            string mapSubDir = Path.Combine(conceptMapDir, "Resources");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteStructureMaps(mapSubDir, resources.Values);
        }

        // TODO(ginoc): Logical models are tracked by URL in collections, but structure mapping is done by name.
        //Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> logical = Compare(FhirArtifactClassEnum.LogicalModel, _left.LogicalModelsByUrl, _right.LogicalModelsByUrl);
        //if (mdWriter is not null)
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

        if (mdWriter is not null)
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

    private bool TryLoadFhirCrossVersionMaps()
    {
        Console.WriteLine($"Loading fhir-cross-version concept maps for conversion from {_leftRLiteral} to {_rightRLiteral}...");

        PackageLoader loader = new(_cache, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
            AutoLoadExpansions = false,
            ResolvePackageDependencies = false,
        });

        _mapCanonical = $"http://hl7.org/fhir/uv/xver/{_leftRLiteral.ToLowerInvariant()}-{_rightRLiteral.ToLowerInvariant()}";

        // create our maps collection
        _maps = new()
        {
            Name = "FHIR Cross Version Maps",
            FhirVersion = FHIRVersion.N5_0_0,
            FhirVersionLiteral = "5.0.0",
            FhirSequence = FhirReleases.FhirSequenceCodes.R5,
            MainPackageId = $"hl7.fhir.uv.xver.{_leftRLiteral.ToLowerInvariant()}-{_rightRLiteral.ToLowerInvariant()}",
            MainPackageVersion = "0.0.1",
            MainPackageCanonical = _mapCanonical,
        };

        if (!TryLoadCrossVersionConceptMaps(loader))
        {
            throw new Exception("Failed to load cross-version maps");
        }



        return true;
    }

    //private bool TryLoadCrossVersionStructureMaps(PackageLoader loader)
    //{
    //    string sourcePath = Path.Combine(_config.CrossVersionRepoPathV1, "temp", $"{_leftRLiteral}_{_rightRLiteral}");
    //    if (!Directory.Exists(sourcePath))
    //    {
    //        throw new DirectoryNotFoundException($"Could not find fhir-cross-version structure map source directory: {sourcePath}");
    //    }

    //    string filenameFilter = "*.json";

    //    string[] files = Directory.GetFiles(sourcePath, filenameFilter, SearchOption.TopDirectoryOnly);

    //    foreach (string filename in files)
    //    {
    //        try
    //        {
    //            object? loaded = loader.ParseContentsSystemTextStream("fhir+json", filename, typeof(StructureMap));
    //            if (loaded is not StructureMap sm)
    //            {
    //                Console.WriteLine($"Error loading {filename}: could not parse as StructureMap");
    //                continue;
    //            }

    //            string url = sm.Url ?? throw new Exception($"StructureMap {filename} is missing a URL");

    //            if (sm.Structure.Count < 2)
    //            {
    //                Console.WriteLine($"Skipping StructureMap {url} with {sm.Structure.Count} structures...");
    //                throw new Exception($"StructureMap {url} does not have enough structures: {string.Join(", ", sm.Structure.Select(s => $"{s.Url}:{s.Mode}:{s.Alias}"))}");
    //            }

    //            StructureMap.StructureComponent? leftStructure = sm.Structure.FirstOrDefault(s => s.Mode == StructureMap.StructureMapModelMode.Source);

    //            if (leftStructure is null)
    //            {
    //                throw new Exception($"StructureMap {url} does not have a source structure");
    //            }

    //            string leftName = leftStructure.Url.Split('/', '#')[^1];

    //            List<StructureMap.StructureComponent> rightStructures = sm.Structure.Where(s => s.Mode == StructureMap.StructureMapModelMode.Target).ToList();

    //            if (rightStructures.Count == 0)
    //            {
    //                throw new Exception($"StructureMap {url} does not have a target structure");
    //            }

    //            if (_maps!.StructureMapsByUrl.ContainsKey(url))
    //            {
    //                Console.WriteLine($"Skipping duplicate structure map definition for {url}...");
    //                continue;
    //            }

    //            // fix our structure URLs to be canonicals
    //            leftStructure.Url = $"{_left.MainPackageCanonical}/StructureDefinition/{leftStructure.Url}|{_left.MainPackageVersion}";

    //            // update our info
    //            sm.Id = $"{_leftRLiteral}-{leftName}-{_rightRLiteral}-{rightName}";
    //            sm.Url = url;
    //            sm.Name = "Map Concepts from " + leftName + " to " + rightName;
    //            sm.Title = $"Cross-version map for concepts from {_leftRLiteral} {leftName} to {_rightRLiteral} {rightName}";

    //            // try to manufacture correct value set URLs based on what we have
    //            sm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/{leftName}|{_left.MainPackageVersion}");
    //            sm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/{rightName}|{_right.MainPackageVersion}");

    //            string leftUrl = $"{_left.MainPackageCanonical}/ValueSet/{leftName}";
    //            string rightUrl = $"{_right.MainPackageCanonical}/ValueSet/{rightName}";

    //            if (sm.Group?.Count == 1)
    //            {
    //                sm.Group[0].Source = leftUrl;
    //                sm.Group[0].Target = rightUrl;
    //            }

    //            // add to our listing of value set maps
    //            if (_valueSetConceptMaps.TryGetValue(leftName, out List<string>? rightList))
    //            {
    //                rightList.Add(rightUrl);
    //            }
    //            else
    //            {
    //                _valueSetConceptMaps.Add(leftName, [rightUrl]);
    //            }



    //            // add this to our maps
    //            _maps!.AddStructureMap(sm, _maps.MainPackageId, _maps.MainPackageVersion);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error loading {filename}: {ex.Message}");
    //        }
    //    }

    //}

    private bool TryLoadCrossVersionConceptMaps(PackageLoader loader)
    {
        // TODO: load these!
        // prefer v2 maps
        //if (!string.IsNullOrEmpty(_config.CrossVersionRepoPathV2))
        //{
        //    if (!TryLoadCrossVersionConceptMapsV2(loader, ConceptMapType.ValueSetConcepts))
        //    {
        //        throw new Exception($"Failed to load cross-version value set concept maps");
        //    }

        //    if (!TryLoadCrossVersionConceptMapsV2(loader, ConceptMapType.DataTypeMap))
        //    {
        //        throw new Exception($"Failed to load cross-version type concept map");
        //    }

        //    if (!TryLoadCrossVersionConceptMapsV2(loader, ConceptMapType.ComplexTypeElements))
        //    {
        //        throw new Exception($"Failed to load cross-version complex type concept maps");
        //    }

        //    if (!TryLoadCrossVersionConceptMapsV2(loader, ConceptMapType.ResourceElements))
        //    {
        //        throw new Exception($"Failed to load cross-version resource concept maps");
        //    }

        //    return true;
        //}

        if (!string.IsNullOrEmpty(_config.CrossVersionRepoPathV1))
        {
            if (!TryLoadCrossVersionConceptMapsV1(loader, "codes"))
            {
                throw new Exception($"Failed to load cross-version code concept maps");
            }

            if (!TryLoadCrossVersionConceptMapsV1(loader, "types"))
            {
                throw new Exception($"Failed to load cross-version type concept maps");
            }

            if (!TryLoadCrossVersionConceptMapsV1(loader, "resources"))
            {
                throw new Exception($"Failed to load cross-version resource concept maps");
            }

            if (!TryLoadCrossVersionConceptMapsV1(loader, "elements"))
            {
                throw new Exception($"Failed to load cross-version element concept maps");
            }

            return true;
        }

        return false;
    }

    private enum ConceptMapType
    {
        ValueSetConcepts,
        DataTypeMap,
        ResourceTypeMap,
        ComplexTypeElements,
        ResourceElements,
    }

    private bool TryLoadCrossVersionConceptMapsV2(PackageLoader loader, ConceptMapType key)
    {
        string sourcePath = Path.Combine(_config.CrossVersionRepoPathV2, $"{_leftRLiteral}_{_rightRLiteral}", "maps");
        if (!Directory.Exists(sourcePath))
        {
            throw new DirectoryNotFoundException($"Could not find fhir-cross-version/{_leftRLiteral}_{_rightRLiteral}/maps directory: {sourcePath}");
        }

        string filenameFilter;

        switch (key)
        {
            case ConceptMapType.ValueSetConcepts:
                sourcePath = Path.Combine(sourcePath, "ValueSets");
                filenameFilter = $"ConceptMap-{_leftRLiteral}-*-{_rightRLiteral}-*.json";
                break;

            case ConceptMapType.DataTypeMap:
                filenameFilter = $"ConceptMap-{_leftRLiteral}-data-types-{_rightRLiteral}.json";
                break;

            case ConceptMapType.ResourceTypeMap:
                filenameFilter = $"ConceptMap-{_leftRLiteral}-resource-types-{_rightRLiteral}.json";
                break;

            case ConceptMapType.ComplexTypeElements:
                sourcePath = Path.Combine(sourcePath, "ComplexTypes");
                filenameFilter = $"ConceptMap-{_leftRLiteral}-*-{_rightRLiteral}-*.json";
                break;

            case ConceptMapType.ResourceElements:
                sourcePath = Path.Combine(sourcePath, "Resources");
                filenameFilter = $"ConceptMap-{_leftRLiteral}-*-{_rightRLiteral}-*.json";
                break;

            default:
                throw new ArgumentException($"Unknown key: {key}");
        }

        string[] files = Directory.GetFiles(sourcePath, filenameFilter, SearchOption.TopDirectoryOnly);

        foreach (string filename in files)
        {
            try
            {
                object? loaded = loader.ParseContentsSystemTextStream("fhir+json", filename, typeof(ConceptMap));
                if (loaded is not ConceptMap cm)
                {
                    Console.WriteLine($"Error loading {filename}: could not parse as ConceptMap");
                    continue;
                }

                // fix urls so we can find things
                switch (key)
                {
                    case ConceptMapType.ValueSetConcepts:
                        {
                            if (_maps!.ConceptMapsByUrl.ContainsKey(cm.Url))
                            {
                                Console.WriteLine($"Skipping duplicate concept map definition for {cm.Url}...");
                                continue;
                            }

                            string leftName;
                            if (cm.SourceScope is Canonical leftCanonical)
                            {
                                leftName = leftCanonical.Uri?.Split(['/', '#'])[^1] ?? throw new Exception($"Invalid left canonical in {cm.Url}");
                            }
                            else
                            {
                                throw new Exception($"Invalid left canonical in {cm.Url}");
                            }

                            string rightUrl;
                            if (cm.TargetScope is Canonical rightCanonical)
                            {
                                rightUrl = rightCanonical.Uri ?? throw new Exception($"Invalid right canonical in {cm.Url}");
                            }
                            else
                            {
                                throw new Exception($"Invalid right canonical in {cm.Url}");
                            }

                            // add to our listing of value set maps
                            if (_valueSetConceptMaps.TryGetValue(leftName, out List<string>? rightList))
                            {
                                rightList.Add(rightUrl);
                            }
                            else
                            {
                                _valueSetConceptMaps.Add(leftName, [rightUrl]);
                            }
                        }
                        break;

                    case ConceptMapType.DataTypeMap:
                        {
                            _dataTypeMap = cm;
                        }
                        break;

                    case ConceptMapType.ResourceTypeMap:
                        {
                            _resourceTypeMap = cm;
                        }
                        break;

                    case ConceptMapType.ComplexTypeElements:
                        {
                            string leftName;
                            if (cm.SourceScope is Canonical leftCanonical)
                            {
                                leftName = leftCanonical.Uri?.Split(['/', '#'])[^1] ?? throw new Exception($"Invalid left canonical in {cm.Url}");
                            }
                            else
                            {
                                throw new Exception($"Invalid left canonical in {cm.Url}");
                            }

                            _elementConceptMaps.Add(leftName, cm);
                        }
                        break;

                    case ConceptMapType.ResourceElements:
                        {
                            string leftName;
                            if (cm.SourceScope is Canonical leftCanonical)
                            {
                                leftName = leftCanonical.Uri?.Split(['/', '#'])[^1] ?? throw new Exception($"Invalid left canonical in {cm.Url}");
                            }
                            else
                            {
                                throw new Exception($"Invalid left canonical in {cm.Url}");
                            }

                            _elementConceptMaps.Add(leftName, cm);
                        }
                        break;
                }

                // add this to our maps
                _maps!.AddConceptMap(cm, _maps.MainPackageId, _maps.MainPackageVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
            }
        }

        return true;
    }

    private bool TryLoadCrossVersionConceptMapsV1(PackageLoader loader, string key)
    {
        string pathV1 = Path.Combine(_config.CrossVersionRepoPathV1, "input", key);
        if (!Directory.Exists(pathV1))
        {
            throw new DirectoryNotFoundException($"Could not find fhir-cross-version/input/{key} directory: {pathV1}");
        }

        string filenameFilter = "-" + _leftRLiteral[1..] + "to" + _rightRLiteral[1..] + ".json";

        string[] files = Directory.GetFiles(pathV1, $"ConceptMap*{filenameFilter}", SearchOption.TopDirectoryOnly);

        foreach (string filename in files)
        {
            try
            {
                object? loaded = loader.ParseContentsSystemTextStream("fhir+json", filename, typeof(ConceptMap));
                if (loaded is not ConceptMap cm)
                {
                    Console.WriteLine($"Error loading {filename}: could not parse as ConceptMap");
                    continue;
                }

                // fix urls so we can find things
                switch (key)
                {
                    case "codes":
                        {
                            (string url, string leftName, string rightName) = BuildCanonicalForValueSetMap(cm);

                            if (_maps!.ConceptMapsByUrl.ContainsKey(url))
                            {
                                Console.WriteLine($"Skipping duplicate concept map definition for {url}...");
                                continue;
                            }

                            // update our info
                            cm.Id = $"{_leftRLiteral}-{leftName}-{_rightRLiteral}-{rightName}";
                            cm.Url = url;
                            cm.Name = "Map Concepts from " + leftName + " to " + rightName;
                            cm.Title = $"Cross-version map for concepts from {_leftRLiteral} {leftName} to {_rightRLiteral} {rightName}";

                            // try to manufacture correct value set URLs based on what we have
                            cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/{leftName}|{_left.MainPackageVersion}");
                            cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/{rightName}|{_right.MainPackageVersion}");

                            string leftUrl = $"{_left.MainPackageCanonical}/ValueSet/{leftName}";
                            string rightUrl = $"{_right.MainPackageCanonical}/ValueSet/{rightName}";

                            if (cm.Group?.Count == 1)
                            {
                                cm.Group[0].Source = leftUrl;
                                cm.Group[0].Target = rightUrl;
                            }

                            // add to our listing of value set maps
                            if (_valueSetConceptMaps.TryGetValue(leftName, out List<string>? rightList))
                            {
                                rightList.Add(rightUrl);
                            }
                            else
                            {
                                _valueSetConceptMaps.Add(leftName, [rightUrl]);
                            }
                        }
                        break;
                    case "types":
                        {
                            string url = _maps!.MainPackageCanonical + "/ConceptMap/DataTypes";

                            // update our info
                            cm.Id = $"{_leftRLiteral}-datatypes-{_rightRLiteral}";
                            cm.Url = url;
                            cm.Name = $"Map Concepts representing data types from {_leftRLiteral} to {_rightRLiteral}";
                            cm.Title = $"Cross-version map for concepts of data types from {_leftRLiteral} to {_rightRLiteral}";

                            // try to manufacture correct value set URLs based on what we have
                            cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/data-types|{_left.MainPackageVersion}");
                            cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/data-types|{_right.MainPackageVersion}");

                            if (cm.Group?.Count == 1)
                            {
                                cm.Group[0].Source = $"{_left.MainPackageCanonical}/ValueSet/data-types";
                                cm.Group[0].Target = $"{_right.MainPackageCanonical}/ValueSet/data-types";
                            }

                            _dataTypeMap = cm;
                        }
                        break;
                    case "resources":
                        {
                            string url = _maps!.MainPackageCanonical + "/ConceptMap/Resources";

                            // update our info
                            cm.Id = $"{_leftRLiteral}-resources-{_rightRLiteral}";
                            cm.Url = url;
                            cm.Name = $"Map Concepts representing resource from {_leftRLiteral} to {_rightRLiteral}";
                            cm.Title = $"Cross-version map for concepts of resources from {_leftRLiteral} to {_rightRLiteral}";

                            // try to manufacture correct value set URLs based on what we have
                            cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/resources|{_left.MainPackageVersion}");
                            cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/resources|{_right.MainPackageVersion}");

                            if (cm.Group?.Count == 1)
                            {
                                cm.Group[0].Source = $"{_left.MainPackageCanonical}/ValueSet/resources";
                                cm.Group[0].Target = $"{_right.MainPackageCanonical}/ValueSet/resources";
                            }

                            _resourceTypeMap = cm;
                        }
                        break;
                    case "elements":
                        {
                            string url = _maps!.MainPackageCanonical + "/ConceptMap/Elements";

                            // update our info
                            cm.Id = $"{_leftRLiteral}-elements-{_rightRLiteral}";
                            cm.Url = url;
                            cm.Name = $"Map Concepts representing elements from {_leftRLiteral} to {_rightRLiteral}";
                            cm.Title = $"Cross-version map for concepts of elements from {_leftRLiteral} to {_rightRLiteral}";

                            // try to manufacture correct value set URLs based on what we have
                            cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/elements|{_left.MainPackageVersion}");
                            cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/elements|{_right.MainPackageVersion}");

                            if (cm.Group?.Count == 1)
                            {
                                cm.Group[0].Source = $"{_left.MainPackageCanonical}/ValueSet/elements";
                                cm.Group[0].Target = $"{_right.MainPackageCanonical}/ValueSet/elements";
                            }

                            _elementMapV1 = cm;
                        }
                        break;
                }

                // add this to our maps
                _maps!.AddConceptMap(cm, _maps.MainPackageId, _maps.MainPackageVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
            }
        }

        return true;
    }

    private void WriteValueSetMap(
    string outputDir,
    IEnumerable<ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> valueSets)
    {
        if (TryBuildConceptMapForValueSets(valueSets, out ConceptMap? cm))
        {
            string filename = Path.Combine(outputDir, $"ConceptMap-{_leftRLiteral}-value-sets-{_rightRLiteral}.json");

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


    private void WriteResourceTypeMap(
        string outputDir,
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources)
    {
        if (TryBuildConceptMapForResourceTypes(resources, out ConceptMap? cm))
        {
            string filename = Path.Combine(outputDir, $"ConceptMap-{_leftRLiteral}-resource-types-{_rightRLiteral}.json");

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

    private void WriteDataTypeMap(
        string outputDir,
        IEnumerable<ComparisonRecord<StructureInfoRec>> primitiveTypes,
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes)
    {
        if (TryBuildConceptMapForDataTypes(primitiveTypes, complexTypes, out ConceptMap? cm))
        {
            string filename = Path.Combine(outputDir, $"ConceptMap-{_leftRLiteral}-data-types-{_rightRLiteral}.json");

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

    private bool TryBuildConceptMapForDataTypes(
        IEnumerable<ComparisonRecord<StructureInfoRec>> primitiveTypes,
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes,
        [NotNullWhen(true)] out ConceptMap? cm)
    { 
        cm = new();

        string url = _maps!.MainPackageCanonical + "/ConceptMap/DataTypes";

        // update our info
        cm.Id = $"{_leftRLiteral}-data-types-{_rightRLiteral}";
        cm.Url = url;
        cm.Name = $"Map Concepts representing data types from {_leftRLiteral} to {_rightRLiteral}";
        cm.Title = $"Cross-version map for concepts of data types from {_leftRLiteral} to {_rightRLiteral}";

        // try to manufacture correct value set URLs based on what we have
        cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/data-types|{_left.MainPackageVersion}");
        cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/data-types|{_right.MainPackageVersion}");

        ConceptMap.GroupComponent group = new();

        group.Source = $"{_left.MainPackageCanonical}/ValueSet/data-types";
        group.Target = $"{_right.MainPackageCanonical}/ValueSet/data-types";

        // traverse primitive types
        foreach (ComparisonRecord<StructureInfoRec> c in primitiveTypes.Where(ci => ci.KeyInLeft == true))
        {
            // skip primitives that do not have serialization-based conversion info
            if ((c.TypeSerializationInfo is null) || (c.TypeSerializationInfo.Count == 0))
            {
                continue;
            }

            // add mappings based on primitive serialization info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.TypeSerializationInfo.Values.Where(tsi => tsi.Source == c.Key).Select(tsi => new ConceptMap.TargetElementComponent()
                {
                    Code = tsi.Target,
                    Display = _right.PrimitiveTypesByName.TryGetValue(tsi.Target, out StructureDefinition? tSd) ? tSd.Description : string.Empty,
                    Relationship = tsi.Relationship,
                    Comment = tsi.Message,
                }).ToList(),
            });
        }

        // traverse complex types
        foreach (IComparisonRecord<StructureInfoRec> c in complexTypes.Where(cti => cti.KeyInLeft == true).OrderBy(cti => cti.Key))
        {
            // put an entry with no map if there is no target
            if (c.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = c.Key,
                    NoMap = true,
                    Display = c.Left[0].Description,
                });

                continue;
            }

            // add mappings based on record info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Name,
                    Display = target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                }).ToList(),
            });
        }

        cm.Group.Add(group);
        return true;
    }

    private bool TryBuildConceptMapForValueSets(
    IEnumerable<ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> valueSets,
    [NotNullWhen(true)] out ConceptMap? cm)
    {
        cm = new();

        string url = _maps!.MainPackageCanonical + "/ConceptMap/ValueSets";

        // update our info
        cm.Id = $"{_leftRLiteral}-value-sets-{_rightRLiteral}";
        cm.Url = url;
        cm.Name = $"Map Concepts representing value set maps from {_leftRLiteral} to {_rightRLiteral}";
        cm.Title = $"Cross-version map for concepts of value sets from {_leftRLiteral} to {_rightRLiteral}";

        // try to manufacture correct value set URLs based on what we have
        cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/value-sets|{_left.MainPackageVersion}");
        cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/value-sets|{_right.MainPackageVersion}");

        ConceptMap.GroupComponent group = new();

        group.Source = $"{_left.MainPackageCanonical}/ValueSet/value-sets";
        group.Target = $"{_right.MainPackageCanonical}/ValueSet/value-sets";

        // traverse resources types
        foreach (IComparisonRecord<ValueSetInfoRec, ConceptInfoRec> c in valueSets.Where(cti => cti.KeyInLeft == true).OrderBy(cti => cti.Key))
        {
            // put an entry with no map if there is no target
            if (c.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = c.Key,
                    NoMap = true,
                    Display = c.Left[0].Description,
                });

                continue;
            }

            // add mappings based on record info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Name,
                    Display = target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                }).ToList(),
            });
        }

        cm.Group.Add(group);
        return true;
    }

    private bool TryBuildConceptMapForResourceTypes(
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources,
        [NotNullWhen(true)] out ConceptMap? cm)
    {
        cm = new();

        string url = _maps!.MainPackageCanonical + "/ConceptMap/ResourceTypes";

        // update our info
        cm.Id = $"{_leftRLiteral}-resource-types-{_rightRLiteral}";
        cm.Url = url;
        cm.Name = $"Map Concepts representing resource types from {_leftRLiteral} to {_rightRLiteral}";
        cm.Title = $"Cross-version map for concepts of resource types from {_leftRLiteral} to {_rightRLiteral}";

        // try to manufacture correct value set URLs based on what we have
        cm.SourceScope = new Canonical($"{_left.MainPackageCanonical}/ValueSet/resource-types|{_left.MainPackageVersion}");
        cm.TargetScope = new Canonical($"{_right.MainPackageCanonical}/ValueSet/resource-types|{_right.MainPackageVersion}");

        ConceptMap.GroupComponent group = new();

        group.Source = $"{_left.MainPackageCanonical}/ValueSet/resource-types";
        group.Target = $"{_right.MainPackageCanonical}/ValueSet/resource-types";

        // traverse resources types
        foreach (IComparisonRecord<StructureInfoRec> c in resources.Where(cti => cti.KeyInLeft == true).OrderBy(cti => cti.Key))
        {
            // put an entry with no map if there is no target
            if (c.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = c.Key,
                    NoMap = true,
                    Display = c.Left[0].Description,
                });

                continue;
            }

            // add mappings based on record info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Name,
                    Display = target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                }).ToList(),
            });
        }

        cm.Group.Add(group);
        return true;
    }

    private void WriteStructureMaps(string outputDir, IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> values)
    {
        foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in values)
        {
            if (TryBuildConceptMapForStructure(c, out ConceptMap? cm))
            {
                string filename = Path.Combine(outputDir, $"ConceptMap-{c.CompositeName}.json");

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
    }

    private bool TryBuildConceptMapForStructure(ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> cRec, [NotNullWhen(true)] out ConceptMap? cm)
    {
        // we cannot write maps that do not have both a source and a target
        if ((cRec.Left.Count == 0) || (cRec.Right.Count == 0))
        {
            cm = null;
            return false;
        }

        if (cRec.Left.Count > 1)
        {
            throw new Exception($"Cannot build concept map for structures with more than source: {cRec.Key}");
        }

        cm = new();

        string leftName = cRec.Left[0].Name.ToPascalCase();
        string rightNameUnderscore = string.Join("_", cRec.Right.Select(r => r.Name.ToPascalCase()));
        string rightNameAnd = string.Join(" and ", cRec.Right.Select(r => r.Name.ToPascalCase()));

        string url = BuildCanonicalForStructureDefinitionMap(leftName, rightNameUnderscore);

        // update our info
        cm.Id = cRec.CompositeName;
        cm.Url = url;
        cm.Name = "Map Concepts from " + leftName + " to " + rightNameAnd;
        cm.Title = $"Cross-version map for concepts from {_leftRLiteral} {leftName} to {_rightRLiteral} {rightNameAnd}";

        // try to manufacture correct value set URLs based on what we have
        cm.SourceScope = new Canonical($"{cRec.Left[0].Url}|{_left.MainPackageVersion}");
        cm.TargetScope = new Canonical($"{cRec.Right[0].Url}|{_right.MainPackageVersion}");

        ConceptMap.GroupComponent group = new();

        group.Source = cRec.Left[0].Url;
        group.Target = cRec.Right[0].Url;

        // traverse elements that exist in our source
        foreach ((string elementKey, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec> elementComparison) in cRec.Children.Where(kvp => kvp.Value.KeyInLeft == true).OrderBy(kvp => kvp.Key))
        {
            // put an entry with no map if there is no target
            if (elementComparison.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = elementKey,
                    NoMap = true,
                    Display = elementComparison.Left[0].Short,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = elementKey,
                Display = elementComparison.Left[0].Short,
                Target = elementComparison.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Path,
                    Display = target.Short,
                    Relationship = elementComparison.Relationship,
                    Comment = elementComparison.Message,
                }).ToList(),
            });
        }

        // check for a value set that could not map anything
        if (group.Element.Count == 0)
        {
            Console.WriteLine($"Not writing ConceptMap for {cRec.CompositeName} - no concepts are mapped to target!");
            cm = null;
            return false;
        }

        cm.Group.Add(group);
        return true;
    }

    private void WriteValueSetMaps(string outputDir, IEnumerable<ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> values)
    {
        foreach (ComparisonRecord<ValueSetInfoRec, ConceptInfoRec> c in values)
        {
            if (TryBuildConceptMapForValueSet(c, out ConceptMap? cm))
            {
                string filename = Path.Combine(outputDir, $"ConceptMap-{c.CompositeName}.json");

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
    }

    private bool TryBuildConceptMapForValueSet(ComparisonRecord<ValueSetInfoRec, ConceptInfoRec> cRec, [NotNullWhen(true)] out ConceptMap? cm)
    {
        // we cannot write maps that do not have both a source and a target
        if ((cRec.Left.Count == 0) || (cRec.Right.Count == 0))
        {
            cm = null;
            return false;
        }

        // for now, we only support 1:1 mappings
        if ((cRec.Left.Count != 1) || (cRec.Right.Count != 1))
        {
            throw new Exception("Cannot build concept map for value sets with more than one mapping");
        }

        if (cRec.Children.Count == 0)
        {
            throw new Exception("Cannot process a comparison with no mappings!");
        }

        cm = new();

        (string url, string leftName, string rightName) = BuildCanonicalForValueSetMap(cRec.Left[0].Url, cRec.Right[0].Url);

        // update our info
        cm.Id = cRec.CompositeName;
        cm.Url = url;
        cm.Name = "Map Concepts from " + leftName + " to " + rightName;
        cm.Title = $"Cross-version map for concepts from {_leftRLiteral} {leftName} to {_rightRLiteral} {rightName}";

        // try to manufacture correct value set URLs based on what we have
        cm.SourceScope = new Canonical($"{cRec.Left[0].Url}|{_left.MainPackageVersion}");
        cm.TargetScope = new Canonical($"{cRec.Right[0].Url}|{_right.MainPackageVersion}");

        ConceptMap.GroupComponent group = new();

        group.Source = cRec.Left[0].Url;
        group.Target = cRec.Right[0].Url;

        // traverse concepts that exist in our source
        foreach ((string conceptKey, ComparisonRecord<ConceptInfoRec> conceptComparison) in cRec.Children.Where(kvp => kvp.Value.KeyInLeft == true).OrderBy(kvp => kvp.Key))
        {
            // put an entry with no map if there is no target
            if (conceptComparison.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = conceptKey,
                    NoMap = true,
                    Display = conceptComparison.Left[0].Description,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = conceptKey,
                Display = conceptComparison.Left[0].Description,
                Target = conceptComparison.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Code,
                    Display = target.Description,
                    Relationship = conceptComparison.Relationship,
                    Comment = conceptComparison.Message,
                }).ToList(),
            });
        }

        // check for a value set that could not map anything
        if (group.Element.Count == 0)
        {
            Console.WriteLine($"Not writing ConceptMap for {cRec.CompositeName} - no concepts are mapped to target!");
            cm = null;
            return false;
        }

        cm.Group.Add(group);
        return true;
    }

    private (string url, string leftName, string rightName) BuildCanonicalForValueSetMap(ConceptMap cm) =>
        BuildCanonicalForValueSetMap(cm.Group?.FirstOrDefault()?.Source ?? cm.Url, cm.Group?.FirstOrDefault()?.Target ?? cm.Url);

    private (string url, string leftName, string rightName) BuildCanonicalForValueSetMap(string leftVsUrl, string rightVsUrl)
    {
        string leftVsMapName = leftVsUrl.Split('/', '#')[^1];
        string rightVsMapName = rightVsUrl.Split('/', '#')[^1];

        if (leftVsMapName == rightVsMapName)
        {
            return ($"{_mapCanonical}/ConceptMap/ValueSet-{leftVsMapName.ToPascalCase()}", leftVsMapName, rightVsMapName);
        }

        return ($"{_mapCanonical}/ConceptMap/ValueSet-{leftVsMapName.ToPascalCase()}-{rightVsMapName.ToPascalCase()}", leftVsMapName, rightVsMapName);
    }

    private string BuildCanonicalForStructureDefinitionMap(string leftName, string rightName)
    {
        if (leftName == rightName)
        {
            return $"{_mapCanonical}/ConceptMap/StructureDefinition-{leftName.ToPascalCase()}";
        }

        return $"{_mapCanonical}/ConceptMap/StructureDefinition-{leftName.ToPascalCase()}-{rightName.ToPascalCase()}";
    }

    private Dictionary<string, ValueSet> GetValueSets(DefinitionCollection dc, Dictionary<string, ValueSet>? other = null)
    {
        other ??= [];

        Dictionary<string, ValueSet> valueSets = [];

        HashSet<string> mappedSets = [];

        foreach ((string left, List<string> rights) in _valueSetConceptMaps)
        {
            mappedSets.Add(left);
            foreach (string right in rights)
            {
                mappedSets.Add(right);
            }
        }

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

    private void WriteStructureConceptMap(PackageComparison pc)
    {

    }

    private void WriteElementConceptMap(PackageComparison pc)
    {

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
    //    //if ((cRec.AdditionalSerializations is null) ||
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

            if ((cRec.TypeSerializationInfo is null) || (cRec.TypeSerializationInfo.Count == 0))
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

    private bool TryCompareConcept(
        FhirArtifactClassEnum artifactClass,
        string conceptCode,
        List<ConceptInfoRec> lSource,
        List<ConceptInfoRec> rSource,
        [NotNullWhen(true)] out ComparisonRecord<ConceptInfoRec>? c)
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
                Key = conceptCode,
                CompositeName = GetName(lSource, rSource),
                Left = [],
                KeyInLeft = false,
                Right = rSource,
                KeyInRight = true,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} new code {conceptCode} does not have a {_leftRLiteral} equivalent",
            };
            return true;
        }

        if (rSource.Count == 0)
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = conceptCode,
                CompositeName = GetName(lSource, rSource),
                Left = lSource,
                KeyInLeft = true,
                Right = [],
                KeyInRight = false,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} removed {_leftRLiteral} code {conceptCode}",
            };
            return true;
        }

        bool keyInLeft = lSource.Any(i => i.Code == conceptCode);
        bool keyInRight = rSource.Any(i => i.Code == conceptCode);

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
            ConceptInfoRec left = lSource.Count == 1 ? lSource[0] : lSource[sourceIndex];
            ConceptInfoRec right = rSource.Count == 1 ? rSource[0] : rSource[sourceIndex];

            if (left.Code != right.Code)
            {
                messages.Add($"{_leftRLiteral} code {left.Code} has a map to {_rightRLiteral} code {right.Code}");
                continue;
            }
            else if (maxIndex != 1)
            {
                messages.Add($"{_leftRLiteral} code {left.Code} has a map to {_rightRLiteral} code {right.Code}");
            }

            if (left.System != right.System)
            {
                messages.Add($"{_rightRLiteral} code {right.Code} has a different system than {_leftRLiteral} code {left.Code}");
            }

            if (left.Description != right.Description)
            {
                messages.Add($"{_rightRLiteral} code {right.Code} has a different description than {_leftRLiteral} code {left.Code}");
            }
        }

        string message;

        if (relationship == CMR.Equivalent)
        {
            message = keyInLeft
                ? $"{_rightRLiteral} code {rSource[0].Code} is equivalent to the {_leftRLiteral} code {conceptCode}"
                : $"{_rightRLiteral} new code {conceptCode} is equivalent to the {_leftRLiteral} code {lSource[0].Code}";
        }
        else if (messages.Count == 0)
        {
            message = keyInLeft
                ? $"{_leftRLiteral} code {conceptCode} maps as: {relationship} for {_rightRLiteral}"
                : $"{_rightRLiteral} new code {conceptCode} maps as: {relationship} for {_leftRLiteral}";
        }
        else
        {
            message = keyInLeft
                ? $"{_leftRLiteral} code {conceptCode} maps as: {relationship} for {_rightRLiteral} because {string.Join(" and ", messages)}"
                : $"{_rightRLiteral} new code {conceptCode} maps as: {relationship} for {_leftRLiteral} because {string.Join(" and ", messages)}";
        }

        // note that we can only be here if the codes have already matched, so we are always equivalent
        c = new()
        {
            ComparisonArtifactType = artifactClass,
            Key = conceptCode,
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

    private string GetName(List<ConceptInfoRec> l, List<ConceptInfoRec> r)
    {
        if (l.Count == 0 && r.Count == 0)
        {
            return string.Empty;
        }

        return
            (l.Count == 0 ? _leftRLiteral : $"{_leftRLiteral}_{string.Join('_', l.Select(i => i.Code.ForName()).Order())}") +
            "_" +
            (r.Count == 0 ? _rightRLiteral : $"{_rightRLiteral}_{string.Join('_', r.Select(i => i.Code.ForName()).Order())}");
    }

    //private (CMR initialRelationship, int maxIndex) GetInitialRelationship<T>(List<T> lSource, List<T> rSource)
    //{
    //    if (lSource.Count == 1 && rSource.Count == 1)
    //    {
    //        return (CMR.Equivalent, 1);
    //    }

    //    if (lSource.Count > 1)
    //    {
    //        return (CMR.SourceIsNarrowerThanTarget, lSource.Count);
    //    }

    //    return (CMR.SourceIsBroaderThanTarget, rSource.Count);
    //}

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

    private bool TryCompareValueSet(
        FhirArtifactClassEnum artifactClass,
        string url,
        List<ValueSetInfoRec> lSource,
        List<ValueSetInfoRec> rSource,
        Dictionary<string, ComparisonRecord<ConceptInfoRec>> conceptComparison,
        [NotNullWhen(true)] out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? c)
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
                Key = url,
                CompositeName = GetName(lSource, rSource),
                Left = [],
                KeyInLeft = false,
                Right = rSource,
                KeyInRight = true,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} added value set: {url}",
                Children = conceptComparison,
            };
            return true;
        }

        if (rSource.Count == 0)
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = url,
                CompositeName = GetName(lSource, rSource),
                Left = lSource,
                KeyInLeft = true,
                Right = [],
                KeyInRight = false,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightRLiteral} removed value set: {url}",
                Children = conceptComparison,
            };
            return true;
        }

        bool keyInLeft = lSource.Any(i => i.Url == url);
        bool keyInRight = rSource.Any(i => i.Url == url);

        if (_leftOnlyClasses.Contains(artifactClass) && !keyInLeft)
        {
            c = null;
            return false;
        }

        // check for all concepts being equivalent
        if ((lSource.Count == 1) &&
            (rSource.Count == 1) &&
            conceptComparison.Values.All(cc => cc.Relationship == CMR.Equivalent))
        {
            c = new()
            {
                ComparisonArtifactType = artifactClass,
                Key = url,
                CompositeName = GetName(lSource, rSource),
                Left = lSource,
                KeyInLeft = keyInLeft,
                Right = rSource,
                KeyInRight = keyInRight,
                NamedMatch = true,
                Relationship = CMR.Equivalent,
                Message = $"{_rightRLiteral} {rSource[0].Url} is equivalent to {_leftRLiteral} {lSource[0].Url}",
                Children = conceptComparison,
            };
            return true;
        }

        (CMR relationship, int maxIndex) = GetInitialRelationship(lSource, rSource);

        for (int sourceIndex = 0; sourceIndex < maxIndex; sourceIndex++)
        {
            ValueSetInfoRec left = lSource.Count == 1 ? lSource[0] : lSource[sourceIndex];
            ValueSetInfoRec right = rSource.Count == 1 ? rSource[0] : rSource[sourceIndex];

            foreach (ComparisonRecord<ConceptInfoRec> ec in conceptComparison.Values)
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
        }

        //string message = relationship switch
        //{
        //    CMR.Equivalent => $"{_rightRLiteral}:{url} is equivalent",
        //    CMR.RelatedTo => $"{_rightRLiteral}:{url} is related to {_leftRLiteral}:{url} (see concepts for details)",
        //    CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral}:{url} subsumes {_leftRLiteral}:{url}",
        //    CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral}:{url} is subsumed by {_leftRLiteral}:{url}",
        //    _ => $"{_rightRLiteral}:{url} is related to {_leftRLiteral}:{url} (see concepts for details)",
        //};

        string message;

        if (keyInLeft)
        {
            message = relationship switch
            {
                CMR.Equivalent => $"{_rightRLiteral} ValueSet {rSource[0].Url} is equivalent to the {_leftRLiteral} ValueSet {url}",
                CMR.RelatedTo => $"{_rightRLiteral} ValueSet {rSource[0].Url} is related to {_leftRLiteral} ValueSet {url} (see concepts for details)",
                CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral} ValueSet {rSource[0].Url} subsumes {_leftRLiteral} ValueSet {url}",
                CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral} ValueSet {rSource[0].Url} is subsumed by {_leftRLiteral} ValueSet {url}",
                _ => $"{_rightRLiteral} ValueSet {rSource[0].Url} is related to {_leftRLiteral} ValueSet {url} (see concepts for details)",
            };
        }
        else
        {
            message = relationship switch
            {
                CMR.Equivalent => $"{_rightRLiteral} new ValueSet {url} is equivalent to the {_leftRLiteral} ValueSet {lSource[0].Url}",
                CMR.RelatedTo => $"{_rightRLiteral} new ValueSet {url} is related to {_leftRLiteral} ValueSet {lSource[0].Url} (see concepts for details)",
                CMR.SourceIsNarrowerThanTarget => $"{_rightRLiteral} new ValueSet {url} subsumes {_leftRLiteral} ValueSet {lSource[0].Url}",
                CMR.SourceIsBroaderThanTarget => $"{_rightRLiteral} new ValueSet {url} is subsumed by {_leftRLiteral} ValueSet {lSource[0].Url}",
                _ => $"{_rightRLiteral} new ValueSet {url} is related to {_leftRLiteral} ValueSet {lSource[0].Url} (see concepts for details)",
            };
        }

        c = new()
        {
            ComparisonArtifactType = artifactClass,
            Key = url,
            CompositeName = GetName(lSource, rSource),
            Left = lSource,
            KeyInLeft = keyInLeft,
            Right = rSource,
            KeyInRight = keyInRight,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            Children = conceptComparison,
        };
        return true;
    }

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
        if (mappedRelationship is not null)
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
            if ((left.ValueSetBindingStrength is not null) || (right.ValueSetBindingStrength is not null))
            {
                if ((left.ValueSetBindingStrength != BindingStrength.Required) && (right.ValueSetBindingStrength == BindingStrength.Required))
                {
                    relationship = ApplyRelationship(relationship, CMR.RelatedTo);

                    if (left.ValueSetBindingStrength is null)
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
                    if (left.ValueSetBindingStrength is null)
                    {
                        messages.Add($"{right.Name} added a binding requirement - {right.ValueSetBindingStrength} {right.BindingValueSet}");
                    }
                    else if (right.ValueSetBindingStrength is null)
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
                        // check for same value set
                        if (unversionedLeft == unversionedRight)
                        {
                            // look for the value set comparison
                            if (_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? boundVsInfo))
                            {
                                // we are okay with equivalent and narrower
                                if (boundVsInfo.Relationship == CMR.Equivalent ||
                                    boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget)
                                {
                                    relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                                    messages.Add($"{right.Name} has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet} ({boundVsInfo.Relationship})");
                                }

                                // check to see if the codes are the same but the systems are different (ok in codes)
                                else if (boundVsInfo.Children.Values.All(cc => (cc.Left.Count == 1) && (cc.Right.Count == 1) && cc.Left[0].Code == cc.Right[0].Code))
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
                            else if (_exclusionSet.Contains(unversionedLeft))
                            {
                                relationship = ApplyRelationship(relationship, CMR.Equivalent);
                                messages.Add($"{right.Name} using {unversionedLeft} is exempted and assumed equivalent");
                            }
                            else
                            {
                                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                                messages.Add($"({right.Name} failed to compare required binding of {left.BindingValueSet})");
                            }
                        }
                        // since these are codes only, we can look for the value set comparisons and check codes across them
                        else if (!_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? leftVsI) ||
                                 !_vsComparisons.TryGetValue(unversionedRight, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? rightVsI))
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"({right.Name} failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                        }

                        // check for any codes from the left binding source not being present in the right binding destination
                        else if (leftVsI.Children.Values.Any(lc => (lc.Left.Count != 1) || (lc.Right.Count != 1) || !rightVsI.Children.Values.Any(rc => lc.Left[0].Code == rc.Right[0].Code)))
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"{right.Name} has INCOMPATIBLE required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                        }
                        else if (_exclusionSet.Contains(unversionedLeft) && _exclusionSet.Contains(unversionedRight))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{right.Name} using {unversionedRight} is exempted and assumed equivalent");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, leftVsI.Children.Count == rightVsI.Children.Count ? CMR.Equivalent : CMR.SourceIsNarrowerThanTarget);
                            messages.Add($"{right.Name} has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                        }
                    }

                    // check for any non-code types (need to match system)
                    if (typeComparison.Any(t => t.Key != "code"))
                    {
                        // check for same value set (non-code type)
                        if (unversionedLeft == unversionedRight)
                        {
                            // look for the value set comparison
                            if (!_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? boundVsInfo))
                            {
                                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                                messages.Add($"({right.Name} failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                            }
                            // we are okay with equivalent and narrower
                            else if (boundVsInfo.Relationship == CMR.Equivalent ||
                                     boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget)
                            {
                                relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                                messages.Add($"{right.Name} has compatible required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet} ({boundVsInfo.Relationship})");
                            }
                            else if (_exclusionSet.Contains(unversionedLeft))
                            {
                                relationship = ApplyRelationship(relationship, CMR.Equivalent);
                                messages.Add($"{right.Name} using {unversionedRight} is exempted and assumed equivalent");
                            }
                            else
                            {
                                relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                                messages.Add($"{right.Name} has INCOMPATIBLE required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet}");
                            }
                        }
                        // since these are not only codes, but are different value sets, we can look for the value set comparisons and check system+codes across them
                        else if (!_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? leftVs) ||
                                 !_vsComparisons.TryGetValue(unversionedRight, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? rightVs))
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"({right.Name} failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                        }
                        // check for any keys (system+code) from the left binding source not being present in the right binding destination
                        else if (leftVs.Children.Keys.Any(lk => !rightVs.Children.ContainsKey(lk)))
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"{right.Name} has INCOMPATIBLE required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet}");
                        }
                        else if (_exclusionSet.Contains(unversionedLeft) && _exclusionSet.Contains(unversionedRight))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{right.Name} using {unversionedRight} is exempted and assumed equivalent");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, leftVs.Children.Count == rightVs.Children.Count ? CMR.Equivalent : CMR.SourceIsNarrowerThanTarget);
                            messages.Add($"{right.Name} has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
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


    private Dictionary<string, ComparisonRecord<ConceptInfoRec>> CompareConcepts(
        FhirArtifactClassEnum artifactClass,
        IReadOnlyDictionary<string, FhirConcept> leftConcepts,
        IReadOnlyDictionary<string, FhirConcept> rightConcepts,
        ConceptMap? cm)
    {
        Dictionary<string, ConceptInfoRec> left = leftConcepts.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, ConceptInfoRec> right = rightConcepts.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ConceptInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        foreach (string conceptCode in keys)
        {
            List<ConceptInfoRec> leftInfoSource;
            List<ConceptInfoRec> rightInfoSource;

            // prefer using a map if we have one
            if (cm is not null)
            {
                HashSet<string> usedSourceCodes = [];
                HashSet<string> usedTargetCodes = [];

                leftInfoSource = [];
                rightInfoSource = [];

                // check to see if the source element has a map
                ConceptMap.SourceElementComponent? sourceMap = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Code == conceptCode).FirstOrDefault();

                // if we have a mapping from the current source, we want to use the target mappings
                if (sourceMap is not null)
                {
                    // pull information about our mapped source concept
                    if (!left.TryGetValue(conceptCode, out ConceptInfoRec? mapSourceInfo))
                    {
                        throw new Exception($"Concept {conceptCode} is mapped as a source but not defined in the left set");
                    }

                    leftInfoSource.Add(mapSourceInfo);
                    usedSourceCodes.Add(conceptCode);

                    // traverse the map targets to pull target information
                    foreach (ConceptMap.TargetElementComponent te in sourceMap.Target)
                    {
                        if (usedTargetCodes.Contains(te.Code))
                        {
                            continue;
                        }

                        if (!right.TryGetValue(te.Code, out ConceptInfoRec? mappedTargetInfo))
                        {
                            throw new Exception($"Concept {te.Code} is mapped as a target but not defined in right set");
                        }

                        rightInfoSource.Add(mappedTargetInfo);
                        usedTargetCodes.Add(te.Code);
                    }
                }

                // if we did not find a source mapping from the source, still add as a target concept if it is in the right set
                if ((usedTargetCodes.Count == 0) &&
                    right.TryGetValue(conceptCode, out ConceptInfoRec? rightConceptCodeInfo))
                {
                    rightInfoSource.Add(rightConceptCodeInfo);
                    usedTargetCodes.Add(conceptCode);
                }

                // also pull the list of target mappings to see if this code is mapped *from* any other source
                List<ConceptMap.SourceElementComponent> targetMaps = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Target.Any(t => usedTargetCodes.Contains(t.Code)))?.ToList() ?? [];

                // traverse all mappings that this target appears in
                foreach (ConceptMap.SourceElementComponent mapElement in targetMaps)
                {
                    // check if this has already been added
                    if (!usedSourceCodes.Contains(mapElement.Code))
                    {
                        // pull information about our mapped source concept
                        if (!left.TryGetValue(mapElement.Code, out ConceptInfoRec? mapSourceInfo))
                        {
                            throw new Exception($"Concept {mapElement.Code} is mapped as a source but not defined in the left set");
                        }

                        leftInfoSource.Add(mapSourceInfo);
                        usedSourceCodes.Add(mapElement.Code);
                    }

                    // traverse the map targets to pull target information
                    foreach (ConceptMap.TargetElementComponent te in mapElement.Target)
                    {
                        if (usedTargetCodes.Contains(te.Code))
                        {
                            continue;
                        }

                        if (!right.TryGetValue(te.Code, out ConceptInfoRec? mappedTargetInfo))
                        {
                            throw new Exception($"Concept {te.Code} is mapped as a target but not defined in right set");
                        }

                        rightInfoSource.Add(mappedTargetInfo);
                        usedTargetCodes.Add(te.Code);
                    }
                }
            }
            else
            {
                // without a map, just try to get the matching source and destination codes
                leftInfoSource = left.TryGetValue(conceptCode, out ConceptInfoRec? leftInfo) ? [leftInfo] : [];
                rightInfoSource = right.TryGetValue(conceptCode, out ConceptInfoRec? rightInfo) ? [rightInfo] : [];
            }

            if (TryCompareConcept(artifactClass, conceptCode, leftInfoSource, rightInfoSource, out ComparisonRecord<ConceptInfoRec>? c))
            {
                comparison.Add(conceptCode, c);
            }
        }

        return comparison;
    }

    private Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> CompareValueSets(
        FhirArtifactClassEnum artifactClass,
        IReadOnlyDictionary<string, ValueSet> leftInput,
        IReadOnlyDictionary<string, ValueSet> rightInput)
    {
        Dictionary<string, ValueSetInfoRec> left = leftInput.ToDictionary(kvp => _left.UnversionedUrlForVs(kvp.Key), kvp => GetInfo(kvp.Value));
        Dictionary<string, ValueSetInfoRec> right = rightInput.ToDictionary(kvp => _right.UnversionedUrlForVs(kvp.Key), kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        foreach (string url in keys)
        {
            List<string> rightUrls = _valueSetConceptMaps.TryGetValue(url, out List<string>? vsTargets) ? vsTargets : [];
            if (rightUrls.Count == 0)
            {
                rightUrls = [url];
            }
            else if (!rightUrls.Contains(url))
            {
                rightUrls.Add(url);
            }

            foreach (string rightUrl in rightUrls)
            {
                ConceptMap? cm = null;

                // check to see if we have loaded maps
                if (_maps is not null)
                {
                    // get our map url
                    (string mapUrl, string leftVsName, string rightVsName) = BuildCanonicalForValueSetMap(url, rightUrl);

                    if (_maps.TryResolveByCanonicalUri(mapUrl, out Resource? r) &&
                        (r is ConceptMap))
                    {
                        // we have a map, so we need to add it to our input
                        cm = r as ConceptMap;
                    }
                }

                List<ValueSetInfoRec> leftInfoSource = left.TryGetValue(url, out ValueSetInfoRec? leftInfo) ? [leftInfo] : [];
                List<ValueSetInfoRec> rightInfoSource = right.TryGetValue(rightUrl, out ValueSetInfoRec? rightInfo) ? [rightInfo] : [];

                Dictionary<string, FhirConcept> leftConcepts = [];
                Dictionary<string, FhirConcept> rightConcepts = [];

                if (leftInput.TryGetValue(url, out ValueSet? leftVs))
                {
                    IEnumerable<FhirConcept> flat = leftVs.cgGetFlatConcepts(_left);
                    foreach (FhirConcept concept in flat)
                    {
                        _ = leftConcepts.TryAdd(concept.Code, concept);
                    }

                    leftInfo = leftInfo! with { ConceptCount = leftConcepts.Count };
                    left[url] = leftInfo;
                }

                if (rightInput.TryGetValue(rightUrl, out ValueSet? rightVs))
                {
                    IEnumerable<FhirConcept> flat = rightVs.cgGetFlatConcepts(_right);
                    foreach (FhirConcept concept in flat)
                    {
                        _ = rightConcepts.TryAdd(concept.Code, concept);
                    }

                    rightInfo = rightInfo! with { ConceptCount = rightConcepts.Count };
                    right[rightUrl] = rightInfo;
                }

                // compare our concepts
                Dictionary<string, ComparisonRecord<ConceptInfoRec>> conceptComparison = CompareConcepts(artifactClass, leftConcepts, rightConcepts, cm);

                if (TryCompareValueSet(artifactClass, url, leftInfoSource, rightInfoSource, conceptComparison, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? c))
                {
                    comparison.Add(url, c);
                }
            }
        }

        return comparison;
    }

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
        Dictionary<string, (string target, CMR relationship)> elementMappings)
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
            if (elementMappings.TryGetValue(edPath, out (string target, CMR relationship) mapped))
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
                FhirArtifactClassEnum.PrimitiveType => _dataTypeMap,
                _ => null
            };

            // prefer using a map if we have one
            if (cm is not null)
            {
                HashSet<string> usedSourceNames = [];
                HashSet<string> usedTargetNames = [];

                leftSource = [];
                rightSource = [];

                // check to see if the source element has a map
                ConceptMap.SourceElementComponent? sourceMap = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Code == sdName).FirstOrDefault();

                // if we have a mapping from the current source, we want to use the target mappings
                if (sourceMap is not null)
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
                // for primitives, a match needs to represent equivalence for sanity elsewhere
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

            if (rightRelationship is null)
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
                FhirArtifactClassEnum.ComplexType => _dataTypeMap,
                FhirArtifactClassEnum.Resource => _resourceTypeMap,
                _ => null
            };

            // prefer using a map if we have one
            if (cm is not null)
            {
                HashSet<string> usedSourceNames = [];
                HashSet<string> usedTargetNames = [];

                leftSource = [];
                rightSource = [];

                // check to see if the source element has a map
                ConceptMap.SourceElementComponent? sourceMap = cm?.Group.FirstOrDefault()?.Element.Where(e => e.Code == sdName).FirstOrDefault();

                // if we have a mapping from the current source, we want to use the target mappings
                if (sourceMap is not null)
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

                            rightSource.Add((rightInput[te.Code], mappedTargetInfo));
                            usedTargetNames.Add(te.Code);
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
            ConceptMap? elementMap = artifactClass switch
            {
                FhirArtifactClassEnum.ComplexType => _elementConceptMaps.TryGetValue(sdName, out ConceptMap? map) ? map : _elementMapV1,
                FhirArtifactClassEnum.Resource => _elementMapV1,
                _ => null
            };

            Dictionary<string, (string target, CMR relationship)> elementMappings = [];

            if (elementMap is not null)
            {
                foreach (ConceptMap.SourceElementComponent sourceElement in elementMap.Group.FirstOrDefault()?.Element ?? [])
                {
                    foreach (ConceptMap.TargetElementComponent targetElement in sourceElement.Target)
                    {
                        elementMappings.Add(sourceElement.Code, (targetElement.Code, targetElement.Relationship ?? CMR.RelatedTo));
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
