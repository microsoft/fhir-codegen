// <copyright file="FhirImplementationGuide.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.BaseModels;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR implementation guide.</summary>
public record class FhirImplementationGuide : FhirCanonicalBase, ICloneable
{
    /// <summary>Values that represent page generation codes.</summary>
    public enum PageGenerationCodes
    {
        [FhirLiteral("html")]
        Html,

        [FhirLiteral("markdown")]
        Markdown,

        [FhirLiteral("xml")]
        Xml,

        [FhirLiteral("generated")]
        Generated,
    }

    /// <summary>A definition grouping.</summary>
    public record class DefinitionGrouping : FhirBase, ICloneable
    {
        /// <summary>Gets or initializes the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets or initializes the description.</summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A definition page.</summary>
    public record class DefinitionPage : FhirBase, ICloneable
    {
        private PageGenerationCodes _pageGeneration = PageGenerationCodes.Generated;
        private string _pageGenerationLiteral = string.Empty;

        /// <summary>Initializes a new instance of the DefinitionPage class.</summary>
        public DefinitionPage() : base() { }

        /// <summary>Initializes a new instance of the DefinitionPage class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected DefinitionPage(DefinitionPage other)
            : base(other)
        {
            SourceUrl = other.SourceUrl;
            SourceMarkdown = other.SourceMarkdown;
            Name = other.Name;
            Title = other.Title;
            GenerationLiteral = other.GenerationLiteral;
            Pages = other.Pages.Select(v => v with { });
        }

        /// <summary>Gets or initializes URL of the source.</summary>
        public string SourceUrl { get; init; } = string.Empty;

        /// <summary>Gets or initializes source markdown.</summary>
        public string SourceMarkdown { get; init; } = string.Empty;

        /// <summary>Gets or initializes the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets or initializes the title.</summary>
        public required string Title { get; init; }

        /// <summary>Gets the generation.</summary>
        public PageGenerationCodes Generation { get => _pageGeneration; }

        /// <summary>Gets or initializes the generation literal.</summary>
        public required string GenerationLiteral
        {
            get => _pageGenerationLiteral;
            init
            {
                _pageGenerationLiteral = value;
                _pageGeneration = value.ToEnum<PageGenerationCodes>() ?? PageGenerationCodes.Generated;
            }
        }

        /// <summary>Gets or initializes the pages.</summary>
        public IEnumerable<DefinitionPage> Pages { get; init; } = Enumerable.Empty<DefinitionPage>();

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A definition parameter.</summary>
    public record class DefinitionParameter : FhirBase, ICloneable
    {
        /// <summary>Gets or initializes the code.</summary>
        public required string Code { get; init; }

