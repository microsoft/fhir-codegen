// <copyright file="GenerationTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FluentAssertions;
using Microsoft.Health.Fhir.CodeGen.Lanugage.Info;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Tests.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

public class GenerationTestFixture
{
    /// <summary>The cache.</summary>
    public IFhirPackageClient _cache;

    /// <summary>The FHIR R5 package entry.</summary>
    public PackageCacheEntry _r5;
    public PackageCacheEntry _extensionsR5;

    //public PackageCacheEntry _r4;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationTestFixture"/> class.
    /// </summary>
    public GenerationTestFixture()
    {
        _cache = FhirCache.Create(new FhirPackageClientSettings()
        {
            CachePath = "~/.fhir",
        });

        _r5 = Load("hl7.fhir.r5.core#5.0.0");
        _extensionsR5 = Load("hl7.fhir.uv.extensions#1.0.0");

        //_r4 = Load("hl7.fhir.r4.core.as.r5#current");
    }

    /// <summary>Loads.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="directive">The directive to load.</param>
    /// <returns>A PackageCacheEntry.</returns>
    private PackageCacheEntry Load(string directive)
    {
        PackageCacheEntry? p = _cache.FindOrDownloadPackageByDirective(directive, false).Result;

        if (p == null)
        {
            throw new Exception($"Failed to load {directive}");
        }

        return (PackageCacheEntry)p;
    }
}
public class GenerationTests : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The cache.</summary>
    private readonly IFhirPackageClient _cache;

    /// <summary>(Immutable) The FHIR R5 core package.</summary>
    private readonly PackageCacheEntry _r5;

    /// <summary>(Immutable) The FHIR R5 extensions package.</summary>
    private readonly PackageCacheEntry _extensionsR5;

    public GenerationTests(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _cache = fixture._cache;
        _r5 = fixture._r5;
        _extensionsR5 = fixture._extensionsR5;
    }

    [Theory]
    [FileData("TestData/Generated/Info-R5.txt")]
    internal async void TestInfoR5(string previous)
    {
        PackageLoader loader = new(_cache);

        DefinitionCollection? loaded = await loader.LoadPackages(_r5.Name, new[] { _r5, _extensionsR5 });

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        LangInfo exportLang = new();

        LangInfo.InfoOptions options = new();

        using (MemoryStream ms = new())
        {
            exportLang.Export(options, loaded, ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using (StreamReader sr = new(ms))
            {
                string current = sr.ReadToEnd();

                File.WriteAllText("TestData/Generated/Info-R5.txt", current);

                current.Should().Be(previous);
            }
        }

    }
}
