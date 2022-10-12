// <copyright file="DiffCompletedEventArgs.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Models;

/// <summary>Additional information for difference completed events.</summary>
public class DiffCompletedEventArgs : EventArgs
{
    /// <summary>Gets or sets the package key a.</summary>
    public string PackageKeyA { get; set; } = "";

    /// <summary>Gets or sets the package key b.</summary>
    public string PackageKeyB { get; set; } = "";

    /// <summary>Gets or sets the difference results.</summary>
    public DiffResults? Results { get; set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="packageKeyA">The package key a.</param>
    /// <param name="packageKeyB">The package key b.</param>
    /// <param name="results">    The results.</param>
    public DiffCompletedEventArgs(string packageKeyA, string packageKeyB, DiffResults? results)
    {
        PackageKeyA = packageKeyA;
        PackageKeyB = packageKeyB;
        Results = results;
    }
}
