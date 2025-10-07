using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Packages.RegistryClients;
using Shouldly;
using static Fhir.CodeGen.Packages.Tests.FullManifestExpectations;

namespace Fhir.CodeGen.Packages.Tests;

public class FhirRegistryHttpTests : IClassFixture<FhirRegistryHttpTestFixture>
{
    private readonly FhirRegistryHttpTestFixture _fixture;
    public FhirRegistryHttpTests(FhirRegistryHttpTestFixture fixture)
    {
        _fixture = fixture;
        _fullManifestExpectationLookup = _fullManifestExpectations.ToLookup(e => (e.PackageName, e.RegistrySource));
    }

    //private record class FullManifestExpectationRecord
    //{
    //    public required string Id { get; init; }
    //    public required bool ShouldBeNull { get; init; }
    //    public required int EntryCountPrimary { get; init; }
    //    public required int EntryCountSecondary { get; init; }
    //    public required int EntryCountCi { get; init; }
    //}

    private ILookup<(string PackageId, PackageRegistryRecord.RegistryTypeCodes? RegistryType), FullManifestExpectationRecord> _fullManifestExpectationLookup;

    [Theory]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirPrimary, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirSecondary, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirPrimary, "hl7.fhir.uv.subscriptions-backport.r4")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirSecondary, "hl7.fhir.uv.subscriptions-backport.r4")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirPrimary, "hl7.fhir.uv.subscriptions-backport.r4b")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirSecondary, "hl7.fhir.uv.subscriptions-backport.r4b")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirPrimary, "hl7.fhir.r4.core")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirSecondary, "hl7.fhir.r4.core")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirPrimary, "hl7.fhir.r4")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirSecondary, "hl7.fhir.r4")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirPrimary, "hl7.fhir.us.core")]
    [InlineData(PackageRegistryRecord.RegistryTypeCodes.FhirSecondary, "hl7.fhir.us.core")]
    public void RegistryCatalogFind(
        PackageRegistryRecord.RegistryTypeCodes registryType,
        string packageId)
    {
        PackageRegistryRecord registryRec = _fixture._registries[registryType];

        if (!_fullManifestExpectationLookup.Contains((packageId, registryType)))
        {
            throw new ArgumentOutOfRangeException(nameof(packageId), $"No expectations found for package ID '{packageId}' and registry type '{registryType}'.");
        }

        FullManifestExpectationRecord? expectations = _fullManifestExpectationLookup[(packageId, registryType)].FirstOrDefault();
        expectations.ShouldNotBeNull($"Missing expectations for package ID '{packageId}'.");

        IRegistryClient client = registryType switch
        {
            PackageRegistryRecord.RegistryTypeCodes.FhirPrimary => FhirNpmClient.Create(registryRec, _fixture._httpClient),
            PackageRegistryRecord.RegistryTypeCodes.FhirSecondary => FhirNpmClient.Create(registryRec, _fixture._httpClient),
            //PackageRegistryRecord.RegistryTypeCodes.FhirCi => FhirNpmClient.Create(registryRec, _fixture._httpClient),
            _ => throw new ArgumentOutOfRangeException(nameof(registryType), $"Unsupported registry type: {registryType}"),
        };

        List<RegistryCatalogRecord>? results = client.Find(name: packageId);
        results.ShouldNotBeNull($"Expected non-null results for registry type {registryType}.");

        if (expectations.CatalogCount is not null)
        {
            results.Count.ShouldBe(
                expectations.CatalogCount.Value,
                $"Expected {expectations.CatalogCount} catalog entries for package '{packageId}' in registry type '{registryType}', but found {results.Count}.");
        }
    }

    public void RegistryGetFullManifest()
    {

    }

}

public class FhirRegistryHttpTestFixture
{
    public PackageHttpMessageHandler _handler;
    public HttpClient _httpClient;
    public string _cacheDirectory;
    public Dictionary<PackageRegistryRecord.RegistryTypeCodes, PackageRegistryRecord> _registries;

    public FhirRegistryHttpTestFixture()
    {
        _handler = new PackageHttpMessageHandler();
        _httpClient = new HttpClient(_handler);
        _cacheDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestData", ".fhir");
        _registries = new()
        {
            {
                PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
                PackageRegistryRecord.FhirPrimaryRegistry
            },
            {
                PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
                PackageRegistryRecord.FhirSecondaryRegistry
            },
            {
                PackageRegistryRecord.RegistryTypeCodes.FhirCi,
                PackageRegistryRecord.FhirCiRegistry
            }
        };
    }
}
