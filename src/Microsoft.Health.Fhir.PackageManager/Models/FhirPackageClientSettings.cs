// <copyright file="FhirPackageClientSettings.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Extensions.Logging;

namespace Microsoft.Health.Fhir.PackageManager.Models;

/// <summary>A FHIR package client settings.</summary>
public record class FhirPackageClientSettings
{
    /// <summary>
    /// Gets or initializes the full pathname of the FHIR cache directory (empty/default will use
    /// '~/.fhir').
    /// </summary>
    public string CachePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes additional registry urls (default includes pacakges.fhir.org and
    /// packages2.fhir.org).
    /// </summary>
    public IEnumerable<string> AdditionalRegistryUrls { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or initializes a value indicating whether to use offline mode (only use the local cache).
    /// </summary>
    public bool OfflineMode { get; init; } = false;
}
