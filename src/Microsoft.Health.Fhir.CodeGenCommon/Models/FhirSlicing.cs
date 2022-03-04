// <copyright file="FhirSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>FHIR Slicing information.</summary>
public class FhirSlicing
{
    private readonly Dictionary<string, FhirSliceDiscriminatorRule> _rules;
    private readonly List<FhirComplex> _slices;
    private readonly Dictionary<string, FhirComplex> _slicesByName;

    /// <summary>Initializes a new instance of the <see cref="FhirSlicing"/> class.</summary>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="definedById">       The identifier of the defined by.</param>
    /// <param name="definedByUrl">      The defined by URL.</param>
    /// <param name="description">       The description.</param>
    /// <param name="isOrdered">         True if ordered, false if not.</param>
    /// <param name="slicingRules">      Rules associated with this slicing group.</param>
    /// <param name="discriminatorRules">The discriminator rules for this slicing group.</param>
    public FhirSlicing(
        string definedById,
        Uri definedByUrl,
        string description,
        bool? isOrdered,
        string slicingRules,
        IEnumerable<FhirSliceDiscriminatorRule> discriminatorRules)
    {
        DefinedById = definedById;
        DefinedByUrl = definedByUrl;
        Description = description;
        IsOrdered = isOrdered == true;

        switch (slicingRules)
        {
            case "Closed":
            case "closed":
                SlicingRules = FhirSlicingRule.Closed;
                break;

            case "Open":
            case "open":
                SlicingRules = FhirSlicingRule.Open;
                break;

            case "OpenAtEnd":
            case "openAtEnd":
                SlicingRules = FhirSlicingRule.OpenAtEnd;
                break;

            default:
                throw new ArgumentException($"Invalid Slicing Rule: {slicingRules}");
        }

        if (discriminatorRules == null)
        {
            throw new ArgumentNullException(nameof(discriminatorRules));
        }

        _rules = new Dictionary<string, FhirSliceDiscriminatorRule>();
        foreach (FhirSliceDiscriminatorRule discriminator in discriminatorRules)
        {
            _rules.Add(discriminator.Path, discriminator);
        }

        _slices = new List<FhirComplex>();
        _slicesByName = new Dictionary<string, FhirComplex>();
    }

    /// <summary>Initializes a new instance of the <see cref="FhirSlicing"/> class.</summary>
    /// <param name="definedById">       The identifier of the defined by.</param>
    /// <param name="definedByUrl">      The defined by URL.</param>
    /// <param name="description">       The description.</param>
    /// <param name="isOrdered">         True if ordered, false if not.</param>
    /// <param name="slicingRules">      Rules associated with this slicing group.</param>
    /// <param name="discriminatorRules">The discriminator rules for this slicing group.</param>
    /// <param name="slices">            The slices.</param>
    public FhirSlicing(
        string definedById,
        Uri definedByUrl,
        string description,
        bool isOrdered,
        FhirSlicingRule slicingRules,
        Dictionary<string, FhirSliceDiscriminatorRule> discriminatorRules,
        List<FhirComplex> slices)
    {
        DefinedById = definedById;
        DefinedByUrl = definedByUrl;
        Description = description;
        IsOrdered = isOrdered;
        SlicingRules = slicingRules;
        _rules = discriminatorRules;

        if (slices == null)
        {
            _slices = new List<FhirComplex>();
        }
        else
        {
            _slices = slices;
        }

        _slicesByName = new Dictionary<string, FhirComplex>();
        foreach (FhirComplex slice in _slices)
        {
            _slicesByName.Add(slice.SliceName, slice);
        }
    }

    /// <summary>Values that represent how slices are interpreted when evaluating an instance.</summary>
    public enum FhirSlicingRule
    {
        /// <summary>No additional content is allowed other than that described by the slices in this profile.</summary>
        Closed,

        /// <summary>Additional content is allowed anywhere in the list.</summary>
        Open,

