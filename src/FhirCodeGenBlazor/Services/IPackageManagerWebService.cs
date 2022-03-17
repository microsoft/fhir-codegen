// <copyright file="IPackageManagerWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenBlazor.Services;

/// <summary>Interface for package manager web service.</summary>
public interface IPackageManagerWebService : IReadOnlyDictionary<string,PackageCacheRecord>
{
    /// <summary>Occurs when On Changed.</summary>
    event EventHandler<EventArgs>? OnChanged;

    /// <summary>State has changed.</summary>
    void StateHasChanged();

    /// <summary>Request package load.</summary>
    /// <param name="directive">   The directive.</param>
    /// <param name="requestState">[out] State of the request.</param>
    void RequestLoad(string directive, out PackageLoadStateEnum requestState);

    /// <summary>Status of load request.</summary>
    /// <param name="directive">The directive.</param>
    /// <returns>A PackageLoadStateEnum.</returns>
    PackageLoadStateEnum LoadRequestStatus(string directive);

    /// <summary>Initializes this object.</summary>
    void Init();
}
