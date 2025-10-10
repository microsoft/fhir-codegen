using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Hl7.Fhir.Rest;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Fhir.CodeGen.Packages.RegistryClients;

public class FhirNpmClient : RegistryClientBase, IPackageRegistryClient
{
    private static readonly List<PackageDirective.DirectiveNameTypeCodes> _supportedNameTypes = [
        PackageDirective.DirectiveNameTypeCodes.CoreFull,
        PackageDirective.DirectiveNameTypeCodes.CorePartial,
        PackageDirective.DirectiveNameTypeCodes.GuideWithSuffix,
        PackageDirective.DirectiveNameTypeCodes.GuideWithoutSuffix,
        ];

    private static readonly List<PackageDirective.DirectiveVersionCodes> _supportedVersionTypes = [
        PackageDirective.DirectiveVersionCodes.Exact,
        PackageDirective.DirectiveVersionCodes.Wildcard,
        PackageDirective.DirectiveVersionCodes.Latest,
        ];

    public RegistryEndpointRecord.RegistryTypeCodes SupportedRegistryType { get => RegistryEndpointRecord.RegistryTypeCodes.FhirNpm; }
    public List<PackageDirective.DirectiveNameTypeCodes> SupportedNameTypes { get => _supportedNameTypes; }
    public List<PackageDirective.DirectiveVersionCodes> SupportedVersionTypes { get => _supportedVersionTypes; }
    
    public RegistryEndpointRecord RegistryEndpoint { get => _registryEndpoint; init => _registryEndpoint = value; }

    public override bool SupportsFindByName => true;
    public override bool SupportsFindByCanonical => true;
    public override bool SupportsFindByFhirVersion => true;

    [SetsRequiredMembers]
    public FhirNpmClient(RegistryEndpointRecord registryEndpoint, HttpClient? client = null)
    {
        if (registryEndpoint.RegistryType != SupportedRegistryType)
        {
            throw new ArgumentException($"Cannot create a {nameof(FhirNpmClient)} using a registry endpoint of type '{registryEndpoint.RegistryType}'");
        }

        _registryEndpoint = registryEndpoint;
        _httpClient = client ?? new HttpClient();
    }

    public override List<RegistryCatalogRecord>? Find(
        string? name = null,
        string? packageCanonical = null,
        string? canonical = null,
        string? fhirVersion = null)
    {
        name = UrlEncoder.Default.Encode(name ?? string.Empty);
        packageCanonical = UrlEncoder.Default.Encode(packageCanonical ?? string.Empty);
        canonical = UrlEncoder.Default.Encode(canonical ?? string.Empty);
        fhirVersion = UrlEncoder.Default.Encode(fhirVersion ?? string.Empty);

        Uri requestUri = new Uri(
            _registryEndpoint.ServerUri,
            $"catalog?op=find&name={name}&pkgcanonical={packageCanonical}&canonical={canonical}&fhirversion={fhirVersion}");

        (System.Net.HttpStatusCode status, string? result) = GetJsonContent(requestUri);

        if (!status.IsSuccessful())
        {
            Console.WriteLine($"Error retrieving catalog from {_registryEndpoint.Url}, status: {status}");
            return null;
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine($"Empty result retrieving catalog from {_registryEndpoint.Url}");
            return null;
        }

        List<RegistryCatalogRecord>? records = JsonSerializer.Deserialize<List<RegistryCatalogRecord>>(result);
        return records;
    }

    public override FullPackageManifest? GetFullManifest(string packageId)
    {
        packageId = UrlEncoder.Default.Encode(packageId);
        Uri requestUri = new Uri(
            _registryEndpoint.ServerUri,
            $"package/{packageId}");

        (System.Net.HttpStatusCode status, string? result) = GetJsonContent(requestUri);

        if (!status.IsSuccessful())
        {
            Console.WriteLine($"Error retrieving manifest for {packageId} from {_registryEndpoint.Url}, status: {status}");
            return null;
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine($"Empty result retrieving manifest for {packageId} from {_registryEndpoint.Url}");
            return null;
        }

        FullPackageManifest? manifest = JsonSerializer.Deserialize<FullPackageManifest>(result);
        return manifest;
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
            Console.WriteLine($"Cannot resolve package directive '{directive.RequestedDirective}' of type '{directive.VersionType}' using FHIR NPM registry - ignoring request.");
            return null;
        }

        FhirSemVer requestedVersion = new(directive.RequestedVersion ?? "*");

