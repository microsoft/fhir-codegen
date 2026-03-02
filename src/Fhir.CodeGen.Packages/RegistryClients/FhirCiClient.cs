using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Hl7.Fhir.Rest;
using Fhir.CodeGen.Common.Packaging;

namespace Fhir.CodeGen.Packages.RegistryClients;

public class FhirCiClient : RegistryClientBase, IPackageRegistryClient
{
    private static readonly List<PackageDirective.DirectiveNameTypeCodes> _supportedNameTypes = [
        PackageDirective.DirectiveNameTypeCodes.CoreFull,
        PackageDirective.DirectiveNameTypeCodes.CorePartial,
        PackageDirective.DirectiveNameTypeCodes.GuideWithSuffix,
        PackageDirective.DirectiveNameTypeCodes.GuideWithoutSuffix,
        ];

    private static readonly List<PackageDirective.DirectiveVersionCodes> _supportedVersionTypes = [
        PackageDirective.DirectiveVersionCodes.CiBuild,
        ];

    public RegistryEndpointRecord.RegistryTypeCodes SupportedRegistryType { get => RegistryEndpointRecord.RegistryTypeCodes.FhirCi; }
    public List<PackageDirective.DirectiveNameTypeCodes> SupportedNameTypes { get => _supportedNameTypes; }
    public List<PackageDirective.DirectiveVersionCodes> SupportedVersionTypes { get => _supportedVersionTypes; }

    public RegistryEndpointRecord RegistryEndpoint { get => _registryEndpoint; init => _registryEndpoint = value; }

    public override bool SupportsFindByName => true;
    public override bool SupportsFindByCanonical => false;
    public override bool SupportsFindByFhirVersion => false;

    /// <summary>(Immutable) URI of the ig list.</summary>
    private static readonly Uri _igListUri = new("https://github.com/FHIR/ig-registry/blob/master/fhir-ig-list.json");

    private Uri _igQasUri = new("https://build.fhir.org/ig/qas.json");
    private Uri _coreCiBranchUri = new("https://build.fhir.org/branches/");
    private Uri _coreCiIgUri = new("https://build.fhir.org/ig/");
    private static Uri _fhirIgListUri = new("https://raw.githubusercontent.com/FHIR/ig-registry/refs/heads/master/fhir-ig-list.json");

    private List<FhirCiQaRecord> _igQaRecords = [];
    private List<FhirCiBranchRecord> _coreBranchRecords = [];
    private DateTimeOffset _qasLastRefresh = DateTimeOffset.MinValue;
    private static readonly TimeSpan _qaStaleSpan = TimeSpan.FromMinutes(30);
    private static Lock _qaRefreshLock = new();
    private ILookup<string?, FhirCiQaRecord>? _igQaRecordsByPackageId = null;

    private string _coreCiPackageId;
    private FhirReleases.FhirSequenceCodes _coreCiFhirSequence;

    private static readonly HashSet<string> _defaultBranchNames = [ "main", "master" ];

    [SetsRequiredMembers]
    public FhirCiClient(RegistryEndpointRecord registryEndpoint, HttpClient? client = null)
    {
        if (registryEndpoint.RegistryType != SupportedRegistryType)
        {
            throw new ArgumentException($"Cannot create a {nameof(FhirCiClient)} using a registry endpoint of type '{registryEndpoint.RegistryType}'");
        }

        _registryEndpoint = registryEndpoint;
        _httpClient = client ?? new HttpClient();

        _igQasUri = new(_registryEndpoint.ServerUri, "ig/qas.json");
        _coreCiBranchUri = new(_registryEndpoint.ServerUri, "branches/");

        _coreCiFhirSequence = Enum.GetValues<FhirReleases.FhirSequenceCodes>().Max();
        _coreCiPackageId = FhirReleases.ToCorePackageDirective(_coreCiFhirSequence);
    }

