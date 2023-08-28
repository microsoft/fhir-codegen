// <copyright file="FhirCodeSystem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR code system.</summary>
public record class FhirCodeSystem : FhirCanonicalBase, ICloneable
{
    /// <summary>
    /// Values that represent meanings of the hierarchy of concepts as represented in this resource.
    /// </summary>
    public enum HierarchyMeaningCodes
    {
        [FhirLiteral("grouped-by")]
        GroupedBy,

        [FhirLiteral("is-a")]
        IsA,

        [FhirLiteral("part-of")]
        PartOf,

        [FhirLiteral("classified-with")]
        ClassifiedWith,
    }

    /// <summary>Values that represent content mode codes.</summary>
    public enum ContentModeCodes
    {
        [FhirLiteral("not-present")]
        NotPresent,

        [FhirLiteral("example")]
        Example,

        [FhirLiteral("fragment")]
        Fragment,

        [FhirLiteral("complete")]
        Complete,

        [FhirLiteral("supplement")]
        Supplement,
    }

    /// <summary>A CodeSystem concept.</summary>
    public record class Concept : FhirCoding, ICloneable
    {
        private Dictionary<string, Concept> _children = new();

        /// <summary>Initializes a new instance of the Concept class.</summary>
        public Concept() : base() { }

        /// <summary>Initializes a new instance of the Concept class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected Concept(Concept other)
            : base(other)
        {
            Children = other.Children.Select(v => v with { });
            if (other.Parent != null)
            {
                if (_children!.ContainsKey(other.Parent.Code))
                {
                    _children[other.Parent.Code] = other.Parent with { Children = Children };
                }
                else
                {
                    _children!.Add(other.Parent.Code, other.Parent with { Children = Children });
                }
            }
        }

        /// <summary>Gets or initializes the parent.</summary>
        public required Concept? Parent { get; init; }

        /// <summary>Gets a value indicating whether this object is root.</summary>
        public bool IsRoot => Parent is null;

        /// <summary>Gets the children concepts, by code.</summary>
        public IEnumerable<Concept> Children
        {
            get => _children.Values;
            init
            {
                _children = value.ToDictionary(v => v.Code, v => v);
            }
        }

