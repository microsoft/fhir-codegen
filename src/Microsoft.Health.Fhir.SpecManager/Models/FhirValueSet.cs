// <copyright file="FhirValueSet.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir value set.</summary>
    public class FhirValueSet
    {
        /// <summary>Initializes a new instance of the <see cref="FhirValueSet"/> class.</summary>
        /// <param name="name">           The name.</param>
        /// <param name="id">             The identifier.</param>
        /// <param name="version">        The version.</param>
        /// <param name="title">          The title.</param>
        /// <param name="url">            The URL.</param>
        /// <param name="standardStatus"> The standard status.</param>
        /// <param name="description">    The description.</param>
        /// <param name="composeIncludes">The compose includes.</param>
        /// <param name="composeExcludes">The compose excludes.</param>
        /// <param name="expansion">      The expansion.</param>
        public FhirValueSet(
            string name,
            string id,
            string version,
            string title,
            string url,
            string standardStatus,
            string description,
            List<FhirValueSetComposition> composeIncludes,
            List<FhirValueSetComposition> composeExcludes,
            FhirValueSetExpansion expansion)
        {
            Name = name;
            Id = id;
            Version = version;
            Title = title;
            URL = url;
            StandardStatus = standardStatus;
            Description = description;
            ComposeIncludes = composeIncludes;
            ComposeExcludes = composeExcludes;
            Expansion = expansion;
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

        /// <summary>Gets the compose includes.</summary>
        /// <value>The compose includes.</value>
        public List<FhirValueSetComposition> ComposeIncludes { get; }

        /// <summary>Gets the compose excludes.</summary>
        /// <value>The compose excludes.</value>
        public List<FhirValueSetComposition> ComposeExcludes { get; }

        /// <summary>Gets the expansions.</summary>
        /// <value>The expansions.</value>
        public FhirValueSetExpansion Expansion { get; }

        /// <summary>Gets a list of FhirTriplets to cover all values in the value set.</summary>
        /// <param name="codeSystems">The code systems.</param>
        /// <returns>The FhirTriplets that are contained in this ValueSet.</returns>
        public List<FhirTriplet> GetValues(Dictionary<string, FhirCodeSystem> codeSystems)
        {
            if (codeSystems == null)
            {
                throw new ArgumentNullException(nameof(codeSystems));
            }

            Dictionary<string, FhirTriplet> values = new Dictionary<string, FhirTriplet>();

            if (ComposeIncludes != null)
            {
                foreach (FhirValueSetComposition comp in ComposeIncludes)
                {
                    if ((comp.System != null) && codeSystems.ContainsKey(comp.System))
                    {
                        AddCodeSystem(ref values, codeSystems[comp.System], comp);
                    }

                    if (comp.Concepts != null)
                    {
                        AddConcepts(ref values, comp.Concepts);
                    }
                }
            }

            if (ComposeExcludes != null)
            {
                foreach (FhirValueSetComposition comp in ComposeExcludes)
                {
                    if ((comp.System != null) && codeSystems.ContainsKey(comp.System))
                    {
                        RemoveCodeSystem(ref values, codeSystems[comp.System], comp);
                    }

                    if (comp.Concepts != null)
                    {
                        RemoveConcepts(ref values, comp.Concepts);
                    }
                }
            }

            List<FhirTriplet> valueList = values.Values.ToList<FhirTriplet>();
            valueList.Sort((a, b) => string.CompareOrdinal(a.SystemAndCode(), b.SystemAndCode()));

            return valueList;
        }

        /// <summary>Removes the code system.</summary>
        /// <param name="values">    [in,out] The values.</param>
        /// <param name="codeSystem">The code system.</param>
        /// <param name="comp">      The component.</param>
        private static void RemoveCodeSystem(
            ref Dictionary<string, FhirTriplet> values,
            FhirCodeSystem codeSystem,
            FhirValueSetComposition comp)
        {
            if (comp.Concepts != null)
            {
                RemoveConcepts(ref values, comp.Concepts);
                return;
            }

            if (comp.Filters != null)
            {
                foreach (FhirValueSetFilter filter in comp.Filters)
                {
                    RemoveFilteredConcepts(ref values, codeSystem.Concepts.Values, filter);
                }
            }

            RemoveConcepts(ref values, codeSystem.Concepts.Values);
        }

        /// <summary>Adds a code system to 'codeSystem'.</summary>
        /// <param name="values">    [in,out] The values.</param>
        /// <param name="codeSystem">The code system.</param>
        /// <param name="comp">      The component.</param>
        private static void AddCodeSystem(
            ref Dictionary<string, FhirTriplet> values,
            FhirCodeSystem codeSystem,
            FhirValueSetComposition comp)
        {
            if (comp.Concepts != null)
            {
                AddConcepts(ref values, comp.Concepts);
                return;
            }

            if (comp.Filters != null)
            {
                foreach (FhirValueSetFilter filter in comp.Filters)
                {
                    AddFilteredConcepts(ref values, codeSystem.Concepts.Values, filter);
                }
            }

            AddConcepts(ref values, codeSystem.Concepts.Values);
        }

        /// <summary>Removes the filtered concepts.</summary>
        /// <param name="values">  [in,out] The values.</param>
        /// <param name="concepts">The concepts.</param>
        /// <param name="filter">  Specifies the filter.</param>
        private static void RemoveFilteredConcepts(
            ref Dictionary<string, FhirTriplet> values,
            IEnumerable<FhirTriplet> concepts,
            FhirValueSetFilter filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds a filtered concepts.</summary>
        /// <param name="values">  [in,out] The values.</param>
        /// <param name="concepts">The concepts.</param>
        /// <param name="filter">  Specifies the filter.</param>
        private static void AddFilteredConcepts(
            ref Dictionary<string, FhirTriplet> values,
            IEnumerable<FhirTriplet> concepts,
            FhirValueSetFilter filter)
        {
            if (concepts != null)
            {
                foreach (FhirTriplet concept in concepts)
                {
                    string key = concept.Key();

                    if (!values.ContainsKey(key))
                    {
                        values.Add(key, concept);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Removes the concepts.</summary>
        /// <param name="values">  [in,out] The values.</param>
        /// <param name="concepts">The concepts.</param>
        private static void RemoveConcepts(
            ref Dictionary<string, FhirTriplet> values,
            IEnumerable<FhirTriplet> concepts)
        {
            foreach (FhirTriplet concept in concepts)
            {
                string key = concept.Key();

                if (values.ContainsKey(key))
                {
                    values.Remove(key);
                }
            }
        }

        /// <summary>Adds the concepts to 'concepts'.</summary>
        /// <param name="values">  [in,out] The values.</param>
        /// <param name="concepts">The concepts.</param>
        private static void AddConcepts(
            ref Dictionary<string, FhirTriplet> values,
            IEnumerable<FhirTriplet> concepts)
        {
            foreach (FhirTriplet concept in concepts)
            {
                string key = concept.Key();

                if (values.ContainsKey(key))
                {
                    continue;
                }

                values.Add(key, concept);
            }
        }
    }
}