    public override List<RegistryCatalogRecord>? Find(
        string? name = null,
        string? packageCanonical = null,
        string? canonical = null,
        string? fhirVersion = null)
    {
        updateIndicies();

        IEnumerable<FhirCiQaRecord>? qaMatches = null;
        IEnumerable<FhirCiBranchRecord>? coreMatches = null;

        FhirReleases.FhirSequenceCodes? fhirSequence = string.IsNullOrEmpty(fhirVersion)
            ? null
            : FhirReleases.FhirVersionToSequence(fhirVersion);

        if (!string.IsNullOrEmpty(name))
        {
            qaMatches = (qaMatches ?? _igQaRecords)
                .Where(r => r.PackageId?.StartsWith(name, StringComparison.OrdinalIgnoreCase) ?? false);

            coreMatches = _coreCiPackageId.StartsWith(name, StringComparison.OrdinalIgnoreCase)
                ? (coreMatches ?? _coreBranchRecords)
                : [];
        }

        if (!string.IsNullOrEmpty(packageCanonical))
        {
            qaMatches = (qaMatches ?? _igQaRecords)
                .Where(r => r.Url?.StartsWith(packageCanonical, StringComparison.OrdinalIgnoreCase) ?? false);

            coreMatches = "http://hl7.org/fhir".StartsWith(packageCanonical, StringComparison.OrdinalIgnoreCase)
                ? (coreMatches ?? _coreBranchRecords)
                : [];
        }

        if (fhirSequence is not null)
        {
            qaMatches = (qaMatches ?? _igQaRecords)
                .Where(r => r.FhirVersion is null ? true : (FhirReleases.FhirVersionToSequence(r.FhirVersion) == fhirSequence));

            coreMatches = _coreCiFhirSequence == fhirSequence
                ? (coreMatches ?? _coreBranchRecords)
                : [];
        }

        List<RegistryCatalogRecord> catalogRecords = (qaMatches ?? _igQaRecords)
            .Select(qa => new RegistryCatalogRecord()
            {
                Name = qa.PackageId,
                Description = qa.Description,
                FhirVersion = qa.FhirVersion,
                PublicationDate = qa.BuildDate?.DateTime ?? qa.BuildDateIso?.DateTime,
                Version = qa.PackageVersion,
                ResourceCount = null,
                Canonical = qaUrlToCanonical(qa.Url),
                Kind = null,
                Url = qa.Url,
                Scope = null,
                Keywords = null,
                GitHubOrg = qa.GitHubOrg,
                GitHubProject = qa.GitHubProject,
                GitHubBranch = qa.GitHubBranch,
            })
            .ToList();

        foreach (FhirCiBranchRecord coreBranch in (coreMatches ?? _coreBranchRecords))
        {
            if (string.IsNullOrEmpty(coreBranch.Name) ||
                coreBranch.Name.Equals("__default/", StringComparison.Ordinal))
            {
                continue;
            }

            catalogRecords.Add(new()
            {
                Name = _coreCiPackageId,
                Description = null,
                FhirVersion = _coreCiFhirSequence.ToLongVersion(),
                PublicationDate = coreBranch.LastModified?.DateTime,
                Version = null,
                ResourceCount = null,
                Canonical = "http://hl7.org/fhir",
                Kind = null,
                Url = new Uri(_coreCiBranchUri, coreBranch.Name).AbsoluteUri,
                Scope = null,
                Keywords = null,
                GitHubOrg = "hl7",
                GitHubProject = "fhir",
                GitHubBranch = coreBranch.Name.EndsWith('/')
                    ? coreBranch.Name![0..^1]
                    : coreBranch.Name,
            });
        }

        return catalogRecords;
    }

    private string? qaUrlToCanonical(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        int igLitLoc = input.IndexOf("/ImplementationGuide/", StringComparison.Ordinal);
        if (igLitLoc == -1)
        {
            return input;
        }

        return input[0..igLitLoc];
    }

    public override FullPackageManifest? GetFullManifest(string packageId)
    {
        updateIndicies();

        // check if this is a core package request
        if (FhirPackageUtils.PackageIsFhirCore(packageId) ||
            FhirPackageUtils.PackageIsFhirCorePartial(packageId))
        {
            return buildCoreCiManifest(packageId);
        }

        return buildIgCiManifest(packageId);
    }

