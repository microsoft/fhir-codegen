// <copyright file="FhirReference.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR reference.</summary>
public record class FhirReference : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirReference"/> class.
    /// </summary>
    public FhirReference() { }

    /// <summary>Initializes a new instance of the <see cref="FhirReference"/> class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirReference(FhirReference other)
    {
        Reference = other.Reference;
        ResourceType = other.ResourceType;
        Identifier = other.Identifier;
        Display = other.Display;
    }

    /// <summary>Gets or initializes the literal reference, Relative, internal or absolute URL.</summary>
    public string Reference { get; init; } = string.Empty;

    /// <summary>Gets or initializes the type the reference refers to (e.g. "Patient") - must be a resource in resources.</summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>Gets or initializes the logical reference, when literal reference is not known.</summary>
    public FhirIdentifier? Identifier { get; init; } = null;

    /// <summary>Gets or initializes the text alternative for the resource.</summary>
    public string Display { get; init; } = string.Empty;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
