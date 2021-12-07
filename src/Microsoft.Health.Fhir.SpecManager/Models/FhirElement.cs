// -------------------------------------------------------------------------------------------------
// <copyright file="FhirElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Extensions;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR element.</summary>
    public class FhirElement : FhirTypeBase
    {
        private readonly Dictionary<string, FhirSlicing> _slicing;
        private Dictionary<string, FhirElementType> _elementTypes;
        private List<string> _codes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirElement"/> class.
        /// </summary>
        /// <param name="id">               Id for this element.</param>
        /// <param name="path">             Dot notation path for this element.</param>
        /// <param name="explicitName">     Explicit name of this element, if present.</param>
        /// <param name="url">              URL of this element (if present).</param>
        /// <param name="fieldOrder">       The field order.</param>
        /// <param name="shortDescription"> Information describing the short.</param>
        /// <param name="purpose">       The definition.</param>
        /// <param name="comment">          The comment.</param>
        /// <param name="validationRegEx">  The validation RegEx.</param>
        /// <param name="baseTypeName">     Name of the base type.</param>
        /// <param name="elementTypes">     Types and associated profiles.</param>
        /// <param name="cardinalityMin">   The cardinality minimum.</param>
        /// <param name="cardinalityMax">   The cardinality maximum.</param>
        /// <param name="isModifier">       If this element modifies the meaning of its parent.</param>
        /// <param name="isSummary">        If this element should be included in summaries.</param>
        /// <param name="defaultFieldName"> Name of a default field, e.g., defaultUri, defaultCode.</param>
        /// <param name="defaultFieldValue">Value of a default field.</param>
        /// <param name="fixedFieldName">   Name of a fixed field, e.g., fixedUri, fixedCode.</param>
        /// <param name="fixedFieldValue">  Value of a fixed field.</param>
        /// <param name="isInherited">      If this element is inherited from somewhere else.</param>
        /// <param name="modifiesParent">   If this element hides a field of its parent.</param>
        /// <param name="bindingStrength">  Strength of binding: required|extensible|preferred|example.</param>
        /// <param name="valueSet">         URL of the value set bound to this element.</param>
        public FhirElement(
            string id,
            string path,
            string explicitName,
            Uri url,
            int fieldOrder,
            string shortDescription,
            string purpose,
            string comment,
            string validationRegEx,
            string baseTypeName,
            Dictionary<string, FhirElementType> elementTypes,
            int cardinalityMin,
            string cardinalityMax,
            bool? isModifier,
            bool? isSummary,
            string defaultFieldName,
            object defaultFieldValue,
            string fixedFieldName,
            object fixedFieldValue,
            bool isInherited,
            bool modifiesParent,
            string bindingStrength,
            string valueSet,
            List<string> fWmapping)
            : base(
                id,
                path,
                url,
                string.Empty,
                false,
                shortDescription,
                purpose,
                comment,
                validationRegEx,
                baseTypeName)
        {
            FieldOrder = fieldOrder;
            _elementTypes = elementTypes;

            _codes = null;
            if ((baseTypeName == "code") ||
                ((elementTypes != null) && elementTypes.Any((e) => { return e.Key == "code"; })))
            {
                if (shortDescription == "formats supported (xml | json | mime type)")
                {
                    _codes = new List<string>()
                    {
                        "xml",
                        "json",
                        "MIME Type",
                    };
                }
                else if (shortDescription == "formats supported (xml | json | ttl | mime type)")
                {
                    _codes = new List<string>()
                    {
                        "xml",
                        "json",
                        "ttl",
                        "MIME Type",
                    };
                }
                else if ((!string.IsNullOrEmpty(shortDescription)) &&
                    shortDescription.Contains('|', StringComparison.Ordinal))
                {
                    _codes = new List<string>();
                    string[] codeValues = shortDescription.Split('|');
                    foreach (string code in codeValues)
                    {
                        string clean = code.Trim();
                        if (clean.Contains(" ", StringComparison.Ordinal))
                        {
                            clean = clean.Substring(0, clean.IndexOf(" ", StringComparison.Ordinal));
                        }

                        _codes.Add(clean.Trim());
                    }
                }
            }

            ExplicitName = explicitName;

            CardinalityMin = cardinalityMin;
            CardinalityMax = MaxCardinality(cardinalityMax);

            if (cardinalityMax == "0")
            {
                HidesParent = true;
            }
            else
            {
                HidesParent = false;
            }

            IsModifier = isModifier == true;
            IsSummary = (isSummary == true) || IsModifier;

            DefaultFieldName = defaultFieldName;
            DefaultFieldValue = defaultFieldValue;

            _slicing = new Dictionary<string, FhirSlicing>();

            FixedFieldName = fixedFieldName;
            FixedFieldValue = fixedFieldValue;

            IsInherited = isInherited;
            ModifiesParent = modifiesParent;

            BindingStrength = bindingStrength;
            ValueSet = valueSet;

            if (string.IsNullOrEmpty(bindingStrength))
            {
                ValueSetBindingStrength = null;
            }
            else
            {
                ValueSetBindingStrength = bindingStrength.ToFhirEnum<ElementDefinitionBindingStrength>();
            }

            FiveWs = fWmapping;
        }

        /// <summary>Values that represent element definition binding strengths.</summary>
        public enum ElementDefinitionBindingStrength : int
        {
            /// <summary>To be conformant, the concept in this element SHALL be from the specified value set.</summary>
            [FhirLiteral("required")]
            Required = 1,

            /// <summary>To be conformant, the concept in this element SHALL be from the specified value set if any of the codes within the value set can apply to the concept being communicated. If the value set does not cover the concept (based on human review), alternate codings (or, data type allowing, text) may be included instead.</summary>
            [FhirLiteral("extensible")]
            Extensible,

            /// <summary>Instances are encouraged to draw from the specified codes for interoperability purposes but are not required to do so to be considered conformant.</summary>
            [FhirLiteral("preferred")]
            Preferred,

            /// <summary>Instances are not expected or even encouraged to draw from the specified value set. The value set merely provides examples of the types of concepts intended to be included.</summary>
            [FhirLiteral("example")]
            Example,
        }

        /// <summary>Gets the explicit name of this element, if one was specified.</summary>
        public string ExplicitName { get; }

        /// <summary>Gets the cardinality minimum.</summary>
        ///
        /// <value>The cardinality minimum.</value>
        public int CardinalityMin { get; }

        /// <summary>Gets the cardinality maximum, -1 for unbounded (e.g., *).</summary>
        /// <value>The cardinality maximum.</value>
        public int CardinalityMax { get; }

        /// <summary>Gets the cardinality maximum string.</summary>
        /// <value>The cardinality maximum string.</value>
        public string CardinalityMaxString
        {
            get
            {
                if (CardinalityMax == -1)
                {
                    return "*";
                }

                return $"{CardinalityMax}";
            }
        }

        /// <summary>Gets the FHIR cardinality string: min..max.</summary>
        public string FhirCardinality => $"{CardinalityMin}..{CardinalityMaxString}";

        /// <summary>Gets a value indicating whether this object is inherited.</summary>
        public bool IsInherited { get; }

        /// <summary>Gets a value indicating whether the modifies parent.</summary>
        public bool ModifiesParent { get; }

        /// <summary>Gets a value indicating whether this field hides a parent field.</summary>
        public bool HidesParent { get; }

        /// <summary>Gets a value indicating whether this object is modifier.</summary>
        public bool IsModifier { get; }

        /// <summary>Gets a value indicating whether this object is summary.</summary>
        public bool IsSummary { get; }

        /// <summary>Gets the field order.</summary>
        public int FieldOrder { get; }

        /// <summary>Gets or sets Code Values allowed for this property.</summary>
        public string CodesName { get; set; }

        /// <summary>Gets the codes.</summary>
        public List<string> Codes => _codes;

        /// <summary>Gets the value set this element is bound to.</summary>
        public string ValueSet { get; }

        /// <summary>Gets the binding strength for a value set binding to this element.</summary>
        public string BindingStrength { get; }

        /// <summary>Gets the element binding strength.</summary>
        public ElementDefinitionBindingStrength? ValueSetBindingStrength { get; }

        /// <summary>Gets types and their associated profiles for this element.</summary>
        /// <value>Types and their associated profiles for this element.</value>
        public Dictionary<string, FhirElementType> ElementTypes { get => _elementTypes; }

        /// <summary>Gets the name of the default field.</summary>
        public string DefaultFieldName { get; }

        /// <summary>Gets the default field value.</summary>
        public object DefaultFieldValue { get; }

        /// <summary>Gets the slicing information.</summary>
        public Dictionary<string, FhirSlicing> Slicing => _slicing;

        /// <summary>Gets the name of the fixed field.</summary>
        public string FixedFieldName { get; }

        /// <summary>Gets the fixed field value.</summary>
        public object FixedFieldValue { get; }

        /// <summary>Gets a value indicating whether this property is an array.</summary>
        public bool IsArray => (CardinalityMax == -1) || (CardinalityMax > 1);

        /// <summary>Gets a value indicating whether this object is optional.</summary>
        public bool IsOptional => CardinalityMin == 0;

        public List<string> FiveWs {get;}

        /// <summary>Maximum cardinality.</summary>
        /// <param name="max">The maximum.</param>
        /// <returns>-1 for unbounded cardinality, value for a specific maximum.</returns>
        private static int MaxCardinality(string max)
        {
            if (string.IsNullOrEmpty(max))
            {
                return -1;
            }

            if (max.Equals("*", StringComparison.Ordinal))
            {
                return -1;
            }

            if (int.TryParse(max, out int parsed))
            {
                return parsed;
            }

            return -1;
        }

        /// <summary>Adds a slicing.</summary>
        /// <param name="slicing">Slicing information for this element, if present.</param>
        internal void AddSlicing(FhirSlicing slicing)
        {
            string url = slicing.DefinedByUrl.ToString();

            if (!_slicing.ContainsKey(url))
            {
                _slicing.Add(url, slicing);
            }
        }

        /// <summary>Adds a component from an element.</summary>
        /// <param name="url">      URL of this element (if present).</param>
        /// <param name="sliceName">Name of the element.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        internal bool AddSlice(string url, string sliceName)
        {
            if (!_slicing.ContainsKey(url))
            {
                return false;
            }

            if (_slicing[url].HasSlice(sliceName))
            {
                return false;
            }

            FhirComplex slice = new FhirComplex(
                    Id,
                    Path,
                    ExplicitName,
                    URL,
                    StandardStatus,
                    false,
                    ShortDescription,
                    Purpose,
                    Comment,
                    ValidationRegEx,
                    BaseTypeName);
            slice.SliceName = sliceName;

            // create a new complex type from the property
            _slicing[url].AddSlice(
                sliceName,
                slice);

            return true;
        }

        /// <summary>Deep copy.</summary>
        /// <param name="primitiveTypeMap">   The primitive type map.</param>
        /// <param name="copySlicing">        True to copy slicing.</param>
        /// <param name="canHideParentFields">True if can hide parent fields, false if not.</param>
        /// <param name="valueSetReferences"> [in,out] Value Set URLs and lists of FHIR paths that
        ///  reference them.</param>
        /// <returns>A FhirElement.</returns>
        public FhirElement DeepCopy(
            Dictionary<string, string> primitiveTypeMap,
            bool copySlicing,
            bool canHideParentFields,
            Dictionary<string, ValueSetReferenceInfo> valueSetReferences)
        {
            // copy the element types
            Dictionary<string, FhirElementType> elementTypes = null;

            if (_elementTypes != null)
            {
                elementTypes = new Dictionary<string, FhirElementType>();

                foreach (KeyValuePair<string, FhirElementType> kvp in _elementTypes)
                {
                    elementTypes.Add(kvp.Key, kvp.Value.DeepCopy(primitiveTypeMap));
                }
            }

            // generate our copy
            FhirElement element = new FhirElement(
                Id,
                Path,
                ExplicitName,
                URL,
                FieldOrder,
                ShortDescription,
                Purpose,
                Comment,
                ValidationRegEx,
                BaseTypeName,
                elementTypes,
                CardinalityMin,
                CardinalityMax == -1 ? "*" : $"{CardinalityMax}",
                IsModifier,
                IsSummary,
                DefaultFieldName,
                DefaultFieldValue,
                FixedFieldName,
                FixedFieldValue,
                IsInherited,
                ModifiesParent,
                BindingStrength,
                ValueSet,
                FiveWs);

            // check for base type name
            if (!string.IsNullOrEmpty(BaseTypeName))
            {
                if ((primitiveTypeMap != null) && primitiveTypeMap.ContainsKey(BaseTypeName))
                {
                    element.BaseTypeName = primitiveTypeMap[BaseTypeName];
                }
                else
                {
                    element.BaseTypeName = BaseTypeName;
                }
            }

            // add slices
            if (copySlicing)
            {
                foreach (KeyValuePair<string, FhirSlicing> kvp in _slicing)
                {
                    FhirSlicing slicing = kvp.Value.DeepCopy(
                            primitiveTypeMap,
                            copySlicing,
                            canHideParentFields,
                            valueSetReferences);

                    element.AddSlicing(slicing);
                }
            }

            // check for referenced value sets
            if ((!IsInherited) &&
                (valueSetReferences != null) &&
                (!string.IsNullOrEmpty(ValueSet)))
            {
                string url;
                int barIndex = ValueSet.IndexOf('|');

                if (barIndex > 0)
                {
                    url = ValueSet.Substring(0, barIndex);
                }
                else
                {
                    url = ValueSet;
                }

                if (!valueSetReferences.ContainsKey(url))
                {
                    valueSetReferences.Add(url, new ValueSetReferenceInfo());
                }

                valueSetReferences[url].AddPath(Path, ValueSetBindingStrength);
            }

            return element;
        }

        /// <summary>Names and types for export.</summary>
        /// <param name="nameConvention">        The name convention.</param>
        /// <param name="typeConvention">        The convention.</param>
        /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
        /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
        /// <param name="isComponent">           (Optional) True if is component, false if not.</param>
        /// <returns>A Dictionary of field names (e.g., ValueBoolean) and types (e.g., boolean).</returns>
        public Dictionary<string, string> NamesAndTypesForExport(
            NamingConvention nameConvention,
            NamingConvention typeConvention,
            bool concatenatePath = false,
            string concatenationDelimiter = "",
            bool isComponent = false)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            string baseName = Name;
            bool isChoice = false;

            if (isComponent)
            {
                values.Add(
                    FhirUtils.ToConvention(Name, Path, nameConvention, concatenatePath, concatenationDelimiter),
                    FhirUtils.ToConvention(Path, string.Empty, typeConvention));

                return values;
            }

            if ((_elementTypes != null) && (_elementTypes.Count > 0))
            {
                if (baseName.Contains("[x]"))
                {
                    baseName = baseName.Replace("[x]", string.Empty);
                    isChoice = true;
                }

                if (isChoice)
                {
                    foreach (FhirElementType elementType in _elementTypes.Values)
                    {
                        string name = FhirUtils.ToConvention(baseName, Path, nameConvention, concatenatePath, concatenationDelimiter);
                        string type = FhirUtils.ToConvention(elementType.Name, string.Empty, typeConvention);

                        string combined = $"{name}{type}";

                        if (!values.ContainsKey(combined))
                        {
                            values.Add(combined, elementType.Type);
                        }
                    }
                }
                else
                {
                    string types = string.Empty;

                    foreach (FhirElementType elementType in _elementTypes.Values)
                    {
                        string type = elementType.Type;

                        if (string.IsNullOrEmpty(types))
                        {
                            types = type;
                        }
                        else
                        {
                            types = $"{types}|{type}";
                        }
                    }

                    string cased = FhirUtils.ToConvention(baseName, string.Empty, nameConvention, concatenatePath, concatenationDelimiter);

                    if (!values.ContainsKey(cased))
                    {
                        values.Add(cased, types);
                    }
                }

                return values;
            }

            values.Add(
                FhirUtils.ToConvention(Name, Path, nameConvention, concatenatePath, concatenationDelimiter),
                FhirUtils.ToConvention(BaseTypeName, string.Empty, typeConvention));

            return values;
        }
    }
}
