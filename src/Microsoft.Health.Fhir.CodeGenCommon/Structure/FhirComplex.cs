// <copyright file="FhirComplex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Instance;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structure;

/// <summary>A FHIR complex.</summary>
public record class FhirComplex : FhirStructureBase, ICloneable
{
    /// <summary>
    /// Values that represent extension context type codes.
    /// http://hl7.org/fhir/ValueSet/extension-context-type
    /// </summary>
    public enum ExtensionContextTypeCodes
    {
        /// <summary>The context is all elements that match the FHIRPath query found in the expression.</summary>
        [FhirLiteral("fhirpath")]
        FhirPath,

        /// <summary>
        /// The context is any element that has an ElementDefinition.id that matches that found in the
        /// expression. This includes ElementDefinition Ids that have slicing identifiers. The full
        /// path for the element is [url]#[elementid]. If there is no #, the Element id is one defined
        /// in the base specification.
        /// </summary>
        [FhirLiteral("element")]
        Element,

        /// <summary>
        /// The context is a particular extension from a particular StructureDefinition, and the
        /// expression is just a uri that identifies the extension.
        /// </summary>
        [FhirLiteral("extension")]
        Extension,
    }

    /// <summary>
    /// An extension context - identifies the types of resource or data type elements to which the
    /// extension can be applied.
    /// </summary>
    public record class ExtensionContext : ICloneable
    {
        ExtensionContextTypeCodes? _extensionContextType;
        string _contextTypeLiteral = string.Empty;

        /// <summary>Initializes a new instance of the ExtensionContext class.</summary>
        public ExtensionContext() { }

        /// <summary>Initializes a new instance of the ExtensionContext class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected ExtensionContext(ExtensionContext other)
        {
            ContextTypeLiteral = other.ContextTypeLiteral;
            Expression = other.Expression;
        }

        /// <summary>
        /// Gets the type of the context record - defines how to interpret the expression
        /// that defines what the context of the extension is.
        /// </summary>
        public ExtensionContextTypeCodes? ContextType { get => _extensionContextType; }

        /// <summary>Gets or initializes the type of the FHIR context.</summary>
        public required string ContextTypeLiteral
        {
            get => _contextTypeLiteral;
            init
            {
                _contextTypeLiteral = value;
                _extensionContextType = value.ToEnum<ExtensionContextTypeCodes>();
            }
        }

