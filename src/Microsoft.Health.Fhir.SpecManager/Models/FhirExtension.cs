// <copyright file="FhirExtension.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR extension.</summary>
    public class FhirExtension
    {
        /// <summary>Initializes a new instance of the <see cref="FhirExtension"/> class.</summary>
        /// <param name="name">             The name.</param>
        /// <param name="id">               The identifier.</param>
        /// <param name="url">              The URL.</param>
        /// <param name="elementPaths">     The full pathname of the element file.</param>
        /// <param name="allowedValueTypes">A list of types of the allowed values.</param>
        /// <param name="isModifier">       True if this object is modifier, false if not.</param>
        /// <param name="isSummary">        True if this object is summary, false if not.</param>
        public FhirExtension(
            string name,
            string id,
            Uri url,
            List<string> elementPaths,
            List<string> allowedValueTypes,
            bool isModifier,
            bool isSummary)
        {
            Name = name;
            Id = id;
            URL = url;
            ElementPaths = elementPaths;
            AllowedValueTypes = allowedValueTypes;
            IsModifier = isModifier;
            IsSummary = isSummary;
        }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>Gets the full pathname of the element file.</summary>
        /// <value>The full pathname of the element file.</value>
        public List<string> ElementPaths { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public Uri URL { get; }

        /// <summary>Gets a list of types of the allowed values.</summary>
        /// <value>A list of types of the allowed values.</value>
        public List<string> AllowedValueTypes { get; }

        /// <summary>Gets a value indicating whether this object is modifier.</summary>
        /// <value>True if this object is modifier, false if not.</value>
        public bool IsModifier { get; }

        /// <summary>Gets a value indicating whether this object is summary.</summary>
        /// <value>True if this object is summary, false if not.</value>
        public bool IsSummary { get; }
    }
}
