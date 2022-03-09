// <copyright file="PackageIndexService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Client.Services;

/// <summary>A service for accessing package indexes information.</summary>
public class PackageIndexService : IPackageIndexService
{
    /// <summary>The package records.</summary>
    private Dictionary<string, PackageCacheRecord> _packageRecords = new();

    /// <summary>Gets or sets the package records.</summary>
    public Dictionary<string, PackageCacheRecord> PackageRecords { get => _packageRecords; set => _packageRecords = value; }

    /// <summary>Occurs when On Changed.</summary>
    public event EventHandler<EventArgs>? OnChanged;

    /// <summary>State has changed.</summary>
    public void StateHasChanged()
    {
        if (OnChanged != null)
        {
            OnChanged(this, new());
        }
    }
}
