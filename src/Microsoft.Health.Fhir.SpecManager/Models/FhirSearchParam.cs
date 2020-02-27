// <copyright file="FhirSearchParam.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir search parameter.</summary>
    public class FhirSearchParam
    {
        /// <summary>Initializes a new instance of the <see cref="FhirSearchParam"/> class.</summary>
        /// <param name="id">           The identifier.</param>
        /// <param name="url">          The URL.</param>
        /// <param name="version">      The version.</param>
        /// <param name="name">         The name.</param>
        /// <param name="purpose">      The purpose.</param>
        /// <param name="code">         The code.</param>
        /// <param name="resourceTypes">The type of the resource.</param>
        /// <param name="valueType">    The type of the value.</param>
        public FhirSearchParam(
            string id,
#pragma warning disable CA1054 // Uri parameters should not be strings
            string url,
#pragma warning restore CA1054 // Uri parameters should not be strings
            string version,
            string name,
            string purpose,
            string code,
            string[] resourceTypes,
            string valueType)
        {
            Id = id;
            Version = version;
            Name = name;
            Purpose = purpose;
            Code = code;
            ValueType = valueType;

            if (!string.IsNullOrEmpty(url))
            {
                URL = new Uri(url);
            }

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

        /// <summary>Gets the purpose.</summary>
        /// <value>The purpose.</value>
        public string Purpose { get; }

        /// <summary>Gets the code.</summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>Gets the type of the resource.</summary>
        /// <value>The type of the resource.</value>
        public List<string> ResourceTypes { get; }

        /// <summary>Gets the type of the value.</summary>
        /// <value>The type of the value.</value>
        public string ValueType { get; }
    }
}
