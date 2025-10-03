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

namespace Fhir.CodeGen.Packages.Tests;

public class FullPackageManifestTests
{
    private record class ExpectedDistTag(string Tag, string Version);

    private record class FullManifestExpectationRecord
    {
        public required string PackageName { get; init; }
        public required bool HasDescription { get; init; }
        public required int VersionCount { get; init; }
        public List<ExpectedDistTag>? DistTags { get; init; } = null;
        public List<FullManifestVersionExpectationRecord>? VersionExpectations { get; init; } = null;
    }

    private record class FullManifestVersionExpectationRecord
    {
        public required string Version { get; init; }
        public required string Name { get; init; }
        public required FhirReleases.FhirSequenceCodes FhirSequence { get; init; }
        public required string TarballUrl { get; init; }
        public string? ShaSum { get; init; } = null;
        public string? Url { get; init; } = null;
        public string? Canonical { get; init; } = null;
    }

    private static readonly Dictionary<string, FullManifestExpectationRecord> _fullManifestExpectations = new()
    {
        {
            "package-info-backport-primary.json",
            new()
            {
                PackageName = "hl7.fhir.uv.subscriptions-backport",
                HasDescription = true,
                VersionCount = 3,
                DistTags = [ new("latest", "1.1.0") ],
                VersionExpectations = [
                    new()
                    {
                        Version = "0.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                        TarballUrl = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0",
                        Url = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/0.1.0",
                    },
                    new()
                    {
                        Version = "1.0.0",
                        Name = "hl7.fhir.uv.subscriptions-backport",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                        ShaSum = "b9fa88a658cf07ecb8c11bf916ffbf6edcf354bc",
                        TarballUrl = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/1.0.0",
                        Url = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/1.0.0",
                    },
                    new()
                    {
                        Version = "1.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                        ShaSum = "92fcbee17a069d25fb1a34e85c40cd5aff975646",
                        TarballUrl = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/1.1.0",
                        Url = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport/1.1.0",
                    },
                ],
            }
        },
        {
            "package-info-backport-primary-r4.json",
            new()
            {
                PackageName = "hl7.fhir.uv.subscriptions-backport.r4",
                HasDescription = true,
                VersionCount = 1,
                DistTags = [ new("latest", "1.1.0") ],
                VersionExpectations = [
                    new()
                    {
                        Version = "1.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport.r4",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                        ShaSum = "ca5e4f4c87857597931eefa9e20d6f0b6de6e44a",
                        TarballUrl = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport.r4/1.1.0",
                        Url = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport.r4/1.1.0",
                    },

                ],
            }
        },
        {
            "package-info-backport-primary-r4b.json",
            new()
            {
                PackageName = "hl7.fhir.uv.subscriptions-backport.r4b",
                HasDescription = true,
                VersionCount = 1,
                DistTags = [ new("latest", "1.1.0") ],
                VersionExpectations = [
                    new()
                    {
                        Version = "1.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport.r4b",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                        ShaSum = "f6bb023e4fa00837234887b8f5942f9a85298e8e",
                        TarballUrl = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport.r4b/1.1.0",
                        Url = "https://packages.simplifier.net/hl7.fhir.uv.subscriptions-backport.r4b/1.1.0",
                    },
                ],
            }
        },
        {
            "package-info-backport-secondary.json",
            new()
            {
                PackageName = "hl7.fhir.uv.subscriptions-backport",
                HasDescription = true,
                VersionCount = 3,
                DistTags = [ new("latest", "1.1.0") ],
                VersionExpectations = [
                    new()
                    {
                        Version = "0.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                        ShaSum = "48C17FB592F5C502EDBEA2E53C9FC9F325506B35",
                        TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport/0.1.0",
                        Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport/0.1.0",
                        Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                    },
                    new()
                    {
                        Version = "1.0.0",
                        Name = "hl7.fhir.uv.subscriptions-backport",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                        ShaSum = "B9FA88A658CF07ECB8C11BF916FFBF6EDCF354BC",
                        TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport/1.0.0",
                        Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport/1.0.0",
                        Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                    },
                    new()
                    {
                        Version = "1.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                        ShaSum = "92FCBEE17A069D25FB1A34E85C40CD5AFF975646",
                        TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport/1.1.0",
                        Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport/1.1.0",
                        Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                    },
                ],
            }
        },
        {
            "package-info-backport-secondary-r4.json",
            new()
            {
                PackageName = "hl7.fhir.uv.subscriptions-backport.R4",
                HasDescription = true,
                VersionCount = 1,
                DistTags = [ new("latest", "1.1.0") ],
                VersionExpectations = [
                    new()
                    {
                        Version = "1.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport.R4",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                        ShaSum = "CA5E4F4C87857597931EEFA9E20D6F0B6DE6E44A",
                        TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport.R4/1.1.0",
                        Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.R4/1.1.0",
                        Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                    },
                ],
            }
        },
        {
            "package-info-backport-secondary-r4b.json",
            new()
            {
                PackageName = "hl7.fhir.uv.subscriptions-backport.R4b",
                HasDescription = true,
                VersionCount = 1,
                DistTags = [ new("latest", "1.1.0") ],
                VersionExpectations = [
                    new()
                    {
                        Version = "1.1.0",
                        Name = "hl7.fhir.uv.subscriptions-backport.R4b",
                        FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                        ShaSum = "F6BB023E4FA00837234887B8F5942F9A85298E8E",
                        TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport.R4b/1.1.0",
                        Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.R4b/1.1.0",
                        Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                    },
                ],
            }
        },
        {
            "package-info-us-core-primary.json",
            new()
            {
                PackageName = "hl7.fhir.us.core",
                HasDescription = true,
                VersionCount = 19,
                DistTags = [ new("latest", "6.1.0") ],
            }
        },
        {
            "package-info-us-core-secondary.json",
            new()
            {
                PackageName = "hl7.fhir.us.core",
                HasDescription = true,
                VersionCount = 19,
                DistTags = [ new("latest", "6.1.0") ],
            }
        },
    };

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

        if (!_fullManifestExpectations.TryGetValue(packageFilename, out FullManifestExpectationRecord? expectations))
        {
            throw new ArgumentException($"No expectations found for test file '{packageFilename}'.");
        }

        string packageJson = LoadFileContents(packageFilename);

        Fhir.CodeGen.Packages.Models.FullPackageManifest? manifest = JsonSerializer.Deserialize<Fhir.CodeGen.Packages.Models.FullPackageManifest>(packageJson);

        manifest.ShouldNotBeNull();
        manifest.Name.ShouldBe(expectations.PackageName);
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

            versionManifest.Name.ShouldBe(expectations.PackageName);
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
            versionManifest.Name!.ShouldBe(versionExpectations.Name);
            FhirReleases.FhirVersionToSequence(versionManifest.AnyFhirVersions!.First()).ShouldBe(versionExpectations.FhirSequence);
            versionManifest.Distribution.TarballUrl!.ShouldBe(versionExpectations.TarballUrl);

            if (versionExpectations.ShaSum == null)
            {
                versionManifest.Distribution.ShaSum.ShouldBeNull();
            }
            else
            {
                versionManifest.Distribution.ShaSum!.ShouldBe(versionExpectations.ShaSum);
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
