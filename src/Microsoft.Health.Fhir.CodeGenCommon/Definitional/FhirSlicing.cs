// <copyright file="FhirSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirSlicing;

namespace Microsoft.Health.Fhir.CodeGenCommon.Definitional;

/// <summary>A FHIR slicing.</summary>
public record class FhirSlicing : IReadOnlyDictionary<string, FhirComplex>, ICloneable
{
    private FhirSlicingRuleCodes _slicingRule;
    private string _slicingRuleLiterals = string.Empty;
    Dictionary<string, SliceDiscriminatorRule> _rules = new();
    Dictionary<string, FhirComplex> _slices = new();
    private readonly HashSet<string> _slicesInDifferential = new();

    /// <summary>Values that represent how slices are interpreted when evaluating an instance.</summary>
    public enum FhirSlicingRuleCodes
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

    /// <summary>Values that represent fhir slice discriminator types.</summary>
    public enum FhirSliceDiscriminatorTypeCodes
    {
        /// <summary>
        /// The slices have different values in the nominated element.
        /// </summary>
        [FhirLiteral("value")]
        Value,

        /// <summary>
        /// The slices are differentiated by the presence or absence of the nominated element.
        /// </summary>
        [FhirLiteral("exists")]
        Exists,

        /// <summary>
        /// The slices have different values in the nominated element, as determined by testing them
        /// against the applicable ElementDefinition.pattern[x].
        /// </summary>
        [FhirLiteral("pattern")]
        [Obsolete("This code means the same as value, and is retained for backwards compatibility reasons", false)]
        Pattern,

        /// <summary>
        /// The slices are differentiated by type of the nominated element.
        /// </summary>
        [FhirLiteral("type")]
        Type,

        /// <summary>
        /// The slices are differentiated by conformance of the nominated element to a specified
        /// profile. Note that if the path specifies .resolve() then the profile is the target profile
        /// on the reference. In this case, validation by the possible profiles is required to differentiate
        /// the slices.
        /// </summary>
        [FhirLiteral("profile")]
        Profile,

        /// <summary>
        /// The slices are differentiated by their index. This is only possible if all but the last slice
        /// have min=max cardinality, and the (optional) last slice contains other undifferentiated elements.
        /// </summary>
        [FhirLiteral("position")]
        Position,
    }

    /// <summary>A slicing discriminator rule.</summary>
    public record class SliceDiscriminatorRule : ICloneable
    {
        private readonly FhirSliceDiscriminatorTypeCodes _discriminatorType;
        private string _discriminatorTypeLiteral = string.Empty;

        /// <summary>Initializes a new instance of the SliceDiscriminatorRule class.</summary>
        public SliceDiscriminatorRule() { }

        /// <summary>Initializes a new instance of the SliceDiscriminatorRule class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected SliceDiscriminatorRule(SliceDiscriminatorRule other)
        {
            DiscriminatorTypeLiteral = other.DiscriminatorTypeLiteral;
            Path = other.Path;
        }

        /// <summary>
        /// Gets the type of the discriminator - how the element value is interpreted when discrimination
        /// is evaluated.
        /// </summary>
        public FhirSliceDiscriminatorTypeCodes DiscriminatorType => _discriminatorType;

        /// <summary>
        /// Gets the type of the FHIR discriminator - how the element value is interpreted when
        /// discrimination is evaluated.
        /// </summary>
        public required string DiscriminatorTypeLiteral
        {
            get => _discriminatorTypeLiteral;
            init
            {
                _discriminatorTypeLiteral = value;
                _discriminatorType = value.ToEnum<FhirSliceDiscriminatorTypeCodes>() ?? FhirSliceDiscriminatorTypeCodes.Value;
            }
        }

        /// <summary>
        /// Gets a FHIRPath expression, using the simple subset of FHIRPath, that is used to identify the
        /// element on which discrimination is based.
        /// </summary>
        public required string Path { get; init; }

        /// <summary>Gets the key.</summary>
        public string Key => _discriminatorType + "+" + Path;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>Initializes a new instance of the FhirSlicing class.</summary>
    public FhirSlicing() { }

    /// <summary>Initializes a new instance of the FhirSlicing class.</summary>
    /// <param name="other">Source for the.</param>
    [SetsRequiredMembers]
    protected FhirSlicing(FhirSlicing other)
    {
        SliceName = other.SliceName;
        IsConstraining = other.IsConstraining;
        DefinedById = other.DefinedById;
        DefinedByUrl = other.DefinedByUrl;
        Description = other.Description;
        IsOrdered = other.IsOrdered;
        FieldOrder = other.FieldOrder;
        SlicingRuleLiteral = other.SlicingRuleLiteral;
        _rules = other._rules.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _slices = other._slices.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _slicesInDifferential = other._slicesInDifferential.DeepCopy();
    }

