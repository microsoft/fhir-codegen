﻿// <copyright file="FhirCodeSystem.cs" company="Microsoft Corporation">
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
        /// <summary>The root concept.</summary>
        private readonly FhirConceptTreeNode _rootConcept;

        /// <summary>The concepts, by code.</summary>
        private readonly Dictionary<string, FhirConceptTreeNode> _conceptLookup;

        /// <summary>Initializes a new instance of the <see cref="FhirCodeSystem"/> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="name">          The name.</param>
        /// <param name="id">            The identifier.</param>
        /// <param name="version">       The version.</param>
        /// <param name="title">         The title.</param>
        /// <param name="url">           The URL.</param>
        /// <param name="standardStatus">The standard status.</param>
        /// <param name="description">   The description.</param>
        /// <param name="content">       The content.</param>
        /// <param name="rootConcept">   The root concept.</param>
        /// <param name="conceptLookup"> The concept lookup.</param>
        public FhirCodeSystem(
            string name,
            string id,
            string version,
            string title,
            string url,
            string standardStatus,
            string description,
            string content,
            FhirConceptTreeNode rootConcept,
            Dictionary<string, FhirConceptTreeNode> conceptLookup)
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
            _rootConcept = rootConcept;
            _conceptLookup = conceptLookup;
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

        /// <summary>Gets the root concept.</summary>
        /// <value>The root concept.</value>
        public FhirConceptTreeNode RootConcept => _rootConcept;

        /// <summary>Gets the concepts (by code).</summary>
        /// <value>The concepts (by code).</value>
        public Dictionary<string, FhirConceptTreeNode> ConceptLookup => _conceptLookup;

        /// <summary>Indexer to get slices based on name.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
        ///  required range.</exception>
        /// <param name="code">The code.</param>
        /// <returns>The indexed item.</returns>
        public FhirConceptTreeNode this[string code]
        {
            get
            {
                if (!_conceptLookup.ContainsKey(code))
                {
                    throw new ArgumentOutOfRangeException(nameof(code));
                }

                return _conceptLookup[code];
            }
        }

        /// <summary>Query if this system contains a concept, specified by code.</summary>
        /// <param name="code">The code.</param>
        /// <returns>True if this system has the concept, false if it does not.</returns>
        public bool ContainsConcept(string code)
        {
            return _conceptLookup.ContainsKey(code);
        }
    }
}
