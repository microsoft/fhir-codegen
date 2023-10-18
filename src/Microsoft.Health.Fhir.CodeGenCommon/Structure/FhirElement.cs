// <copyright file="FhirElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Expectations;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.Health.Fhir.CodeGenCommon.Instance.FhirCanonicalBase;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structure;

/// <summary>
/// A FHIR element definition.
/// Note that this class needs to implement IConformanceAnnotated because we don't want all
/// definitions to inherit from ConformanceAnnotatedBase.
/// </summary>
public record class FhirElement : FhirDefinitionBase, IConformanceAnnotated, ICloneable
{
    private int _cardinalityMax = -1;
    private Dictionary<string, FhirElementType> _elementTypes = new();
    private Dictionary<string, List<FhirElementMapping>> _mappings = new();
    private IEnumerable<PropertyRepresentationCodes> _representations = Enumerable.Empty<PropertyRepresentationCodes>();
    private IEnumerable<string> _representationLiterals = Enumerable.Empty<string>();
    private string _bindingStrength = string.Empty;
    private ElementDefinitionBindingStrength? _valueSetBindingStrength = null;
    private Dictionary<string, FhirSlicing> _slicing = new();
    private string _fiveWs = string.Empty;
    private bool _inDifferential = false;
    private bool _modifiesParent = false;
    private HashSet<string> _conditions = new();
    private Dictionary<string, FhirConstraint> _constraints = new();

    /// <summary>Values that represent element definition binding strengths.</summary>
    public enum ElementDefinitionBindingStrength : int
    {
        /// <summary>Instances are not expected or even encouraged to draw from the specified value set. The value set merely provides examples of the types of concepts intended to be included.</summary>
        [FhirLiteral("example")]
        Example = 1,

        /// <summary>Instances are encouraged to draw from the specified codes for interoperability purposes but are not required to do so to be considered conformant.</summary>
        [FhirLiteral("preferred")]
        Preferred,

        /// <summary>To be conformant, the concept in this element SHALL be from the specified value set if any of the codes within the value set can apply to the concept being communicated. If the value set does not cover the concept (based on human review), alternate codings (or, data type allowing, text) may be included instead.</summary>
        [FhirLiteral("extensible")]
        Extensible,

        /// <summary>To be conformant, the concept in this element SHALL be from the specified value set.</summary>
        [FhirLiteral("required")]
        Required,
    }

    /// <summary>Values that represent how a property is represented when serialized.</summary>
    public enum PropertyRepresentationCodes : int
    {
        /// <summary>In XML, this property is represented as an attribute not an element.</summary>
        [FhirLiteral("xmlAttr")]
        xmlAttr = 1,

        /// <summary>This element is represented using the XML text attribute (primitives only).</summary>
        [FhirLiteral("xmlText")]
        xmlText,

        /// <summary>The type of this element is indicated using xsi:type.</summary>
        [FhirLiteral("typeAttr")]
        typeAttr,

        /// <summary>Use CDA narrative instead of XHTML.</summary>
        [FhirLiteral("cdaText")]
        cdaText,

        /// <summary>The property is represented using XHTML.</summary>
        [FhirLiteral("xhtml")]
        xhtml,
    }

    /// <summary>Initializes a new instance of the FhirElement class.</summary>
    public FhirElement() : base() { }

