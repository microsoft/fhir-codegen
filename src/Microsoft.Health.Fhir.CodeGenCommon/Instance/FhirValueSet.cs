// <copyright file="FhirValueSet.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Structure;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

public record class FhirValueSet : FhirCanonicalBase, ICloneable
{
    /// <summary>Values that represent filter operator codes.</summary>
    public enum FilterOperatorCodes
    {
        [FhirLiteral("=")]
        Equals,

        [FhirLiteral("is-a")]
        IsA,

        [FhirLiteral("descendent-of")]
        DescendentOf,

        [FhirLiteral("is-not-a")]
        IsNotA,

        [FhirLiteral("regex")]
        Regex,

        [FhirLiteral("in")]
        In,

        [FhirLiteral("not-in")]
        NotIn,

        [FhirLiteral("generalizes")]
        Generalizes,

        [FhirLiteral("child-of")]
        ChildOf,

        [FhirLiteral("descendent-leaf")]
        DescendentLeaf,

        [FhirLiteral("exists")]
        Exists
    }

    /// <summary>A composition filter filter.</summary>
    public record class ComposeFilter : ICloneable
    {
        private string _opLiteral = string.Empty;
        private FilterOperatorCodes _op = FilterOperatorCodes.Exists;

        /// <summary>Initializes a new instance of the ComposeFilter class.</summary>
        public ComposeFilter() { }

        /// <summary>Initializes a new instance of the ComposeFilter class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected ComposeFilter(ComposeFilter other)
        {
            Property = other.Property;
            OpLiteral = other.OpLiteral;
            Value = other.Value;
        }

        /// <summary>Gets or initializes the property name.</summary>
        public required string Property { get; init; }

        /// <summary>Gets the operation.</summary>
        public FilterOperatorCodes Op { get => _op; }

        /// <summary>Gets or initializes the operation literal.</summary>
        public required string OpLiteral
        {
            get => _opLiteral;
            init
            {
                _opLiteral = value;
                _op = value.ToEnum<FilterOperatorCodes>() ?? FilterOperatorCodes.Exists;
            }
        }

