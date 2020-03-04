﻿// -------------------------------------------------------------------------------------------------
// <copyright file="FhirTypeBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>
    /// A base class for FHIR types to inherit from (common properties).
    /// </summary>
    public class FhirTypeBase
    {
        /// <summary>The extensions.</summary>
        private Dictionary<string, FhirExtension> _extensions;

        /// <summary>Initializes a new instance of the <see cref="FhirTypeBase"/> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="path">            The full pathname of the file.</param>
        /// <param name="url">             The URL.</param>
        /// <param name="standardStatus">  The standard status.</param>
        /// <param name="shortDescription">The description.</param>
        /// <param name="purpose">         The purpose of this definition.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        internal FhirTypeBase(
            string path,
            Uri url,
            string standardStatus,
            string shortDescription,
            string purpose,
            string comment,
            string validationRegEx)
        {
            // sanity checks
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            // set internal values
            Path = path;
            StandardStatus = standardStatus;
            ShortDescription = shortDescription;
            Purpose = purpose;
            Comment = comment;
            ValidationRegEx = validationRegEx;
            URL = url;

            // check for components in the path
            string[] components = path.Split('.');
            Name = components[components.Length - 1];
            NameCapitalized = Capitalize(Name);

            _extensions = new Dictionary<string, FhirExtension>();
        }

        /// <summary>Initializes a new instance of the <see cref="FhirTypeBase"/> class.</summary>
        /// <param name="path">            The full pathname of the file.</param>
        /// <param name="url">             The URL.</param>
        /// <param name="standardStatus">  The standard status.</param>
        /// <param name="shortDescription">The description.</param>
        /// <param name="purpose">         The purpose of this definition.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        /// <param name="baseTypeName">    The name of the base type.</param>
        internal FhirTypeBase(
            string path,
            Uri url,
            string standardStatus,
            string shortDescription,
            string purpose,
            string comment,
            string validationRegEx,
            string baseTypeName)
            : this(
                path,
                url,
                standardStatus,
                shortDescription,
                purpose,
                comment,
                validationRegEx)
        {
            BaseTypeName = baseTypeName;
        }

        /// <summary>Gets the full path to this object.</summary>
        /// <value>The full pathname of the file.</value>
        public string Path { get; }

        /// <summary>
        /// Gets a natural language name identifying the structure definition. This name should be usable as an
        /// identifier for the module by machine processing applications such as code generation.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets name field with the first letter capitalized, useful in various languages and PascalCase joining.
        /// </summary>
        /// <value>The name capitalized.</value>
        public string NameCapitalized { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public Uri URL { get; }

        /// <summary>
        /// Gets status of this type in the standards process - use FhirCommon.StandardStatusCodes
        /// see: http://hl7.org/fhir/valueset-standards-status.html.
        /// </summary>
        /// <value>The standard status.</value>
        public string StandardStatus { get; }

        /// <summary>Gets or sets the Name of the type this type inherits from (null if none).</summary>
        /// <value>The name of the base type.</value>
        public string BaseTypeName { get; set; }

        /// <summary>
        /// Gets a concise description of what this element means (e.g. for use in autogenerated summaries).
        /// </summary>
        /// <value>The description.</value>
        public string ShortDescription { get; }

        /// <summary>
        /// Gets a complete explanation of the meaning of the data element for human readability.  For
        /// the case of elements derived from existing elements (e.g. constraints), the definition SHALL be
        /// consistent with the base definition, but convey the meaning of the element in the particular
        /// context of use of the resource. (Note: The text you are reading is specified in
        /// ElementDefinition.definition).
        /// </summary>
        /// <value>The definition.</value>
        public string Purpose { get; }

        /// <summary>
        /// Gets explanatory notes and implementation guidance about the data element, including notes about how
        /// to use the data properly, exceptions to proper use, etc. (Note: The text you are reading is
        /// specified in ElementDefinition.comment).
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; }

        /// <summary>
        /// Gets a RegEx string used to validate values of a type or property.
        /// </summary>
        /// <value>The validation RegEx.</value>
        public string ValidationRegEx { get; }

        /// <summary>Gets the extensions.</summary>
        /// <value>The extensions.</value>
        public Dictionary<string, FhirExtension> Extensions { get => _extensions; }

        /// <summary>Capitalizes a name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>A string.</returns>
        private static string Capitalize(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            return string.Concat(name.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture), name.Substring(1));
        }

        /// <summary>Adds an extension.</summary>
        /// <param name="extension">The extension.</param>
        internal void AddExtension(FhirExtension extension)
        {
            string url = extension.URL.ToString();

            if (!_extensions.ContainsKey(url))
            {
                _extensions.Add(url, extension);
            }
        }
    }
}