    private FullPackageManifest? buildIgCiManifest(string packageId)
    {
        // get the qa records that match this package id
        List<FhirCiQaRecord> qaRecords = _igQaRecords
            .Where(r => r.PackageId?.Equals(packageId, StringComparison.OrdinalIgnoreCase) ?? false)
            .ToList();

        if (qaRecords.Count == 0)
        {
            return null;
        }

        Dictionary<string, PackageManifest> versions = [];
        DateTimeOffset? mainBranchModified = null;

        // build our version information
        foreach (FhirCiQaRecord qaRec in qaRecords)
        {
            if (string.IsNullOrEmpty(qaRec.GitHubBranch))
            {
                continue;
            }

            string? versionTag = null;

            if (_defaultBranchNames.Contains(qaRec.GitHubBranch))
            {
                // check if we already have a main branch (some repos have 'main' and 'master' still)
                if (mainBranchModified is not null)
                {
                    // keep same or newer
                    if (mainBranchModified >= (qaRec.BuildDate ?? qaRec.BuildDateIso ?? DateTimeOffset.MinValue))
                    {
                        continue;
                    }
                }

                mainBranchModified = qaRec.BuildDate ?? qaRec.BuildDateIso ?? DateTimeOffset.MinValue;
                versionTag = "current";
            }

            versionTag ??= "current$" + qaRec.GitHubBranch;

            string branchUrl = qaRec.RepositoryUrl is null
                ? qaRec.GitHubBranch
                : qaRec.RepositoryUrl.Replace("/qa.json", string.Empty);

            Uri branchUri = branchUrl.EndsWith('/')
                ? new(_coreCiIgUri, branchUrl)
                : new(_coreCiIgUri, branchUrl + "/");

            versions[versionTag] = new()
            {
                Name = qaRec.PackageId ?? packageId,
                FhirVersion = qaRec.FhirVersion is null
                    ? null
                    : [qaRec.FhirVersion],
                Version = versionTag,
                Distribution = new()
                {
                    TarballUrl = new Uri(branchUri, "package.tgz").AbsoluteUri,
                },
                CanonicalUrl = "http://hl7.org/fhir",
                WebPublicationUrl = branchUri.AbsoluteUri,
                PublicationDate = qaRec.BuildDate?.DateTime ?? qaRec.BuildDateIso?.DateTime,
                OriginalVersion = qaRec.PackageVersion,
            };
        }

        if (versions.Count == 0)
        {
            return null;
        }

        // construct a package manifest using the branches as versions
        return new()
        {
            Id = packageId,
            Name = packageId,
            Versions = versions,
        };
    }

