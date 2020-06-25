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
    public class FhirValueSet : ICloneable
    {
        private List<FhirConcept> _valueList = null;
        private HashSet<string> _codeSystems = new HashSet<string>();

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
        /// <param name="concepts">       The resolved concepts.</param>
        /// <param name="referencedCodeSystems">The list of code systems referenced by this value set.</param>
        private FhirValueSet(
            string name,
            string id,
            string version,
            string title,
            string url,
            string standardStatus,
            string description,
            List<FhirValueSetComposition> composeIncludes,
            List<FhirValueSetComposition> composeExcludes,
            FhirValueSetExpansion expansion,
            List<FhirConcept> concepts,
            HashSet<string> referencedCodeSystems)
            : this(
                name,
                id,
                version,
                title,
                url,
                standardStatus,
                description,
                composeIncludes,
                composeExcludes,
                expansion)
        {
            _valueList = concepts;
            _codeSystems = referencedCodeSystems;
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

        /// <summary>Gets the key.</summary>
        /// <value>The key.</value>
        public string Key => $"{URL}|{Version}";

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

        /// <summary>Gets the concepts.</summary>
        /// <value>The concepts.</value>
        public List<FhirConcept> Concepts => _valueList;

        /// <summary>Gets the referenced code systems.</summary>
        /// <value>The referenced code systems.</value>
        public HashSet<string> ReferencedCodeSystems => _codeSystems;

        /// <summary>Gets a list of FhirTriplets to cover all values in the value set.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="codeSystems">The code systems.</param>
        public void Resolve(Dictionary<string, FhirCodeSystem> codeSystems)
        {
            if (_valueList != null)
            {
                return;
            }

            if (codeSystems == null)
            {
                throw new ArgumentNullException(nameof(codeSystems));
            }

            _codeSystems.Clear();

            Dictionary<string, FhirConcept> values = new Dictionary<string, FhirConcept>();

            if (ComposeIncludes != null)
            {
                foreach (FhirValueSetComposition comp in ComposeIncludes)
                {
                    if (comp.Concepts != null)
                    {
                        AddConcepts(ref values, comp.Concepts, codeSystems);
                        continue;
                    }

                    if ((comp.System != null) &&
                        codeSystems.ContainsKey(comp.System))
                    {
                        if (!_codeSystems.Contains(comp.System))
                        {
                            _codeSystems.Add(comp.System);
                        }

                        AddCodeSystem(ref values, codeSystems[comp.System], comp);
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

            _valueList = values.Values.ToList<FhirConcept>();
            _valueList.Sort((a, b) => string.CompareOrdinal(a.SystemAndCode(), b.SystemAndCode()));
        }

        /// <summary>Removes the code system.</summary>
        /// <param name="values">    [in,out] The values.</param>
        /// <param name="codeSystem">The code system.</param>
        /// <param name="comp">      The component.</param>
        private static void RemoveCodeSystem(
            ref Dictionary<string, FhirConcept> values,
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
        private void AddCodeSystem(
            ref Dictionary<string, FhirConcept> values,
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
            ref Dictionary<string, FhirConcept> values,
            IEnumerable<FhirConcept> concepts,
            FhirValueSetFilter filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds a filtered concepts.</summary>
        /// <param name="values">  [in,out] The values.</param>
        /// <param name="concepts">The concepts.</param>
        /// <param name="filter">  Specifies the filter.</param>
        private static void AddFilteredConcepts(
            ref Dictionary<string, FhirConcept> values,
            IEnumerable<FhirConcept> concepts,
            FhirValueSetFilter filter)
        {
            if (concepts != null)
            {
                foreach (FhirConcept concept in concepts)
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
            ref Dictionary<string, FhirConcept> values,
            IEnumerable<FhirConcept> concepts)
        {
            foreach (FhirConcept concept in concepts)
            {
                string key = concept.Key();

                if (values.ContainsKey(key))
                {
                    values.Remove(key);
                }
            }
        }

        /// <summary>Adds the concepts to 'concepts'.</summary>
        /// <param name="values">     [in,out] The values.</param>
        /// <param name="concepts">   The concepts.</param>
        /// <param name="codeSystems">(Optional) The code systems.</param>
        private void AddConcepts(
            ref Dictionary<string, FhirConcept> values,
            IEnumerable<FhirConcept> concepts,
            Dictionary<string, FhirCodeSystem> codeSystems = null)
        {
            foreach (FhirConcept concept in concepts)
            {
                string key = concept.Key();

                if (values.ContainsKey(key))
                {
                    continue;
                }

                if ((codeSystems != null) &&
                    codeSystems.ContainsKey(concept.System) &&
                    codeSystems[concept.System].Concepts.ContainsKey(concept.Code))
                {
                    if (!_codeSystems.Contains(concept.System))
                    {
                        _codeSystems.Add(concept.System);
                    }

                    values.Add(key, codeSystems[concept.System].Concepts[concept.Code]);
                    continue;
                }

                values.Add(key, concept);
            }
        }

        /// <summary>Deep copy.</summary>
        /// <param name="resolveRules">(Optional) True to resolve rules.</param>
        /// <returns>A FhirValueSet.</returns>
        public object Clone()
        {
            List<FhirValueSetComposition> includes = new List<FhirValueSetComposition>();

            if (ComposeIncludes != null)
            {
                includes = ComposeIncludes.Select(i => (FhirValueSetComposition)i.Clone()).ToList();
            }

            List<FhirValueSetComposition> excludes = new List<FhirValueSetComposition>();

            if (ComposeExcludes != null)
            {
                excludes = ComposeExcludes.Select(e => (FhirValueSetComposition)e.Clone()).ToList();
            }

            FhirValueSetExpansion expansion = null;

            if (Expansion != null)
            {
                expansion = (FhirValueSetExpansion)Expansion.Clone();
            }

            List<FhirConcept> concepts = null;

            if (Concepts != null)
            {
                concepts = Concepts.Select(c => (FhirConcept)c.Clone()).ToList();
            }

            HashSet<string> codeSystems = new HashSet<string>();
            if (_codeSystems != null)
            {
                foreach (string val in _codeSystems)
                {
                    codeSystems.Add(val);
                }
            }

            return new FhirValueSet(
                Name,
                Id,
                Version,
                Title,
                URL,
                StandardStatus,
                Description,
                includes,
                excludes,
                expansion,
                concepts,
                codeSystems);
        }
    }
}
