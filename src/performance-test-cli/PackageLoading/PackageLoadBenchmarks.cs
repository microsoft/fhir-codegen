// <copyright file="PackageLoadBenchmarks.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;

namespace performance_test_cli.PackageLoading;

/// <summary>
/// Represents a class for benchmarking package loading.
/// </summary>
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[MemoryDiagnoser]
public class PackageLoadBenchmarks
{
    private IFhirPackageClient _cache;
    private PackageLoader _loader = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageLoadBenchmarks"/> class.
    /// </summary>
    public PackageLoadBenchmarks()
    {
        _cache = FhirCache.Create(new FhirPackageClientSettings()
        {
            CachePath = "~/.fhir",
        });
    }

    /// <summary>
    /// Gets all test directives.
    /// </summary>
    public static IEnumerable<IEnumerable<string>> AllTestDirectives => new List<string[]>()
        {
            new string[] { "hl7.fhir.r5.core#5.0.0", "hl7.fhir.r5.expansions#5.0.0", "hl7.fhir.uv.extensions#1.0.0" },
        };

    /// <summary>
    /// Gets or sets the test directives.
    /// </summary>
    [ParamsSource(nameof(AllTestDirectives))]
    public IEnumerable<string> TestDirectives { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the test entries.
    /// </summary>
    public PackageCacheEntry[] TestEntries { get; set; } = Array.Empty<PackageCacheEntry>();

    /// <summary>
    /// Loads the packages with POCO deserialization model.
    /// </summary>
    /// <returns>The loaded definition collection.</returns>
    [BenchmarkCategory("Load")]
    [Benchmark(Baseline = true)]
    public object? LoadWithPoco()
    {
        if (_loader == null)
        {
            throw new Exception("Loader not initialized");
        }

        if (TestEntries.Length == 0)
        {
            throw new Exception("No test entries");
        }

        DefinitionCollection? loaded = _loader.LoadPackages(TestEntries[0].Name, TestEntries);

        return loaded;
    }

    /// <summary>
    /// Loads the packages with System.Text.Json deserialization model.
    /// </summary>
    /// <returns>The loaded definition collection.</returns>
    [BenchmarkCategory("Load")]
    [Benchmark]
    public object? LoadWithSystemTextJson()
    {
        if (_loader == null)
        {
            throw new Exception("Loader not initialized");
        }

        if (TestEntries.Length == 0)
        {
            throw new Exception("No test entries");
        }

        DefinitionCollection? loaded = _loader.LoadPackages(TestEntries[0].Name, TestEntries);

        return loaded;
    }

    /// <summary>
    /// Performs the common setup for package loading.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    public void CommonSetup()
    {
        // traverse the directives for this run and ensure the packages are downloaded and available
        foreach (string directive in TestDirectives)
        {
            PackageCacheEntry? p = _cache.FindOrDownloadPackageByDirective(directive, false).Result;

            if (p == null)
            {
                throw new Exception($"Failed to load {directive}");
            }

            TestEntries = TestEntries.Append((PackageCacheEntry)p).ToArray();
        }
    }

    /// <summary>
    /// Performs the setup for benchmarking with POCO deserialization model.
    /// </summary>
    [GlobalSetup(Targets = new[] { nameof(LoadWithPoco) })]
    public void SetupPoco()
    {
        CommonSetup();

        _loader = new(_cache, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.Poco,
        });
    }

    /// <summary>
    /// Performs the setup for benchmarking with System.Text.Json deserialization model.
    /// </summary>
    [GlobalSetup(Targets = new[] { nameof(LoadWithSystemTextJson) })]
    public void SetupSystemTextJson()
    {
        CommonSetup();

        _loader = new(_cache, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
        });
    }

    /// <summary>
    /// Performs the global cleanup.
    /// </summary>
    [GlobalCleanup]
    public void GlobalCleanup()
    {
    }
}
