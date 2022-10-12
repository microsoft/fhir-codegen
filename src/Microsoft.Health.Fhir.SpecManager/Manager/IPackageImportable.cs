// <copyright file="IPackageImportable.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Interface for package importable.</summary>
public interface IPackageImportable
{
    /// <summary>Gets or sets the type of the package group.</summary>
    public FhirPackageCommon.FhirPackageTypeEnum PackageType { get; set; }

    /// <summary>Gets or sets the package details.</summary>
    public NpmPackageDetails PackageDetails { get; set; }

    /// <summary>Gets or sets the FHIR major release, by enum.</summary>
    public FhirPackageCommon.FhirSequenceEnum FhirSequence { get; set; }

    /// <summary>Gets or sets the major version.</summary>
    [Obsolete("R4B made major versions as integers tricky, use the FhirMajorVersion", false)]
    public int MajorVersion { get; set; }

    /// <summary>Gets or sets the name of the package.</summary>
    public string PackageName { get; set; }

    /// <summary>Gets or sets the version string.</summary>
    public string VersionString { get; set; }

    /// <summary>Gets a dictionary with the known primitive types for this version of FHIR.</summary>
    public Dictionary<string, FhirPrimitive> PrimitiveTypes { get; }

    /// <summary>Gets a dictionary with the known complex types for this version of FHIR.</summary>
    public Dictionary<string, FhirComplex> ComplexTypes { get; }

    /// <summary>Gets a dictionary with the known resources for this version of FHIR.</summary>
    public Dictionary<string, FhirComplex> Resources { get; }

    /// <summary>Gets a dictionary with the known logical models.</summary>
    public Dictionary<string, FhirComplex> LogicalModels { get; }

    /// <summary>Gets the profiles by id dictionary.</summary>
    public Dictionary<string, FhirComplex> Profiles { get; }

    /// <summary>Gets a profiles by base type dictionary.</summary>
    public Dictionary<string, Dictionary<string, FhirComplex>> ProfilesByBaseType { get; }

    /// <summary>Gets an extensions by URL dictionary.</summary>
    public Dictionary<string, FhirComplex> ExtensionsByUrl { get; }

    /// <summary>Gets the code systems by URL dictionary.</summary>
    public Dictionary<string, FhirCodeSystem> CodeSystems { get; }

    /// <summary>Gets the value sets by URL dictionary.</summary>
    public Dictionary<string, FhirValueSetCollection> ValueSetsByUrl { get; }

    /// <summary>Gets the extensions per path, in a dictionary keyed by URL.</summary>
    public Dictionary<string, Dictionary<string, FhirComplex>> ExtensionsByPath { get; }

    /// <summary>Gets the system operations.</summary>
    public Dictionary<string, FhirOperation> SystemOperations { get; }

    /// <summary>Gets search parameters defined for all resources.</summary>
    public Dictionary<string, FhirSearchParam> AllResourceParameters { get; }

    /// <summary>Gets search parameters that control search results.</summary>
    public Dictionary<string, FhirSearchParam> SearchResultParameters { get; }

    /// <summary>Gets search parameters defined for all interactions.</summary>
    public Dictionary<string, FhirSearchParam> AllInteractionParameters { get; }

    /// <summary>Gets or sets the name of the package release.</summary>
    public string ReleaseName { get; set; }

    /// <summary>Gets or sets the name of the examples package.</summary>
    public string ExamplesPackageName { get; set; }

    /// <summary>Gets or sets the name of the expansions package.</summary>
    public string ExpansionsPackageName { get; set; }