        /// <summary>Gets the expression - where the extension can be used in instances.</summary>
        public required string Expression { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A stucture mapping.</summary>
    public record class StructureMapping : ICloneable
    {
        /// <summary>Gets or sets the internal id when this mapping is used.</summary>
        public required string Identity { get; init; }

        /// <summary>Gets or sets canonical uri - identifies what this mapping refers to.</summary>
        public string CanonicalUri { get; init; } = string.Empty;

        /// <summary>Gets or sets the name of what this mapping refers to.</summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>Gets or sets the comment - Versions, Issues, Scope limitations etc..</summary>
        public string Comment { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    private FhirElement _rootElement = null!;
    private Dictionary<string, FhirElement> _elements = new();
    private Dictionary<string, FhirComplex> _components = new();
    private Dictionary<string, FhirSearchParam> _searchParameters = new();
    private Dictionary<string, FhirOperation> _typeOperations = new();
    private Dictionary<string, FhirOperation> _instanceOperations = new();
    private IEnumerable<ExtensionContext> _extensionContexts = Enumerable.Empty<ExtensionContext>();
    private Dictionary<string, FhirConstraint> _constraints = new();
    private Dictionary<string, StructureMapping> _mappings = new();

    /// <summary>Initializes a new instance of the FhirComplex class.</summary>
    public FhirComplex() { }

    /// <summary>Initializes a new instance of the FhirComplex class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirComplex(FhirComplex other)
        : base(other)
    {
        ExplicitName = other.ExplicitName;
        IsAbstract = other.IsAbstract;
        Parent = other.Parent;
        ParentArtifactClass = other.ParentArtifactClass;
        DefiningPackageDirective = other.DefiningPackageDirective;
        _rootElement = other._rootElement with { };
        _elements = other._elements.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _components = other._components.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _searchParameters = other._searchParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _typeOperations = other._typeOperations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _instanceOperations = other._instanceOperations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _extensionContexts = other._extensionContexts.Select(v => v with { });
        _constraints = other._constraints.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _mappings = other._mappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
    }

    /// <summary>Gets the explicit name of this structure, if provided.</summary>
    public string ExplicitName { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether this object is abstract.</summary>
    public required bool IsAbstract { get; init; }

    /// <summary>Gets the parent artifact or null if this is a root definition.</summary>
    public required FhirComplex? Parent { get; init; }

    /// <summary>Gets or sets the parent artifact class.</summary>
    public required Models.FhirArtifactClassEnum ParentArtifactClass { get; init; }

    /// <summary>Gets or initializes the defining package directive.</summary>
    public required string DefiningPackageDirective { get; init; }

    /// <summary>Gets the root element.</summary>
    public FhirElement RootElement { get => _rootElement; set => _rootElement = value; }

    /// <summary>Gets the elements.</summary>
    public Dictionary<string, FhirElement> Elements { get => _elements; }

    /// <summary>Gets the components.</summary>
    public Dictionary<string, FhirComplex> Components { get => _components; }

    /// <summary>Gets the search parameters.</summary>
    public Dictionary<string, FhirSearchParam> SearchParameters { get => _searchParameters; }

    /// <summary>Gets the type operations.</summary>
    public Dictionary<string, FhirOperation> TypeOperations { get => _typeOperations; }

    /// <summary>Gets the instance operations.</summary>
    public Dictionary<string, FhirOperation> InstanceOperations { get => _instanceOperations; }

    /// <summary>Gets the extension contexts.</summary>
    public IEnumerable<ExtensionContext> ExtensionContexts { get => _extensionContexts; }

    /// <summary>Gets the constraints.</summary>
    public IEnumerable<FhirConstraint> Constraints { get => _constraints.Values; }

    /// <summary>Gets the constraints by key.</summary>
    public Dictionary<string, FhirConstraint> ConstraintsByKey { get => _constraints; }

    /// <summary>Gets the mappings - external specifications that the content is mapped to.</summary>
    public Dictionary<string, StructureMapping> Mappings { get => _mappings; init => _mappings = value; }

    /// <summary>Gets the root element mappings.</summary>
    public Dictionary<string, List<FhirElementMapping>> RootElementMappings { get => _rootElement.Mappings; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };

    /// <summary>Attempts to get explicit name.</summary>
    /// <param name="path">        Full pathname of the file.</param>
    /// <param name="explicitName">[out] Name of the explicit.</param>
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

    public bool GetParentAndFieldName(
        string url,
        string[] idComponents,
        string[] pathComponents,
        out FhirComplex? parent,
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
        out FhirComplex? parent,
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

        if (!_elements.TryGetValue(path, out FhirElement? pathElement) ||
            (pathElement == null))
        {
            // fail
            parent = null;
            field = string.Empty;
            sliceName = string.Empty;
            return false;
        }

        // check for needing to divert into a slice
        string nextIdSlice = GetSliceNameIfPresent(idComponents, startIndex + 1);

        if ((!string.IsNullOrEmpty(nextIdSlice)) &&
            pathElement.SlicesByName.TryGetValue(url, out FhirSlicing? elementSlicing) &&
            (elementSlicing != null) &&
            elementSlicing.TryGetValue(nextIdSlice, out FhirComplex slice))
        {
            // recurse into slice
            return slice.GetParentAndFieldNameRecurse(
                url,
                idComponents,
                pathComponents,
                startIndex + 1,
                out parent,
                out field,
                out sliceName);
        }

        // check for matching element, but no component
        if ((!_components.TryGetValue(path, out FhirComplex? pathComponent)) ||
            (pathComponent == null))
        {
            string elementType = pathElement.BaseTypeName;

            if (string.IsNullOrEmpty(elementType) && (pathElement.ElementTypes.Count > 0))
            {
                elementType = pathElement.ElementTypes.Values.ElementAt(0).Name;
            }

            pathComponent = new()
            {
                ArtifactClass = ArtifactClass,
                Id = pathElement.Id,
                Name = pathElement.Name,
                Path = pathElement.Path,
                ExplicitName = pathElement.ExplicitName,
                BaseTypeName = elementType,
                Version = Version,
                Url = pathElement.Url,
                PublicationStatus = PublicationStatus,
                FhirMaturityLevel = FhirMaturityLevel,
                IsExperimental = IsExperimental,
                ShortDescription = pathElement.ShortDescription,
                Purpose = pathElement.Purpose,
                Comment = pathElement.Comment,
                ValidationRegEx = pathElement.ValidationRegEx,
                NarrativeText = NarrativeText,
                NarrativeStatus = NarrativeStatus,
                FhirVersion = FhirVersion,
                Mappings = Mappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { }),
                IsAbstract = IsAbstract,
                Parent = this,
                ParentArtifactClass = ArtifactClass,
                DefiningPackageDirective = DefiningPackageDirective,
                RootElement = pathElement,
            };

            _components.Add(path, pathComponent);

            // update the element to have the type of the component we just added
            pathElement = pathElement with { BaseTypeName = elementType };
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
}
