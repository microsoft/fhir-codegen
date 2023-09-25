// <copyright file="FhirPackageVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.PackageManager;

/// <summary>FHIR Package version information.</summary>
internal class FhirPackageVersionInfo
{
    /// <summary>Gets or sets the name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the date.</summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>Gets or sets the version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets URL of the document.</summary>
    [JsonPropertyName("url")]
    public Uri? URL { get; set; } = null;

    /// <summary>Gets or sets the distribution.</summary>
    [JsonPropertyName("dist")]
    public DistributionInfo? Distribution { get; set; } = null;

    /// <summary>Gets or sets the FHIR version.</summary>
    [JsonPropertyName("fhirVersion")]
    public string FhirVersion { get; set; } = string.Empty;

    /// <summary>Gets or sets the unlisted.</summary>
    [JsonPropertyName("unlisted")]
    public string Unlisted { get; set; } = string.Empty;

    /// <summary>Gets or sets the canonical.</summary>
    [JsonPropertyName("canonical")]
    public Uri? Canonical { get; set; } = null;

    /// <summary>Gets or sets the security.</summary>
    [JsonPropertyName("security")]
    public string Security { get; set; } = string.Empty;

    /// <summary>Gets or sets the kind.</summary>
    [JsonPropertyName("kind")]
    public string PackageKind { get; set; } = string.Empty;

    /// <summary>Gets or sets the count. </summary>
    [JsonPropertyName("count")]
    public string Count { get; set; } = string.Empty;

    /// <summary>Information about the distribution.</summary>
    public class DistributionInfo
    {
        /// <summary>Gets or sets the hash sha.</summary>
        [JsonPropertyName("shasum")]
        public string HashSHA { get; set; } = string.Empty;

        /// <summary>Gets or sets URL of the tarball.</summary>
        [JsonPropertyName("tarball")]
        public string TarballUrl { get; set; } = string.Empty;
    }
}
