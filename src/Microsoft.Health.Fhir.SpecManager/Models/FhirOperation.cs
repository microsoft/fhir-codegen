// <copyright file="FhirOperation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir operation.</summary>
    public class FhirOperation : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirOperation"/> class.
        /// </summary>
        /// <param name="id">               The identifier.</param>
        /// <param name="url">              The URL.</param>
        /// <param name="version">          The version.</param>
        /// <param name="name">             The name.</param>
        /// <param name="description">      The description.</param>
        /// <param name="definedOnSystem">  True if defined on system, false if not.</param>
        /// <param name="definedOnType">    True if defined on type, false if not.</param>
        /// <param name="definedOnInstance">True if defined on instance, false if not.</param>
        /// <param name="code">             The code.</param>
        /// <param name="comment">          The comment.</param>
        /// <param name="resourceTypes">    A list of types of the resources.</param>
        /// <param name="parameters">       The allowed parameters to this operation.</param>
        internal FhirOperation(
            string id,
            Uri url,
            string version,
            string name,
            string description,
            bool definedOnSystem,
            bool definedOnType,
            bool definedOnInstance,
            string code,
            string comment,
            IEnumerable<string> resourceTypes,
            List<FhirParameter> parameters)
        {
            Id = id;
            URL = url;
            Version = version;
            Name = name;
            Description = description;
            DefinedOnSystem = definedOnSystem;
            DefinedOnType = definedOnType;
            DefinedOnInstance = definedOnInstance;
            Code = code;
            Comment = comment;
            Parameters = parameters;

            if (resourceTypes != null)
            {
                ResourceTypes = resourceTypes.ToList();
            }
        }

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public Uri URL { get; }

        /// <summary>Gets the version.</summary>
        /// <value>The version.</value>
        public string Version { get; }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>Gets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>Gets a value indicating whether the defined on system.</summary>
        /// <value>True if defined on system, false if not.</value>
        public bool DefinedOnSystem { get; }

        /// <summary>Gets a value indicating whether the defined on type.</summary>
        /// <value>True if defined on type, false if not.</value>
        public bool DefinedOnType { get; }

        /// <summary>Gets a value indicating whether the defined on instance.</summary>
        /// <value>True if defined on instance, false if not.</value>
        public bool DefinedOnInstance { get; }

        /// <summary>Gets the code.</summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>Gets the comment.</summary>
        /// <value>The comment.</value>
        public string Comment { get; }

        /// <summary>Gets a list of types of the resources.</summary>
        /// <value>A list of types of the resources.</value>
        public List<string> ResourceTypes { get; }

        /// <summary>Gets allowed parameters to this operation.</summary>
        /// <value>The allowed parameters to this operation.</value>
        public List<FhirParameter> Parameters { get; }

        /// <summary>Deep copy.</summary>
        /// <returns>A FhirOperation.</returns>
        public object Clone()
        {
            List<string> resourceTypes = new List<string>();

            if (ResourceTypes != null)
            {
                foreach (string resourceType in ResourceTypes)
                {
                    resourceTypes.Add(string.Copy(resourceType));
                }
            }

            List<FhirParameter> parameters = new List<FhirParameter>();

            if (Parameters != null)
            {
                foreach (FhirParameter parameter in Parameters)
                {
                    parameters.Add(parameter.DeepCopy());
                }
            }

            return new FhirOperation(
                Id,
                URL,
                Version,
                Name,
                Description,
                DefinedOnSystem,
                DefinedOnType,
                DefinedOnInstance,
                Code,
                Comment,
                resourceTypes,
                parameters);
        }
    }
}
