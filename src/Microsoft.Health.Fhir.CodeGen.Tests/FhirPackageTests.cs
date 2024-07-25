// <copyright file="FhirPackageTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

/// <summary>A FHIR package test fixture.</summary>
public class FhirPackageTestFixture
{
    public readonly string? CachePath = null;

    /// <summary>The FHIR R5 package entries.</summary>
    public readonly string[] EntriesR5 =
    [
        "hl7.fhir.r5.core#5.0.0",
        "hl7.fhir.r5.expansions#5.0.0",
        "hl7.fhir.uv.extensions#1.0.0",
    ];


    /// <summary>The FHIR R4B package entries.</summary>
    public readonly string[] EntriesR4B =
    [
        "hl7.fhir.r4b.core#4.3.0",
        "hl7.fhir.r4b.expansions#4.3.0",
        "hl7.fhir.uv.extensions#1.0.0",
    ];

    /// <summary>The FHIR R4 package entries.</summary>
    public readonly string[] EntriesR4 =
    [
        "hl7.fhir.r4.core#4.0.1",
        "hl7.fhir.r4.expansions#4.0.1",
        "hl7.fhir.uv.extensions#1.0.0",
    ];

    /// <summary>The FHIR STU3 package entries.</summary>
    public readonly string[] EntriesR3 =
    [
        "hl7.fhir.r3.core#3.0.2",
        "hl7.fhir.r3.expansions#3.0.2",
    ];

    /// <summary>The FHIR DSTU2 package entries.</summary>
    public readonly string[] EntriesR2 =
    [
        "hl7.fhir.r2.core#1.0.2",
        "hl7.fhir.r2.expansions#1.0.2",
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestFixture"/> class.
    /// </summary>
    public FhirPackageTestFixture()
    {
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
    private const int _countResourcesByName = 160;              // 157 concrete resources, 3 abstract
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
    [Trait("Category", "Parse")]
    [Trait("Format", "Json")]
    [Trait("FhirVersion", "R5")]
    internal async Task ParseCorePackage(LoaderOptions.JsonDeserializationModel jsonModel)
    {
        PackageLoader loader = new(new() { FhirCacheDirectory = _fixture.CachePath }, new() { JsonModel = jsonModel });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR5);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl, string.Join("\n", loaded.CodeSystemsByUrl.Keys.OrderBy(v => v)));
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl, string.Join("\n", loaded.ValueSetsByVersionedUrl.Keys.OrderBy(v => v)));
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName, string.Join("\n", loaded.PrimitiveTypesByName.Keys.OrderBy(v => v)));
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName, string.Join("\n", loaded.ComplexTypesByName.Keys.OrderBy(v => v)));
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName, string.Join("\n", loaded.ResourcesByName.Keys.OrderBy(v => v)));
        loaded.LogicalModelsByUrl.Should().HaveCount(_countLogicalModelsByName, string.Join("\n", loaded.LogicalModelsByUrl.Keys.OrderBy(v => v)));
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl, string.Join("\n", loaded.ExtensionsByUrl.Keys.OrderBy(v => v)));
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl, string.Join("\n", loaded.ProfilesByUrl.Keys.OrderBy(v => v)));
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl, string.Join("\n", loaded.OperationsByUrl.Keys.OrderBy(v => v)));
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl, string.Join("\n", loaded.CapabilityStatementsByUrl.Keys.OrderBy(v => v)));
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl, string.Join("\n", loaded.ImplementationGuidesByUrl.Keys.OrderBy(v => v)));
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl, string.Join("\n", loaded.CompartmentsByUrl.Keys.OrderBy(v => v)));
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
    [Trait("Category", "Parse")]
    [Trait("Format", "Json")]
    [Trait("FhirVersion", "R4B")]
    internal async Task ParseCorePackage()
    {
        PackageLoader loader = new(new() { FhirCacheDirectory = _fixture.CachePath }, new() { JsonModel = LoaderOptions.JsonDeserializationModel.Default });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR4B);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl, string.Join("\n", loaded.CodeSystemsByUrl.Keys.OrderBy(v => v)));
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl, string.Join("\n", loaded.ValueSetsByVersionedUrl.Keys.OrderBy(v => v)));
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName, string.Join("\n", loaded.PrimitiveTypesByName.Keys.OrderBy(v => v)));
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName, string.Join("\n", loaded.ComplexTypesByName.Keys.OrderBy(v => v)));
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName, string.Join("\n", loaded.ResourcesByName.Keys.OrderBy(v => v)));
        loaded.LogicalModelsByUrl.Should().HaveCount(_countLogicalModelsByName, string.Join("\n", loaded.LogicalModelsByUrl.Keys.OrderBy(v => v)));
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl, string.Join("\n", loaded.ExtensionsByUrl.Keys.OrderBy(v => v)));
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl, string.Join("\n", loaded.ProfilesByUrl.Keys.OrderBy(v => v)));
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl, string.Join("\n", loaded.OperationsByUrl.Keys.OrderBy(v => v)));
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl, string.Join("\n", loaded.CapabilityStatementsByUrl.Keys.OrderBy(v => v)));
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl, string.Join("\n", loaded.ImplementationGuidesByUrl.Keys.OrderBy(v => v)));
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl, string.Join("\n", loaded.CompartmentsByUrl.Keys.OrderBy(v => v)));
    }
}