    /// <summary>Gets the name of the slice - the name for this particular element in a set of slices.</summary>
    public required string SliceName { get; init; }

    /// <summary>
    /// Gets whether this slice is constraining (if this slice definition constrains an inherited
    /// slice definition (or not)).
    /// </summary>
    public bool? IsConstraining { get; init; } = null;

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
    public FhirSlicingRuleCodes SlicingRules { get => _slicingRule; }

    /// <summary>Initializes the FHIR slicing rules.</summary>
    /// <exception cref="InvalidConstraintException">Thrown when an Invalid Constraint error condition
    ///  occurs.</exception>
    public required string SlicingRuleLiteral
    {
        get => _slicingRuleLiterals;
        init
        {
            _slicingRuleLiterals = value;
            _slicingRule = value.ToEnum<FhirSlicingRuleCodes>() ?? FhirSlicingRuleCodes.Open;
        }
    }

    /// <summary>Gets the element values that are used to distinguish the slices.</summary>
    /// <value>The element values that are used to distinguish the slices.</value>
    public Dictionary<string, SliceDiscriminatorRule> DiscriminatorRules { get => _rules; }

    /// <summary>Gets a value indicating whether this object has differential slices.</summary>
    public bool HasDifferentialSlices => _slicesInDifferential.Any();

    /// <summary>Gets the slices.</summary>
    /// <value>The slices.</value>
    public IEnumerable<FhirComplex> Slices => _slices.Values;

    /// <summary>Gets the keys.</summary>
    /// <typeparam name="string">     Type of the string.</typeparam>
    /// <typeparam name="FhirComplex">Type of the FHIR complex.</typeparam>
    IEnumerable<string> IReadOnlyDictionary<string, FhirComplex>.Keys => _slices.Keys;

    /// <summary>Gets the values.</summary>
    /// <typeparam name="string">     Type of the string.</typeparam>
    /// <typeparam name="FhirComplex">Type of the FHIR complex.</typeparam>
    IEnumerable<FhirComplex> IReadOnlyDictionary<string, FhirComplex>.Values => _slices.Values;

    /// <summary>Gets the number of. </summary>
    /// <typeparam name="string">      Type of the string.</typeparam>
    /// <typeparam name="FhirComplex>">Type of the FHIR complex></typeparam>
    int IReadOnlyCollection<KeyValuePair<string, FhirComplex>>.Count => _slices.Count();

    /// <summary>Indexer to get items within this collection using array index syntax.</summary>
    /// <typeparam name="string">     Type of the string.</typeparam>
    /// <typeparam name="FhirComplex">Type of the FHIR complex.</typeparam>
    /// <param name="key">The key.</param>
    /// <returns>The indexed item.</returns>
    FhirComplex IReadOnlyDictionary<string, FhirComplex>.this[string key] => _slices[key];

    /// <summary>Query if 'key' contains key.</summary>
    /// <typeparam name="string">     Type of the string.</typeparam>
    /// <typeparam name="FhirComplex">Type of the FHIR complex.</typeparam>
    /// <param name="key">The key.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool IReadOnlyDictionary<string, FhirComplex>.ContainsKey(string key) => _slices.ContainsKey(key);

    /// <summary>Attempts to get value a FhirComplex from the given string.</summary>
    /// <typeparam name="string">     Type of the string.</typeparam>
    /// <typeparam name="FhirComplex">Type of the FHIR complex.</typeparam>
    /// <param name="key">  The key.</param>
    /// <param name="value">[out] The value.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool IReadOnlyDictionary<string, FhirComplex>.TryGetValue(string key, out FhirComplex value) => _slices.TryGetValue(key, out value);

    /// <summary>Gets the enumerator.</summary>
    /// <typeparam name="string">      Type of the string.</typeparam>
    /// <typeparam name="FhirComplex>">Type of the FHIR complex></typeparam>
    /// <returns>The enumerator.</returns>
    IEnumerator<KeyValuePair<string, FhirComplex>> IEnumerable<KeyValuePair<string, FhirComplex>>.GetEnumerator() => _slices.GetEnumerator();

    /// <summary>Gets the enumerator.</summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => _slices.GetEnumerator();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
