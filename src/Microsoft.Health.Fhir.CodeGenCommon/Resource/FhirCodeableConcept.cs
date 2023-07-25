// <copyright file="FhirCodeableConcept.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR codeable concept.</summary>
public record class FhirCodeableConcept : ICloneable
{
    /// <summary>Initializes a new instance of the <see cref="FhirCodeableConcept"/> class.</summary>
    public FhirCodeableConcept() { }

    /// <summary>Initializes a new instance of the FhirCodeableConcept class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirCodeableConcept(FhirCodeableConcept other)
    {
        Coding = other.Coding.Select(v => v with { });
        Text = other.Text;
    }

    /// <summary>Gets or initializes the code defined by a terminology system.</summary>
    public required IEnumerable<FhirCoding> Coding { get; init; } = Enumerable.Empty<FhirCoding>();

    /// <summary>Gets or initializes the plain text representation of the concept.</summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
