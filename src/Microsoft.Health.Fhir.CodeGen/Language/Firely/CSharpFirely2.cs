// <copyright file="CSharpFirely2.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Ncqa.Cql.Model;
using static Microsoft.Health.Fhir.CodeGen.Language.Firely.CSharpFirelyCommon;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.Language.Firely;

public sealed class CSharpFirely2 : ILanguage, IFileHashTestable
{
    private bool _generateHashesInsteadOfOutput = false;
    bool IFileHashTestable.GenerateHashesInsteadOfOutput
    {
        get => _generateHashesInsteadOfOutput;
        set => _generateHashesInsteadOfOutput = value;
    }

    private readonly Dictionary<string, string> _fileHashes = [];
    Dictionary<string, string> IFileHashTestable.FileHashes => _fileHashes;

    /// <summary>(Immutable) Name of the language.</summary>
    private const string LanguageName = "CSharpFirely2";

    /// <summary>Gets the language name.</summary>
    public string Name => LanguageName;

    public Type ConfigType => typeof(FirelyGenOptions);

    /// <summary>Gets a value indicating whether this language is idempotent.</summary>
    public bool IsIdempotent => false;

    /// <summary>The namespace to use during export.</summary>
    private const string Namespace = "Hl7.Fhir.Model";

    /// <summary>FHIR information we are exporting.</summary>
    private DefinitionCollection _info = null!;

    /// <summary>Options for controlling the export.</summary>
    private FirelyGenOptions _options = null!;

    /// <summary>Keep track of information about written value sets.</summary>
    private Dictionary<string, WrittenValueSetInfo> _writtenValueSets = [];

    /// <summary>The split characters.</summary>
    private static readonly char[] _splitChars = ['|', ' '];

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer = null!;

    /// <summary>The model writer.</summary>
    private ExportStreamWriter _modelWriter = null!;

    /// <summary>Pathname of the export directory.</summary>
    private string _exportDirectory = string.Empty;

