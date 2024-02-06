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

namespace Microsoft.Health.Fhir.CodeGen.Tests;

/// <summary>A FHIR package test fixture.</summary>
public class FhirPackageTestFixture
{
    /// <summary>The cache.</summary>
    public IFhirPackageClient _cache;

    /// <summary>The FHIR R5 package entry.</summary>
    public PackageCacheEntry _r5;
    public PackageCacheEntry _extensionsR5;

    //public PackageCacheEntry _r4;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestFixture"/> class.
    /// </summary>
    public FhirPackageTestFixture()
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

/// <summary>A FHIR package tests.</summary>
public class FhirPackageTestsR5 : IClassFixture<FhirPackageTestFixture>
{
    private const int _countCodeSystemsByUrl = 485;             // 448 in core, 485 in core+extensions
    private const int _countValueSetsByUrl = 848;               // 788 in core, 848 in core+extensions
    private const int _countPrimitiveTypesByName = 21;
    private const int _countComplexTypesByName = 48;
    private const int _countResourcesByName = 162;
    private const int _countLogicalModelsByName = 10;
    private const int _countExtensionsByUrl = 512;              // 0 in core, 512 in core+extensions
    private const int _countProfilesByUrl = 66;
    private const int _countSearchParametersByUrl = 1263;       // 1244 in core, 1263 in core+extensions
    private const int _countOperationsByUrl = 61;
    private const int _countCapabilityStatementsByUrl = 6;
    private const int _countImplementationGuidesByUrl = 3;      // 2 in core, 3 in core+extensions
    private const int _countCompartmentsByUrl = 6;

    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The cache.</summary>
    private readonly IFhirPackageClient _cache;

    /// <summary>(Immutable) The FHIR R5 core package.</summary>
    private readonly PackageCacheEntry _r5;

    /// <summary>(Immutable) The FHIR R5 extensions package.</summary>
    private readonly PackageCacheEntry _extensionsR5;

    /// <summary>
    /// Initializes a new instance of the Microsoft.Health.Fhir.CodeGen.Tests.FhirPackageTests
    /// class.
    /// </summary>
    /// <param name="fixture">         The fixture.</param>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public FhirPackageTestsR5(FhirPackageTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cache = fixture._cache;
        _testOutputHelper = testOutputHelper;
        _r5 = fixture._r5;
        _extensionsR5 = fixture._extensionsR5;
    }

    /// <summary>Parse core package.</summary>
    [Fact]
    internal async void ParseCorePackage()
    {
        PackageLoader loader = new(_cache);

        DefinitionCollection? loaded = await loader.LoadPackages(_r5.Name, new[] { _r5, _extensionsR5 });

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl);
        loaded.ValueSetsByUrl.Should().HaveCount(_countValueSetsByUrl);
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