public class FhirPackageTestsR4 : IClassFixture<FhirPackageTestFixture>
{
    private const int _countCodeSystemsByUrl = 1090;
    private const int _countValueSetsByUrl = 1377;
    private const int _countPrimitiveTypesByName = 20;
    private const int _countComplexTypesByName = 41;
    private const int _countResourcesByName = 148;
    private const int _countLogicalModelsByName = 5;
    private const int _countExtensionsByUrl = 561;
    private const int _countProfilesByUrl = 48;
    private const int _countSearchParametersByUrl = 1410;
    private const int _countOperationsByUrl = 47;
    private const int _countCapabilityStatementsByUrl = 8;
    private const int _countImplementationGuidesByUrl = 1;
    private const int _countCompartmentsByUrl = 6;

    /// <summary>
    /// The test output helper.
    /// </summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly FhirPackageTestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestsR4"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    public FhirPackageTestsR4(FhirPackageTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    /// <summary>
    /// Parses the core package.
    /// </summary>
    /// <param name="jsonModel">The JSON deserialization model.</param>
    [Fact]
    [Trait("Category", "Parse")]
    [Trait("Format", "Json")]
    [Trait("FhirVersion", "R4")]
    internal async Task ParseCorePackage()
    {
        PackageLoader loader = new(new() { FhirCacheDirectory = _fixture.CachePath }, new() { JsonModel = LoaderOptions.JsonDeserializationModel.Default });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR4);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl, string.Join("\n", loaded.CodeSystemsByUrl.Keys.OrderBy(v => v)));
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl, string.Join("\n", loaded.ValueSetsByVersionedUrl.Keys.OrderBy(v => v)));
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName, string.Join("\n", loaded.PrimitiveTypesByName.Keys.OrderBy(v => v)));
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName, string.Join("\n", loaded.ComplexTypesByName.Keys.OrderBy(v => v)));
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName, string.Join("\n", loaded.ResourcesByName.Keys.OrderBy(v => v)));
        loaded.LogicalModelsByUrl.Should().HaveCount(_countLogicalModelsByName, string.Join("\n", loaded.LogicalModelsByUrl.Keys.OrderBy(v => v)));
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl, string.Join("\n", loaded.ExtensionsByUrl.Keys.OrderBy(v => v)));
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl, string.Join("\n", loaded.ProfilesByUrl.Keys.OrderBy(v => v)));
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl, string.Join("\n", loaded.OperationsByUrl.Keys.OrderBy(v => v)));
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl, string.Join("\n", loaded.CapabilityStatementsByUrl.Keys.OrderBy(v => v)));
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl, string.Join("\n", loaded.ImplementationGuidesByUrl.Keys.OrderBy(v => v)));
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl, string.Join("\n", loaded.CompartmentsByUrl.Keys.OrderBy(v => v)));
    }
}

public class FhirPackageTestsR3 : IClassFixture<FhirPackageTestFixture>
{
    private const int _countCodeSystemsByUrl = 941;
    private const int _countValueSetsByUrl = 1154;
    private const int _countPrimitiveTypesByName = 18;
    private const int _countComplexTypesByName = 35;
    private const int _countResourcesByName = 119;
    private const int _countLogicalModelsByName = 4;
    private const int _countExtensionsByUrl = 376;
    private const int _countProfilesByUrl = 33;
    private const int _countSearchParametersByUrl = 1247;
    private const int _countOperationsByUrl = 37;
    private const int _countCapabilityStatementsByUrl = 8;
    private const int _countImplementationGuidesByUrl = 2;
    private const int _countCompartmentsByUrl = 6;

    /// <summary>
    /// The test output helper.
    /// </summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly FhirPackageTestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestsR3"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    public FhirPackageTestsR3(FhirPackageTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    /// <summary>
    /// Parses the core package.
    /// </summary>
    /// <param name="jsonModel">The JSON deserialization model.</param>
    [Fact]
    [Trait("Category", "Parse")]
    [Trait("Format", "Json")]
    [Trait("FhirVersion", "R3")]
    internal async Task ParseCorePackage()
    {
        PackageLoader loader = new(new() { FhirCacheDirectory = _fixture.CachePath }, new() { JsonModel = LoaderOptions.JsonDeserializationModel.Default });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR3);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl, string.Join("\n", loaded.CodeSystemsByUrl.Keys.OrderBy(v => v)));
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl, string.Join("\n", loaded.ValueSetsByVersionedUrl.Keys.OrderBy(v => v)));
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName, string.Join("\n", loaded.PrimitiveTypesByName.Keys.OrderBy(v => v)));
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName, string.Join("\n", loaded.ComplexTypesByName.Keys.OrderBy(v => v)));
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName, string.Join("\n", loaded.ResourcesByName.Keys.OrderBy(v => v)));
        loaded.LogicalModelsByUrl.Should().HaveCount(_countLogicalModelsByName, string.Join("\n", loaded.LogicalModelsByUrl.Keys.OrderBy(v => v)));
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl, string.Join("\n", loaded.ExtensionsByUrl.Keys.OrderBy(v => v)));
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl, string.Join("\n", loaded.ProfilesByUrl.Keys.OrderBy(v => v)));
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl, string.Join("\n", loaded.OperationsByUrl.Keys.OrderBy(v => v)));
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl, string.Join("\n", loaded.CapabilityStatementsByUrl.Keys.OrderBy(v => v)));
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl, string.Join("\n", loaded.ImplementationGuidesByUrl.Keys.OrderBy(v => v)));
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl, string.Join("\n", loaded.CompartmentsByUrl.Keys.OrderBy(v => v)));
    }
}

