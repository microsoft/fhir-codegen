// <copyright file="ArtifactIndexChangedEventArgs.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace FhirCodeGenBlazor.Models;

/// <summary>Additional information for artifact index changed events.</summary>
public class ArtifactIndexChangedEventArgs : EventArgs
{
    /// <summary>Gets or sets the name of the package.</summary>
    public string PackageName { get; set; } = "";

    /// <summary>Gets or sets the version.</summary>
    public string Version { get; set; } = "";
}
