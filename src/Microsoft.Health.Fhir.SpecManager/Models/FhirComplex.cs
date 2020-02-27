// -------------------------------------------------------------------------------------------------
// <copyright file="FhirComplex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A class representing a FHIR complex type.</summary>
    public class FhirComplex : FhirTypeBase
    {
        /// <summary>Initializes a new instance of the <see cref="FhirComplex"/> class.</summary>
        /// <param name="path">            Full pathname of the file.</param>
        /// <param name="standardStatus">  The standard status.</param>
        /// <param name="shortDescription">Information describing the short.</param>
        /// <param name="purpose">         The purpose.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        public FhirComplex(
            string path,
            string standardStatus,
            string shortDescription,
            string purpose,
            string comment,
            string validationRegEx)
            : base(
                path,
                standardStatus,
                shortDescription,
                purpose,
                comment,
                validationRegEx)
        {
            Components = new Dictionary<string, FhirComplex>();
            Properties = new Dictionary<string, FhirProperty>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirComplex"/> class.
        /// </summary>
        /// <param name="path">            Full pathname of the file.</param>
        /// <param name="standardStatus">  The standard status.</param>
        /// <param name="shortDescription">Information describing the short.</param>
        /// <param name="purpose">         The purpose.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        /// <param name="baseTypeName">    Name of the base type.</param>
        public FhirComplex(
            string path,
            string standardStatus,
            string shortDescription,
            string purpose,
            string comment,
            string validationRegEx,
            string baseTypeName)
            : this(
                path,
                standardStatus,
                shortDescription,
                purpose,
                comment,
                validationRegEx)
        {
            BaseTypeName = baseTypeName;
        }

        /// <summary>Gets or sets a value indicating whether this object is placeholder.</summary>
        /// <value>True if this object is placeholder, false if not.</value>
        public bool IsPlaceholder { get; set; }

        /// <summary>Gets the properties.</summary>
        /// <value>The properties.</value>
        public Dictionary<string, FhirProperty> Properties { get; }

        /// <summary>Gets the components.</summary>
        /// <value>The components.</value>
        public Dictionary<string, FhirComplex> Components { get; }

        /// <summary>Adds a component from a property.</summary>
        /// <param name="path">Name of the property.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool AddComponentFromProperty(string path)
        {
            if ((!Properties.ContainsKey(path)) ||
                Components.ContainsKey(path))
            {
                return false;
            }

            FhirProperty property = Properties[path];

            // create a new complex type from the property
            Components.Add(property.Path, new FhirComplex(
                property.Path,
                property.StandardStatus,
                property.ShortDescription,
                property.Purpose,
                property.Comment,
                property.ValidationRegEx,
                property.BaseTypeName));

            return true;
        }

        /// <summary>Gets the parent and field name.</summary>
        /// <param name="elementComponents">The element components.</param>
        /// <param name="parent">           [out] The parent.</param>
        /// <param name="field">            [out] The field.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool GetParentAndFieldName(
            string[] elementComponents,
            out FhirComplex parent,
            out string field)
        {
            // sanity checks - need at least 2 path components to have a parent
            if ((elementComponents == null) || (elementComponents.Length < 2))
            {
                parent = null;
                field = string.Empty;
                return false;
            }

            // find the parent and field name
            return GetParentAndFieldNameRecurse(
                elementComponents,
                0,
                out parent,
                out field);
        }

        /// <summary>Gets the parent and field name, recursively.</summary>
        /// <param name="elementComponents">The element components.</param>
        /// <param name="startIndex">       The start index.</param>
        /// <param name="parent">           [out] The parent.</param>
        /// <param name="field">            [out] The field.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool GetParentAndFieldNameRecurse(
            string[] elementComponents,
            int startIndex,
            out FhirComplex parent,
            out string field)
        {
            // check for no name match
            if (!Name.Equals(elementComponents[startIndex], StringComparison.Ordinal))
            {
                // fail
                parent = null;
                field = string.Empty;
                return false;
            }

            // check for being the parent to the field
            if (startIndex == (elementComponents.Length - 2))
            {
                parent = this;
                field = elementComponents[elementComponents.Length - 1];
                return true;
            }

            // build the path to this location
            string path = PathForComponents(elementComponents, 0, startIndex + 1);

            // check for matching property, but no component
            if (Properties.ContainsKey(path) &&
                (!Components.ContainsKey(path)))
            {
                // add component from property
                AddComponentFromProperty(path);
            }

            // check Components for match
            if (Components.ContainsKey(path))
            {
                // recurse
                return Components[path].GetParentAndFieldNameRecurse(
                    elementComponents,
                    startIndex + 1,
                    out parent,
                    out field);
            }

            // fail
            parent = null;
            field = string.Empty;
            return false;
        }

        /// <summary>Path for components.</summary>
        /// <param name="components">The components.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">  The end index.</param>
        /// <returns>A string.</returns>
        private static string PathForComponents(string[] components, int startIndex, int endIndex)
        {
            string val = components[startIndex];

            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                val += $".{components[i]}";
            }

            return val;
        }
    }
}
