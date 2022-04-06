// <copyright file="IPackageManagerWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace FhirCodeGenBlazor.Services;

/// <summary>Interface for package manager web service.</summary>
public interface IPackageManagerWebService : IReadOnlyDictionary<string,PackageCacheRecord>
{
    /// <summary>Occurs when On Changed.</summary>
    event EventHandler<EventArgs>? OnChanged;

    /// <summary>State has changed.</summary>
    void StateHasChanged();

    /// <summary>Request download.</summary>
    /// <param name="directive">   The directive.</param>
    /// <param name="branchName">  Name of the branch.</param>
    /// <param name="requestState">[out] State of the request.</param>
    void RequestDownload(string directive, string branchName, out PackageLoadStateEnum requestState);

    /// <summary>Request package load.</summary>
    /// <param name="directive">   The directive.</param>
    /// <param name="requestState">[out] State of the request.</param>
    void RequestLoad(string directive, out PackageLoadStateEnum requestState);

    /// <summary>Deletes the package described by directive.</summary>
    /// <param name="directive">The directive.</param>
    void DeletePackage(string directive);

    /// <summary>Status of load request.</summary>
    /// <param name="directive">The directive.</param>
    /// <returns>A PackageLoadStateEnum.</returns>
    PackageLoadStateEnum LoadRequestStatus(string directive);

    /// <summary>
    /// Attempts to get package manifests an IEnumerable&lt;RegistryPackageManifest&gt; from the
    /// given string.
    /// </summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="manifests">  [out] The manifests.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool TryGetRegistryManifests(string packageName, out IEnumerable<RegistryPackageManifest> manifests);

    /// <summary>
    /// Attempts to get core ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool TryGetCoreCiPackageDetails(string branchName, out NpmPackageDetails details);

    /// <summary>
    /// Attempts to get guide ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool TryGetGuideCiPackageDetails(string branchName, out NpmPackageDetails details);


    /// <summary>Initializes this object.</summary>
    void Init();
}
