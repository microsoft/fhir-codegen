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

namespace Fhir.CodeGen.Packages.RegistryClients;

public class FhirNpmClient : RegistryClientBase, IRegistryClient
{
    public static IRegistryClient Create(PackageRegistryRecord record, HttpClient? client = null) => new FhirNpmClient(record, client);

    public PackageRegistryRecord RegistryRecord { get => _registryRecord; init => _registryRecord = value; }

    public override bool SupportsFindByName => true;
    public override bool SupportsFindByCanonical => true;
    public override bool SupportsFindByFhirVersion => true;

    [SetsRequiredMembers]
    public FhirNpmClient(PackageRegistryRecord registryRecord, HttpClient? client = null)
    {
        _registryRecord = registryRecord;
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
            _registryRecord.ServerUri,
            $"catalog?op=find&name={name}&pkgcanonical={packageCanonical}&canonical={canonical}&fhirversion={fhirVersion}");

        (string? result, System.Net.HttpStatusCode status) = doJsonGet(requestUri);

        if (!status.IsSuccessful())
        {
            Console.WriteLine($"Error retrieving catalog from {_registryRecord.ServerUrl}, status: {status}");
            return null;
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine($"Empty result retrieving catalog from {_registryRecord.ServerUrl}");
            return null;
        }

        List<RegistryCatalogRecord>? records = JsonSerializer.Deserialize<List<RegistryCatalogRecord>>(result);
        return records;
    }

    public override FullPackageManifest? GetFullManifest(string packageId)
    {
        packageId = UrlEncoder.Default.Encode(packageId);
        Uri requestUri = new Uri(
            _registryRecord.ServerUri,
            $"package/{packageId}");

        (string? result, System.Net.HttpStatusCode status) = doJsonGet(requestUri);

        if (!status.IsSuccessful())
        {
            Console.WriteLine($"Error retrieving manifest for {packageId} from {_registryRecord.ServerUrl}, status: {status}");
            return null;
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine($"Empty result retrieving manifest for {packageId} from {_registryRecord.ServerUrl}");
            return null;
        }

        FullPackageManifest? manifest = JsonSerializer.Deserialize<FullPackageManifest>(result);
        return manifest;
    }
}
