// <copyright file="PackageLoadBenchmarks.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace performance_test_cli.PackageLoading;

/// <summary>
/// Represents a class for benchmarking package loading.
/// </summary>
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[MemoryDiagnoser]
public class PackageLoadBenchmarks
{
    public readonly string? CachePath = null;

    private PackageLoader _loader = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageLoadBenchmarks"/> class.
    /// </summary>
    public PackageLoadBenchmarks()
    {
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
    public string[] TestDirectives { get; set; } = [];

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

        if (TestDirectives.Length == 0)
        {
            throw new Exception("No test entries");
        }

        DefinitionCollection? loaded = _loader.LoadPackages(TestDirectives).Result;

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

        if (TestDirectives.Length == 0)
        {
            throw new Exception("No test entries");
        }

        DefinitionCollection? loaded = _loader.LoadPackages(TestDirectives).Result;

        return loaded;
    }

    /// <summary>
    /// Performs the common setup for package loading.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    public void CommonSetup()
    {
        //// traverse the directives for this run and ensure the packages are downloaded and available
        //foreach (string directive in TestDirectives)
        //{
        //    PackageCacheEntry? p = _cache.FindOrDownloadPackageByDirective(directive, false).Result;

        //    if (p == null)
        //    {
        //        throw new Exception($"Failed to load {directive}");
        //    }

        //    TestEntries = [.. TestEntries, (PackageCacheEntry)p];
        //}
    }

    /// <summary>
    /// Performs the setup for benchmarking with POCO deserialization model.
    /// </summary>
    [GlobalSetup(Targets = [nameof(LoadWithPoco)])]
    public void SetupPoco()
    {
        CommonSetup();

        _loader = new(new()
        {
            CachePath = CachePath,
            JsonModel = LoaderOptions.JsonDeserializationModel.Poco,
        });
    }

    /// <summary>
    /// Performs the setup for benchmarking with System.Text.Json deserialization model.
    /// </summary>
    [GlobalSetup(Targets = [nameof(LoadWithSystemTextJson)])]
    public void SetupSystemTextJson()
    {
        CommonSetup();

        _loader = new(new()
        {
            CachePath = CachePath,
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
