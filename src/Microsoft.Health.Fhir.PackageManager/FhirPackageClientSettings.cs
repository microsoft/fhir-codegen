// <copyright file="FhirPackageClientSettings.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Extensions.Logging;

namespace Microsoft.Health.Fhir.PackageManager;

/// <summary>A FHIR package client settings.</summary>
public record class FhirPackageClientSettings
{
    /// <summary>
    /// Gets or initializes the full pathname of the FHIR cache directory (empty/default will use
    /// '~/.fhir').
    /// </summary>
    public string? CachePath { get; init; } = null;

    /// <summary>
    /// Gets or initializes a value indicating whether this object use official FHIR registries.
    /// </summary>
    public bool UseOfficialFhirRegistries { get; init; } = true;

    /// <summary>
    /// Gets or initializes additional FHIR registry urls.
    /// </summary>
    public string[] AdditionalFhirRegistryUrls { get; init; } = [];

    /// <summary>Gets or initializes the additional NPM registry urls.</summary>
    public string[] AdditionalNpmRegistryUrls { get; init; } = [];

    /// <summary>
    /// Gets or initializes a value indicating whether to use offline mode (only use the local cache).
    /// </summary>
    public bool OfflineMode { get; init; } = false;
}
