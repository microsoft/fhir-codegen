// <copyright file="IGListRec.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.PackageManager.Models;

/// <summary>Information about FHIR Implementation Guides, from fhir-ig-list.json.</summary>
internal class IGListRec
{
    public IEnumerable<GuideRec> Guides { get; set; } = [];

    public class GuideRec
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("npm-name")]
        public string NpmName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("authority")]
        public string Authority { get; set; } = string.Empty;

        [JsonPropertyName("product")]
        public string[] Product { get; set; } = [];

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string[] Language { get; set; } = [];

        [JsonPropertyName("implementations")]
        public Implementation[] Implementations { get; set; } = [];

        [JsonPropertyName("history")]
        public string History { get; set; } = string.Empty;

        [JsonPropertyName("canonical")]
        public string Canonical { get; set; } = string.Empty;

        [JsonPropertyName("ci-build")]
        public string CIBuild { get; set; } = string.Empty;

        [JsonPropertyName("analysis")]
        public Analysis? Analysis { get; set; } = null;

        [JsonPropertyName("editions")]
        public Edition[] Editions { get; set; } = [];
    }

    public class Analysis
    {
        [JsonPropertyName("carePlanning")]
        public bool? CarePlanning { get; set; } = null;

        [JsonPropertyName("clinicalCore")]
        public bool? ClinicalCore { get; set; } = null;

        [JsonPropertyName("content")]
        public bool? Content { get; set; } = null;

        [JsonPropertyName("diagnostics")]
        public bool? Diagnostics { get; set; } = null;

        [JsonPropertyName("documents")]
        public bool? Documents { get; set; } = null;

        [JsonPropertyName("financials")]
        public bool? Financials { get; set; } = null;

        [JsonPropertyName("medsMgmt")]
        public bool? MedicationManagement { get; set; } = null;

        [JsonPropertyName("questionnaire")]
        public bool? Questionnaire { get; set; } = null;

        [JsonPropertyName("rest")]
        public bool? Rest { get; set; } = null;

        [JsonPropertyName("scheduling")]
        public bool? Scheduling { get; set; } = null;

        [JsonPropertyName("profiles")]
        public int Profiles { get; set; } = 0;

        [JsonPropertyName("extensions")]
        public int Extensions { get; set; } = 0;

        [JsonPropertyName("operations")]
        public int Operations { get; set; } = 0;

        [JsonPropertyName("logicals")]
        public int Logicals { get; set; } = 0;

        [JsonPropertyName("valuesets")]
        public int Valuesets { get; set; } = 0;

        [JsonPropertyName("codeSystems")]
        public int CodeSystems { get; set; } = 0;

        [JsonPropertyName("examples")]
        public int Examples { get; set; } = 0;
    }

    public class Implementation
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class Edition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("ig-version")]
        public string IgVersion { get; set; } = string.Empty;

        [JsonPropertyName("package")]
        public string Package { get; set; } = string.Empty;

        [JsonPropertyName("fhir-version")]
        public string[] FhirVersion { get; set; } = [];

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