        /// <summary>Gets or initializes the value.</summary>
        public required string Value { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>ValueSet composition information.</summary>
    public record class Compose : ICloneable
    {
        /// <summary>Initializes a new instance of the Compose class.</summary>
        public Compose() { }

        /// <summary>Initializes a new instance of the Compose class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        public Compose(Compose other)
        {
            LockedDate = other.LockedDate;
            IsInactive = other.IsInactive;
            Includes = other.Includes.Select(v => v with { });
            Excludes = other.Excludes.Select(v => v with { });
            Properties = other.Properties.Select(v => v);
        }

        /// <summary>Gets or initializes the locked date.</summary>
        public string LockedDate { get; init; } = string.Empty;

        /// <summary>Gets or initializes the is inactive.</summary>
        public bool? IsInactive { get; init; } = null;

        /// <summary>Gets or initializes the includes.</summary>
        public required IEnumerable<ComposeInclude> Includes { get; init; }

        /// <summary>Gets or initializes the excludes.</summary>
        public IEnumerable<ComposeExclude> Excludes { get; init; } = Enumerable.Empty<ComposeExclude>();

        /// <summary>Gets or initializes the properties.</summary>
        public IEnumerable<string> Properties { get; init; } = Enumerable.Empty<string>();

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A compose include.</summary>
    public record class ComposeInclude : ComposeNode { }

    /// <summary>A compose exclude.</summary>
    public record class ComposeExclude : ComposeNode { }

    /// <summary>A compose node.</summary>
    public abstract record class ComposeNode : ICloneable
    {
        /// <summary>Initializes a new instance of the ComposeNode class.</summary>
        public ComposeNode() { }

        /// <summary>Initializes a new instance of the ComposeNode class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected ComposeNode(ComposeNode other)
        {
            System = other.System;
            Version = other.Version;
            Concepts = other.Concepts.Select(v => v with { });
            Filters = other.Filters.Select(v => v with { });
            IncludedValueSets = other.IncludedValueSets.Select(v => v);
            Copyright = other.Copyright;
        }

        /// <summary>Gets or initializes the system.</summary>
        public string System { get; init; } = string.Empty;

        /// <summary>Gets or initializes the version.</summary>
        public string Version { get; init; } = string.Empty;

        /// <summary>Gets or initializes the concepts.</summary>
        public IEnumerable<FhirCodeSystem.Concept> Concepts { get; init; } = Enumerable.Empty<FhirCodeSystem.Concept>();

        /// <summary>Gets or initializes the filters.</summary>
        public IEnumerable<ComposeFilter> Filters { get; init; } = Enumerable.Empty<ComposeFilter>();

        /// <summary>Gets or initializes included value sets.</summary>
        public IEnumerable<string> IncludedValueSets { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets or initializes the copyright.</summary>
        public string Copyright { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>An expansion parameter.</summary>
    public record class ExpansionParameter : ICloneable
    {
        /// <summary>Gets or initializes the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets or initializes the value.</summary>
        public dynamic? Value { get; init; } = null;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>An expansion property.</summary>
    public record class ExpansionProperty : ICloneable
    {
        /// <summary>Gets or initializes the code.</summary>
        public required string Code { get; init; }

        /// <summary>Gets or initializes URL of the document.</summary>
        public string Url { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    public record class ValueSetReference
    {
        private Dictionary<string, FhirElement> _referencingElementsByPath = new();
        private Dictionary<string, FhirElement> _referencingExternalElementsByUrl = new();
        private FhirElement.ElementDefinitionBindingStrength _strongestBinding = FhirElement.ElementDefinitionBindingStrength.Example;

        public ValueSetReference() { }

        [SetsRequiredMembers]
        protected ValueSetReference(ValueSetReference other)
        {

        }

        /// <summary>Gets the elements that reference this value set, by FHIR element path.</summary>
        public Dictionary<string, FhirElement> ReferencingElementsByPath { get => _referencingElementsByPath; }

        /// <summary>
        /// Gets the full pathname of the external referencing elements (e.g., extensions, profiles) by
        /// definitional URL.
        /// </summary>
        public Dictionary<string, FhirElement> ReferencingExternalElementsByUrl { get => _referencingExternalElementsByUrl; }

        /// <summary>Gets the strongest binding.</summary>
        public FhirElement.ElementDefinitionBindingStrength StrongestBinding { get => _strongestBinding; }

        public void AddReferencingElement(FhirElement element)
        {
            if ((element == null) ||
                string.IsNullOrEmpty(element.Path) ||
                (element.ValueSetBindingStrength == null) ||
                _referencingElementsByPath.ContainsKey(element.Path))
            {
                return;
            }

            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if ((element.ValueSetBindingStrength != null) &&
                (element.ValueSetBindingStrength > _strongestBinding))
            {
                _strongestBinding = (FhirElement.ElementDefinitionBindingStrength)element.ValueSetBindingStrength;
            }

            if (!string.IsNullOrEmpty(element.Path))
            {
                if (!_referencingElementsByPath.ContainsKey(element.Path))
                {
                    _referencingElementsByPath.Add(element.Path, element);
                }
            }
            else if (!string.IsNullOrEmpty(element.Url))
            {
                if (!_referencingExternalElementsByUrl.ContainsKey(element.Url))
                {
                    _referencingExternalElementsByUrl.Add(element.Url, element);
                }
            }
            else if (!string.IsNullOrEmpty(element.Name))
            {
                if (!_referencingExternalElementsByUrl.ContainsKey(element.Name))
                {
                    _referencingExternalElementsByUrl.Add(element.Name, element);
                }
            }
        }
    }

    /// <summary>A ValueSet expansion.</summary>
    public record class ValueSetExpansion : ICloneable
    {
        private Dictionary<string, FhirCodeSystem.Concept> _concepts = new();
        private bool _isLimitedExpansion = false;
        private IEnumerable<ExpansionParameter> _parameters = Enumerable.Empty<ExpansionParameter>();

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Identifier { get; init; } = string.Empty;

        // TODO: add support for fetching additional pages of expansions
        /// <summary>Gets or initializes URL of the next page.</summary>
        public string NextPageUrl { get; init; } = string.Empty;

        /// <summary>Gets the Date/Time of the timestamp.</summary>
        /// <value>The timestamp.</value>
        public required string Timestamp { get; init; }

        /// <summary>Gets the number of. </summary>
        /// <value>The total.</value>
        public int? Total { get; init; } = null;

        /// <summary>Gets the offset.</summary>
        /// <value>The offset.</value>
        public int? Offset { get; init; } = null;

        /// <summary>Gets a value indicating whether this object is limited expansion.</summary>
        public bool IsLimitedExpansion { get => _isLimitedExpansion; }

        /// <summary>Gets options for controlling the operation.</summary>
        /// <value>The parameters.</value>
        public IEnumerable<ExpansionParameter> Parameters
        {
            get => _parameters;
            init
            {
                _parameters = value;

                IEnumerable<ExpansionParameter> limitedParams = _parameters.Where(p => p.Name == "limitedExpansion");

                if (limitedParams.Any())
                {
                    switch (limitedParams.First().Value)
                    {
                        case -1:
                        case "-1":
                        case true:
                        case "true":
                            _isLimitedExpansion = true;
                            break;
                    }
                }
            }
        }

        /// <summary>Gets or initializes the properties.</summary>
        public IEnumerable<ExpansionProperty> Properties { get; init; } = Enumerable.Empty<ExpansionProperty>();

        /// <summary>Gets or initializes the contains.</summary>
        public IEnumerable<FhirCodeSystem.Concept> Concepts
        {
            get => _concepts.Values;
            init
            {
                _concepts = value.ToDictionary(v => v.Code, v => v);
            }
        }

        /// <summary>Gets the contains by code.</summary>
        public Dictionary<string, FhirCodeSystem.Concept> ConceptsByCode { get => _concepts; }

        /// <summary>Indexer to get concepts based on code.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
        ///  required range.</exception>
        /// <param name="code">The code.</param>
        /// <returns>The concept item.</returns>
        public FhirCodeSystem.Concept this[string code]
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

        /// <summary>Query if this object contains the given code.</summary>
        /// <param name="code">The string to test for containment.</param>
        /// <returns>True if the object is in this collection, false if not.</returns>
        public bool Contains(string code) => _concepts.ContainsKey(code);

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    private HashSet<string> _referencedCodeSystems = new();

    /// <summary>Initializes a new instance of the FhirValueSet class.</summary>
    public FhirValueSet() : base() { }

    /// <summary>Initializes a new instance of the FhirValueSet class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirValueSet(FhirValueSet other)
        : base(other)
    {
        Composition = other.Composition == null ? null : other.Composition with { };
        Expansion = other.Expansion == null ? null : other.Expansion with { };
        Concepts = other.Concepts.Select(c => c with { });
        ReferencedCodeSystems = other.ReferencedCodeSystems.Select(r => r);
        References = other.References with { };
    }

    /// <summary>Gets or initializes the composition.</summary>
    public Compose? Composition { get; init; } = null;

    /// <summary>Gets or initializes the expansion.</summary>
    public ValueSetExpansion? Expansion { get; init; } = null;

    /// <summary>Gets or initializes the concepts.</summary>
    public IEnumerable<FhirCodeSystem.Concept> Concepts { get; init; } = Enumerable.Empty<FhirCodeSystem.Concept>();

    /// <summary>Gets or initializes the referenced code systems.</summary>
    public IEnumerable<string> ReferencedCodeSystems
    {
        get => _referencedCodeSystems.AsEnumerable();
        init
        {
            _referencedCodeSystems = value.ToHashSet();
        }
    }

    /// <summary>Gets the set the referenced code system belongs to.</summary>
    public HashSet<string> ReferencedCodeSystemSet { get => _referencedCodeSystems; }

    public ValueSetReference References { get; init; } = new();

    /// <summary>Gets the key.</summary>
    public string Key => $"{Url}|{Version}";

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
