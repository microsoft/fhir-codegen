using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Fhir.CodeGen.Packages.RegistryClients;

public record class ResolvedDirectiveUri
{
    public required Uri TarballUri { get; init; }
    public required string? ShaSum { get; init; }
    public required FhirSemVer ResolvedVersion { get; init; }
}


public interface IRegistryClient
{
    static IRegistryClient Create(RegistryEndpointRecord record, HttpClient? httpClient = null) =>
        record.RegistryType switch
        {
            RegistryEndpointRecord.RegistryTypeCodes.FhirNpm => new FhirNpmClient(record, httpClient),
            RegistryEndpointRecord.RegistryTypeCodes.FhirCi => new FhirCiClient(record, httpClient),
            _ => throw new Exception($"Unsupported registry type: {record.RegistryType}"),
        };

    RegistryEndpointRecord.RegistryTypeCodes SupportedRegistryType { get; }

    List<PackageDirective.DirectiveNameTypeCodes> SupportedNameTypes { get; }
    List<PackageDirective.DirectiveVersionCodes> SupportedVersionTypes { get; }

    RegistryEndpointRecord RegistryEndpoint { get; }

    bool SupportsFindByName { get; }
    bool SupportsFindByCanonical { get; }
    bool SupportsFindByFhirVersion { get; }

    List<RegistryCatalogRecord>? Find(
        string? name = null,
        string? packageCanonical = null,
        string? canonical = null,
        string? fhirVersion = null);

    FullPackageManifest? GetFullManifest(string packageId);

    PackageManifest? Resolve(
        PackageDirective directive,
        FhirReleases.FhirSequenceCodes? requiredFhirSequence = null,
        bool allowPrerelease = true);

    ResolvedDirectiveUri? GetDownloadUri(
        PackageDirective directive,
        bool requireRegistryResolution = false);

    (HttpStatusCode status, string? result) GetJsonContent(Uri requestUri);

    (HttpStatusCode status, Stream? content) GetHttpStream(Uri requestUri);
}
