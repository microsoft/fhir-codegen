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
    public class FhirExtension : FhirTypeBase
    {
        private Dictionary<string, FhirElement> _properties;

        /// <summary>Initializes a new instance of the <see cref="FhirExtension"/> class.</summary>
        /// <param name="name">            The name.</param>
        /// <param name="id">              The identifier.</param>
        /// <param name="url">             The URL.</param>
        /// <param name="standardStatus">  The standard status.</param>
        /// <param name="shortDescription">Information describing the short.</param>
        /// <param name="definition">      The definition.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="elementPaths">    The element paths where this extension is valid.</param>
        /// <param name="isModifier">      True if this object is modifier, false if not.</param>
        /// <param name="isSummary">       True if this object is summary, false if not.</param>
        public FhirExtension(
            string name,
            string id,
            Uri url,
            string standardStatus,
            string shortDescription,
            string definition,
            string comment,
            List<string> elementPaths,
            bool isModifier,
            bool isSummary)
            : base(
                name,
                url,
                standardStatus,
                shortDescription,
                definition,
                comment,
                string.Empty)
        {
            ElementPaths = elementPaths;
            IsModifier = isModifier;
            IsSummary = isSummary;

            _properties = new Dictionary<string, FhirElement>();
        }

        /// <summary>Gets the full pathname of the element file.</summary>
        /// <value>The full pathname of the element file.</value>
        public List<string> ElementPaths { get; }

        /// <summary>Gets a list of types of the allowed values.</summary>
        /// <value>A list of types of the allowed values.</value>
        public Dictionary<string, List<string>> AllowedTypesAndProfiles { get; }

        /// <summary>Gets a value indicating whether this object is modifier.</summary>
        /// <value>True if this object is modifier, false if not.</value>
        public bool IsModifier { get; }

        /// <summary>Gets a value indicating whether this object is summary.</summary>
        /// <value>True if this object is summary, false if not.</value>
        public bool IsSummary { get; }

        /// <summary>Gets the properties.</summary>
        /// <value>The properties.</value>
        public Dictionary<string, FhirElement> Properties { get => _properties; }
    }
}
