// <copyright file="FhirUsageContext.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR usage context.</summary>
public class FhirUsageContext : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirUsageContext"/> class.
    /// </summary>
    public FhirUsageContext() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirUsageContext"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    public FhirUsageContext(FhirUsageContext other)
    {
        ContextCode = other.ContextCode;
        ValueCodeable = other.ValueCodeable;
        ValueQuantity = other.ValueQuantity;
        ValueRange = other.ValueRange;
        ValueReference = other.ValueReference;
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

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone() => new FhirUsageContext(this);
}
