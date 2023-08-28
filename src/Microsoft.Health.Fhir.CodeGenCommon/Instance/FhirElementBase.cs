// <copyright file="FhirElementBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR element base.</summary>
public record class FhirElementBase : ICloneable
{
    /// <summary>Initializes a new instance of the FhirElementBase class.</summary>
    public FhirElementBase() { }

    /// <summary>Initializes a new instance of the FhirElementBase class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirElementBase(FhirElementBase other)
    {
        Id = other.Id;
        Extensions = other.Extensions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>Gets the Id for this element/resource/datatype.</summary>
    /// <value>The Id for this element/resource/datatype.</value>
    public string Id { get; init; } = string.Empty;

    /// <summary>Gets or initializes the extensions.</summary>
    public Dictionary<string, object> Extensions { get; init; } = new();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