    private FullPackageManifest? buildCoreCiManifest(string packageId)
    {
        // determine the FHIR version of the request
        FhirReleases.FhirSequenceCodes requestedCoreSequence = FhirReleases.FhirVersionToSequence(packageId);

        // allow the known match and unknown versions
        if ((requestedCoreSequence != _coreCiFhirSequence) &&
            (requestedCoreSequence != FhirReleases.FhirSequenceCodes.Unknown))
        {
            return null;
        }

        // if we do not have any branches, we are sunk (note this generally means we are offline - there is always a primary core branch)
        if (_coreBranchRecords.Count == 0)
        {
            return null;
        }

        Dictionary<string, PackageManifest> versions = [];
        DateTimeOffset? mainBranchModified = null;

        foreach (FhirCiBranchRecord coreBranchRec in _coreBranchRecords)
        {
            if (string.IsNullOrEmpty(coreBranchRec.Name) ||
                coreBranchRec.Name.Equals("__default/", StringComparison.Ordinal))
            {
                continue;
            }

            string coreBranchName = coreBranchRec.Name[0..^1];
            string? versionTag = null;

            if (_defaultBranchNames.Contains(coreBranchName))
            {
                // check if we already have a main branch (some repos have 'main' and 'master' still)
                if (mainBranchModified is not null)
                {
                    // keep same or newer
                    if (mainBranchModified >= (coreBranchRec.LastModified ?? DateTimeOffset.MinValue))
                    {
                        continue;
                    }
                }

                mainBranchModified = coreBranchRec.LastModified ?? DateTimeOffset.MinValue;
                versionTag = "current";
            }

            if (versionTag is null)
            {
                versionTag = "current$" + coreBranchName;
            }

            // try to get the manifest
            (HttpStatusCode httpStatus, string? corePackageManifestJson) = GetJsonContent(
                new Uri(_coreCiBranchUri, $"{coreBranchName}/{requestedCoreSequence.ToCorePackageId()}.manifest.json"));

            if (httpStatus.IsSuccessful() &&
                (corePackageManifestJson is not null))
            {
                PackageManifest? coreCiManifest = JsonSerializer.Deserialize<PackageManifest>(corePackageManifestJson);

                if (coreCiManifest is not null)
                {
                    versions[versionTag] = new()
                    {
                        Name = coreCiManifest.Name,
                        Description = "FHIR Core " + requestedCoreSequence.ToRLiteral(),
                        FhirVersion = coreCiManifest.AnyFhirVersions ?? [requestedCoreSequence.ToLongVersion()],
                        Version = versionTag,
                        Distribution = new()
                        {
                            TarballUrl = new Uri(_coreCiBranchUri, $"{coreBranchName}/{requestedCoreSequence.ToCorePackageId()}.tgz").AbsoluteUri,
                        },
                        Type = "Core",
                        CanonicalUrl = "http://hl7.org/fhir",
                        WebPublicationUrl = new Uri(_coreCiBranchUri, coreBranchName + "/").AbsoluteUri,
                        PublicationDate = coreCiManifest.PublicationDate ?? coreBranchRec.LastModified?.DateTime,
                        OriginalVersion = coreCiManifest.Version,
                    };

                    continue;
                }
            }

            // get the version information
            (httpStatus, string? coreVersionInfo) = GetContent(new Uri(_coreCiBranchUri, coreBranchName + "/version.info"), "text/plain");
            if (!httpStatus.IsSuccessful() ||
                (coreVersionInfo is null))
            {
                // if there is neither a manifest nor a version.info, the build cannot be used
                continue;
            }

            // grab the contents we can out of the version.info file
            parseVersionInfoIni(
                coreVersionInfo,
                out string coreFhirVersion,
                out string coreVersion,
                out string coreBuildId,
                out DateTimeOffset? coreBuildDate);

            versions[versionTag] = new()
            {
                Name = requestedCoreSequence.ToCorePackageId(),
                Description = "FHIR Core CI - " + requestedCoreSequence.ToRLiteral(),
                FhirVersion = [requestedCoreSequence.ToLongVersion()],
                Version = versionTag,
                Distribution = new()
                {
                    TarballUrl = new Uri(_coreCiBranchUri, $"{coreBranchName}/{requestedCoreSequence.ToCorePackageId()}.tgz").AbsoluteUri,
                },
                Type = "Core",
                CanonicalUrl = "http://hl7.org/fhir",
                WebPublicationUrl = new Uri(_coreCiBranchUri, coreBranchName + "/").AbsoluteUri,
                PublicationDate = coreBuildDate?.DateTime ?? coreBranchRec.LastModified?.DateTime,
                OriginalVersion = coreVersion,
            };

            continue;
        }

        if (versions.Count == 0)
        {
            return null;
        }

        // construct a package manifest using the core branches as versions
        return new()
        {
            Id = requestedCoreSequence.ToCorePackageId(),
            Name = requestedCoreSequence.ToCorePackageId(),
            Description = "FHIR Core CI - " + requestedCoreSequence.ToRLiteral(),
            Versions = versions,
        };
    }

    /// <summary>Gets local version information.</summary>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <param name="contents">   The contents.</param>
    /// <param name="fhirVersion">[out] The FHIR version.</param>
    /// <param name="version">    [out] The version string (e.g., 4.0.1).</param>
    /// <param name="buildId">    [out] Identifier for the build.</param>
    /// <param name="buildDate">  [out] The build date.</param>
    private static void parseVersionInfoIni(
        string contents,
        out string fhirVersion,
        out string version,
        out string buildId,
        out DateTimeOffset? buildDate)
    {
        IEnumerable<string> lines = contents.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        fhirVersion = string.Empty;
        version = string.Empty;
        buildId = string.Empty;
        buildDate = null;

        foreach (string line in lines)
        {
            if (!line.Contains('='))
            {
                continue;
            }

            string[] kvp = line.Split('=');

            if (kvp.Length != 2)
            {
                continue;
            }

            switch (kvp[0])
            {
                case "FhirVersion":
                    fhirVersion = kvp[1];
                    break;

                case "version":
                    version = kvp[1];
                    break;

                case "buildId":
                    buildId = kvp[1];
                    break;

                case "date":
                    {
                        if (DateTimeOffset.TryParseExact(
                            kvp[1],
                            "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture.DateTimeFormat,
                            DateTimeStyles.None, out DateTimeOffset dto))
                        {
                            buildDate = dto;
                        }
                    }
                    break;
            }
        }
    }

