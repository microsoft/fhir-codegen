// <copyright file="CiTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FluentAssertions;
using Microsoft.Health.Fhir.PackageManager.Models;
using Xunit.Abstractions;

using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

public class RegistryTests : IClassFixture<RegistryTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The cache.</summary>
    private readonly FhirCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryTests"/> class.
    /// </summary>
    /// <param name="fixture">         The fixture.</param>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public RegistryTests(RegistryTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cache = fixture._cache;
        _testOutputHelper = testOutputHelper;
        fixture._handler._testOutputHelper = testOutputHelper;
    }

    /// <summary>Resolve version range.</summary>
    /// <param name="packageId">        Identifier for the package.</param>
    /// <param name="nameType">         Type of the name.</param>
    /// <param name="version">          The version.</param>
    /// <param name="shouldFindVersion">The should find version.</param>
    /// <param name="shouldSucceed">    True if should succeed.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "1.1.x", "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "1.1", "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, "1.1.x", "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, "1.1", "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", DirectiveNameTypeCodes.GuideWithSuffix, "1.1.x", "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", DirectiveNameTypeCodes.GuideWithSuffix, "1.1", "1.1.0", true)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, "3.0.x", "3.0.1", true)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, "5.0.x", "5.0.1", true)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, "6.0.x", "6.0.0", true)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, "6.1.x", "6.1.0", true)]
    [InlineData("hl7.fhir.r4.core", DirectiveNameTypeCodes.CoreFull, "4.0.x", "4.0.1", true)]
    [InlineData("hl7.fhir.r4.core", DirectiveNameTypeCodes.CoreFull, "4.0", "4.0.1", true)]
    [InlineData("hl7.fhir.r4", DirectiveNameTypeCodes.CorePartial, "4.0.x", "4.0.1", true)]
    [InlineData("hl7.fhir.r4", DirectiveNameTypeCodes.CorePartial, "4.0", "4.0.1", true)]

    internal void ResolveVersionRange(
        string packageId,
        DirectiveNameTypeCodes nameType,
        string version,
        string shouldFindVersion,
        bool shouldSucceed)
    {
        // fill out a FhirDirective as if we had parsed it already
        FhirDirective directive = new()
        {
            Directive = $"{packageId}#{version}",
            PackageId = packageId,
            NameType = nameType,
            FhirRelease = string.Empty,
            PackageVersion = version,
            VersionType = DirectiveVersionCodes.Partial,
        };

        bool success = _cache.TryResolveVersionRange(ref directive);

        success.Should().Be(shouldSucceed);

        if (!success)
        {
            return;
        }

        directive.PackageId.Should().Be(packageId);
        directive.VersionType.Should().Be(DirectiveVersionCodes.Exact);
        directive.PackageVersion.Should().Be(shouldFindVersion);
        directive.ResolvedTarballUrl.Should().NotBeNullOrEmpty();
    }

    /// <summary>Resolve version range.</summary>
    /// <param name="packageId">        Identifier for the package.</param>
    /// <param name="nameType">         Type of the name.</param>
    /// <param name="version">          The version.</param>
    /// <param name="shouldFindVersion">The should find version.</param>
    /// <param name="shouldSucceed">    True if should succeed.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, "1.1.0", true)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", DirectiveNameTypeCodes.GuideWithSuffix, "1.1.0", true)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, "6.1.0", true)]
    [InlineData("hl7.fhir.r4.core", DirectiveNameTypeCodes.CoreFull, "4.0.1", true)]
    [InlineData("hl7.fhir.r4", DirectiveNameTypeCodes.CorePartial, "4.0.1", true)]
    internal void ResolveVersionLatest(
        string packageId,
        DirectiveNameTypeCodes nameType,
        string shouldFindVersion,
        bool shouldSucceed)
    {
        // fill out a FhirDirective as if we had parsed it already
        FhirDirective directive = new()
        {
            Directive = $"{packageId}#latest",
            PackageId = packageId,
            NameType = nameType,
            FhirRelease = string.Empty,
            PackageVersion = "latest",
            VersionType = DirectiveVersionCodes.Latest,
        };

        bool success = _cache.TryResolveVersionLatest(ref directive);

        success.Should().Be(shouldSucceed);

        if (!success)
        {
            return;
        }

        directive.PackageId.Should().Be(packageId);
        directive.VersionType.Should().Be(DirectiveVersionCodes.Exact);
        directive.PackageVersion.Should().Be(shouldFindVersion);
        directive.ResolvedTarballUrl.Should().NotBeNullOrEmpty();
    }

    /// <summary>Gets the manifests.</summary>
    /// <param name="packageId">            Identifier for the package.</param>
    /// <param name="nameType">             Type of the name.</param>
    /// <param name="expectedManifestCount">Number of expected manifests.</param>
    /// <param name="expectedVersionCount"> Number of expected versions.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, 2, 6)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, 2, 2)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", DirectiveNameTypeCodes.GuideWithSuffix, 2, 2)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, 2, 38)]
    [InlineData("hl7.fhir.r4.core", DirectiveNameTypeCodes.CoreFull, 2, 2)]
    [InlineData("hl7.fhir.r4", DirectiveNameTypeCodes.CorePartial, 2, 2)]
    internal void GetManifests(
        string packageId,
        DirectiveNameTypeCodes nameType,
        int expectedManifestCount,
        int expectedVersionCount)
    {
        // fill out a FhirDirective as if we had parsed it already
        FhirDirective directive = new()
        {
            Directive = $"{packageId}#latest",
            PackageId = packageId,
            NameType = nameType,
            FhirRelease = string.Empty,
            PackageVersion = "latest",
            VersionType = DirectiveVersionCodes.Latest,
        };

        bool success = _cache.TryGetRegistryManifests(ref directive);

        if (expectedManifestCount == 0)
        {
            success.Should().BeFalse();
            return;
        }

        success.Should().BeTrue();
        directive.Manifests.Should().NotBeNullOrEmpty();
        directive.Manifests.Should().HaveCount(expectedManifestCount);
        directive.Manifests.Values.Sum(m => m.Versions.Count).Should().Be(expectedVersionCount);
    }

    /// <summary>Gets the catalogs.</summary>
    /// <param name="packageId">           Identifier for the package.</param>
    /// <param name="nameType">            Type of the name.</param>
    /// <param name="expectedCatalogCount">Number of expected catalogs.</param>
    /// <param name="expectedEntryCount">  Number of expected entries.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, 2, 6)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, 2, 4)]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", DirectiveNameTypeCodes.GuideWithSuffix, 2, 2)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, 2, 2)]
    [InlineData("hl7.fhir.r4.core", DirectiveNameTypeCodes.CoreFull, 2, 4)]
    [InlineData("hl7.fhir.r4", DirectiveNameTypeCodes.CorePartial, 2, 21)]
    internal void GetCatalogs(
        string packageId,
        DirectiveNameTypeCodes nameType,
        int expectedCatalogCount,
        int expectedEntryCount)
    {
        // fill out a FhirDirective as if we had parsed it already
        FhirDirective directive = new()
        {
            Directive = $"{packageId}#latest",
            PackageId = packageId,
            NameType = nameType,
            FhirRelease = string.Empty,
            PackageVersion = "latest",
            VersionType = DirectiveVersionCodes.Latest,
        };

        bool success = _cache.TryCatalogSearch(ref directive);

        if (expectedCatalogCount == 0)
        {
            success.Should().BeFalse();
            return;
        }

        success.Should().BeTrue();
        directive.CatalogEntries.Should().NotBeNullOrEmpty();
        directive.CatalogEntries.Should().HaveCount(expectedCatalogCount);
        directive.CatalogEntries.Values.Sum(m => m.Keys.Count).Should().Be(expectedEntryCount);
    }

    /// <summary>Resolve name from catalog.</summary>
    /// <param name="packageId">   Identifier for the package.</param>
    /// <param name="nameType">    Type of the name.</param>
    /// <param name="resolvedId">  Identifier for the resolved.</param>
    /// <param name="fhirSequence">(Optional) The FHIR sequence.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "hl7.fhir.uv.subscriptions-backport", FhirCache.FhirSequenceCodes.R4B)]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "hl7.fhir.uv.subscriptions-backport.r4", FhirCache.FhirSequenceCodes.R4)]
    [InlineData("hl7.fhir.us.core", DirectiveNameTypeCodes.GuideWithoutSuffix, "hl7.fhir.us.core")]
    internal void ResolveNameFromCatalog(
        string packageId,
        DirectiveNameTypeCodes nameType,
        string resolvedId,
        FhirCache.FhirSequenceCodes fhirSequence = FhirCache.FhirSequenceCodes.Unknown)
    {
        // fill out a FhirDirective as if we had parsed it already
        FhirDirective directive = new()
        {
            Directive = $"{packageId}#latest",
            PackageId = packageId,
            NameType = nameType,
            FhirRelease = string.Empty,
            PackageVersion = "latest",
            VersionType = DirectiveVersionCodes.Latest,
        };

        bool success = _cache.TryResolveNameFromCatalog(ref directive, fhirSequence);

        success.Should().BeTrue();

        if (!success)
        {
            return;
        }

        directive.PackageId.Should().Be(resolvedId);

        if (fhirSequence == FhirCache.FhirSequenceCodes.Unknown)
        {
            directive.FhirRelease.Should().NotBeNullOrEmpty();
        }
        else
        {
            directive.FhirRelease.Should().Be(FhirCache.ToRLiteral(fhirSequence));
        }
    }
}

/// <summary>A registry test fixture.</summary>
public class RegistryTestFixture
{
    public PackageHttpMessageHandler _handler;

    public FhirCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryTestFixture"/> class.
    /// </summary>
    public RegistryTestFixture()
    {
        _handler = new PackageHttpMessageHandler();
        _cache = new FhirCache(Path.Combine(Directory.GetCurrentDirectory(), "data", ".fhir"), null, null);
        _cache._httpClient = new(_handler);
    }
}
