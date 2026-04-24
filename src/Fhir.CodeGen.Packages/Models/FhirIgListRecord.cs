using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

/// <summary>
/// Full publication list of all known implementation guides
/// </summary>
/// <remarks>
/// From https://github.com/FHIR/ig-registry/tree/master
/// </remarks>
public record class FhirIgListRecord
{
    /// <summary>
    /// Analysis record of the IG
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    public record class AnalysisRecord
    {
        [JsonPropertyName("error")]
        public string? Error { get; init; } = null;

        [JsonPropertyName("content")]
        public bool? ContentFlag { get; init; } = null;

        [JsonPropertyName("carePlanning")]
        public bool? CarePlanningFlag { get; init; } = null;

        [JsonPropertyName("clinicalCore")]
        public bool? ClinicalCoreFlag { get; init; } = null;

        [JsonPropertyName("codeSystems")]
        public int? CodeSystemCount { get; init; } = null;

        [JsonPropertyName("documents")]
        public bool? DocumentFlag { get; init; } = null;

        [JsonPropertyName("examples")]
        public int? ExampleCount { get; init; } = null;

        [JsonPropertyName("extensions")]
        public int? ExtensionCount { get; init; } = null;

        [JsonPropertyName("logicals")]
        public int? LogicalModelCount { get; init; } = null;

        [JsonPropertyName("medsMgmt")]
        public bool? MedicationManagementFlag { get; init; } = null;

        [JsonPropertyName("operations")]
        public int? OperationCount { get; init; } = null;

        [JsonPropertyName("profiles")]
        public int? ProfileCount { get; init; } = null;

        [JsonPropertyName("questionnaire")]
        public bool? QuestionnaireFlag { get; init; } = null;

        [JsonPropertyName("rest")]
        public bool? RestFlag { get; init; } = null;

        [JsonPropertyName("valuesets")]
        public int? ValueSetCount { get; init; } = null;
    }

    /// <summary>
    /// Published edition information
    /// </summary>
    public record class EditionRecord
    {
        /// <summary>
        /// Human name for published edition
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// the stated version of the IG itself
        /// </summary>
        [JsonPropertyName("ig-version")]
        public required string IgVersion { get; init; }

        /// <summary>
        /// he npm-name and version for the IG e.g. hl7.fhir.us.core#3.0.1
        /// </summary>
        [JsonPropertyName("package")]
        public required string CacheDirective { get; init; }

        /// <summary>
        /// array for the the version of the FHIR spec that the IG is based on e.g. ["4.0.0"]
        /// </summary>
        [JsonPropertyName("fhir-version")]
        public required string[] FhirVersions { get; init; }

        /// <summary>
        /// where the edition is found (just the base URL - do not include the index.html etc)
        /// </summary>
        [JsonPropertyName("url")]
        public required string PublicationBaseUrl { get; init; }
    }

    /// <summary>
    /// Known implementations of the IG
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    public record class ImplementationRecord
    {
        [JsonPropertyName("name")]
        public string? Name { get; init; } = null;

        [JsonPropertyName("type")]
        public string? EntryType { get; init; } = null;

        [JsonPropertyName("url")]
        public string? Url { get; init; } = null;
    }


    /// <summary>
    /// human name for the implenentation guide
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// the npm-name for all versions of this IG, if they are all the same (e.g. "hl7.fhir.us.core")
    /// </summary>
    [JsonPropertyName("npm-name")]
    public string? NpmName { get; init; } = null;

    /// <summary>
    /// arbitrary category for sorting/filtering - check existing ones
    /// </summary>
    [JsonPropertyName("category")]
    public required string Category { get; init; }

    /// <summary>
    /// a human readable description of the IG contents
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    /// Who is responsible for publishing the IG. All IGs published by HL7 or affiliates are "HL7"
    /// </summary>
    [JsonPropertyName("authority")]
    public required string Authority { get; init; }

    /// <summary>
    /// Appears to be product family, if published by HL7
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    [JsonPropertyName("product")]
    public string[]? Product { get; init; } = null;

    /// <summary>
    /// ISO 2 letter code, or "UV" for international
    /// </summary>
    [JsonPropertyName("country")]
    public required string Country { get; init; }

    /// <summary>
    /// Appears to be two-letter language code
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    [JsonPropertyName("langauge")]
    public string[]? Language { get; init; }

    /// <summary>
    /// URL to see a list of all published versions of the IG
    /// </summary>
    [JsonPropertyName("history")]
    public string? History { get; init; } = null;

    /// <summary>
    /// Appears to be the canonical URL of the package
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    [JsonPropertyName("canonical")]
    public string? Canonical { get; init; } = null;

    /// <summary>
    /// URL to see the CI build of the IG (just the base URL - do not include the index.html etc)
    /// </summary>
    [JsonPropertyName("ci-build")]
    public string? CiBuild { get; init; } = null;

    /// <summary>
    /// optional array containing at least 1 published edition information
    /// </summary>
    [JsonPropertyName("editions")]
    public EditionRecord[]? Editions { get; init; } = null;

    /// <summary>
    /// Appears to be selected content from QA
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    [JsonPropertyName("analysis")]
    public AnalysisRecord? Analysis { get; init; } = null;


    /// <summary>
    /// Appears to be a record of known implementations
    /// </summary>
    /// <remarks>
    /// Not documented at https://github.com/FHIR/ig-registry/tree/master
    /// </remarks>
    [JsonPropertyName("implementations")]
    public List<ImplementationRecord>? Implementations { get; init; } = null;
}

