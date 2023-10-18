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
    /// <param name="directive">The directive.</param>
    /// <remarks>Ensure that any requested directives appear in data/qas-full.json</remarks>
    [Theory]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "4.3.0", "")]
    [InlineData("hl7.fhir.uv.subscriptions-backport", DirectiveNameTypeCodes.GuideWithoutSuffix, "4.3.0", "propose-changes")]

    //[InlineData("hl7.fhir.uv.subscriptions-backport.r4", DirectiveNameTypeCodes.GuideWithSuffix, "4.0.1", "")]
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
