using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.Packaging;

namespace Fhir.CodeGen.Packages.Models;

public record class PackageDirective
{
    /// <summary>Values that represent directive name type codes.</summary>
    public enum DirectiveNameTypeCodes
    {
        Unknown,
        CoreFull,
        CorePartial,
        GuideWithSuffix,
        GuideWithoutSuffix,
    }

    /// <summary>Values that represent directive version codes.</summary>
    public enum DirectiveVersionCodes
    {
        Unknown,
        Exact,
        Wildcard,
        Latest,
        LocalBuild,
        CiBuild,
        NonSemVer,
    }

    /// <summary>Initializes a new instance of the <see cref="PackageDirective"/> class.</summary>
    public PackageDirective() { }

    [SetsRequiredMembers]
    public PackageDirective(string requestedDirective)
    {
        RequestedDirective = requestedDirective.Replace('#', '@');
        parseDirective(requestedDirective);
    }

    [SetsRequiredMembers]
    public PackageDirective(string packageId, string version)
    {
        RequestedDirective = packageId + "@" + version;
        parseDirective(RequestedDirective);
    }

    /// <summary>Initializes a new instance of the PackageDirective class.</summary>
    /// <param name="other">The instance to copy from.</param>
    [SetsRequiredMembers]
    protected PackageDirective(PackageDirective other)
    {
        RequestedDirective = other.RequestedDirective;
        PackageId = other.PackageId;
        RequestedVersion = other.RequestedVersion;
        ResolvedVersion = other.ResolvedVersion;
        FhirCacheVersion = other.FhirCacheVersion;
        NameType = other.NameType;
        VersionType = other.VersionType;
        DeclaredWithAlias = other.DeclaredWithAlias;
        NpmAlias = other.NpmAlias;
        RequestedCiBranch = other.RequestedCiBranch;
        RegistryResolutions = other.RegistryResolutions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        FhirCiResolution = other.FhirCiResolution;
        FullPackageManifests = other.FullPackageManifests.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        CatalogEntries = other.CatalogEntries.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToDictionary(kvp2 => kvp2.Key, kvp2 => kvp2.Value with { }));
    }

    /// <summary>Gets or sets the directive.</summary>
    public required string RequestedDirective { get; init; }

    /// <summary>Gets or sets the identifier of the package.</summary>
    public string? PackageId { get; set; } = null;

    private string? _requestedVersion = null;
    private FhirSemVer? _requestedAsSemVer = null;
    public string? RequestedVersion
    {
        get => _requestedVersion;
        set
        {
            _requestedVersion = value;
            if (value is not null)
            {
                _requestedAsSemVer = new(value);
            }
        }
    }
    public FhirSemVer? RequestedVersionParsed => _requestedAsSemVer;

    public FhirSemVer? ResolvedVersion { get; set; } = null;

    public FhirSemVer? FhirCacheVersion { get; set; } = null;

    public string? FhirCacheDirective => ((PackageId is null) || (FhirCacheVersion is null))
        ? null
        : PackageId + "#" + FhirCacheVersion.ToString();

    public string? NpmDirective => (PackageId is null)
        ? null
        : (ResolvedVersion is not null)
            ? PackageId + "@" + ResolvedVersion.ToString()
            : (FhirCacheVersion is not null)
                ? PackageId + "@" + FhirCacheVersion.ToString()
                : null;

    public string AnyDirective => ((PackageId is null) || (RequestedVersion is null))
        ? RequestedDirective
        : PackageId + "@" + RequestedVersion;

    public DirectiveNameTypeCodes NameType { get; set; } = DirectiveNameTypeCodes.Unknown;
    public DirectiveVersionCodes VersionType { get; set; } = DirectiveVersionCodes.Unknown;

    public bool DeclaredWithAlias { get; set; } = false;
    public string? NpmAlias { get; set; } = null;

    public string? RequestedCiBranch { get; set; } = null;

    public Dictionary<RegistryEndpointRecord, PackageManifest> RegistryResolutions { get; set; } = [];

    public FhirCiQaRecord? FhirCiResolution { get; set; } = null;


    /// <summary>The manifests.</summary>
    public Dictionary<RegistryEndpointRecord, FullPackageManifest> FullPackageManifests = [];

    /// <summary>The catalog entries.</summary>
    /// <remarks>
    /// Note these records contain *very* limited data
    /// </remarks>
    public Dictionary<RegistryEndpointRecord, Dictionary<string, RegistryCatalogRecord>> CatalogEntries = [];

    private void parseDirective(string directive)
    {
        ReadOnlySpan<char> span = directive.AsSpan().Trim();

        // first, check for an alias
        int aliasLiteralLoc = span.IndexOf("@npm:", StringComparison.Ordinal);
        if (aliasLiteralLoc > 0)
        {
            NpmAlias = directive[0..aliasLiteralLoc];
            DeclaredWithAlias = true;
            span = span.Slice(aliasLiteralLoc + 5);
        }

        // find the id-version delimiter
        int delimiterLoc = span.IndexOfAny('@', '#');
        if (delimiterLoc == -1)
        {
            // this is a version-only, default to 'latest'
            PackageId = span.ToString();
            RequestedVersion = "latest";
        }
        else
        {
            PackageId = span[0..delimiterLoc].ToString();
            RequestedVersion = span[(delimiterLoc + 1)..].ToString();
        }

        // determine the type of package
        if (FhirPackageUtils.PackageIsFhirRelease(PackageId))
        {
            NameType = DirectiveNameTypeCodes.CoreFull;
        }
        else if (FhirPackageUtils.PackageIsFhirCorePartial(PackageId))
        {
            NameType = DirectiveNameTypeCodes.CorePartial;
        }
        else if (FhirPackageUtils.PackageEndsWithFhirVersion(PackageId))
        {
            NameType = DirectiveNameTypeCodes.GuideWithSuffix;
        }
        else
        {
            NameType = DirectiveNameTypeCodes.GuideWithoutSuffix;
        }

        // try to parse via SemVer
        FhirSemVer? parsed = new(RequestedVersion);

        // determine the type of version request
        switch (RequestedVersion.ToLowerInvariant())
        {
            case "latest":
                VersionType = DirectiveVersionCodes.Latest;
                break;

            case "dev":
                VersionType = DirectiveVersionCodes.LocalBuild;
                FhirCacheVersion = parsed;
                break;

            case "current":
                VersionType = DirectiveVersionCodes.CiBuild;
                FhirCacheVersion = parsed;
                break;

            case "*":
                VersionType = DirectiveVersionCodes.Wildcard;
                break;

            default:
                {
                    if (RequestedVersion.StartsWith("current$", StringComparison.OrdinalIgnoreCase))
                    {
                        VersionType = DirectiveVersionCodes.CiBuild;
                        RequestedCiBranch = RequestedVersion[8..];
                        FhirCacheVersion = parsed;
                    }
                    else if (RequestedVersion.StartsWith("dev$", StringComparison.OrdinalIgnoreCase))
                    {
                        VersionType = DirectiveVersionCodes.LocalBuild;
                        RequestedCiBranch = RequestedVersion[4..];
                        FhirCacheVersion = parsed;
                    }
                    else if (!parsed.IsValid)
                    {
                        VersionType = DirectiveVersionCodes.NonSemVer;
                        FhirCacheVersion = parsed;
                    }
                    else if (parsed.IsFullyQualified)
                    {
                        VersionType = DirectiveVersionCodes.Exact;
                        ResolvedVersion = parsed;
                        FhirCacheVersion = parsed;
                    }
                    else
                    {
                        VersionType = DirectiveVersionCodes.Wildcard;
                    }
                }
                break;
        }

    }
}
