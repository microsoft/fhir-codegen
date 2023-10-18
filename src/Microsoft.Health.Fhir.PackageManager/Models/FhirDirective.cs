// <copyright file="FhirDirective.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.PackageManager.Models;

/// <summary>A FHIR directive.</summary>
internal record class FhirDirective
{
    /// <summary>Values that represent directive name type codes.</summary>
    internal enum DirectiveNameTypeCodes
    {
        Unknown,
        CoreFull,
        CorePartial,
        GuideWithSuffix,
        GuideWithoutSuffix,
    }

    /// <summary>Values that represent directive version codes.</summary>
    internal enum DirectiveVersionCodes
    {
        Unknown,
        Exact,
        Partial,
        Latest,
        Local,
        ContinuousIntegration,
        NonSemVer,
    }

    /// <summary>Initializes a new instance of the <see cref="FhirDirective"/> class.</summary>
    public FhirDirective() { }

    /// <summary>Initializes a new instance of the FhirDirective class.</summary>
    /// <param name="other">The instance to copy from.</param>
    [SetsRequiredMembers]
    protected FhirDirective(FhirDirective other)
    {
        Directive = other.Directive;
        PackageId = other.PackageId;
        NameType = other.NameType;
        FhirRelease = other.FhirRelease;
        PackageVersion = other.PackageVersion;
        VersionType = other.VersionType;
        CiUrl = other.CiUrl;
        CiOrg = other.CiOrg;
        CiBranch = other.CiBranch;
        Manifests = other.Manifests.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
    }

    /// <summary>Gets or sets the directive.</summary>
    public required string Directive { get; init; }

    /// <summary>Gets or sets the identifier of the package.</summary>
    public string PackageId { get; set; } = string.Empty;

    /// <summary>Type of the name.</summary>
    public DirectiveNameTypeCodes NameType = DirectiveNameTypeCodes.Unknown;

    /// <summary>Gets or sets the FHIR release.</summary>
    public string FhirRelease { get; set; } = string.Empty;

    /// <summary>Gets or sets the package version.</summary>
    public string PackageVersion { get; set; } = string.Empty;

    /// <summary>Type of the version.</summary>
    public DirectiveVersionCodes VersionType = DirectiveVersionCodes.Unknown;

    /// <summary>Gets or sets URL of the resolved tarball.</summary>
    public string ResolvedTarballUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the resolved sha checksum.</summary>
    public string ResolvedSha { get; set; } = string.Empty;

    /// <summary>Gets or sets URL of the ci.</summary>
    public string CiUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the ci organisation.</summary>
    public string CiOrg { get; set; } = string.Empty;

    /// <summary>Gets or sets the ci branch.</summary>
    public string CiBranch { get; set; } = string.Empty;

    /// <summary>The build date.</summary>
    public string BuildDate { get; set; } = string.Empty;

    /// <summary>The manifests.</summary>
    public Dictionary<Uri, RegistryPackageManifest> Manifests = new();

    /// <summary>The catalog entries.</summary>
    /// <remarks>
    /// Note these records contain *very* limited data, but they are wire-compatible
    /// and it is not worth maintaining multiple models for the same data.
    /// </remarks>
    public Dictionary<Uri, Dictionary<string, FhirNpmPackageDetails>> CatalogEntries = new();
}
