// <copyright file="FhirIdentifier.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Instance.FhirCanonicalBase;
using static Microsoft.Health.Fhir.CodeGenCommon.Structure.FhirComplex;
using Microsoft.Health.Fhir.CodeGenCommon.Instance;

namespace Microsoft.Health.Fhir.CodeGenCommon.BaseModels;

/// <summary>A FHIR identifier.</summary>
public record class FhirIdentifier : FhirBase, ICloneable
{
    /// <summary>Values that represent identifier use codes.</summary>
    public enum IdentifierUseCodes
    {
        /// <summary>
        /// The identifier recommended for display and use in real-world interactions which should be
        /// used when such identifier is different from the "official" identifier.
        /// </summary>
        [FhirLiteral("usual")]
        Usual,

        /// <summary>
        /// The identifier considered to be most trusted for the identification of this item.
        /// Sometimes also known as "primary" and "main". The determination of "official" is subjective
        /// and implementation guides often provide additional guidelines for use.
        /// </summary>
        [FhirLiteral("official")]
        Office,

        /// <summary>A temporary identifier.</summary>
        [FhirLiteral("temp")]
        Temp,

        /// <summary>
        /// An identifier that was assigned in secondary use - it serves to identify the object in a relative
        /// context, but cannot be consistently assigned to the same object again in a different context.
        /// </summary>
        [FhirLiteral("secondary")]
        Secondary,

        /// <summary>
        /// The identifier id no longer considered valid, but may be relevant for search purposes. E.g.
        /// Changes to identifier schemes, account merges, etc.
        /// </summary>
        [FhirLiteral("old")]
        Old,
    }

    private readonly IdentifierUseCodes? _use = null;
    private string _useLiteral = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirIdentifier"/> class.
    /// </summary>
    public FhirIdentifier() : base() { }

    /// <summary>Initializes a new instance of the <see cref="FhirIdentifier"/> class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirIdentifier(FhirIdentifier other)
        : base(other)
    {
        UseLiteral = other.UseLiteral;
        TypeCodeable = other.TypeCodeable == null ? null : other.TypeCodeable with { };
        System = other.System;
        Value = other.Value;
        PeriodStart = other.PeriodStart;
        PeriodEnd = other.PeriodEnd;
        Assigner = other.Assigner == null ? null : other.Assigner with { };
    }

    /// <summary>Gets the purpose of this identifier.</summary>
    public IdentifierUseCodes? Use => _use;

    /// <summary>Gets or initializes the FHIR purpose of this identifier.</summary>
    public string UseLiteral
    {
        get => _useLiteral;
        init
        {
            _useLiteral = value;
            _use = value.ToEnum<IdentifierUseCodes>();
        }
    }

    /// <summary>Gets or initializes the coded description of identifier.</summary>
    public FhirCodeableConcept? TypeCodeable { get; init; } = null;

    /// <summary>Gets or initializes the namespace for the identifier value.</summary>
    public string System { get; init; } = string.Empty;

    /// <summary>Gets or initializes the value that is unique.</summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>Gets or initializes the start of the time period when id is/was valid for use.</summary>
    public string PeriodStart { get; init; } = string.Empty;

    /// <summary>Gets or initializes the end of the time period when id is/was valid for use.</summary>
    public string PeriodEnd { get; init; } = string.Empty;

    /// <summary>Gets or initializes the organization that issued id (may be just text).</summary>
    public FhirReference? Assigner { get; init; } = null;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
