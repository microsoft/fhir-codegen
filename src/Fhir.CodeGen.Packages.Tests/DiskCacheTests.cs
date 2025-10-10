using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.CacheClients;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Common.Packaging;
using Shouldly;
using static Fhir.CodeGen.Packages.Tests.FullManifestExpectations;

namespace Fhir.CodeGen.Packages.Tests;

public class DiskCacheTests : IClassFixture<DiskCacheTestFixture>
{
    private readonly DiskCacheTestFixture _fixture;

    public DiskCacheTests(DiskCacheTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task IndexedTestPackages()
    {
        List<CachedPackageRecord> packages = await _fixture._cache.ListCachedPackages();
        packages.Count.ShouldBe(11);

        foreach (CachedPackageRecord p in packages)
        {
            // act depending on cache directive
            switch (p.Directive.FhirCacheDirective)
            {
                case "example.org.ig#notsemver":
                    {
                        p.Directive.VersionType.ShouldBe(PackageDirective.DirectiveVersionCodes.NonSemVer);
                        p.Directive.RequestedVersion.ShouldBe("notsemver");
                        p.Directive.ResolvedVersion.ShouldNotBeNull();
                        p.Directive.ResolvedVersion.IsValid.ShouldBe(false);
                        p.Directive.ResolvedVersion.SourceString.ShouldBe("notsemver");

                        p.Directive.NameType.ShouldBe(PackageDirective.DirectiveNameTypeCodes.GuideWithoutSuffix);
                        p.Directive.PackageId.ShouldBe("example.org.ig");

                        p.FileIndex.ShouldBeNull();

                        p.Manifest.ShouldNotBeNull();
                        IReadOnlyList<string>? packageFhirVersions = p.Manifest.AnyFhirVersions;
                        packageFhirVersions.ShouldNotBeNull();
                        packageFhirVersions.Count.ShouldBe(1);
                        FhirReleases.FhirVersionToSequence(packageFhirVersions[0]).ShouldBe(FhirReleases.FhirSequenceCodes.R4);
                        p.Manifest.Authors.ShouldNotBeNull();
                        p.Manifest.Authors.Count.ShouldBe(1);
                        p.Manifest.Directories.ShouldNotBeNull();
                        p.Manifest.Directories.Count.ShouldBe(1);
                        p.Manifest.PublicationDate.ShouldBeEquivalentTo(new DateTime(2023, 09, 16, 03, 55, 59));
                        p.Manifest.CanonicalUrl.ShouldBe("http://example.org/fhir/ig");
                    }
                    break;

                case "hl7.fhir.r4.core#4.0.1":
                    {
                        p.Directive.VersionType.ShouldBe(PackageDirective.DirectiveVersionCodes.Exact);
                        p.Directive.RequestedVersion.ShouldBe("4.0.1");
                        p.Directive.ResolvedVersion.ShouldNotBeNull();
                        p.Directive.ResolvedVersion.IsValid.ShouldBe(true);
                        p.Directive.ResolvedVersion.SourceString.ShouldBe("4.0.1");
                        p.Directive.ResolvedVersion.Major.ShouldBe(4);
                        p.Directive.ResolvedVersion.Minor.ShouldBe(0);
                        p.Directive.ResolvedVersion.Patch.ShouldBe(1);
                        p.Directive.ResolvedVersion.PreRelease.ShouldBeNull();
                        p.Directive.ResolvedVersion.BuildMetadata.ShouldBeNull();

                        p.Directive.NameType.ShouldBe(PackageDirective.DirectiveNameTypeCodes.CoreFull);
                        p.Directive.PackageId.ShouldBe("hl7.fhir.r4.core");

                        p.FileIndex.ShouldNotBeNull();
                        p.FileIndex.Files.ShouldNotBeNull();
                        p.FileIndex.Files.Count.ShouldBe(4581);

                        p.Manifest.ShouldNotBeNull();
                        IReadOnlyList<string>? packageFhirVersions = p.Manifest.AnyFhirVersions;
                        packageFhirVersions.ShouldNotBeNull();
                        packageFhirVersions.Count.ShouldBe(1);
                        FhirReleases.FhirVersionToSequence(packageFhirVersions[0]).ShouldBe(FhirReleases.FhirSequenceCodes.R4);
                        p.Manifest.Authors.ShouldNotBeNull();
                        p.Manifest.Authors.Count.ShouldBe(1);
                        p.Manifest.Directories.ShouldBeNull();
                        p.Manifest.PublicationDate.ShouldBeNull();
                        p.Manifest.CanonicalUrl.ShouldBe("http://hl7.org/fhir");
                    }
                    break;

                case "hl7.fhir.r6.core#current":
                    {
                        p.Directive.VersionType.ShouldBe(PackageDirective.DirectiveVersionCodes.CiBuild);
                        p.Directive.RequestedVersion.ShouldBe("current");
                        p.Directive.ResolvedVersion.ShouldNotBeNull();
                        p.Directive.ResolvedVersion.IsValid.ShouldBe(true);
                        p.Directive.ResolvedVersion.SourceString.ShouldBe("6.0.0-ballot3");
                        p.Directive.ResolvedVersion.Major.ShouldBe(6);
                        p.Directive.ResolvedVersion.Minor.ShouldBe(0);
                        p.Directive.ResolvedVersion.Patch.ShouldBe(0);
                        p.Directive.ResolvedVersion.PreRelease.ShouldBe("ballot3");
                        p.Directive.ResolvedVersion.BuildMetadata.ShouldBeNull();
                        p.Directive.RequestedCiBranch.ShouldBeNull();

                        p.Directive.NameType.ShouldBe(PackageDirective.DirectiveNameTypeCodes.CoreFull);
                        p.Directive.PackageId.ShouldBe("hl7.fhir.r6.core");

                        p.FileIndex.ShouldNotBeNull();
                        p.FileIndex.Files.ShouldNotBeNull();
                        p.FileIndex.Files.Count.ShouldBe(3068);

                        p.Manifest.ShouldNotBeNull();
                        IReadOnlyList<string>? packageFhirVersions = p.Manifest.AnyFhirVersions;
                        packageFhirVersions.ShouldNotBeNull();
                        packageFhirVersions.Count.ShouldBe(1);
                        FhirReleases.FhirVersionToSequence(packageFhirVersions[0]).ShouldBe(FhirReleases.FhirSequenceCodes.R6);
                        p.Manifest.Authors.ShouldNotBeNull();
                        p.Manifest.Authors.Count.ShouldBe(1);
                        p.Manifest.Directories.ShouldNotBeNull();
                        p.Manifest.Directories.Count.ShouldBe(2);
                        p.Manifest.PublicationDate.ShouldBe(new DateTime(2025, 09, 26, 21, 39, 16));
                        p.Manifest.CanonicalUrl.ShouldBe("http://hl7.org/fhir");
                    }
                    break;

                case "hl7.fhir.r6.core#current$branch":
                    {
                        p.Directive.VersionType.ShouldBe(PackageDirective.DirectiveVersionCodes.CiBuild);
                        p.Directive.RequestedVersion.ShouldBe("current$branch");
                        p.Directive.ResolvedVersion.ShouldNotBeNull();
                        p.Directive.ResolvedVersion.IsValid.ShouldBe(true);
                        p.Directive.ResolvedVersion.SourceString.ShouldBe("6.0.0-ballot3");
                        p.Directive.ResolvedVersion.Major.ShouldBe(6);
                        p.Directive.ResolvedVersion.Minor.ShouldBe(0);
                        p.Directive.ResolvedVersion.Patch.ShouldBe(0);
                        p.Directive.ResolvedVersion.PreRelease.ShouldBe("ballot3");
                        p.Directive.ResolvedVersion.BuildMetadata.ShouldBeNull();
                        p.Directive.RequestedCiBranch.ShouldBe("branch");

                        p.Directive.NameType.ShouldBe(PackageDirective.DirectiveNameTypeCodes.CoreFull);
                        p.Directive.PackageId.ShouldBe("hl7.fhir.r6.core");

                        p.FileIndex.ShouldNotBeNull();
                        p.FileIndex.Files.ShouldNotBeNull();
                        p.FileIndex.Files.Count.ShouldBe(3068);

                        p.Manifest.ShouldNotBeNull();
                        IReadOnlyList<string>? packageFhirVersions = p.Manifest.AnyFhirVersions;
                        packageFhirVersions.ShouldNotBeNull();
                        packageFhirVersions.Count.ShouldBe(1);
                        FhirReleases.FhirVersionToSequence(packageFhirVersions[0]).ShouldBe(FhirReleases.FhirSequenceCodes.R6);
                        p.Manifest.Authors.ShouldNotBeNull();
                        p.Manifest.Authors.Count.ShouldBe(1);
                        p.Manifest.Directories.ShouldNotBeNull();
                        p.Manifest.Directories.Count.ShouldBe(2);
                        p.Manifest.PublicationDate.ShouldBe(new DateTime(2025, 09, 26, 21, 39, 16));
                        p.Manifest.CanonicalUrl.ShouldBe("http://hl7.org/fhir");
                    }
                    break;
            }
        }
    }
}

public class DiskCacheTestFixture : IDisposable
{
    public PackageHttpMessageHandler _handler;
    public HttpClient _httpClient;
    public string _cacheDirectory;
    public Dictionary<int, RegistryEndpointRecord> _registries;
    public DiskCacheClient _cache;

    public DiskCacheTestFixture()
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
            },
            //{
            //    _FhirCiServer,
            //    RegistryEndpointRecord.FhirCiRegistry
            //},
        };
        _cache = new(
            _cacheDirectory,
            registryEndpoints: _registries.Values.ToList(),
            httpClient: _httpClient);
    }

    public void Dispose()
    {
    }
}
