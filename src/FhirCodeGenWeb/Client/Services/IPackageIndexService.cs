// <copyright file="IPackageIndexService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Client.Services;

/// <summary>Interface for package index service.</summary>
public interface IPackageIndexService
{
    /// <summary>Gets or sets the package records.</summary>
    Dictionary<string, PackageCacheRecord> PackageRecords { get; set; }

    /// <summary>Occurs when On Changed.</summary>
    event EventHandler<EventArgs> OnChanged;

    /// <summary>State has changed.</summary>
    void StateHasChanged();
}
