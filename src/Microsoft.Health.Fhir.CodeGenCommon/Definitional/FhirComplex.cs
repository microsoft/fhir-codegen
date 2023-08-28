// <copyright file="FhirComplex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Definitional;

/// <summary>A FHIR complex.</summary>
public record class FhirComplex : FhirModelBase, ICloneable
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
    public Dictionary<string, StructureMapping> Mappings { get => _mappings; }

    /// <summary>Gets the root element mappings.</summary>
    public Dictionary<string, List<FhirElementMapping>> RootElementMappings { get => _rootElement.Mappings; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
