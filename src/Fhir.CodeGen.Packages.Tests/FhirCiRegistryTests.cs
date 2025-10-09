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

public class FhirCiRegistryTests : IClassFixture<FhirCiRegistryTestFixture>
{
    private readonly FhirCiRegistryTestFixture _fixture;
    public FhirCiRegistryTests(FhirCiRegistryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(_CiServer, "hl7.fhir.uv.subscriptions-backport", new string[] { "master", "propose-changes" })]
    public void RegistryCatalogFind(
        int registryDiscriminator,
        string packageId,
        string[]? branchNames)
    {
        RegistryEndpointRecord registryRec = _fixture._registries[registryDiscriminator];

        IRegistryClient client = IRegistryClient.Create(registryRec, _fixture._httpClient);
        List<RegistryCatalogRecord>? results = client.Find(name: packageId);
        results.ShouldNotBeNull($"Expected non-null results for registry {registryDiscriminator}.");

        if (branchNames is null)
        {
            results.Count.ShouldBe(0, $"Expected no results for registry {registryDiscriminator}.");
            return;
        }

        results.Count.ShouldBe(branchNames.Length, $"Expected {branchNames.Length} results for registry {registryDiscriminator}.");
        foreach (string branchName in branchNames)
        {
            results.Any(r => r.GitHubBranch == branchName).ShouldBeTrue($"Expected to find branch '{branchName}' in results for registry {registryDiscriminator}.");
        }
    }

    [Theory]
    [InlineData(_CiServer, "hl7.fhir.uv.subscriptions-backport", new string[] { "current", "current$propose-changes" })]
    public void RegistryGetFullManifest(
        int registryDiscriminator,
        string packageId,
        string[]? versions)
    {
        RegistryEndpointRecord registryRec = _fixture._registries[registryDiscriminator];

        IRegistryClient client = IRegistryClient.Create(registryRec, _fixture._httpClient);

        FullPackageManifest? manifest = client.GetFullManifest(packageId);

        if (versions is null)
        {
            manifest.ShouldBeNull();
            return;
        }

        manifest.ShouldNotBeNull();
        manifest.Versions.ShouldNotBeNull();
        manifest.Versions.Count.ShouldBe(versions.Length);

        foreach (string expectedVersion in versions)
        {
            manifest.Versions.TryGetValue(expectedVersion, out PackageManifest? versionManifest).ShouldBeTrue();
            versionManifest.Distribution.ShouldNotBeNull();
            versionManifest.Distribution.TarballUrl.ShouldNotBeNull();
        }
    }
}

public class FhirCiRegistryTestFixture
{
    public PackageHttpMessageHandler _handler;
    public HttpClient _httpClient;
    public string _cacheDirectory;
    public Dictionary<int, RegistryEndpointRecord> _registries;

    public FhirCiRegistryTestFixture()
    {
        _handler = new PackageHttpMessageHandler();
        _httpClient = new HttpClient(_handler);
        _cacheDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestData", ".fhir");
        _registries = new()
        {
            {
                _CiServer,
                RegistryEndpointRecord.FhirCiRegistry
            }
        };
    }
}
