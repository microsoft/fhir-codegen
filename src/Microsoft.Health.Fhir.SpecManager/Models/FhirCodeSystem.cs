// <copyright file="FhirCodeSystem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir code system.</summary>
    public class FhirCodeSystem
    {
        /// <summary>The concepts, by code.</summary>
        private readonly Dictionary<string, FhirConcept> _concepts;

        /// <summary>Initializes a new instance of the <see cref="FhirCodeSystem"/> class.</summary>
        /// <param name="name">          The name.</param>
        /// <param name="id">            The identifier.</param>
        /// <param name="version">       The version.</param>
        /// <param name="title">         The title.</param>
        /// <param name="url">           The URL.</param>
        /// <param name="standardStatus">The standard status.</param>
        /// <param name="description">   The description.</param>
        /// <param name="content">       The content.</param>
        /// <param name="concepts">      The concepts, by code.</param>
        public FhirCodeSystem(
            string name,
            string id,
            string version,
            string title,
            string url,
            string standardStatus,
            string description,
            string content,
            Dictionary<string, FhirConcept> concepts)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            Name = name;
            Id = id;
            Version = version;
            Title = title;
            URL = url;
            StandardStatus = standardStatus;
            Description = description;
            Content = content;
            _concepts = concepts;
        }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>Gets the version.</summary>
        /// <value>The version.</value>
        public string Version { get; }

        /// <summary>Gets the title.</summary>
        /// <value>The title.</value>
        public string Title { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public string URL { get; }

        /// <summary>Gets the standard status.</summary>
        /// <value>The standard status.</value>
        public string StandardStatus { get; }

        /// <summary>Gets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>Gets the content.</summary>
        /// <value>The content.</value>
        public string Content { get; }

        /// <summary>Gets the concepts (by code).</summary>
        /// <value>The concepts (by code).</value>
        public Dictionary<string, FhirConcept> Concepts => _concepts;
    }
}
