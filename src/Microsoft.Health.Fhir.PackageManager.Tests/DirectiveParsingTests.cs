﻿// <copyright file="FhirDirectiveTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using FluentAssertions;
using Microsoft.Health.Fhir.PackageManager;
using Microsoft.Health.Fhir.PackageManager.Models;
using Xunit.Abstractions;

using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

/// <summary>A FHIR directive tests.</summary>
public class DirectiveParsingTests
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveParsingTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public DirectiveParsingTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("hl7.fhir.uv.ig.r4#1.0.0", true, DirectiveNameTypeCodes.GuideWithSuffix, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.uv.ig#1.0.0", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.uv.ig.r4#1.0.x", true, DirectiveNameTypeCodes.GuideWithSuffix, DirectiveVersionCodes.Partial)]
    [InlineData("hl7.fhir.uv.ig#1.0.x", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Partial)]
    [InlineData("hl7.fhir.uv.ig.r4#latest", true, DirectiveNameTypeCodes.GuideWithSuffix, DirectiveVersionCodes.Latest)]
    [InlineData("hl7.fhir.uv.ig#latest", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Latest)]
    [InlineData("hl7.fhir.uv.ig.r4#dev", true, DirectiveNameTypeCodes.GuideWithSuffix, DirectiveVersionCodes.Local)]
    [InlineData("hl7.fhir.uv.ig#dev", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Local)]
    [InlineData("hl7.fhir.uv.ig.r4#current", true, DirectiveNameTypeCodes.GuideWithSuffix, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.uv.ig#current", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.uv.ig.r4#current$branch", true, DirectiveNameTypeCodes.GuideWithSuffix, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.uv.ig#current$branch", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.r4.core#4.0.1", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4#4.0.1", true, DirectiveNameTypeCodes.CorePartial, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.core#4.0.x", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Partial)]
    [InlineData("hl7.fhir.r4#4.0.x", true, DirectiveNameTypeCodes.CorePartial, DirectiveVersionCodes.Partial)]
    [InlineData("hl7.fhir.r4.core#latest", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Latest)]
    [InlineData("hl7.fhir.r4#latest", true, DirectiveNameTypeCodes.CorePartial, DirectiveVersionCodes.Latest)]
    [InlineData("hl7.fhir.r4.core#dev", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Local)]
    [InlineData("hl7.fhir.r4#dev", true, DirectiveNameTypeCodes.CorePartial, DirectiveVersionCodes.Local)]
    [InlineData("hl7.fhir.r4.core#current", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.r4#current", true, DirectiveNameTypeCodes.CorePartial, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.r4.core#current$branch", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.r4#current$branch", true, DirectiveNameTypeCodes.CorePartial, DirectiveVersionCodes.ContinuousIntegration)]
    [InlineData("hl7.fhir.r4.expansions#4.0.1", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.examples#4.0.1", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.search#4.0.1", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.corexml#4.0.1", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.elements#4.0.1", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.notcore#4.0.1", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Exact)]
    [InlineData("hl7.fhir.r4.core", true, DirectiveNameTypeCodes.CoreFull, DirectiveVersionCodes.Latest)]
    [InlineData("hl7.fhir.r4.core#invalid", false, null, null)]
    [InlineData("hl7.fhir.uv.ig#invalid", false, null, null)]
    [InlineData("example.org.ig#notsemver", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.NonSemVer)]
    [InlineData("hl7.fhir.r4.core#4", false, null, null)]
    [InlineData("hl7.fhir.r4#4", false, null, null)]
    [InlineData("example.org.ig#4", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.NonSemVer)]
    [InlineData("example.org.ig#1.0.x", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Partial)]
    [InlineData("example.org.ig#1.0.0", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.Exact)]
    [InlineData("example.org.ig#1.0", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.NonSemVer)]
    [InlineData("example.org.ig#20010101", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.NonSemVer)]
    [InlineData("too#many#segments", false, null, null)]

    internal void ParseDirective(
        string input,
        bool shouldPass,
        DirectiveNameTypeCodes? nameType,
        DirectiveVersionCodes? versionType)
    {
        bool parsed = FhirCache.TryParseDirective(input, out FhirDirective? directive);

        parsed.Should().Be(shouldPass);

        if (shouldPass)
        {
            directive.Should().NotBeNull();
            directive!.NameType.Should().Be(nameType);
            directive!.VersionType.Should().Be(versionType);
        }
    }

    [Theory]
    [InlineData("hl7.fhir.r4#4.0.1", true, "http://hl7.org/fhir/R4/hl7.fhir.r4.core.tgz")]
    [InlineData("hl7.fhir.r4.core#4.0.1", true, "http://hl7.org/fhir/R4/hl7.fhir.r4.core.tgz")]
    [InlineData("hl7.fhir.r4.core#4.0", true, "")]
    [InlineData("hl7.fhir.r4.core#latest", true, "")]
    [InlineData("hl7.fhir.uv.subscriptions-backport#1.1.0", true, "http://hl7.org/fhir/uv/subscriptions-backport/package.tgz")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4#1.1.0", true, "http://hl7.org/fhir/uv/subscriptions-backport/package.r4.tgz")]
    internal void CheckPublicationUrl(
        string input,
        bool shouldPass,
        string expectedUrl = "")
    {
        bool parsed = FhirCache.TryParseDirective(input, out FhirDirective? directive);

        parsed.Should().Be(shouldPass);

        if (shouldPass)
        {
            directive.Should().NotBeNull();
            directive!.PublicationPackageUrl.Should().Be(expectedUrl);
        }
    }

    [Theory]
    [InlineData("http://hl7.org/fhir/R4/hl7.fhir.r4.core.tgz", true, "hl7.fhir.r4.core")]
    [InlineData("http://hl7.org/fhir/R4/index.html", true, "hl7.fhir.r4.core")]
    [InlineData("http://hl7.org/fhir/R4/", true, "hl7.fhir.r4.core")]
    [InlineData("http://hl7.org/fhir/R4", true, "hl7.fhir.r4.core")]
    [InlineData("https://hl7.org/fhir/R4/hl7.fhir.r4.core.tgz", true, "hl7.fhir.r4.core")]
    [InlineData("https://hl7.org/fhir/R4/", true, "hl7.fhir.r4.core")]

    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/package.r4.tgz", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/package.tgz", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/index.html", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport", true, "hl7.fhir.uv.subscriptions-backport")]

    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/2021Jan/package.r4.tgz", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/2021Jan/package.tgz", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/2021Jan/index.html", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/2021Jan/", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/2021Jan", true, "hl7.fhir.uv.subscriptions-backport")]

    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/STU1.1/package.r4.tgz", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/STU1.1/package.tgz", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/STU1.1/index.html", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/STU1.1/", true, "hl7.fhir.uv.subscriptions-backport")]
    [InlineData("https://hl7.org/fhir/uv/subscriptions-backport/STU1.1", true, "hl7.fhir.uv.subscriptions-backport")]

    [InlineData("https://hl7.org/fhir/smart-app-launch/package.tgz", true, "hl7.fhir.uv.smart-app-launch")]
    [InlineData("https://hl7.org/fhir/smart-app-launch/index.html", true, "hl7.fhir.uv.smart-app-launch")]
    [InlineData("https://hl7.org/fhir/smart-app-launch/", true, "hl7.fhir.uv.smart-app-launch")]
    [InlineData("https://hl7.org/fhir/smart-app-launch", true, "hl7.fhir.uv.smart-app-launch")]

    [InlineData("http://build.fhir.org/hl7.fhir.r6.core.tgz", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/hl7.fhir.r6.core.tgz", true, "hl7.fhir.r6.core")]
    [InlineData("http://build.fhir.org/", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/", true, "hl7.fhir.r6.core")]
    [InlineData("http://build.fhir.org/index.html", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/index.html", true, "hl7.fhir.r6.core")]

    [InlineData("http://build.fhir.org/branches/test/hl7.fhir.r6.core.tgz", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/branches/test/hl7.fhir.r6.core.tgz", true, "hl7.fhir.r6.core")]
    [InlineData("http://build.fhir.org/branches/test/", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/branches/test/", true, "hl7.fhir.r6.core")]
    [InlineData("http://build.fhir.org/branches/test/index.html", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/branches/test/index.html", true, "hl7.fhir.r6.core")]
    [InlineData("http://build.fhir.org/branches/test", true, "hl7.fhir.r6.core")]
    [InlineData("https://build.fhir.org/branches/test", true, "hl7.fhir.r6.core")]

    internal void ParseUrl(
        string url,
        bool shouldPass,
        string expectedId = "")
    {
        bool parsed = FhirCache.TryParseUrl(url, out FhirDirective? directive);

        parsed.Should().Be(shouldPass);

        if (shouldPass)
        {
            directive.Should().NotBeNull();
            directive!.PackageId.Should().Be(expectedId);
        }

    }
}
