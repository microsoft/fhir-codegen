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
    public IFhirPackageClient Cache;

    /// <summary>The FHIR R5 package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR5;

    /// <summary>The FHIR R4B package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR4B;

    //public PackageCacheEntry _r4;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationTestFixture"/> class.
    /// </summary>
    public GenerationTestFixture()
    {
        Cache = FhirCache.Create(new FhirPackageClientSettings()
        {
            CachePath = "~/.fhir",
        });

        EntriesR5 = new List<PackageCacheEntry>()
        {
            Load("hl7.fhir.r5.core#5.0.0"),
            Load("hl7.fhir.r5.expansions#5.0.0"),
            Load("hl7.fhir.uv.extensions#1.0.0"),
        };

        EntriesR4B = new List<PackageCacheEntry>()
        {
            Load("hl7.fhir.r4b.core#4.3.0"),
            Load("hl7.fhir.r4b.expansions#4.3.0"),
            Load("hl7.fhir.uv.extensions#1.0.0"),
        };
    }

    /// <summary>Loads.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="directive">The directive to load.</param>
    /// <returns>A PackageCacheEntry.</returns>
    private PackageCacheEntry Load(string directive)
    {
        PackageCacheEntry? p = Cache.FindOrDownloadPackageByDirective(directive, false).Result;

        if (p == null)
        {
            throw new Exception($"Failed to load {directive}");
        }

        return (PackageCacheEntry)p;
    }
}

public class GenerationTestsR5 : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly GenerationTestFixture _fixture;

    public GenerationTestsR5(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Theory]
    [FileData("TestData/Generated/Info-R5.txt")]
    internal async void TestInfoR5(string previous)
    {
        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR5.First().Name, _fixture.EntriesR5);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        // set the allowed terminology servers
        //loaded.TxServers = new[] { "http://tx.fhir.org/r5" };

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

                // update th current file contents (manual)
                File.WriteAllText("TestData/Generated/Info-R5.txt", current);

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(previous);
            }
        }

    }
}

public class GenerationTestsR4B : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly GenerationTestFixture _fixture;

    public GenerationTestsR4B(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Theory]
    [FileData("TestData/Generated/Info-R4B.txt")]
    internal async void TestInfoR4B(string previous)
    {
        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR4B.First().Name, _fixture.EntriesR4B);

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

                // update th current file contents (manual)
                File.WriteAllText("TestData/Generated/Info-R4B.txt", current);

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(previous);
            }
        }

    }
}
