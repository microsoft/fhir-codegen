// <copyright file="FhirValueSet.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir value set.</summary>
public class FhirValueSet : ICloneable
{
    private List<FhirConcept> _valueList = null;
    private HashSet<string> _codeSystems = new();
    private List<string> _referencedPaths = new();
    private List<string> _referencedResources = new();
    private FhirElement.ElementDefinitionBindingStrength _strongestBinding;

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
        HashSet<string> referencedCodeSystems,
        FhirElement.ElementDefinitionBindingStrength strongestBinding)
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
        _strongestBinding = strongestBinding;
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

    /// <summary>Gets the list of elements (by Path) that reference this value set.</summary>
    public List<string> ReferencedByPaths => _referencedPaths;

    /// <summary>Gets the list of resources or complex types that reference this value set.</summary>
    public List<string> ReferencedByComplexes => _referencedResources;

    /// <summary>Gets the strongest binding this value set is referenced as (null for unreferenced).</summary>
    public FhirElement.ElementDefinitionBindingStrength? StrongestBinding => _strongestBinding;

    /// <summary>Sets the references.</summary>
    /// <param name="referenceInfo">Reference information for this value set.</param>
    public void SetReferences(ValueSetReferenceInfo referenceInfo)
    {
        if (referenceInfo == null)
        {
            return;
        }

        if (_referencedResources == null)
        {
            _referencedResources = new();
        }

        if (_referencedPaths == null)
        {
            _referencedPaths = new();
        }

        HashSet<string> resources = new HashSet<string>();
        HashSet<string> paths = new HashSet<string>();

        foreach (string path in referenceInfo.Paths)
        {
            if (paths.Contains(path))
            {
                continue;
            }

            string resource = path.Substring(0, path.IndexOf('.'));
            if (!resources.Contains(resource))
            {
                resources.Add(resource);
                _referencedResources.Add(resource);
            }

            _referencedPaths.Add((string)path.Clone());
            paths.Add(path);
        }

        _strongestBinding = referenceInfo.StrongestBinding;
    }

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

        if (_codeSystems == null)
        {
            _codeSystems = new();
        }
        else
        {
            _codeSystems.Clear();
        }

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

        if ((Expansion != null) && (Expansion.Contains != null))
        {
            AddConcepts(ref values, Expansion.Contains, codeSystems);
        }

        _valueList = values.Values.ToList<FhirConcept>();
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
            ApplyFilteredConcepts(ref values, codeSystem, comp.Filters, false, true);
            return;
        }

        RemoveFromNode(
            ref values,
            codeSystem.RootConcept,
            false,
            true,
            false,
            string.Empty,
            null,
            null,
            null);
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
            ApplyFilteredConcepts(ref values, codeSystem, comp.Filters, true, false);
            return;
        }

        AddFromNode(
            ref values,
            codeSystem.RootConcept,
            false,
            true,
            false,
            string.Empty,
            null,
            null,
            null);
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
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="values">    [in,out] The values.</param>
    /// <param name="codeSystem">The code system.</param>
    /// <param name="filters">   Specifies the filters.</param>
    /// <param name="include">   True to include, false to exclude.</param>
    /// <param name="exclude">   True to exclude, false to include.</param>
    private static void ApplyFilteredConcepts(
        ref Dictionary<string, FhirConcept> values,
        FhirCodeSystem codeSystem,
        List<FhirValueSetFilter> filters,
        bool include,
        bool exclude)
    {
        if (codeSystem == null)
        {
            throw new ArgumentNullException(nameof(codeSystem));
        }

        if ((filters == null) || (filters.Count == 0))
        {
            throw new ArgumentNullException(nameof(filters));
        }

        if (include && exclude)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            throw new Exception("Cannot include and exclude the same filters!");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        if ((!include) && (!exclude))
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            throw new Exception("Must either include or exclude for filters!");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        string startingCode = string.Empty;
        bool includeSelf = false;
        bool includeChildren = false;
        bool includeParents = false;
        string exclusionKey = string.Empty;
        Regex regex = null;
        HashSet<string> inclusionSet = null;
        HashSet<string> exclusionSet = null;
        int maxRecusrions = -1;
        List<KeyValuePair<string, string>> filterProperties = new List<KeyValuePair<string, string>>();

        foreach (FhirValueSetFilter filter in filters)
        {
            string filterKey = $"{filter.Property}:{filter.Operation}";

            switch (filterKey)
            {
                case "concept:=":
                    startingCode = filter.Value;
                    includeSelf = true;
                    includeChildren = false;
                    includeParents = false;

                    break;

                case "concept:is-a":
                    startingCode = filter.Value;
                    includeSelf = true;
                    includeChildren = true;
                    includeParents = false;

                    break;

                case "concept:descendent-of":
                    startingCode = filter.Value;
                    includeSelf = false;
                    includeChildren = true;
                    includeParents = false;

                    break;

                case "concept:is-not-a":
                    exclusionKey = filter.Value;
                    break;

                case "concept:regex":
                    regex = new Regex(filter.Value);
                    break;

                case "concept:in":
                    inclusionSet = new HashSet<string>();

                    string[] inculsions = filter.Value.Split(',');

                    foreach (string value in inculsions)
                    {
                        inclusionSet.Add(value);
                    }

                    break;

                case "concept:not-in":
                    exclusionSet = new HashSet<string>();

                    string[] exclusions = filter.Value.Split(',');

                    foreach (string value in exclusions)
                    {
                        exclusionSet.Add(value);
                    }

                    break;

                case "concept:generalizes":
                    startingCode = filter.Value;
                    includeSelf = true;
                    includeChildren = false;
                    includeParents = true;

                    break;

                case "parent:=":
                    startingCode = filter.Value;
                    includeSelf = false;
                    includeChildren = true;
                    includeParents = false;
                    maxRecusrions = 1;
                    break;

                case "child:=":
                    startingCode = filter.Value;
                    includeSelf = false;
                    includeChildren = false;
                    includeParents = true;
                    maxRecusrions = 1;
                    break;

                // ignore these
                case "acme-plasma:=":
                    return;

                case "concept:exists":
                default:
                    if (filter.Operation == "=")
                    {
                        filterProperties.Add(new KeyValuePair<string, string>(filter.Property, filter.Value));

                        includeSelf = true;
                        includeChildren = true;
                        includeParents = false;

                        continue;
                    }

                    throw new NotImplementedException($"Unhandled filter: {filterKey}");
            }
        }

        FhirConceptTreeNode startingNode = codeSystem.RootConcept;

        if ((!string.IsNullOrEmpty(startingCode)) &&
            codeSystem.ContainsConcept(startingCode))
        {
            startingNode = codeSystem[startingCode];
        }

        if (include)
        {
            AddFromNode(
                ref values,
                startingNode,
                includeSelf,
                includeChildren,
                includeParents,
                exclusionKey,
                regex,
                inclusionSet,
                exclusionSet,
                maxRecusrions,
                filterProperties);
        }

        if (exclude)
        {
            RemoveFromNode(
                ref values,
                startingNode,
                includeSelf,
                includeChildren,
                includeParents,
                exclusionKey,
                regex,
                inclusionSet,
                exclusionSet,
                maxRecusrions,
                filterProperties);
        }
    }

    /// <summary>Removes from node.</summary>
    /// <param name="values">          [in,out] The values.</param>
    /// <param name="node">            The node.</param>
    /// <param name="includeSelf">     True to include, false to exclude the self.</param>
    /// <param name="includeChildren"> True to include, false to exclude the children.</param>
    /// <param name="includeParents">  True to include, false to exclude the parents.</param>
    /// <param name="exclusionKey">    The exclusion key.</param>
    /// <param name="regex">           The RegEx.</param>
    /// <param name="inclusionSet">    Set the inclusion belongs to.</param>
    /// <param name="exclusionSet">    Set the exclusion belongs to.</param>
    /// <param name="maxRecursions">   (Optional) The maximum recursions (-1 for no limit).</param>
    /// <param name="filterProperties">(Optional) The include properties.</param>
    private static void RemoveFromNode(
        ref Dictionary<string, FhirConcept> values,
        FhirConceptTreeNode node,
        bool includeSelf,
        bool includeChildren,
        bool includeParents,
        string exclusionKey,
        Regex regex,
        HashSet<string> inclusionSet,
        HashSet<string> exclusionSet,
        int maxRecursions = -1,
        List<KeyValuePair<string, string>> filterProperties = null)
    {
        if ((!string.IsNullOrEmpty(exclusionKey)) &&
            (node.Concept.Code == exclusionKey))
        {
            return;
        }

        if (includeSelf &&
            (!values.ContainsKey(node.Concept.Code)) &&
            ((regex == null) || regex.IsMatch(node.Concept.Code)) &&
            ((inclusionSet == null) || inclusionSet.Contains(node.Concept.Code)) &&
            ((exclusionSet == null) || (!exclusionSet.Contains(node.Concept.Code))))
        {
            string key = node.Concept.Key();

            if (!values.ContainsKey(key))
            {
                if ((filterProperties != null) && (filterProperties.Count > 0))
                {
                    if (node.Concept.MatchesProperties(filterProperties))
                    {
                        values.Remove(key);
                    }
                }
                else
                {
                    values.Remove(key);
                }
            }
        }

        if (includeChildren &&
            (node.Children != null) &&
            (maxRecursions != 0))
        {
            if (maxRecursions > 0)
            {
                maxRecursions--;
            }

            foreach (FhirConceptTreeNode child in node.Children.Values)
            {
                RemoveFromNode(
                    ref values,
                    child,
                    true,
                    true,
                    false,
                    exclusionKey,
                    regex,
                    inclusionSet,
                    exclusionSet,
                    maxRecursions,
                    filterProperties);
            }
        }

        if (includeParents &&
            (node.Parent != null) &&
            (maxRecursions != 0))
        {
            if (maxRecursions > 0)
            {
                maxRecursions--;
            }

            RemoveFromNode(
                ref values,
                node.Parent,
                true,
                false,
                true,
                exclusionKey,
                regex,
                inclusionSet,
                exclusionSet,
                maxRecursions,
                filterProperties);
        }
    }

    /// <summary>Adds from node.</summary>
    /// <param name="values">          [in,out] The values.</param>
    /// <param name="node">            The node.</param>
    /// <param name="includeSelf">     True to include, false to exclude the self.</param>
    /// <param name="includeChildren"> True to include, false to exclude the children.</param>
    /// <param name="includeParents">  True to include, false to exclude the parents.</param>
    /// <param name="exclusionKey">    The exclusion key.</param>
    /// <param name="regex">           The RegEx.</param>
    /// <param name="inclusionSet">    Set the inclusion belongs to.</param>
    /// <param name="exclusionSet">    Set the exclusion belongs to.</param>
    /// <param name="maxRecursions">   (Optional) The maximum recursions (-1 for no limit).</param>
    /// <param name="filterProperties">(Optional) The include properties.</param>
    private static void AddFromNode(
        ref Dictionary<string, FhirConcept> values,
        FhirConceptTreeNode node,
        bool includeSelf,
        bool includeChildren,
        bool includeParents,
        string exclusionKey,
        Regex regex,
        HashSet<string> inclusionSet,
        HashSet<string> exclusionSet,
        int maxRecursions = -1,
        List<KeyValuePair<string, string>> filterProperties = null)
    {
        if ((!string.IsNullOrEmpty(exclusionKey)) &&
            (node.Concept != null) &&
            (node.Concept.Code == exclusionKey))
        {
            return;
        }

        if (includeSelf &&
            (node.Concept != null) &&
            (!values.ContainsKey(node.Concept.Code)) &&
            ((regex == null) || regex.IsMatch(node.Concept.Code)) &&
            ((inclusionSet == null) || inclusionSet.Contains(node.Concept.Code)) &&
            ((exclusionSet == null) || (!exclusionSet.Contains(node.Concept.Code))))
        {
            string key = node.Concept.Key();

            if (!values.ContainsKey(key))
            {
                if ((filterProperties != null) && (filterProperties.Count > 0))
                {
                    if (node.Concept.MatchesProperties(filterProperties))
                    {
                        values.Add(key, node.Concept);
                    }
                }
                else
                {
                    values.Add(key, node.Concept);
                }
            }
        }

        if (includeChildren &&
            (node.Children != null) &&
            (maxRecursions != 0))
        {
            if (maxRecursions > 0)
            {
                maxRecursions--;
            }

            foreach (FhirConceptTreeNode child in node.Children.Values)
            {
                AddFromNode(
                    ref values,
                    child,
                    true,
                    true,
                    false,
                    exclusionKey,
                    regex,
                    inclusionSet,
                    exclusionSet,
                    maxRecursions,
                    filterProperties);
            }
        }

        if (includeParents &&
            (node.Parent != null) &&
            (maxRecursions != 0))
        {
            if (maxRecursions > 0)
            {
                maxRecursions--;
            }

            AddFromNode(
                ref values,
                node.Parent,
                true,
                false,
                true,
                exclusionKey,
                regex,
                inclusionSet,
                exclusionSet,
                maxRecursions,
                filterProperties);
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
        if (_codeSystems == null)
        {
            _codeSystems = new();
        }

        foreach (FhirConcept concept in concepts)
        {
            string key = concept.Key();

            if ((codeSystems != null) &&
                codeSystems.ContainsKey(concept.System) &&
                codeSystems[concept.System].ContainsConcept(concept.Code))
            {
                if (!_codeSystems.Contains(concept.System))
                {
                    _codeSystems.Add(concept.System);
                }

                if (values.ContainsKey(key))
                {
                    values.Remove(key);
                }

                values.Add(key, codeSystems[concept.System][concept.Code].Concept);
                continue;
            }

            if (values.ContainsKey(key))
            {
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
            codeSystems,
            _strongestBinding);
    }
}
