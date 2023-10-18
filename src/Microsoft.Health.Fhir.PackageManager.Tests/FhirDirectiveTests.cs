// <copyright file="FhirDirectiveTests.cs" company="Microsoft Corporation">
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
public class FhirDirectiveTests
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirDirectiveTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public FhirDirectiveTests(ITestOutputHelper testOutputHelper)
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
    [InlineData("example.org.ig#invalid", true, DirectiveNameTypeCodes.GuideWithoutSuffix, DirectiveVersionCodes.NonSemVer)]
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
}