        PackageDirective.DirectiveNameTypeCodes nameType = directive.NameType;
        string packageId = directive.PackageId;
        string requiredSuffix = (requiredFhirSequence is null) 
            ? string.Empty
            : "." + requiredFhirSequence.Value.ToRLiteral();

        // if this is a partial core package, we need to convert it to a full core package
        if (nameType == PackageDirective.DirectiveNameTypeCodes.CorePartial)
        {
            nameType = PackageDirective.DirectiveNameTypeCodes.CoreFull;
            packageId = directive.PackageId + ".core";
        }

        // if we are in a core package, we do not need to check suffixes
        if (nameType == PackageDirective.DirectiveNameTypeCodes.CoreFull)
        {
            requiredSuffix = string.Empty;
        }

        // if we have a required FHIR version and we do not already have a matching suffix, we need to try and resolve
        if ((requiredFhirSequence is not null) &&
            !string.IsNullOrEmpty(requiredSuffix) &&
            !packageId.EndsWith(requiredSuffix, StringComparison.OrdinalIgnoreCase))
        {
            List<RegistryCatalogRecord>? catalogEntries = Find(name: packageId);

            // only update our package id if we found a better option
            if ((catalogEntries is not null) &&
                (catalogEntries.Count > 1))
            {
                // check to see if there is a package that has our sequence as a requiredSuffix
                RegistryCatalogRecord? matchingEntry = catalogEntries
                    .Where(c => c.Name?.EndsWith(requiredSuffix, StringComparison.OrdinalIgnoreCase) ?? false)
                    .FirstOrDefault();

                if (matchingEntry is not null)
                {
                    packageId = matchingEntry.Name!;
                }
            }
        }

        // get the full manifest for this package
        FullPackageManifest? fullPackageManifest = GetFullManifest(packageId);
        if (fullPackageManifest is null)
        {
            Console.WriteLine($"Failed to retrieve the manifest for '{packageId}' from FHIR NPM registry '{RegistryEndpoint.ServerUri}' ({RegistryEndpoint.RegistryType})");
            return null;
        }

        if ((fullPackageManifest.Versions is null) ||
            (fullPackageManifest.Versions.Count == 0))
        {
            Console.WriteLine($"Manifest for '{packageId}' for FHIR NPM registry '{RegistryEndpoint.ServerUri}' ({RegistryEndpoint.RegistryType}) contained no versions!");
            return null;
        }

        // if we are latest, check to see if there is an explicit dist-tag for it
        if ((directive.VersionType == PackageDirective.DirectiveVersionCodes.Latest) &&
            (fullPackageManifest.DistributionTags is not null) &&
            fullPackageManifest.DistributionTags.TryGetValue("latest", out string? latestVersion) &&
            fullPackageManifest.Versions.TryGetValue(latestVersion, out PackageManifest? latestManifest))
        {
            return latestManifest;
        }

        // filter the versions that match our requested version and prerelease filter (if requested)
        IEnumerable<PackageManifest> matchingVersions = fullPackageManifest.Versions.Values
            .Where(pm => (allowPrerelease || (pm.ParsedVersion.IsPrerelease == false)) && pm.ParsedVersion.Satisfies(requestedVersion));

        // return the highest remaining version or null if there are none
        return matchingVersions
            .OrderByDescending(pm => pm.ParsedVersion, new FhirSemVerComparer())
            .FirstOrDefault();
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

        // resolve depending on the version type
        switch (directive.VersionType)
        {
            // if this is an exact version, we can construct the URI based on the known pattern
            case PackageDirective.DirectiveVersionCodes.Exact:
                {
                    if (!requireRegistryResolution &&
                        !string.IsNullOrEmpty(directive.RequestedVersion))
                    {
                        return new()
                        {
                            TarballUri = new(_registryEndpoint.ServerUri, $"{directive.PackageId}/{directive.RequestedVersion}"),
                            ShaSum = null,
                            ResolvedVersion = new(directive.RequestedVersion),
                        };
                    }
                }
                break;

            // non-exact but resolvable version types
            case PackageDirective.DirectiveVersionCodes.Latest:
            case PackageDirective.DirectiveVersionCodes.Wildcard:
                // fall-through to attempt resolving via registry
                break;

            default:
                Console.WriteLine($"Cannot resolve package directive '{directive.RequestedDirective}' of type '{directive.VersionType}' using FHIR NPM registry - ignoring request.");
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
}
