// <copyright file="FhirUsageContext.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR usage context.</summary>
public record class FhirUsageContext : FhirElementBase, ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirUsageContext"/> class.
    /// </summary>
    public FhirUsageContext() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirUsageContext"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirUsageContext(FhirUsageContext other)
        : base(other)
    {
        ContextCode = other.ContextCode with { };
        ValueCodeable = other.ValueCodeable == null ? null : other.ValueCodeable with { };
        ValueQuantity = other.ValueQuantity == null ? null : other.ValueQuantity with { };
        ValueRange = other.ValueRange == null ? null : other.ValueRange with { };
        ValueReference = other.ValueReference == null ? null : other.ValueReference with { };
    }

    /// <summary>Gets or initializes the context code.</summary>
    public required FhirCoding ContextCode { get; init; }

    /// <summary>Gets or initializes the value.</summary>
    public FhirCodeableConcept? ValueCodeable { get; init; } = null;

    /// <summary>Gets or initializes the value quantity.</summary>
    public FhirQuantity? ValueQuantity { get; init; } = null;

    /// <summary>Gets or initializes the value range.</summary>
    public FhirRange? ValueRange { get; init; } = null;

    /// <summary>Gets or initializes the value reference.</summary>
    public FhirReference? ValueReference { get; init; } = null;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
