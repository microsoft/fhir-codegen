// <copyright file="GenerationTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Language.Info;
using Microsoft.Health.Fhir.CodeGen.Language.OpenApi;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Tests.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

public class GenerationTestFixture
{
    /// <summary>True to write generated files.</summary>
    public static bool WriteGeneratedFiles = false;

    /// <summary>The package loader.</summary>
    public PackageLoader? Loader = null;

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
    /// Initializes a new instance of the <see cref="GenerationTestFixture"/> class.
    /// </summary>
    public GenerationTestFixture()
    {
    }
}

public class GenerationTestsR5 : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The fixture.</summary>
    private readonly GenerationTestFixture _fixture;

    /// <summary>The loaded.</summary>
    private DefinitionCollection _loaded = null!;

    public GenerationTestsR5(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        _fixture.Loader = new(new() { FhirCacheDirectory = _fixture.CachePath });

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(_fixture.EntriesR5).Result;

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            throw new Exception("Failed to load R5");
        }

        _loaded = loaded;
    }

    [Theory]
    [InlineData("Info", "TestData/Generated/Info-R5.txt")]
    [InlineData("TypeScript", "TestData/Generated/TypeScript-R5.ts")]
    [Trait("Category", "Generation")]
    [Trait("FhirVersion", "R5")]
    internal void TestLangR5(string langName, string filePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        string data = File.ReadAllText(path);

        using (MemoryStream ms = new())
        {
            switch (langName)
            {
                case "Info":
                    {
                        LangInfo exportLang = new();
                        LangInfo.InfoOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new()
                        {
                            WriteStream = ms,
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown language: {langName}");
            }


            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using (StreamReader sr = new(ms))
            {
                string current = sr.ReadToEnd();

                // update the current file contents (manual)
                if (GenerationTestFixture.WriteGeneratedFiles)
                {
                    File.WriteAllText(filePath, current);
                    Assert.Fail("Generated files updated, please re-run the test");
                }

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(data);
            }
        }
    }
}

public class GenerationTestsR4B : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The fixture.</summary>
    private readonly GenerationTestFixture _fixture;

    /// <summary>The loaded.</summary>
    private DefinitionCollection _loaded = null!;

    public GenerationTestsR4B(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        _fixture.Loader = new(new() { FhirCacheDirectory = _fixture.CachePath });

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(_fixture.EntriesR4B).Result;

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            throw new Exception("Failed to load R4B");
        }

        _loaded = loaded;
    }

    [Theory]
    [InlineData("Info", "TestData/Generated/Info-R4B.txt")]
    [InlineData("TypeScript", "TestData/Generated/TypeScript-R4B.ts")]
    [Trait("Category", "Generation")]
    [Trait("FhirVersion", "R4B")]
    internal void TestLangR4B(string langName, string filePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        string data = File.ReadAllText(path);

        using (MemoryStream ms = new())
        {
            switch (langName)
            {
                case "Info":
                    {
                        LangInfo exportLang = new();
                        LangInfo.InfoOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown language: {langName}");
            }


            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using (StreamReader sr = new(ms))
            {
                string current = sr.ReadToEnd();

                // update the current file contents (manual)
                if (GenerationTestFixture.WriteGeneratedFiles)
                {
                    File.WriteAllText(filePath, current);
                    Assert.Fail("Generated files updated, please re-run the test");
                }

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(data);
            }
        }
    }
}

