using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Common.Packaging;
using Shouldly;

namespace Fhir.CodeGen.Packages.Tests;

public class PackageManifestTests
{
    internal record class PackageManifestExpectationRecord
    {
        public required string Name { get; init; }
        public required string Version { get; init; }
        public string? OriginalVersion { get; init; } = null;
        public required FhirReleases.FhirSequenceCodes? FhirSequence { get; init; }
        public required string? WebPublicationUrl { get; init; }
        public DateTime? PackageDate { get; init; } = null;
        public string? Canonical { get; init; } = null;
        public string? AuthorName { get; init; } = null;
        public string? AuthorEmail { get; init; } = null;
        public string? AuthorUrl { get; init; } = null;
        public int? DependencyCount { get; init; } = null;
        public List<(string id, string version)>? Depdencies { get; init; } = null;
        public int? MaintainerCount {  get; init; } = null;
        public List<(string? name, string? email, string? url)>? Maintainers { get; init; } = null;
        public bool? HasTitle { get; init; } = null;
        public bool? HasDescription { get; init; } = null;
    }

    internal static readonly Dictionary<(string id, string version), PackageManifestExpectationRecord> _packageJsonExpectations = new()
    {
        {
            ("example.org.ig", "notsemver"),
            new()
            {
                Name = "example.org.ig",
                Version = "notsemver",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "https://example.org/ci/ig/",
                PackageDate = new DateTime(2023, 09, 16, 03, 55, 59),
                Canonical = "http://example.org/fhir/ig",
                AuthorName = "Example.org",
                HasTitle = true,
                HasDescription = true,
            }
        },
        {
            ("hl7.fhir.r4.core", "4.0.1"),
            new()
            {
                Name = "hl7.fhir.r4.core",
                Version = "4.0.1",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/R4",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                //PackageDate = new DateTime(2019, 10, 30, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4.corexml", "4.0.1"),
            new()
            {
                Name = "hl7.fhir.r4.corexml",
                Version = "4.0.1",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/R4",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                // do not include a package date here - it is an old-style package.json that does not have it
            }
        },
        {
            ("hl7.fhir.r4.elements", "4.0.1"),
            new()
            {
                Name = "hl7.fhir.r4.elements",
                Version = "4.0.1",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/R4",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2019, 10, 30, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4.examples", "4.0.1"),
            new()
            {
                Name = "hl7.fhir.r4.examples",
                Version = "4.0.1",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/R4",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2019, 10, 30, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4.expansions", "4.0.1"),
            new()
            {
                Name = "hl7.fhir.r4.expansions",
                Version = "4.0.1",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/R4",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2019, 10, 30, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4.core", "current"),
            new()
            {
                Name = "hl7.fhir.r4.core",
                Version = "4.0.1",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/R4",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
            }
        },

        {
            ("hl7.fhir.r4b.core", "4.3.0"),
            new()
            {
                Name = "hl7.fhir.r4b.core",
                Version = "4.3.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/R4B",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2022, 05, 28, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4b.expansions", "4.3.0"),
            new()
            {
                Name = "hl7.fhir.r4b.expansions",
                Version = "4.3.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/R4B",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2022, 05, 28, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4b.corexml", "4.3.0"),
            new()
            {
                Name = "hl7.fhir.r4b.corexml",
                Version = "4.3.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/R4B",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2022, 05, 28, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4b.elements", "4.3.0"),
            new()
            {
                Name = "hl7.fhir.r4b.elements",
                Version = "4.3.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/R4B",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2022, 05, 28, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r4b.examples", "4.3.0"),
            new()
            {
                Name = "hl7.fhir.r4b.examples",
                Version = "4.3.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/R4B",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
                PackageDate = new DateTime(2022, 05, 28, 12, 00, 00),
            }
        },
        {
            ("hl7.fhir.r6.core", "current"),
            new()
            {
                Name = "hl7.fhir.r6.core",
                Version = "6.0.0-cibuild",
                FhirSequence = FhirReleases.FhirSequenceCodes.R6,
                WebPublicationUrl = "http://hl7.org/fhir/R6",
                Canonical = "http://hl7.org/fhir",
                AuthorName = "HL7 Inc",
                HasTitle = true,
                HasDescription = true,
            }
        },
        {
            ("hl7.fhir.us.core", "6.1.0"),
            new()
            {
                Name = "hl7.fhir.us.core",
                Version = "6.1.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/us/core/STU6.1",
                Canonical = "http://hl7.org/fhir/us/core",
                HasTitle = true,
                HasDescription = true,
            }
        },
        {
            ("hl7.fhir.uv.patient-corrections", "dev"),
            new()
            {
                Name = "hl7.fhir.uv.patient-corrections",
                Version = "dev",
                OriginalVersion = "1.0.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "file://C:\\specs\\fhir-patient-correction\\output",
                PackageDate = new DateTime(2023, 08, 21, 14, 17, 44),
                Canonical = "http://hl7.org/fhir/uv/patient-corrections",
                AuthorName = "HL7 International - Patient Empowerment Workgroup",
                Maintainers = [("Virginia Lorenzi", "vlorenzi@nyp.org", null)],
                HasTitle = true,
                HasDescription = true,
            }
        },
        {
            ("hl7.fhir.uv.subscriptions-backport", "1.1.0"),
            new()
            {
                Name = "hl7.fhir.uv.subscriptions-backport",
                Version = "1.1.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/uv/subscriptions-backport/1.1.0",
                PackageDate = new DateTime(2023, 01, 11, 03, 34, 12),
                Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                AuthorName = "HL7 FHIR Infrastructure WG",
                Depdencies = [
                    ("hl7.fhir.r4.core", "4.0.0"),
                    ("hl7.terminology.r4", "5.0.0"),
                    ],
                Maintainers = [
                    ("HL7 FHIR Infrastructure WG", null, "https://hl7.org/Special/committees/fiwg"),
                    ("Gino Canessa", "mailto:gino.canessa@microsoft.com", null),
                    ("Eric Haas", "mailto:ehaas@healthedatainc.com", null),
                    ],
                HasTitle = true,
                HasDescription = true,
            }
        },
        {
            ("hl7.fhir.uv.subscriptions-backport.r4", "1.1.0"),
            new()
            {
                Name = "hl7.fhir.uv.subscriptions-backport.r4",
                Version = "1.1.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = "http://hl7.org/fhir/uv/subscriptions-backport/STU1.1",
                PackageDate = new DateTime(2023, 01, 11, 15, 34, 12),
                Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                AuthorName = "HL7 FHIR Infrastructure WG",
                Depdencies = [
                    ("hl7.fhir.r4.core", "4.0.0"),
                    ("hl7.terminology.r4", "5.0.0"),
                    ],
                Maintainers = [
                    ("HL7 FHIR Infrastructure WG", null, "https://hl7.org/Special/committees/fiwg"),
                    ("Gino Canessa", "mailto:gino.canessa@microsoft.com", null),
                    ("Eric Haas", "mailto:ehaas@healthedatainc.com", null),
                    ],
                HasTitle = true,
                HasDescription = true,
            }
        },
        {
            ("hl7.fhir.uv.subscriptions-backport.r4b", "1.1.0"),
            new()
            {
                Name = "hl7.fhir.uv.subscriptions-backport.r4b",
                Version = "1.1.0",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = "http://hl7.org/fhir/uv/subscriptions-backport/STU1.1",
                PackageDate = new DateTime(2023, 01, 11, 15, 34, 12),
                Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                AuthorName = "HL7 FHIR Infrastructure WG",
                Depdencies = [
                    ("hl7.fhir.r4b.core", "4.3.0"),
                    ("hl7.terminology.r4", "5.0.0"),
                    ],
                Maintainers = [
                    ("HL7 FHIR Infrastructure WG", null, "https://hl7.org/Special/committees/fiwg"),
                    ("Gino Canessa", "mailto:gino.canessa@microsoft.com", null),
                    ("Eric Haas", "mailto:ehaas@healthedatainc.com", null),
                    ],
                HasTitle = true,
                HasDescription = true,
            }
        },
    };

    private static readonly Dictionary<(string id, string version), PackageManifestExpectationRecord> _shortManifestExpectations = new()
    { 
        {
            ("hl7.fhir.uv.subscriptions-backport", "1.1.0"),
            new()
            {
                Name = "hl7.fhir.uv.subscriptions-backport",
                Version = "1.1.0",
                PackageDate = new DateTime(2022, 12, 02, 14, 28, 32),
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                WebPublicationUrl = null,
            }
        },
        {
            ("hl7.fhir.uv.subscriptions-backport.r4", "1.1.0"),
            new()
            {
                Name = "hl7.fhir.uv.subscriptions-backport.r4",
                Version = "1.1.0",
                PackageDate = new DateTime(2023, 01, 09, 22, 27, 50),
                FhirSequence = null,
                WebPublicationUrl = null,
            }
        },
        {
            ("hl7.fhir.uv.subscriptions-backport.r4b", "1.2.0-cibuild"),
            new()
            {
                Name = "hl7.fhir.uv.subscriptions-backport.r4b",
                Version = "1.2.0-cibuild",
                PackageDate = new DateTime(2023, 11, 30, 21, 43, 49),
                FhirSequence = null,
                WebPublicationUrl = null,
            }
        },
        {
            ("ihe.iti.pdqm", "2.4.0"),
            new()
            {
                Name = "ihe.iti.pdqm",
                Version = "2.4.0",
                PackageDate = new DateTime(2022, 02, 28, 18, 59, 57),
                FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                WebPublicationUrl = null,
            }
        },
    };

    private void testManifestExpectations(PackageManifest manifest, PackageManifestExpectationRecord expectations)
    {
        manifest.Name.ShouldBe(expectations.Name);
        manifest.Version.ShouldBe(expectations.Version);
        manifest.WebPublicationUrl.ShouldBe(expectations.WebPublicationUrl);

        if (expectations.FhirSequence != null)
        {
            manifest.AnyFhirVersions.ShouldNotBeNull();
            manifest.AnyFhirVersions!.Count.ShouldBe(1);
            FhirReleases.FhirVersionToSequence(manifest.AnyFhirVersions!.First()).ShouldBe(expectations.FhirSequence.Value);
        }
        else
        {
            manifest.FhirVersion.ShouldBeNull();
            manifest.FhirVersions.ShouldBeNull();
            manifest.FhirVersionList.ShouldBeNull();
        }

        if (expectations.OriginalVersion != null)
        {
            manifest.OriginalVersion.ShouldBe(expectations.OriginalVersion);
        }

        if (expectations.PackageDate != null)
        {
            manifest.PublicationDate.ShouldNotBeNull();
            manifest.PublicationDate.ShouldBeEquivalentTo(expectations.PackageDate);
        }

        if (expectations.Canonical != null)
        {
            manifest.CanonicalUrl.ShouldBe(expectations.Canonical);
        }

        int authorCount = manifest.Authors?.Count() ?? 0;
        NpmPersonRecord? firstAuthor = manifest.Authors?.FirstOrDefault();

        if (expectations.AuthorName != null)
        {
            firstAuthor.ShouldNotBeNull();
            firstAuthor.Name.ShouldBe(expectations.AuthorName);
        }

        if (expectations.AuthorEmail != null)
        {
            firstAuthor.ShouldNotBeNull();
            firstAuthor.Email.ShouldBe(expectations.AuthorEmail);
        }

        if (expectations.AuthorUrl != null)
        {
            firstAuthor.ShouldNotBeNull();
            firstAuthor.Url.ShouldNotBeNull();
        }

        if (expectations.DependencyCount != null)
        {
            manifest.Dependencies.ShouldNotBeNull();
            manifest.Dependencies!.Count().ShouldBe(expectations.DependencyCount.Value);
        }

        if (expectations.Depdencies != null)
        {
            manifest.Dependencies.ShouldNotBeNull();

            foreach ((string expectId, string expectVersion) in expectations.Depdencies)
            {
                manifest.Dependencies!.TryGetValue(expectId, out string? manifestDepVersion).ShouldBeTrue();
                manifestDepVersion.ShouldNotBeNull();
                manifestDepVersion.ShouldBe(expectVersion);
            }
        }

        if (expectations.MaintainerCount != null)
        {
            manifest.Maintainers.ShouldNotBeNull();
            manifest.Maintainers.Count().ShouldBe(expectations.MaintainerCount.Value);
        }

        if (expectations.Maintainers != null)
        {
            manifest.Maintainers.ShouldNotBeNull();

            foreach ((string? expectName, string? expectEmail, string? expectUrl) in expectations.Maintainers)
            {
                NpmPersonRecord? maintainer = manifest.Maintainers.Where(m =>
                    ((expectName == null) || m.Name.Equals(expectName, StringComparison.Ordinal)) &&
                    ((expectEmail == null) || (m.Email?.Equals(expectEmail, StringComparison.Ordinal) ?? false)) &&
                    ((expectUrl == null) || (m.Url?.Equals(expectUrl, StringComparison.Ordinal) ?? false)))
                    .FirstOrDefault();

                maintainer.ShouldNotBeNull();
            }
        }

        if (expectations.HasTitle == true)
        {
            manifest.Title.ShouldNotBeNullOrEmpty();
        }
        else if (expectations.HasTitle == false)
        {
            manifest.Title.ShouldBeNullOrEmpty();
        }

        if (expectations.HasDescription == true)
        {
            manifest.Description.ShouldNotBeNullOrEmpty();
        }
        else if (expectations.HasDescription == false)
        {
            manifest.Description.ShouldBeNullOrEmpty();
        }
    }

    private void testShortManifestExpectations(PackageManifest manifest, PackageManifestExpectationRecord expectations)
    {
        manifest.Name.ShouldBe(expectations.Name);
        manifest.Version.ShouldBe(expectations.Version);

        if (expectations.FhirSequence != null)
        {
            manifest.AnyFhirVersions.ShouldNotBeNull();
            manifest.AnyFhirVersions!.Count.ShouldBe(1);
            FhirReleases.FhirVersionToSequence(manifest.AnyFhirVersions!.First()).ShouldBe(expectations.FhirSequence.Value);
        }
        else
        {
            manifest.FhirVersion.ShouldBeNull();
            manifest.FhirVersions.ShouldBeNull();
            manifest.FhirVersionList.ShouldBeNull();
        }

        if (expectations.PackageDate != null)
        {
            manifest.PublicationDate.ShouldNotBeNull();
            manifest.PublicationDate.ShouldBeEquivalentTo(expectations.PackageDate);
        }
    }


    [Theory]
    [InlineData("example.org.ig", "notsemver")]
    [InlineData("hl7.fhir.r4.core", "4.0.1")]
    [InlineData("hl7.fhir.r4.core", "current")]
    [InlineData("hl7.fhir.r6.core", "current")]
    [InlineData("hl7.fhir.uv.patient-corrections", "dev")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", "1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", "1.1.0")]
    public void ParsePackageJson(string packageName, string packageVersion)
    {
        if (string.IsNullOrEmpty(packageName))
        {
            throw new ArgumentNullException(nameof(packageName));
        }

        if (string.IsNullOrEmpty(packageVersion))
        {
            throw new ArgumentNullException(nameof(packageVersion));
        }

        string directive = packageName + "#" + packageVersion;

        if (!_packageJsonExpectations.TryGetValue((packageName, packageVersion), out PackageManifestExpectationRecord? expectations))
        {
            throw new ArgumentException($"No expectations found for test '{directive}'.");
        }

        string packageJson = readPackageJson(directive);

        PackageManifest? manifest = JsonSerializer.Deserialize<PackageManifest>(packageJson);

        manifest.ShouldNotBeNull();

        testManifestExpectations(manifest, expectations);
    }

    [Theory]
    [InlineData("manifest-backport.json")]
    [InlineData("manifest-backport-r4.json")]
    [InlineData("manifest-backport-r4b.json")]
    [InlineData("manifest-ihe-pdqm.json")]
    public void ParseManifest(string manifestFilename)
    {
        if (string.IsNullOrEmpty(manifestFilename))
        {
            throw new ArgumentNullException(nameof(manifestFilename));
        }

        string manifestJson = readJsonFile(manifestFilename);

        if (string.IsNullOrEmpty(manifestJson))
        {
            throw new ArgumentException($"Failed to load manifest file '{manifestFilename}'");
        }

        PackageManifest? manifest = JsonSerializer.Deserialize<PackageManifest>(manifestJson);
        manifest.ShouldNotBeNull();
        manifest.Name.ShouldNotBeNullOrWhiteSpace();
        manifest.Version.ShouldNotBeNullOrWhiteSpace();

        string directive = manifest.Name + "#" + manifest.Version;

        if (!_shortManifestExpectations.TryGetValue((manifest.Name, manifest.Version), out PackageManifestExpectationRecord? expectations))
        {
            throw new ArgumentException($"No expectations found for test '{directive}'.");
        }

        testShortManifestExpectations(manifest, expectations);
    }

    private static string readJsonFile(string filename)
    {
        // Get the absolute path to the file
        string path = Path.GetRelativePath(
            Directory.GetCurrentDirectory(),
            Path.Combine("TestData", filename));

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        return File.ReadAllText(path);
    }

    private static string readPackageJson(string packageDirective)
    {
        // Get the absolute path to the file
        string path = Path.GetRelativePath(
            Directory.GetCurrentDirectory(),
            Path.Combine("TestData", ".fhir", "packages", packageDirective, "package", "package.json"));

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        return File.ReadAllText(path);
    }
}