public class FhirPackageTestsR2 : IClassFixture<FhirPackageTestFixture>
{
    private const int _countCodeSystemsByUrl = 822;     // note - there are zero in the definitions, the 822 are contained in VS resources
    private const int _countValueSetsByUrl = 1016;
    private const int _countPrimitiveTypesByName = 18;
    private const int _countComplexTypesByName = 28;
    private const int _countResourcesByName = 96;
    private const int _countLogicalModelsByName = 0;
    private const int _countExtensionsByUrl = 285;
    private const int _countProfilesByUrl = 107;
    private const int _countSearchParametersByUrl = 907;
    private const int _countOperationsByUrl = 18;
    private const int _countCapabilityStatementsByUrl = 5;
    private const int _countImplementationGuidesByUrl = 0;
    private const int _countCompartmentsByUrl = 0;

    /// <summary>
    /// The test output helper.
    /// </summary>
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly FhirPackageTestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPackageTestsR2"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    public FhirPackageTestsR2(FhirPackageTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    /// <summary>
    /// Parses the core package.
    /// </summary>
    /// <param name="jsonModel">The JSON deserialization model.</param>
    [Fact]
    [Trait("Category", "Parse")]
    [Trait("Format", "Json")]
    [Trait("FhirVersion", "R2")]
    internal async Task ParseCorePackage()
    {
        PackageLoader loader = new(new() { FhirCacheDirectory = _fixture.CachePath }, new() { JsonModel = LoaderOptions.JsonDeserializationModel.Default });

        DefinitionCollection? loaded = await loader.LoadPackages(_fixture.EntriesR2);

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            return;
        }

        loaded.CodeSystemsByUrl.Should().HaveCount(_countCodeSystemsByUrl, string.Join("\n", loaded.CodeSystemsByUrl.Keys.OrderBy(v => v)));
        loaded.ValueSetsByVersionedUrl.Should().HaveCount(_countValueSetsByUrl, string.Join("\n", loaded.ValueSetsByVersionedUrl.Keys.OrderBy(v => v)));
        loaded.PrimitiveTypesByName.Should().HaveCount(_countPrimitiveTypesByName, string.Join("\n", loaded.PrimitiveTypesByName.Keys.OrderBy(v => v)));
        loaded.ComplexTypesByName.Should().HaveCount(_countComplexTypesByName, string.Join("\n", loaded.ComplexTypesByName.Keys.OrderBy(v => v)));
        loaded.ResourcesByName.Should().HaveCount(_countResourcesByName, string.Join("\n", loaded.ResourcesByName.Keys.OrderBy(v => v)));
        loaded.LogicalModelsByUrl.Should().HaveCount(_countLogicalModelsByName, string.Join("\n", loaded.LogicalModelsByUrl.Keys.OrderBy(v => v)));
        loaded.ExtensionsByUrl.Should().HaveCount(_countExtensionsByUrl, string.Join("\n", loaded.ExtensionsByUrl.Keys.OrderBy(v => v)));
        loaded.ProfilesByUrl.Should().HaveCount(_countProfilesByUrl, string.Join("\n", loaded.ProfilesByUrl.Keys.OrderBy(v => v)));
        loaded.SearchParametersByUrl.Should().HaveCount(_countSearchParametersByUrl);
        loaded.OperationsByUrl.Should().HaveCount(_countOperationsByUrl, string.Join("\n", loaded.OperationsByUrl.Keys.OrderBy(v => v)));
        loaded.CapabilityStatementsByUrl.Should().HaveCount(_countCapabilityStatementsByUrl, string.Join("\n", loaded.CapabilityStatementsByUrl.Keys.OrderBy(v => v)));
        loaded.ImplementationGuidesByUrl.Should().HaveCount(_countImplementationGuidesByUrl, string.Join("\n", loaded.ImplementationGuidesByUrl.Keys.OrderBy(v => v)));
        loaded.CompartmentsByUrl.Should().HaveCount(_countCompartmentsByUrl, string.Join("\n", loaded.CompartmentsByUrl.Keys.OrderBy(v => v)));
    }
}
