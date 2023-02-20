// <copyright file="FhirComplex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Xml.Linq;
using System;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A class representing a FHIR complex type.</summary>
public class FhirComplex : FhirModelBase, ICloneable
{
    private Dictionary<string, FhirComplex> _components;
    private FhirElement _rootElement;
    private Dictionary<string, FhirElement> _elements;
    private Dictionary<string, FhirSearchParam> _searchParameters;
    private Dictionary<string, FhirOperation> _typeOperations;
    private Dictionary<string, FhirOperation> _instanceOperations;
    private List<string> _contextElements;
    private Dictionary<string, FhirConstraint> _constraintsByKey;

    /// <summary>Initializes a new instance of the <see cref="FhirComplex"/> class.</summary>
    /// <param name="artifactType">    FHIR artifact type of this Complex object.</param>
    /// <param name="id">              The id of this resource/datatype/extension.</param>
    /// <param name="name">            Name of this definition.</param>
    /// <param name="path">            The dot-notation path to this resource/datatype/extension.</param>
    /// <param name="explicitName">    Explicit name for this complex structure, if provided.</param>
    /// <param name="baseTypeName">    Base type of this complex structure, if provided.</param>
    /// <param name="baseTypeCanonical">Base type canonical of this complex structure, if provided</param>
    /// <param name="version">         Version of this definition.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="fmmLevel">        The FHIR Maturity Model level.</param>
    /// <param name="isExperimental">  If this complex resource is flagged as experimental.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The purpose.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> Validation Regular Expression for this structure.</param>
    /// <param name="narrative">       Narrative content for this object.</param>
    /// <param name="narrativeStatus"> Status of any included narrative content.</param>
    /// <param name="fhirVersion">     FHIR Version specified by this object.</param>
    /// <param name="mappings">        Mapping definitions for links to external specifications.</param>
    /// <param name="rootElementMappings">Mapping values from the root element.</param>
    public FhirComplex(
        FhirArtifactClassEnum artifactType,
        string id,
        string name,
        string path,
        string explicitName,
        string baseTypeName,
        string baseTypeCanonical,
        string version,
        Uri url,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        string narrative,
        string narrativeStatus,
        string fhirVersion,
        Dictionary<string, FhirStructureDefMapping> mappings,
        Dictionary<string, List<FhirElementDefMapping>> rootElementMappings)
        : base(
            artifactType,
            id,
            name,
            path,
            baseTypeName,
            baseTypeCanonical,
            version,
            url,
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx,
            narrative,
            narrativeStatus,
            fhirVersion)
    {
        _components = new();
        _rootElement = null;
        _elements = new();
        _searchParameters = new();
        _typeOperations = new();
        _instanceOperations = new();
        _constraintsByKey = new();
        _contextElements = new();
        SliceName = string.Empty;
        ExplicitName = explicitName;
        Parent = null;
        ParentArtifactClass = FhirArtifactClassEnum.Unknown;
        ResolvedParentDirective = string.Empty;
        Mappings = mappings ?? new();
        RootElementMappings = rootElementMappings ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirComplex"/> class.
    /// </summary>
    /// <param name="artifactType">    Type of artifact this complex object represents.</param>
    /// <param name="id">              The id of this resource/data type/extension.</param>
    /// <param name="name">            The 'name' of this model.</param>
    /// <param name="explicitName">    Explicit name for this complex structure, if provided.</param>
    /// <param name="baseTypeName">    Base type name for this complex structure, if provided.</param>
    /// <param name="baseTypeCanonical">Base type canonical of this complex structure, if provided</param>
    /// <param name="version">         Version of this definition.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="fmmLevel">        The FHIR Maturity Model level.</param>
    /// <param name="isExperimental">  If this complex type is marked experimental.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The definition.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="contextElements"> The context elements.</param>
    /// <param name="isAbstract">      If the complex structure is an abstract type.</param>
    /// <param name="validationRegEx"> Validation regex pattern for this definition.</param>
    /// <param name="narrative">       Narrative content for this object.</param>
    /// <param name="narrativeStatus"> Status of any included narrative content.</param>
    /// <param name="fhirVersion">     FHIR Version specified by this object.</param>
    /// <param name="mappings">        Mapping definitions for links to external specifications.</param>
    public FhirComplex(
        FhirArtifactClassEnum artifactType,
        string id,
        string name,
        string path,
        string explicitName,
        string baseTypeName,
        string baseTypeCanonical,
        string version,
        Uri url,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        List<string> contextElements,
        bool isAbstract,
        string validationRegEx,
        string narrative,
        string narrativeStatus,
        string fhirVersion,
        Dictionary<string, FhirStructureDefMapping> mappings,
        Dictionary<string, List<FhirElementDefMapping>> rootElementMappings)
        : this(
            artifactType,
            id,
            name,
            path,
            explicitName,
            baseTypeName,
            baseTypeCanonical,
            version,
            url,
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx,
            narrative,
            narrativeStatus,
            fhirVersion,
            mappings,
            rootElementMappings)
    {
        _contextElements = contextElements;
        IsAbstract = isAbstract;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirComplex"/> class for a slice.
    /// </summary>
    /// <param name="artifactType">    Type of artifact this complex object represents.</param>
    /// <param name="id">              The id of this resource/data type/extension.</param>
    /// <param name="elementName">     The element name this model is defined by (e.g., the path to a backbone element).</param>
    /// <param name="sliceName">       Name of this slice.</param>
    /// <param name="path">            Path to this definition.</param>
    /// <param name="explicitName">    Explicit name for this complex structure, if provided.</param>
    /// <param name="baseTypeName">    Base type name for this complex structure, if provided.</param>
    /// <param name="baseTypeCanonical">Base type canonical of this complex structure, if provided</param>
    /// <param name="version">         Version of this definition.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="fmmLevel">        The FHIR Maturity Model level.</param>
    /// <param name="isExperimental">  If this complex type is marked experimental.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The definition.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="contextElements"> The context elements.</param>
    /// <param name="isAbstract">      If the complex structure is an abstract type.</param>
    /// <param name="validationRegEx"> Validation regex pattern for this definition.</param>
    /// <param name="narrative">       Narrative content for this object.</param>
    /// <param name="narrativeStatus"> Status of any included narrative content.</param>
    /// <param name="fhirVersion">     FHIR Version specified by this object.</param>
    /// <param name="mappings">        Mapping definitions for links to external specifications.</param>
    public FhirComplex(
        FhirArtifactClassEnum artifactType,
        string id,
        string elementName,
        string sliceName,
        string path,
        string explicitName,
        string version,
        Uri url,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        string narrative,
        string narrativeStatus,
        string fhirVersion,
        Dictionary<string, FhirStructureDefMapping> mappings,
        Dictionary<string, List<FhirElementDefMapping>> rootElementMappings)
        : this(
            artifactType,
            id,
            elementName,
            path,
            explicitName,
            string.Empty,
            string.Empty,
            version,
            url,
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx,
            narrative,
            narrativeStatus,
            fhirVersion,
            mappings,
            rootElementMappings)
    {
        SliceName = sliceName;
    }

    /// <summary>
    /// Initializes a new instance of the Microsoft.Health.Fhir.CodeGenCommon.Models.FhirComplex
    /// class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirComplex(FhirComplex source)
    : base(
        source.ArtifactClass,
        source.Id,
        source.Name,
        source.Path,
        source.BaseTypeName,
        source._baseTypeCanonical,
        source.Version,
        source.URL,
        source.PublicationStatus,
        source.StandardStatus,
        source.FhirMaturityLevel,
        source.IsExperimental,
        source.ShortDescription,
        source.Purpose,
        source.Comment,
        source.ValidationRegEx,
        source.NarrativeText,
        source.NarrativeStatus,
        source.FhirVersion)
    {
        ExplicitName = source.ExplicitName;
        IsAbstract = source.IsAbstract;
        IsPlaceholder = source.IsPlaceholder;
        Parent = source.Parent;
        ParentArtifactClass = source.ParentArtifactClass;
        ResolvedParentDirective = source.ResolvedParentDirective;
        SliceName = source.SliceName;
        _rootElement = (FhirElement)source._rootElement?.Clone() ?? null;
        _elements = source._elements?.DeepCopy() ?? null;
        _components = source._components?.DeepCopy() ?? null;
        _searchParameters = source._searchParameters?.DeepCopy() ?? null;
        _typeOperations = source._typeOperations?.DeepCopy() ?? null;
        _instanceOperations = source._instanceOperations?.DeepCopy() ?? null;
        _contextElements = source._contextElements?.Select(v => v).ToList() ?? null;
        _constraintsByKey = source._constraintsByKey?.DeepCopy() ?? null;
        Mappings = source.Mappings?.DeepCopy() ?? null;
        RootElementMappings = source.RootElementMappings?.DeepCopy() ?? null;
    }

    /// <summary>Values that represent fhir complex types.</summary>
    public enum FhirComplexType
    {
        /// <summary>An enum constant representing the data type option.</summary>
        DataType,

        /// <summary>An enum constant representing the resource option.</summary>
        Resource,

        /// <summary>An enum constant representing the extension option.</summary>
        Extension,

        /// <summary>An enum constant representing the profile option.</summary>
        Profile,

        /// <summary>An enum constant representing the logical model option.</summary>
        LogicalModel,
    }

    /// <summary>Gets the explicit name of this structure, if provided.</summary>
    public string ExplicitName { get; }

    /// <summary>Gets a value indicating whether this object is abstract.</summary>
    public bool IsAbstract { get; }

    /// <summary>Gets or sets a value indicating whether this object is placeholder.</summary>
    /// <value>True if this object is placeholder, false if not.</value>
    public bool IsPlaceholder { get; set; }

    /// <summary>Gets or sets the parent.</summary>
    public FhirComplex Parent { get; set; }

    /// <summary>Gets or sets the parent artifact class.</summary>
    public FhirArtifactClassEnum ParentArtifactClass { get; set; }

    /// <summary>Gets or sets the resolved parent directive.</summary>
    public string ResolvedParentDirective { get; set; }

    /// <summary>Gets or sets the name of the slice.</summary>
    /// <value>The name of the slice.</value>
    public string SliceName { get; set; }

    /// <summary>Gets the root element.</summary>
    public FhirElement RootElement { get => _rootElement; }

    /// <summary>Gets the elements.</summary>
    /// <value>The elements.</value>
    public Dictionary<string, FhirElement> Elements { get => _elements; }

    /// <summary>Gets the components.</summary>
    /// <value>The components.</value>
    public Dictionary<string, FhirComplex> Components { get => _components; }

    /// <summary>Gets the search parameters.</summary>
    /// <value>Search Parameters defined on this resource.</value>
    public Dictionary<string, FhirSearchParam> SearchParameters { get => _searchParameters; }

    /// <summary>Gets the type operations.</summary>
    /// <value>The type operations.</value>
    public Dictionary<string, FhirOperation> TypeOperations { get => _typeOperations; }

    /// <summary>Gets the instance operations.</summary>
    /// <value>The instance operations.</value>
    public Dictionary<string, FhirOperation> InstanceOperations { get => _instanceOperations; }

    /// <summary>Gets the context elements.</summary>
    /// <value>The context elements.</value>
    public List<string> ContextElements { get => _contextElements; }

    /// <summary>Gets the constraints.</summary>
    public IEnumerable<FhirConstraint> Constraints { get => _constraintsByKey.Values; }

    /// <summary>Gets the mappings - external specifications that the content is mapped to.</summary>
    public Dictionary<string, FhirStructureDefMapping> Mappings { get; } = new();

    /// <summary>Gets the root element mappings.</summary>
    public Dictionary<string, List<FhirElementDefMapping>> RootElementMappings { get; } = new();

    /// <summary>Adds a constraint.</summary>
    /// <param name="constraint">The constraint.</param>
    public void AddConstraint(FhirConstraint constraint)
    {
        if (_constraintsByKey.ContainsKey(constraint.Key))
        {
            return;
        }

        _constraintsByKey.Add(constraint.Key, constraint);
    }

    /// <summary>Adds a search parameter.</summary>
    /// <param name="searchParam">The search parameter.</param>
    public void AddSearchParameter(FhirSearchParam searchParam)
    {
        if (_searchParameters.ContainsKey(searchParam.Code))
        {
            return;
        }

        // add this parameter
        _searchParameters.Add(searchParam.Code, searchParam);
    }

    /// <summary>Adds an operation.</summary>
    /// <param name="operation">The operation.</param>
    public void AddOperation(FhirOperation operation)
    {
        if (operation.DefinedOnType)
        {
            if (!_typeOperations.ContainsKey(operation.Code))
            {
                _typeOperations.Add(operation.Code, operation);
            }
        }

        if (operation.DefinedOnInstance)
        {
            if (!_instanceOperations.ContainsKey(operation.Code))
            {
                _instanceOperations.Add(operation.Code, operation);
            }
        }
    }

    /// <summary>Adds a root element.</summary>
    /// <param name="element">The element.</param>
    public void AddRootElement(FhirElement element)
    {
        _rootElement = element;
    }

    /// <summary>Adds a context element.</summary>
    /// <param name="element">The element.</param>
    public void AddContextElement(string element)
    {
        if (_contextElements == null)
        {
            _contextElements = new List<string>();
        }

        _contextElements.Add(element);
    }

    /// <summary>Adds a component from an element.</summary>
    /// <param name="path">Name of the element.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool AddComponentFromElement(string path)
    {
        if ((!_elements.ContainsKey(path)) ||
            _components.ContainsKey(path))
        {
            return false;
        }

        FhirElement property = _elements[path];

        string elementType = property.BaseTypeName;

        if (string.IsNullOrEmpty(elementType) && (property.ElementTypes.Count > 0))
        {
            elementType = property.ElementTypes.Values.ElementAt(0).Name;
        }

        // TODO: adding name test
        if (_components.ContainsKey(property.Name))
        {
            // change the element to point at the correct area
            _elements[path].BaseTypeName = property.Path;

            return true;
        }

        // TODO: changed Name to path here

        // create a new complex type from the property
        _components.Add(
            property.Path,
            new FhirComplex(
                ArtifactClass,
                property.Id,
                property.Name,
                property.Path,
                property.ExplicitName,
                elementType,
                string.Empty,
                Version,
                property.URL,
                PublicationStatus,
                StandardStatus,
                FhirMaturityLevel,
                IsExperimental,
                property.ShortDescription,
                property.Purpose,
                property.Comment,
                property.ValidationRegEx,
                NarrativeText,
                NarrativeStatus,
                FhirVersion,
                Mappings.DeepCopy(),
                RootElementMappings.DeepCopy()));

        // change the element to point at the new area
        _elements[path].BaseTypeName = property.Path;

        return true;
    }

    /// <summary>Attempts to get an explicit name for a component path.</summary>
    /// <param name="path">        Name of the element.</param>
    /// <param name="explicitName">[out] Explicit name for this complex structure, if provided.</param>
    /// <param name="startIndex">  (Optional) The start index.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetExplicitName(
        string path,
        out string explicitName,
        int startIndex = 0)
    {
        if (string.IsNullOrEmpty(path))
        {
            explicitName = string.Empty;
            return false;
        }

        int index = path.IndexOf('.', startIndex + 1);

        string currentPath;

        if (index != -1)
        {
            currentPath = path.Substring(0, index);
        }
        else
        {
            currentPath = path;
        }

        if (_components.ContainsKey(currentPath))
        {
            if (index == -1)
            {
                explicitName = _components[currentPath].ExplicitName;

                if (string.IsNullOrEmpty(explicitName))
                {
                    explicitName = string.Empty;
                    return false;
                }

                return true;
            }

            return _components[currentPath].TryGetExplicitName(path, out explicitName, index);
        }

        if (_elements.ContainsKey(currentPath))
        {
            explicitName = _elements[currentPath].ExplicitName;

            if (string.IsNullOrEmpty(explicitName))
            {
                explicitName = string.Empty;
                return false;
            }

            return true;
        }

        explicitName = string.Empty;
        return false;
    }

    /// <summary>Gets the parent and field name.</summary>
    /// <param name="url">           URL of the resource.</param>
    /// <param name="idComponents">  The id components.</param>
    /// <param name="pathComponents">The path components.</param>
    /// <param name="parent">        [out] The parent.</param>
    /// <param name="field">         [out] The field.</param>
    /// <param name="sliceName">     [out] Name of the slice.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool GetParentAndFieldName(
        string url,
        string[] idComponents,
        string[] pathComponents,
        out FhirComplex parent,
        out string field,
        out string sliceName)
    {
        // sanity checks - need at least 2 path components to have a parent
        if ((idComponents == null) || (idComponents.Length < 2) ||
            (pathComponents == null) || (pathComponents.Length < 2))
        {
            parent = null;
            field = string.Empty;
            sliceName = string.Empty;
            return false;
        }

        // find the parent and field name
        return GetParentAndFieldNameRecurse(
            url,
            idComponents,
            pathComponents,
            0,
            out parent,
            out field,
            out sliceName);
    }

