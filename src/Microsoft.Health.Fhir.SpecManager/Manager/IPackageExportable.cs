// <copyright file="IPackageExportable.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Interface for package exportable.</summary>
public interface IPackageExportable
{
    /// <summary>Gets the type of the package group.</summary>
    public FhirPackageCommon.FhirPackageTypeEnum PackageType { get; }

    /// <summary>Gets the package details.</summary>
    public NpmPackageDetails PackageDetails { get; }

    /// <summary>Gets  the FHIR major release, by enum.</summary>
    public FhirPackageCommon.FhirSequenceEnum FhirSequence { get; }

    /// <summary>Gets the major version.</summary>
    [Obsolete("R4B made major versions as integers tricky, use the FhirMajorVersion", false)]
    public int MajorVersion { get; }

    /// <summary>Gets the name of the package.</summary>
    public string PackageName { get; }

    /// <summary>Gets the version string.</summary>
    public string VersionString { get; }

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

    /// <summary>Gets all known operations, by Url.</summary>
    public Dictionary<string, FhirOperation> OperationsByUrl { get; }

    /// <summary>Gets search parameters defined for all resources.</summary>
    public Dictionary<string, FhirSearchParam> AllResourceParameters { get; }

    /// <summary>Gets search parameters that control search results.</summary>
    public Dictionary<string, FhirSearchParam> SearchResultParameters { get; }

    /// <summary>Gets search parameters defined for all interactions.</summary>
    public Dictionary<string, FhirSearchParam> AllInteractionParameters { get; }

    /// <summary>Gets all search parameters by URL.</summary>
    public Dictionary<string, FhirSearchParam> SearchParametersByUrl { get; }

    /// <summary>Gets known implementation guides, keyed by URL.</summary>
    public Dictionary<string, FhirImplementationGuide> ImplementationGuidesByUrl { get; }

    /// <summary>Gets known capability statements, keyed by URL.</summary>
    public Dictionary<string, FhirCapabiltyStatement> CapabilitiesByUrl { get; }

    /// <summary>Gets the node info by path dictionary.</summary>
    public Dictionary<string, FhirNodeInfo> NodeByPath { get; }

    /// <summary>Gets the excluded keys.</summary>
    public HashSet<string> ExcludedKeys { get; }

    /// <summary>Gets inheritance names hash.</summary>
    /// <param name="key">The key.</param>
    /// <returns>The inheritance names hash.</returns>
    public HashSet<string> GetInheritanceNamesHash(string key);

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

    /// <summary>Attempts to get artifact.</summary>
    /// <param name="token">             The token.</param>
    /// <param name="artifact">          [out] The artifact.</param>
    /// <param name="artifactClass">     [out] The artifact class.</param>
    /// <param name="resolvedPackage">   [out] The resolved package.</param>
    /// <param name="resolveParentLinks">True to resolve parent links.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetArtifact(
        string token,
        out object artifact,
        out FhirArtifactClassEnum artifactClass,
        out string resolvedPackage,
        bool resolveParentLinks,
        FhirArtifactClassEnum knownArtifactClass);

    /// <summary>Attempts to get node information about the node described by the path.</summary>
    /// <param name="path">Full pathname of the file.</param>
    /// <param name="node">[out] The node information.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetNodeInfo(string path, out FhirNodeInfo node);
}
