// <copyright file="FhirQuantity.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Resource.FhirCanonicalBase;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR quantity.</summary>
public class FhirQuantity : ICloneable
{
    private QuantityComparatorCodes? _comparator = null;
    private string _fhirComparator = string.Empty;

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
    public FhirQuantity() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirQuantity"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    public FhirQuantity(FhirQuantity other)
    {
        Value = other.Value;
        FhirComparator = other.FhirComparator;
        Unit = other.Unit;
        System = other.System;
        Code = other.Code;
    }

    /// <summary>Gets or initializes the numerical value (with implicit precision).</summary>
    public decimal? Value { get; init; } = null;

    /// <summary>Gets the comparator.</summary>
    public QuantityComparatorCodes? Comparator { get => _comparator; }

    /// <summary>Gets or initializes the FHIR comparator.</summary>
    public string FhirComparator
    {
        get => _fhirComparator;
        init
        {
            _fhirComparator = value;
            if (_fhirComparator.TryFhirEnum(out QuantityComparatorCodes v))
            {
                _comparator = v;
            }
        }
    }

    /// <summary>Gets or initializes the unit representation.</summary>
    public string Unit { get; init; } = string.Empty;

    /// <summary>Gets or initializes the system that defines coded unit form.</summary>
    public string System { get; init; } = string.Empty;

    /// <summary>Gets or initializes the coded form of the unit.</summary>
    public string Code { get; init; } = string.Empty;

    object ICloneable.Clone() => new FhirQuantity(this);
}
