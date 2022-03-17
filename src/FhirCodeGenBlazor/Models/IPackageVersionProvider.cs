// <copyright file="IPackageVersionProvider.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
namespace FhirCodeGenBlazor.Models;

/// <summary>Interface for has package and version.</summary>
public interface IPackageVersionProvider
{
    /// <summary>Gets or sets the name of the package.</summary>
    string PackageName { get; set; }

    /// <summary>Gets or sets the version.</summary>
    string Version { get; set; }
}
