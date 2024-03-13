// <copyright file="GenerationTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FluentAssertions;
using Microsoft.Health.Fhir.CodeGen.Lanugage;
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

    /// <summary>The FHIR R4 package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR4;

    /// <summary>The FHIR STU3 package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR3;

    /// <summary>The FHIR DSTU2 package entries.</summary>
    public IEnumerable<PackageCacheEntry> EntriesR2;

    /// <summary>True to write generated files.</summary>
    public static bool WriteGeneratedFiles = false;

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

        EntriesR4 = new List<PackageCacheEntry>()
        {
            Load("hl7.fhir.r4.core#4.0.1"),
            Load("hl7.fhir.r4.expansions#4.0.1"),
            Load("hl7.fhir.uv.extensions#1.0.0"),
        };

        EntriesR3 = new List<PackageCacheEntry>()
        {
            Load("hl7.fhir.r3.core#3.0.2"),
            Load("hl7.fhir.r3.expansions#3.0.2"),
        };

        EntriesR2 = new List<PackageCacheEntry>()
        {
            Load("hl7.fhir.r2.core#1.0.2"),
            Load("hl7.fhir.r2.expansions#1.0.2"),
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

    /// <summary>(Immutable) The fixture.</summary>
    private readonly GenerationTestFixture _fixture;

    /// <summary>The loaded.</summary>
    private DefinitionCollection _loaded = null!;

    public GenerationTestsR5(GenerationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = loader.LoadPackages(_fixture.EntriesR5.First().Name, _fixture.EntriesR5).Result;

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
    internal void TestGenR5(string langName, string filePath)
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
                        LangInfo.InfoOptions options = new();
                        options.WriteStream = ms;
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new();
                        options.WriteStream = ms;
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

        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = loader.LoadPackages(_fixture.EntriesR4B.First().Name, _fixture.EntriesR4B).Result;

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
    internal void TestInfoR4B(string langName, string filePath)
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
                        LangInfo.InfoOptions options = new();
                        options.WriteStream = ms;
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new();
                        options.WriteStream = ms;
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

        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = loader.LoadPackages(_fixture.EntriesR4.First().Name, _fixture.EntriesR4).Result;

        loaded.Should().NotBeNull();

        if (loaded == null)
        {
            throw new Exception("Failed to load R4");
        }

        _loaded = loaded;
    }

    [Theory]
    [InlineData("Info", "TestData/Generated/Info-R4.txt")]
    [InlineData("TypeScript", "TestData/Generated/TypeScript-R4.ts")]
    internal void TestInfoR4(string langName, string filePath)
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
                        LangInfo.InfoOptions options = new();
                        options.WriteStream = ms;
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new();
                        options.WriteStream = ms;
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

        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = loader.LoadPackages(_fixture.EntriesR3.First().Name, _fixture.EntriesR3).Result;

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
    internal void TestInfoR3(string langName, string filePath)
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
                        LangInfo.InfoOptions options = new();
                        options.WriteStream = ms;
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new();
                        options.WriteStream = ms;
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

        PackageLoader loader = new(_fixture.Cache, new());

        DefinitionCollection? loaded = loader.LoadPackages(_fixture.EntriesR2.First().Name, _fixture.EntriesR2).Result;

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
    internal void TestInfoR2(string langName, string filePath)
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
                        LangInfo.InfoOptions options = new();
                        options.WriteStream = ms;
                        exportLang.Export(options, _loaded);
                    }
                    break;

                case "TypeScript":
                    {
                        TypeScript exportLang = new();
                        TypeScript.TypeScriptOptions options = new();
                        options.WriteStream = ms;
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