    /// <summary>Structures to skip generating.</summary>
    internal static readonly HashSet<string> ExclusionSet =
    [
        /* These two types are generated as interfaces, so require special handling and
         * are not to be treated as normal resources. */
        "CanonicalResource",
        "MetadataResource",

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

    private static readonly Dictionary<(string,string), VersionIndependentResourceTypesAll> _searchParamDefsTargetRemovals = new()
    {
        [("DiagnosticReport", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("RiskAssessment", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("List", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("VisionPrescription", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("ServiceRequest", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("Flag", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("Observation", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("NutritionOrder", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("Composition", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("DeviceRequest", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
        [("Procedure", "encounter")] = VersionIndependentResourceTypesAll.EpisodeOfCare,
    };

    /// <summary>
    /// List of types introduced in R5 that are retrospectively introduced in R3 and R4.
    /// </summary>
    internal static readonly List<WrittenModelInfo> SharedR5DataTypes =
    [
        new("BackboneType", "BackboneType", true),
        new("Base", "Base",true),
        new("DataType", "DataType", true),
        new("PrimitiveType", "PrimitiveType", true),
    ];

    /// <summary>
    /// List of complex datatype classes that are part of the 'base' subset. See <see cref="GenSubset"/>.
    /// </summary>
    private static readonly List<string> _baseSubsetComplexTypes =
    [
        "Address",
        "Attachment",
        "BackboneElement",
        "BackboneType",
        "Base",
        "CodeableConcept",
        "Coding",
        "ContactPoint",
        "ContactDetail",
        "DataType",
        "Duration",
        "Element",
        "Extension",
        "HumanName",
        "Identifier",
        "Meta",
        "Narrative",
        "Period",
        "PrimitiveType",
        "Quantity",
        "Range",
        "Ratio",
        "Reference",
        "Signature",
        "UsageContext",
        "CodeableReference"
    ];

    /// <summary>
    /// List of complex datatype classes that are part of the 'conformance' subset. See <see cref="GenSubset"/>.
    /// </summary>
    private static readonly List<string> _conformanceSubsetComplexTypes =
    [
        "ElementDefinition",
        "RelatedArtifact",
    ];

    /// <summary>
    /// List of resource classes that are part of the 'base' subset. See <see cref="GenSubset"/>.
    /// </summary>
    private static readonly List<string> _baseSubsetResourceTypes =
    [
        "Binary",
        "Bundle",
        "DomainResource",
        "OperationOutcome",
        "Parameters",
        "Resource",
    ];


    /// <summary>
    /// List of resource classes that are part of the 'conformance' subset. See <see cref="GenSubset"/>.
    /// </summary>
    private static readonly List<string> _conformanceSubsetResourceTypes =
    [
        "CapabilityStatement",
        "CodeSystem",
        "ElementDefinition",
        "StructureDefinition",
        "ValueSet",
    ];

    /// <summary>
    /// List of all valuesets that we publish in the base subset
    /// </summary>
    private static readonly HashSet<string> _baseSubsetValueSets =
    [
        "http://hl7.org/fhir/ValueSet/publication-status",
        "http://hl7.org/fhir/ValueSet/FHIR-version",
        "http://hl7.org/fhir/ValueSet/search-param-type",
        "http://hl7.org/fhir/ValueSet/filter-operator",
        "http://hl7.org/fhir/ValueSet/version-independent-all-resource-types"
    ];

    /// <summary>
    /// List of all valuesets that we publish in the conformance subset.
    /// </summary>
    private static readonly HashSet<string> _conformanceSubsetValueSets =
    [
        "http://hl7.org/fhir/ValueSet/capability-statement-kind",
        "http://hl7.org/fhir/ValueSet/binding-strength",

        // These are necessary for CapabilityStatement/CapabilityStatement2
        // CapStat2 has disappeared in ballot, if that becomes final,
        // these don't have to be created as shared valuesets anymore.
        "http://hl7.org/fhir/ValueSet/restful-capability-mode",
        "http://hl7.org/fhir/ValueSet/type-restful-interaction",
        "http://hl7.org/fhir/ValueSet/system-restful-interaction",

        // For these valuesets the algorithm to determine whether a vs is shared
        // is still considering core extensions too. When this is corrected,
        // these can probably go.
        "http://hl7.org/fhir/ValueSet/constraint-severity",

        "http://hl7.org/fhir/ValueSet/codesystem-content-mode"
    ];

    /// <summary>
    ///  List of all valuesets that we should publish as a shared Enum although there is only 1 reference to it.
    /// </summary>
    internal static readonly List<(string, string)> ExplicitSharedValueSets =
    [
        // This enum should go to Template-binding.cs because otherwise it will introduce a breaking change.
        ("R4", "http://hl7.org/fhir/ValueSet/messageheader-response-request"),
        ("R4", "http://hl7.org/fhir/ValueSet/concept-map-equivalence"),
        ("R4B", "http://hl7.org/fhir/ValueSet/messageheader-response-request"),
        ("R4B", "http://hl7.org/fhir/ValueSet/concept-map-equivalence"),
        ("R5", "http://hl7.org/fhir/ValueSet/constraint-severity"),
    ];

    /// <summary>Gets the reserved words.</summary>
    /// <value>The reserved words.</value>
    private static readonly HashSet<string> _reservedWords = [];

    private static readonly Func<WrittenModelInfo, bool> _supportedResourcesFilter = wmi => !wmi.IsAbstract;
    private static readonly Func<WrittenModelInfo, bool> _fhirToCsFilter = wmi => !_excludeFromCsToFhir!.Contains(wmi.FhirName);
    private static readonly Func<WrittenModelInfo, bool> _csToStringFilter = _fhirToCsFilter;

    private static readonly string[] _excludeFromCsToFhir =
    [
        "CanonicalResource",
        "MetadataResource",
    ];

    /// <summary>
    /// The list of elements that would normally be represented using a CodeOfT enum, but that we
    /// want to be generated as a normal Code instead.
    /// </summary>
    private static readonly List<string> _codedElementOverrides =
    [
        "CapabilityStatement.rest.resource.type"
    ];

    /// <summary>
    /// Some valuesets have names that are the same as element names or are just not nice - use this collection
    /// to change the name of the generated enum as required.
    /// </summary>
    internal static readonly Dictionary<string, string> EnumNamesOverride = new()
    {
        ["http://hl7.org/fhir/ValueSet/characteristic-combination"] = "CharacteristicCombinationCode",
        ["http://hl7.org/fhir/ValueSet/claim-use"] = "ClaimUseCode",
        ["http://hl7.org/fhir/ValueSet/content-type"] = "ContentTypeCode",
        ["http://hl7.org/fhir/ValueSet/exposure-state"] = "ExposureStateCode",
        ["http://hl7.org/fhir/ValueSet/verificationresult-status"] = "StatusCode",
        ["http://terminology.hl7.org/ValueSet/v3-Confidentiality"] = "ConfidentialityCode",
        ["http://hl7.org/fhir/ValueSet/variable-type"] = "VariableTypeCode",
        ["http://hl7.org/fhir/ValueSet/group-measure"] = "GroupMeasureCode",
        ["http://hl7.org/fhir/ValueSet/coverage-kind"] = "CoverageKindCode",
        ["http://hl7.org/fhir/ValueSet/fhir-types"] = "FHIRAllTypes"
    };

    private record ElementTypeChange(FhirReleases.FhirSequenceCodes Since,
        TypeReference DeclaredTypeReference);

    private static readonly ElementTypeChange[] _stringToMarkdown =
    [
        new(FhirReleases.FhirSequenceCodes.STU3, PrimitiveTypeReference.String),
        new(FhirReleases.FhirSequenceCodes.R5, PrimitiveTypeReference.Markdown)
    ];

    /// <summary>
    /// Given one of the versions, returns a string that describes from which version until which
    /// version that change was in effect.
    /// </summary>
    private static string VersionChangeMessage(ElementTypeChange[] changeSet, ElementTypeChange thisChange, bool capitalize)
    {
        int index = Array.IndexOf(changeSet, thisChange);
        if (index == -1) throw new ArgumentException("Change needs to be part of the set", nameof(thisChange));

        if (index + 1 >= changeSet.Length)
        {
            // This is the last change in the set
            return $"{(capitalize ? "S" : "s")}tarting from {thisChange.Since}";
        }

        int now = (int)thisChange.Since;
        int next = (int)changeSet[index + 1].Since;
        IEnumerable<FhirReleases.FhirSequenceCodes> versionOrdinals =
            Enumerable.Range(now, next - now).Cast<FhirReleases.FhirSequenceCodes>();
        string versions = string.Join(", ", versionOrdinals.Select(v => v.ToString()));
        versions = replaceLastOccurrence(versions, ", ", " and ");
        return $"{(capitalize ? "I" : "i")}n {versions}";

        static string replaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            return source.Remove(place, find.Length).Insert(place, replace);
        }
    }

    // ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
    private static readonly Dictionary<string, ElementTypeChange[]> _elementTypeChanges = new()
    {
        ["Attachment.size"] = [
            new(FhirReleases.FhirSequenceCodes.STU3, PrimitiveTypeReference.UnsignedInt),
            new (FhirReleases.FhirSequenceCodes.R5, PrimitiveTypeReference.Integer64),
        ],
        ["Attachment.url"] = [
            new(FhirReleases.FhirSequenceCodes.STU3, PrimitiveTypeReference.Uri),
            new(FhirReleases.FhirSequenceCodes.R4, PrimitiveTypeReference.Url),
        ],
        ["Meta.profile"] = [
            new(FhirReleases.FhirSequenceCodes.STU3, PrimitiveTypeReference.Uri),
            new(FhirReleases.FhirSequenceCodes.R4, PrimitiveTypeReference.Canonical),
        ],
        ["Bundle.link.relation"] = [
            new(FhirReleases.FhirSequenceCodes.STU3, PrimitiveTypeReference.String),
            new(FhirReleases.FhirSequenceCodes.R5, PrimitiveTypeReference.Code)
        ],

        ["ElementDefinition.constraint.requirements"] = _stringToMarkdown,
        ["ElementDefinition.binding.description"] = _stringToMarkdown,
        ["ElementDefinition.mapping.comment"] = _stringToMarkdown,
        ["CapabilityStatement.implementation.description"] = _stringToMarkdown,
     };
    // ReSharper restore ArrangeObjectCreationWhenTypeNotEvident

    private readonly Dictionary<string, string> _sinceAttributes = new()
    {
        ["Meta.source"] = "R4",
        ["Reference.type"] = "R4",
        ["Bundle.timestamp"] = "R4",
        ["Binary.data"] = "R4",
        ["ValueSet.compose.property"] = "R5",
        ["ValueSet.compose.include.copyright"] = "R5",
        ["ValueSet.expansion.property"] = "R5",
        ["ValueSet.expansion.contains.property"] = "R5",
        ["ValueSet.scope"] = "R5",
        ["Bundle.issues"] = "R5",
        ["CapabilityStatement.rest.resource.conditionalPatch"] = "R5",
        ["CapabilityStatement.versionAlgorithm"] = "R5",
        ["CapabilityStatement.copyrightLabel"] = "R5",
        ["CapabilityStatement.acceptLanguage"] = "R5",
        ["CapabilityStatement.identifier"] = "R5",
        ["CodeSystem.concept.designation.additionalUse"] = "R5",
        ["CodeSystem.approvalDate"] = "R5",
        ["CodeSystem.lastReviewDate"] = "R5",
        ["CodeSystem.effectivePeriod"] = "R5",
        ["CodeSystem.topic"] = "R5",
        ["CodeSystem.author"] = "R5",
        ["CodeSystem.editor"] = "R5",
        ["CodeSystem.reviewer"] = "R5",
        ["CodeSystem.endorser"] = "R5",
        ["CodeSystem.relatedArtifact"] = "R5",
        ["CodeSystem.copyrightLabel"] = "R5",
        ["CodeSystem.versionAlgorithm"] = "R5",
        ["ElementDefinition.constraint.suppress"] = "R5",
        ["ElementDefinition.mustHaveValue"] = "R5",
        ["ElementDefinition.valueAlternatives"] = "R5",
        ["ElementDefinition.obligation"] = "R5",
        ["ElementDefinition.obligation.code"] = "R5",
        ["ElementDefinition.obligation.actor"] = "R5",
        ["ElementDefinition.obligation.documentation"] = "R5",
        ["ElementDefinition.obligation.usage"] = "R5",
        ["ElementDefinition.obligation.filter"] = "R5",
        ["ElementDefinition.obligation.filterDocumentation"] = "R5",
        ["ElementDefinition.obligation.process"] = "R5",
        ["ElementDefinition.binding.additional"] = "R5",
        ["ElementDefinition.binding.additional.purpose"] = "R5",
        ["ElementDefinition.binding.additional.valueSet"] = "R5",
        ["ElementDefinition.binding.additional.documentation"] = "R5",
        ["ElementDefinition.binding.additional.shortDoco"] = "R5",
        ["ElementDefinition.binding.additional.usage"] = "R5",
        ["ElementDefinition.binding.additional.any"] = "R5",
        ["StructureDefinition.versionAlgorithm"] = "R5",
        ["StructureDefinition.copyrightLabel"] = "R5",
        ["ValueSet.compose.include.concept.designation.additionalUse"] = "R5",
        ["ValueSet.expansion.next"] = "R5",
        ["ValueSet.expansion.contains.property.subProperty"] = "R5",
        ["ValueSet.expansion.contains.property.subProperty.code"] = "R5",
        ["ValueSet.expansion.contains.property.subProperty.value"] = "R5",
        ["ValueSet.approvalDate"] = "R5",
        ["ValueSet.lastReviewDate"] = "R5",
        ["ValueSet.effectivePeriod"] = "R5",
        ["ValueSet.topic"] = "R5",
        ["ValueSet.author"] = "R5",
        ["ValueSet.editor"] = "R5",
        ["ValueSet.reviewer"] = "R5",
        ["ValueSet.endorser"] = "R5",
        ["ValueSet.relatedArtifact"] = "R5",
        ["ValueSet.copyrightLabel"] = "R5",
        ["ValueSet.versionAlgorithm"] = "R5",
        ["Attachment.height"] = "R5",
        ["Attachment.width"] = "R5",
        ["Attachment.frames"] = "R5",
        ["Attachment.duration"] = "R5",
        ["Attachment.pages"] = "R5",
        ["RelatedArtifact.classifier"] = "R5",
        ["RelatedArtifact.resourceReference"] = "R5",
        ["RelatedArtifact.publicationStatus"] = "R5",
        ["RelatedArtifact.publicationDate"] = "R5",
        ["Signature.data"] = "R4",
        ["Signature.who"] = "R4",
        ["Signature.onBehalfOf"] = "R4",
        ["Signature.sigFormat"] = "R4",
        ["Signature.targetFormat"] = "R4"
    };

    private readonly Dictionary<string, (string since, string newName)> _untilAttributes = new()
    {
        ["Binary.content"] = ("R4", "Binary.data"),
        ["ElementDefinition.constraint.xpath"] = ("R5", ""),
        ["ValueSet.scope.focus"] = ("R5", ""),
        ["RelatedArtifact.url"] = ("R5", ""),
        ["Signature.blob"] = ("R4", "Signature.data"),
        ["Signature.contentType"] = ("R4", "")
    };

    private static string? GetExplicitName(ElementDefinition ed, FhirReleases.FhirSequenceCodes sequence) =>
        (ed.Path, sequence) switch
        {
            ("Evidence.statistic.attributeEstimate.attributeEstimate", _) => "AttributeEstimate",
            ("Citation.citedArtifact.contributorship.summary", _) => "CitedArtifactContributorshipSummary",
            ("Measure.group", FhirReleases.FhirSequenceCodes.R6) => "GroupBackboneComponent",
            ("Measure.group.component", FhirReleases.FhirSequenceCodes.R6) => "GroupComponent",
            ("Measure.group.stratifier.component", FhirReleases.FhirSequenceCodes.R6) => "GroupStratifierComponent",
            ("MolecularDefinition.location.sequenceLocation.coordinateInterval", FhirReleases.FhirSequenceCodes.R6) =>
                "LocationSequenceLocationCoordinateIntervalComponent",
            ("MolecularDefinition.location.sequenceLocation.coordinateInterval.coordinateSystem", FhirReleases.FhirSequenceCodes.R6) =>
                "LocationSequenceLocationCoordinateIntervalCoordinateSystemComponent",
            ("MolecularDefinition.representation.extracted.coordinateInterval", FhirReleases.FhirSequenceCodes.R6) =>
                "RepresentationExtractedCoordinateIntervalComponent",
            ("MolecularDefinition.representation.extracted.coordinateInterval.coordinateSystem", FhirReleases.FhirSequenceCodes.R6) =>
                "RepresentationExtractedCoordinateIntervalCoordinateSystemComponent",
            ("MolecularDefinition.representation.relative.edit.coordinateInterval", FhirReleases.FhirSequenceCodes.R6) =>
                "RepresentationRelativeEditCoordinateIntervalComponent",
            ("MolecularDefinition.representation.relative.edit.coordinateInterval.coordinateSystem", FhirReleases.FhirSequenceCodes.R6) =>
                "RepresentationRelativeEditCoordinateIntervalCoordinateSystemComponent",

            ("TestPlan.scope", FhirReleases.FhirSequenceCodes.R6) => "ScopeComponent",
            ("TestPlan.testCase.scope", FhirReleases.FhirSequenceCodes.R6) => "TestCaseScopeComponent",
            _ => ed.cgExplicitName()
        };

    /// <summary>True to export five ws.</summary>
    private bool _exportFiveWs = true;

    /// <summary>Gets the FHIR primitive type map.</summary>
    /// <value>The FHIR primitive type map.</value>
    Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => PrimitiveTypeMap;

    /// <summary>If a Cql ModelInfo is available, this will be the parsed XML model file.</summary>
    private Ncqa.Cql.Model.ModelInfo? _cqlModelInfo = null;
    private IDictionary<string, Ncqa.Cql.Model.ClassInfo>? _cqlModelClassInfo = null;

    /// <summary>Export the passed FHIR version into the specified directory.</summary>
    /// <param name="untypedOptions"></param>
    /// <param name="info">           The information.</param>
    public void Export(object untypedOptions, DefinitionCollection info)
    {
        if (untypedOptions is not FirelyGenOptions options)
        {
            throw new ArgumentException("Options must be of type FirelyGenOptions");
        }

        var subset = options.Subset;

        // STU3 satellite is a combination of satellite and conformance
        if ((info.FhirSequence == FhirReleases.FhirSequenceCodes.STU3) &&
            (subset == GenSubset.Satellite))
        {
            subset = GenSubset.Satellite | GenSubset.Conformance;
        }

        // By definition, we should not have any element type changes for sattelites, they
        // should only have their own, defined types from the spec.
        if (subset.HasFlag(GenSubset.Satellite))
            _elementTypeChanges.Clear();

        // only generate base definitions for R5
        if (subset.HasFlag(GenSubset.Base) &&
            info.FhirSequence is not (FhirReleases.FhirSequenceCodes.R6 or FhirReleases.FhirSequenceCodes.R5))
        {
            Console.WriteLine($"Aborting {LanguageName} for {info.FhirSequence}: code generation for the 'base' subset should be run on R5/R6 only.");
            return;
        }

        // conformance subset is only valid for STU3 and R5
        if (subset.HasFlag(GenSubset.Conformance) &&
            info.FhirSequence is not (FhirReleases.FhirSequenceCodes.STU3 or
                FhirReleases.FhirSequenceCodes.R5 or FhirReleases.FhirSequenceCodes.R6))

        {
            Console.WriteLine($"Aborting {LanguageName} for {info.FhirSequence}: code generation for the 'conformance' subset should be run on STU3 or R5/R6 only.");
            return;
        }

        _exportFiveWs = options.ExportFiveWs;

        // set internal vars so we don't pass them to every function
        _info = info;
        _options = options;
        _exportDirectory = options.OutputDirectory;
        _writtenValueSets = [];

        if (!Directory.Exists(_exportDirectory))
        {
            Directory.CreateDirectory(_exportDirectory);
        }

        if (!Directory.Exists(Path.Combine(_exportDirectory, "Generated")))
        {
            Directory.CreateDirectory(Path.Combine(_exportDirectory, "Generated"));
        }

        string cqlModelResourceKey = options.CqlModel;
        if (!string.IsNullOrEmpty(cqlModelResourceKey))
        {
            _cqlModelInfo = CqlModels.LoadEmbeddedResource(cqlModelResourceKey);
            _cqlModelClassInfo = CqlModels.ClassesByName(_cqlModelInfo);
        }

        var allPrimitives = new Dictionary<string, WrittenModelInfo>();
        var allComplexTypes = new Dictionary<string, WrittenModelInfo>();
        var allResources = new Dictionary<string, WrittenModelInfo>();
        var dummy = new Dictionary<string, WrittenModelInfo>();

        string infoFilename = Path.Combine(_exportDirectory, "Generated", "_GeneratorLog.cs");

        // update the models for consistency across different versions of FHIR
        ModifyDefinitionsForConsistency();

        using Stream infoStream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(infoFilename, FileMode.Create);
        using ExportStreamWriter infoWriter = new(infoStream);

        if (_generateHashesInsteadOfOutput)
        {
            infoWriter.NewLine = "\r\n";
        }

        _modelWriter = infoWriter;

        WriteGenerationComment(infoWriter);

        if (options.ExportStructures.Contains(FhirArtifactClassEnum.ValueSet))
        {
            WriteSharedValueSets(subset);
        }

        _modelWriter.WriteLineIndented("// Generated items");

        if (options.ExportStructures.Contains(FhirArtifactClassEnum.PrimitiveType))
        {
            WritePrimitiveTypes(_info.PrimitiveTypesByName.Values, ref dummy, subset);
        }

        AddModels(allPrimitives, _info.PrimitiveTypesByName.Values);

        if (options.ExportStructures.Contains(FhirArtifactClassEnum.ComplexType))
        {
            WriteComplexDataTypes(_info.ComplexTypesByName.Values, ref dummy, subset);
        }

        AddModels(allComplexTypes, _info.ComplexTypesByName.Values);
        AddModels(allComplexTypes, SharedR5DataTypes);

        if (options.ExportStructures.Contains(FhirArtifactClassEnum.Resource))
        {
            WriteResources(_info.ResourcesByName.Values, ref dummy, subset);
        }

        AddModels(allResources, _info.ResourcesByName.Values);

        if (options.ExportStructures.Contains(FhirArtifactClassEnum.Interface))
        {
            WriteInterfaces(_info.InterfacesByName.Values, ref dummy, subset);
        }

        if (subset.HasFlag(GenSubset.Satellite))
        {
            WriteModelInfo(allPrimitives, allComplexTypes, allResources);
        }

        if (_generateHashesInsteadOfOutput)
        {
            infoWriter.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(infoStream);
            _fileHashes.Add(infoFilename[_exportDirectory.Length..], hash);

            infoWriter.Close();
        }
        else
        {
            infoWriter.Flush();
            infoWriter.Close();
        }
    }

    /// <summary>
    /// Modifies the definition structures for consistency.  Note that this makes the export *not* idempotent.
    /// </summary>
    private void ModifyDefinitionsForConsistency()
    {
        // We need to modify the (R4+-based) definition of Binary, to include
        // the pre-R4 element "content".
        if (_info.ResourcesByName.TryGetValue("Binary", out StructureDefinition? sdBinary))
        {
            if (!sdBinary.cgTryGetElementByPath("Binary.content", out _) &&
                sdBinary.cgTryGetElementByPath("Binary.data", out ElementDefinition? edData))
            {
                // make a copy of the data element
                ElementDefinition edContent = (ElementDefinition)edData.DeepCopy();

                // update the copied element to be the content element
                edContent.ElementId = "Binary.content";
                edContent.Path = "Binary.content";
                edContent.Base = new ElementDefinition.BaseComponent { Path = "Binary.content", Min = 0, Max = "1" };

                edContent.cgSetFieldOrder(edData.cgFieldOrder(), edData.cgComponentFieldOrder());

                // add our element and track info, note that we are not increasing
                // the orders since they are duplicate elements from different versions
                _ = _info.TryInsertElement(sdBinary, edContent, false);
            }
        }

        // We need to modify the definition of Signature, to include
        // the STU3 content.
        if (_info.ComplexTypesByName.TryGetValue("Signature", out StructureDefinition? sdSignature))
        {
            if (!sdSignature.cgTryGetElementByPath("Signature.blob", out _) &&
                sdSignature.cgTryGetElementByPath("Signature.data", out ElementDefinition? edData))
            {
                // make a copy of the data element
                ElementDefinition edBlob = (ElementDefinition)edData.DeepCopy();

                // update the copied element to be the blob element
                edBlob.ElementId = "Signature.blob";
                edBlob.Path = "Signature.blob";
                edBlob.Base = new() { Path = "Signature.blob", Min = 0, Max = "1" };
                edBlob.Min = 0;
                edBlob.Max = "1";

                edBlob.cgSetFieldOrder(edData.cgFieldOrder(), edData.cgComponentFieldOrder());

                //edBlob.RemoveExtension(CommonDefinitions.ExtUrlEdFieldOrder);
                //edBlob.RemoveExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder);

                //edBlob.AddExtension(CommonDefinitions.ExtUrlEdFieldOrder, new Integer(edData.cgFieldOrder() + 1));
                //edBlob.AddExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder, new Integer(edData.cgComponentFieldOrder() + 1));

                // add our element and track info
                _ = _info.TryInsertElement(sdSignature, edBlob, false);
            }

            if (!sdSignature.cgTryGetElementByPath("Signature.contentType", out ElementDefinition? edContentType))
            {
                // create a new element for the contentType (values pulled from STU3)
                edContentType = new()
                {
                    ElementId = "Signature.contentType",
                    Path = "Signature.contentType",
                    Short = "The technical format of the signature",
                    Definition = "A mime type that indicates the technical format of the signature. Important mime types are application/signature+xml for X ML DigSig, application/jwt for JWT, and image/* for a graphical image of a signature, etc.",
                    Min = 0,
                    Max = "1",
                    Base = new() { Path = "Signature.contentType", Min = 0, Max = "1" },
                    Type = [new() { Code = "code" }],
                    IsSummary = true,
                    Binding = new()
                    {
                        Strength = Hl7.Fhir.Model.BindingStrength.Required,
                        ValueSet = new Canonical("http://www.rfc-editor.org/bcp/bcp13.txt"),
                        Description = "The mime type of an attachment. Any valid mime type is allowed.",
                        Extension =
                        [
                            new()
                            {
                                Url = CommonDefinitions.ExtUrlBindingName,
                                Value = new FhirString("MimeType"),
                            },
                            new()
                            {
                                Url = CommonDefinitions.ExtUrlIsCommonBinding,
                                Value = new FhirBoolean(true),
                            }
                        ]
                    }
                };

                edContentType.cgSetFieldOrder(7, 6);

                // add our element and track info
                _ = _info.TryInsertElement(sdSignature, edContentType, false);
            }
            else
            {
                // move the current element to after onBehalfOf
                edContentType.cgSetFieldOrder(7, 6);
            }

            if (sdSignature.cgTryGetElementById("Signature.who", out ElementDefinition? edWho))
            {
                // make it a choice type by adding uri, like it was in STU3
                edWho.ElementId = "Signature.who[x]";
                edWho.Path = "Signature.who[x]";
                edWho.Base.Path = "Signature.who[x]";
                edWho.Type.Add(new() { Code = "uri" });

                //_ = _info.TryUpdateElement(sdSignature, edWho);
            }

            if (sdSignature.cgTryGetElementById("Signature.onBehalfOf", out ElementDefinition? edOnBehalfOf))
            {
                // make it a choice type by adding uri, like it was in STU3
                edOnBehalfOf.ElementId = "Signature.onBehalfOf[x]";
                edOnBehalfOf.Path = "Signature.onBehalfOf[x]";
                edOnBehalfOf.Base.Path = "Signature.onBehalfOf[x]";
                edOnBehalfOf.Type.Add(new() { Code = "uri" });

                // TODO: fix the order (should be 6th total, 5th in component)
                edOnBehalfOf.cgSetFieldOrder(6, 5);
            }
        }

        // Element ValueSet.scope.focus has been removed in R5 (5.0.0-snapshot3). Adding this element to the list of Resources,
        // so we can add a [NotMapped] attribute later.
        if (_info.ResourcesByName.TryGetValue("ValueSet", out StructureDefinition? sdValueSet) &&
            sdValueSet.cgTryGetElementById("ValueSet.scope", out _) &&
            !sdValueSet.cgTryGetElementById("ValueSet.scope.focus", out _))
        {
            // create a new element for the focus (values pulled from 5.0.0-snapshot1)
            ElementDefinition edFocus = new()
            {
                ElementId = "ValueSet.scope.focus",
                Path = "ValueSet.scope.focus",
                Short = "General focus of the Value Set as it relates to the intended semantic space",
                Definition = "The general focus of the Value Set as it relates to the intended semantic space. This can be the information about clinical relevancy or the statement about the general focus of the Value Set, such as a description of types of messages, payment options, geographic locations, etc.",
                Min = 0,
                Max = "1",
                Base = new() { Path = "ValueSet.scope.focus", Min = 0, Max = "1" },
                Type = [new() { Code = "string" }],
                Constraint =
                [
                    new()
                    {
                        Key = "ele-1",
                        Severity = ConstraintSeverity.Error,
                        Human = "All FHIR elements must have a @value or children",
                        Expression = "hasValue() or (children().count() > id.count())",
                    },
                ],
                IsSummary = false,
                IsModifier = false,
                MustSupport = false,
            };

            edFocus.cgSetFieldOrder(123, 3);

            // TODO(ginoc): This insertion is currently pushing exclusionCriteria to Order=60 in file (componentOrder 5) - it should not
            // add our element and track info
            _ = _info.TryInsertElement(sdValueSet, edFocus, true);
        }

        // Element Bundle.link.relation changed from FhirString to Code<Hl7.Fhir.Model.Bundle.LinkRelationTypes> in R5 (5.0.0-snapshot3).
        // We decided to leave the type to FhirString
        if (_info.ResourcesByName.TryGetValue("Bundle", out StructureDefinition? sdBundle) &&
            sdBundle.cgTryGetElementById("Bundle.link.relation", out ElementDefinition? edRelation))
        {
            edRelation.Type = [new() { Code = "string" }];

            _ = _info.TryUpdateElement(sdBundle, edRelation);
        }

        // Element ElementDefinition.constraint.xpath has been removed in R5 (5.0.0-snapshot3). Adding this element to the list of ComplexTypes,
        // so we can add a [NotMapped] attribute later.
        if (_info.ComplexTypesByName.TryGetValue("ElementDefinition", out StructureDefinition? sdElementDefinition) &&
            sdElementDefinition.cgTryGetElementById("ElementDefinition.constraint", out _) &&
            !sdElementDefinition.cgTryGetElementById("ElementDefinition.constraint.xpath", out _))
        {
            // create a new element for the xpath (values pulled from 5.0.0-snapshot1)
            ElementDefinition edXPath = new()
            {
                ElementId = "ElementDefinition.constraint.xpath",
                Path = "ElementDefinition.constraint.xpath",
                Short = "XPath expression of constraint",
                Definition = "An XPath expression of constraint that can be executed to see if this constraint is met.",
                Min = 0,
                Max = "1",
                Base = new() { Path = "ElementDefinition.constraint.xpath", Min = 0, Max = "1" },
                Type = [new() { Code = "string" }],
                IsSummary = true,
            };

            // try to get the offsets from ElementDefinition.constraint.expression (we want to be after that element)
            if (sdElementDefinition.cgTryGetElementById("ElementDefinition.constraint.expression", out ElementDefinition? edConstraintExpression))
            {
                edXPath.cgSetFieldOrder(edConstraintExpression.cgFieldOrder() + 1, edConstraintExpression.cgComponentFieldOrder() + 1);
            }
            else
            {
                edXPath.cgSetFieldOrder(66, 8);
            }

            // add our element and track info
            _ = _info.TryInsertElement(sdElementDefinition, edXPath, true);
        }

        // We need to modify the (R4+-based) definition of RelatedArtifact, to include
        // the pre-R4 element "url".
        if (_info.ComplexTypesByName.TryGetValue("RelatedArtifact", out StructureDefinition? sdRelatedArtifact) &&
            !sdRelatedArtifact.cgTryGetElementById("RelatedArtifact.url", out _))
        {
            // create a new element for the url (values pulled from STU3)
            ElementDefinition edUrl = new()
            {
                ElementId = "RelatedArtifact.url",
                Path = "RelatedArtifact.url",
                Short = "Where the artifact can be accessed",
                Definition = "A url for the artifact that can be followed to access the actual content.",
                Comment = "If a document or resource element is present, this element SHALL NOT be provided (use the url or reference in the Attachment or resource reference).",
                Min = 0,
                Max = "1",
                Base = new() { Path = "RelatedArtifact.url", Min = 0, Max = "1" },
                Type = [new() { Code = "url" }],
                IsSummary = true,
            };

            edUrl.cgSetFieldOrder(8, 7);

            // add our element and track info
            _ = _info.TryInsertElement(sdRelatedArtifact, edUrl, true);
        }

        // correct issues in FHIR 6.0.0-ballot2
        if ((_info.MainPackageId == "hl7.fhir.r6.core") && (_info.MainPackageVersion == "6.0.0-ballot2"))
        {
            if (_info.ResourcesByName.TryGetValue("TestScript", out StructureDefinition? sdTestScript))
            {
                if (sdTestScript.cgTryGetElementById("TestScript.setup.action.common.parameter", out ElementDefinition? edSetupCommonParameter))
                {
                    // Set the class name to use for this new backbone element
                    edSetupCommonParameter.AddExtension("http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name", new FhirString("SetupActionCommonParameter"));
                }
                if (sdTestScript.cgTryGetElementById("TestScript.common.parameter", out ElementDefinition? edCommonParameter))
                {
                    // Set the class name to use for this new backbone element
                    edCommonParameter.AddExtension("http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name", new FhirString("CommonParameter"));
                }
                if (sdTestScript.cgTryGetElementById("TestScript.setup.action.common", out ElementDefinition? edSetupActionCommon))
                {
                    // Set the class name to use for this new backbone element
                    edSetupActionCommon.AddExtension("http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name", new FhirString("SetupActionCommon"));
                }
            }
        }
    }

    /// <summary>Writes a model information.</summary>
    /// <param name="writtenPrimitives">   The written primitives.</param>
    /// <param name="writtenComplexTypes">List of types of the written complexes.</param>
    /// <param name="writtenResources">   The written resources.</param>
    private void WriteModelInfo(
        Dictionary<string, WrittenModelInfo> writtenPrimitives,
        Dictionary<string, WrittenModelInfo> writtenComplexTypes,
        Dictionary<string, WrittenModelInfo> writtenResources)
    {
        string filename = Path.Combine(_exportDirectory, "Generated", "Template-ModelInfo.cs");

        using Stream stream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(filename, FileMode.Create);
        using ExportStreamWriter writer = new(stream);

        if (_generateHashesInsteadOfOutput)
        {
            writer.NewLine = "\r\n";
        }

        _writer = writer;

        WriteGenerationComment();

        _writer.WriteLineIndented("using System;");
        _writer.WriteLineIndented("using System.Collections;");
        _writer.WriteLineIndented("using System.Collections.Generic;");
        _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
        _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
        _writer.WriteLineIndented("using System.Linq;");
        _writer.WriteLineIndented("using System.Runtime.Serialization;");
        _writer.WriteLine(string.Empty);

        WriteCopyright();

        WriteNamespaceOpen();

        WriteIndentedComment(
            "A class with methods to retrieve information about the\n" +
            "FHIR definitions based on which this assembly was generated.");

        _writer.WriteLineIndented("public partial class ModelInfo");

        // open class
        OpenScope();

        WriteSupportedResources(writtenResources.Values.Where(_supportedResourcesFilter));

        WriteFhirVersion();

        WriteFhirToCs(writtenPrimitives.Values.Where(_fhirToCsFilter), writtenComplexTypes.Values.Where(_fhirToCsFilter), writtenResources.Values.Where(_fhirToCsFilter));
        WriteCsToString(writtenPrimitives.Values.Where(_csToStringFilter), writtenComplexTypes.Values.Where(_csToStringFilter), writtenResources.Values.Where(_csToStringFilter));

        WriteSearchParameters();
        var dataTypes = writtenPrimitives.Concat(writtenComplexTypes).ToDictionary(we => we.Key, we => we.Value);
        WriteOpenTypes(dataTypes);

        // close class
        CloseScope();

        WriteNamespaceClose();

        if (_generateHashesInsteadOfOutput)
        {
            writer.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(stream);
            _fileHashes.Add(filename[_exportDirectory.Length..], hash);

            writer.Close();
        }
        else
        {
            writer.Flush();
            writer.Close();
        }
    }


    private void WriteOpenTypes(Dictionary<string,WrittenModelInfo> types)
    {
        _writer.WriteIndentedComment("The open types that are used in the FHIR model. These are types that can be used in the 'value' field of an Extension.", isSummary: true);
        _writer.WriteIndentedComment("This list differs from the one in the written documentation (https://www.hl7.org/fhir/datatypes.html),\n" +
            "but we assume the list in Extension.value[x] is the more authorative.",
                                     isRemarks: true, isSummary: false);
        _writer.WriteLineIndented("public static readonly Type[] OpenTypes =");
        OpenScope();

        var extensionValue = _info.TryFindElementByPath("Extension.value[x]",
            out StructureDefinition? _,
            out ElementDefinition? edExtensionValue)
            ? edExtensionValue.Type
            : throw new InvalidOperationException("Could not find Extension.value[x] element in definitions");

        foreach (var typeName in extensionValue.Select(ev => ev.Code).OrderBy(c => c))
        {
            _writer.WriteLineIndented($"typeof({types[typeName].CsName}),");
        }

        CloseScope(includeSemicolon: true);
    }

    /// <summary>Writes the search parameters.</summary>
    private void WriteSearchParameters()
    {
        _writer.WriteLineIndented("public static List<SearchParamDefinition> SearchParameters = new List<SearchParamDefinition>()");
        OpenScope();

        foreach (StructureDefinition complex in _info.ResourcesByName.Values.OrderBy(c => c.Name))
        {
            IReadOnlyDictionary<string, SearchParameter> resourceSearchParams = _info.SearchParametersForBase(complex.Name);
            if (!resourceSearchParams.Any())
            {
                continue;
            }

            foreach (SearchParameter sp in resourceSearchParams.Values.OrderBy(s => s.Name))
            {
                // TODO:R6: Add support for the 'resource' search parameter type
                if ((sp.TypeElement.ObjectValue is "resource"))
                {
                    Console.WriteLine($"Skipping SearchParameter {sp.Id} ({sp.Url}) because it is target type of 'resource'!!!");
                    continue;
                }

                if (sp.Experimental == true)
                {
                    continue;
                }

                string description;

                if ((!string.IsNullOrEmpty(sp.Description)) &&
                    sp.Description.StartsWith("Multiple", StringComparison.Ordinal))
                {
                    description = string.Empty;
                }
                else
                {
                    description = sp.Description;
                }

                string searchType = sp.Type == null
                    ? string.Empty
                    : FhirSanitizationUtils.SanitizedToConvention(sp.Type.GetLiteral()!, NamingConvention.PascalCase);

                string path = sp.cgXPath();
                if (!string.IsNullOrEmpty(path))
                {
                    string temp = path
                        .Replace("f:", string.Empty, StringComparison.Ordinal)
                        .Replace('/', '.')
                        .Replace('(', '[')
                        .Replace(')', ']');

                    IEnumerable<string> split = temp
                        .Split(_splitChars, StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => s.StartsWith(complex.Name + ".", StringComparison.Ordinal));

                    path = "\"" + string.Join("\", \"", split) + "\"";
                }

                string target;

                if (!sp.Target.Any())
                {
                    target = string.Empty;
                }
                else
                {
                    List<VersionIndependentResourceTypesAll> sc =
                        [..sp.Target.Where(t => t != null).Cast<VersionIndependentResourceTypesAll>()];

                    // HACK: for http://hl7.org/fhir/SearchParameter/clinical-encounter,
                    // none of the base resources have EpisodeOfCare as target, except
                    // Procedure and DeviceRequest. There is no way you can see this from the
                    // source data we generate this from, afaik, so we need to make
                    // a special case here.
                    // Brian P reported that there are many such exceptions - but this one
                    // was reported as a bug. Again, there is no way to know this from our
                    // inputs, so this will remain manually maintained input.
                    if (sp.Id == "clinical-encounter")
                    {
                        if (_info.FhirSequence == FhirReleases.FhirSequenceCodes.STU3)
                        {
                            if (complex.Name != "Procedure" && complex.Name != "DeviceRequest")
                            {
                                sc.Remove(VersionIndependentResourceTypesAll.EpisodeOfCare);
                            }
                        }
                        else
                        {
                            if (complex.Name != "DocumentReference")
                            {
                                sc.Remove(VersionIndependentResourceTypesAll.EpisodeOfCare);
                            }
                        }
                    }

                    if (_info.FhirSequence != FhirReleases.FhirSequenceCodes.STU3)
                    {
                        if(_searchParamDefsTargetRemovals.TryGetValue((complex.Name,sp.Name), out var targetRemoval))
                        {
                            sc.Remove(targetRemoval);
                        }
                    }

                    var targetStrings = sc.Select(s => $"VersionIndependentResourceTypesAll.{s.GetLiteral()}")
                        .Order();
                    target = $", Target = [{string.Join(", ", targetStrings)}]";
                }

                string xpath = string.IsNullOrEmpty(sp.cgXPath()) ? string.Empty : ", XPath = \"" + sp.cgXPath() + "\"";
                string expression = string.IsNullOrEmpty(sp.Expression) ? string.Empty : ", Expression = \"" + sp.Expression + "\"";
                string urlComponent = $", Url = \"{sp.Url}\"";

                string[] components = sp.Component?.Select(c => $"""new SearchParamComponent("{c.Definition}", "{c.Expression}")""").ToArray() ?? [];
                string strComponents = (components.Length > 0) ? $", Component = new SearchParamComponent[] {{ {string.Join(",", components)} }}" : string.Empty;

                _writer.WriteLineIndented(
                    $"new SearchParamDefinition() " +
                        $"{{" +
                        $" Resource = \"{complex.Name}\"," +
                        $" Name = \"{sp.Name}\"," +
                        $" Code = \"{sp.Code}\"," +
                        (_info.FhirSequence == FhirReleases.FhirSequenceCodes.STU3 ?
                            $" Description = @\"{SanitizeForMarkdown(description)}\"," :
                            $" Description = new Markdown(@\"{SanitizeForMarkdown(description)}\"),") +
                        $" Type = SearchParamType.{searchType}," +
                        $" Path = [{path}]" +
                        target +
                        xpath +
                        expression +
                        urlComponent +
                        strComponents +
                        $" }},");
            }
        }

        CloseScope(true);
    }

    /// <summary>Sanitize for markdown.</summary>
    /// <param name="value">The value.</param>
    private static string SanitizeForMarkdown(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Replace("\"", "\"\"").Replace("\r", @"\r").Replace("\n", @"\n");
    }

    /// <summary>Writes the C# to FHIR map dictionary.</summary>
    /// <param name="writtenPrimitives">  The written primitives.</param>
    /// <param name="writtenComplexTypes">List of types of the written complexes.</param>
    /// <param name="writtenResources">   The written resources.</param>
    private void WriteCsToString(
        IEnumerable<WrittenModelInfo> writtenPrimitives,
        IEnumerable<WrittenModelInfo> writtenComplexTypes,
        IEnumerable<WrittenModelInfo> writtenResources)
    {
        _writer.WriteLineIndented("public static Dictionary<Type,string> FhirCsTypeToString = new Dictionary<Type,string>()");
        OpenScope();

        foreach (WrittenModelInfo type in writtenPrimitives.Concat(writtenComplexTypes).OrderBy(t => t.FhirName))
        {
            _writer.WriteLineIndented($"{{ typeof({type.CsName}), \"{type.FhirName}\" }},");
        }

        _writer.WriteLine(string.Empty);

        foreach (WrittenModelInfo type in writtenResources.OrderBy(t => t.FhirName))
        {
            _writer.WriteLineIndented($"{{ typeof({type.CsName}), \"{type.FhirName}\" }},");
        }

        CloseScope(true);
    }

    /// <summary>Writes the FHIR to C# map dictionary.</summary>
    /// <param name="writtenPrimitives">  The written primitives.</param>
    /// <param name="writtenComplexTypes">List of types of the written complexes.</param>
    /// <param name="writtenResources">   The written resources.</param>
    private void WriteFhirToCs(
        IEnumerable<WrittenModelInfo> writtenPrimitives,
        IEnumerable<WrittenModelInfo> writtenComplexTypes,
        IEnumerable<WrittenModelInfo> writtenResources)
    {
        _writer.WriteLineIndented("public static Dictionary<string,Type> FhirTypeToCsType = new Dictionary<string,Type>()");
        OpenScope();

        foreach (WrittenModelInfo type in writtenPrimitives.Concat(writtenComplexTypes).OrderBy(t => t.FhirName))
        {
            _writer.WriteLineIndented($"{{ \"{type.FhirName}\", typeof({type.CsName}) }},");
        }

        _writer.WriteLine(string.Empty);

        foreach (WrittenModelInfo type in writtenResources.OrderBy(t => t.FhirName))
        {
            _writer.WriteLineIndented($"{{ \"{type.FhirName}\", typeof({type.CsName}) }},");
        }

        CloseScope(true);
    }

    /// <summary>Writes the FHIR version.</summary>
    private void WriteFhirVersion()
    {
        _writer.WriteLineIndented($"""public static string Version => "{_info.FhirVersionLiteral}";""");
        _writer.WriteLine();
    }

    /// <summary>Writes the supported resources dictionary.</summary>
    /// <param name="resources">The written resources.</param>
    private void WriteSupportedResources(IEnumerable<WrittenModelInfo> resources)
    {
        _writer.WriteLineIndented("public static List<string> SupportedResources = new List<string>()");
        OpenScope();

        foreach (WrittenModelInfo wmi in resources.OrderBy(s => s.FhirName))
        {
            _writer.WriteLineIndented($"\"{wmi.FhirName}\",");
        }

        CloseScope(true);
    }

    /// <summary>Writes the shared enums.</summary>
    private void WriteSharedValueSets(GenSubset subset)
    {
        HashSet<string> usedEnumNames = [];

        string filename = Path.Combine(_exportDirectory, "Generated", "Template-Bindings.cs");

        using Stream stream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(filename, FileMode.Create);
        using ExportStreamWriter writer = new(stream);

        if (_generateHashesInsteadOfOutput)
        {
            writer.NewLine = "\r\n";
        }

        _writer = writer;

        WriteHeaderBasic();
        WriteNamespaceOpen();

        // traverse all versions of all value sets
        foreach ((string unversionedUrl, string[] versions) in _info.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            if (ExclusionSet.Contains(unversionedUrl))
            {
                continue;
            }

            // traverse value sets starting with highest version
            foreach (string vsVersion in versions.OrderDescending())
            {
                if (!_info.TryGetValueSet(unversionedUrl, vsVersion, out ValueSet? vs))
                {
                    continue;
                }

                IEnumerable<StructureElementCollection> coreBindings = _info.CoreBindingsForVs(vs.Url).ToList();
                Hl7.Fhir.Model.BindingStrength? strongestBinding = _info.StrongestBinding(coreBindings);

                if (strongestBinding != Hl7.Fhir.Model.BindingStrength.Required)
                {
                    /* Since required bindings cannot be extended, those are the only bindings that
                        can be represented using enums in the POCO classes (using <c>Code&lt;T&gt;</c>). All other coded members
                        use <c>Code</c>, <c>Coding</c> or <c>CodeableConcept</c>.
                        Consequently, we only need to generate enums for valuesets that are used as
                        required bindings anywhere in the data model. */
                    continue;
                }

                IEnumerable<string> referencedBy = coreBindings.cgExtractBaseTypes(_info);

                if ((referencedBy.Count() < 2) && !ExplicitSharedValueSets.Contains((_info.FhirSequence.ToString(), vs.Url)))
                {
                    /* ValueSets that are used in a single POCO are generated as a nested enum inside that
                        * POCO, not here in the shared valuesets */

                    continue;
                }

                // If this is a shared valueset that will be generated in the base or conformance subset,
                // don't also generate it here.
                bool writeValueSet =
                    (subset.HasFlag(GenSubset.Satellite) && !(_baseSubsetValueSets.Contains(vs.Url) || _conformanceSubsetValueSets.Contains(vs.Url)))
                    || subset.HasFlag(GenSubset.Conformance) && _conformanceSubsetValueSets.Contains(vs.Url)
                    || subset.HasFlag(GenSubset.Base) && _baseSubsetValueSets.Contains(vs.Url);

                if (!WriteEnum(vs, string.Empty, usedEnumNames, silent: !writeValueSet))
                {
                    // value set could not be written (e.g., could not be expanded)
                    Console.WriteLine($"Wanted to write an enum for ValueSet {vs.Url}, but could not.");
                    continue;
                }

                if (writeValueSet)
                {
                    _modelWriter.WriteLineIndented($"// Generated Shared Enumeration: {_writtenValueSets[vs.Url].ValueSetName} ({vs.Url})");
                }
                else
                {
                    _modelWriter.WriteLineIndented($"// Deferred generation of Shared Enumeration (will be generated in another subset): {_writtenValueSets[vs.Url].ValueSetName} ({vs.Url})");
                }

                _modelWriter.IncreaseIndent();

                foreach (string path in coreBindings.SelectMany(ec => ec.Elements.Select(e => e)).Order(ElementDefinitionComparer.Instance).Select(e => e.Path))
                {
                    string name = path.Split('.')[0];

                    if (_info.ComplexTypesByName.ContainsKey(name))
                    {
                        _modelWriter.WriteLineIndented($"// Used in model class (type): {path}");
                        continue;
                    }

                    _modelWriter.WriteLineIndented($"// Used in model class (resource): {path}");
                }

                _modelWriter.DecreaseIndent();
                _modelWriter.WriteLine(string.Empty);
            }
        }

        WriteNamespaceClose();

        if (_generateHashesInsteadOfOutput)
        {
            writer.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(stream);
            _fileHashes.Add(filename[_exportDirectory.Length..], hash);

            writer.Close();
        }
        else
        {
            writer.Flush();
            writer.Close();
        }
    }

    private void WriteInterfaces(
        IEnumerable<StructureDefinition> complexes,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        foreach (StructureDefinition complex in complexes.OrderBy(c => c.Name))
        {
            //if (_exclusionSet.Contains(complex.Name))
            //{
            //    continue;
            //}

            // for now, we only generate interfaces when running in the satellite configuration
            if (subset.HasFlag(GenSubset.Satellite))
            {
                WriteInterface(complex, ref writtenModels, subset);
            }
        }
    }

    private void WriteInterface(
        StructureDefinition complex,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        string exportName = "I" + complex.Name.ToPascalCase();
        string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

        _modelWriter.WriteLineIndented($"// {exportName}.cs");

        using Stream stream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(filename, FileMode.Create);
        using ExportStreamWriter writer = new(stream);

        if (_generateHashesInsteadOfOutput)
        {
            writer.NewLine = "\r\n";
        }

        _writer = writer;

        WriteHeaderComplexDataType();

        WriteNamespaceOpen();

        WriteInterfaceComponent(complex.cgComponent(), exportName, subset, ref writtenModels);

        WriteNamespaceClose();

        WriteFooter();

        if (_generateHashesInsteadOfOutput)
        {
            writer.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(stream);
            _fileHashes.Add(filename[_exportDirectory.Length..], hash);

            writer.Close();
        }
        else
        {
            writer.Flush();
            writer.Close();
        }
    }


    /// <summary>Write C# classes for FHIR resources.</summary>
    /// <param name="complexes">    The complex data types.</param>
    /// <param name="writtenModels">[in,out] The written models.</param>
    /// <param name="subset"></param>
    private void WriteResources(
        IEnumerable<StructureDefinition> complexes,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        foreach (StructureDefinition complex in complexes.OrderBy(c => c.Name))
        {
            if (ExclusionSet.Contains(complex.Name))
            {
                continue;
            }

            if ((subset.HasFlag(GenSubset.Base) && _baseSubsetResourceTypes.Contains(complex.Name)) ||
                (subset.HasFlag(GenSubset.Conformance) && _conformanceSubsetResourceTypes.Contains(complex.Name)) ||
                (subset.HasFlag(GenSubset.Satellite) && !_baseSubsetResourceTypes.Concat(_conformanceSubsetResourceTypes).Contains(complex.Name)))
            {
                WriteResource(complex, ref writtenModels, subset);
            }
        }
    }

    /// <summary>Write a C# class for a FHIR resource.</summary>
    /// <param name="complex">      The complex data type.</param>
    /// <param name="writtenModels">[in,out] The written models.</param>
    /// <param name="subset"></param>
    private void WriteResource(
        StructureDefinition complex,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        string exportName = complex.Name.ToPascalCase();

        writtenModels.Add(
            complex.Name,
            new WrittenModelInfo(FhirName: complex.Name, CsName: $"{Namespace}.{exportName}",
                IsAbstract: complex.Abstract == true));

        string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

        _modelWriter.WriteLineIndented($"// {exportName}.cs");

        using Stream stream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(filename, FileMode.Create);
        using ExportStreamWriter writer = new(stream);

        if (_generateHashesInsteadOfOutput)
        {
            writer.NewLine = "\r\n";
        }

        _writer = writer;

        WriteHeaderComplexDataType();

        WriteNamespaceOpen();

        WriteComponent(complex.cgComponent(), exportName, true, subset);

        WriteNamespaceClose();

        WriteFooter();

        if (_generateHashesInsteadOfOutput)
        {
            writer.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(stream);
            _fileHashes.Add(filename[_exportDirectory.Length..], hash);

            writer.Close();
        }
        else
        {
            writer.Flush();
            writer.Close();
        }
    }

    /// <summary>Writes the complex data types.</summary>
    /// <param name="complexes">    The complex data types.</param>
    /// <param name="writtenModels">[in,out] The written models.</param>
    /// <param name="subset"></param>
    private void WriteComplexDataTypes(
        IEnumerable<StructureDefinition> complexes,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        foreach (StructureDefinition complex in complexes.OrderBy(c => c.Name))
        {
            if (ExclusionSet.Contains(complex.Name))
            {
                continue;
            }

            if ((subset.HasFlag(GenSubset.Base) && _baseSubsetComplexTypes.Contains(complex.Name)) ||
                (subset.HasFlag(GenSubset.Conformance) && _conformanceSubsetComplexTypes.Contains(complex.Name)) ||
                (subset.HasFlag(GenSubset.Satellite) && !_baseSubsetComplexTypes.Concat(_conformanceSubsetComplexTypes).Contains(complex.Name)))
            {
                WriteComplexDataType(complex, ref writtenModels, subset);
            }
        }
    }

    /// <summary>Writes a complex data type.</summary>
    /// <param name="complex">      The complex data type.</param>
    /// <param name="writtenModels">[in,out] The written models.</param>
    /// <param name="subset"></param>
    private void WriteComplexDataType(
        StructureDefinition complex,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        string exportName = complex.Name.ToPascalCase();

        if (TypeNameMappings.TryGetValue(exportName, out string? value))
        {
            exportName = value;
        }

        writtenModels.Add(
            complex.Name,
            new WrittenModelInfo(FhirName: complex.Name, CsName: $"{Namespace}.{exportName}",
                IsAbstract: complex.Abstract == true));

        string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

        _modelWriter.WriteLineIndented($"// {exportName}.cs");

        using Stream stream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(filename, FileMode.Create);
        using ExportStreamWriter writer = new(stream);

        if (_generateHashesInsteadOfOutput)
        {
            writer.NewLine = "\r\n";
        }

        _writer = writer;

        WriteHeaderComplexDataType();

        WriteNamespaceOpen();

        WriteComponent(complex.cgComponent(), exportName, false, subset);

        WriteNamespaceClose();

        WriteFooter();

        if (_generateHashesInsteadOfOutput)
        {
            writer.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(stream);
            _fileHashes.Add(filename[_exportDirectory.Length..], hash);

            writer.Close();
        }
        else
        {
            writer.Flush();
            writer.Close();
        }
    }

    private void WriteInterfaceComponent(
        ComponentDefinition complex,
        string exportName,
        GenSubset subset,
        ref Dictionary<string, WrittenModelInfo> writtenModels)
    {
        string complexName = complex.cgName();

        List<WrittenElementInfo> exportedElements = [];

        WriteIndentedComment($"{complex.Element.Short}");

        StructureDefinition? parentInterface = _info.GetParentInterface(complex.Structure);

        if (parentInterface == null)
        {
            _writer.WriteLineIndented($"public interface {exportName}");
        }
        else
        {
            string parentInterfaceExportName = "I" + parentInterface.Name.ToPascalCase();

            _writer.WriteLineIndented(
                $"public interface" +
                    $" {exportName}" +
                    $" : {Namespace}.{parentInterfaceExportName}");
        }


        // open class
        OpenScope();

        WriteInterfaceElements(complex, exportName, ref exportedElements);

        // close class
        CloseScope();

        // get the list of resources that implement this interface
        foreach (StructureDefinition resourceSd in _info.ResourcesForInterface(complex.Structure).OrderBy(s => s.Name))
        {
            // check if this is a model we have written (do not write for resources out of our subset)
            if (!writtenModels.ContainsKey(resourceSd.Name))
            {
                continue;
            }

            string resourceExportName = resourceSd.Name.ToPascalCase();

            _writer.WriteLineIndented($"public partial class {resourceExportName} : {exportName}");

            // open class
            OpenScope();

            // get the elements for this resource and put them into a dictionary for easy lookup.
            // note that we can restrict to top level since interfaces are currently only top level
            // use the name as determined by BuildElementInfo for the key
            Dictionary<string, ElementDefinition> resourceElements = resourceSd.cgElements(topLevelOnly: true)
                .ToDictionary(e => e.cgName(removeChoiceMarker: true));

            // iterate over the elements of the interface we exported
            foreach (WrittenElementInfo interfaceEi in exportedElements)
            {
                string pn = exportName + "." + interfaceEi.PropertyName;

                WrittenElementInfo? resourceEi = null;
                if (resourceElements.TryGetValue(interfaceEi.FhirElementName, out ElementDefinition? resourceEd))
                {
                    resourceEi = BuildElementInfo(resourceExportName, resourceEd);
                }

                WriteInterfaceElementGettersAndSetters(
                    resourceExportName,
                    resourceEd,
                    resourceEi,
                    exportName,
                    interfaceEi);
            }

            // close class
            CloseScope();
        }
    }

    private void WriteInterfaceElementGettersAndSetters(
        string resourceExportName,
        ElementDefinition? resourceEd,
        WrittenElementInfo? resourceEi,
        string interfaceExportName,
        WrittenElementInfo interfaceEi)
    {
        string pn = interfaceEi.PropertyName;
        bool isList = interfaceEi.PropertyType is ListTypeReference;
        string it = isList ? interfaceEi.PropertyType.PropertyTypeString : WithNullabilityMarking(interfaceEi.PropertyType.PropertyTypeString);

        if ((resourceEd == null) || (resourceEi == null))
        {
            writeEmptyGetterAndSetter(it, pn, isList);
        }
        else if (interfaceEi.PropertyType.PropertyTypeString == resourceEi.PropertyType.PropertyTypeString)
        {
            writeOneOnOneGetterAndSetter(it, pn, isList);
        }

        // a resource is allowed to have a scalar in place of a list
        else if ((interfaceEi.PropertyType is ListTypeReference interfaceLtr) &&
            (interfaceLtr.Element.PropertyTypeString == resourceEi.PropertyType.PropertyTypeString))
        {
            writeSingleToListGetterAndSetter(it, pn, isList);
        }
        else
        {
            writeIncompatibleGetterSetter(it, pn, isList);
        }

        if (!TryGetPrimitiveType(interfaceEi.PropertyType, out PrimitiveTypeReference? interfacePtr))
        {
            return;
        }

        string ppn = interfaceEi.PrimitiveHelperName!;
        string pit = isList ? interfacePtr.ConveniencePropertyTypeString : WithNullabilityMarking(interfacePtr.ConveniencePropertyTypeString);

        if ((resourceEd == null) || (resourceEi == null))
        {
            writeEmptyGetterAndSetter(pit, ppn, isList);
        }
        else if (interfaceEi.PropertyType == resourceEi.PropertyType)
        {
            writeOneOnOneGetterAndSetter(pit, ppn, isList);
        }
        else
        {
            writeIncompatibleGetterSetter(pit, ppn, isList);
        }

        return;

        void writeOneOnOneGetterAndSetter(string propertyType, string propertyName, bool allowNull)
        {
            if(allowNull)
                _writer.WriteLineIndented("[AllowNull]");
            _writer.WriteLineIndented("[IgnoreDataMember]");
            _writer.WriteLineIndented($"{propertyType} {interfaceExportName}.{propertyName}");
            OpenScope();
            _writer.WriteLineIndented($"get => {propertyName};");
            _writer.WriteLineIndented($"set => {propertyName} = value!;");
            CloseScope();
        }

        void writeSingleToListGetterAndSetter(string propertyType, string propertyName, bool allowNull)
        {
            if (allowNull)
                _writer.WriteLineIndented("[AllowNull]");
            _writer.WriteLineIndented("[IgnoreDataMember]");
            _writer.WriteLineIndented($"{propertyType} {interfaceExportName}.{propertyName}");
            OpenScope();

            _writer.WriteLineIndented($"get => {propertyName} is null ? [] : [{propertyName}];");

            _writer.WriteLineIndented("set");
            OpenScope();
            _writer.WriteLineIndented($"{propertyName} = value switch");

            OpenScope();
            _writer.WriteLineIndented($"{{ Count: 0 }} => null,");
            _writer.WriteLineIndented($"{{ Count: 1 }} => value.First(),");
            _writer.WriteLineIndented($"_ => throw new NotImplementedException(\"Resource {resourceExportName} can only have a single {propertyName} value\")");
            CloseScope(includeSemicolon: true, suppressNewline: true);

            CloseScope(suppressNewline: true);
            CloseScope();
        }

        void writeIncompatibleGetterSetter(string propertyType, string propertyName, bool allowNull)
        {
            string message = $"{resourceExportName}.{resourceEi.PropertyName} is incompatible with " +
                             $"{interfaceExportName}.{interfaceEi.FhirElementName}.";
            WriteIndentedComment(message, isSummary: false, isRemarks: true);

            if (allowNull)
                _writer.WriteLineIndented("[AllowNull]");
            _writer.WriteLineIndented("[IgnoreDataMember]");
            _writer.WriteLineIndented($"{propertyType} {interfaceExportName}.{propertyName}");

            OpenScope();
            _writer.WriteLineIndented($"get => {emptyInterfaceType()};");
            _writer.WriteLineIndented($"set => throw new NotImplementedException(\"{message}\");");
            CloseScope();
        }

        string emptyInterfaceType() => interfaceEi.PropertyType is ListTypeReference ? "[]" : "null";

        void writeEmptyGetterAndSetter(string propertyType, string propertyName, bool allowNull)
        {
            if (allowNull)
                _writer.WriteLineIndented("[AllowNull]");
            _writer.WriteLineIndented("[IgnoreDataMember]");
            _writer.WriteLineIndented($"{propertyType} {interfaceExportName}.{propertyName}");
            OpenScope();
            _writer.WriteLineIndented($"get => {emptyInterfaceType()};");
            _writer.WriteLineIndented($"set => throw new NotImplementedException(\"Resource {resourceExportName}" +
                                      $" does not implement {interfaceExportName}.{interfaceEi.FhirElementName}\");");
            CloseScope();
        }
    }

    private static string WithNullabilityMarking(string type) => type.EndsWith("?") ? type : type + "?";


    private void WriteInterfaceElements(
        ComponentDefinition complex,
        string exportedComplexName,
        ref List<WrittenElementInfo> exportedElements)
    {
        IOrderedEnumerable<ElementDefinition> elementsToGenerate = complex.cgGetChildren()
            .Where(e => !e.cgIsInherited(complex.Structure))
            .OrderBy(e => e.cgFieldOrder());

        string structureName = complex.cgName();

        foreach (ElementDefinition element in elementsToGenerate)
        {
            WrittenElementInfo ei = BuildElementInfo(exportedComplexName, element);
            exportedElements.Add(ei);

            string name = element.cgName(removeChoiceMarker: true);
            string? since = _sinceAttributes.GetValueOrDefault(element.Path);
            (string, string)? until = _untilAttributes.TryGetValue(element.Path, out (string, string) u) ? u : default((string, string)?);

            string? description = MakeAttributeRemarkForNewOrDeprecatedProperties(name, element.Short.Replace("{{title}}", structureName), since, until);

            if (TryGetPrimitiveType(ei.PropertyType, out PrimitiveTypeReference? eiPtr))
            {
                WriteIndentedComment(element.Short.Replace("{{title}}", structureName));
                _writer.WriteLineIndented($"/// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>");

                _writer.WriteLineIndented($"{WithNullabilityMarking(eiPtr.ConveniencePropertyTypeString)} {ei.PrimitiveHelperName} {{ get; set; }}");
                _writer.WriteLine();
            }

            if (description != null) WriteIndentedComment(description);
            var typ = ei.PropertyType is ListTypeReference
                ? ei.PropertyType.PropertyTypeString
                : WithNullabilityMarking(ei.PropertyType.PropertyTypeString);
            _writer.WriteLineIndented("[AllowNull]");
            _writer.WriteLineIndented($"{typ} {ei.PropertyName} {{ get; set; }}");
            _writer.WriteLine();
        }
    }

    private void WriteComponentComment(ComponentDefinition cd)
    {
        List<string> strings = [];

        if (!string.IsNullOrEmpty(cd.Element.Short))
        {
            strings.Add(cd.Element.Short);
        }

        if (!string.IsNullOrEmpty(cd.Element.Definition) &&
            !cd.Element.Definition.Equals(cd.Element.Short, StringComparison.Ordinal) &&
            !cd.Element.Definition.Equals(cd.Element.Short + ".", StringComparison.Ordinal))
        {
            strings.Add(cd.Element.Definition);
        }

        if (!string.IsNullOrEmpty(cd.Element.Comment) &&
            !cd.Element.Comment.Equals(cd.Element.Short, StringComparison.Ordinal) &&
            !cd.Element.Comment.Equals(cd.Element.Definition, StringComparison.Ordinal))
        {
            strings.Add(cd.Element.Comment);
        }

        switch (strings.Count)
        {
            case 0:
                WriteIndentedComment("MISSING DESCRIPTION");
                return;

            case 1:
                WriteIndentedComment(strings[0]);
                return;

            case 2:
                WriteIndentedComment(strings[0]);
                WriteIndentedComment(strings[1], isSummary: false, isRemarks: true);
                return;

            case 3:
                WriteIndentedComment(strings[0]);
                WriteIndentedComment(string.Join("\n", strings.Skip(1)), isSummary: false, isRemarks: true);
                return;
        }
    }

    /// <summary>Writes a component.</summary>
    /// <param name="complex">              The complex data type.</param>
    /// <param name="exportName">           Name of the export.</param>
    /// <param name="isResource">           True if is resource, false if not.</param>
    /// <param name="depth">                The depth.</param>
    /// <param name="subset"></param>
    private void WriteComponent(
        ComponentDefinition complex,
        string exportName,
        bool isResource,
        GenSubset subset)
    {
        string complexName = complex.cgName();
        bool isAbstract = complex.Structure.Abstract == true;

        List<WrittenElementInfo> exportedElements = [];

        WriteComponentComment(complex);
        WriteSerializable();

        string fhirTypeConstructor = $"\"{complexName}\",\"{complex.cgUrl()}\"";
        _writer.WriteLineIndented($"[FhirType({fhirTypeConstructor})]");

        var isPatientClass = false;

        if (complex.cgBaseTypeName(_info, false) == "Quantity")
        {
            // Constrained quantities are handled differently
            WriteConstrainedQuantity(complex, exportName);
            return;
        }

        string abstractFlag = isAbstract ? " abstract" : string.Empty;
        List<string> interfaces = [];

        if (_cqlModelInfo?.patientClassName != null)
        {
            // Just skip the model alias, I am currently not bothered enough to be more precise
            var className = _cqlModelInfo.patientClassName.Split('.')[1];
            isPatientClass = complexName == className;
        }

        if (isPatientClass) interfaces.Add($"{Namespace}.IPatient");

        ElementDefinition? identifierElement = null;

        if (isResource)
        {
            identifierElement = complex.cgGetChildren(includeDescendants: false).SingleOrDefault(isIdentifierProperty);
            if (identifierElement != null)
            {
                if (identifierElement.cgIsArray())
                    interfaces.Add("IIdentifiable<List<Identifier>>");
                else
                    interfaces.Add("IIdentifiable<Identifier?>");
            }
        }

        WrittenElementInfo? primaryCodeElementInfo = isResource ? getPrimaryCodedElementInfo(complex, exportName) : null;

        if (primaryCodeElementInfo != null)
        {
            string nullable = primaryCodeElementInfo.PropertyType is ListTypeReference ? "" : "?";
            interfaces.Add($"ICoded<{primaryCodeElementInfo.PropertyType.PropertyTypeString}{nullable}>");
        }

        ElementDefinition? modifierElement = complex.cgGetChild("modifierExtension");
        if (modifierElement != null)
        {
            if (!modifierElement.cgIsInherited(complex.Structure))
            {
                interfaces.Add($"{Namespace}.IModifierExtendable");
            }
        }

        string interfacesSuffix = interfaces.Count != 0 ? $", {string.Join(", ", interfaces)}" : string.Empty;
        string classDeclaration = $"public{abstractFlag} partial class" +
                                  $" {exportName}";

        if (complex.Structure.BaseDefinition is not null)
        {
            classDeclaration +=
                $" : {Namespace}.{DetermineExportedBaseTypeName(complex.cgBaseTypeName(_info, false))}{interfacesSuffix}";
        }

        _writer.WriteLineIndented(classDeclaration);

        // open class
        OpenScope();

        if(complex.Structure.Abstract != true)
            WritePropertyTypeName(complex.cgName());

        string validationRegEx = complex.cgValidationRegEx();
        if (!string.IsNullOrEmpty(validationRegEx))
        {
            WriteIndentedComment(
                $"Must conform to pattern \"{validationRegEx}\"",
                false);

            _writer.WriteLineIndented($"public const string PATTERN = @\"{validationRegEx}\";");

            _writer.WriteLine(string.Empty);
        }

        WriteEnums(complex, exportName);

        // check for nested components
        foreach (ComponentDefinition component in complex.cgChildComponents(_info))
        {
            string componentExportName;

            if (GetExplicitName(component.Element, _info.FhirSequence) is {} explicitName)
            {
                componentExportName = $"{explicitName}Component";
            }
            else
            {
                componentExportName =
                    $"{component.cgName(NamingConvention.PascalCase)}Component";
            }

            WriteBackboneComponent(
                component,
                componentExportName,
                exportName,
                subset);
        }

        WriteElements(complex, exportName, ref exportedElements, subset);

        if (identifierElement != null)
        {
            if (identifierElement.cgIsArray())
                _writer.WriteLineIndented("List<Identifier> IIdentifiable<List<Identifier>>.Identifier { get => Identifier; set => Identifier = value!; }");
            else
                _writer.WriteLineIndented("Identifier? IIdentifiable<Identifier?>.Identifier { get => Identifier; set => Identifier = value!; }");

            _writer.WriteLine(string.Empty);
        }

        if (primaryCodeElementInfo != null)
        {
            var (codedType, bang) = primaryCodeElementInfo.PropertyType switch
            {
                ListTypeReference { PropertyTypeString: { } n } => (n, string.Empty),
                { PropertyTypeString: {} n } => (WithNullabilityMarking(n), "!")
            };

            _writer.WriteLineIndented($"{codedType} ICoded<{codedType}>.Code {{ get => {primaryCodeElementInfo.PropertyName}; " +
                                      $"set => {primaryCodeElementInfo.PropertyName} = value{bang}; }}");
            _writer.WriteLineIndented($"IReadOnlyCollection<Coding> ICoded.ToCodings() => {primaryCodeElementInfo.PropertyName}?.ToCodings() ?? [];");
            _writer.WriteLine(string.Empty);
        }

        if (isPatientClass)
        {
            var birthdayProperty = exportedElements.SingleOrDefault(ee => ee.FhirElementName + ".value" == _cqlModelInfo?.patientBirthDatePropertyName);

            if (birthdayProperty != null)
            {
                _writer.WriteLineIndented($"Hl7.Fhir.Model.Date? {Namespace}.IPatient.BirthDate => {birthdayProperty.PropertyName};");
                _writer.WriteLine(string.Empty);
            }
        }

        WriteCopyTo(exportName, exportedElements);

        if (!isAbstract || exportName == "Base")
        {
            WriteDeepCopy(exportName);
        }

        WriteCompareChildren(exportName, exportedElements);
        WriteDictionarySupport(exportName, exportedElements);

        // close class
        CloseScope();

        WrittenElementInfo? getPrimaryCodedElementInfo(ComponentDefinition complex, string exportName)
        {
            var primaryCodePath = _cqlModelClassInfo?.TryGetValue(complex.cgName(), out var classInfo) == true && !string.IsNullOrEmpty(classInfo.primaryCodePath)
                ? (complex.cgName() + "." + classInfo.primaryCodePath)
                : null;

            var elem = primaryCodePath != null ? (tryFindElementInComplex(complex, primaryCodePath, out var e) ? e : null) : null;
            var primaryCodeElementInfo = elem != null ? BuildElementInfo(exportName, elem) : null;

            if (primaryCodePath != null && primaryCodeElementInfo == null)
            {
                Console.WriteLine($"Warning: Cannot locate primary code path {primaryCodePath}, so no ICoded<T> was added to this type's signature.");
            }

            return primaryCodeElementInfo;
        }
    }

    private bool tryFindElementInComplex(ComponentDefinition component, string name, out ElementDefinition elem)
    {
        if (component.Structure.cgTryGetElementByPath(name, out elem!)) return true;
        if (component.Structure.cgTryGetElementByPath(name + "[x]", out elem!)) return true;

        return false;
    }

    private string DetermineExportedBaseTypeName(string baseTypeName)
    {
        // These two classes are more like interfaces, we treat their subclasses
        // as subclasses of DomainResource instead.
        if (baseTypeName == "MetadataResource" || baseTypeName == "CanonicalResource")
        {
            return "DomainResource";
        }

        if (_info.FhirSequence < FhirReleases.FhirSequenceCodes.R5)
        {
            // Promote R4 datatypes (all derived from Element/BackboneElement) to the right new subclass
            if (baseTypeName == "BackboneElement" && _info.FhirSequence > FhirReleases.FhirSequenceCodes.STU3)
            {
                return "BackboneType";
            }

            if (baseTypeName == "Element")
            {
                return "DataType";
            }
        }

        return baseTypeName;
    }

    private string getDynamicTypeForAbstractTypeName(string abstractTypeName) =>
        abstractTypeName switch
        {
            "Hl7.Fhir.Model.Resource" => "DynamicResource",
            "Hl7.Fhir.Model.DataType" => "DynamicDataType",
            "Hl7.Fhir.Model.PrimitiveType" => "DynamicPrimitive",
            { } s => s
        };

    private void WriteDictionarySupport(string exportName, List<WrittenElementInfo> exportedElements)
    {
        WriteDictionaryTryGetValue(exportName, exportedElements);
        WriteDictionaryTrySetValue(exportName, exportedElements);
        WriteDictionaryPairs(exportName, exportedElements);
    }

    private void WriteDictionaryPairs(string exportName, List<WrittenElementInfo> exportedElements)
    {
        // Base implementation differs from subclasses and is hand-written code in a separate partical class
        if (exportName == "Base")
        {
            return;
        }

        if (!exportedElements.Any())
        {
            return;
        }

        _writer.WriteLineIndented("public override IEnumerable<KeyValuePair<string, object>> EnumerateElements()");
        OpenScope();
        _writer.WriteLineIndented("foreach (var kvp in base.EnumerateElements()) yield return kvp;");

        foreach (WrittenElementInfo info in exportedElements)
        {
            string elementProp = $"\"{info.FhirElementName}\"";
            _writer.WriteLineIndented($"if (_{info.PropertyName}{(info.PropertyType is ListTypeReference ? "?.Any() is true" : " is not null")} && !_{info.PropertyName}.InOverflow<{getDynamicTypeForAbstractTypeName(info.PropertyType.PropertyTypeString)}>()) yield return new " +
                $"KeyValuePair<string,object>({elementProp},_{info.PropertyName});");
        }

        CloseScope();
    }

    private void WriteDictionaryTryGetValue(string exportName, List<WrittenElementInfo> exportedElements)
    {
       // Base implementation differs from subclasses and is hand-written code in a separate partial class
       if (exportName == "Base")
       {
           return;
       }

        // Don't override anything if there are no additional elements.
        if (!exportedElements.Any())
        {
            return;
        }

        _writer.WriteLineIndented("public override bool TryGetValue(string key, [NotNullWhen(true)] out object? value)");
        OpenScope();

        // switch
        _writer.WriteLineIndented("switch (key)");
        OpenScope();

        foreach (WrittenElementInfo info in exportedElements)
        {
            writeCase(info.FhirElementName, info.PropertyName, info.PropertyType);
        }

        void writeCase(string key, string propName, TypeReference type)
        {
            string overflowTypeName = getDynamicTypeForAbstractTypeName(type.PropertyTypeString);

            _writer.WriteLineIndented($"case \"{key}\":");
            _writer.IncreaseIndent();

            _writer.WriteLineIndented($"if (_{propName}.InOverflow<{overflowTypeName}>())");
            _writer.OpenScope();
            _writer.WriteLineIndented($"value = Overflow[\"{key}\"];");
            _writer.WriteLineIndented("return true;");
            _writer.CloseScope();
            _writer.WriteLineIndented($"value = _{propName};");
            _writer.WriteLineIndented($"return (value as {type.PropertyTypeString}){(type is ListTypeReference ? "?.Any() is true" : " is not null")};");

            _writer.DecreaseIndent();
        }

        _writer.WriteLineIndented("default:");
        _writer.IncreaseIndent();
        writeBaseTryGetValue();

        _writer.DecreaseIndent();

        // end switch
        CloseScope(includeSemicolon: false);

        CloseScope();

        void writeBaseTryGetValue() => _writer.WriteLineIndented("return base.TryGetValue(key, out value);");
    }


    private void WriteDictionaryTrySetValue(string exportName, List<WrittenElementInfo> exportedElements)
    {
        // Base implementation differs from subclasses and is hand-written code in a separate partical class
        if (exportName == "Base")
        {
            return;
        }

        // Don't override anything if there are no additional elements.
        if (!exportedElements.Any())
        {
            return;
        }

        _writer.WriteLineIndented("public override Base SetValue(string key, object? value)");
        OpenScope();

        _writer.WriteLineIndented("if(value is not (null or Hl7.Fhir.Model.Base or IList)) throw new ArgumentException(\"Value must be a Base or a list of Base\", nameof(value));");

        // switch
        _writer.WriteLineIndented("switch (key)");
        OpenScope();

        foreach (WrittenElementInfo info in exportedElements)
        {
            writeSetValueCase(info.FhirElementName, null, info.PropertyType, info.PropertyName, info.Required);
        }

        void writeSetValueCase(string fhirName, string? when, TypeReference type, string propName, bool required)
        {
            string overflowTypeName = getDynamicTypeForAbstractTypeName(type.PropertyTypeString);

            _writer.WriteLineIndented(when is not null ? $"case \"{fhirName}\" when {when}:" : $"case \"{fhirName}\":");

            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"if (value is not ({type.PropertyTypeString} or null))");
            _writer.OpenScope();
            _writer.WriteLineIndented($"{propName} = OverflowNull<{overflowTypeName}>.INSTANCE;");
            _writer.WriteLineIndented($"Overflow[\"{fhirName}\"] = value;");
            _writer.CloseScope();

            // Because our list properties are never null when you get them, but can be set to null,
            // we need a bang after the assignment here for lists, but not for other elements.
            _writer.WriteLineIndented($"else {propName} = ({type.PropertyTypeString}?)value{(type is ListTypeReference || required ? "!" : "")};");
            _writer.WriteLineIndented($"return this;");
            _writer.DecreaseIndent();
        }

        _writer.WriteLineIndented("default:");
        _writer.IncreaseIndent();
        writeBaseTrySetValue();

        _writer.DecreaseIndent();

        // end switch
        CloseScope(includeSemicolon: false);

        CloseScope();

        void writeBaseTrySetValue() => _writer.WriteLineIndented("return base.SetValue(key, value);");
    }

    /// <summary>Writes the PairwiseEquality.</summary>
    /// <param name="exportName">      Name of the export.</param>
    /// <param name="exportedElements">The exported elements.</param>
    private void WriteCompareChildren(
        string exportName,
        List<WrittenElementInfo> exportedElements)
    {
        // Base implementation is hand-written code in a separate partial class.
        if (exportName == "Base")
        {
            return;
        }

        _writer.WriteLineIndented("public override bool CompareChildren(Base other, IEqualityComparer<Base> comparer)");

        OpenScope();
        _writer.WriteLineIndented($"if(other is not {exportName} otherT) return false;");
        _writer.WriteLine(string.Empty);

        _writer.WriteLineIndented("if(!base.CompareChildren(otherT, comparer)) return false;");

        _writer.WriteLineIndented(
            "#pragma warning disable CS8604 // Possible null reference argument - netstd2.1 has a wrong nullable signature here");

        foreach (WrittenElementInfo info in exportedElements)
        {
            if(info.PropertyType is CqlTypeReference)
            {
                _writer.WriteLineIndented(
                    $"if( _{info.PropertyName} != otherT._{info.PropertyName} )" +
                        $" return false;");
            }
            else if (info.PropertyType is ListTypeReference)
            {
                _writer.WriteLineIndented(
                    $"if(!comparer.ListEquals(_{info.PropertyName}, otherT._{info.PropertyName}))" +
                    $" return false;");
            }
            else
            {
                _writer.WriteLineIndented(
                    $"if(!comparer.Equals(_{info.PropertyName}, otherT._{info.PropertyName}))" +
                    $" return false;");
            }
        }

        _writer.WriteLineIndented("#pragma warning restore CS8604 // Possible null reference argument.");
        _writer.WriteLine(string.Empty);

        if (exportName == "PrimitiveType")
        {
            _writer.WriteLineIndented("return Equals(JsonValue, otherT.JsonValue);");
            _writer.WriteLine(string.Empty);
        }
        else
            _writer.WriteLineIndented("return true;");

        CloseScope();
    }

    /// <summary>Writes a copy to.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="exportName">      Name of the export.</param>
    /// <param name="exportedElements">The exported elements.</param>
    private void WriteCopyTo(
        string exportName,
        List<WrittenElementInfo> exportedElements)
    {
        var specifier = exportName == "Base" ? "virtual" : "override";
        _writer.WriteLineIndented($"protected internal {specifier} void CopyToInternal(Base other)");
        OpenScope();
        _writer.WriteLineIndented($"if(other is not {exportName} dest)");
        _writer.IncreaseIndent();
        _writer.WriteLineIndented("throw new ArgumentException(\"Can only copy to an object of the same type\", \"other\");");
        _writer.DecreaseIndent();
        _writer.WriteLine();

        if (exportName == "Base")
        {
            _writer.WriteLineIndented("if (_annotations is not null)");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("dest.annotations.AddRange(annotations);");
            _writer.DecreaseIndent();
            _writer.WriteLine(string.Empty);
            _writer.WriteLineIndented("if (HasOverflow)");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("Overflow.CopyToInternal(dest.Overflow);");
            _writer.DecreaseIndent();
        }
        else
        {
            _writer.WriteLineIndented("base.CopyToInternal(dest);");
        }

        foreach (WrittenElementInfo info in exportedElements)
        {
            if (info.PropertyType is ListTypeReference)
            {
                _writer.WriteLineIndented(
                    $"if(_{info.PropertyName} is not null)" +
                        $" dest.{info.PropertyName} = new {info.PropertyType.PropertyTypeString}(_{info.PropertyName}.DeepCopyInternal());");
            }
            else
            {
                _writer.WriteLineIndented(
                    $"if(_{info.PropertyName} is not null) dest.{info.PropertyName} = " +
                       (info.PropertyType is CqlTypeReference ?
                        $"_{info.PropertyName};" :
                        $"({info.PropertyType.PropertyTypeString})_{info.PropertyName}.DeepCopyInternal();"));
            }
        }

        if (exportName == "PrimitiveType")
            _writer.WriteLineIndented("if (JsonValue != null) dest.JsonValue = JsonValue;");

        CloseScope();
    }

    /// <summary>Writes a deep copy.</summary>
    /// <param name="exportName">Name of the export.</param>
    private void WriteDeepCopy(
        string exportName)
    {
        // Base implementation differs from subclasses.
        if (exportName == "Base")
        {
            _writer.WriteLineIndented("protected internal abstract Base DeepCopyInternal();");
            _writer.WriteLine(string.Empty);
            return;
        }

        _writer.WriteLineIndented("protected internal override Base DeepCopyInternal()");
        OpenScope();
        _writer.WriteLineIndented($"var instance = new {exportName}();");
        _writer.WriteLineIndented("CopyToInternal(instance);");
        _writer.WriteLineIndented("return instance;");
        CloseScope();
    }

    /// <summary>Writes a constrained quantity.</summary>
    /// <param name="complex">   The complex data type.</param>
    /// <param name="exportName">Name of the export.</param>
    private void WriteConstrainedQuantity(
        ComponentDefinition complex,
        string exportName)
    {
        _writer.WriteLineIndented(
            $"public partial class" +
                $" {exportName}" +
                $" : Quantity");

        // open class
        OpenScope();

        if(complex.Structure.Abstract != true)
            WritePropertyTypeName(complex.Structure.Name);

        _writer.WriteLineIndented("protected internal override Base DeepCopyInternal()");
        OpenScope();
        _writer.WriteLineIndented($"var instance = new {exportName}();");
        _writer.WriteLineIndented("CopyToInternal(instance);");
        _writer.WriteLineIndented("return instance;");
        CloseScope();

        //_writer.WriteLineIndented("// TODO: Add code to enforce these constraints:");
        //WriteComponentComment(complex);
        //WriteIndentedComment(complex.Structure.Purpose, isSummary: false, singleLine: true);

        // close class
        CloseScope();
    }

    private string capitalizeThoseSillyBackboneNames(string path) =>
        path.Length == 1 ? path :
               path.StartsWith('.') ?
                char.ToUpper(path[1]) + capitalizeThoseSillyBackboneNames(path.Substring(2))
                : path[0] + capitalizeThoseSillyBackboneNames(path.Substring(1));

    /// <summary>Writes a component.</summary>
    /// <param name="complex">              The complex data type.</param>
    /// <param name="exportName">           Name of the export.</param>
    /// <param name="parentExportName">     Name of the parent export.</param>
    /// <param name="subset"></param>
    private void WriteBackboneComponent(
        ComponentDefinition complex,
        string exportName,
        string parentExportName,
        GenSubset subset)
    {
        List<WrittenElementInfo> exportedElements = [];

        WriteComponentComment(complex);

        string? explicitName = GetExplicitName(complex.Element, _info.FhirSequence);

        bool useConcatenationInName = complex.Structure.Name == "Citation";

        string explicitNamePart = explicitName ??
                                  complex.cgName(NamingConvention.PascalCase, useConcatenationInName, useConcatenationInName);
        string componentName = complex.Element.Path;

        WriteSerializable();
        _writer.WriteLineIndented($"[FhirType(\"{componentName}\", IsBackboneType=true)]");

        _writer.WriteLineIndented(
            $"public partial class" +
                $" {exportName}" +
                $" : {Namespace}.{complex.cgBaseTypeName(_info, false)}");

        // open class
        OpenScope();

        if(complex.Structure.Abstract != true)
            WritePropertyTypeName(componentName);

        WriteElements(complex, exportName, ref exportedElements, subset);

        if (exportedElements.Count > 0)
        {
            WriteCopyTo(exportName, exportedElements);
        }

        WriteDeepCopy(exportName);

        if (exportedElements.Count > 0)
        {
            WriteCompareChildren(exportName, exportedElements);
            WriteDictionarySupport(exportName, exportedElements);
        }

        // close class
        CloseScope();

        // check for nested components
        foreach (ComponentDefinition component in complex.cgChildComponents(_info))
        {
            string componentExportName;

            if (GetExplicitName(component.Element, _info.FhirSequence) is {} componentExplicitName)
            {
                componentExportName = $"{componentExplicitName}Component";
            }
            else
            {
                componentExportName =
                    $"{component.cgName(NamingConvention.PascalCase, useConcatenationInName, useConcatenationInName)}Component";
            }

            WriteBackboneComponent(
                component,
                componentExportName,
                parentExportName,
                subset);
        }
    }

    /// <summary>Writes the enums.</summary>
    /// <param name="complex">      The complex data type.</param>
    /// <param name="className">    Name of the class this enum is being written in.</param>
    /// <param name="usedEnumNames">(Optional) List of names of the used enums.</param>
    private void WriteEnums(
        ComponentDefinition complex,
        string className,
        HashSet<string>? usedEnumNames = null,
        HashSet<string>? processedValueSets = null)
    {
        usedEnumNames ??= [];

        processedValueSets ??= [];

        IEnumerable<ElementDefinition> childElements = complex.cgGetChildren();

        if (childElements.Any())
        {
            foreach (ElementDefinition element in childElements)
            {
                if ((!string.IsNullOrEmpty(element.Binding?.ValueSet)) &&
                    (element.Binding!.Strength == Hl7.Fhir.Model.BindingStrength.Required) &&
                    _info.TryExpandVs(element.Binding.ValueSet, out ValueSet? vs))
                {
                    WriteEnum(vs, className, usedEnumNames);
                    processedValueSets.Add(vs.Url);
                }
            }
        }

        foreach (ComponentDefinition component in complex.cgChildComponents(_info))
        {
            WriteEnums(component, className, usedEnumNames, processedValueSets);
        }

    }

    /// <summary>Writes a value set as an enum.</summary>
    /// <param name="vs">       The vs.</param>
    /// <param name="className">Name of the class this enum is being written in.</param>
    /// <param name="usedEnumNames"></param>
    /// <param name="silent">Do not actually write parameter to file, just add it in memory.</param>
    private bool WriteEnum(
        ValueSet vs,
        string className,
        HashSet<string> usedEnumNames,
        bool silent = false)
    {
        if (_writtenValueSets.ContainsKey(vs.Url))
        {
            return true;
        }

        if (ExclusionSet.Contains(vs.Url))
        {
            return false;
        }

        string name = (vs.Name ?? vs.Id)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal);

        string nameSanitized = FhirSanitizationUtils.SanitizeForProperty(name, _reservedWords, NamingConvention.PascalCase);

        // Enums and their containing classes cannot have the same name,
        // so we have to correct these here
        if (EnumNamesOverride.TryGetValue(vs.Url, out var replacementName))
        {
            nameSanitized = replacementName;
        }

        if (usedEnumNames.Contains(nameSanitized))
        {
            return true;
        }

        usedEnumNames.Add(nameSanitized);

        if (silent)
        {
            _writtenValueSets.Add(
                vs.Url,
                new WrittenValueSetInfo(className, nameSanitized));

            return true;
        }

        FhirConcept[] concepts = vs.cgGetFlatConcepts(_info).ToArray();

        if (concepts.Length == 0)
        {
            // TODO(ginoc): 2024.09.19 - do we want to start using a Terminology server to expand these?
            // value set that cannot be expanded and does not have an expansion provided
            return false;
        }


        IEnumerable<string> referencedCodeSystems = vs.cgReferencedCodeSystems().ToList();

        if (referencedCodeSystems.Count() == 1)
        {
            WriteIndentedComment(
                $"{vs.Description}\n" +
                $"(url: {vs.Url})\n" +
                $"(system: {referencedCodeSystems.First()})");
        }
        else
        {
            WriteIndentedComment(
                $"{vs.Description}\n" +
                $"(url: {vs.Url})\n" +
                $"(systems: {referencedCodeSystems.Count()})");
        }

        string defaultSystem = GetDefaultCodeSystem(concepts);

        _writer.WriteLineIndented($"[FhirEnumeration(\"{name}\", \"{vs.Url}\", \"{defaultSystem}\")]");

        _writer.WriteLineIndented($"public enum {nameSanitized}");

        OpenScope();

        HashSet<string> usedLiterals = [];

        foreach (FhirConcept concept in concepts)
        {
            if (concept.IsAbstract is true)
                continue;

            string codeName = ConvertEnumValue(concept.Code);
            string codeValue = FhirSanitizationUtils.SanitizeForValue(concept.Code);
            string description = string.IsNullOrEmpty(concept.Definition)
                ? $"MISSING DESCRIPTION\n(system: {concept.System})"
                : $"{FhirSanitizationUtils.SanitizeForValue(concept.Definition)}\n(system: {concept.System})";

            if (concept.HasProperty("status", "deprecated"))
            {
                description += "\nThis enum is DEPRECATED.";
            }

            WriteIndentedComment(description);

            string display = FhirSanitizationUtils.SanitizeForValue(concept.Display);

            if (concept.System != defaultSystem)
            {
                _writer.WriteLineIndented($"[EnumLiteral(\"{codeValue}\", \"{concept.System}\"), Description(\"{display}\")]");
            }
            else
            {
                _writer.WriteLineIndented($"[EnumLiteral(\"{codeValue}\"), Description(\"{display}\")]");
            }

            if (usedLiterals.Contains(codeName))
            {
                // start at 2 so that the unadorned version makes sense as v1
                for (int i = 2; i < 1000; i++)
                {
                    if (usedLiterals.Contains($"{codeName}_{i}"))
                    {
                        continue;
                    }

                    codeName = $"{codeName}_{i}";
                    break;
                }
            }

            usedLiterals.Add(codeName);

            _writer.WriteLineIndented($"{codeName},");
        }

        // HACK for short-term R6 support....
        // We need to add the FHIR version enum literals for R6, as they are not part of the
        // R5 distribution, which we are using at this moment to generate Base/Conformance.
        if (vs.Url == "http://hl7.org/fhir/ValueSet/FHIR-version")
        {
            _writer.WriteIndented(
                """
                    /// <summary>
                    /// R6 Versions.
                    /// (system: http://hl7.org/fhir/FHIR-version)
                    /// </summary>
                    [EnumLiteral("6.0"), Description("6.0")]
                    N6_0,
                    /// <summary>
                    /// R6 Final Version.
                    /// (system: http://hl7.org/fhir/FHIR-version)
                    /// </summary>
                    [EnumLiteral("6.0.0"), Description("6.0.0")]
                    N6_0_0,
                    /// <summary>
                    /// R6 1st Draft Ballot.
                    /// (system: http://hl7.org/fhir/FHIR-version)
                    /// </summary>
                    [EnumLiteral("6.0.0-ballo1"), Description("6.0.0-ballot1")]
                    N6_0_0Ballo1,
                    /// <summary>
                    /// R6 2nd Draft Ballot.
                    /// (system: http://hl7.org/fhir/FHIR-version)
                    /// </summary>
                    [EnumLiteral("6.0.0-ballot2"), Description("6.0.0-ballot2")]
                    N6_0_0Ballot2,
                    /// <summary>
                    /// R6 3rd Draft Ballot.
                    /// (system: http://hl7.org/fhir/FHIR-version)
                    /// </summary>
                    [EnumLiteral("6.0.0-ballot3"), Description("6.0.0-ballot3")]
                    N6_0_0Ballot3,
                """);
        }


        CloseScope();

        _writtenValueSets.Add(
            vs.Url,
            new WrittenValueSetInfo(className, nameSanitized));

        return true;
    }

    private static string GetDefaultCodeSystem(IEnumerable<FhirConcept> concepts)
    {
        return concepts.Select(c => c.System)
                        .GroupBy(c => c)
                        .OrderByDescending(c => c.Count())
                        .First().Key;
    }

    /// <summary>Convert enum value - see Template-Model.tt#2061.</summary>
    /// <param name="name">The name.</param>
    /// <returns>The enum converted value.</returns>
    private static string ConvertEnumValue(string name) => CSharpFirelyCommon.ConvertEnumValue(name);

    /// <summary>
    /// Determines whether this element qualifies as an identifying element.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static bool isIdentifierProperty(ElementDefinition element)
    {
        return element.Path.EndsWith(".identifier", StringComparison.Ordinal) &&
            (element.Type.Count == 1) &&
            (element.Type.First().Code == "Identifier");
    }

    /// <summary>Writes the elements.</summary>
    /// <param name="complex">              The complex data type.</param>
    /// <param name="exportedComplexName">  Name of the exported complex parent.</param>
    /// <param name="exportedElements">     [in,out] The exported elements.</param>
    /// <param name="subset"></param>
    private void WriteElements(
        ComponentDefinition complex,
        string exportedComplexName,
        ref List<WrittenElementInfo> exportedElements,
        GenSubset subset)
    {
        var elementsToGenerate = complex.cgGetChildren()
            .Where(e => !e.cgIsInherited(complex.Structure))
            .OrderBy(e => e.cgFieldOrder());

        int orderOffset = complex.Element.cgFieldOrder();

        foreach (ElementDefinition element in elementsToGenerate)
        {
            WriteElement(
                exportedComplexName,
                element,
                ref exportedElements,
                subset,
                orderOffset);
        }
    }
    private void WriteFhirElementAttribute(string name, string summary, string? isModifier, ElementDefinition element,
        string choice, string fiveWs, string? since = null, (string, string)? until = null,
        string? xmlSerialization = null, string? declaredType = null)
    {
        var xmlser = xmlSerialization is null ? null : $", XmlSerialization = XmlRepresentation.{xmlSerialization}";
        string attributeText = $"[FhirElement(\"{name}\"{xmlser}{summary}{isModifier}, Order={GetOrder(element)}{choice}{fiveWs}";
        if (since is not null)
        {
            attributeText += $", Since=FhirRelease.{since}";
        }

        if (declaredType is not null)
        {
            attributeText += $", DeclaredType={declaredType}";
        }

        attributeText += ")]";
        _writer.WriteLineIndented(attributeText);

        if (until != null)
        {
            _writer.WriteLineIndented($"[NotMapped(Since=FhirRelease.{until.Value.Item1})]");
        }
    }

    /// <summary>Writes an element.</summary>
    /// <param name="exportedComplexName">Name of the exported complex parent.</param>
    /// <param name="element">            The element.</param>
    /// <param name="exportedElements">   [in,out] The exported elements.</param>
    /// <param name="subset">             .</param>
    /// <param name="orderOffset">      The relative order.</param>
    private void WriteElement(
        string exportedComplexName,
        ElementDefinition element,
        ref List<WrittenElementInfo> exportedElements,
        GenSubset subset,
        int orderOffset)
    {
        string name = element.cgName(removeChoiceMarker: true);

        WrittenElementInfo ei = BuildElementInfo(exportedComplexName, element);
        exportedElements.Add(ei);

        BuildElementOptionalFlags(
            _info,
            element,
            subset,
            out string summary,
            out string isModifier,
            out string choice,
            out string allowedTypes,
            out string resourceReferences);

        string fiveWs = string.Empty;

        if (_exportFiveWs && (!string.IsNullOrEmpty(element.cgFiveWs())))
        {
            fiveWs = $", FiveWs=\"{element.cgFiveWs()}\"";
        }

        string path = element.cgPath();
        string? since = _sinceAttributes.GetValueOrDefault(path);
        (string, string)? until = _untilAttributes.TryGetValue(path, out (string, string) u) ? u : default((string, string)?);

        // TODO: Modify these elements in ModifyDefinitionsForConsistency
        string? remarks = path switch
        {
            "Signature.who" => "Note 1: Since R4 the type of this element should be a fixed type (ResourceReference). For backwards compatibility it remains of type DataType.\nNote 2: Since R5 the cardinality is expanded to 0..1 (previous it was 1..1).",
            "Signature.onBehalfOf" => "Since R4 the type of this element should be a fixed type (ResourceReference). For backwards compatibility it remains of type DataType.",
            "Signature.when" => "Since R5 the cardinality is expanded to 0..1 (previous it was 1..1).",
            "Signature.type" => "Since R5 the cardinality is expanded to 0..* (previous it was 1..*).",
            _ => MakeAttributeRemarkForNewOrDeprecatedProperties(name, null, since, until)
        };

        if(element.Short is not null) WriteIndentedComment(element.Short.EnsurePeriod());
        remarks = MakeAttributeRemarkForChangedTypes(path, remarks);
        if (remarks is not null) WriteIndentedComment(remarks, isSummary: false, isRemarks: true);

        string? xmlSerialization = path == "Narrative.div" ? "XHtml" :
            path is "Extension.url" or "Element.id" ? "XmlAttr" :
            ei.PropertyType is CqlTypeReference ? "XmlAttr" :
            null;

        if (path == "OperationOutcome.issue.severity")
        {
            WriteFhirElementAttribute(name, summary, ", IsModifier=true", element, choice, fiveWs);
            WriteFhirElementAttribute(name, summary, null, element, choice, fiveWs, since: "R4");
        }
        else if (path is "Signature.who" or "Signature.onBehalfOf")
        {
            WriteFhirElementAttribute(name, summary, isModifier, element, ", Choice = ChoiceType.DatatypeChoice", fiveWs);
            WriteFhirElementAttribute(name, summary, isModifier, element, "", fiveWs, since: since);
            _writer.WriteLineIndented($"[AllowedTypes(typeof(ResourceReference), Since = FhirRelease.R4)]");
        }
        else
        {
            WriteFhirElementAttribute(name, summary, isModifier, element, choice, fiveWs, since, until, xmlSerialization);
        }

        if (!string.IsNullOrEmpty(element.cgBindingName()))
        {
            _writer.WriteLineIndented($"[Binding(\"{element.cgBindingName()}\")]");
        }

        if (_elementTypeChanges.TryGetValue(path, out ElementTypeChange[]? changes))
        {
            foreach(ElementTypeChange change in changes)
            {
                _writer.WriteLineIndented(BuildAllowedTypesAttribute([change.DeclaredTypeReference], change.Since));
            }
        }

        bool notClsCompliant = !string.IsNullOrEmpty(allowedTypes) ||
            !string.IsNullOrEmpty(resourceReferences);

        if (notClsCompliant)
        {
            _writer.WriteLineIndented("[CLSCompliant(false)]");
        }

        if (!string.IsNullOrEmpty(resourceReferences))
        {
            if (path is "Signature.who" or "Signature.onBehalfOf")
            {
                _writer.WriteLineIndented($"[References(\"Practitioner\",\"RelatedPerson\",\"Patient\",\"Device\",\"Organization\")]");
                _writer.WriteLineIndented($"[References(\"Practitioner\",\"PractitionerRole\",\"RelatedPerson\",\"Patient\",\"Device\",\"Organization\", Since=FhirRelease.R4)]");

            }
            else
            {
                _writer.WriteLineIndented(resourceReferences);
            }
        }

        if (!string.IsNullOrEmpty(allowedTypes))
        {
            _writer.WriteLineIndented(allowedTypes);
        }

        if ((element.Min != 0) ||
            (element.cgCardinalityMax() != 1))
        {
            _writer.WriteLineIndented($"[Cardinality(Min={element.Min},Max={element.cgCardinalityMax()})]");
        }

        WriteElementGettersAndSetters(element, ei);
    }


    private static string? MakeAttributeRemarkForNewOrDeprecatedProperties(string name, string? baseRemark, string? since = null, (string, string)? until = null)
    {
        var deprecationRemark = (since, until, baseRemark) switch
        {
            (not null, _, _) => $"Element was introduced in {since}, do not use when working with older releases.",
            (_, (var release, ""), _) => $"Element is deprecated since {release}, do not use with {release} and newer releases.",
            (_, (var release, var replacedBy), _) => $"Element is replaced by '{replacedBy}' since {release}. Do not use this element '{name}' with {release} and newer releases.",
            _ => null
        };

        if(baseRemark is null)
        {
            return deprecationRemark;
        }

        return $"{baseRemark}. {deprecationRemark}";
    }

    private static string? MakeAttributeRemarkForChangedTypes(string path, string? baseDescription)
    {
        if(!_elementTypeChanges.TryGetValue(path, out ElementTypeChange[]? changes))
        {
            return baseDescription;
        }

        string changedDescription = $"The type of this element has changed over time. Make sure to use "
                                    + string.Join(", ",
                                        changes.Select(change => $"{change.DeclaredTypeReference.PropertyTypeString} {VersionChangeMessage(changes, change, false)}")) + ".";

        return baseDescription is null ? changedDescription : $"{baseDescription}. {changedDescription}";
    }

    private static (string? enumName, string? enumClass) GetVsInfoForCodedElement(DefinitionCollection info, ElementDefinition element, Dictionary<string, WrittenValueSetInfo> writtenValueSets)
    {
        if ((element.Binding?.Strength != Hl7.Fhir.Model.BindingStrength.Required) ||
            (!info.TryExpandVs(element.Binding.ValueSet, out ValueSet? vs)) ||
            ExclusionSet.Contains(vs.Url) ||
            (_codedElementOverrides.Contains(element.Path) && info.FhirSequence >= FhirReleases.FhirSequenceCodes.R4) ||
            !writtenValueSets.TryGetValue(vs.Url, out WrittenValueSetInfo? vsInfo))
        {
            return (null, null);
        }

        string vsClass = vsInfo.ClassName;
        string vsName = vsInfo.ValueSetName;

        if (string.IsNullOrEmpty(vsClass))
        {
            return (vsName, null);
        }

        string pascal = element.cgName().ToPascalCase();
        if (string.Equals(vsName, pascal, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Using the name '{pascal}' for the property would lead to a compiler error. " +
                $"Change the name of the valueset '{vs.Url}' by adapting the _enumNamesOverride variable in the generator and rerun.");
        }

        return (vsName, vsClass);
    }

    private static TypeReference DetermineTypeReferenceForFhirElement(
        DefinitionCollection info,
        ElementDefinition element,
        Dictionary<string, WrittenValueSetInfo> writtenValueSets)
    {
        var typeRef = determineTypeReferenceForFhirElementName();
        bool isList = element.cgCardinalityMax() != 1;

        return isList ? new ListTypeReference(typeRef) : typeRef;

        TypeReference determineTypeReferenceForFhirElementName()
        {
            if(_elementTypeChanges.TryGetValue(element.Path, out ElementTypeChange[]? changes))
            {
                // If the element has a type change, we need to use DataType, to make
                // sure the property can capture all the types.
                if(changes.All(c => c.DeclaredTypeReference is PrimitiveTypeReference))
                {
                    return PrimitiveTypeReference.PrimitiveType;
                }

                return ComplexTypeReference.DataTypeReference;
            }

            string initialTypeName = getTypeNameFromElement();

            // Elements of type Code or Code<T> have their own naming/types, so handle those separately.
            var (vsName,vsClass) = initialTypeName == "code"
                ? GetVsInfoForCodedElement(info, element, writtenValueSets)
                : (null,null);

            return TypeReference.BuildFromFhirTypeName(initialTypeName, vsName, vsClass);

            string getTypeNameFromElement()
            {
                string btn = element.cgBaseTypeName(info, true);
                if (!string.IsNullOrEmpty(btn))
                {
                    // TODO(ginoc): this should move into cgBaseTypeName();
                    // check to see if the referenced element has an explicit name
                    if (info.TryFindElementByPath(btn, out StructureDefinition? _, out ElementDefinition? targetEd))
                    {
                        return BuildTypeNameForNestedComplexType(targetEd, btn, info.FhirSequence);
                    }

                    return btn;
                }

                return element.Type.Count == 1
                    ? element.Type.First().cgName()
                    : "DataType";
            }
        }
    }

    internal static bool TryGetPrimitiveType(TypeReference tr, [NotNullWhen(true)] out PrimitiveTypeReference? ptr)
    {
        switch (tr)
        {
            case PrimitiveTypeReference p:
                ptr = p;
                return true;
            case ListTypeReference { Element: PrimitiveTypeReference pltr }:
                ptr = pltr;
                return true;
            default:
                ptr = null;
                return false;
        }
    }

    internal WrittenElementInfo BuildElementInfo(
        string exportedComplexName,
        ElementDefinition element)
    {
        return BuildElementInfo(_info, exportedComplexName, element, _writtenValueSets);
    }

    internal static WrittenElementInfo BuildElementInfo(
        DefinitionCollection info,
        string exportedComplexName,
        ElementDefinition element,
        Dictionary<string, WrittenValueSetInfo> writtenValueSets)
    {
        var typeRef = DetermineTypeReferenceForFhirElement(info, element, writtenValueSets);

        string name = element.cgName(removeChoiceMarker: true);
        string pascal =
            element.Path == "Element.id"
                ? "ElementId"
                : name.ToPascalCase();
        bool forPrimitiveType = TryGetPrimitiveType(typeRef, out _);

        return new WrittenElementInfo(
            FhirElementName: name,
            FhirElementPath: element.Path,
            PropertyName: forPrimitiveType ? $"{pascal}Element" : pascal,
            PropertyType: typeRef,
            PrimitiveHelperName: forPrimitiveType
                    ? (pascal == exportedComplexName ? $"{pascal}_" : pascal)
                    : null, // Since properties cannot have the same name as their enclosing types, we'll add a '_' suffix if this happens.
            Required: element.Min > 0
        );
    }

    private void WriteElementGettersAndSetters(ElementDefinition element, WrittenElementInfo ei)
    {
        _writer.WriteLineIndented("[DataMember]");

        string overflowTypeName = ei.PropertyType.PropertyTypeString switch
        {
            "Hl7.Fhir.Model.Resource" => "DynamicResource",
            "Hl7.Fhir.Model.DataType" => "DynamicDataType",
            "Hl7.Fhir.Model.PrimitiveType" => "DynamicPrimitive",
            { } s => s
        };

        if (ei.PropertyType is ListTypeReference)
            _writer.WriteLineIndented("[AllowNull]");
        _writer.WriteLineIndented($"public {(ei.PropertyType is ListTypeReference || ei.Required ? ei.PropertyType.PropertyTypeString : $"{ei.PropertyType.PropertyTypeString}?")} {ei.PropertyName}");

        OpenScope();

        _writer.WriteLineIndented("get");
        OpenScope();
        _writer.WriteLineIndented($"if(_{ei.PropertyName}.InOverflow<{overflowTypeName}>())");
        _writer.IncreaseIndent();
        _writer.WriteLineIndented($"throw CodedValidationException.FromTypes(typeof({ei.PropertyType.PropertyTypeString}), Overflow[\"{ei.FhirElementName}\"]);");
        _writer.DecreaseIndent();
        _writer.WriteLineIndented(ei.PropertyType is not ListTypeReference ? $"return _{ei.PropertyName}{(ei.Required ? "!" : "")};" : $"return _{ei.PropertyName} ??= [];");

        CloseScope();

        _writer.WriteLineIndented("set");
        OpenScope();
        _writer.WriteLineIndented($"if (_{ei.PropertyName}.InOverflow<{overflowTypeName}>())");
        _writer.IncreaseIndent();
        _writer.WriteLineIndented($"Overflow.Remove(\"{ei.FhirElementName}\");");
        _writer.DecreaseIndent();
        _writer.WriteLineIndented($"_{ei.PropertyName} = value;");
        _writer.WriteLineIndented($"OnPropertyChanged(\"{ei.PropertyName}\");");
        CloseScope();

        CloseScope();
        _writer.WriteLineIndented($"private {ei.PropertyType.PropertyTypeString}? _{ei.PropertyName};");
        _writer.WriteLine(string.Empty);

        bool needsHelperProperty = ei.PropertyType is
            PrimitiveTypeReference or
            ListTypeReference { Element: PrimitiveTypeReference };

        if (needsHelperProperty)
        {
            // If the property has had multiple types over time, we need to generate a helper property for each type.
            if(_elementTypeChanges.TryGetValue(element.Path, out ElementTypeChange[]? changes))
            {
                ElementTypeChange lastChange = changes.Last();

                foreach(ElementTypeChange change in changes)
                {
                    // The DeclaredType given by the maintainer is the type of the element, even if it repeats,
                    // so let's wrap that type in a list if applicable.
                    TypeReference propType = ei.PropertyType is ListTypeReference ?
                        new ListTypeReference(change.DeclaredTypeReference) : change.DeclaredTypeReference;
                    string helperName = change == lastChange
                        ? ei.PrimitiveHelperName!
                        : $"{ei.PrimitiveHelperName}{change.DeclaredTypeReference.Name.ToPascalCase()}";
                    string versionsRemark = $"Use this property {VersionChangeMessage(changes, change, false)}.";
                    WritePrimitiveHelperProperty(element.Short, ei, propType, helperName, versionsRemark);
                }
            }
            else
            {
                WritePrimitiveHelperProperty(element.Short, ei, ei.PropertyType, ei.PrimitiveHelperName!);
            }
        }
    }

    private void WritePrimitiveHelperProperty(string description, WrittenElementInfo ei,
        TypeReference? propType, string helperPropName, string? versionsRemark = null)
    {
        string descriptionText = versionsRemark is null
            ? description
            : $"{description}. {versionsRemark}";
        WriteIndentedComment(descriptionText);
        _writer.WriteLineIndented("/// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>");

        _writer.WriteLineIndented("[IgnoreDataMember]");

        switch (propType)
        {
            case PrimitiveTypeReference ptr:
                string typeString = WithNullabilityMarking(ptr.ConveniencePropertyTypeString);
                _writer.WriteLineIndented($"public {typeString} {helperPropName}");

                OpenScope();
                string propAccess = versionsRemark is not null
                    ? $"(({MostGeneralValueAccessorType(ptr)}?){ei.PropertyName})"
                    : $"{ei.PropertyName}";
                _writer.WriteLineIndented($"get => {propAccess}?.Value;");

                _writer.WriteLineIndented("set");
                OpenScope();
                _writer.WriteLineIndented($"{ei.PropertyName} = value is null ? null! : new {ptr.PropertyTypeString}(value);");
                _writer.WriteLineIndented($"OnPropertyChanged(\"{helperPropName}\");");
                CloseScope(suppressNewline: true);
                CloseScope();
                break;
            case ListTypeReference { Element: PrimitiveTypeReference lptr }:
                string nullableTypeList = WithNullabilityMarking(lptr.ConveniencePropertyTypeString);
                _writer.WriteLineIndented($"public IEnumerable<{nullableTypeList}> {helperPropName}");

                OpenScope();

                _writer.WriteIndented($"get => _{ei.PropertyName}");
                if(versionsRemark is not null)
                    _writer.Write($"?.Cast<{MostGeneralValueAccessorType(lptr)}>()");
                _writer.WriteLine($"?.Select(elem => elem.Value) ?? [];");

                _writer.WriteLineIndented("set");
                OpenScope();

                _writer.WriteLineIndented($"if (value == null)");

                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{ei.PropertyName} = null!;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{ei.PropertyName} = " +
                                          $"new {ei.PropertyType.PropertyTypeString}" +
                                          $"(value.Select(elem=>new {lptr.PropertyTypeString}(elem)));");
                _writer.DecreaseIndent();

                _writer.WriteLineIndented($"OnPropertyChanged(\"{helperPropName}\");");
                CloseScope(suppressNewline: true);
                CloseScope();
                break;
        }
    }

    private static string MostGeneralValueAccessorType(PrimitiveTypeReference ptr)
    {
        return ptr.ConveniencePropertyTypeString switch
        {
            "string" => "IValue<string>",
            _ => ptr.PropertyTypeString
        };
    }


    /// <summary>
    /// Determine the type name for an element that has child elements, based on the definition and
    /// the declared type.
    /// </summary>
    /// <param name="ed">  The ed.</param>
    /// <param name="type">The type.</param>
    /// <returns>A string.</returns>
    private static string BuildTypeNameForNestedComplexType(ElementDefinition ed, string type, FhirReleases.FhirSequenceCodes sequence)
    {
        if (GetExplicitName(ed, sequence) is {} explicitTypeName)
        {
            string parentName = type.Substring(0, type.IndexOf('.'));
            return $"{parentName}" +
                $".{explicitTypeName}" +
                $"Component";
        }

        // check for *possibly* already processed
        if (type.EndsWith("Component", StringComparison.Ordinal))
        {
            // if the path does not end in component, we are good
            if (!ed.Path.EndsWith("Component", StringComparison.Ordinal))
            {
                return type;
            }

            // check for already appending a 'Component' literal
            if (type.EndsWith("ComponentComponent", StringComparison.Ordinal))
            {
                return type;
            }

            // fall through to continue processing
        }

        string[] components = type.Split('.');

        // citation needs special handling
        if ((components.Length > 2) && ed.Path.StartsWith("Citation.", StringComparison.Ordinal))
        {
            return string.Join(".", components[0], string.Join(string.Empty, components.Skip(1).ToPascalCase())) + "Component";
        }

        if (components.Length > 1)
        {
            return string.Join(".", components[0], components[^1].ToPascalCase()) + "Component";
        }

        return type;
    }

    /// <summary>Builds element optional flags.</summary>
    /// <param name="info">              The definition information.</param>
    /// <param name="element">           The element.</param>
    /// <param name="subset">            .</param>
    /// <param name="summary">           [out] The summary.</param>
    /// <param name="isModifier">        [out].</param>
    /// <param name="choice">            [out] The choice.</param>
    /// <param name="allowedTypes">      [out] List of types of the allowed.</param>
    /// <param name="resourceReferences">[out] The resource references.</param>
    internal static void BuildElementOptionalFlags(
        DefinitionCollection info,
        ElementDefinition element,
        GenSubset subset,
        out string summary,
        out string isModifier,
        out string choice,
        out string allowedTypes,
        out string resourceReferences)
    {
        choice = string.Empty;
        allowedTypes = string.Empty;
        resourceReferences = string.Empty;

        // We think it's a bug that extension's members are not marked as "in summary", so correct that here.
        bool inSummary = element.IsSummary == true ||
                         element.Path == "Extension.url" || element.Path == "Extension.value[x]";

        summary = inSummary ? ", InSummary=true" : string.Empty;
        isModifier = element.IsModifier == true ? ", IsModifier=true" : string.Empty;

        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> elementTypes = element.cgTypes();

        if (elementTypes.Any())
        {
            if (elementTypes.Count == 1)
            {
                string elementType = elementTypes.First().Key;

                if (elementType == "Resource")
                {
                    choice = ", Choice=ChoiceType.ResourceChoice";
                }
            }
            else
            {
                string firstType = elementTypes.First().Key;

                if (info.PrimitiveTypesByName.ContainsKey(firstType) ||
                    info.ComplexTypesByName.ContainsKey(firstType))
                {
                    choice = ", Choice=ChoiceType.DatatypeChoice";
                }

                if (info.ResourcesByName.ContainsKey(firstType))
                {
                    choice = ", Choice=ChoiceType.ResourceChoice";
                }

                // When we generating classes in the base subset, we have to avoid generating an
                // [AllowedTypes] attribute that contains class names that are not
                // present in the current version of the standard. So, in principle, we don't generate
                // this attribute in the base subset, unless all types mentioned are present in the
                // exception list above.
                static bool isPrimitive(string name) => char.IsLower(name[0]);
                bool allTypesAvailable =
                    elementTypes.Keys.All(en =>
                        isPrimitive(en) // primitives are available everywhere
                        || _baseSubsetComplexTypes.Contains(en) // base subset types are all available everywhere
                        || (subset.HasFlag(GenSubset.Conformance) && _conformanceSubsetComplexTypes.Contains(en)) // otherwise, conformance types are available in conformance
                        || subset.HasFlag(GenSubset.Satellite)  // main has access to all types
                        );

                if (allTypesAvailable)
                {
                    IEnumerable<TypeReference> typeRefs = elementTypes.Values.Select(v => TypeReference.BuildFromFhirTypeName(v.Code));
                    allowedTypes = BuildAllowedTypesAttribute(typeRefs, null);
                }
                else if (elementTypes.Count > 30)
                {
                    allowedTypes = BuildOpenAllowedTypesAttribute();
                }
                else
                    throw new InvalidOperationException("Cannot generate AllowedTypes attribute for element " +
                        $"{element.Path} with types {string.Join(", ", elementTypes.Keys)} because " +
                        $"not all types in the choice are available in the current subset ({subset}).");
            }
        }

        if (elementTypes.Any())
        {
            foreach ((string _, ElementDefinition.TypeRefComponent elementType) in elementTypes.Where(kvp => (kvp.Key == "Reference") && kvp.Value.TargetProfile.Any()))
            {
                resourceReferences = "[References(" +
                    string.Join(",", elementType.cgTargetProfiles().Keys.Select(name => "\"" + name + "\"")) +
                    ")]";
                break;
            }
        }
    }

    /// <summary>Writes a property type name.</summary>
    /// <param name="name">      The name.</param>
    private void WritePropertyTypeName(string name)
    {
        WriteIndentedComment("FHIR Type Name");

        _writer.WriteLineIndented($"""public override string TypeName => "{name}";""");

        _writer.WriteLine(string.Empty);
    }

    /// <summary>Writes a primitive types.</summary>
    /// <param name="primitives">   The primitives.</param>
    /// <param name="writtenModels">[in,out] The written models.</param>
    /// <param name="subset"></param>
    private void WritePrimitiveTypes(
        IEnumerable<StructureDefinition> primitives,
        ref Dictionary<string, WrittenModelInfo> writtenModels,
        GenSubset subset)
    {
        // The FHIR primitives are all part of the base subset.
        if (subset is not GenSubset.Base) return;

        foreach (StructureDefinition primitive in primitives.OrderBy(sd => sd.Name))
        {
            if (ExclusionSet.Contains(primitive.Name))
            {
                continue;
            }

            WritePrimitiveType(primitive, ref writtenModels);
        }
    }

    /// <summary>Writes a primitive type.</summary>
    /// <param name="primitive">    The primitive.</param>
    /// <param name="writtenModels">[in,out] The written models.</param>
    private void WritePrimitiveType(
        StructureDefinition primitive,
        ref Dictionary<string, WrittenModelInfo> writtenModels)
    {
        string exportName;
        string typeName;

        if (TypeNameMappings.TryGetValue(primitive.Name, out string? tmValue))
        {
            exportName = tmValue;
        }
        else
        {
            exportName = primitive.Name.ToPascalCase();
        }

        if (PrimitiveTypeMap.TryGetValue(primitive.Name, out string? ptmValue))
        {
            typeName = ptmValue;
        }
        else
        {
            typeName = primitive.cgpBaseTypeName();
        }

        writtenModels.Add(
            primitive.Name,
            new WrittenModelInfo(FhirName: primitive.Name, CsName: $"{Namespace}.{exportName}", IsAbstract: false));

        string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

        _modelWriter.WriteLineIndented($"// {exportName}.cs");

        using Stream stream = _generateHashesInsteadOfOutput
            ? new MemoryStream()
            : new FileStream(filename, FileMode.Create);
        using ExportStreamWriter writer = new(stream);

        if (_generateHashesInsteadOfOutput)
        {
            writer.NewLine = "\r\n";
        }

        _writer = writer;

        WriteHeaderPrimitive();

        WriteNamespaceOpen();

        if (!string.IsNullOrEmpty(primitive.cgpDefinition()))
        {
            WriteIndentedComment($"Primitive Type {primitive.Name}\n{primitive.cgpDefinition()}");
        }
        else
        {
            WriteIndentedComment($"Primitive Type {primitive.Name}");
        }

        if (!string.IsNullOrEmpty(primitive.cgpComment()))
        {
            WriteIndentedComment(primitive.cgpComment(), isSummary: false, isRemarks: true);
        }

        _writer.WriteLineIndented("[System.Diagnostics.DebuggerDisplay(@\"\\{Value={Value}}\")]");
        WriteSerializable();

        string fhirTypeConstructor = $"\"{primitive.Name}\",\"{primitive.Url}\"";
        _writer.WriteLineIndented($"[FhirType({fhirTypeConstructor})]");

        _writer.WriteLineIndented(
            $"public partial class" +
            $" {exportName}" +
            $" : PrimitiveType, " +
            PrimitiveValueInterface(typeName));

        // open class
        OpenScope();

        if (primitive.Abstract != true)
            WritePropertyTypeName(primitive.Name);

        if (!string.IsNullOrEmpty(primitive.cgpValidationRegEx()))
        {
            WriteIndentedComment(
                $"Must conform to the pattern \"{primitive.cgpValidationRegEx()}\"",
                false);

            _writer.WriteLineIndented($"public const string PATTERN = @\"{primitive.cgpValidationRegEx()}\";");
            _writer.WriteLine(string.Empty);
        }

        var nullableTypeName = typeName.EndsWith('?') ? typeName : typeName + '?';

        _writer.WriteLineIndented($"public {exportName}({nullableTypeName} value)");
        OpenScope();
        _writer.WriteLineIndented("Value = value;");
        CloseScope();

        _writer.WriteLineIndented($"public {exportName}(): this(({nullableTypeName})null) {{}}");
        _writer.WriteLine(string.Empty);

        // For some primitive pocos, we need a hand-written value propery since we are using
        // a different value type for JsonValue and Value.
        if (exportName is not ("Integer64" or "Base64Binary" or "Instant"))
        {
            WriteIndentedComment("Primitive value of the element");

            _writer.WriteLineIndented(
                "[FhirElement(\"value\", IsPrimitiveValue=true, XmlSerialization=XmlRepresentation.XmlAttr, InSummary=true, Order=30)]");

            _writer.WriteLineIndented("[DataMember]");

            _writer.WriteLineIndented($"public {nullableTypeName} Value");
            OpenScope();

            var typeNameInSwitch = typeName.EndsWith("?") ? typeName[..^1] : typeName;
            _writer.WriteLineIndented($"get {{ return JsonValue is {typeNameInSwitch} or null ? ({nullableTypeName})JsonValue : throw COVE.FromTypes(typeof({exportName}), JsonValue); }}");
            _writer.WriteLineIndented("set { JsonValue = value; OnPropertyChanged(\"Value\"); }");
            CloseScope();
        }

        WriteDeepCopy(exportName);

        // close class
        CloseScope();

        WriteNamespaceClose();

        WriteFooter();

        if (_generateHashesInsteadOfOutput)
        {
            writer.Flush();

            // generate the hash
            string hash = FileSystemUtils.GenerateSha256(stream);
            _fileHashes.Add(filename[_exportDirectory.Length..], hash);

            writer.Close();
        }
        else
        {
            writer.Flush();
            writer.Close();
        }
    }

    private string getSystemTypeForFhirType(string fhirType)
    {
        var systemTypeName = fhirType switch
        {
            "boolean" => "Boolean",
            "integer" => "Integer",
            "unsignedInt" => "Integer",
            "positiveInt" => "Integer",
            "integer64" => "Long",
            "time" => "Time",
            "date" => "Date",
            "instant" => "DateTime",
            "dateTime" => "DateTime",
            "decimal" => "Decimal",
            _ => "String"
        };

        return "SystemPrimitive." + systemTypeName;
    }

    private void WriteSerializable()
    {
        _writer.WriteLineIndented("[Serializable]");
        _writer.WriteLineIndented("[DataContract]");
    }

    private static string PrimitiveValueInterface(string valueType)
    {
        if (valueType.EndsWith('?'))
        {
            string nullableType = valueType.TrimEnd('?');
            return $"INullableValue<{nullableType}>";
        }
        else
        {
            return $"IValue<{valueType}>";
        }
    }

    /// <summary>Writes the namespace open.</summary>
    private void WriteNamespaceOpen()
    {
        _writer.WriteLineIndented($"namespace {Namespace}");
        OpenScope();
    }

    /// <summary>Writes the namespace close.</summary>
    private void WriteNamespaceClose()
    {
        CloseScope();
    }

    /// <summary>Writes a header.</summary>
    private void WriteHeaderBasic()
    {
        WriteGenerationComment();

        _writer.WriteLineIndented("using Hl7.Fhir.Utility;");
        _writer.WriteLine(string.Empty);

        WriteCopyright();
    }

    /// <summary>Writes a header.</summary>
    private void WriteHeaderComplexDataType()
    {
        WriteGenerationComment();

        _writer.WriteLineIndented("using System;");
        _writer.WriteLineIndented("using System.Collections;");
        _writer.WriteLineIndented("using System.Collections.Generic;");
        _writer.WriteLineIndented("using System.Linq;");
        _writer.WriteLineIndented("using System.Runtime.Serialization;");
        _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
        _writer.WriteLineIndented("using Hl7.Fhir.Serialization;");
        _writer.WriteLineIndented("using Hl7.Fhir.Specification;");
        _writer.WriteLineIndented("using Hl7.Fhir.Utility;");
        _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
        _writer.WriteLineIndented("using System.Diagnostics.CodeAnalysis;");
        _writer.WriteLineIndented("using SystemPrimitive = Hl7.Fhir.ElementModel.Types;");
        _writer.WriteLine();

        _writer.WriteLineIndented("#nullable enable");
        _writer.WriteLine();

        WriteCopyright();

#if DISABLED    // 2020.07.01 - should be exporting everything with necessary summary tags
            _writer.WriteLineI("#pragma warning disable 1591 // suppress XML summary warnings ");
            _writer.WriteLine(string.Empty);
#endif
    }

    /// <summary>Writes a header above a primitive class.</summary>
    private void WriteHeaderPrimitive()
    {
        WriteGenerationComment();

        _writer.WriteLineIndented("using System;");
        _writer.WriteLineIndented("using System.Runtime.Serialization;");
        _writer.WriteLineIndented("using System.Text.RegularExpressions;");
        _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
        _writer.WriteLineIndented("using Hl7.Fhir.Specification;");
        _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
        _writer.WriteLineIndented("using System.Diagnostics.CodeAnalysis;");
        _writer.WriteLineIndented("using SystemPrimitive = Hl7.Fhir.ElementModel.Types;");
        _writer.WriteLineIndented("using COVE=Hl7.Fhir.Validation.CodedValidationException;");
        _writer.WriteLine(string.Empty);

        _writer.WriteLineIndented("#nullable enable");
        _writer.WriteLine();

        WriteCopyright();
    }

    /// <summary>Writes the generation comment.</summary>
    /// <param name="writer">(Optional) The currently in-use text writer.</param>
    private void WriteGenerationComment(ExportStreamWriter? writer = null)
    {
        writer ??= _writer;

        writer.WriteLineIndented("// <auto-generated/>");
        writer.WriteLineIndented($"// Contents of: {string.Join(", ", _info.Manifests.Select(kvp => kvp.Key))}");
        //writer.WriteLineIndented($"// Contents of: {_info.PackageName} version: {_info.VersionString}");

        if (_options.ExportKeys.Count != 0)
        {
            string restrictions = string.Join("|", _options.ExportKeys);
            _writer.WriteLine($"  // Restricted to: {restrictions}");
        }

        writer.WriteLine(string.Empty);
    }

    /// <summary>Writes the copyright.</summary>
    private void WriteCopyright()
    {
        _writer.WriteLineIndented("/*");
        _writer.WriteLineIndented("  Copyright (c) 2011+, HL7, Inc.");
        _writer.WriteLineIndented("  All rights reserved.");
        _writer.WriteLineIndented("  ");
        _writer.WriteLineIndented("  Redistribution and use in source and binary forms, with or without modification, ");
        _writer.WriteLineIndented("  are permitted provided that the following conditions are met:");
        _writer.WriteLineIndented("  ");
        _writer.WriteLineIndented("   * Redistributions of source code must retain the above copyright notice, this ");
        _writer.WriteLineIndented("     list of conditions and the following disclaimer.");
        _writer.WriteLineIndented("   * Redistributions in binary form must reproduce the above copyright notice, ");
        _writer.WriteLineIndented("     this list of conditions and the following disclaimer in the documentation ");
        _writer.WriteLineIndented("     and/or other materials provided with the distribution.");
        _writer.WriteLineIndented("   * Neither the name of HL7 nor the names of its contributors may be used to ");
        _writer.WriteLineIndented("     endorse or promote products derived from this software without specific ");
        _writer.WriteLineIndented("     prior written permission.");
        _writer.WriteLineIndented("  ");
        _writer.WriteLineIndented("  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND ");
        _writer.WriteLineIndented("  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED ");
        _writer.WriteLineIndented("  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. ");
        _writer.WriteLineIndented("  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, ");
        _writer.WriteLineIndented("  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT ");
        _writer.WriteLineIndented("  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR ");
        _writer.WriteLineIndented("  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, ");
        _writer.WriteLineIndented("  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ");
        _writer.WriteLineIndented("  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE ");
        _writer.WriteLineIndented("  POSSIBILITY OF SUCH DAMAGE.");
        _writer.WriteLineIndented("  ");
        _writer.WriteLineIndented("*/");
        _writer.WriteLine(string.Empty);
    }

    /// <summary>Writes a footer.</summary>
    private void WriteFooter()
    {
        WriteIndentedComment("end of file", singleLine: true);
    }

    /// <summary>Opens the scope.</summary>
    private void OpenScope()
        => CSharpFirelyCommon.OpenScope(_writer);

    /// <summary>Closes the scope.</summary>
    private void CloseScope(bool includeSemicolon = false, bool suppressNewline = false)
        => CSharpFirelyCommon.CloseScope(_writer, includeSemicolon, suppressNewline);

    /// <summary>Writes an indented comment.</summary>
    /// <param name="value">    The value.</param>
    /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
    /// <param name="singleLine"></param>
    private void WriteIndentedComment(string value, bool isSummary = true, bool singleLine = false, bool isRemarks = false)
        => _writer.WriteIndentedComment(value.TrimEnd(), isSummary, singleLine, isRemarks);

    /// <summary>Adds a set of FhirTypes to a total set of exportable WrittenModelInfos.</summary>
    private static void AddModels(
       Dictionary<string, WrittenModelInfo> total,
       IEnumerable<WrittenModelInfo> typesToAdd)
    {
        foreach (WrittenModelInfo type in typesToAdd)
        {
            if (total.ContainsKey(type.FhirName))
            {
                continue;
            }

            total.Add(type.FhirName, type);
        }
    }

    private static void AddModels(
        Dictionary<string, WrittenModelInfo> total,
        IEnumerable<StructureDefinition> typesToAdd)
    {
        AddModels(total, typesToAdd.OrderBy(sd => sd.Name).Select(ta => CreateWMI(ta)));

        WrittenModelInfo CreateWMI(StructureDefinition t)
        {
            string exportName;

            if (TypeNameMappings.TryGetValue(t.Name, out string? tmValue))
            {
                exportName = tmValue;
            }
            else
            {
                exportName = t.Name.ToPascalCase();
            }

            return new WrittenModelInfo(FhirName: t.Name, CsName: $"{Namespace}.{exportName}", IsAbstract: t.Abstract == true);
        }
    }

    /// <summary>Information about a written value set.</summary>
    internal record WrittenValueSetInfo(string ClassName, string ValueSetName);

    /// <summary>Information about the written element.</summary>
    internal record WrittenElementInfo(
        string FhirElementName,
        string FhirElementPath,
        string PropertyName,
        TypeReference PropertyType,
        string? PrimitiveHelperName,
        bool Required = false);

    /// <summary>Information about the written model.</summary>
    internal record WrittenModelInfo(string FhirName, string CsName, bool IsAbstract);
}