        /// <summary>
        /// Additional content is allowed, but only at the end of the list. Note that using this requires
        /// that the slices be ordered, which makes it hard to share uses. This should only be done where
        /// absolutely required.
        /// </summary>
        OpenAtEnd,
    }

    /// <summary>Gets the identifier of the defined by.</summary>
    /// <value>The identifier of the defined by.</value>
    public string DefinedById { get; }

    /// <summary>Gets URL of the defined by.</summary>
    /// <value>The defined by URL.</value>
    public Uri DefinedByUrl { get; }

    /// <summary>Gets the text description of how slicing works (or not).</summary>
    /// <value>The text description of how slicing works (or not).</value>
    public string Description { get; }

    /// <summary>Gets a value indicating whether elements must be in same order as slices.</summary>
    /// <value>True if elements must be in same order as slices, false if not.</value>
    public bool IsOrdered { get; }

    /// <summary>Gets the field order.</summary>
    /// <value>The field order.</value>
    public int FieldOrder { get; }

    /// <summary>Gets how slices are interpreted when evaluating an instance.</summary>
    /// <value>How slices are interpreted when evaluating an instance.</value>
    public FhirSlicingRule SlicingRules { get; }

    /// <summary>Gets the element values that are used to distinguish the slices.</summary>
    /// <value>The element values that are used to distinguish the slices.</value>
    public Dictionary<string, FhirSliceDiscriminatorRule> DiscriminatorRules => _rules;

    /// <summary>Gets the slices.</summary>
    /// <value>The slices.</value>
    public List<FhirComplex> Slices => _slices;

    /// <summary>Indexer to get slices based on name.</summary>
    /// <param name="sliceName">Name of the slice.</param>
    /// <returns>The indexed item.</returns>
    public FhirComplex this[string sliceName]
    {
        get
        {
            if (!_slicesByName.ContainsKey(sliceName))
            {
                throw new ArgumentOutOfRangeException(nameof(sliceName));
            }

            return _slicesByName[sliceName];
        }
    }

    /// <summary>Query if 'name' has slice.</summary>
    /// <param name="name">The name.</param>
    /// <returns>True if slice, false if not.</returns>
    public bool HasSlice(string name)
    {
        return _slicesByName.ContainsKey(name);
    }

    /// <summary>Adds a slice.</summary>
    /// <param name="sliceName">Name of the slice.</param>
    /// <param name="slice">    The slice.</param>
    public void AddSlice(string sliceName, FhirComplex slice)
    {
        slice.SliceName = sliceName;
        _slices.Add(slice);
        _slicesByName.Add(sliceName, slice);
    }

    /// <summary>Deep copy.</summary>
    /// <param name="primitiveTypeMap">   The primitive type map.</param>
    /// <param name="copySlicing">        True to copy slicing.</param>
    /// <param name="canHideParentFields">True if can hide parent fields, false if not.</param>
    /// <param name="valueSetReferences"> [in,out] Value Set URLs and lists of FHIR paths that
    ///  reference them.</param>
    /// <returns>A FhirSlicing.</returns>
    public FhirSlicing DeepCopy(
        Dictionary<string, string> primitiveTypeMap,
        bool copySlicing,
        bool canHideParentFields,
        Dictionary<string, ValueSetReferenceInfo> valueSetReferences)
    {
        Dictionary<string, FhirSliceDiscriminatorRule> rules = new Dictionary<string, FhirSliceDiscriminatorRule>();

        foreach (KeyValuePair<string, FhirSliceDiscriminatorRule> kvp in _rules)
        {
            rules.Add(kvp.Key, kvp.Value.DeepCopy());
        }

        List<FhirComplex> slices = new List<FhirComplex>();

        foreach (FhirComplex slice in _slices)
        {
            slices.Add(
                slice.DeepCopy(
                    primitiveTypeMap,
                    copySlicing,
                    canHideParentFields,
                    valueSetReferences,
                    null));
        }

        return new FhirSlicing(
            DefinedById,
            DefinedByUrl,
            Description,
            IsOrdered,
            SlicingRules,
            rules,
            slices);
    }
}
