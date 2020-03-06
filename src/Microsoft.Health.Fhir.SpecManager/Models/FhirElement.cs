// -------------------------------------------------------------------------------------------------
// <copyright file="FhirElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR element.</summary>
    public class FhirElement : FhirTypeBase
    {
        private readonly Dictionary<string, FhirSlicing> _slicing;
        private Dictionary<string, FhirElementType> _elementTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirElement"/> class.
        /// </summary>
        /// <param name="id">               Id for this element.</param>
        /// <param name="path">             Dot notation path for this element.</param>
        /// <param name="url">              URL of this element (if present).</param>
        /// <param name="fieldOrder">       The field order.</param>
        /// <param name="shortDescription"> Information describing the short.</param>
        /// <param name="definition">       The definition.</param>
        /// <param name="comment">          The comment.</param>
        /// <param name="validationRegEx">  The validation RegEx.</param>
        /// <param name="baseTypeName">     Name of the base type.</param>
        /// <param name="elementTypes">     Types and associated profiles.</param>
        /// <param name="cardinalityMin">   The cardinality minimum.</param>
        /// <param name="cardinalityMax">   The cardinaltiy maximum.</param>
        /// <param name="isModifier">       If this element modifies the meaning of its parent.</param>
        /// <param name="isSummary">        If this element should be included in summaries.</param>
        /// <param name="defaultFieldName"> Name of a default field, e.g., defaultUri, defaultCode.</param>
        /// <param name="defaultFieldValue">Value of a default field.</param>
        /// <param name="fixedFieldName">   Name of a fixed field, e.g., fixedUri, fixedCode.</param>
        /// <param name="fixedFieldValue">  Value of a fixed field.</param>
        public FhirElement(
            string id,
            string path,
            Uri url,
            int fieldOrder,
            string shortDescription,
            string definition,
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
            object fixedFieldValue)
            : base(
                id,
                path,
                url,
                string.Empty,
                shortDescription,
                definition,
                comment,
                validationRegEx,
                baseTypeName)
        {
            FieldOrder = fieldOrder;
            _elementTypes = elementTypes;

            CardinalityMin = cardinalityMin;
            CardinalityMax = MaxCardinality(cardinalityMax);

            IsModifier = isModifier == true;
            IsSummary = isSummary == true;

            DefaultFieldName = defaultFieldName;
            DefaultFieldValue = defaultFieldValue;

            _slicing = new Dictionary<string, FhirSlicing>();

            FixedFieldName = fixedFieldName;
            FixedFieldValue = fixedFieldValue;
        }

        /// <summary>Gets the cardinality minimum.</summary>
        ///
        /// <value>The cardinality minimum.</value>
        public int CardinalityMin { get; }

        /// <summary>Gets the cardinaltiy maximum, -1 for unbounded (e.g., *).</summary>
        /// <value>The cardinaltiy maximum.</value>
        public int? CardinalityMax { get; }

        /// <summary>Gets a value indicating whether this object is modifier.</summary>
        /// <value>True if this object is modifier, false if not.</value>
        public bool IsModifier { get; }

        /// <summary>Gets a value indicating whether this object is summary.</summary>
        /// <value>True if this object is summary, false if not.</value>
        public bool IsSummary { get; }

        /// <summary>Gets the field order.</summary>
        /// <value>The field order.</value>
        public int FieldOrder { get; }

        /// <summary>Gets or sets Code Values allowed for this property.</summary>
        /// <value>The code values.</value>
        public string CodesName { get; set; }

        /// <summary>Gets types and their associated profiles for this element.</summary>
        /// <value>Types and their associated profiles for this element.</value>
        public Dictionary<string, FhirElementType> ElementTypes { get => _elementTypes; }

        /// <summary>Gets the name of the default field.</summary>
        /// <value>The name of the default field.</value>
        public string DefaultFieldName { get; }

        /// <summary>Gets the default field value.</summary>
        /// <value>The default field value.</value>
        public object DefaultFieldValue { get; }

        /// <summary>Gets the slicing information</summary>
        /// <value>The slicing.</value>
        public Dictionary<string, FhirSlicing> Slicing => _slicing;

        /// <summary>Gets the name of the fixed field.</summary>
        /// <value>The name of the fixed field.</value>
        public string FixedFieldName { get; }

        /// <summary>Gets the fixed field value.</summary>
        /// <value>The fixed field value.</value>
        public object FixedFieldValue { get; }

        /// <summary>Gets a value indicating whether this property is an array.</summary>
        /// <value>True if this object is array, false if not.</value>
        public bool IsArray => (CardinalityMax == -1) || (CardinalityMax > 1);

        /// <summary>Gets a value indicating whether this object is optional.</summary>
        /// <value>True if this object is optional, false if not.</value>
        public bool IsOptional => CardinalityMin == 0;

        /// <summary>Maximum cardinality.</summary>
        /// <param name="max">The maximum.</param>
        /// <returns>Null for unbounded cardinality, value for a specific maximum.</returns>
        private static int? MaxCardinality(string max)
        {
            if (string.IsNullOrEmpty(max))
            {
                return null;
            }

            if (max.Equals("*", StringComparison.Ordinal))
            {
                return null;
            }

            if (int.TryParse(max, out int parsed))
            {
                return parsed;
            }

            return null;
        }

        /// <summary>Adds a slicing.</summary>
        /// <param name="slicing">Slicing information for this element, if present.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
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
            if (_slicing[url].Slices.ContainsKey(sliceName))
            {
                return false;
            }

            // create a new complex type from the property
            _slicing[url].AddSlice(
                sliceName,
                new FhirComplex(
                    Id,
                    Path,
                    URL,
                    StandardStatus,
                    ShortDescription,
                    Purpose,
                    Comment,
                    ValidationRegEx,
                    BaseTypeName));

            return true;
        }
    }
}
