// <copyright file="FhirComplex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A class representing a FHIR complex type.</summary>
public class FhirComplex : FhirTypeBase
{
    private Dictionary<string, FhirComplex> _components;
    private Dictionary<string, FhirElement> _elements;
    private Dictionary<string, FhirSearchParam> _searchParameters;
    private Dictionary<string, FhirOperation> _typeOperations;
    private Dictionary<string, FhirOperation> _instanceOperations;
    private List<string> _contextElements;
    private List<FhirConstraint> _constraints;

    /// <summary>Initializes a new instance of the <see cref="FhirComplex"/> class.</summary>
    /// <param name="id">              The id of this resource/datatype/extension.</param>
    /// <param name="path">            The dot-notation path to this resource/datatype/extension.</param>
    /// <param name="explicitName">    Explicit name for this complex structure, if provided.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="isExperimental">  If this complex resource is flagged as experimental.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The purpose.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    public FhirComplex(
        string id,
        string path,
        string explicitName,
        Uri url,
        string standardStatus,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx)
        : base(
            id,
            path,
            url,
            standardStatus,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx)
    {
        _components = new Dictionary<string, FhirComplex>();
        _elements = new Dictionary<string, FhirElement>();
        _searchParameters = new Dictionary<string, FhirSearchParam>();
        _typeOperations = new Dictionary<string, FhirOperation>();
        _instanceOperations = new Dictionary<string, FhirOperation>();
        _constraints = new List<FhirConstraint>();
        _contextElements = new ();
        SliceName = string.Empty;
        ExplicitName = explicitName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirComplex"/> class.
    /// </summary>
    /// <param name="id">              The id of this resource/data type/extension.</param>
    /// <param name="path">            The dot-notation path to this resource/data type/extension.</param>
    /// <param name="explicitName">    Explicit name for this complex structure, if provided.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="isExperimental">  If this complex type is marked experimental.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The purpose.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    /// <param name="contextElements"> The context elements.</param>
    /// <param name="isAbstract">      If the complex structure is an abstract type.</param>
    public FhirComplex(
        string id,
        string path,
        string explicitName,
        Uri url,
        string standardStatus,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        List<string> contextElements,
        bool isAbstract)
        : this(
            id,
            path,
            explicitName,
            url,
            standardStatus,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx)
    {
        _contextElements = contextElements;
        IsAbstract = isAbstract;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirComplex"/> class.</summary>
    /// <param name="id">              The id of this resource/datatype/extension.</param>
    /// <param name="path">            The dot-notation path to this resource/datatype/extension.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="explicitName">    Explicit name for this complex structure, if provided.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="isExperimental">  If this complex structure is considered experimental.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The purpose.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    /// <param name="baseTypeName">    Name of the base type.</param>
    public FhirComplex(
        string id,
        string path,
        string explicitName,
        Uri url,
        string standardStatus,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        string baseTypeName)
        : this(
            id,
            path,
            explicitName,
            url,
            standardStatus,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx)
    {
        BaseTypeName = baseTypeName;
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
    }

    /// <summary>Gets the explicit name of this structure, if provided.</summary>
    public string ExplicitName { get; }

    /// <summary>Gets a value indicating whether this object is abstract.</summary>
    public bool IsAbstract { get; }

    /// <summary>Gets or sets a value indicating whether this object is placeholder.</summary>
    /// <value>True if this object is placeholder, false if not.</value>
    public bool IsPlaceholder { get; set; }

    /// <summary>Gets or sets the name of the slice.</summary>
    /// <value>The name of the slice.</value>
    public string SliceName { get; set; }

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
    public List<FhirConstraint> Constraints { get => _constraints; }

    /// <summary>Adds a constraint.</summary>
    /// <param name="constraint">The constraint.</param>
    public void AddConstraint(FhirConstraint constraint)
    {
        _constraints.Add(constraint);
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

        // create a new complex type from the property
        _components.Add(
            property.Path,
            new FhirComplex(
                property.Id,
                property.Path,
                property.ExplicitName,
                property.URL,
                property.StandardStatus,
                property.IsExperimental,
                property.ShortDescription,
                property.Purpose,
                property.Comment,
                property.ValidationRegEx,
                elementType));

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

    /// <summary>Deep copy - cannot use Clone because of needed parameters.</summary>
    /// <param name="primitiveTypeMap">     The primitive type map.</param>
    /// <param name="copySlicing">          True to copy slicing.</param>
    /// <param name="canHideParentFields">  True if can hide parent fields, false if not.</param>
    /// <param name="valueSetReferences">   [in,out] Value Set URLs and lists of FHIR paths that
    ///  reference them.</param>
    /// <param name="typeMapByPath">        [in,out] Type mappings by path.</param>
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
        Dictionary<string, FhirServerSearchParam> supportedSearchParams = null,
        Dictionary<string, FhirServerSearchParam> serverSearchParams = null,
        Dictionary<string, FhirServerOperation> supportedOperations = null,
        Dictionary<string, FhirServerOperation> serverOperations = null,
        bool includeExperimental = false)
    {
        List<string> contextElements = null;

        if (ContextElements != null)
        {
            contextElements = new List<string>();
            foreach (string contextElement in ContextElements)
            {
                contextElements.Add(new string(contextElement));
            }
        }

        // generate our base copy
        FhirComplex complex = new FhirComplex(
                Id,
                Path,
                ExplicitName,
                URL,
                StandardStatus,
                IsExperimental,
                ShortDescription,
                Purpose,
                Comment,
                ValidationRegEx,
                ContextElements,
                IsAbstract);

        if (!string.IsNullOrEmpty(SliceName))
        {
            complex.SliceName = this.SliceName;
        }

        if (!string.IsNullOrEmpty(BaseTypeName))
        {
            if ((primitiveTypeMap != null) && primitiveTypeMap.ContainsKey(this.BaseTypeName))
            {
                complex.BaseTypeName = primitiveTypeMap[this.BaseTypeName];
            }
            else
            {
                complex.BaseTypeName = this.BaseTypeName;
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
                    valueSetReferences);

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

        if (_constraints != null)
        {
            complex._constraints = _constraints.Select(c => (FhirConstraint)c.Clone()).ToList();
        }

        return complex;
    }
}