        /// <summary>Gets or initializes the value.</summary>
        public required string Value { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A definition resource.</summary>
    public record class DefinitionResource : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the ArtifactResource class.</summary>
        public DefinitionResource() : base() { }

        /// <summary>Initializes a new instance of the ArtifactResource class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected DefinitionResource(DefinitionResource other)
            : base(other)
        {
            ResourceReference = other.ResourceReference with { };
            FhirVersions = other.FhirVersions.Select(v => v);
            Name = other.Name;
            Description = other.Description;
            IsExample = other.IsExample;
            Profiles = other.Profiles.Select(v => v);
            GroupingId = other.GroupingId;
        }

        /// <summary>Gets or initializes the resource reference.</summary>
        public required FhirReference ResourceReference { get; init; }

        /// <summary>Gets or initializes the FHIR versions.</summary>
        public IEnumerable<string> FhirVersions { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets or initializes the name.</summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>Gets or initializes the description.</summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>Gets or initializes the is example.</summary>
        public bool? IsExample { get; init; } = null;

        /// <summary>Gets or initializes the profiles.</summary>
        public IEnumerable<string> Profiles { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets or initializes the identifier of the grouping.</summary>
        public string GroupingId { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A definition template.</summary>
    public record class DefinitionTemplate : FhirBase, ICloneable
    {
        /// <summary>Gets or initializes the code.</summary>
        public required string Code { get; init; }

        /// <summary>Gets or initializes the source for the.</summary>
        public required string Source { get; init; }

        /// <summary>Gets or initializes the scope.</summary>
        public string Scope { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A dependency.</summary>
    public record class Dependency : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the Dependency class.</summary>
        public Dependency() : base() { }

        /// <summary>Initializes a new instance of the Dependency class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected Dependency(Dependency other)
            : base(other)
        {
            Canonical = other.Canonical;
            PackageId = other.PackageId;
            Version = other.Version;
            Reason = other.Reason;
        }

        /// <summary>Gets or initializes the canonical URL of the dependency.</summary>
        public required string Canonical { get; init; }

        /// <summary>Gets or initializes the identifier of the package.</summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>Gets or initializes the version.</summary>
        public string Version { get; init; } = string.Empty;

        /// <summary>Gets or initializes the reason.</summary>
        public string Reason { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A global profile.</summary>
    public record class GlobalProfile : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the GlobalProfile class.</summary>
        public GlobalProfile() : base() { }

        /// <summary>Initializes a new instance of the GlobalProfile class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected GlobalProfile(GlobalProfile other)
            : base(other)
        {
            ResourceType = other.ResourceType;
            Canonical = other.Canonical;
        }

        /// <summary>Gets or initializes the type of the resource.</summary>
        public required string ResourceType { get; init; }

        /// <summary>Gets or initializes the canonical.</summary>
        public required string Canonical { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A guide definition.</summary>
    public record class GuideDefinition : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the GuideDefinition class.</summary>
        public GuideDefinition() : base() { }

        /// <summary>Initializes a new instance of the GuideDefinition class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected GuideDefinition(GuideDefinition other)
            : base(other)
        {
            Groupings = other.Groupings.Select(v => v);
            Resources = other.Resources.Select(v => v);
            Page = (other.Page == null) ? null : other.Page with { };
            Parameters = other.Parameters.Select(v => v);
            Templates = other.Templates.Select(v => v);
        }

        /// <summary>Gets or initializes the groupings.</summary>
        public IEnumerable<DefinitionGrouping> Groupings { get; init; } = Enumerable.Empty<DefinitionGrouping>();

        /// <summary>Gets or initializes the resources.</summary>
        public IEnumerable<DefinitionResource> Resources { get; init; } = Enumerable.Empty<DefinitionResource>();

        /// <summary>Gets or initializes the page.</summary>
        public DefinitionPage? Page { get; init; } = null;

        /// <summary>Gets or initializes options for controlling the operation.</summary>
        public IEnumerable<DefinitionParameter> Parameters { get; init; } = Enumerable.Empty<DefinitionParameter>();

        /// <summary>Gets or initializes the templates.</summary>
        public IEnumerable<DefinitionTemplate> Templates { get; init; } = Enumerable.Empty<DefinitionTemplate>();

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A list of the guide.</summary>
    public record class GuideManifest : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the GuideManifest class.</summary>
        public GuideManifest() : base() { }

        /// <summary>Initializes a new instance of the GuideManifest class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected GuideManifest(GuideManifest other)
            : base(other)
        {
            Rendering = other.Rendering;
            Resource = other.Resource.Select(v => v with { });
            Page = other.Page.Select(v => v with { });
            Images = other.Images.Select(v => v);
            OtherFiles = other.OtherFiles.Select(v => v);
        }

        /// <summary>Gets or initializes the rendering.</summary>
        public string Rendering { get; init; } = string.Empty;

        /// <summary>Gets or initializes the resource.</summary>
        public required IEnumerable<ManifestResource> Resource { get; init; } = Enumerable.Empty<ManifestResource>();

        /// <summary>Gets or initializes the page.</summary>
        public IEnumerable<ManifestPage> Page { get; init; } = Enumerable.Empty<ManifestPage>();

        /// <summary>Gets or initializes the images.</summary>
        public IEnumerable<string> Images { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets or initializes the other files.</summary>
        public IEnumerable<string> OtherFiles { get; init; } = Enumerable.Empty<string>();
    }

    /// <summary>A manifest page.</summary>
    public record class ManifestPage : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the ManifestPage class.</summary>
        public ManifestPage() : base() { }

        /// <summary>Initializes a new instance of the ManifestPage class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected ManifestPage(ManifestPage other)
            : base(other)
        {
            Name = other.Name;
            Title = other.Title;
            Anchors = other.Anchors.Select(v => v);
        }

        /// <summary>Gets or initializes the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets or initializes the title.</summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>Gets or initializes the anchors.</summary>
        public IEnumerable<string> Anchors { get; init; } = Enumerable.Empty<string>();

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A manifest resource.</summary>
    public record class ManifestResource : FhirBase, ICloneable
    {
        /// <summary>Initializes a new instance of the ManifestResource class.</summary>
        public ManifestResource() : base() { }

        /// <summary>Initializes a new instance of the ManifestResource class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected ManifestResource(ManifestResource other)
            : base(other)
        {
            Reference = other.Reference with { };
            IsExample = other.IsExample;
            Profiles = other.Profiles.Select(v => v);
            RelativePath = other.RelativePath;
        }

        /// <summary>Gets or initializes the reference.</summary>
        public required FhirReference Reference { get; init; }

        /// <summary>Gets or initializes the is example.</summary>
        public bool? IsExample { get; init; } = null;

        /// <summary>Gets or initializes the profiles.</summary>
        public IEnumerable<string> Profiles { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets or initializes the full pathname of the relative file.</summary>
        public string RelativePath { get; init; } = string.Empty;
    }

    /// <summary>Initializes a new instance of the FhirImplementationGuide class.</summary>
    public FhirImplementationGuide() : base() { }

    /// <summary>Initializes a new instance of the FhirImplementationGuide class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    public FhirImplementationGuide(FhirImplementationGuide other)
        : base(other)
    {
        FhirVersion = other.FhirVersion.Select(v => v);
        Dependencies = other.Dependencies.Select(v => v with { });
        GlobalProfiles = other.GlobalProfiles.Select(v => v with { });
        Definition = (other.Definition == null) ? null : other.Definition with { };
    }

    /// <summary>Gets or initializes the FHIR version.</summary>
    public required IEnumerable<string> FhirVersion { get; init; }

    /// <summary>Gets or initializes the dependencies.</summary>
    public IEnumerable<Dependency> Dependencies { get; init; } = Enumerable.Empty<Dependency>();

    /// <summary>Gets or initializes the global profiles.</summary>
    public IEnumerable<GlobalProfile> GlobalProfiles { get; init; } = Enumerable.Empty<GlobalProfile>();

    /// <summary>Gets or initializes the definition.</summary>
    public GuideDefinition? Definition { get; init; } = null;
    
    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
