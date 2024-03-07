// <copyright file="FhirPackageTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using FluentAssertions;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using Microsoft.Health.Fhir.PackageManager.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

/// <summary>A FHIR package test fixture.</summary>
public class FhirPackageTestFixture
{
    /// <summary>The cache.</summary>
    public IFhirPackageClient Cache;

    /// <summary>The FHIR R5 package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR5;

    /// <summary>The FHIR R4B package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR4B;

    //public PackageCacheEntry _r4;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestFixture"/> class.
    /// </summary>
    public FhirPackageTestFixture()
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

/// <summary>
/// Represents a test fixture for FHIR package tests.
/// </summary>
public class FhirPackageTestsR5 : IClassFixture<FhirPackageTestFixture>
{
    private const int _countCodeSystemsByUrl = 485;             // 448 in core, 485 in +extensions
    private const int _countValueSetsByUrl = 887;               // 788 in core, 848 in +extensions, 887 in +expansions
    private const int _countPrimitiveTypesByName = 21;
    private const int _countComplexTypesByName = 48;
    private const int _countResourcesByName = 162;
    private const int _countLogicalModelsByName = 10;
    private const int _countExtensionsByUrl = 512;              // 0 in core, 512 in +extensions
    private const int _countProfilesByUrl = 66;
    private const int _countSearchParametersByUrl = 1263;       // 1244 in core, 1263 in +extensions
    private const int _countOperationsByUrl = 61;
    private const int _countCapabilityStatementsByUrl = 6;
    private const int _countImplementationGuidesByUrl = 3;      // 2 in core, 3 in +extensions
    private const int _countCompartmentsByUrl = 6;

    /// <summary>
    /// The test output helper.
    /// </summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly FhirPackageTestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestsR5"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    public FhirPackageTestsR5(FhirPackageTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    /// <summary>
    /// Parses the core package.
    /// </summary>
    /// <param name="jsonModel">The JSON deserialization model.</param>
    [Theory]
    [InlineData(LoaderOptions.JsonDeserializationModel.Poco)]
    [InlineData(LoaderOptions.JsonDeserializationModel.SystemTextJson)]
    internal async void ParseCorePackage(LoaderOptions.JsonDeserializationModel jsonModel)
    {
        PackageLoader loader = new(_fixture.Cache, new() { JsonModel = jsonModel });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR5.First().Name, _fixture.EntriesR5);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl);
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl);
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName);
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName);
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName);
        loaded.LogicalModelsByName.Should().HaveCount(_countLogicalModelsByName);
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl);
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl);
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl);
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl);
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl);
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl);
    }
}

public class FhirPackageTestsR4B : IClassFixture<FhirPackageTestFixture>
{
    private const int _countCodeSystemsByUrl = 565;
    private const int _countValueSetsByUrl = 805;
    private const int _countPrimitiveTypesByName = 20;
    private const int _countComplexTypesByName = 43;
    private const int _countResourcesByName = 143;
    private const int _countLogicalModelsByName = 4;
    private const int _countExtensionsByUrl = 559;
    private const int _countProfilesByUrl = 43;
    private const int _countSearchParametersByUrl = 1444;
    private const int _countOperationsByUrl = 47;
    private const int _countCapabilityStatementsByUrl = 6;
    private const int _countImplementationGuidesByUrl = 2;
    private const int _countCompartmentsByUrl = 6;

    /// <summary>
    /// The test output helper.
    /// </summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly FhirPackageTestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestsR5"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    public FhirPackageTestsR4B(FhirPackageTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    /// <summary>
    /// Parses the core package.
    /// </summary>
    /// <param name="jsonModel">The JSON deserialization model.</param>
    [Fact]
    internal async void ParseCorePackage()
    {
        PackageLoader loader = new(_fixture.Cache, new() { JsonModel = LoaderOptions.JsonDeserializationModel.Default });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR4B.First().Name, _fixture.EntriesR4B);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl);
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl);
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName);
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName);
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName);
        loaded.LogicalModelsByName.Should().HaveCount(_countLogicalModelsByName);
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl);
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl);
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl);
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl);
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl);
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl);
    }
}
