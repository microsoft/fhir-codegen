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
        private Dictionary<string, FhirElementType> _elementTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirElement"/> class.
        /// </summary>
        /// <param name="path">            Full pathname of the file.</param>
        /// <param name="url">             URL of the resource.</param>
        /// <param name="fieldOrder">      The field order.</param>
        /// <param name="shortDescription">Information describing the short.</param>
        /// <param name="definition">      The definition.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        /// <param name="baseTypeName">    Name of the base type.</param>
        /// <param name="elementTypes">    Types and associated profiles.</param>
        /// <param name="cardinalityMin">  The cardinality minimum.</param>
        /// <param name="cardinalityMax">  The cardinaltiy maximum.</param>
        public FhirElement(
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
            string cardinalityMax)
            : base(
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
        }

        /// <summary>Gets the cardinality minimum.</summary>
        ///
        /// <value>The cardinality minimum.</value>
        public int CardinalityMin { get; }

        /// <summary>Gets the cardinaltiy maximum, -1 for unbounded (e.g., *).</summary>
        /// <value>The cardinaltiy maximum.</value>
        public int? CardinalityMax { get; }

        /// <summary>Gets the field order.</summary>
        /// <value>The field order.</value>
        public int FieldOrder { get; }

        /// <summary>Gets or sets Code Values allowed for this property.</summary>
        /// <value>The code values.</value>
        public string CodesName { get; set; }

        /// <summary>Gets types and their associated profiles for this element.</summary>
        /// <value>Types and their associated profiles for this element.</value>
        public Dictionary<string, FhirElementType> ElementTypes { get => _elementTypes; }

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
    }
}
