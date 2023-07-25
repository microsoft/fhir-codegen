// <copyright file="FhirSearchParam.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Resource;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR search parameter.</summary>
public record class FhirSearchParam : FhirModelBase, IWithExpectations, ICloneable
{
    private bool? _compositeResolvesCorrectly = null;
    private string _fhirSearchType = string.Empty;
    private readonly SearchParameterTypeCodes _searchType;

    /// <summary>Values that represent search parameter type codes.</summary>
    public enum SearchParameterTypeCodes
    {
        /// <summary>Search parameter SHALL be a number (a whole number, or a decimal).</summary>
        [FhirLiteral("number")]
        Number,

        /// <summary>Search parameter is on a date/time. The date format is the standard XML format, though other formats may be supported.</summary>
        [FhirLiteral("date")]
        Date,

        /// <summary>Search parameter is a simple string, like a name part. Search is case-insensitive and accent-insensitive. May match just the start of a string. String parameters may contain spaces.</summary>
        [FhirLiteral("string")]
#pragma warning disable CA1720 // Identifier contains type name
        String,
#pragma warning restore CA1720 // Identifier contains type name

        /// <summary>Search parameter on a coded element or identifier. May be used to search through the text, display, code and code/codesystem (for codes) and label, system and key (for identifier). Its value is either a string or a pair of namespace and value, separated by a "|", depending on the modifier used.</summary>
        [FhirLiteral("token")]
        Token,

        /// <summary>A reference to another resource (Reference or canonical).</summary>
        [FhirLiteral("reference")]
        Reference,

        /// <summary>A composite search parameter that combines a search on two values together.</summary>
        [FhirLiteral("composite")]
        Composite,

        /// <summary>A search parameter that searches on a quantity.</summary>
        [FhirLiteral("quantity")]
        Quantity,

        /// <summary>A search parameter that searches on a URI (RFC 3986).</summary>
        [FhirLiteral("uri")]
        Uri,

        /// <summary>Special logic applies to this parameter per the description of the search parameter.</summary>
        [FhirLiteral("special")]
        Special,
    }

    /// <summary>A composite component.</summary>
    public record class CompositeComponent : ICloneable
    {
        /// <summary>Initializes a new instance of the CompositeComponent class.</summary>
        public CompositeComponent() { }

        /// <summary>Initializes a new instance of the CompositeComponent class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected CompositeComponent(CompositeComponent other)
        {
            Expression = other.Expression;
            Definition = other.Definition;
            DefinitionParam = other.DefinitionParam == null ? null : other.DefinitionParam with { };
        }

        /// <summary>Gets the expression.</summary>
        public required string Expression { get; init; }

        /// <summary>Gets the definition.</summary>
        public required string Definition { get; init; }

        /// <summary>Gets or sets the definition parameter.</summary>
        public FhirSearchParam? DefinitionParam { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>Initializes a new instance of the FhirSearchParam class.</summary>
    public FhirSearchParam() { }

    /// <summary>Initializes a new instance of the FhirSearchParam class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirSearchParam(FhirSearchParam other)
        : base(other)
    {
        Code = other.Code;
        ResourceTypes = other.ResourceTypes.Select(v => v);
        Targets = other.Targets.Select(v => v);
        FhirSearchType = other.FhirSearchType;
        XPath = other.XPath;
        XPathUsage = other.XPathUsage;
        Expression = other.Expression;
        Components = other.Components.Select(v => v with { });
        _compositeResolvesCorrectly = other._compositeResolvesCorrectly;
    }

    /// <summary>Gets the code.</summary>
    public required string Code { get; init; }

    /// <summary>Gets the type of the resource.</summary>
    public required IEnumerable<string> ResourceTypes { get; init; }

    /// <summary>Gets the resource (e.g., reference) targets.</summary>
    public IEnumerable<string> Targets { get; init; } = Enumerable.Empty<string>();

    /// <summary>Gets the type of the search parameter input.</summary>
    public SearchParameterTypeCodes SearchType => _searchType;

    /// <summary>Gets the FHIR Literal search parameter type.</summary>
    public required string FhirSearchType
    {
        get => _fhirSearchType;
        init
        {
            _fhirSearchType = value;
            if (_fhirSearchType.TryFhirEnum(out SearchParameterTypeCodes v))
            {
                _searchType = v;
            }
        }
    }

    /// <summary>Gets the XPath specification for this search parameter.</summary>
    public string XPath { get; init; } = string.Empty;

    /// <summary>Gets the XPath usage information.</summary>
    public string XPathUsage { get; init; } = string.Empty;

    /// <summary>Gets the FHIRPath expression that extracts values for this parameter.</summary>
    public string Expression { get; init; } = string.Empty;

    /// <summary>Gets the components.</summary>
    public IEnumerable<CompositeComponent> Components { get; init; } = Enumerable.Empty<CompositeComponent>();

    /// <summary>Gets wheter all composite definitions resolve correctly.</summary>
    public bool? CompositeResolvesCorrectly { get => _compositeResolvesCorrectly; init => _compositeResolvesCorrectly = value; }

    /// <summary>Gets the obligations by actor.</summary>
    public Dictionary<string, IEnumerable<FhirObligation>> ObligationsByActor { get; init; } = new();

    /// <summary>Convert this object into a string representation.</summary>
    /// <returns>A string that represents this object.</returns>
    public override string ToString()
    {
        if (!ObligationsByActor.Any())
        {
            return Name;
        }

        return Name + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
