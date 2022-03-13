// <copyright file="IClientArtifactService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using FhirCodeGenWeb.Client.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Client.Services;

/// <summary>Interface for client artifact service.</summary>
public interface IClientArtifactService
{
    /// <summary>Gets or sets the package records.</summary>
    Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>> ArtifactsForPackage(string packageName, string version);

    /// <summary>Updates the artifacts asynchronous.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>An asynchronous result.</returns>
    Task UpdateArtifactsAsync(string packageName, string version);

    /// <summary>Occurs when On Changed.</summary>
    event EventHandler<ArtifactIndexChangedEventArgs>? OnChanged;

    /// <summary>State has changed.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    void StateHasChanged(string packageName, string version);
}
