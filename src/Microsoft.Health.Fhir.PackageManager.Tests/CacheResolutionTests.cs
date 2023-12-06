// <copyright file="CacheResolutionTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FluentAssertions;
using Microsoft.Health.Fhir.PackageManager.Models;
using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;
using Xunit.Abstractions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

public class CacheResolutionTests : IClassFixture<CacheResolutionTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The cache.</summary>
    private readonly FhirCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheResolutionTests"/> class.
    /// </summary>
    /// <param name="fixture">         The fixture.</param>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public CacheResolutionTests(CacheResolutionTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cache = fixture._cache;
        _testOutputHelper = testOutputHelper;
        fixture._handler._testOutputHelper = testOutputHelper;
    }

    /// <summary>Resolve directive.</summary>
    /// <param name="directive">    The directive.</param>
    /// <param name="shouldSucceed">True if should succeed.</param>
    /// <param name="expectedDir">  The expected dir.</param>
    [Theory]
    [InlineData("hl7.fhir.uv.ig.r4#1.0.0", false)]
    [InlineData("hl7.fhir.uv.ig#1.0.0", false)]
    [InlineData("hl7.fhir.uv.ig.r4#1.0.x", false)]
    [InlineData("hl7.fhir.uv.ig#1.0.x", false)]
    [InlineData("hl7.fhir.uv.ig.r4#latest", false)]
    [InlineData("hl7.fhir.uv.ig#latest", false)]
    [InlineData("hl7.fhir.uv.ig.r4#dev", false)]
    [InlineData("hl7.fhir.uv.ig#dev", false)]
    [InlineData("hl7.fhir.uv.ig.r4#current", false)]
    [InlineData("hl7.fhir.uv.ig#current", false)]
    [InlineData("hl7.fhir.uv.ig.r4#current$branch", false)]
    [InlineData("hl7.fhir.uv.ig#current$branch", false)]
    [InlineData("hl7.fhir.r4.core#4.0.1", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4#4.0.1", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4.core#4.0.x", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4#4.0.x", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4.core#latest", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4#latest", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4.core#dev", false)]
    [InlineData("hl7.fhir.r5#dev", false)]
    [InlineData("hl7.fhir.r5.core#current", false)]
    [InlineData("hl7.fhir.r5#current", false)]
    [InlineData("hl7.fhir.r5.core#current$branch", false)]
    [InlineData("hl7.fhir.r5.core#current$notExistantbranch", false)]
    [InlineData("hl7.fhir.r5#current$branch", false)]
    [InlineData("hl7.fhir.r6#dev", true, "hl7.fhir.r6.core#current")]
    [InlineData("hl7.fhir.r6.core#current", true, "hl7.fhir.r6.core#current")]     // core builds are not in QAS
    [InlineData("hl7.fhir.r6#current", true, "hl7.fhir.r6.core#current")]
    [InlineData("hl7.fhir.r6.core#current$branch", true, "hl7.fhir.r6.core#current$branch")]
    [InlineData("hl7.fhir.r6.core#current$notExistantbranch", false)]
    [InlineData("hl7.fhir.r6#current$branch", true, "hl7.fhir.r6.core#current$branch")]
    [InlineData("hl7.fhir.r4.examples#4.0.1", true, "hl7.fhir.r4.examples#4.0.1")]
    [InlineData("hl7.fhir.r4.notcore#4.0.1", false)]
    [InlineData("hl7.fhir.r4.core", true, "hl7.fhir.r4.core#4.0.1")]
    [InlineData("hl7.fhir.r4.core#invalid", false)]
    [InlineData("hl7.fhir.uv.ig#invalid", false)]
    [InlineData("example.org.ig#notsemver", true, "example.org.ig#notsemver")]
    [InlineData("hl7.fhir.r4.core#4", false)]
    [InlineData("hl7.fhir.r4#4", false)]
    [InlineData("too#many#segments", false)]
    [InlineData("hl7.fhir.uv.subscriptions-backport#latest", true, "hl7.fhir.uv.subscriptions-backport#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport#1.1.x", true, "hl7.fhir.uv.subscriptions-backport#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport#1.1", true, "hl7.fhir.uv.subscriptions-backport#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4#latest", true, "hl7.fhir.uv.subscriptions-backport.r4#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4#1.1.x", true, "hl7.fhir.uv.subscriptions-backport.r4#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4#1.1", true, "hl7.fhir.uv.subscriptions-backport.r4#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b#latest", true, "hl7.fhir.uv.subscriptions-backport.r4b#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b#1.1.x", true, "hl7.fhir.uv.subscriptions-backport.r4b#1.1.0")]
    [InlineData("hl7.fhir.uv.subscriptions-backport.r4b#1.1", true, "hl7.fhir.uv.subscriptions-backport.r4b#1.1.0")]
    [InlineData("hl7.fhir.uv.patient-corrections#dev", true, "hl7.fhir.uv.patient-corrections#dev")]
    internal void ResolveDirective(
        string directive,
        bool shouldSucceed,
        string expectedResolution = "")
    {
        bool success = _cache.TryResolveDirective(
            directive,
            out PackageCacheEntry? package);

        success.Should().Be(shouldSucceed);

        if (!success)
        {
            return;
        }

        package.Should().NotBeNull();

        if (package == null)
        {
            return;
        }

        package.Value.resolvedDirective.Should().BeEquivalentTo(expectedResolution);
    }
}

/// <summary>A cache resolution test fixture.</summary>
public class CacheResolutionTestFixture
{
    public PackageHttpMessageHandler _handler;

    public FhirCache _cache;

    //public static readonly string[] _packageDirs = new string[]
    //{
    //    "hl7.fhir.r4.core#4.0.1",
    //    "hl7.fhir.r4.core#current",
    //    "hl7.fhir.r4.expansions#4.0.1",
    //    "hl7.fhir.r4.examples#4.0.1",
    //    "hl7.fhir.r4.search#4.0.1",
    //    "example.org.ig#notsemver",
    //    "hl7.fhir.uv.subscriptions-backport#1.1.0",
    //    "hl7.fhir.uv.subscriptions-backport.r4#1.1.0",
    //    "hl7.fhir.uv.subscriptions-backport.r4b#1.1.0",
    //    "hl7.fhir.uv.patient-corrections#dev",
    //};

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheResolutionTestFixture"/> class.
    /// </summary>
    public CacheResolutionTestFixture()
    {
        _handler = new PackageHttpMessageHandler();

        //string cacheDir = Path.Combine(Directory.GetCurrentDirectory(), "data", ".fhir");

        //if (!Directory.Exists(cacheDir))
        //{
        //    Directory.CreateDirectory(cacheDir);
        //}

        //foreach (string pDir in _packageDirs)
        //{
        //    string packageDir = Path.Combine(cacheDir, "packages", pDir);

        //    if (!Directory.Exists(packageDir))
        //    {
        //        Directory.CreateDirectory(packageDir);
        //    }
        //}

        _cache = new FhirCache(Path.Combine(Directory.GetCurrentDirectory(), "data", ".fhir"), null, null);
        FhirCache._httpClient = new(_handler);
        //_cache._httpClient = new(_handler);
    }
}
