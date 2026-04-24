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
    [InlineData("hl7.fhir.uv.subscriptions-backport", new string[] { "master", "propose-changes" })]
    [InlineData("hl7.fhir.r6.core", new string[] { "master", "2025-09-gg-artifacts-menu" })]
    [InlineData("hl7.fhir.r6", new string[] { "master", "2025-09-gg-artifacts-menu" })]
    public void RegistryCatalogFind(
        string packageId,
        string[]? branchNames)
    {
        RegistryEndpointRecord registryRec = _fixture._registries[_FhirCiServer];

        IPackageRegistryClient client = IPackageRegistryClient.Create(registryRec, _fixture._httpClient);
        List<RegistryCatalogRecord>? results = client.Find(name: packageId);
        results.ShouldNotBeNull($"Expected non-null results for CI registry.");

        if (branchNames is null)
        {
            results.Count.ShouldBe(0, $"Expected no results for CI registry.");
            return;
        }

        results.Count.ShouldBeGreaterThanOrEqualTo(branchNames.Length, $"Expected at least {branchNames.Length} results for CI registry.");
        foreach (string branchName in branchNames)
        {
            results.Any(r => r.GitHubBranch == branchName).ShouldBeTrue($"Expected to find branch '{branchName}' in results for CI registry.");
        }
    }

    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", new string[] { "current", "current$propose-changes" })]
    [InlineData("hl7.fhir.r6.core", new string[] { "current", "current$2025-09-gg-artifacts-menu" })]
    [InlineData("hl7.fhir.r6", new string[] { "current", "current$2025-09-gg-artifacts-menu" })]
    public void RegistryGetFullManifest(
        string packageId,
        string[]? versions)
    {
        RegistryEndpointRecord registryRec = _fixture._registries[_FhirCiServer];
        IPackageRegistryClient client = IPackageRegistryClient.Create(registryRec, _fixture._httpClient);

        FullPackageManifest? manifest = client.GetFullManifest(packageId);

        if (versions is null)
        {
            manifest.ShouldBeNull();
            return;
        }

        manifest.ShouldNotBeNull();
        manifest.Versions.ShouldNotBeNull();
        manifest.Versions.Count.ShouldBeGreaterThanOrEqualTo(versions.Length, $"Expected at least {versions.Length} versions for CI registry.");

        foreach (string expectedVersion in versions)
        {
            manifest.Versions.TryGetValue(expectedVersion, out PackageManifest? versionManifest).ShouldBeTrue();
            versionManifest.Distribution.ShouldNotBeNull();
            versionManifest.Distribution.TarballUrl.ShouldNotBeNull();
        }
    }

    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport#current", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#current$propose-changes", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#current$not-a-branch", false)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#dev", false)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#1.1.0", false)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#latest", false)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#*", false)]
    [InlineData("hl7.fhir.r6.core#current", true)]
    [InlineData("hl7.fhir.r6.core#current$2025-09-gg-artifacts-menu", true)]
    [InlineData("hl7.fhir.r6.core#current$not-a-branch", false)]
    [InlineData("hl7.fhir.r6#current", true)]
    [InlineData("hl7.fhir.r6#current$2025-09-gg-artifacts-menu", true)]
    public void RegistryGetVersionManifest(
        string directive,
        bool exists)
    {
        RegistryEndpointRecord registryRec = _fixture._registries[_FhirCiServer];
        IPackageRegistryClient client = IPackageRegistryClient.Create(registryRec, _fixture._httpClient);

        PackageDirective pd = new(directive);

        if (exists)
        {
            pd.VersionType.ShouldBe(PackageDirective.DirectiveVersionCodes.CiBuild);
        }

        PackageManifest? manifest = client.Resolve(pd);
        if (!exists)
        {
            manifest.ShouldBeNull();
            return;
        }

        manifest.ShouldNotBeNull();
        manifest.Version.ShouldNotBeNullOrEmpty();
        manifest.Distribution.ShouldNotBeNull();
        manifest.Distribution.TarballUrl.ShouldNotBeNullOrEmpty();
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
                _FhirCiServer,
                RegistryEndpointRecord.FhirCiRegistry
            }
        };
    }
}