    /// <summary>Initializes a new instance of the FhirElement class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirElement(FhirElement other)
        : base(other)
    {
        BasePath = other.BasePath;
        RootArtifact = other.RootArtifact with { };
        ExplicitName = other.ExplicitName;
        CardinalityMin = other.CardinalityMin;
        CardinalityMaxString = other.CardinalityMaxString;
        _inDifferential = other._inDifferential;
        IsInherited = other.IsInherited;
        ModifiesParent = other.ModifiesParent;
        HidesParent = other.HidesParent;
        IsModifier = other.IsModifier;
        IsModifierReason = other.IsModifierReason;
        IsSummary = other.IsSummary;
        IsMustSupport = other.IsMustSupport;
        IsSimple = other.IsSimple;
        FieldOrder = other.FieldOrder;
        FhirMappings = other._mappings.Values.SelectMany(l => l).Select(m => m with { });
        RepresentationLiterals = other._representationLiterals.Select(v => v);
        ValueSet = other.ValueSet;
        BindingStrength = other.BindingStrength;
        BindingName = other.BindingName;
        _elementTypes = other._elementTypes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        DefaultFieldName = other.DefaultFieldName;
        DefaultFieldValue = other.DefaultFieldValue;
        _slicing = other._slicing.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        FixedFieldName = other.FixedFieldName;
        FixedFieldValue = other.FixedFieldValue;
        PatternFieldName = other.PatternFieldName;
        PatternFieldValue = other.PatternFieldValue;
        _fiveWs = other._fiveWs;
        _conditions = other._conditions.ToHashSet();
        _constraints = other._constraints.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });

        ConformanceExpectation = other.ConformanceExpectation with { };
        ObligationsByActor = other.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(v => v with { }));
    }

    /// <summary>Gets the dot-notation path to the base definition for this record.</summary>
    public required string BasePath { get; init; }

    /// <summary>Gets the root artifact this definition belongs to.</summary>
    public required FhirComplex RootArtifact { get; init; }

    /// <summary>Gets the explicit name of this element, if one was specified.</summary>
    public required string ExplicitName { get; init; }

    /// <summary>Gets the cardinality minimum.</summary>
    public required int CardinalityMin { get; init; }

    /// <summary>Gets the cardinality maximum, -1 for unbounded (e.g., *).</summary>
    /// <value>The cardinality maximum.</value>
    public int CardinalityMax { get => _cardinalityMax; }

    /// <summary>Gets the cardinality maximum string.</summary>
    /// <value>The cardinality maximum string.</value>
    public required string CardinalityMaxString
    {
        get => _cardinalityMax switch
        {
            -1 => "*",
            _ => _cardinalityMax.ToString(),
        };

        init
        {
            if (value == "*")
            {
                _cardinalityMax = -1;
            }
            else if (int.TryParse(value, out int result))
            {
                _cardinalityMax = result;
            }
            else
            {
                throw new ArgumentException($"Invalid cardinality max value: {value}");
            }
        }
    }

    /// <summary>Gets the FHIR cardinality string: min..max.</summary>
    public string FhirCardinality => $"{CardinalityMin}..{CardinalityMaxString}";

    /// <summary>True if this element appears in the differential.</summary>
    public bool InDifferential
    {
        get => _inDifferential;
        set
        {
            _inDifferential = value;
            if (IsInherited)
            {
                _modifiesParent = value;
            }
        }
    }

    /// <summary>Gets a value indicating whether this object is inherited from a parent definition.</summary>
    public required bool IsInherited { get; init; }

    /// <summary>Gets a value indicating whether the modifies parent.</summary>
    public bool ModifiesParent { get => _modifiesParent; init => _modifiesParent = value; }

    /// <summary>Gets a value indicating whether this field hides a parent field.</summary>
    public bool HidesParent { get; init; } = false;

    /// <summary>
    /// Gets whether this element must have a value - for primitives, that a value must be present 
    /// and not replaced by an extension (e.g., a Data Absent Reason).
    /// </summary>
    public bool? MustHaveValue { get; init; } = null;

    /// <summary>
    /// Gets the value alternatives - extensions that are allowed to replace a primitive value.
    /// </summary>
    public IEnumerable<string> ValueAlternatives { get; init; } = Enumerable.Empty<string>();

    /// <summary>Gets a value indicating whether this element is a modifier to the containing structure.</summary>
    public required bool IsModifier { get; init; }

    /// <summary>Gets the is modifier reason.</summary>
    public string IsModifierReason { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether this object is summary.</summary>
    public required bool IsSummary { get; init; }

    /// <summary>Gets a value indicating whether this object is must support.</summary>
    public bool IsMustSupport { get; init;  } = false;

    /// <summary>Gets a value indicating whether this object is simple (no extended properties).</summary>
    public bool IsSimple { get; init; } = false;

    /// <summary>Gets the field order.</summary>
    public required int FieldOrder { get; init; }

    /// <summary>Gets the mappings of this element to another set of definitions.</summary>
    public Dictionary<string, List<FhirElementMapping>> Mappings { get => _mappings; }

    /// <summary>Initializes the FHIR mappings.</summary>
    public IEnumerable<FhirElementMapping> FhirMappings
    {
        init
        {
            // build our dictionary
            value?.ForEach(m =>
            {
                if (!_mappings.ContainsKey(m.Identity))
                {
                    _mappings[m.Identity] = new();
                }

                _mappings[m.Identity].Add(m);
            });

            // if we have a w5 mapping, set the convenience value
            if (_mappings.TryGetValue("w5", out List<FhirElementMapping>? w5maps) &&
                (w5maps?.Any() ?? false))
            {
                _fiveWs = w5maps.First().Map;
            }
        }
    }

    /// <summary>Gets the representation codes.</summary>
    public IEnumerable<PropertyRepresentationCodes> Representations { get => _representations; }

    /// <summary>Gets or initializes the FHIR representations.</summary>
    public IEnumerable<string> RepresentationLiterals
    {
        get => _representationLiterals;
        init
        {
            _representationLiterals = value;
            _representations = value.ToEnum<PropertyRepresentationCodes>();
        }
    }

    /// <summary>Gets the value set this element is bound to.</summary>
    public string ValueSet { get; init; } = string.Empty;

    /// <summary>Gets the binding strength for a value set binding to this element.</summary>
    public string BindingStrength
    {
        get => _bindingStrength;
        init
        {
            _bindingStrength = value;
            _valueSetBindingStrength = value.ToEnum<ElementDefinitionBindingStrength>();
        }
    }

    /// <summary>Gets the element binding strength.</summary>
    public ElementDefinitionBindingStrength? ValueSetBindingStrength => _valueSetBindingStrength;

    /// <summary>Gets the binding name for a value set binding to this element.</summary>
    public string BindingName { get; init; } = string.Empty;

    /// <summary>Gets types and their associated profiles for this element.</summary>
    /// <value>Types and their associated profiles for this element.</value>
    public Dictionary<string, FhirElementType> ElementTypes { get => _elementTypes; init => _elementTypes = value; }

    /// <summary>Gets the name of the default field.</summary>
    public string DefaultFieldName { get; init; } = string.Empty;

    /// <summary>Gets the default field value.</summary>
    public object? DefaultFieldValue { get; init; } = null;

    /// <summary>Gets the slicing information.</summary>
    public Dictionary<string, FhirSlicing> SlicesByName => _slicing;

    public IEnumerable<FhirSlicing> Slices
    {
        get => _slicing.Values;
        init
        {
            _slicing = value?.ToDictionary(s => s.SliceName, s => s) ?? new();
        }
    }

    /// <summary>Gets the name of the fixed field.</summary>
    public string FixedFieldName { get; init; } = string.Empty;

    /// <summary>Gets the fixed field value.</summary>
    public object? FixedFieldValue { get; init; } = null;

    /// <summary>Gets the name of the pattern field.</summary>
    public string PatternFieldName { get; init; } = string.Empty;

    /// <summary>Gets the pattern field value.</summary>
    public object? PatternFieldValue { get; init; } = null;

    /// <summary>Gets a value indicating whether this property is an array.</summary>
    public bool IsArray => (CardinalityMax == -1) || (CardinalityMax > 1);

    /// <summary>Gets a value indicating whether this object is optional.</summary>
    public bool IsOptional => CardinalityMin == 0;

    /// <summary>Gets the 5Ws mapping for this element.</summary>
    public string FiveWs => _fiveWs;

    /// <summary>Gets the conditions - references to invariants about presence.</summary>
    public HashSet<string> Conditions { get => _conditions; }

    /// <summary>Initializes the FHIR conditions - references to invariants about presence.</summary>
    public IEnumerable<string> FhirConditions
    {
        init
        {
            _conditions = value?.ToHashSet() ?? new();
        }
    }

    /// <summary>Gets the constraints - conditions that must evaluate to true.</summary>
    public IEnumerable<FhirConstraint> Constraints
    {
        get => _constraints.Values;
        init
        {
            _constraints = new();
            value?.ForEach(c =>
            {
                if (_constraints.ContainsKey(c.Key))
                {
                    return;
                }

                if (!(RootArtifact?.ConstraintsByKey.ContainsKey(c.Key) ?? false))
                {
                    // TODO: need to sort out what is necessary here
                    //RootArtifact?.AddConstraint(c);
                }

                _constraints[c.Key] = c;
            });
        }
    }

    /// <summary>Gets the constraints (conditions that must evaluate to true) by key.</summary>
    public Dictionary<string, FhirConstraint> ConstraintsByKey { get => _constraints; }

    public bool AddSlice(FhirSlicing slice)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets or initializes the conformance expectation.</summary>
    public FhirExpectation ConformanceExpectation { get; init; } = new();

    /// <summary>Gets the obligations by actor.</summary>
    public Dictionary<string, IEnumerable<FhirObligation>> ObligationsByActor { get; init; } = new();

    /// <summary>Convert this object into a string representation.</summary>
    /// <returns>A string that represents this object.</returns>
    public override string ToString()
    {
        if (!ObligationsByActor.Any())
        {
            return Name;
        }

        return Name + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
