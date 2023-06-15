// <copyright file="FhirElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirElement;
using System.Reflection.Emit;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Refactor;

/// <summary>A FHIR element definition.</summary>
public record class FhirElement : FhirDefinitionBase
{
    private int _cardinalityMax = -1;
    private Dictionary<string, FhirElementType> _elementTypes = new();
    private Dictionary<string, List<FhirElementMapping>> _mappings = new();
    private List<PropertyRepresentationCodes> _representations = new();
    private string _bindingStrength = string.Empty;
    private ElementDefinitionBindingStrength? _valueSetBindingStrength = null;

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

    /// <summary>Gets the dot-notation path to the base definition for this record.</summary>
    public required string BasePath { get; init; }

    /// <summary>Gets the root artifact.</summary>
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

    /// <summary>Gets a value indicating whether this object is inherited.</summary>
    public bool IsInherited { get; init; } = false;

    /// <summary>Gets a value indicating whether the modifies parent.</summary>
    public bool ModifiesParent { get; init; } = false;

    /// <summary>Gets a value indicating whether this field hides a parent field.</summary>
    public bool HidesParent { get; init; } = false;

    /// <summary>Gets a value indicating whether this object is modifier.</summary>
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
    public Dictionary<string, List<FhirElementMapping>> Mappings { get => _mappings; init => _mappings = value; }

    /// <summary>Initializes the FHIR mappings.</summary>
    public IEnumerable<FhirElementMapping> FhirMappings
    {
        init
        {
            foreach (FhirElementMapping m in value ?? Enumerable.Empty<FhirElementMapping>())
            {
                if (!_mappings.ContainsKey(m.Identity))
                {
                    _mappings[m.Identity] = new();
                }

                _mappings[m.Identity].Add(m);
            }
        }
    }

    /// <summary>Gets the representation codes.</summary>
    public IEnumerable<PropertyRepresentationCodes> Representations { get => _representations; }

    /// <summary>Gets or initializes the FHIR representations.</summary>
    public IEnumerable<string> FhirRepresentations
    {
        init
        {
            _representations = value?.ToFhirEnumList<PropertyRepresentationCodes>() ?? new();
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
            if (_bindingStrength.TryFhirEnum(out ElementDefinitionBindingStrength edbs))
            {
                _valueSetBindingStrength = edbs;
            }
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
    public Dictionary<string, FhirSlicing> Slicing => _slicing;

    /// <summary>Gets the name of the fixed field.</summary>
    public string FixedFieldName { get; }

    /// <summary>Gets the fixed field value.</summary>
    public object FixedFieldValue { get; }

    /// <summary>Gets the name of the pattern field.</summary>
    public string PatternFieldName { get; }

    /// <summary>Gets the pattern field value.</summary>
    public object PatternFieldValue { get; }

    /// <summary>Gets a value indicating whether this property is an array.</summary>
    public bool IsArray => (CardinalityMax == -1) || (CardinalityMax > 1);

    /// <summary>Gets a value indicating whether this object is optional.</summary>
    public bool IsOptional => CardinalityMin == 0;

    /// <summary>Gets the five Ws mapping list for the current element.</summary>
    public string FiveWs => _fiveWs;

    /// <summary>True if this element appears in the differential.</summary>
    public bool InDifferential => _inDifferential;

    /// <summary>Gets the conditions.</summary>
    public HashSet<string> Conditions => _conditions;

    /// <summary>Gets the constraints.</summary>
    public IEnumerable<FhirConstraint> Constraints { get => _constraintsByKey.Values; }

    /// <summary>Gets the constraints by key.</summary>
    public Dictionary<string, FhirConstraint> ConstraintsByKey { get => _constraintsByKey; }

    /// <summary>
    /// Process this element after creation. Tests for additional processing that requires values to
    /// already be present, e.g., parsing a short description into codes for a coded element.
    /// </summary>
    public void Process()
    {
        //CheckForCodes();
    }

    ///// <summary>Check for codes.</summary>
    //private void CheckForCodes()
    //{
    //    if ((!_baseTypeName.Equals("code", StringComparison.Ordinal)) &&
    //        (!_elementTypes.Values.Where(et => et.Type.Equals("code", StringComparison.Ordinal)).Any()))
    //    {
    //        return;
    //    }

    //    if (ShortDescription == "formats supported (xml | json | mime type)")
    //    {
    //        _codes = new string[]
    //            {
    //                "xml",
    //                "json",
    //                "MIME Type",
    //            };
    //    }
    //    else if (ShortDescription == "formats supported (xml | json | ttl | mime type)")
    //    {
    //        _codes = new string[]
    //            {
    //                "xml",
    //                "json",
    //                "ttl",
    //                "MIME Type",
    //            };
    //    }
    //    else if (ShortDescription.Contains('|', StringComparison.Ordinal))
    //    {
    //        _codes = ShortDescription
    //            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    //            .Select(s => s.Contains(' ', StringComparison.Ordinal) ? s.Split(' ').First() : s)
    //            .ToArray();
    //    }
    //}
}
