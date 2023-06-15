// <copyright file="FhirSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Data;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Refactor;

/// <summary>A FHIR slicing.</summary>
public record class FhirSlicing : ICloneable, IReadOnlyDictionary<string, FhirComplex>
{
    private FhirSlicingRule _slicingRules;

    /// <summary>Values that represent how slices are interpreted when evaluating an instance.</summary>
    public enum FhirSlicingRule
    {
        /// <summary>No additional content is allowed other than that described by the slices in this profile.</summary>
        [FhirLiteral("closed")]
        Closed,

        /// <summary>Additional content is allowed anywhere in the list.</summary>
        [FhirLiteral("open")]
        Open,

        /// <summary>
        /// Additional content is allowed, but only at the end of the list. Note that using this requires
        /// that the slices be ordered, which makes it hard to share uses. This should only be done where
        /// absolutely required.
        /// </summary>
        [FhirLiteral("openAtEnd")]
        OpenAtEnd,
    }

    protected FhirSlicing(FhirSlicing source)
    {
        DefinedById = source.DefinedById;
        DefinedByUrl = source.DefinedByUrl;
        Description = source.Description;
        IsOrdered = source.IsOrdered;
        FieldOrder = source.FieldOrder;
        _slicingRules = source.SlicingRules;

    }

    /// <summary>Gets the identifier of the defined by.</summary>
    public required string DefinedById { get; init; }

    /// <summary>Gets URL of the defined by.</summary>
    public required string DefinedByUrl { get; init; }

    /// <summary>Gets the text description of how slicing works (or not).</summary>
    /// <value>The text description of how slicing works (or not).</value>
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether elements must be in same order as slices.</summary>
    /// <value>True if elements must be in same order as slices, false if not.</value>
    public required bool IsOrdered { get; init; }

    /// <summary>Gets the field order.</summary>
    /// <value>The field order.</value>
    public required int FieldOrder { get; init; }

    /// <summary>Gets how slices are interpreted when evaluating an instance.</summary>
    /// <value>How slices are interpreted when evaluating an instance.</value>
    public FhirSlicingRule SlicingRules => _slicingRules;

    /// <summary>Initializes the FHIR slicing rules.</summary>
    /// <exception cref="InvalidConstraintException">Thrown when an Invalid Constraint error condition
    ///  occurs.</exception>
    public string FhirSlicingRules
    {
        init
        {
            _slicingRules = value switch
            {
                "closed" => FhirSlicingRule.Closed,
                "open" => FhirSlicingRule.Open,
                "openAtEnd" => FhirSlicingRule.OpenAtEnd,
                _ => throw new InvalidConstraintException($"Invalid slicing rule: {value}")
            };
        }
    }

    /// <summary>Gets the element values that are used to distinguish the slices.</summary>
    /// <value>The element values that are used to distinguish the slices.</value>
    public Dictionary<string, FhirSliceDiscriminatorRule> DiscriminatorRules => _rules;

    /// <summary>Gets the slices.</summary>
    /// <value>The slices.</value>
    public List<FhirComplex> Slices => _slices;

    /// <summary>Gets a value indicating whether this object has differential slices.</summary>
    public bool HasDifferentialSlices => _slicesInDifferential.Any();

    object ICloneable.Clone() => this with { };

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirSlicing.</returns>
    public FhirSlicing DeepCopy() => this with { };
}
