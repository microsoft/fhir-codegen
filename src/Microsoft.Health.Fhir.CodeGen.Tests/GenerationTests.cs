// <copyright file="GenerationTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Language.Firely;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationTestFixture"/> class.
    /// </summary>
    public GenerationTestFixture()
    {
    }

    /// <summary>Compare generation.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="existingPath">        Full pathname of the existing file.</param>
    /// <param name="currentMS">           The current milliseconds.</param>
    /// <param name="compareLinesDirectly">(Optional) True to compare lines directly, false to compare set of line hashes.</param>
    internal static void CompareGeneration(string existingPath, MemoryStream currentMS, bool compareLinesDirectly = true)
    {
        // make sure the MS is up to date and at the beginning
        currentMS.Flush();
        currentMS.Seek(0, SeekOrigin.Begin);

        if (WriteGeneratedFiles)
        {
            using (StreamReader sr = new(currentMS))
            {
                string current = sr.ReadToEnd();

                File.WriteAllText(existingPath, current);
                Assert.Fail("Generated files updated, please re-run the test");
            }

            return;
        }

        if (!Path.IsPathRooted(existingPath))
        {
            existingPath = Path.Combine(Directory.GetCurrentDirectory(), existingPath);
            existingPath = Path.GetFullPath(existingPath);
        }

        if (!File.Exists(existingPath))
        {
            throw new ArgumentException($"Could not find file at path: {existingPath}");
        }

        string version = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(DefinitionCollection))!.Location).ProductVersion?.ToString() ?? string.Empty;

        if (version.Contains('+'))
        {
            version = version.Substring(0, version.IndexOf('+'));
        }

        using FileStream existingFS = new(existingPath, FileMode.Open, FileAccess.Read);

        // compare files line by line
        using StreamReader existingReader = new(existingFS);
        using StreamReader currentReader = new(currentMS);

        HashSet<string> existingLines = [];
        HashSet<string> currentLines = [];

        int i = 0;
        bool done = false;
        while (!done)
        {
            string? existingLine = existingReader.ReadLine();
            string? currentLine = currentReader.ReadLine();

            if ((existingLine == null) && (currentLine == null))
            {
                done = true;
                break;
            }

            if (existingLine == null)
            {
                Assert.Fail($"Failed to read line {i} in existing file!");
                return;
            }

            if (currentLine == null)
            {
                Assert.Fail($"Failed to read line {i} in current file!");
                return;
            }

            i++;
            if (currentLine.Contains(version))
            {
                // skip any lines with a version in them
                continue;
            }

            if (compareLinesDirectly)
            {
                currentLine.Should().Be(existingLine, $"Line {i} found:\n\t{currentLine}\nexpected:\n\t{existingLine}");
            }
            else
            {
                existingLines.Add(existingLine.TrimEnd());
                currentLines.Add(currentLine.TrimEnd());
            }
        }

        if (!compareLinesDirectly)
        {
            currentLines.Count().Should().Be(existingLines.Count(), "Line count does not match");

            currentLines.ExceptWith(existingLines);

            currentLines.Should().BeEmpty("Lines do not match:\n" + string.Join("\n", currentLines));
        }
    }

    internal static void CompareGenerationHashes(string existingPath, Dictionary<string, string> current)
    {
        if (WriteGeneratedFiles)
        {
            File.WriteAllText(existingPath, JsonSerializer.Serialize(current));
            Assert.Fail("Hash file updated, please re-run the test");

            return;
        }

        if (!File.Exists(existingPath))
        {
            throw new ArgumentException($"Could not find file at path: {existingPath}");
        }

        string json = File.ReadAllText(existingPath);

        Dictionary<string, string>? previous = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        previous.Should().NotBeNull();
        if (previous == null)
        {
            return;
        }

        previous.Count.Should().Be(current.Count);

        foreach ((string currentPath, string hash) in current)
        {
            string path = currentPath.Contains('/')
                ? currentPath.Replace('/', '\\')
                : currentPath;

            _ = previous.TryGetValue(path, out string? previousHash);

            hash.Should().BeEquivalentTo(previousHash, $"Hashes do not match for {path}!");
        }
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

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(TestCommon.EntriesR5).Result;

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

            GenerationTestFixture.CompareGeneration(path, ms);
        }
    }

    [Theory]
    [InlineData(CSharpFirelyCommon.GenSubset.Base, "TestData/Hashes/CSharpFirely2-R5-Base.json")]
    [InlineData(CSharpFirelyCommon.GenSubset.Conformance, "TestData/Hashes/CSharpFirely2-R5-Conformance.json")]
    [InlineData(CSharpFirelyCommon.GenSubset.Satellite, "TestData/Hashes/CSharpFirely2-R5-Satellite.json")]
    [Trait("Category", "Generation")]
    [Trait("Comparison", "Hash")]
    [Trait("FhirVersion", "R5")]
    internal void TestFirelyHashesR5(CSharpFirelyCommon.GenSubset subset, string comparisonFilePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(comparisonFilePath)
            ? comparisonFilePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), comparisonFilePath);

        FirelyGenOptions options = new()
        {
            Subset = subset,
        };
        CSharpFirely2 exportLang = new();

        IFileHashTestable langHashTestable = exportLang;

        langHashTestable.GenerateHashesInsteadOfOutput = true;

        exportLang.Export(options, _loaded);

        GenerationTestFixture.CompareGenerationHashes(path, langHashTestable.FileHashes);
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

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(TestCommon.EntriesR4B).Result;

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

            GenerationTestFixture.CompareGeneration(path, ms);
        }
    }

    [Theory]
    [InlineData(CSharpFirelyCommon.GenSubset.Satellite, "TestData/Hashes/CSharpFirely2-R4B-Satellite.json")]
    [Trait("Category", "Generation")]
    [Trait("Comparison", "Hash")]
    [Trait("FhirVersion", "R4B")]
    internal void TestFirelyHashesR4B(CSharpFirelyCommon.GenSubset subset, string comparisonFilePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(comparisonFilePath)
            ? comparisonFilePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), comparisonFilePath);

        FirelyGenOptions options = new()
        {
            Subset = subset,
        };
        CSharpFirely2 exportLang = new();

        IFileHashTestable langHashTestable = exportLang;

        langHashTestable.GenerateHashesInsteadOfOutput = true;

        exportLang.Export(options, _loaded);

        GenerationTestFixture.CompareGenerationHashes(path, langHashTestable.FileHashes);
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

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(TestCommon.EntriesR4).Result;

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

        csPath = string.IsNullOrEmpty(csPath)
            ? string.Empty
            : Path.IsPathRooted(csPath)
                ? csPath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), csPath);

        bool compareLinesDirectly = true;

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
                        compareLinesDirectly = false;
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
                        compareLinesDirectly = false;
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
                        compareLinesDirectly = false;
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
                        compareLinesDirectly = false;
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
                        compareLinesDirectly = false;
                    }
                    break;

                default:
                    throw new ArgumentException($"Unknown language: {langName}");
            }

            GenerationTestFixture.CompareGeneration(path, ms, compareLinesDirectly);
        }
    }

    [Theory]
    [InlineData(CSharpFirelyCommon.GenSubset.Satellite, "TestData/Hashes/CSharpFirely2-R4-Satellite.json")]
    [Trait("Category", "Generation")]
    [Trait("Comparison", "Hash")]
    [Trait("FhirVersion", "R4")]
    internal void TestFirelyHashesR4(CSharpFirelyCommon.GenSubset subset, string comparisonFilePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(comparisonFilePath)
            ? comparisonFilePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), comparisonFilePath);

        FirelyGenOptions options = new()
        {
            Subset = subset,
        };
        CSharpFirely2 exportLang = new();

        IFileHashTestable langHashTestable = exportLang;

        langHashTestable.GenerateHashesInsteadOfOutput = true;

        exportLang.Export(options, _loaded);

        GenerationTestFixture.CompareGenerationHashes(path, langHashTestable.FileHashes);
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

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(TestCommon.EntriesR3).Result;

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

            GenerationTestFixture.CompareGeneration(path, ms);
        }
    }

    [Theory]
    [InlineData(CSharpFirelyCommon.GenSubset.Satellite, "TestData/Hashes/CSharpFirely2-R3-Satellite.json")]
    [Trait("Category", "Generation")]
    [Trait("Comparison", "Hash")]
    [Trait("FhirVersion", "R3")]
    internal void TestFirelyHashesR3(CSharpFirelyCommon.GenSubset subset, string comparisonFilePath)
    {
        // Get the absolute path to the file
        string path = Path.IsPathRooted(comparisonFilePath)
            ? comparisonFilePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), comparisonFilePath);

        FirelyGenOptions options = new()
        {
            Subset = subset,
        };
        CSharpFirely2 exportLang = new();

        IFileHashTestable langHashTestable = exportLang;

        langHashTestable.GenerateHashesInsteadOfOutput = true;

        exportLang.Export(options, _loaded);

        GenerationTestFixture.CompareGenerationHashes(path, langHashTestable.FileHashes);
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

        DefinitionCollection? loaded = _fixture.Loader.LoadPackages(TestCommon.EntriesR2).Result;

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

            GenerationTestFixture.CompareGeneration(path, ms);
        }
    }
}
