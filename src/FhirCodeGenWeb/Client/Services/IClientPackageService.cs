// <copyright file="IClientPackageService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.AspNetCore.Components;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Client.Services;

/// <summary>Interface for package index service.</summary>
public interface IClientPackageService
{
    /// <summary>Gets or sets the package records.</summary>
    Dictionary<string, PackageCacheRecord> PackageRecords { get; }

    /// <summary>Updates the packages and status.</summary>
    /// <returns>An asynchronous result.</returns>
    Task UpdatePackagesAndStatusAsync();

    /// <summary>Loads a package.</summary>
    /// <param name="directive">The directive.</param>
    void LoadPackageAsync(string directive);
    
    /// <summary>Occurs when On Changed.</summary>
    event EventHandler<EventArgs>? OnChanged;

    /// <summary>State has changed.</summary>
    void StateHasChanged();
}
