// <copyright file="PackageCopyOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Options when copying an IFhirPackageInfo.</summary>
public class PackageCopyOptions
{
    /// <summary>Gets or sets the primitive type map.</summary>
    public Dictionary<string, string> PrimitiveTypeMap { get; set; } = new();

    /// <summary>Gets or sets a list of exports.</summary>
    public IEnumerable<string> ExportList { get; set; } = new List<string>();

    /// <summary>Gets or sets a value indicating whether to copy primitives.</summary>
    public bool CopyPrimitives { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy complex types.</summary>
    public bool CopyComplexTypes { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy resources.</summary>
    public bool CopyResources { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy logical models.</summary>
    public bool CopyLogicalModels { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy extensions.</summary>
    public bool CopyExtensions { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy profiles.</summary>
    public bool CopyProfiles { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy search parameters.</summary>
    public bool CopySearchParameters { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy operations.</summary>
    public bool CopyOperations { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to copy implementation guides.</summary>
    public bool CopyImplementationGuides { get; set; } = true;

    /// <summary>Gets or sets the extension urls.</summary>
    public HashSet<string> ExtensionUrls { get; set; } = null;

    /// <summary>Gets or sets the extension element paths.</summary>
    public HashSet<string> ExtensionElementPaths { get; set; } = null;

    /// <summary>Gets or sets information describing the server.</summary>
    public Models.FhirServerInfo ServerInfo { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the experimental should be included.
    /// </summary>
    public bool IncludeExperimental { get; set; } = false;
}
