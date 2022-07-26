// <copyright file="IPackageDiffWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using FhirCodeGenBlazor.Models;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Services;

/// <summary>Interface for package difference web service.</summary>
public interface IPackageDiffWebService
{
    /// <summary>Occurs when On Difference Completed.</summary>
    event EventHandler<DiffCompletedEventArgs>? OnDiffCompleted;

    /// <summary>Difference has completed.</summary>
    /// <param name="packageKeyA">The package key a.</param>
    /// <param name="packageKeyB">The package key b.</param>
    /// <param name="results">    The results.</param>
    void DiffHasCompleted(string packageKeyA, string packageKeyB, DiffResults? results);
    
    /// <summary>Initializes this object.</summary>
    void Init();

    /// <summary>Request difference.</summary>
    /// <param name="A">      An IPackageExportable to process.</param>
    /// <param name="B">      An IPackageExportable to process.</param>
    /// <param name="options">Options for controlling the operation.</param>
    /// <returns>An asynchronous result.</returns>
    Task RequestDiff(IPackageExportable A, IPackageExportable B, DifferOptions options);

    /// <summary>Request difference.</summary>
    /// <param name="A">      An IPackageExportable to process.</param>
    /// <param name="keyA">   The key of the artifact in package A to compare.</param>
    /// <param name="B">      An IPackageExportable to process.</param>
    /// <param name="keyB">   The key of the artifact in package B to compare.</param>
    /// <param name="options">Options for controlling the operation.</param>
    /// <returns>An asynchronous result.</returns>
    Task RequestDiff(
        IPackageExportable A,
        string keyA,
        IPackageExportable B,
        string keyB,
        DifferOptions options);

    /// <summary>Request difference.</summary>
    /// <param name="A">      An IPackageExportable to process.</param>
    /// <param name="keysA">  The keys of artifacts in package A to compare.</param>
    /// <param name="B">      An IPackageExportable to process.</param>
    /// <param name="keysB">  The keys of artifacts in package B to compare.</param>
    /// <param name="options">Options for controlling the operation.</param>
    /// <returns>An asynchronous result.</returns>
    Task RequestDiff(
        IPackageExportable A,
        List<string> keysA,
        IPackageExportable B,
        List<string> keysB,
        DifferOptions options);
}