    public PackageManifest? Resolve(
        PackageDirective directive,
        FhirReleases.FhirSequenceCodes? requiredFhirSequence = null,
        bool allowPrerelease = true)
    {
        if (directive.PackageId is null)
        {
            Console.WriteLine($"Cannot resolve a directive that has a null package id!");
            return null;
        }

        if (!_supportedVersionTypes.Contains(directive.VersionType))
        {
            Console.WriteLine($"Cannot resolve package directive '{directive.RequestedDirective}' of type '{directive.VersionType}' using FHIR CI client - ignoring request.");
            return null;
        }

        updateIndicies();

        FullPackageManifest? fullPackageManifest = null;

        // check if this is a core package request
        if (FhirPackageUtils.PackageIsFhirCore(directive.PackageId) ||
            FhirPackageUtils.PackageIsFhirCorePartial(directive.PackageId))
        {
            fullPackageManifest = buildCoreCiManifest(directive.PackageId);
        }
        else
        {
            fullPackageManifest = buildIgCiManifest(directive.PackageId);
        }

        if ((fullPackageManifest is null) ||
            (fullPackageManifest.Versions is null) ||
            (fullPackageManifest.Versions.Count == 0))
        {
            return null;
        }

        string versionLiteral = directive.RequestedVersion
            ?? (directive.RequestedCiBranch is null ? null : $"current${directive.RequestedCiBranch}")
            ?? "current";

        if (fullPackageManifest.Versions.TryGetValue(versionLiteral, out PackageManifest? pm))
        {
            return pm;
        }

        return null;
    }

    public ResolvedDirectiveUri? GetDownloadUri(
        PackageDirective directive,
        bool requireRegistryResolution = false)
    {
        if (directive.PackageId is null)
        {
            Console.WriteLine($"Cannot resolve a directive that has a null package id!");
            return null;
        }

        if (!_supportedVersionTypes.Contains(directive.VersionType))
        {
            Console.WriteLine($"Cannot resolve package directive '{directive.RequestedDirective}' of type '{directive.VersionType}' using FHIR CI client - ignoring request.");
            return null;
        }

        PackageManifest? versionManifest = Resolve(directive);
        if ((versionManifest?.Distribution is null) ||
            string.IsNullOrEmpty(versionManifest.Distribution.TarballUrl))
        {
            return null;
        }

        return new()
        {
            TarballUri = new(versionManifest.Distribution.TarballUrl),
            ShaSum = versionManifest.Distribution.ShaSum,
            ResolvedVersion = new(versionManifest.Version),
        };
    }

    private void updateIndicies()
    {
        if (DateTimeOffset.Now.Subtract(_qasLastRefresh) <= _qaStaleSpan)
        {
            return;
        }

        try
        {
            lock (_qaRefreshLock)
            {
                // check to see if someone else refreshed while waiting for the lock
                if (DateTimeOffset.Now.Subtract(_qasLastRefresh) <= _qaStaleSpan)
                {
                    return;
                }

                (HttpStatusCode status, string? json) = GetJsonContent(_igQasUri);
                if (!status.IsSuccessful() ||
                    (json is null))
                {
                    Console.WriteLine($"Failed to retrieve QA records: {status} from '{_igQasUri.AbsoluteUri}'");
                    return;
                }

                _igQaRecords = JsonSerializer.Deserialize<List<FhirCiQaRecord>>(json) ?? [];
                _igQaRecordsByPackageId = _igQaRecords.ToLookup(r => r.PackageId);

                (status, json) = GetJsonContent(_coreCiBranchUri);
                if (!status.IsSuccessful() ||
                    (json is null))
                {
                    Console.WriteLine($"Failed to core branch records: {status} from '{_coreCiBranchUri.AbsoluteUri}'");
                    return;
                }

                _coreBranchRecords = JsonSerializer.Deserialize<List<FhirCiBranchRecord>>(json) ?? [];

                // flag we have updated
                _qasLastRefresh = DateTimeOffset.Now;
            }
        }
        catch (Exception ex)
        {
            // swallow errors and continue
            Console.WriteLine($"Failed to update the CI index content: {ex.Message}");
        }
    }
}