        /// <summary>Gets the children by code.</summary>
        public Dictionary<string, Concept> ChildrenByCode { get => _children; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A code system filter.</summary>
    public record class FilterDefinition : FhirElementBase, ICloneable
    {
        private HashSet<string> _operators = new();

        /// <summary>Initializes a new instance of the <see cref="FilterDefinition"/> class.</summary>
        public FilterDefinition() : base() { }

        /// <summary>Initializes a new instance of the <see cref="FilterDefinition"/> class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected FilterDefinition(FilterDefinition other)
            : base(other)
        {
            Code = other.Code;
            Description = other.Description;
            Operators = other.Operators.Select(v => v);
            Value = other.Value;
        }

        /// <summary>Gets the code that identifies the filter.</summary>
        public required string Code { get; init; }

        /// <summary>Gets the description - how or why the filter is used.</summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>Gets the list of operators that can be used with the filter.</summary>
        public required IEnumerable<string> Operators
        {
            get => _operators.AsEnumerable();
            init
            {
                _operators = value.ToHashSet();
            }
        }

        /// <summary>Gets the set the operator belongs to.</summary>
        public HashSet<string> OperatorSet { get => _operators; }

        /// <summary>Gets the filter value.</summary>
        public required string Value { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    private readonly Dictionary<string, Concept> _concepts = new();

    private HierarchyMeaningCodes? _hierarchyMeaning = null;
    private string _hierarchyMeaningLiteral = string.Empty;

    private ContentModeCodes _contentModeCodes = ContentModeCodes.NotPresent;
    private string _contentModeLiteral = string.Empty;

    /// <summary>Initializes a new instance of the FhirCodeSystem class.</summary>
    public FhirCodeSystem() : base() { }

    /// <summary>Initializes a new instance of the FhirCodeSystem class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirCodeSystem(FhirCodeSystem other)
        :base(other)
    {
        Content = other.Content;
        IsCaseSensitive = other.IsCaseSensitive;
        CompleteValueSetCanonical = other.CompleteValueSetCanonical;
        HierarchyMeaningLiteral = other.HierarchyMeaningLiteral;
        IsCompositional = other.IsCompositional;
        ContentModeLiteral = other.ContentModeLiteral;
        ConceptCount = other.ConceptCount;
        Filters = other.Filters.Select(v => v with { });
        Concepts = other.Concepts.Select(v => v with { });
        if (other.RootConcept is not null)
        {
            if (_concepts!.ContainsKey(other.RootConcept.Code))
            {
                RootConcept = _concepts[other.RootConcept.Code];
            }
            else
            {
                RootConcept = other.RootConcept with { };
            }
        }
    }

    /// <summary>
    /// Gets the extent of the content of the code system (the concepts and codes it defines) are
    /// represented in this resource instance.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>Gets a flag for if code comparison is case sensitive.</summary>
    public bool? IsCaseSensitive { get; init; } = null;

    /// <summary>Gets a canonical reference to the value set with entire code system.</summary>
    public string CompleteValueSetCanonical { get; init; } = string.Empty;

    /// <summary>Gets the meaning of the hierarchy of concepts as represented in this resource.</summary>
    public HierarchyMeaningCodes? HierarchyMeaning { get => _hierarchyMeaning; }

    /// <summary>Gets or initializes the literal for the HierarchyMeaning</summary>
    public string HierarchyMeaningLiteral
    {
        get => _hierarchyMeaningLiteral;
        init
        {
            _hierarchyMeaningLiteral = value;
            _hierarchyMeaning = value.ToEnum<HierarchyMeaningCodes>();
        }
    }

    /// <summary>Gets if the code system defines a compositional grammar.</summary>
    public bool? IsCompositional { get; init; } = null;

    /// <summary>Gets the content mode.</summary>
    public ContentModeCodes ContentMode { get => _contentModeCodes; }

    /// <summary>Gets or initializes the content mode literal.</summary>
    public required string ContentModeLiteral
    {
        get => _contentModeLiteral;
        init
        {
            _contentModeLiteral = value;
            _contentModeCodes = value.ToEnum<ContentModeCodes>() ?? ContentModeCodes.NotPresent;
        }
    }

    /// <summary>Gets or initializes the number of concepts.</summary>
    public uint? ConceptCount { get; init; } = null;

    /// <summary>Gets the filters.</summary>
    public IEnumerable<FilterDefinition> Filters { get; init; } = Enumerable.Empty<FilterDefinition>();

    /// <summary>Gets the properties.</summary>
    public IEnumerable<FhirCoding.PropertyDefinition> Properties { get; init; } = Enumerable.Empty<FhirCoding.PropertyDefinition>();

    /// <summary>Gets the root concept.</summary>
    public Concept? RootConcept { get; init; } = null;

    /// <summary>Gets the concepts.</summary>
    public IEnumerable<Concept> Concepts
    {
        get => _concepts.Values;
        init
        {
            _concepts = value.ToDictionary(c => c.Code, c => c);
        }
    }

    /// <summary>Indexer to get slices based on name.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="code">The code.</param>
    /// <returns>The indexed item.</returns>
    public Concept this[string code]
    {
        get
        {
            if (!_concepts.ContainsKey(code))
            {
                throw new ArgumentOutOfRangeException(nameof(code));
            }

            return _concepts[code];
        }
    }

    /// <summary>Query if this system contains a concept, specified by code.</summary>
    /// <param name="code">The code.</param>
    /// <returns>True if this system has the concept, false if it does not.</returns>
    public bool ContainsConcept(string code)
    {
        return _concepts.ContainsKey(code);
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
