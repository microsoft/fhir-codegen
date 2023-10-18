// <copyright file="UnitTest1.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.PackageManager;
using Microsoft.Health.Fhir.PackageManager.Models;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

///// <summary>A FHIR cache tests.</summary>
//public class FhirCacheTestFixture
//{
//    public HttpMessageHandler _handler;

//    public FhirCache _cache;

//    /// <summary>
//    /// Initializes a new instance of the
//    /// Microsoft.Health.Fhir.PackageManager.Tests.FhirCacheTestFixture class.
//    /// </summary>
//    public FhirCacheTestFixture()
//    {
//        _handler = new PackageHttpMessageHandler();
//        _cache = new FhirCache(Path.GetRelativePath(Directory.GetCurrentDirectory(), "data/.fhir"), null, null);
//        _cache._httpClient = new(_handler);
//    }
//}

///// <summary>A FHIR cache tests.</summary>
//public class FhirCacheTests : IClassFixture<FhirCacheTestFixture>
//{
//    /// <summary>(Immutable) The test output helper.</summary>
//    private readonly ITestOutputHelper _testOutputHelper;

//    /// <summary>(Immutable) The cache.</summary>
//    private readonly FhirCache _cache;

//    /// <summary>
//    /// Initializes a new instance of the <see cref="FhirCacheTests"/> class.
//    /// </summary>
//    /// <param name="fixture">         The fixture.</param>
//    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
//    public FhirCacheTests(FhirCacheTestFixture fixture, ITestOutputHelper testOutputHelper)
//    {
//        _cache = fixture._cache;
//        _testOutputHelper = testOutputHelper;
//    }
//}
