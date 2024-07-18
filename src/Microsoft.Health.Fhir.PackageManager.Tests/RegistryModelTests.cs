// <copyright file="FhirDirectiveTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using FluentAssertions;
using Microsoft.Health.Fhir.PackageManager;
using Microsoft.Health.Fhir.PackageManager.Models;
using Microsoft.Health.Fhir.PackageManager.Tests.Extensions;
using Xunit.Abstractions;

using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

/// <summary>FHIR npm registry model tests.</summary>
public class RegistryModelTests
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryModelTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public RegistryModelTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [FileData("data/catalog-backport-primary.json")]
    [FileData("data/catalog-backport-secondary.json")]
    internal void ParseCatalogRecsBackport(string json)
    {
        FhirNpmPackageDetails[]? recs = JsonSerializer.Deserialize<FhirNpmPackageDetails[]>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        recs.Should().NotBeNullOrEmpty();
        recs!.Length.Should().Be(3);

        recs![0].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        recs![0].FhirVersion.Should().Be("R4B");
        recs![0].FhirVersions.Should().NotBeNullOrEmpty();
        recs![0].FhirVersions.First().Should().Be("R4B");
        recs![0].FhirVersionList.Should().NotBeNullOrEmpty();
        recs![0].FhirVersionList.First().Should().Be("R4B");

        recs![1].Name.Should().Be("hl7.fhir.uv.subscriptions-backport.r4");
        recs![1].FhirVersion.Should().Be("R4");
        recs![1].FhirVersions.Should().NotBeNullOrEmpty();
        recs![1].FhirVersions.First().Should().Be("R4");
        recs![1].FhirVersionList.Should().NotBeNullOrEmpty();
        recs![1].FhirVersionList.First().Should().Be("R4");

        recs![2].Name.Should().Be("hl7.fhir.uv.subscriptions-backport.r4b");
        recs![2].FhirVersion.Should().Be("R4B");
        recs![2].FhirVersions.Should().NotBeNullOrEmpty();
        recs![2].FhirVersions.First().Should().Be("R4B");
        recs![2].FhirVersionList.Should().NotBeNullOrEmpty();
        recs![2].FhirVersionList.First().Should().Be("R4B");
    }

    [Theory]
    [FileData("data/package-info-backport-primary.json")]
    [FileData("data/package-info-backport-secondary.json")]
    internal void ParsePackageInfoDeserializeBackport(string json)
    {
        RegistryPackageManifest? manifest = JsonSerializer.Deserialize<RegistryPackageManifest>(json);

        manifest.Should().NotBeNull();

        manifest!.Id.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.DistributionTags.Should().NotBeNullOrEmpty();
        manifest!.DistributionTags.Should().ContainKey("latest");
        manifest!.DistributionTags["latest"].Should().Be("1.1.0");

        manifest!.Versions.Should().NotBeNullOrEmpty();
        manifest!.Versions.Count.Should().Be(3);
        manifest!.Versions.Should().ContainKey("0.1.0");
        manifest!.Versions["0.1.0"].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Versions["0.1.0"].Version.Should().Be("0.1.0");
        manifest!.Versions["0.1.0"].FhirVersion.Should().Be("R4");
        manifest!.Versions.Should().ContainKey("1.0.0");
        manifest!.Versions["1.0.0"].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Versions["1.0.0"].Version.Should().Be("1.0.0");
        manifest!.Versions["1.0.0"].FhirVersion.Should().Be("R4B");
        manifest!.Versions.Should().ContainKey("1.1.0");
        manifest!.Versions["1.1.0"].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Versions["1.1.0"].Version.Should().Be("1.1.0");
        manifest!.Versions["1.1.0"].FhirVersion.Should().Be("R4B");
    }

    [Theory]
    [FileData("data/package-info-backport-primary.json")]
    [FileData("data/package-info-backport-secondary.json")]
    internal void ParsePackageInfoParseBackport(string json)
    {
        RegistryPackageManifest? manifest = RegistryPackageManifest.Parse(json);

        manifest.Should().NotBeNull();

        manifest!.Id.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.DistributionTags.Should().NotBeNullOrEmpty();
        manifest!.DistributionTags.Should().ContainKey("latest");
        manifest!.DistributionTags["latest"].Should().Be("1.1.0");

        manifest!.Versions.Should().NotBeNullOrEmpty();
        manifest!.Versions.Count.Should().Be(3);
        manifest!.Versions.Should().ContainKey("0.1.0");
        manifest!.Versions["0.1.0"].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Versions["0.1.0"].Version.Should().Be("0.1.0");
        manifest!.Versions["0.1.0"].FhirVersion.Should().Be("R4");
        manifest!.Versions.Should().ContainKey("1.0.0");
        manifest!.Versions["1.0.0"].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Versions["1.0.0"].Version.Should().Be("1.0.0");
        manifest!.Versions["1.0.0"].FhirVersion.Should().Be("R4B");
        manifest!.Versions.Should().ContainKey("1.1.0");
        manifest!.Versions["1.1.0"].Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        manifest!.Versions["1.1.0"].Version.Should().Be("1.1.0");
        manifest!.Versions["1.1.0"].FhirVersion.Should().Be("R4B");
    }
}
