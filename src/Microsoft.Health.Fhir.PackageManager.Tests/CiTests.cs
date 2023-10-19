// <copyright file="CiTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FluentAssertions;
using Microsoft.Health.Fhir.PackageManager.Models;
using Xunit.Abstractions;

using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

public class CiTests : IClassFixture<CiTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The cache.</summary>
    private readonly FhirCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CiTests"/> class.
    /// </summary>
    /// <param name="fixture">         The fixture.</param>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public CiTests(CiTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cache = fixture._cache;
        _testOutputHelper = testOutputHelper;
        fixture._handler._testOutputHelper = testOutputHelper;
    }

    /// <summary>Resolve ci by directive.</summary>
    /// <remarks>Ensure that any requested directives appear in data/qas-full.json.</remarks>
    /// <param name="packageId">  Identifier for the package.</param>
    /// <param name="nameType">   Type of the name.</param>
    /// <param name="fhirRelease">The FHIR release.</param>
    /// <param name="branchName"> Name of the branch.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "4.3.0", "")]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "4.3.0", "propose-changes")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, "4.0.1", "")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b", DirectiveNameTypeCodes.GuideWithSuffix, "4.3.0", "")]

    internal void ResolveCiByDirective(
        string packageId,
        DirectiveNameTypeCodes nameType,
        string fhirRelease,
        string branchName)
    {
        // fill out a FhirDirective as if we had parsed it already
        FhirDirective directive = new()
        {
            Directive = string.IsNullOrEmpty(branchName) ? $"{packageId}#current" : $"{packageId}#current{branchName}",
            PackageId = packageId,
            NameType = nameType,
            FhirRelease = fhirRelease,
            PackageVersion = string.IsNullOrEmpty(branchName) ? "current" : $"current{branchName}",
            VersionType = DirectiveVersionCodes.ContinuousIntegration,
            CiBranch = branchName,
        };

        bool success = _cache.TryResolveCi(ref directive);

        success.Should().BeTrue();

        directive.PackageId.Should().Be(packageId);
        directive.FhirRelease.Should().Be(fhirRelease);

        directive.CiUrl.Should().NotBeNullOrEmpty();
        directive.CiOrg.Should().NotBeNullOrEmpty();
        directive.CiBranch.Should().NotBeNullOrEmpty();

        if (!string.IsNullOrEmpty(branchName))
        {
            directive.CiBranch.Should().Be(branchName);
        }
    }

    /// <summary>Resolve ci URL.</summary>
    /// <param name="url">                URL of the resource.</param>
    /// <param name="resolvedId">         Identifier for the resolved.</param>
    /// <param name="resolvedFhirVersion">The resolved FHIR version.</param>
    /// <param name="resolvedVersion">    The resolved version.</param>
    [Theory]
    [InlineData("https://profiles.ihe.net/ITI/PDQm", "ihe.iti.pdqm", "4.0.1", "2.4.0")]
    [InlineData("https://profiles.ihe.net/ITI/PDQm/package.tgz", "ihe.iti.pdqm", "4.0.1", "2.4.0")]
    [InlineData("https://profiles.ihe.net/ITI/PDQm/index.html", "ihe.iti.pdqm", "4.0.1", "2.4.0")]

    [InlineData("HL7/fhir-subscription-backport-ig/branches/master/qa.json", "hl7.fhir.uv.subscriptions-backport", "4.3.0", "1.1.0")]
    // TODO: need to determine if this is worth dealing with
    //[InlineData("HL7/fhir-subscription-backport-ig/branches/master/qa.json", "hl7.fhir.uv.subscriptions-backport", "4.0.1", "1.1.0")]

    [InlineData("http://hl7.org/fhir/uv/subscriptions-backport/ImplementationGuide/hl7.fhir.uv.subscriptions-backport", "hl7.fhir.uv.subscriptions-backport", "4.3.0", "1.1.0")]

    [InlineData("https://build.fhir.org/ig/HL7/fhir-subscription-backport-ig/", "hl7.fhir.uv.subscriptions-backport", "4.3.0", "1.1.0")]
    [InlineData("https://build.fhir.org/ig/HL7/fhir-subscription-backport-ig/package.tgz", "hl7.fhir.uv.subscriptions-backport", "4.3.0", "1.1.0")]
    [InlineData("https://build.fhir.org/ig/HL7/fhir-subscription-backport-ig/package.r4.tgz", "hl7.fhir.uv.subscriptions-backport.r4", "4.0.1", "1.1.0")]

    internal void ResolveCiUrl(
        string url,
        string resolvedId,
        string resolvedFhirVersion,
        string resolvedVersion)
    {
        bool success = _cache.TryResolveCi(url, out FhirDirective? directive);

        success.Should().BeTrue();
        directive.Should().NotBeNull();

        if (directive == null) return;

        directive.PackageId.Should().Be(resolvedId);
        directive.VersionType.Should().Be(DirectiveVersionCodes.ContinuousIntegration);
        directive.FhirRelease.Should().Be(resolvedFhirVersion);
        directive.PackageVersion.Should().Be(resolvedVersion);
        directive.ResolvedTarballUrl.Should().NotBeNullOrEmpty();
    }
}

/// <summary>A ci test fixture.</summary>
public class CiTestFixture
{
    public PackageHttpMessageHandler _handler;

    public FhirCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CiTestFixture"/> class.
    /// </summary>
    public CiTestFixture()
    {
        _handler = new PackageHttpMessageHandler();
        _cache = new FhirCache(Path.GetRelativePath(Directory.GetCurrentDirectory(), "data/.fhir"), null, null);
        _cache._httpClient = new(_handler);
    }
}
