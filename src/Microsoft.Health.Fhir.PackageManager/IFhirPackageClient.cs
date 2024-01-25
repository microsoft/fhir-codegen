// <copyright file="IFhirPackageClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.PackageManager;

/// <summary>Interface for FHIR package client.</summary>
public interface IFhirPackageClient : IDisposable
{
    /// <summary>Occurs when a package has been downloaded or deleted.</summary>
    event EventHandler<EventArgs>? OnChanged;

    /// <summary>Creates a new IFhirPackageClient.</summary>
    /// <param name="settings">(Optional) Options for controlling the operation.</param>
    /// <returns>An IFhirPackageClient.</returns>
    static abstract IFhirPackageClient Create(FhirPackageClientSettings? settings = null);

    /// <summary>Deletes the package described by packageDirective.</summary>
    /// <param name="packageDirective">The package directive.</param>
    void DeletePackage(string packageDirective);

    /// <summary>
    /// Resolve a package directive, donwload the package if necessary, and return the local and
    /// extracted package information.
    /// </summary>
    /// <param name="directive">          The directive.</param>
    /// <param name="includeDependencies">(Optional) True to include, false to exclude the dependencies.</param>
    /// <param name="cancellationToken">  (Optional) A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields the package by directive.</returns>
    Task<PackageCacheEntry?> FindOrDownloadPackageByDirective(
        string directive,
        bool includeDependencies = false,
        CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Resolve a package URL, donwload the package if necessary, and return the local and extracted
    /// package information.
    /// </summary>
    /// <param name="url">                URL of the package tgz or IG page URL.</param>
    /// <param name="includeDependencies">(Optional) True to include, false to exclude the dependencies.</param>
    /// <param name="cancellationToken">  (Optional) A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields the package by URL.</returns>
    Task<PackageCacheEntry?> FindOrDownloadPackageByUrl(
        string url,
        bool includeDependencies = false,
        CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>Gets local entries.</summary>
    /// <param name="name">          (Optional) Name of the package to search for.  By default, the
    ///  value is used as a 'starts-with' and case-insensitive comparison with local package names
    ///  (e.g., will return all local packages).</param>
    /// <param name="exactMatchOnly">(Optional) True to only return only exact name matches.</param>
    /// <returns>The local entries.</returns>
    IEnumerable<PackageCacheEntry> LocalPackages(
        string name = "",
        bool exactMatchOnly = false);

    /// <summary>Gets a manifest.</summary>
    /// <param name="packageEntry">The package entry.</param>
    /// <returns>The manifest.</returns>
    CachePackageManifest? GetManifest(PackageCacheEntry packageEntry);

    /// <summary>Gets the indexed contents (i.e., via parsing .index.json).</summary>
    /// <param name="packageEntry">The package entry.</param>
    /// <returns>The indexed contents.</returns>
    PackageContents? GetIndexedContents(PackageCacheEntry packageEntry);
}
