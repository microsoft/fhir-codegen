// <copyright file="PackageComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
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
using static Hl7.Fhir.Model.VerificationResult;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.PackageComparer;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.ComparisonUtils;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using System.Resources;
using Hl7.Fhir.Specification.Snapshot;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGen.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class PackageComparer
{
    private const string _crossDefinitionVersion = "1.0.0";

    private DefinitionCollection _source;
    private DefinitionCollection _target;

    private CrossVersionMapCollection? _crossVersion = null;

    private Hl7.Fhir.Specification.Snapshot.SnapshotGenerator _snapshotGenerator;

    private Dictionary<string, CMR> _typeRelationships = [];

    private string _sourceShortVersion;
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

    internal static readonly HashSet<string> _exclusionBaseTypes =
    [
        "Element.id",
        "Extension",
    ];

    public PackageComparer(ConfigCompare config, DefinitionCollection source, DefinitionCollection target)
    {
        _config = config;
        _source = source;
        _target = target;

        _sourceShortVersion = source.FhirSequence.ToShortVersion();
        _sourceRLiteral = source.FhirSequence.ToRLiteral();
        _targetRLiteral = target.FhirSequence.ToRLiteral();

        _firelySerializerOptions = new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector).Pretty();

        _snapshotGenerator = new(_source);

        //if (!string.IsNullOrEmpty(config.OllamaUrl) &&
        //    !string.IsNullOrEmpty(config.OllamaModel))
        //{
        //    _httpClient = new HttpClient();
        //    _ollamaUri = config.OllamaUrl.EndsWith("generate", StringComparison.OrdinalIgnoreCase)
        //        ? new Uri(config.OllamaUrl)
        //        : new Uri(new Uri(config.OllamaUrl), "api/generate");
        //}
    }

    /// <summary>
    /// Compares this optional bool object to another to determine their relative ordering.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="compareExtensions">(Optional) The bool to compare to this object.</param>
    /// <returns>
    /// PackageComparison record with results based on the Source (left) package.
    /// </returns>
    public PackageComparison Compare()
    {
        Console.WriteLine(
            $"Comparing {_source.MainPackageId}#{_source.MainPackageVersion}" +
            $" and {_target.MainPackageId}#{_target.MainPackageVersion}");

        // check for loading cross-version maps
        if (!string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            _crossVersion = new(_source, _target);

            if (!_crossVersion.TryLoadCrossVersionMaps(_config.CrossVersionMapSourcePath))
            {
                Console.WriteLine("Failed to load requested cross-version maps");
                _crossVersion = null;
                //throw new Exception("Failed to load requested cross-version maps");
            }
        }

        // check if we are saving cross version maps and did not load any
        if (_crossVersion == null)
        {
            // create our cross-version map collection
            _crossVersion = new(_source, _target);
        }


        // need to expand every value set for comparison
        Dictionary<string, ValueSet> vsLeft = GetValueSets(_source);
        _vsComparisons = CompareValueSets(vsLeft, GetValueSets(_target));

        Dictionary<string, List<PrimitiveTypeComparison>> primitiveComparisons = ComparePrimitives(_source.PrimitiveTypesByName, _target.PrimitiveTypesByName);
        _crossVersion?.UpdateDataTypeMap(primitiveComparisons);

        Dictionary<string, List<StructureComparison>> complexTypeComparisons = CompareStructures(_source.ComplexTypesByName, _target.ComplexTypesByName);
        _crossVersion?.UpdateDataTypeMap(complexTypeComparisons);

        Dictionary<string, List<StructureComparison>> resources = CompareStructures(_source.ResourcesByName, _target.ResourcesByName);

        Dictionary<string, List<StructureComparison>> extensions = CompareStructures(_source.ExtensionsByUrl, _target.ExtensionsByUrl);

        // TODO(ginoc): Logical models are tracked by URL in collections, but structure mapping is done by name.
        //Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> logical = Compare(FhirArtifactClassEnum.LogicalModel, _left.LogicalModelsByUrl, _right.LogicalModelsByUrl);

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
            Extensions = extensions,
        };

        return packageComparison;
    }

    public string GetConfiguredOutputDirectory(string? additionalPathComponent = null)
    {
        bool useCrossVersionDest = !string.IsNullOrEmpty(_config.CrossVersionMapDestinationPath);

        string baseDir = useCrossVersionDest
            ? _config.CrossVersionMapDestinationPath
            : !string.IsNullOrEmpty(_config.OutputDirectory)
            ? _config.OutputDirectory
            : throw new Exception("Cannot write files without an OutputDirectory or CrossVersionMapDestinationPath");

        if (!Path.IsPathRooted(baseDir))
        {
            baseDir = FileSystemUtils.FindRelativeDir(string.Empty, baseDir, true);
        }

        if (!string.IsNullOrEmpty(additionalPathComponent))
        {
            return useCrossVersionDest
                ? Path.Combine(baseDir, $"{_sourceRLiteral}_{_targetRLiteral}", additionalPathComponent)
                : Path.Combine(baseDir, additionalPathComponent);
        }

        return useCrossVersionDest
            ? Path.Combine(baseDir, $"{_sourceRLiteral}_{_targetRLiteral}")
            : baseDir;
    }

    public void WriteComparisonResultJson(PackageComparison packageComparison, string? outputDir = null)
    {
        outputDir ??= GetConfiguredOutputDirectory();
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string jsonFilename = Path.Combine(outputDir, "comparison.json");

        using FileStream jsonFs = new(jsonFilename, FileMode.Create, FileAccess.Write);
        using Utf8JsonWriter jsonWriter = new(jsonFs, new JsonWriterOptions() { Indented = false, });
        {
            JsonSerializer.Serialize(jsonWriter, packageComparison);
        }
    }

    public void WriteMarkdownFiles(PackageComparison packageComparison, string? outputDir = null)
    {
        outputDir ??= GetConfiguredOutputDirectory("pages");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // build our filename
        string mdFilename = "overview.md";

        string mdFullFilename = Path.Combine(outputDir, mdFilename);

        using ExportStreamWriter mdWriter = CreateMarkdownWriter(mdFullFilename);

        if (packageComparison.ValueSets.Count != 0)
        {
            WriteComparisonOverview(mdWriter, "Value Sets", packageComparison.ValueSets);

            string mdSubDir = Path.Combine(outputDir, "ValueSets");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<ValueSetComparison> vcs in packageComparison.ValueSets.Values)
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
        }

        if (packageComparison.PrimitiveTypes.Count != 0)
        {
            WriteComparisonOverview(mdWriter, "Primitive Types", packageComparison.PrimitiveTypes);

            string mdSubDir = Path.Combine(outputDir, "PrimitiveTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<PrimitiveTypeComparison> cs in packageComparison.PrimitiveTypes.Values)
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

        if (packageComparison.ComplexTypes.Count != 0)
        {
            WriteComparisonOverview(mdWriter, "Complex Types", packageComparison.ComplexTypes);

            string mdSubDir = Path.Combine(outputDir, "ComplexTypes");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<StructureComparison> vcs in packageComparison.ComplexTypes.Values)
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
        }

        if (packageComparison.Resources.Count != 0)
        {
            WriteComparisonOverview(mdWriter, "Resources", packageComparison.Resources);

            string mdSubDir = Path.Combine(outputDir, "Resources");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<StructureComparison> vcs in packageComparison.Resources.Values)
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
        }

        if (packageComparison.Extensions.Count != 0)
        {
            WriteComparisonOverview(mdWriter, "Extensions", packageComparison.Extensions);

            string mdSubDir = Path.Combine(outputDir, "Extensions");
            if (!Directory.Exists(mdSubDir))
            {
                Directory.CreateDirectory(mdSubDir);
            }

            foreach (List<StructureComparison> vcs in packageComparison.Extensions.Values)
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
        }

        // TODO(ginoc): Logical models are tracked by URL in collections, but structure mapping is done by name.
        //WriteComparisonOverview(mdWriter, "Logical Models", logical.Values);

        //mdSubDir = Path.Combine(outputDir, "LogicalModels");
        //if (!Directory.Exists(mdSubDir))
        //{
        //    Directory.CreateDirectory(mdSubDir);
        //}

        //foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in logical.Values)
        //{
        //string filename = Path.Combine(mdSubDir, $"{c.CompositeName}.md");

        //    using ExportStreamWriter writer = CreateMarkdownWriter(filename);
        //    {
        //        WriteComparisonFile(writer, string.Empty, c);
        //    }
        //}

        mdWriter.Flush();
        mdWriter.Close();
        mdWriter.Dispose();
    }

    public void WriteMapFiles(PackageComparison packageComparison, string? outputDir = null, ConfigCompare.ComparisonMapSaveStyle? mapSaveStyle = null)
    {
        outputDir ??= GetConfiguredOutputDirectory("maps");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        mapSaveStyle ??= _config.MapSaveStyle;

        if (packageComparison.ValueSets.Count != 0)
        {
            // write out the value set maps
            string mapSubDir = Path.Combine(outputDir, "ValueSets");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteValueSetMaps(mapSubDir, packageComparison.ValueSets.Values.SelectMany(vl => vl.Select(v => v)));
        }

        if ((packageComparison.PrimitiveTypes.Count != 0) || (packageComparison.ComplexTypes.Count != 0))
        {
            // write out the data type map
            WriteDataTypeMap(outputDir, packageComparison.PrimitiveTypes, packageComparison.ComplexTypes);
        }

        if (packageComparison.ComplexTypes.Count != 0)
        {
            string mapSubDir = Path.Combine(outputDir, "ComplexTypes");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteStructureBasedConceptMaps(mapSubDir, packageComparison.ComplexTypes.Values.SelectMany(l => l.Select(s => s)));
        }

        if (packageComparison.Resources.Count != 0)
        {
            // write out the resource type map
            WriteResourceTypeMap(outputDir, packageComparison.Resources);

            string mapSubDir = Path.Combine(outputDir, "Resources");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteStructureBasedConceptMaps(mapSubDir, packageComparison.Resources.Values.SelectMany(l => l.Select(s => s)));
        }

        if (packageComparison.Extensions.Count != 0)
        {
            string mapSubDir = Path.Combine(outputDir, "Extensions");
            if (!Directory.Exists(mapSubDir))
            {
                Directory.CreateDirectory(mapSubDir);
            }

            WriteStructureBasedConceptMaps(mapSubDir, packageComparison.Extensions.Values.SelectMany(l => l.Select(s => s)));
        }
    }

    public void WriteCrossVersionExtensionArtifacts(PackageComparison packageComparison, string? outputDir = null)
    {
        FhirJsonSerializationSettings jsonSettings = new FhirJsonSerializationSettings() { Pretty = true };

        outputDir ??= GetConfiguredOutputDirectory("xver");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // process value sets
        foreach (ValueSetComparison vsComparison in packageComparison.ValueSets.Values.SelectMany(l => l.Select(v => v)).OrderBy(vs => vs.CompositeName))
        {
            if (BuildCrossVersionValueSet(vsComparison) is ValueSet vs)
            {
                string filename = $"ValueSet-{vs.Id}.json";
                string path = Path.Combine(outputDir, filename);

                File.WriteAllText(path, vs.ToJson(jsonSettings));
            }
        }

        // determine the types we have that cannot be used as extension values
        Dictionary<string, List<string>> nonExtensionTypesAndContexts = [];
        HashSet<string> allowedExtTypes = [];

        if (_target.ResourcesByName.TryGetValue("Extension", out StructureDefinition? targetExtensionSd) &&
            targetExtensionSd.cgTryGetElementByPath("Extension.value[x]", out ElementDefinition? targetExtensionValueEd))
        {
            allowedExtTypes = new(targetExtensionValueEd.cgTypes().Keys);
        }

        Dictionary<string, string> complexTypeExtensionMap = [];

        // process complex types
        foreach (StructureComparison sdComparison in packageComparison.ComplexTypes.Values.SelectMany(l => l.Select(s => s)).OrderBy(s => s.CompositeName))
        {
            // check for relationships we can skip
            if ((sdComparison.Relationship == CMR.Equivalent) ||
                (sdComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
            {
                continue;
            }

            // grab the structure definition for the source of this comparison
            if (!_source.ComplexTypesByName.TryGetValue(sdComparison.Source.Id, out StructureDefinition? sourceSd))
            {
                throw new Exception($"Could not find listed source complex type: {sdComparison.Source.Id}");
            }

            // iterate over the elements in this comparison
            foreach (ElementComparison edComparison in sdComparison.ElementComparisons.Values)
            {
                // grab the element definition for the source of this comparison
                if (!sourceSd.cgTryGetElementByPath(edComparison.Source.Path, out ElementDefinition? sourceEd))
                {
                    throw new Exception($"Could not find listed source element: {edComparison.Source.Path}");
                }

                StructureDefinition? ext = BuildCrossVersionExtension(
                    sdComparison,
                    sourceSd,
                    edComparison,
                    sourceEd,
                    allowedExtTypes,
                    nonExtensionTypesAndContexts);
                if (ext != null)
                {
                    string filename = $"StructureDefinition-{ext.Id.Replace("%5Bx%5D", "")}.json";
                    string path = Path.Combine(outputDir, filename);

                    File.WriteAllText(path, ext.ToJson(jsonSettings));
                }

                // for data types ONLY, we do not want to nest into the type if it is new (resources can)
                if (sdComparison.Relationship == null)
                {
                    if (ext != null)
                    {
                        // add if we have a datatype extension, add it to our map
                        complexTypeExtensionMap.Add(sourceSd.Id, ext.Url);
                    }

                    break;
                }
            }
        }

        _ = _source.ResourcesByName.TryGetValue("Basic", out StructureDefinition? basicSd);

        // process resources
        foreach (StructureComparison sdComparison in packageComparison.Resources.Values.SelectMany(l => l.Select(s => s)).OrderBy(s => s.CompositeName))
        {
            // check for relationships we can skip
            if ((sdComparison.Relationship == CMR.Equivalent) ||
                (sdComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
            {
                continue;
            }

            // skip the Extension resource
            if (sdComparison.Source.Id == "Extension")
            {
                continue;
            }

            // grab the structure definition for the source of this comparison
            if (!_source.ResourcesByName.TryGetValue(sdComparison.Source.Id, out StructureDefinition? sourceSd))
            {
                throw new Exception($"Could not find listed source resource: {sdComparison.Source.Id}");
            }

            // iterate over the elements in this comparison
            foreach (ElementComparison edComparison in sdComparison.ElementComparisons.Values)
            {
                // grab the element definition for the source of this comparison
                if (!sourceSd.cgTryGetElementByPath(edComparison.Source.Path, out ElementDefinition? sourceEd))
                {
                    throw new Exception($"Could not find listed source element: {edComparison.Source.Path}");
                }

                // skip Element.id and Extension definitions
                if ((!string.IsNullOrEmpty(sourceEd.Base?.Path)) &&
                    _exclusionBaseTypes.Contains(sourceEd.Base!.Path))
                {
                    continue;
                }

                // if we are adding a new resource, we need to filter out elements that exist in Basic, but not the root
                if ((sdComparison.Relationship == null) && sourceEd.Path.Contains("."))
                {
                    string basicPath = sourceEd.Path.Replace(sdComparison.Source.Id, "Basic");
                    if (basicSd?.cgTryGetElementByPath(basicPath, out _) ?? false)
                    {
                        continue;
                    }
                }

                StructureDefinition? ext = BuildCrossVersionExtension(
                    sdComparison,
                    sourceSd,
                    edComparison,
                    sourceEd,
                    allowedExtTypes,
                    nonExtensionTypesAndContexts,
                    basicSd,
                    complexTypeExtensionMap);
                if (ext != null)
                {
                    string filename = $"StructureDefinition-{ext.Id.Replace("%5Bx%5D", "")}.json";
                    string path = Path.Combine(outputDir, filename);

                    File.WriteAllText(path, ext.ToJson(jsonSettings));
                }
            }
        }

        // TODO(ginoc): need to process complex types that did not need to be generated for differences but are needed because they are not valid extension types
    }

    private string ExtensionUrlForElementPath(string path) =>
        $"http://hl7.org/fhir/{_sourceShortVersion}/StructureDefinition/extension-{path.Replace("[", "%5B").Replace("]", "%5D")}";

    private StructureDefinition? BuildCrossVersionExtension(
        StructureComparison sdComparison,
        StructureDefinition sourceSd,
        ElementComparison edComparison,
        ElementDefinition sourceEd,
        HashSet<string> allowedExtensionTypes,
        Dictionary<string, List<string>> nonExtensionTypesAndContexts,
        StructureDefinition? basicSd = null,
        Dictionary<string, string>? complexTypeExtensionMap = null)
    {
        // check for relationships with no definitions
        if ((edComparison.Relationship == CMR.Equivalent) ||
            (edComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
        {
            return null;
        }

        // do not build extensions for extension elements
        if ((sourceEd.Type.Count == 1) && (sourceEd.Type[0].Code == "Extension"))
        {
            return null;
        }

        string extensionIdPath = edComparison.Source.Path.Replace("[", "%5B").Replace("]", "%5D");

        // determine the context based on the closest path that has a target
        StructureDefinition.ContextComponent context = new() { Type = StructureDefinition.ExtensionContextType.Element, };
        string searchPath = sourceEd.Path;
        while (!string.IsNullOrEmpty(searchPath))
        {
            // get the comparison for this path
            if (sdComparison.ElementComparisons.TryGetValue(searchPath, out ElementComparison? contextComparison))
            {
                // check for a target
                if (contextComparison.TargetMappings.FirstOrDefault()?.Target is ElementInfoRec ctxTarget)
                {
                    context.Expression = ctxTarget.Path;
                    break;
                }
            }

            // trim the path down
            int index = searchPath.LastIndexOf('.');
            if (index == -1)
            {
                searchPath = string.Empty;
            }
            else
            {
                searchPath = searchPath[..index];
            }
        }

        if (string.IsNullOrEmpty(context.Expression))
        {
            // check for a structure target, use basic if there is none
            context.Expression = sdComparison.Target?.Id ?? "Basic";
        }

        // complex types always have the context of element

        StructureDefinition ext = new()
        {
            Id = extensionIdPath,
            Url = ExtensionUrlForElementPath(extensionIdPath),
            Name = $"XVerExtension{edComparison.Source.Path}",
            Title = $"Cross-Version Extension for FHIR {_sourceRLiteral}:{edComparison.Source.Path} compared to FHIR {_targetRLiteral}",
            Description = edComparison.Message,
            Status = PublicationStatus.Draft,
            Experimental = true,
            FhirVersion = FHIRVersion.N5_0_0,
            Kind = StructureDefinition.StructureDefinitionKind.ComplexType,
            Abstract = false,
            Context = [ context ],
            Type = "Extension",
            BaseDefinition = "http://hl7.org/fhir/StructureDefinition/Extension",
            Derivation = StructureDefinition.TypeDerivationRule.Constraint,
            Differential = new()
            {
                Element = new(),
            },
        };

        // add this element and any children
        AddElementToExtension(
            ext,
            "Extension",
            sdComparison,
            sourceSd,
            sourceEd,
            allowedExtensionTypes,
            nonExtensionTypesAndContexts,
            edComparison.Message,
            basicSd,
            complexTypeExtensionMap);

        // create a new snapshot
        ext.Snapshot = new StructureDefinition.SnapshotComponent();
        ext.Snapshot.Element = _snapshotGenerator.GenerateAsync(ext).Result;

        return ext;
    }

    public (string? shortText, string? definition, string? comment) GetTextForExtensionElement(ElementDefinition ed, string? reason)
    {
        List<string> strings = [];

        if (!string.IsNullOrEmpty(ed.Short))
        {
            strings.Add(ed.Short);
        }

        if (!string.IsNullOrEmpty(ed.Definition) &&
            !ed.Definition.Equals(ed.Short, StringComparison.Ordinal) &&
            !ed.Definition.Equals(ed.Short + ".", StringComparison.Ordinal))
        {
            strings.Add(ed.Definition);
        }

        if (!string.IsNullOrEmpty(reason))
        {
            strings.Add(reason!);
        }

        if (!string.IsNullOrEmpty(ed.Comment) &&
            !ed.Comment.Equals(ed.Short, StringComparison.Ordinal) &&
            !ed.Comment.Equals(ed.Definition, StringComparison.Ordinal))
        {
            strings.Add(ed.Comment);
        }

        switch (strings.Count)
        {
            case 0:
                return (null, null, null);

            case 1:
                return (strings[0], null, null);

            case 2:
                return (strings[0], strings[1], null);

            default:
                return (strings[0], strings[1], string.Join("\n", strings.Skip(2)));
        }
    }

    private void AddElementToExtension(
        StructureDefinition ext,
        string elementPath,
        StructureComparison sdComparison,
        StructureDefinition sourceSd,
        ElementDefinition sourceEd,
        HashSet<string> allowedExtensionTypes,
        Dictionary<string, List<string>> nonExtensionTypesAndContexts,
        string? reason = null,
        StructureDefinition? basicSd = null,
        Dictionary<string, string>? complexTypeExtensionMap = null)
    {
        // do not build extensions for extension elements
        if (sourceEd.Type.Count == 1 && sourceEd.Type[0].Code == "Extension")
        {
            return;
        }

        // get the comparison info for this element
        ElementComparison edComparison = sdComparison.ElementComparisons[sourceEd.Path];

        (string? edShortText, string? edDefinition, string? edComment) = GetTextForExtensionElement(sourceEd, reason);

        ext.Differential.Element.Add(new()
        {
            Path = elementPath,
            Short = edShortText,
            Definition = edDefinition,
            Comment = edComment,
            Min = sourceEd.Min,
            Max = sourceEd.Max,
            IsModifier = sourceEd.IsModifier,
        });

        // check for children
        ElementDefinition[] children = sourceSd.cgElements(sourceEd.Path, false, false, true).ToArray();

        if (children.Length == 0)
        {
            Dictionary<string, string> sliceUrisByType = [];
            List<ElementDefinition.TypeRefComponent> valueTypes = [];
            List<string> promotedReferences = [];

            // get the differential types for this element
            List<ElementDefinition.TypeRefComponent> requestedValueTypes = GetDifferentialTypes(sourceEd.cgTypesForExt(), edComparison);

            foreach (ElementDefinition.TypeRefComponent tr in requestedValueTypes)
            {
                // check to see if this is in the complex extension map
                if (complexTypeExtensionMap?.TryGetValue(tr.Code, out string? ctUri) == true)
                {
                    // add a slice uri for this
                    sliceUrisByType.Add(tr.Code, ctUri);

                    continue;
                }

                // check to see if this points to a resource
                if (_source.ResourcesByName.ContainsKey(tr.Code))
                {
                    if (tr.Code.Contains('/'))
                    {
                        promotedReferences.Add(tr.Code);
                    }
                    else
                    {
                        promotedReferences.Add("http://hl7.org/fhir/StructureDefinition/" + tr.Code);
                    }

                    continue;
                }

                // TODO(ginoc): need to sort out what to do with types that are not allowed as extension values
                // check to see if this type is not allowed in extensions
                if (allowedExtensionTypes.Contains(tr.Code) == false)
                {
                    // add this type to the non-extension types
                    if (nonExtensionTypesAndContexts.TryGetValue(tr.Code, out List<string>? contexts) == false)
                    {
                        contexts = new();
                        nonExtensionTypesAndContexts.Add(tr.Code, contexts);
                    }

                    // for now, just add the path to our tracking dictionary
                    // TODO(ginoc): likely need the extension URL, but need to sort out nested URLs
                    contexts.Add(sourceEd.Path);

                    // add a slice uri for this
                    sliceUrisByType.Add(tr.Code, ExtensionUrlForElementPath(tr.Code));

                    continue;
                }

                // add this type
                valueTypes.Add(tr);
            }

            // check for the number of allowed types
            if (valueTypes.Count == 0)
            {
                // not allowed a value if there are no allowed types
                ext.Differential.Element.Add(new()
                {
                    Path = elementPath + ".value[x]",
                    Max = "0",
                });
            }
            else
            {
                // handle any promoted reference types
                if (promotedReferences.Count != 0)
                {
                    ElementDefinition.TypeRefComponent? valueReference = valueTypes.FirstOrDefault(t => t.Code == "Reference");
                    bool addReference = valueReference == null;
                    valueReference ??= new() { Code = "Reference", TargetProfile = [], };

                    // set our profiles to match both sets
                    valueReference.TargetProfile = promotedReferences.Union(valueReference.TargetProfile);

                    if (addReference)
                    {
                        valueTypes.Add(valueReference);
                    }
                }

                // TODO(ginoc): Bindings are incorrect - need to use the value sets we generate as differentials
                ext.Differential.Element.Add(new()
                {
                    Path = elementPath + ".value[x]",
                    Min = 1,
                    Type = valueTypes,
                    Binding = sourceEd.Binding,
                });
            }

            // check for any sub-extension profiles (complex types that do not exist)
            if (sliceUrisByType.Count != 0)
            {
                foreach ((string sliceType, string sliceUri) in sliceUrisByType)
                {
                    ext.Differential.Element.Add(new()
                    {
                        Path = elementPath + ".extension:" + sliceType,
                        SliceName = sliceType,
                        SliceIsConstraining = true,
                        Type = new() { new() { Code = "Extension", Profile = [ sliceUri ] } },
                    });
                }
            }
        }
        else
        {
            foreach (ElementDefinition child in children)
            {
                if  (basicSd != null)
                {
                    string basicPath = sourceEd.Path.Replace(sdComparison.Source.Id, "Basic");
                    if (basicSd.cgTryGetElementByPath(basicPath, out _))
                    {
                        continue;
                    }
                }

                // skip Element.id and Extension definitions
                if ((!string.IsNullOrEmpty(child.Base?.Path)) &&
                     _exclusionBaseTypes.Contains(child.Base!.Path))
                {
                    continue;
                }

                AddElementToExtension(
                    ext,
                    elementPath + ".extension:" + child.cgName(),
                    sdComparison,
                    sourceSd,
                    child,
                    allowedExtensionTypes,
                    nonExtensionTypesAndContexts,
                    basicSd: basicSd,
                    complexTypeExtensionMap: complexTypeExtensionMap);
            }
        }

        // add the URL based on root or sub-extension
        if (elementPath == "Extension")
        {
            ext.Differential.Element.Add(new()
            {
                Path = elementPath + ".url",
                Fixed = ext.UrlElement,
            });
        }
        else
        {
            ext.Differential.Element.Add(new()
            {
                Path = elementPath + ".url",
                Fixed = new FhirUri(sourceEd.cgName(removeChoiceMarker: false).Replace("[", "%5B").Replace("]", "%5D")),
            });
        }
    }

    private List<ElementDefinition.TypeRefComponent> GetDifferentialTypes(
        List<ElementDefinition.TypeRefComponent> source,
        ElementComparison edComparison)
    {
        List<ElementDefinition.TypeRefComponent> result = [];

        foreach (ElementDefinition.TypeRefComponent sourceTr in source)
        {
            ElementTypeComparison? etComparison = edComparison.TargetMappings.FirstOrDefault()?.TypeComparisons[sourceTr.Code];

            if (etComparison == null)
            {
                // add this type as is
                result.Add(sourceTr);
                continue;
            }

            // check for types we can skip
            if ((etComparison.Relationship == CMR.Equivalent) ||
                (etComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
            {
                continue;
            }

            // start with an empty type
            ElementDefinition.TypeRefComponent tr = new();

            // iterate over the targets to remove duplicate types/profiles/targetProfiles
            foreach (ElementTypeComparisonDetails details in etComparison.TargetTypes)
            {
                // check for types we should skip
                if ((details.Relationship == CMR.Equivalent) ||
                    (details.Relationship == CMR.SourceIsNarrowerThanTarget))
                {
                    continue;
                }

                // add the type
                tr.Code = sourceTr.Code;

                // check the profiles
                foreach (string profile in sourceTr.Profile)
                {
                    if (details.Target?.Profiles.Contains(profile) == false)
                    {
                        tr.ProfileElement.Add(profile);
                    }
                }

                // check the target profiles
                foreach (string targetProfile in sourceTr.TargetProfile)
                {
                    if (details.Target?.TargetProfiles.Contains(targetProfile) == false)
                    {
                        tr.TargetProfileElement.Add(targetProfile);
                    }
                }
            }

            if (!string.IsNullOrEmpty(tr.Code))
            {
                result.Add(tr);
                continue;
            }

            //// check to see if there is a matching type on both sides
            //if (filter.Find(fTR => fTR.Code == sourceTr.Code) is ElementDefinition.TypeRefComponent filterTr)
            //{
            //    if ((sourceTr.TargetProfile.Any() == false) && (filterTr.TargetProfile.Any() == false))
            //    {
            //        // no target profiles and types match, so skip this
            //        continue;
            //    }

            //    List<string> additionalProfiles = sourceTr.TargetProfile.Where(stp => filterTr.TargetProfile.Contains(stp) == false).ToList();

            //    // check for the source having any targets that the filter does not
            //    if (additionalProfiles.Count != 0)
            //    {
            //        // add this type, but only the different profiles
            //        ElementDefinition.TypeRefComponent filteredTr = (ElementDefinition.TypeRefComponent)sourceTr.DeepCopy();
            //        filteredTr.TargetProfile = additionalProfiles;
            //        result.Add(filterTr);
            //        continue;
            //    }

            //    // no additional profiles, so skip this
            //    continue;
            //}

            //// add this type
            //result.Add(sourceTr);
        }

        return result;
    }


    private ValueSet? BuildCrossVersionValueSet(ValueSetComparison comparison)
    {
        // check for relationships with no definitions
        if ((comparison.Relationship == CMR.Equivalent) ||
            (comparison.Relationship == CMR.SourceIsNarrowerThanTarget))
        {
            return null;
        }

        string sourceDashTarget = $"{_sourceRLiteral}-{_targetRLiteral}";
        string vsId = $"{sourceDashTarget}-{comparison.Source.Id}";

        ValueSet vs = new()
        {
            Url = $"http://hl7.org/fhir/uv/xver/ValueSet/{vsId}",
            Id = vsId,
            Version = _crossDefinitionVersion,
            Name = comparison.Source.Name,
            Title = comparison.Source.Title,
            Status = PublicationStatus.Draft,
            Experimental = true,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Description = "Cross-version difference of " + comparison.Source.Description,
            Purpose = comparison.Message,
            Compose = new()
            {
                Include = [],
            },
            Expansion = new()
            {
                Contains = [],
            },
        };

        Dictionary<string, ValueSet.ConceptSetComponent> composeIncludes = [];

        // determine concepts we need to add
        foreach (ConceptComparison cc in comparison.ConceptComparisons.Values)
        {
            // check for relationships with no definitions
            if ((cc.Relationship == CMR.Equivalent) ||
                (cc.Relationship == CMR.SourceIsNarrowerThanTarget))
            {
                continue;
            }

            // add to the expansion
            vs.Expansion.Contains.Add(new()
            {
                System = cc.Source.System,
                Version = cc.Source.Version,
                Code = cc.Source.Code,
                Display = cc.Source.Display,
                Extension = new()
                {
                    new Extension("http://hl7.org/fhir/uv/xver/StructureDefinition/cross-version-message", new FhirString(cc.Message)),
                },
            });

            // sort out what we need to add to the compose include
            string key = cc.Source.System + "|" + cc.Source.Version;

            if (!composeIncludes.TryGetValue(key, out ValueSet.ConceptSetComponent? concept))
            {
                concept = new();
                composeIncludes.Add(key, concept);
            }

            concept.Concept.Add(new()
            {
                Code = cc.Source.Code,
                Display = cc.Source.Display,
            });
        }

        // add the compose includes
        foreach ((string key, ValueSet.ConceptSetComponent concept) in composeIncludes)
        {
            string[] parts = key.Split('|');
            vs.Compose.Include.Add(new()
            {
                System = parts[0],
                Version = parts[1],
                Concept = concept.Concept,
            });
        }

        return vs;
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

    private void WriteStructureBasedConceptMaps(string outputDir, IEnumerable<StructureComparison> values)
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
            writer.WriteLine($"| {status} | {count} |");
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
            writer.WriteLine($"| {status} | {count} |");
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
            writer.WriteLine($"| {status} | {count} |");
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

        foreach (ConceptComparison cc in cRec.ConceptComparisons.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
        {
            if (cc.TargetMappings.Count == 0)
            {
                writer.WriteLine($"| {cc.Source.Code} | - | {cc.GetStatusString()} | {cc.Message} |");
                continue;
            }

            foreach (ConceptComparisonDetails cd in cc.TargetMappings)
            {
                writer.WriteLine($"| {cc.Source.Code} | {cd.Target?.Code ?? "-"} | {cd.GetStatusString()} | {cd.Message} |");
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

        foreach (ElementComparison ec in cRec.ElementComparisons.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
        {
            if (ec.TargetMappings.Count == 0)
            {
                writer.WriteLine($"| {ec.Source.Path} | - | {ec.GetStatusString()} | {ec.Message} |");
                continue;
            }

            foreach (ElementComparisonDetails cd in ec.TargetMappings)
            {
                writer.WriteLine($"| {ec.Source.Path} | {cd.Target?.Path ?? "-"} | {cd.GetStatusString()} | {cd.Message} |");
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


    private bool TryGetVsComparison(string sourceUrl, string targetUrl, [NotNullWhen(true)] out ValueSetComparison? valueSetComparison)
    {
        if (!_vsComparisons.TryGetValue(sourceUrl, out List<ValueSetComparison>? vcs))
        {
            valueSetComparison = null;
            return false;
        }

        valueSetComparison = vcs.FirstOrDefault(vc => vc.Target?.Url == targetUrl);
        return valueSetComparison != null;
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

            // check for something that has no counterpart
            if (comparisons.Count == 0)
            {
                comparisons.Add(new()
                {
                    Relationship = null,
                    Message = $"{sourceVs.Url} does not exist in target and has no mapping",
                    Source = new()
                    {
                        Url = sourceVs.Url,
                        Id = sourceVs.Id,
                        Name = sourceVs.Name,
                        NamePascal = sourceVs.Name.ToPascalCase(),
                        Title = sourceVs.Title,
                        Description = sourceVs.Description,
                    },
                    Target = null,
                    CompositeName = $"{_sourceRLiteral}-{sourceVs.Name}",
                    ConceptComparisons = sourceVs.cgGetFlatConcepts(_source).Select(c => new ConceptComparison()
                    {
                        Source = GetInfo(c),
                        TargetMappings = [],
                        Relationship = null,
                        Message = $"{c.Key} does not exist in target and has no mapping",
                    }).ToDictionary(c => c.Source.Code),
                });
            }
        }

        return results;
    }

    internal bool TryCompareValueSetConcepts(
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
                                        Version = targetVs.Version,
                                        Code = mapTargetElement.Code,
                                        Display = mapTargetElement.Display,
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
                            .Select(tc => tc.Target?.Code != targetCode
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
            cc.Relationship == CMR.SourceIsNarrowerThanTarget;

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

    private string MessageForPrimitiveRelationship(CMR? r, ConceptMap.SourceElementComponent se, ConceptMap.TargetElementComponent te) => r switch
    {
        null => $"{_sourceRLiteral} `{se.Code}` has no mapping into {_targetRLiteral} primitives.",
        CMR.Equivalent => $"{_sourceRLiteral} `{se.Code}` is equivalent to {_targetRLiteral} `{te.Code}`.",
        CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} `{se.Code}` is broader than {_targetRLiteral} {te.Code} and requires mapping choice. `{se.Code}` maps to {string.Join(" and ", se.Target.Select(t => $"`{t.Code}`"))}.",
        _ => $"{_sourceRLiteral} `{se.Code}` maps as {r} to the target {_targetRLiteral} `{te.Code}`.",
    };


    private string MessageForComparisonRelationship(CMR? r, StructureDefinition sourceSd, StructureDefinition targetSd) => r switch
    {
        null => $"There is no mapping from {_sourceRLiteral} `{sourceSd.Name}` to {_targetRLiteral} `{targetSd.Name}`.",
        CMR.Equivalent => $"{_sourceRLiteral} `{sourceSd.Name}` is equivalent to {_targetRLiteral} `{targetSd.Name}`.",
        CMR.SourceIsBroaderThanTarget => $"{_sourceRLiteral} `{sourceSd.Name}` is broader than {_targetRLiteral} `{targetSd.Name}` and requires mapping choices for conversion.",
        _ => $"{_sourceRLiteral} `{sourceSd.Name}` maps as {r} to {_targetRLiteral} `{targetSd.Name}`.",
    };

    private string UnversionedUrl(string url) => url.Contains('|') ? url.Split('|')[0] : url;


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

            string sourceName = sourceSd.Name.StartsWith("http")
                ? sourceSd.Id
                : sourceSd.Name;

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
                                string targetName = targetSd.Name.StartsWith("http")
                                    ? targetSd.Id
                                    : targetSd.Name;

                                comparisons.Add(new()
                                {
                                    SourceTypeLiteral = sourceCode,
                                    Source = sourceInfo,
                                    TargetTypeLiteral = mapTargetElement.Code,
                                    Target = GetInfo(targetSd),
                                    CompositeName = $"{_sourceRLiteral}-{sourceName.ToCamelCase()}-{_targetRLiteral}-{targetName.ToCamelCase()}",
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
                                        Id = sourceSd.Id,
                                        Name = mapTargetElement.Code,
                                        Url = string.Empty,
                                        Title = string.Empty,
                                        Description = string.Empty,
                                        Purpose = string.Empty,
                                        SnapshotCount = 0,
                                        DifferentialCount = 0,
                                    },
                                    CompositeName = $"{_sourceRLiteral}-{sourceName.ToCamelCase()}-{_targetRLiteral}-{mapTargetElement.Code.ToCamelCase()}",
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
                string targetName = targetSd.Name.StartsWith("http")
                    ? targetSd.Id
                    : targetSd.Name;

                comparisons.Add(new()
                {
                    SourceTypeLiteral = sourceCode,
                    Source = sourceInfo,
                    TargetTypeLiteral = sourceCode,
                    Target = GetInfo(targetSd),
                    CompositeName = $"{_sourceRLiteral}-{sourceName.ToCamelCase()}-{_targetRLiteral}-{targetName.ToCamelCase()}",
                    Relationship = CMR.Equivalent,
                    Message = $"{_sourceRLiteral} `{sourceCode}` is assumed equivalent to {_targetRLiteral} `{sourceCode}` (no map, but names match)",
                });
            }
        }

        return results;
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

            // check for something that has no counterpart
            if (comparisons.Count == 0)
            {
                string sourceName = sourceSd.Name.StartsWith("http") ? sourceSd.Id : sourceSd.Name;

                comparisons.Add(new()
                {
                    Relationship = null,
                    Message = $"{sourceSdName} does not exist in target and has no mapping",
                    Source = new()
                    {
                        Id = sourceSd.Id,
                        Name = sourceSd.Name,
                        Url = sourceSd.Url,
                        Title = sourceSd.Title,
                        Description = sourceSd.Description,
                        Purpose = sourceSd.Purpose,
                        SnapshotCount = sourceSd.Snapshot.Element.Count,
                        DifferentialCount = sourceSd.Differential.Element.Count,
                    },
                    Target = null,
                    CompositeName = $"{_sourceRLiteral}-{sourceName}",
                    ElementComparisons = sourceSd.cgElements().Select(e => new ElementComparison()
                    {
                        Source = GetInfo(e),
                        TargetMappings = [],
                        Relationship = null,
                        Message = $"{e.Path} does not exist in target and has no mapping",
                    }).ToDictionary(e => e.Source.Path),
                });
            }
        }

        return results;
    }

    internal bool TryCompareStructureElements(
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

        string sourceName = sourceSd.Name.StartsWith("http") ? sourceSd.Id.ToPascalCase() : sourceSd.Name.ToPascalCase();
        string targetName = targetSd.Name.StartsWith("http") ? targetSd.Id.ToPascalCase() : targetSd.Name.ToPascalCase();

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

        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> sourceTypes = sourceEd.cgTypes(coerceToR5: true);
        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> targetTypes = targetEd.cgTypes(coerceToR5: true);

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
                        else if (boundVsInfo.ConceptComparisons.Values.All(cc => cc.TargetMappings.Any(tc => tc.Target?.Code == cc.Source.Code)))
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

                HashSet<string> scratch = new HashSet<string>(targetTypeInfo.Profiles);

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

                scratch = new HashSet<string>(targetTypeInfo.TargetProfiles);

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


    private ConceptInfoRec GetInfo(FhirConcept c)
    {
        return new()
        {
            System = c.System,
            Version = c.Version,
            Code = c.Code,
            Display = c.Display,
            Description = c.Definition,
        };
    }

    private ValueSetInfoRec GetInfo(ValueSet vs)
    {
        return new()
        {
            Url = UnversionedUrl(vs.Url),
            Id = vs.Id,
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
            Types = ed.Type.Select(tr => GetInfo(tr.cgAsR5())).ToDictionary(i => string.IsNullOrEmpty(i.Name) ? ed.Path : i.Name),
        };
    }

    private StructureInfoRec GetInfo(StructureDefinition sd)
    {
        return new StructureInfoRec()
        {
            Id = sd.Id,
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
            Console.WriteLine($"TryAskOllama <<< caught: {ex.Message}{(string.IsNullOrEmpty(ex.InnerException?.Message) ? string.Empty : ex.InnerException!.Message)}");
        }

        guess = null;
        confidence = 0;
        return false;
    }
}
