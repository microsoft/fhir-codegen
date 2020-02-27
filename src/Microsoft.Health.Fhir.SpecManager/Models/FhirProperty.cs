// -------------------------------------------------------------------------------------------------
// <copyright file="FhirProperty.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR property (in a complex-type or resource).</summary>
    public class FhirProperty : FhirTypeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirProperty"/> class.
        /// </summary>
        /// <param name="path">            Full pathname of the file.</param>
        /// <param name="fieldOrder">      The field order.</param>
        /// <param name="shortDescription">Information describing the short.</param>
        /// <param name="definition">      The definition.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        /// <param name="baseTypeName">    Name of the base type.</param>
        /// <param name="expandedTypes">   A list of types of the expanded.</param>
        /// <param name="cardinalityMin">  The cardinality minimum.</param>
        /// <param name="cardinalityMax">  The cardinaltiy maximum.</param>
        /// <param name="targetProfiles">  Target profiles valid for this type</param>
        public FhirProperty(
            string path,
            int fieldOrder,
            string shortDescription,
            string definition,
            string comment,
            string validationRegEx,
            string baseTypeName,
            HashSet<string> expandedTypes,
            int cardinalityMin,
            string cardinalityMax,
            string[] targetProfiles)
            : base(
                path,
                string.Empty,
                shortDescription,
                definition,
                comment,
                validationRegEx,
                baseTypeName)
        {
            FieldOrder = fieldOrder;
            ChoiceTypes = expandedTypes;
            CardinalityMin = cardinalityMin;
            CardinalityMax = MaxCardinality(cardinalityMax);

            // process target profiles
            TargetProfiles = new HashSet<string>();
            if (targetProfiles != null)
            {
                foreach (string targetProfile in targetProfiles)
                {
                    TargetProfiles.Add(targetProfile.Substring(targetProfile.LastIndexOf('/') + 1));
                }
            }
        }

        /// <summary>Gets the cardinality minimum.</summary>
        ///
        /// <value>The cardinality minimum.</value>
        public int CardinalityMin { get; }

        /// <summary>Gets the cardinaltiy maximum, -1 for unbounded (e.g., *).</summary>
        ///
        /// <value>The cardinaltiy maximum.</value>
        public int? CardinalityMax { get; }

        /// <summary>Gets the field order.</summary>
        /// <value>The field order.</value>
        public int FieldOrder { get; }

        /// <summary>Gets or sets Code Values allowed for this property.</summary>
        ///
        /// <value>The code values.</value>
        public string CodesName { get; set; }

        /// <summary>Gets or sets URL of the value set.</summary>
        ///
        /// <value>The value set URL.</value>
        public string ValueSetUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is inherited.</summary>
        ///
        /// <value>True if this object is inherited, false if not.</value>
        public bool IsInherited { get; set; }

        /// <summary>Gets a list of choice types for this property.</summary>
        ///
        /// <value>A list of types allowed for this property.</value>
        public HashSet<string> ChoiceTypes { get; }

        /// <summary>Gets target profiles.</summary>
        /// <value>The target profiles.</value>
        public HashSet<string> TargetProfiles { get; }

        /// <summary>Gets a value indicating whether this property is an array.</summary>
        ///
        /// <value>True if this object is array, false if not.</value>
        public bool IsArray
        {
            get
            {
                return (CardinalityMax == -1) || (CardinalityMax > 1);
            }
        }

        /// <summary>Gets a value indicating whether this object is optional.</summary>
        ///
        /// <value>True if this object is optional, false if not.</value>
        public bool IsOptional
        {
            get
            {
                return CardinalityMin == 0;
            }
        }

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
