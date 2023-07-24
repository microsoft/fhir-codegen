// <copyright file="FhirSearchParam.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR search parameter.</summary>
public record class FhirSearchParam : FhirModelBase
{
    private bool? _compositeResolvesCorrectly = null;

    /// <summary>A composite component.</summary>
    public record class CompositeComponent
    {
        /// <summary>Gets the expression.</summary>
        public required string Expression { get; init; }

        /// <summary>Gets the definition.</summary>
        public required string Definition { get; init; }

        /// <summary>Gets or sets the definition parameter.</summary>
        public FhirSearchParam? DefinitionParam { get; init; }
    }

    /// <summary>Initializes a new instance of the FhirSearchParam class.</summary>
    /// <param name="other">The other.</param>
    protected FhirSearchParam(FhirSearchParam other)
        : base(other)
    {
        Code = other.Code;
        ResourceTypes = other.ResourceTypes.Select(v => v);
        Targets = other.Targets.Select(v => v);
        SearchType = other.SearchType;
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

    /// <summary>Gets the search parameter type.</summary>
    public required string SearchType { get; init; }

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
}
