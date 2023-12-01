// <copyright file="CoreCiVersion.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.PackageManager.Models;

/// <summary>A core CI version record.</summary>
internal record class CoreCiVersion
{
    /// <summary>Gets or initializes the FHIR version.</summary>
    [JsonPropertyName("FhirVersion")]
    public string FhirVersion { get; init; } = string.Empty;

    /// <summary>Gets or initializes the version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>Gets or initializes the identifier of the build.</summary>
    [JsonPropertyName("buildId")]
    public string BuildId { get; init; } = string.Empty;

    /// <summary>Gets or initializes the build date.</summary>
    [JsonPropertyName("date")]
    public string BuildDate { get; init; } = string.Empty;
}
