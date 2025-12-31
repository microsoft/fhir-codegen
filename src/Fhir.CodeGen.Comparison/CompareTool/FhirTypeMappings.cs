// <copyright file="PrimitiveMappings.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using System.Text;
using Fhir.CodeGen.Common.Packaging;
using Hl7.Fhir.Model;
using Octokit;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

public class FhirTypeMappings
{
    public static readonly List<string> CanonicalTargets = [
        "ActivityDefinition",
        "ActorDefinition",
        "CapabilityStatement",
        "ChargeItemDefinition",
        "Citation",
        "CodeSystem",
        "CompartmentDefinition",
        "ConceptMap",
        "ConditionDefinition",
        "EffectEvidenceSynthesis",
        "EventDefinition",
        "Evidence",
        "EvidenceReport",
        "EvidenceVariable",
        "ExampleScenario",
        "GraphDefinition",
        "ImplementationGuide",
        "Library",
        "Measure",
        "MessageDefinition",
        "NamingSystem",
        "OperationDefinition",
        "PlanDefinition",
        "Questionnaire",
        "Requirements",
        "ResearchDefinition",
        "ResearchElementDefinition",
        "RiskEvidenceSynthesis",
        "SearchParameter",
        "SpecimenDefinition",
        "StructureDefinition",
        "StructureMap",
        "SubscriptionTopic",
        "TerminologyCapabilities",
        "TestPlan",
        "TestScript",
        "ValueSet",
        ];


    /// <summary>
    /// Represents a mapping between source and target types for code generation.
    /// </summary>
    /// <param name="SourceType">The source type in the mapping.</param>
    /// <param name="TargetType">The target type in the mapping.</param>
    /// <param name="ConceptDomainRelationship">The relationship between the concept domains of the source and target types.</param>
    /// <param name="ValueDomainRelationship">The relationship between the value domains of the source and target types.</param>
    public readonly record struct CodeGenTypeMapping(
        string SourceType,
        string TargetType,
        CMR ConceptDomainRelationship,
        CMR ValueDomainRelationship,
        CMR Relationship)
    {
        /// <summary>
        /// Gets a comment describing the relationship between the concept domains of the source and target types.
        /// </summary>
        public string ConceptDomainComment => ConceptDomainRelationship switch
        {
            CMR.Equivalent => (SourceType == TargetType)
                ? "The type is the same"
                : $"{SourceType} and {TargetType} are conceptually interchangeable if deemed appropriate by the context of use",
            CMR.SourceIsNarrowerThanTarget => $"`{SourceType}` is considered a narrower concept domain than `{TargetType}`",
            CMR.SourceIsBroaderThanTarget => $"`{SourceType}` is considered a broader concept domain than `{TargetType}`",
            _ => $"Conceptually, `{SourceType}` and `{TargetType}` should be treated as `{ConceptDomainRelationship}`"
        };

        /// <summary>
        /// Gets a comment describing the relationship between the value domains of the source and target types.
        /// </summary>
        public string ValueDomainComment => ValueDomainRelationship switch
        {
            CMR.Equivalent => (SourceType == TargetType)
                ? "The type is the same"
                : $"`{SourceType}` and `{TargetType}` cover the same value domain",
            CMR.SourceIsNarrowerThanTarget => $"`{SourceType}` covers a smaller value domain than `{TargetType}`",
            CMR.SourceIsBroaderThanTarget => $"`{SourceType} covers a larger value domain than `{TargetType}`",
            _ => $"In value domain, `{SourceType}` and `{TargetType}` should be treated as `{ConceptDomainRelationship}`"
        };

        public string Comment => (SourceType == TargetType)
            ? "The types are the same"
            : $"Concept domain: {ConceptDomainComment}. Value domain: {ValueDomainComment}";

        //public CMR Relationship => ConceptDomainRelationship switch
        //{
        //    CMR.Equivalent => ValueDomainRelationship,
        //    _ => ConceptDomainRelationship
        //};
    }

