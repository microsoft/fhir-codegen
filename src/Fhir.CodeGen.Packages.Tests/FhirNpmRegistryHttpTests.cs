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

public class FhirNpmRegistryHttpTests : IClassFixture<FhirNpmRegistryHttpTestFixture>
{
    private readonly FhirNpmRegistryHttpTestFixture _fixture;
    public FhirNpmRegistryHttpTests(FhirNpmRegistryHttpTestFixture fixture)
    {
        _fixture = fixture;
        _fullManifestExpectationLookup = _fullManifestExpectations.ToLookup(e => (e.PackageName, e.RegistryDiscriminator));
    }

    //private record class FullManifestExpectationRecord
    //{
    //    public required string Id { get; init; }
    //    public required bool ShouldBeNull { get; init; }
    //    public required int EntryCountPrimary { get; init; }
    //    public required int EntryCountSecondary { get; init; }
    //    public required int EntryCountCi { get; init; }
    //}

    private ILookup<(string PackageId, int? RegistryDiscriminator), FullManifestExpectationRecord> _fullManifestExpectationLookup;

    [Theory]
    [InlineData(_Simplifier, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData(_Packages2, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData(_Simplifier, "hl7.fhir.uv.subscriptions-backport.r4")]
    [InlineData(_Packages2, "hl7.fhir.uv.subscriptions-backport.r4")]
    [InlineData(_Simplifier, "hl7.fhir.uv.subscriptions-backport.r4b")]
    [InlineData(_Packages2, "hl7.fhir.uv.subscriptions-backport.r4b")]
    [InlineData(_Simplifier, "hl7.fhir.r4.core")]
    [InlineData(_Packages2, "hl7.fhir.r4.core")]
    [InlineData(_Simplifier, "hl7.fhir.r4")]
    [InlineData(_Packages2, "hl7.fhir.r4")]
    [InlineData(_Simplifier, "hl7.fhir.us.core")]
    [InlineData(_Packages2, "hl7.fhir.us.core")]
    public void RegistryCatalogFind(
        int registryDiscriminator,
        string packageId)
    {
        RegistryEndpointRecord registryRec = _fixture._registries[registryDiscriminator];

        if (!_fullManifestExpectationLookup.Contains((packageId, registryDiscriminator)))
        {
            throw new ArgumentOutOfRangeException(nameof(packageId), $"No expectations found for package ID '{packageId}' and registry: {registryDiscriminator}.");
        }

        FullManifestExpectationRecord? expectations = _fullManifestExpectationLookup[(packageId, registryDiscriminator)].FirstOrDefault();
        expectations.ShouldNotBeNull($"Missing expectations for package ID '{packageId}'.");

        IPackageRegistryClient client = IPackageRegistryClient.Create(registryRec, _fixture._httpClient);

        List<RegistryCatalogRecord>? results = client.Find(name: packageId);
        results.ShouldNotBeNull($"Expected non-null results for registry {registryDiscriminator}.");

        if (expectations.CatalogCount is not null)
        {
            results.Count.ShouldBe(
                expectations.CatalogCount.Value,
                $"Expected {expectations.CatalogCount} catalog entries for package '{packageId}' in registry {registryDiscriminator}, but found {results.Count}.");
        }
    }

    //public void RegistryGetFullManifest()
    //{

    //}

}

public class FhirNpmRegistryHttpTestFixture
{
    public PackageHttpMessageHandler _handler;
    public HttpClient _httpClient;
    public string _cacheDirectory;
    public Dictionary<int, RegistryEndpointRecord> _registries;

    public FhirNpmRegistryHttpTestFixture()
    {
        _handler = new PackageHttpMessageHandler();
        _httpClient = new HttpClient(_handler);
        _cacheDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestData", ".fhir");
        _registries = new()
        {
            {
                _Simplifier,
                RegistryEndpointRecord.FhirPrimaryRegistry
            },
            {
                _Packages2,
                RegistryEndpointRecord.FhirSecondaryRegistry
            }
        };
    }
}