    /// <summary>Gets or sets the ballot prefix (e.g., 2021Jan).</summary>
    public string BallotPrefix { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is development build.</summary>
    public bool IsDevBuild { get; set; }

    /// <summary>Gets or sets the development branch.</summary>
    public string DevBranch { get; set; }

    /// <summary>Gets or sets the identifier of the build.</summary>
    public string BuildId { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is local build.</summary>
    public bool IsLocalBuild { get; set; }

    /// <summary>Gets or sets the pathname of the local directory.</summary>
    public string LocalDirectory { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is on disk.</summary>
    public bool IsOnDisk { get; set; }

    /// <summary>Gets or sets the Date/Time of the last downloaded.</summary>
    public DateTime? LastDownloaded { get; set; }

    /// <summary>Gets the node info by path dictionary.</summary>
    public Dictionary<string, FhirNodeInfo> NodeByPath { get; }

    /// <summary>Adds a code system.</summary>
    /// <param name="codeSystem">The code system.</param>
    public void AddCodeSystem(FhirCodeSystem codeSystem);

    /// <summary>Adds a complex type.</summary>
    /// <param name="complex">The complex.</param>
    public void AddComplexType(FhirComplex complex);

    /// <summary>Adds an extension.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="extension">The extension.</param>
    public void AddExtension(FhirComplex extension);

    /// <summary>Adds an operation.</summary>
    /// <param name="operation">The operation.</param>
    public void AddOperation(FhirOperation operation);

    /// <summary>Adds a primitive.</summary>
    /// <param name="primitive">The primitive.</param>
    public void AddPrimitive(FhirPrimitive primitive);

    /// <summary>Adds a profile.</summary>
    /// <param name="complex">The complex.</param>
    public void AddProfile(FhirComplex complex);

    /// <summary>Adds a resource.</summary>
    /// <param name="resource">The resource object.</param>
    public void AddResource(FhirComplex resource);

    /// <summary>Adds a logical model.</summary>
    /// <param name="logicalModel">The logical model.</param>
    public void AddLogicalModel(FhirComplex logicalModel);

    /// <summary>Adds a search parameter.</summary>
    /// <param name="searchParam">The search parameter.</param>
    public void AddSearchParameter(FhirSearchParam searchParam);

    /// <summary>Adds a value set.</summary>
    /// <param name="valueSet">Set the value belongs to.</param>
    public void AddValueSet(FhirValueSet valueSet);

    public void AddImplementationGuide(FhirImplementationGuide ig);

    /// <summary>Adds a versioned parameter.</summary>
    /// <param name="searchMagicType">Type of the search magic.</param>
    /// <param name="name">           The name.</param>
    /// <param name="parameterType">  Type of the parameter.</param>
    public void AddVersionedParam(FhirSearchParam.ParameterGrouping searchMagicType, string name, string parameterType);

    /// <summary>Determines if we can converter has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ConverterHasIssues(out int errorCount, out int warningCount);

    /// <summary>Displays the converter issues.</summary>
    public void DisplayConverterIssues();

    /// <summary>Query if 'key' is a known complex data type.</summary>
    /// <param name="key">The key.</param>
    /// <returns>True if 'key' exists, false if not.</returns>
    public bool HasComplex(string key);

    /// <summary>Query if 'urlOrKey' has value set.</summary>
    /// <param name="urlOrKey">The URL or key.</param>
    /// <returns>True if value set, false if not.</returns>
    public bool HasValueSet(string urlOrKey);

    /// <summary>Parses resource an object from the given string.</summary>
    /// <param name="json">The JSON.</param>
    /// <returns>A typed Resource object.</returns>
    public object ParseResource(string json);

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resource">[out] The resource object.</param>
    public void ProcessResource(object resource);

    /// <summary>Determine if we should ignore resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ShouldIgnoreResource(string resourceName);

    /// <summary>Determine if we should process resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ShouldProcessResource(string resourceName);

    /// <summary>Determine if we should skip file.</summary>
    /// <param name="filename">Filename of the file.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ShouldSkipFile(string filename);

    /// <summary>Attempts to get node information a FhirNodeInfo from the given string.</summary>
    /// <param name="path">Full pathname of the file.</param>
    /// <param name="node">[out] The node.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetNodeInfo(string path, out FhirNodeInfo node);

    /// <summary>Attempts to get explicit name a string from the given string.</summary>
    /// <param name="path">        Full pathname of the file.</param>
    /// <param name="explicitName">[out] Name of the explicit.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetExplicitName(string path, out string explicitName);

    /// <summary>Attempts to get value set a FhirValueSet from the given string.</summary>
    /// <param name="urlOrKey">The URL or key.</param>
    /// <param name="vs">      [out] The vs.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetValueSet(string urlOrKey, out FhirValueSet vs);
}
