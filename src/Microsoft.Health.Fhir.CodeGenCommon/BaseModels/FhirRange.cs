// <copyright file="FhirRange.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.BaseModels;

/// <summary>A FHIR range.</summary>
public record class FhirRange : FhirBase, ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirRange"/> class.
    /// </summary>
    public FhirRange() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirRange"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirRange(FhirRange other)
        : base(other)
    {
        Low = other.Low == null ? null : other.Low with { };
        High = other.High == null ? null : other.High with { };
    }

    /// <summary>Gets or initializes the low.</summary>
    public FhirQuantity? Low { get; init; } = null;

    /// <summary>Gets or initializes the high.</summary>
    public FhirQuantity? High { get; init; } = null;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
