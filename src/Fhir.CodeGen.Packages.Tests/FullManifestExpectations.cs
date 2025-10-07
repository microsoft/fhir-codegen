using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Fhir.CodeGen.Packages.Tests;

internal static class FullManifestExpectations
{
    internal record class ExpectedDistTag(string Tag, string Version);

    internal record class FullManifestExpectationRecord
    {
        public required string? LocalPackageJsonFilename { get; init; }
        public required PackageRegistryRecord.RegistryTypeCodes? RegistrySource { get; init; }
        public required string PackageName { get; init; }
        public required bool HasDescription { get; init; }
        public required int VersionCount { get; init; }
        public required int? CatalogCount { get; init; }
        public List<ExpectedDistTag>? DistTags { get; init; } = null;
        public List<FullManifestVersionExpectationRecord>? VersionExpectations { get; init; } = null;
    }

    internal record class FullManifestVersionExpectationRecord
    {
        public required string Name { get; init; }
        public required string Version { get; init; }
        public required FhirReleases.FhirSequenceCodes FhirSequence { get; init; }
        public required string TarballUrl { get; init; }
        public string? ShaSum { get; init; } = null;
        public string? Url { get; init; } = null;
        public string? Canonical { get; init; } = null;
    }

    internal static readonly List<FullManifestExpectationRecord> _fullManifestExpectations = [
        new()
        {
            LocalPackageJsonFilename = "package-info-backport-primary.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
            PackageName = "hl7.fhir.uv.subscriptions-backport",
            HasDescription = true,
            VersionCount = 3,
            CatalogCount = 3,
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
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-backport-primary-r4.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
            PackageName = "hl7.fhir.uv.subscriptions-backport.r4",
            HasDescription = true,
            VersionCount = 1,
            CatalogCount = 2,
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
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-backport-primary-r4b.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
            PackageName = "hl7.fhir.uv.subscriptions-backport.r4b",
            HasDescription = true,
            VersionCount = 1,
            CatalogCount = 1,
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
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-backport-secondary.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
            PackageName = "hl7.fhir.uv.subscriptions-backport",
            HasDescription = true,
            VersionCount = 3,
            CatalogCount = 3,
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
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-backport-secondary-r4.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
            PackageName = "hl7.fhir.uv.subscriptions-backport.r4",
            HasDescription = true,
            VersionCount = 1,
            CatalogCount = 2,
            DistTags = [ new("latest", "1.1.0") ],
            VersionExpectations = [
                new()
                {
                    Version = "1.1.0",
                    Name = "hl7.fhir.uv.subscriptions-backport.r4",
                    FhirSequence = FhirReleases.FhirSequenceCodes.R4,
                    ShaSum = "CA5E4F4C87857597931EEFA9E20D6F0B6DE6E44A",
                    TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport.R4/1.1.0",
                    Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.R4/1.1.0",
                    Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                },
            ],
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-backport-secondary-r4b.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
            PackageName = "hl7.fhir.uv.subscriptions-backport.r4b",
            HasDescription = true,
            VersionCount = 1,
            CatalogCount = 1,
            DistTags = [ new("latest", "1.1.0") ],
            VersionExpectations = [
                new()
                {
                    Version = "1.1.0",
                    Name = "hl7.fhir.uv.subscriptions-backport.r4b",
                    FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                    ShaSum = "F6BB023E4FA00837234887B8F5942F9A85298E8E",
                    TarballUrl = "https://packages2.fhir.org/hl7.fhir.uv.subscriptions-backport.R4b/1.1.0",
                    Url = "https://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.R4b/1.1.0",
                    Canonical = "http://hl7.org/fhir/uv/subscriptions-backport",
                },
            ],
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-us-core-primary.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
            PackageName = "hl7.fhir.us.core",
            HasDescription = true,
            VersionCount = 19,
            CatalogCount = 1,
            DistTags = [ new("latest", "6.1.0") ],
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-us-core-secondary.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
            PackageName = "hl7.fhir.us.core",
            HasDescription = true,
            VersionCount = 19,
            CatalogCount = 1,
            DistTags = [ new("latest", "6.1.0") ],
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-r4-core-primary.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
            PackageName = "hl7.fhir.r4.core",
            HasDescription = true,
            VersionCount = 1,
            CatalogCount = 2,
            DistTags = [ new("latest", "4.0.1") ],
        },
        new()
        {
            LocalPackageJsonFilename = "package-info-r4-core-secondary.json",
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
            PackageName = "hl7.fhir.r4.core",
            HasDescription = true,
            VersionCount = 1,
            CatalogCount = 2,
            DistTags = [ new("latest", "4.0.1") ],
        },
        new()
        {
            LocalPackageJsonFilename = null,
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirPrimary,
            PackageName = "hl7.fhir.r4",
            HasDescription = true,
            VersionCount = -1,
            CatalogCount = 11,
        },
        new()
        {
            LocalPackageJsonFilename = null,
            RegistrySource = PackageRegistryRecord.RegistryTypeCodes.FhirSecondary,
            PackageName = "hl7.fhir.r4",
            HasDescription = true,
            VersionCount = -1,
            CatalogCount = 10,
        },
    ];

}