    /// <summary>Gets the parent and field name, recursively.</summary>
    /// <param name="url">           URL of the resource.</param>
    /// <param name="idComponents">  The id components.</param>
    /// <param name="pathComponents">The path components.</param>
    /// <param name="startIndex">    The start index.</param>
    /// <param name="parent">        [out] The parent.</param>
    /// <param name="field">         [out] The field.</param>
    /// <param name="sliceName">     [out] Name of the slice.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool GetParentAndFieldNameRecurse(
        string url,
        string[] idComponents,
        string[] pathComponents,
        int startIndex,
        out FhirComplex parent,
        out string field,
        out string sliceName)
    {
        // check for being the parent to the field
        if (startIndex == (pathComponents.Length - 2))
        {
            // check for slice name on field
            sliceName = GetSliceNameIfPresent(idComponents, idComponents.Length - 1);

            parent = this;
            field = pathComponents[pathComponents.Length - 1];

            return true;
        }

        // build the path to the next item in the path
        string path = DotForComponents(pathComponents, 0, startIndex + 1);

        // check for needing to divert into a slice
        string nextIdSlice = GetSliceNameIfPresent(idComponents, startIndex + 1);
        if (_elements.ContainsKey(path) &&
            (!string.IsNullOrEmpty(nextIdSlice)))
        {
            // recurse into slice
            return _elements[path].Slicing[url][nextIdSlice].GetParentAndFieldNameRecurse(
                url,
                idComponents,
                pathComponents,
                startIndex + 1,
                out parent,
                out field,
                out sliceName);
        }

        // check for matching element, but no component
        if (_elements.ContainsKey(path) &&
            (!_components.ContainsKey(path)))
        {
            // add component from the element
            AddComponentFromElement(path);
        }

        // check Components for match
        if (_components.ContainsKey(path))
        {
            // recurse
            return _components[path].GetParentAndFieldNameRecurse(
                url,
                idComponents,
                pathComponents,
                startIndex + 1,
                out parent,
                out field,
                out sliceName);
        }

        // fail
        parent = null;
        field = string.Empty;
        sliceName = string.Empty;
        return false;
    }

    /// <summary>Attempts to get slice.</summary>
    /// <param name="idComponents">The id components.</param>
    /// <param name="index">       Zero-based index of the.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static string GetSliceNameIfPresent(string[] idComponents, int index)
    {
        string[] split = idComponents[index].Split(':');

        if (split.Length == 1)
        {
            return string.Empty;
        }

        return split[1];
    }

    /// <summary>Path for components.</summary>
    /// <param name="components">The components.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">  The end index.</param>
    /// <returns>A string.</returns>
    private static string DotForComponents(
        string[] components,
        int startIndex,
        int endIndex)
    {
        string val = components[startIndex];

        for (int i = startIndex + 1; i <= endIndex; i++)
        {
            val += $".{components[i]}";
        }

        return val;
    }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirComplex(this);
    }

    /// <summary>Deep copy - cannot use Clone because of needed parameters.</summary>
    /// <param name="artifactType">         Type of the artifact.</param>
    /// <param name="primitiveTypeMap">     The primitive type map.</param>
    /// <param name="copySlicing">          True to copy slicing.</param>
    /// <param name="canHideParentFields">  True if can hide parent fields, false if not.</param>
    /// <param name="valueSetReferences">   Value Set URLs and lists of FHIR paths that reference
    ///  them.</param>
    /// <param name="typeMapByPath">        Type mappings by path.</param>
    /// <param name="supportedSearchParams">(Optional) Options for controlling the supported search.</param>
    /// <param name="serverSearchParams">   (Optional) Options for controlling the server search.</param>
    /// <param name="supportedOperations">  (Optional) The supported operations.</param>
    /// <param name="serverOperations">     (Optional) The server operations.</param>
    /// <param name="includeExperimental">  (Optional) True to include, false to exclude the
    ///  experimental.</param>
    /// <returns>A FhirComplex.</returns>
    public FhirComplex DeepCopy(
        Dictionary<string, string> primitiveTypeMap,
        bool copySlicing,
        bool canHideParentFields,
        Dictionary<string, ValueSetReferenceInfo> valueSetReferences,
        Dictionary<string, FhirNodeInfo> typeMapByPath,
        Dictionary<string, FhirCapSearchParam> supportedSearchParams = null,
        Dictionary<string, FhirCapSearchParam> serverSearchParams = null,
        Dictionary<string, FhirCapOperation> supportedOperations = null,
        Dictionary<string, FhirCapOperation> serverOperations = null,
        bool includeExperimental = false)
    {
        List<string> contextElements = ContextElements?.Select(s => new string(s)).ToList() ?? null;

        // generate our base copy
        FhirComplex complex = new FhirComplex(
            ArtifactClass,
            Id,
            Name,
            Path,
            ExplicitName,
            BaseTypeName,
            BaseTypeCanonical,
            Version,
            URL,
            PublicationStatus,
            StandardStatus,
            FhirMaturityLevel,
            IsExperimental,
            ShortDescription,
            Purpose,
            Comment,
            ContextElements,
            IsAbstract,
            ValidationRegEx,
            NarrativeText,
            NarrativeStatus,
            FhirVersion,
            Mappings.DeepCopy(),
            RootElementMappings.DeepCopy());

        complex._rootElement = _rootElement;
        complex.SliceName = SliceName;

        if (!string.IsNullOrEmpty(BaseTypeName))
        {
            if ((primitiveTypeMap != null) && primitiveTypeMap.ContainsKey(BaseTypeName))
            {
                complex.BaseTypeName = primitiveTypeMap[BaseTypeName];
            }
            else
            {
                complex.BaseTypeName = BaseTypeName;
            }
        }

        // copy elements - must retain field order!
        foreach (FhirElement element in _elements.Values.OrderBy(s => s.FieldOrder))
        {
            // check for hiding a parent field
            if ((!canHideParentFields) && element.HidesParent)
            {
                continue;
            }

            FhirElement copied = element.DeepCopy(
                    primitiveTypeMap,
                    copySlicing,
                    canHideParentFields,
                    valueSetReferences,
                    complex);

            complex.Elements.Add(element.Path, copied);
        }

        // copy backbone elements (unordered)
        foreach (KeyValuePair<string, FhirComplex> kvp in _components)
        {
            FhirComplex node = kvp.Value.DeepCopy(
                primitiveTypeMap,
                copySlicing,
                canHideParentFields,
                valueSetReferences,
                typeMapByPath);

            if (typeMapByPath != null)
            {
                typeMapByPath.Add(
                    node.Path,
                    new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Component, node));
            }

            complex.Components.Add(
                kvp.Key,
                node);
        }

        // search
        if (supportedSearchParams == null)
        {
            foreach (KeyValuePair<string, FhirSearchParam> kvp in _searchParameters)
            {
                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                complex.SearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
            }
        }
        else if (serverSearchParams == null)
        {
            foreach (KeyValuePair<string, FhirSearchParam> kvp in _searchParameters)
            {
                if (!supportedSearchParams.ContainsKey(kvp.Key))
                {
                    continue;
                }

                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                complex.SearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
            }
        }
        else
        {
            foreach (KeyValuePair<string, FhirSearchParam> kvp in _searchParameters)
            {
                if ((!supportedSearchParams.ContainsKey(kvp.Key)) &&
                    (!serverSearchParams.ContainsKey(kvp.Key)))
                {
                    continue;
                }

                complex.SearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
            }
        }

        // type operations
        if (supportedOperations == null)
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in _typeOperations)
            {
                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                complex.TypeOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }
        else
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in _typeOperations)
            {
                if (!supportedOperations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                complex.TypeOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }

        // instance operations
        if (supportedOperations == null)
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in _instanceOperations)
            {
                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                complex.InstanceOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }
        else if (serverOperations == null)
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in _instanceOperations)
            {
                if (!supportedOperations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                complex.InstanceOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }
        else
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in _instanceOperations)
            {
                if ((!supportedOperations.ContainsKey(kvp.Key)) &&
                    (!serverOperations.ContainsKey(kvp.Key)))
                {
                    continue;
                }

                complex.InstanceOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }

        complex._constraintsByKey = _constraintsByKey.DeepCopy();

        return complex;
    }
}