    /// <summary>
    /// An array of mappings between primitive FHIR types.
    /// Each mapping defines the relationship between a source type and a target type.
    /// </summary>
    /// <remarks>
    /// The mappings are used to determine how types are converted or compared during code generation.
    /// </remarks>
    internal static readonly CodeGenTypeMapping[] PrimitiveMappings = [
        new("base64Binary", "base64Binary", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("boolean", "boolean", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("canonical", "canonical", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("canonical", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("canonical", "uri", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("code", "code", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("code", "id", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("code", "string", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("code", "oid", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("code", "uri", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),

        new("date", "date", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("date", "dateTime", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsBroaderThanTarget),

        new("dateTime", "dateTime", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("dateTime", "date", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),

        new("decimal", "decimal", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("decimal", "unsignedInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),

        new("id", "id", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("id", "code", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("id", "oid", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("id", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),

        new("instant", "instant", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("integer", "integer", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("integer", "integer64", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("integer", "positiveInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("integer", "unsignedInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),

        new("integer64", "integer64", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("integer64", "integer", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("integer64", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),

        new("markdown", "markdown", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("markdown", "string", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("oid", "oid", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("oid", "code", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("oid", "id", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("oid", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),

        new("positiveInt", "positiveInt", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("positiveInt", "integer", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("positiveInt", "unsignedInt", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),

        new("string", "string", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("string", "canonical", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("string", "code", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("string", "id", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("string", "integer64", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("string", "markdown", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("string", "oid", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("string", "uri", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),

        new("time", "time", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("unsignedInt", "unsignedInt", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("unsignedInt", "decimal", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("unsignedInt", "integer", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("unsignedInt", "positiveInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),

        new("uri", "uri", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("uri", "canonical", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("uri", "code", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("uri", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("uri", "url", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("url", "url", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("url", "uri", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("uuid", "uuid", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),

        new("xhtml", "xhtml", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
    ];

    internal static Dictionary<string, string> PrimitiveTypeFallbacks = new()
    {
        { "canonical", "uri" },
        { "code", "string" },
        { "integer64", "string" },
        { "markdown", "string" },
        { "oid", "uri" },
        { "positiveInt", "integer" },
        { "unsignedInt", "integer" },
        { "url", "uri" },
        { "uuid", "uri" },
        { "xhtml", "uri" },
    };

    internal static ILookup<(string, string), CodeGenTypeMapping> PrimitiveLookupByPair =
        PrimitiveMappings.ToLookup(m => (m.SourceType, m.TargetType), m => m);

    internal static readonly Dictionary<string, CodeGenTypeMapping> CompositeMappingOverrides = new()
    {
        { "R2-Quantity-R3-Age", new("Quantity", "Age", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget) },
        { "R2-Quantity-R3-Count", new("Quantity", "Count", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget) },
        { "R2-Quantity-R3-Distance", new("Quantity", "Distance", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget) },
        { "R2-Quantity-R3-Duration", new("Quantity", "Duration", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget) },
        { "R2-Quantity-R3-Money", new("Quantity", "Money", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget) },
        { "R2-Quantity-R3-SimpleQuantity", new("Quantity", "SimpleQuantity", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget) },

        { "R3-Age-R2-Quantity", new("Age", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget) },
        { "R3-Count-R2-Quantity", new("Count", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget) },
        { "R3-Distance-R2-Quantity", new("Distance", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget) },
        { "R3-Duration-R2-Quantity", new("Duration", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget) },
        { "R3-Money-R2-Quantity", new("Money", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget) },
        { "R3-SimpleQuantity-R2-Quantity", new("SimpleQuantity", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget) },
    };

    static internal bool TryGetMapping(
        string sourceType,
        string targetType,
        [NotNullWhen(true)] out CodeGenTypeMapping? mapping)
    {
        foreach (CodeGenTypeMapping m in PrimitiveMappings)
        {
            if (m.SourceType == sourceType && m.TargetType == targetType)
            {
                mapping = m;
                return true;
            }
        }
        mapping = default;
        return false;
    }

    internal static List<CodeGenTypeMapping> GetComplexTypeMaps(
        FhirReleases.FhirSequenceCodes left,
        FhirReleases.FhirSequenceCodes right)
    {
        switch (left, right)
        {
            case (FhirReleases.FhirSequenceCodes.DSTU2, FhirReleases.FhirSequenceCodes.STU3):
                return InitialComplexTypeMaps_R2_R3;
            case (FhirReleases.FhirSequenceCodes.STU3, FhirReleases.FhirSequenceCodes.DSTU2):
                return InitialComplexTypeMaps_R3_R2;
            case (FhirReleases.FhirSequenceCodes.STU3, FhirReleases.FhirSequenceCodes.R4):
                return InitialComplexTypeMaps_R3_R4;
            case (FhirReleases.FhirSequenceCodes.R4, FhirReleases.FhirSequenceCodes.STU3):
                return InitialComplexTypeMaps_R4_R3;
            case (FhirReleases.FhirSequenceCodes.R4, FhirReleases.FhirSequenceCodes.R4B):
                return InitialComplexTypeMaps_R4_R4B;
            case (FhirReleases.FhirSequenceCodes.R4, FhirReleases.FhirSequenceCodes.R5):
                return InitialComplexTypeMaps_R4B_R5;
            case (FhirReleases.FhirSequenceCodes.R4B, FhirReleases.FhirSequenceCodes.R4):
                return InitialComplexTypeMaps_R4B_R4;
            case (FhirReleases.FhirSequenceCodes.R4B, FhirReleases.FhirSequenceCodes.R5):
                return InitialComplexTypeMaps_R4B_R5;
            case (FhirReleases.FhirSequenceCodes.R5, FhirReleases.FhirSequenceCodes.R4B):
                return InitialComplexTypeMaps_R5_R4B;
            case (FhirReleases.FhirSequenceCodes.R5, FhirReleases.FhirSequenceCodes.R4):
                return InitialComplexTypeMaps_R5_R4B;
            default:
                return [];
        }
    }

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R2_R3 => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Age", CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget),
        new("Quantity", "Count", CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget),
        new("Quantity", "Distance", CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget),
        new("Quantity", "Money", CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget),
        new("Quantity", "Duration", CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget , CMR.SourceIsBroaderThanTarget),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Signature", "Signature", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Timing", "Timing", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        ];

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R3_R2 => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Distance", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Duration", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Money", "Quantity", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Signature", "Signature", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Timing", "Timing", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        ];

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R3_R4 => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Age", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactDetail", "ContactDetail", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Contributor", "Contributor", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Count", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("DataRequirement", "DataRequirement", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Distance", "Distance", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Dosage", "Dosage", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Duration", "Duration", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Money", "Money", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ParameterDefinition", "ParameterDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RelatedArtifact", "RelatedArtifact", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Signature", "Signature", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Timing", "Timing", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("TriggerDefinition", "TriggerDefinition", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("UsageContext", "UsageContext", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        ];

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R4_R3 => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Age", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactDetail", "ContactDetail", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Contributor", "Contributor", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Count", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("DataRequirement", "DataRequirement", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Distance", "Distance", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Dosage", "Dosage", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Duration", "Duration", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Money", "Money", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ParameterDefinition", "ParameterDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RelatedArtifact", "RelatedArtifact", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Signature", "Signature", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Timing", "Timing", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("TriggerDefinition", "TriggerDefinition", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("UsageContext", "UsageContext", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        ];

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R4_R4B => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Age", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableReference", CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactDetail", "ContactDetail", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Contributor", "Contributor", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Count", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("DataRequirement", "DataRequirement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Distance", "Distance", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Dosage", "Dosage", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Duration", "Duration", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Expression", "Expression", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("MarketingStatus", "MarketingStatus", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Money", "Money", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ParameterDefinition", "ParameterDefinition", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Population", "Population", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ProdCharacteristic", "ProdCharacteristic", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ProductShelfLife", "ProductShelfLife", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RelatedArtifact", "RelatedArtifact", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Signature", "Signature", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Timing", "Timing", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("TriggerDefinition", "TriggerDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("UsageContext", "UsageContext", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        ];

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R4B_R4 => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Age", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableReference", "CodeableConcept", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("CodeableReference", "Reference", CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactDetail", "ContactDetail", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Contributor", "Contributor", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Count", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("DataRequirement", "DataRequirement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Distance", "Distance", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Dosage", "Dosage", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Duration", "Duration", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Expression", "Expression", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("MarketingStatus", "MarketingStatus", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Money", "Money", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ParameterDefinition", "ParameterDefinition", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Population", "Population", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ProdCharacteristic", "ProdCharacteristic", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ProductShelfLife", "ProductShelfLife", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RelatedArtifact", "RelatedArtifact", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Signature", "Signature", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Timing", "Timing", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("TriggerDefinition", "TriggerDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("UsageContext", "UsageContext", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        ];


    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R4B_R5 => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Age", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableReference", "CodeableReference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactDetail", "ContactDetail", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Contributor", "Contributor", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Count", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("DataRequirement", "DataRequirement", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("DataType", "DataType", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Distance", "Distance", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Dosage", "Dosage", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Duration", "Duration", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Expression", "Expression", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("MarketingStatus", "MarketingStatus", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Money", "Money", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ParameterDefinition", "ParameterDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ProductShelfLife", "ProductShelfLife", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RatioRange", "RatioRange", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RelatedArtifact", "RelatedArtifact", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Signature", "Signature", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Timing", "Timing", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("TriggerDefinition", "TriggerDefinition", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("UsageContext", "UsageContext", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        ];

    private static List<CodeGenTypeMapping> InitialComplexTypeMaps_R5_R4B => [
        new("Address", "Address", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Age", "Age", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Annotation", "Annotation", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Attachment", "Attachment", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("BackboneElement", "BackboneElement", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableConcept", "CodeableConcept", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("CodeableReference", "CodeableReference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Coding", "Coding", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactDetail", "ContactDetail", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ContactPoint", "ContactPoint", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Contributor", "Contributor", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Count", "Count", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("DataRequirement", "DataRequirement", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("DataType", "DataType", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Distance", "Distance", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Dosage", "Dosage", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Duration", "Duration", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Element", "Element", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ElementDefinition", "ElementDefinition", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Expression", "Expression", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Extension", "Extension", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("HumanName", "HumanName", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Identifier", "Identifier", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("MarketingStatus", "MarketingStatus", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Meta", "Meta", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Money", "Money", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Narrative", "Narrative", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ParameterDefinition", "ParameterDefinition", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Period", "Period", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("ProductShelfLife", "ProductShelfLife", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget, CMR.SourceIsNarrowerThanTarget),
        new("Quantity", "Quantity", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Range", "Range", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Ratio", "Ratio", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RatioRange", "RatioRange", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Reference", "Reference", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("RelatedArtifact", "RelatedArtifact", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("SampledData", "SampledData", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("Signature", "Signature", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("Timing", "Timing", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        new("TriggerDefinition", "TriggerDefinition", CMR.Equivalent, CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget),
        new("UsageContext", "UsageContext", CMR.Equivalent, CMR.Equivalent, CMR.Equivalent),
        ];
}
