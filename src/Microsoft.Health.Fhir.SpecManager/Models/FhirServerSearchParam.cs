// <copyright file="FhirServerSearchParam.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Extensions;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR server search parameter.</summary>
    public class FhirServerSearchParam
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirServerSearchParam"/> class.
        /// </summary>
        /// <param name="name">               The name.</param>
        /// <param name="definitionCanonical">The definition canonical.</param>
        /// <param name="parameterType">      The type of the parameter.</param>
        /// <param name="documentation">      The documentation.</param>
        public FhirServerSearchParam(
            string name,
            string definitionCanonical,
            SearchParameterType parameterType,
            string documentation)
        {
            Name = name;
            DefinitionCanonical = definitionCanonical;
            ParameterType = parameterType;
            Documentation = documentation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirServerSearchParam"/> class.
        /// </summary>
        /// <param name="name">               The name.</param>
        /// <param name="definitionCanonical">The definition canonical.</param>
        /// <param name="parameterType">      The type of the parameter.</param>
        /// <param name="documentation">      The documentation.</param>
        public FhirServerSearchParam(
            string name,
            string definitionCanonical,
            string parameterType,
            string documentation)
        {
            Name = name;
            DefinitionCanonical = definitionCanonical;
            ParameterType = parameterType.ToFhirEnum<SearchParameterType>();
            Documentation = documentation;
        }

        /// <summary>
        /// Values that represent the type of value a search parameter refers to, and how the content is
        /// interpreted.
        /// </summary>
        public enum SearchParameterType
        {
            /// <summary>Search parameter SHALL be a number (a whole number, or a decimal).</summary>
            [FhirLiteral("number")]
            Number,

            /// <summary>Search parameter is on a date/time. The date format is the standard XML format, though other formats may be supported.</summary>
            [FhirLiteral("date")]
            Date,

            /// <summary>Search parameter is a simple string, like a name part. Search is case-insensitive and accent-insensitive. May match just the start of a string. String parameters may contain spaces.</summary>
            [FhirLiteral("string")]
#pragma warning disable CA1720 // Identifier contains type name
            String,
#pragma warning restore CA1720 // Identifier contains type name

            /// <summary>Search parameter on a coded element or identifier. May be used to search through the text, display, code and code/codesystem (for codes) and label, system and key (for identifier). Its value is either a string or a pair of namespace and value, separated by a "|", depending on the modifier used.</summary>
            [FhirLiteral("token")]
            Token,

            /// <summary>A reference to another resource (Reference or canonical).</summary>
            [FhirLiteral("reference")]
            Reference,

            /// <summary>A composite search parameter that combines a search on two values together.</summary>
            [FhirLiteral("composite")]
            Composite,

            /// <summary>A search parameter that searches on a quantity.</summary>
            [FhirLiteral("quantity")]
            Quantity,

            /// <summary>A search parameter that searches on a URI (RFC 3986).</summary>
            [FhirLiteral("uri")]
            Uri,

            /// <summary>Special logic applies to this parameter per the description of the search parameter.</summary>
            [FhirLiteral("special")]
            Special,
        }

        /// <summary>Gets the name.</summary>
        public string Name { get; }

        /// <summary>Gets the definition canonical.</summary>
        public string DefinitionCanonical { get; }

        /// <summary>Gets the type of the parameter.</summary>
        public SearchParameterType ParameterType { get; }

        /// <summary>Gets the documentation.</summary>
        public string Documentation { get; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        public object Clone()
        {
            return new FhirServerSearchParam(
                Name,
                DefinitionCanonical,
                ParameterType,
                Documentation);
        }
    }
}
