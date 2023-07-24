// <copyright file="FhirRange.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR range.</summary>
public class FhirRange : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirRange"/> class.
    /// </summary>
    public FhirRange() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirRange"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    public FhirRange(FhirRange other)
    {
        Low = other.Low;
        High = other.High;
    }

    /// <summary>Gets or initializes the low.</summary>
    public FhirQuantity? Low { get; init; } = null;

    /// <summary>Gets or initializes the high.</summary>
    public FhirQuantity? High { get; init; } = null;

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone() => new FhirRange(this);
}
