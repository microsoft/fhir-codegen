// <copyright file="FhirPackageArtifacts.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR artifacts.</summary>
public class FhirPackageArtifacts
{
    /// <summary>Gets or sets a list of types of the primitives.</summary>
    public HashSet<string> PrimitiveTypes { get; set; } = new();

    /// <summary>Gets or sets a list of types of the complexes.</summary>
    public HashSet<string> ComplexTypes { get; set; } = new();

    /// <summary>Gets or sets the resources.</summary>
    public HashSet<string> Resources { get; set; } = new();

    /// <summary>Gets or sets the extension urls.</summary>
    public HashSet<string> ExtensionUrls { get; set; } = new();

    /// <summary>Gets or sets the operations.</summary>
    public HashSet<string> Operations { get; set; } = new();

    /// <summary>Gets or sets the search parameter urls.</summary>
    public HashSet<string> SearchParameterUrls { get; set; } = new();

    /// <summary>Gets or sets the code systems.</summary>
    public HashSet<string> CodeSystems { get; set; } = new();

    /// <summary>Gets or sets the sets the value belongs to.</summary>
    public HashSet<string> ValueSets { get; set; } = new();

    /// <summary>Gets or sets the profiles.</summary>
    public HashSet<string> Profiles { get; set; } = new();
}
