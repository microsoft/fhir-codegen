// <copyright file="FhirConceptTreeNode.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir concept tree node.</summary>
    public class FhirConceptTreeNode
    {
        /// <summary>The concept.</summary>
        private readonly FhirConcept _concept;

        /// <summary>The children.</summary>
        private Dictionary<string, FhirConceptTreeNode> _children;

        /// <summary>The parent.</summary>
        private readonly FhirConceptTreeNode _parent;

        /// <summary>Initializes a new instance of the <see cref="FhirConceptTreeNode"/> class.</summary>
        /// <param name="concept">The concept.</param>
        /// <param name="parent"> The parent.</param>
        public FhirConceptTreeNode(FhirConcept concept, FhirConceptTreeNode parent)
        {
            _concept = concept;
            _children = new Dictionary<string, FhirConceptTreeNode>();
            _parent = parent;
        }

        /// <summary>Gets the concept.</summary>
        /// <value>The concept.</value>
        public FhirConcept Concept => _concept;

        /// <summary>Gets the children.</summary>
        /// <value>The children.</value>
        public Dictionary<string, FhirConceptTreeNode> Children => _children;

        /// <summary>Gets the parent.</summary>
        /// <value>The parent.</value>
        public FhirConceptTreeNode Parent => _parent;

        /// <summary>Gets a value indicating whether this object is root.</summary>
        /// <value>True if this object is root, false if not.</value>
        public bool IsRoot => _concept == null;

        /// <summary>Adds a node.</summary>
        /// <param name="concept">The concept.</param>
        /// <returns>A FhirConceptTreeNode.</returns>
        public FhirConceptTreeNode AddChild(FhirConcept concept)
        {
            if (concept == null)
            {
                return null;
            }

            if (_children.ContainsKey(concept.Code))
            {
                return _children[concept.Code];
            }

            FhirConceptTreeNode node = new FhirConceptTreeNode(concept, this);

            _children.Add(concept.Code, node);

            return node;
        }
    }
}