public class GenerationTestsR4 : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The fixture.</summary>
    private readonly GenerationTestFixture _fixture;

    /// <summary>The loaded.</summary>
    private DefinitionCollection _loaded = null!;

    public GenerationTestsR4(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        _fixture.Loader = new(new() { FhirCacheDirectory = _fixture.CachePath });

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(_fixture.EntriesR4).Result;

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            throw new Exception("Failed to load R4");
        }

        _loaded = loaded;
    }

    [Theory]
    [InlineData("Info", "TestData/Generated/Info-R4.txt", "")]
    [InlineData("TypeScript", "TestData/Generated/TypeScript-R4.ts", "")]
    //[InlineData("OpenApi-Json-Inline-None", "TestData/Generated/OpenApi-R4-Inline-None.json")]
    [InlineData("OpenApi-Json-Inline-None-Candle-Filtered", "TestData/Generated/OpenApi-R4-Inline-None-Candle-Filtered.json", "TestData/R4/CapabilityStatement-candle-local.json")]
    [InlineData("OpenApi-Yaml-Inline-None-Candle-Filtered", "TestData/Generated/OpenApi-R4-Inline-None-Candle-Filtered.yaml", "TestData/R4/CapabilityStatement-candle-local.json")]
    [InlineData("OpenApi-Yaml-Inline-Names-Candle-Filtered", "TestData/Generated/OpenApi-R4-Inline-Names-Candle-Filtered.yaml", "TestData/R4/CapabilityStatement-candle-local.json")]
    [InlineData("OpenApi-Yaml-Inline-Detailed-Candle-Filtered", "TestData/Generated/OpenApi-R4-Inline-Detailed-Candle-Filtered.yaml", "TestData/R4/CapabilityStatement-candle-local.json")]
    [Trait("Category", "Generation")]
    [Trait("FhirVersion", "R4")]
    internal void TestLangR4(string langName, string filePath, string csPath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        string data = File.ReadAllText(path);

        csPath = string.IsNullOrEmpty(csPath)
            ? string.Empty
            : Path.IsPathRooted(csPath)
                ? csPath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), csPath);

        using (MemoryStream ms = new())
        {
            switch (langName)
            {
                case "Info":
                    {
                        LangInfo exportLang = new();
                        LangInfo.InfoOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "OpenApi-Json-Inline-None":
                    {
                        // note that we can only use the WriteStream for OpenApi if MultiFile is off
                        OpenApiOptions options = new()
                        {
                            WriteStream = ms,
                            MultiFile = false,
                            OpenApiVersion = OpenApiCommon.OaVersion.v3,
                            FileFormat = OpenApiCommon.OaFileFormat.JSON,
                            SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.Inline,
                            SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.None,
                        };

                        LangOpenApi exportLang = new();
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "OpenApi-Json-Inline-None-Candle-Filtered":
                    {
                        if (string.IsNullOrEmpty(csPath))
                        {
                            throw new ArgumentException($"Missing csPath for {langName}");
                        }

                        object? csObj = _fixture.Loader?.ParseContents43("application/fhir+json", csPath);

                        CapabilityStatement? cs = csObj is CapabilityStatement c
                            ? c
                            : throw new ArgumentException("Failed to parse CapabilityStatement");

                        // note that we can only use the WriteStream for OpenApi if MultiFile is off
                        OpenApiOptions options = new()
                        {
                            WriteStream = ms,
                            MultiFile = false,
                            OpenApiVersion = OpenApiCommon.OaVersion.v3,
                            FileFormat = OpenApiCommon.OaFileFormat.JSON,
                            SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.Inline,
                            SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.None,
                            ServerCapabilities = cs,
                            ExportKeys = [ "Patient", "Observation" ]
                        };

                        LangOpenApi exportLang = new();
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "OpenApi-Yaml-Inline-None-Candle-Filtered":
                    {
                        if (string.IsNullOrEmpty(csPath))
                        {
                            throw new ArgumentException($"Missing csPath for {langName}");
                        }

                        object? csObj = _fixture.Loader?.ParseContents43("application/fhir+json", csPath);

                        CapabilityStatement? cs = csObj is CapabilityStatement c
                            ? c
                            : throw new ArgumentException("Failed to parse CapabilityStatement");

                        // note that we can only use the WriteStream for OpenApi if MultiFile is off
                        OpenApiOptions options = new()
                        {
                            WriteStream = ms,
                            MultiFile = false,
                            OpenApiVersion = OpenApiCommon.OaVersion.v3,
                            FileFormat = OpenApiCommon.OaFileFormat.YAML,
                            SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.Inline,
                            SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.None,
                            ServerCapabilities = cs,
                            ExportKeys = ["Patient", "Observation"]
                        };

                        LangOpenApi exportLang = new();
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "OpenApi-Yaml-Inline-Names-Candle-Filtered":
                    {
                        if (string.IsNullOrEmpty(csPath))
                        {
                            throw new ArgumentException($"Missing csPath for {langName}");
                        }

                        object? csObj = _fixture.Loader?.ParseContents43("application/fhir+json", csPath);

                        CapabilityStatement? cs = csObj is CapabilityStatement c
                            ? c
                            : throw new ArgumentException("Failed to parse CapabilityStatement");

                        // note that we can only use the WriteStream for OpenApi if MultiFile is off
                        OpenApiOptions options = new()
                        {
                            WriteStream = ms,
                            MultiFile = false,
                            OpenApiVersion = OpenApiCommon.OaVersion.v3,
                            FileFormat = OpenApiCommon.OaFileFormat.YAML,
                            SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.Inline,
                            SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.Names,
                            ServerCapabilities = cs,
                            ExportKeys = ["Patient", "Observation"]
                        };

                        LangOpenApi exportLang = new();
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "OpenApi-Yaml-Inline-Detailed-Candle-Filtered":
                    {
                        if (string.IsNullOrEmpty(csPath))
                        {
                            throw new ArgumentException($"Missing csPath for {langName}");
                        }

                        object? csObj = _fixture.Loader?.ParseContents43("application/fhir+json", csPath);

                        CapabilityStatement? cs = csObj is CapabilityStatement c
                            ? c
                            : throw new ArgumentException("Failed to parse CapabilityStatement");

                        // note that we can only use the WriteStream for OpenApi if MultiFile is off
                        OpenApiOptions options = new()
                        {
                            WriteStream = ms,
                            MultiFile = false,
                            OpenApiVersion = OpenApiCommon.OaVersion.v3,
                            FileFormat = OpenApiCommon.OaFileFormat.YAML,
                            SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.Inline,
                            SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.Detailed,
                            ServerCapabilities = cs,
                            ExportKeys = ["Patient", "Observation"]
                        };

                        LangOpenApi exportLang = new();
                        exportLang.Export(options, _loaded);
                    }
                    break;

                default:
                    throw new ArgumentException($"Unknown language: {langName}");
            }


            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using (StreamReader sr = new(ms))
            {
                string current = sr.ReadToEnd();

                // update the current file contents (manual)
                if (GenerationTestFixture.WriteGeneratedFiles)
                {
                    File.WriteAllText(filePath, current);
                    Assert.Fail("Generated files updated, please re-run the test");
                }

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(data);
            }
        }
    }
}

public class GenerationTestsR3 : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The fixture.</summary>
    private readonly GenerationTestFixture _fixture;

    /// <summary>The loaded.</summary>
    private DefinitionCollection _loaded = null!;

    public GenerationTestsR3(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        _fixture.Loader = new(new() { FhirCacheDirectory = _fixture.CachePath });

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(_fixture.EntriesR3).Result;

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            throw new Exception("Failed to load STU3");
        }

        _loaded = loaded;
    }

    [Theory]
    [InlineData("Info", "TestData/Generated/Info-R3.txt")]
    [InlineData("TypeScript", "TestData/Generated/TypeScript-R3.ts")]
    [Trait("Category", "Generation")]
    [Trait("FhirVersion", "R3")]
    internal void TestLangR3(string langName, string filePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        string data = File.ReadAllText(path);

        using (MemoryStream ms = new())
        {
            switch (langName)
            {
                case "Info":
                    {
                        LangInfo exportLang = new();
                        LangInfo.InfoOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown language: {langName}");
            }


            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using (StreamReader sr = new(ms))
            {
                string current = sr.ReadToEnd();

                // update the current file contents (manual)
                if (GenerationTestFixture.WriteGeneratedFiles)
                {
                    File.WriteAllText(filePath, current);
                    Assert.Fail("Generated files updated, please re-run the test");
                }

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(data);
            }
        }
    }
}

public class GenerationTestsR2 : IClassFixture<GenerationTestFixture>
{
    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>(Immutable) The fixture.</summary>
    private readonly GenerationTestFixture _fixture;

    /// <summary>The loaded.</summary>
    private DefinitionCollection _loaded = null!;

    public GenerationTestsR2(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        _fixture.Loader = new(new() { FhirCacheDirectory = _fixture.CachePath });

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(_fixture.EntriesR2).Result;

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            throw new Exception("Failed to load DSTU2");
        }

        _loaded = loaded;
    }

    [Theory]
    [InlineData("Info", "TestData/Generated/Info-R2.txt")]
    [InlineData("TypeScript", "TestData/Generated/TypeScript-R2.ts")]
    [Trait("Category", "Generation")]
    [Trait("FhirVersion", "R2")]
    internal void TestLangR2(string langName, string filePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        string data = File.ReadAllText(path);

        using (MemoryStream ms = new())
        {
            switch (langName)
            {
                case "Info":
                    {
                        LangInfo exportLang = new();
                        LangInfo.InfoOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new()
                        {
                            WriteStream = ms
                        };
                        exportLang.Export(options, _loaded);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown language: {langName}");
            }


            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using (StreamReader sr = new(ms))
            {
                string current = sr.ReadToEnd();

                // update the current file contents (manual)
                if (GenerationTestFixture.WriteGeneratedFiles)
                {
                    File.WriteAllText(filePath, current);
                    Assert.Fail("Generated files updated, please re-run the test");
                }

                // should the types like canonical be canonical::canonical or canonical::string?
                current.Should().Be(data);
            }
        }
    }
}
