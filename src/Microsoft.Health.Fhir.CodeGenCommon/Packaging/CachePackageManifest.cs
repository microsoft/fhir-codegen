// <copyright file="CachePackageManifest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.CodeGenCommon.Packaging;

/// <summary>A package manifest (package.json) loaded from a cached package.</summary>
/// <remarks>
/// See
/// https://confluence.hl7.org/pages/viewpage.action?pageId=35718629#NPMPackageSpecification-Packagemanifest
/// for details.
/// </remarks>
public record class CachePackageManifest
{
    /// <summary>A package mantainer.</summary>
    public record class PackageMantainer
    {
        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>Gets or sets the email.</summary>
        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        /// <summary>Gets or sets URL of the document.</summary>
        [JsonPropertyName("url")]
        public string Url { get; init; } = string.Empty;
    }

    /// <summary>Initializes a new instance of the CachePackageManifest class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected CachePackageManifest(CachePackageManifest other)
    {
        // copy all properties
        Name = other.Name;
        Version = other.Version;
        CanonicalUrl = other.CanonicalUrl;
        PublicationUrl = other.PublicationUrl;
        Homepage = other.Homepage;
        Title = other.Title;
        Description = other.Description;
        FhirVersions = other.FhirVersions.Select(v => v);
        Type = other.Type;
        Dependencies = other.Dependencies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Keywords = other.Keywords.Select(k => k);
        Author = other.Author;
        Mantainers = other.Mantainers.Select(m => m with { });
        License = other.License;
        Jurisdiction = other.Jurisdiction;
        Directories = other.Directories.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Date = other.Date;
        ToolsVersion = other.ToolsVersion;
    }

    /// <summary>Gets the name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>Gets the name.</summary>
    [JsonPropertyName("version")]
    public required string Version { get; init; }

    /// <summary>Gets the root canonical URL for this package.</summary>
    [JsonPropertyName("canonical")]
    public string CanonicalUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets where the human readable representation (e.g. IG) that represents this version of this
    /// package is published on the web (if there is such a thing).
    /// </summary>
    [JsonPropertyName("url")]
    public string PublicationUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the project homepage (e.g. where information about the project that produced the package
    /// can be found on the web).
    /// </summary>
    [JsonPropertyName("homepage")]
    public string Homepage { get; init; } = string.Empty;

    /// <summary>Gets the title.</summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    /// <summary>Gets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets the FHIR versions.</summary>
    /// <remarks>
    /// This is only used in the case where there is no dependency on a core package. Note: this
    /// usually happens when the package is based on a transient unpublished version of FHIR, which
    /// was allowed in the past but will no longer be allowed. So this should only be seen in old
    /// balloted packages.
    /// </remarks>
    [JsonPropertyName("fhirVersions")]
    public IEnumerable<string> FhirVersions { get; init; } = Enumerable.Empty<string>();

    /// <summary>Gets the hint that provides guidance for the use of the package.</summary>
    /// <remarks>
    /// Known types are (case-insensitive):
    /// - Conformance: a set of Conformance Resources in the base package folder (/package)
    /// - IG: a FHIR implementation guide package(has an ImplementationGuide resource in /package,
    ///       along with conformance resources, and also contains example resources in
    ///       /package/example)
    /// - Core: contains the conformance related resources for the main FHIR
    ///         specification(effectively, this is a special type of "conformance" that marks it as
    ///         the core specification, which could also be inferred from it's name such as
    ///         hl7.fhir.r4.core, but other branches / ballots etc may vary, so this is simpler than
    ///         inferring from the name)
    /// - fhir.core: older format of 'core' package (deprecated)
    /// - Examples : contains the example resources found in the main FHIR specification in /package.
    /// - Group: a package that only includes (e.g.depends on) other packages (won't contain FHIR
    ///          resources directly). The versions listed in this package must include all the
    ///          versions found in the included packages. Note that this is is used for the set of
    ///          packages that represent a full core specification
    /// - Tool: A package that contains tool specific files to support specific tools (won't contain
    ///         FHIR resources or specify a FHIR version)
    /// - IG-Template: an IG template for use by IG publishing tools (won't contain FHIR resources or
    ///                specify a version)
    /// </remarks>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>Gets or sets the package dependencies - keys are package names, values are package versions.</summary>
    [JsonPropertyName("dependencies")]
    public Dictionary<string, string> Dependencies { get; init; } = new Dictionary<string, string>();

    /// <summary>Gets the keywords.</summary>
    [JsonPropertyName("keywords")]
    public IEnumerable<string> Keywords { get; init; } = Enumerable.Empty<string>();

    /// <summary>Gets or sets the author.</summary>
    [JsonPropertyName("author")]
    public string Author { get; init; } = string.Empty;

    /// <summary>Gets or sets the mantainers.</summary>
    [JsonPropertyName("maintainers")]
    public IEnumerable<PackageMantainer> Mantainers { get; init; } = Enumerable.Empty<PackageMantainer>();

    /// <summary>Gets or sets the SPDX-convention license name.</summary>
    [JsonPropertyName("license")]
    public string License { get; init; } = string.Empty;

    /// <summary>Gets the jurisdiction.</summary>
    /// <remarks>
    /// From the CommonJuridictionCodes (http://hl7.org/fhir/ValueSet/jurisdiction-common) from the
    /// fhir.tx.support.* package (fhir.tx.support.r3, fhir.tx.support.r4, ...)
    /// </remarks>
    [JsonPropertyName("jurisdiction")]
    public string Jurisdiction { get; init; } = string.Empty;

    /// <summary>
    /// Gets package directories - values are unprefixed relative paths from the package directory
    /// directory.
    /// </summary>
    /// <remarks>
    /// Known directory keys are:
    ///  - lib: contains the package's definitions
    ///  - bin: contains any executables the package provides
    ///  - man: contains any man pages the package provides
    ///  - doc: contains documentation for the package
    ///  - example: contains example resource instances for the package
    ///  - test: contains test resources for the package.
    /// Note that this list does NOT include:
    /// - xml: XML schema definitions for the package (.sch)
    /// - openapi: OpenAPI definitions for the package (.json)
    /// - rdf: RDF definitions for the package (.ttl)
    /// TODO: not documented at
    ///  https://confluence.hl7.org/pages/viewpage.action?pageId=35718629#NPMPackageSpecification-Packagemanifest.
    /// </remarks>
    [JsonPropertyName("directories")]
    public Dictionary<string, string> Directories { get; init; } = new Dictionary<string, string>();

    /// <summary>Gets or sets the date.</summary>
    /// <remarks>TODO: not documented at https://confluence.hl7.org/pages/viewpage.action?pageId=35718629#NPMPackageSpecification-Packagemanifest </remarks>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Gets or sets the tools version.</summary>
    /// <remarks>TODO: not documented at https://confluence.hl7.org/pages/viewpage.action?pageId=35718629#NPMPackageSpecification-Packagemanifest </remarks>
    [JsonPropertyName("tools-version")]
    public int? ToolsVersion { get; init; } = null;


}
