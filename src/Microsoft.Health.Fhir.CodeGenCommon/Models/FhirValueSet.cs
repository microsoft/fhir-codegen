// <copyright file="FhirValueSet.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.RegularExpressions;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir value set.</summary>
public class FhirValueSet : ICloneable
{
    private List<FhirConcept> _valueList = null;
    private HashSet<string> _codeSystems = new();
    private List<string> _referencedPaths = new();
    private Dictionary<string, FhirElement> _referencingElementsByPath = new();
    private Dictionary<string, FhirElement> _referencingExternalElementsByUrl = new();
    private List<string> _referencedResources = new();
    private Dictionary<string, FhirElement.ElementDefinitionBindingStrength> _bindingStrengthByType = new();
    private Dictionary<string, FhirElement.ElementDefinitionBindingStrength> _externalBindingStrengthByType = new();
    private FhirElement.ElementDefinitionBindingStrength _strongestBinding;
    private FhirElement.ElementDefinitionBindingStrength _strongestExternalBinding;

    /// <summary>Initializes a new instance of the <see cref="FhirValueSet"/> class.</summary>
    /// <param name="name">             The name.</param>
    /// <param name="id">               The identifier.</param>
    /// <param name="version">          The version.</param>
    /// <param name="title">            The title.</param>
    /// <param name="url">              The URL.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">   The standard status.</param>
    /// <param name="fmmLevel">         The fmm level.</param>
    /// <param name="description">      The description.</param>
    /// <param name="composeIncludes">  The compose includes.</param>
    /// <param name="composeExcludes">  The compose excludes.</param>
    /// <param name="expansion">        The expansion.</param>
    public FhirValueSet(
        string name,
        string id,
        string version,
        string title,
        string url,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
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
        PublicationStatus = publicationStatus;
        StandardStatus = standardStatus;
        FhirMaturityLevel = fmmLevel;
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
    private FhirValueSet(FhirValueSet source)
    {
        Name = source.Name;
        Id = source.Id;
        Version = source.Version;
        Title = source.Title;
        URL = source.URL;
        PublicationStatus = source.PublicationStatus;
        StandardStatus = source.StandardStatus;
        FhirMaturityLevel = source.FhirMaturityLevel;
        Description = source.Description;

        ComposeIncludes = source.ComposeIncludes?.Select(c => new FhirValueSetComposition(c)).ToList() ?? new();
        ComposeExcludes = source.ComposeExcludes?.Select(c => new FhirValueSetComposition(c)).ToList() ?? new();
        Expansion = (FhirValueSetExpansion)source.Expansion?.Clone() ?? null;
        _valueList = source._valueList?.Select(c => new FhirConcept(c)).ToList() ?? new();
        _codeSystems = source._codeSystems?.Select(s => s).ToHashSet() ?? new();
        _referencedPaths = source._referencedPaths?.Select(s => s).ToList() ?? new();
        _referencingElementsByPath = source._referencingElementsByPath?.ShallowCopy() ?? new();
        _referencingExternalElementsByUrl = source._referencingExternalElementsByUrl?.ShallowCopy() ?? new();
        _referencedResources = source._referencedResources?.Select(s => s).ToList() ?? new();
        _bindingStrengthByType = source._bindingStrengthByType?.ShallowCopy() ?? new();
        _externalBindingStrengthByType = source._externalBindingStrengthByType?.ShallowCopy() ?? new();
        _strongestBinding = source._strongestBinding;
        _strongestExternalBinding = source._strongestExternalBinding;
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

    /// <summary>Gets the publication status.</summary>
    public string PublicationStatus { get; }

    /// <summary>
    /// Gets status of this type in the standards process
    /// see: http://hl7.org/fhir/valueset-standards-status.html.
    /// </summary>
    /// <value>The standard status.</value>
    public string StandardStatus { get; }

    /// <summary>Gets the FHIR maturity level.</summary>
    public int? FhirMaturityLevel { get; }

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
    [Obsolete("Will be removing this in favor of ReferencingElementsByPath in the future")]
    public List<string> ReferencedByPaths => _referencedPaths;

    /// <summary>Gets the full pathname of the referencing elements by file.</summary>
    public Dictionary<string, FhirElement> ReferencingElementsByPath => _referencingElementsByPath;

    /// <summary>Gets the full pathname of the external referencing elements (e.g., extensions, profiles) by definitional URL.</summary>
    public Dictionary<string, FhirElement> ReferencingExternalElementsByUrl => _referencingExternalElementsByUrl;

    /// <summary>Gets the list of resources or complex types that reference this value set.</summary>
    public List<string> ReferencedByComplexes => _referencedResources;

    /// <summary>
    /// Gets a Dictionary of strongest bindings by FHIR element type
    /// </summary>
    public Dictionary<string, FhirElement.ElementDefinitionBindingStrength> StrongestBindingByType => _bindingStrengthByType;

    /// <summary>Gets the type of the strongest external (e.g., profile, extension) binding by type.</summary>
    public Dictionary<string, FhirElement.ElementDefinitionBindingStrength> StrongestExternalBindingByType => _externalBindingStrengthByType;

    /// <summary>Gets the strongest binding this value set is referenced as (null for unreferenced).</summary>
    public FhirElement.ElementDefinitionBindingStrength? StrongestBinding => _strongestBinding;

    /// <summary>Gets the strongest external (e.g., profile, extension) binding.</summary>
    public FhirElement.ElementDefinitionBindingStrength? StrongestExternalBinding => _strongestExternalBinding;

    /// <summary>Sets the references.</summary>
    /// <param name="referenceInfo">Reference information for this value set.</param>
    public void SetReferences(ValueSetReferenceInfo referenceInfo)
    {
        if (referenceInfo == null)
        {
            return;
        }

        HashSet<string> matchedRoots = new();
        HashSet<string> paths = new();
        HashSet<string> urls = new();

        foreach ((string path, FhirElement element) in referenceInfo.ReferencingElementsByPath.OrderBy(s => s.Value.RootArtifact?.ArtifactClass))
        {
            switch (element.RootArtifact.ArtifactClass)
            {
                // 'primary' references downstream callers want to know about
                case FhirArtifactClassEnum.PrimitiveType:
                case FhirArtifactClassEnum.ComplexType:
                case FhirArtifactClassEnum.Resource:
                    {
                        if (paths.Contains(path))
                        {
                            continue;
                        }

                        if (CheckPathForDataType(matchedRoots, path, element.RootArtifact))
                        {
                            // do not add this path - it is handled by a type higher in the tree
                            continue;
                        }

                        _referencingElementsByPath.Add(path, element);
                        paths.Add(path);
                        _referencedPaths.Add(path);

                        string rootName = path.Substring(0, path.IndexOf('.'));

                        if (!matchedRoots.Contains(rootName))
                        {
                            matchedRoots.Add(rootName);
                            _referencedResources.Add(rootName);
                        }

                        if ((element.ElementTypes != null) &&
                            (element.ValueSetBindingStrength != null))
                        {
                            foreach (string fhirType in element.ElementTypes.Keys)
                            {
                                if ((!_bindingStrengthByType.ContainsKey(fhirType)) ||
                                    (_bindingStrengthByType[fhirType] < element.ValueSetBindingStrength))
                                {
                                    _bindingStrengthByType[fhirType] = (FhirElement.ElementDefinitionBindingStrength)element.ValueSetBindingStrength;
                                }
                            }
                        }
                    }
                    break;

                // additional or 'external' references - important to track but typically handled differently
                case FhirArtifactClassEnum.Extension:
                case FhirArtifactClassEnum.Profile:
                case FhirArtifactClassEnum.LogicalModel:
                case FhirArtifactClassEnum.ConceptMap:
                    {
                        string url = element.RootArtifact.Url;

                        if (urls.Contains(url))
                        {
                            continue;
                        }

                        urls.Add(url);
                        _referencingExternalElementsByUrl.Add(url, element);

                        if ((element.ElementTypes != null) &&
                            (element.ValueSetBindingStrength != null))
                        {
                            foreach (string fhirType in element.ElementTypes.Keys)
                            {
                                if ((!_externalBindingStrengthByType.ContainsKey(fhirType)) ||
                                    (_externalBindingStrengthByType[fhirType] < element.ValueSetBindingStrength))
                                {
                                    _externalBindingStrengthByType[fhirType] = (FhirElement.ElementDefinitionBindingStrength)element.ValueSetBindingStrength;
                                }
                            }
                        }
                    }
                    break;

                // don't bother tracking references derived from these classes
                case FhirArtifactClassEnum.NamingSystem:
                case FhirArtifactClassEnum.Operation:
                case FhirArtifactClassEnum.SearchParameter:
                case FhirArtifactClassEnum.CodeSystem:
                case FhirArtifactClassEnum.ValueSet:
                case FhirArtifactClassEnum.CapabilityStatement:
                case FhirArtifactClassEnum.Compartment:
                case FhirArtifactClassEnum.StructureMap:
                case FhirArtifactClassEnum.ImplementationGuide:
                case FhirArtifactClassEnum.Unknown:
                default:
                    {
                        continue;
                    }
            }

        }

        _strongestBinding = _bindingStrengthByType.Any()
            ? _bindingStrengthByType.Values.Max()
            : FhirElement.ElementDefinitionBindingStrength.Example;

        _strongestExternalBinding = _externalBindingStrengthByType.Any()
            ? _externalBindingStrengthByType.Values.Max()
            : FhirElement.ElementDefinitionBindingStrength.Example;
    }

    /// <summary>Check path for data type.</summary>
    /// <param name="matchedRoots">The matched roots.</param>
    /// <param name="path">        Full pathname of the file.</param>
    /// <param name="root">        The element.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool CheckPathForDataType(
        HashSet<string> matchedRoots,
        string path,
        FhirComplex root)
    {
        FhirComplex complex = root;

        bool found = false;
        int index = path.IndexOf('.');
        int len = path.Length;
        while ((index != -1) && (index != len))
        {
            string subPath = path.Substring(0, index);

            if (!complex.Elements.ContainsKey(subPath))
            {
                // find next
                index = path.IndexOf('.', index + 1);
                continue;
            }

            if (complex.Elements[subPath].ElementTypes.Count > 1)
            {
                // this cannot be skipped
                continue;
            }

            string subType = complex.Elements[subPath].ElementTypes?.First().Key ?? complex.Elements[subPath].BaseTypeName;

            if (matchedRoots.Contains(subType))
            {
                found = true;
                break;
            }

            if (complex.Components.ContainsKey(subPath))
            {
                complex = complex.Components[subPath];
            }

            // find next
            index = path.IndexOf('.', index + 1);
        }

        return found;
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
        List<string> filterProperties = new();

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
                        filterProperties.Add(filter.Property + ":" + filter.Value);

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
        List<string> filterProperties = null)
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
        List<string> filterProperties = null)
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
    /// <returns>A FhirValueSet.</returns>
    public object Clone()
    {
        return new FhirValueSet(this);
    }
}
