using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Packages.Tests.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Shouldly;
using static Fhir.CodeGen.Packages.Tests.FullManifestExpectations;

namespace Fhir.CodeGen.Packages.Tests;

public class FullPackageManifestTests
{
    internal ILookup<string, FullManifestExpectationRecord> _fullManifestExpectationLookup;

    public FullPackageManifestTests()
    {
        _fullManifestExpectationLookup = _fullManifestExpectations
            .Where(e => !string.IsNullOrEmpty(e.LocalPackageJsonFilename))
            .ToLookup(e => e.LocalPackageJsonFilename!, StringComparer.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("package-info-backport-primary.json")]
    [InlineData("package-info-backport-primary-r4.json")]
    [InlineData("package-info-backport-primary-r4b.json")]
    [InlineData("package-info-backport-secondary.json")]
    [InlineData("package-info-backport-secondary-r4.json")]
    [InlineData("package-info-backport-secondary-r4b.json")]
    [InlineData("package-info-us-core-primary.json")]
    [InlineData("package-info-us-core-secondary.json")]
    public void ParseFullManifest(
        string packageFilename)
    {
        if (string.IsNullOrEmpty(packageFilename))
        {
            throw new ArgumentNullException(nameof(packageFilename));
        }

        if (!_fullManifestExpectationLookup.Contains(packageFilename))
        {
            throw new ArgumentException($"No expectations found for test file '{packageFilename}'.");
        }

        FullManifestExpectationRecord? expectations = _fullManifestExpectationLookup[packageFilename].First();

        string packageJson = LoadFileContents(packageFilename);

        Fhir.CodeGen.Packages.Models.FullPackageManifest? manifest = JsonSerializer.Deserialize<Fhir.CodeGen.Packages.Models.FullPackageManifest>(packageJson);

        manifest.ShouldNotBeNull();
        manifest.Name.ShouldNotBeNull();
        manifest.Name.ToLowerInvariant().ShouldBe(expectations.PackageName.ToLowerInvariant());
        if (expectations.HasDescription)
        {
            manifest.Description.ShouldNotBeNullOrWhiteSpace();
        }
        else
        {
            manifest.Description.ShouldBeNull();
        }

        if (expectations.DistTags != null && expectations.DistTags.Count > 0)
        {
            manifest.DistributionTags.ShouldNotBeNull();
            manifest.DistributionTags!.Count.ShouldBe(expectations.DistTags.Count);
            foreach (ExpectedDistTag expectedTag in expectations.DistTags)
            {
                manifest.DistributionTags.ShouldContainKey(expectedTag.Tag);
                manifest.DistributionTags![expectedTag.Tag].ShouldBe(expectedTag.Version);
            }
        }
        else
        {
            manifest.DistributionTags.ShouldBeNull();
        }

        List<string> testedVersions = [];

        manifest.Versions?.ShouldNotBeNull();
        manifest.Versions!.Count.ShouldBe(expectations.VersionCount);
        foreach ((string version, Models.PackageManifest versionManifest) in manifest.Versions)
        {
            version.ShouldNotBeNullOrWhiteSpace();
            versionManifest.ShouldNotBeNull();
            versionManifest.Version.ShouldBe(version);

            versionManifest.Name.ShouldNotBeNull();
            versionManifest.Name.ToLowerInvariant().ShouldBe(expectations.PackageName.ToLowerInvariant());
            versionManifest.Distribution.ShouldNotBeNull();
            versionManifest.Distribution!.TarballUrl.ShouldNotBeNullOrWhiteSpace();
            versionManifest.AnyFhirVersions.ShouldNotBeNull();
            versionManifest.AnyFhirVersions!.Count.ShouldBe(1);

            FullManifestVersionExpectationRecord? versionExpectations = expectations.VersionExpectations?
                .Where(ve => ve.Version == version)
                .FirstOrDefault();

            if (versionExpectations == null)
            {
                continue;
            }

            testedVersions.Add(versionExpectations.Version);

            versionManifest.Version.ShouldBe(versionExpectations.Version);
            versionManifest.Name!.ToLowerInvariant().ShouldBe(versionExpectations.Name.ToLowerInvariant());
            FhirReleases.FhirVersionToSequence(versionManifest.AnyFhirVersions!.First()).ShouldBe(versionExpectations.FhirSequence);
            versionManifest.Distribution.TarballUrl!.ShouldBe(versionExpectations.TarballUrl);

            if (versionExpectations.ShaSum == null)
            {
                versionManifest.Distribution.ShaSum.ShouldBeNull();
            }
            else
            {
                versionManifest.Distribution.ShaSum!.ToLowerInvariant().ShouldBe(versionExpectations.ShaSum.ToLowerInvariant());
            }

            if (versionExpectations.Url == null)
            {
                versionManifest.WebPublicationUrl.ShouldBeNull();
            }
            else
            {
                versionManifest.WebPublicationUrl!.ShouldBe(versionExpectations.Url);
            }

            if (versionExpectations.Canonical == null)
            {
                versionManifest.CanonicalUrl.ShouldBeNull();
            }
            else
            {
                versionManifest.CanonicalUrl!.ShouldBe(versionExpectations.Canonical);
            }
        }

        // if we have *any* version expectations, we should be testing all of them
        if (expectations.VersionExpectations != null)
        {
            manifest.Versions.Count().ShouldBe(expectations.VersionExpectations.Count());
            manifest.Versions.Count().ShouldBe(testedVersions.Count());
        }
    }

    [Fact]
    public void ParseSnippet01()
    {
        string json = """
            {
                "name": "hl7.fhir.uv.subscriptions-backport",
                "version": "0.1.0",
                "description": "None.",
                "dist": {
                    "tarball": "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0"
                },
                "fhirVersion": "R4",
                "url": "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0"
            }
            """;

        PackageManifest? parsed = JsonSerializer.Deserialize<PackageManifest>(json);

        parsed.ShouldNotBeNull();
        parsed.Name.ShouldBe("hl7.fhir.uv.subscriptions-backport");
        parsed.Version.ShouldBe("0.1.0");
        parsed.Description.ShouldBe("None.");
        parsed.Distribution.ShouldNotBeNull();
        parsed.Distribution.TarballUrl?.ShouldNotBeNullOrWhiteSpace();
        parsed.Distribution.TarballUrl.ShouldBe("https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0");

        IReadOnlyList<string>? fv = parsed.AnyFhirVersions;
        fv.ShouldNotBeNull();
        fv.Count.ShouldBe(1);
        fv.First().ShouldBe("R4");

        parsed.WebPublicationUrl.ShouldBe("https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0");
    }

    [Fact]
    public void ParseSnippet02()
    {
        string json = """
            {
                "tarball": "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0"
            }
            """;

        NpmManifestDistRecord? parsed = JsonSerializer.Deserialize<NpmManifestDistRecord>(json);

        parsed.ShouldNotBeNull();
        parsed.TarballUrl.ShouldNotBeNull();
    }

    private static string LoadFileContents(string filePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), Path.Combine("TestData", filePath));

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        return File.ReadAllText(path);
    }
}
