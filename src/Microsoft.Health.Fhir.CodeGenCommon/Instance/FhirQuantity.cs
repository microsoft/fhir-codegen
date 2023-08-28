// <copyright file="FhirQuantity.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Instance.FhirCanonicalBase;
using static Microsoft.Health.Fhir.CodeGenCommon.Instance.FhirObligation;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR quantity.</summary>
public record class FhirQuantity : FhirElementBase, ICloneable
{
    private QuantityComparatorCodes? _comparator = null;
    private string _comparatorLiteral = string.Empty;

    /// <summary>Values that represent quantity comparator codes.</summary>
    public enum QuantityComparatorCodes
    {
        /// <summary>An enum constant representing the less than option.</summary>
        [FhirLiteral("<")]
        LessThan,

        /// <summary>An enum constant representing the less than or equal option.</summary>
        [FhirLiteral("<=")]
        LessThanOrEqual,

        /// <summary>An enum constant representing the greater than option.</summary>
        [FhirLiteral(">")]
        GreaterThan,

        /// <summary>An enum constant representing the greater than or equal option.</summary>
        [FhirLiteral(">=")]
        GreaterThanOrEqual,

        /// <summary>An enum constant representing the approximately option.</summary>
        [FhirLiteral("ad")]
        Approximately,
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirQuantity"/> class.
    /// </summary>
    public FhirQuantity() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirQuantity"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirQuantity(FhirQuantity other)
        : base(other)
    {
        Value = other.Value;
        ComparatorLiteral = other.ComparatorLiteral;
        Unit = other.Unit;
        System = other.System;
        Code = other.Code;
    }

    /// <summary>Gets or initializes the numerical value (with implicit precision).</summary>
    public decimal? Value { get; init; } = null;

    /// <summary>Gets the comparator.</summary>
    public QuantityComparatorCodes? Comparator { get => _comparator; }

    /// <summary>Gets or initializes the FHIR comparator.</summary>
    public string ComparatorLiteral
    {
        get => _comparatorLiteral;
        init
        {
            _comparatorLiteral = value;
            _comparator = value.ToEnum<QuantityComparatorCodes>();
        }
    }

    /// <summary>Gets or initializes the unit representation.</summary>
    public string Unit { get; init; } = string.Empty;

    /// <summary>Gets or initializes the system that defines coded unit form.</summary>
    public string System { get; init; } = string.Empty;

    /// <summary>Gets or initializes the coded form of the unit.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
