// <copyright file="ConfigRoot.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGen.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

/// <summary>
/// Root configuration with common settings.
/// </summary>
public class ConfigRoot : ICodeGenConfig
{
    /// <summary>
    /// Gets or sets the pathname of the FHIR cache directory.
    /// </summary>
    [ConfigOption(
        ArgName = "--fhir-cache",
        EnvName = "Fhir_Cache",
        Description = "Location of the FHIR cache (none specified defaults to user .fhir directory).")]
    public string? FhirCacheDirectory { get; set; } = null;

    /// <summary>
    /// Gets or sets the configuration option for the FHIR cache directory.
    /// </summary>
    private static ConfigurationOption FhirCacheDirectoryParameter { get; } = new()
    {
        Name = "FhirCache",
        EnvVarName = "Fhir_Cache",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string?>("--fhir-cache", "Location of the FHIR cache (none specified defaults to user .fhir directory).")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--use-official-registries",
        EnvName = "Use_Official_Registries",
        Description = "Use official FHIR registries to resolve packages.")]
    public bool UseOfficialRegistries { get; set; } = true;

    private static ConfigurationOption UseOfficialRegistriesParameter { get; } = new()
    {
        Name = "UseOfficialRegistries",
        EnvVarName = "Use_Official_Registries",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--use-official-registries", "Use official FHIR registries to resolve packages.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--additional-fhir-registry-urls",
        EnvName = "Additional_FHIR_Registry_Urls",
        ArgArity = "0..*",
        Description = "Additional FHIR registry URLs to use.")]
    public string[] AdditionalFhirRegistryUrls { get; set; } = Array.Empty<string>();

    private static ConfigurationOption AdditionalFhirRegistryUrlsParameter { get; } = new()
    {
        Name = "AdditionalFhirRegistryUrls",
        EnvVarName = "Additional_FHIR_Registry_Urls",
        DefaultValue = Array.Empty<string>(),
        CliOption = new System.CommandLine.Option<string[]>("--additional-fhir-registry-urls", "Additional FHIR registry URLs to use.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    [ConfigOption(
    ArgName = "--additional-npm-registry-urls",
    EnvName = "Additional_NPM_Registry_Urls",
    ArgArity = "0..*",
    Description = "Additional NPM registry URLs to use.")]
    public string[] AdditionalNpmRegistryUrls { get; set; } = Array.Empty<string>();

    private static ConfigurationOption AdditionalNpmRegistryUrlsParameter { get; } = new()
    {
        Name = "AdditionalNpmRegistryUrls",
        EnvVarName = "Additional_NPM_Registry_Urls",
        DefaultValue = Array.Empty<string>(),
        CliOption = new System.CommandLine.Option<string[]>("--additional-npm-registry-urls", "Additional NPM registry URLs to use.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets the pathname of the output directory.
    /// </summary>
    [ConfigOption(
        ArgAliases = ["--output-path", "--output-directory", "--output-dir"],
        EnvName = "Output_Path",
        Description = "File or directory to write output.")]
    public string OutputDirectory { get; set; } = "./generated";

    /// <summary>
    /// Gets or sets the configuration option for the output directory.
    /// </summary>
    private static ConfigurationOption OutputDirectoryParameter { get; } = new()
    {
        Name = "OutputPath",
        EnvVarName = "Output_Path",
        DefaultValue = "./generated",
        CliOption = new System.CommandLine.Option<string>(["--output-path", "--output-directory", "--output-dir"], "File or directory to write output.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the filename of the output file.</summary>
    public string OutputFilename { get; set; } = string.Empty;

    private static ConfigurationOption OutputFilenameParameter { get; } = new()
    {
        Name = "OutputFilename",
        EnvVarName = "Output_Filename",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--output-filename", "Filename to write output.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets the packages to load.
    /// </summary>
    [ConfigOption(
        ArgAliases = ["--package", "--load-package", "-p"],
        EnvName = "Load_Package",
        ArgArity = "0..*",
        Description = "Package to load, either as directive ([name]#[version/literal]) or URL.")]
    public string[] Packages { get; set; } = [];

    /// <summary>
    /// Gets or sets the configuration option for the packages to load.
    /// </summary>
    private static ConfigurationOption PackagesParameter { get; } = new()
    {
        Name = "Packages",
        DefaultValue = Array.Empty<string>(),
        CliOption = new System.CommandLine.Option<string[]>(["--package", "--load-package", "-p"], "Package to load, either as directive ([name]#[version/literal]) or URL.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--auto-load-expansions",
        EnvName = "Auto_Load_Expansions",
        Description = "When loading core packages, load the expansions packages automatically.")]
    public bool AutoLoadExpansions { get; set; } = true;

    private static ConfigurationOption AutoLoadExpansionsParameter { get; } = new()
    {
        Name = "AutoLoadExpansions",
        EnvVarName = "Auto_Load_Expansions",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--auto-load-expansions", "When loading core packages, load the expansions packages automatically.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--resolve-dependencies",
        EnvName = "Resolve_Dependencies",
        Description = "Resolve package dependencies.")]
    public bool ResolvePackageDependencies { get; set; } = false;

    private static ConfigurationOption ResolvePackageDependenciesParameter { get; } = new()
    {
        Name = "ResolvePackageDependencies",
        EnvVarName = "Resolve_Dependencies",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--resolve-dependencies", "Resolve package dependencies.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private static readonly FhirArtifactClassEnum[] _defaultLoadStructures =
    [
        FhirArtifactClassEnum.PrimitiveType,
        FhirArtifactClassEnum.ComplexType,
        FhirArtifactClassEnum.Resource,
        FhirArtifactClassEnum.Interface,
        FhirArtifactClassEnum.Extension,
        FhirArtifactClassEnum.Operation,
        FhirArtifactClassEnum.SearchParameter,
        FhirArtifactClassEnum.CodeSystem,
        FhirArtifactClassEnum.ValueSet,
        FhirArtifactClassEnum.Profile,
        FhirArtifactClassEnum.LogicalModel,
        FhirArtifactClassEnum.Compartment,
        FhirArtifactClassEnum.ConceptMap,
        FhirArtifactClassEnum.NamingSystem,
        FhirArtifactClassEnum.StructureMap,
        FhirArtifactClassEnum.ImplementationGuide,
        FhirArtifactClassEnum.CapabilityStatement,
    ];

    /// <summary>Gets or sets the FHIR structures to load, default is all.</summary>
    [ConfigOption(
        ArgName = "--load-structures",
        EnvName = "Load_Structures",
        Description = "Types of FHIR structures to load.",
        ArgArity = "0..*")]
    public FhirArtifactClassEnum[] LoadStructures { get; set; } = _defaultLoadStructures;

    //public HashSet<FhirArtifactClassEnum> ProcessStructures { get; set; } = new();

    private static ConfigurationOption LoadStructuresParameter { get; } = new()
    {
        Name = "LoadStructures",
        EnvVarName = "Load_Structures",
        DefaultValue = _defaultLoadStructures,
        CliOption = new System.CommandLine.Option<FhirArtifactClassEnum[]>("--load-structures", "Types of FHIR structures to load.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    private static readonly FhirArtifactClassEnum[] _defaultExportStructures =
    [
        FhirArtifactClassEnum.PrimitiveType,
        FhirArtifactClassEnum.ComplexType,
        FhirArtifactClassEnum.Resource,
        FhirArtifactClassEnum.Interface,
        FhirArtifactClassEnum.Extension,
        FhirArtifactClassEnum.Operation,
        FhirArtifactClassEnum.SearchParameter,
        FhirArtifactClassEnum.CodeSystem,
        FhirArtifactClassEnum.ValueSet,
        FhirArtifactClassEnum.Profile,
        FhirArtifactClassEnum.LogicalModel,
        FhirArtifactClassEnum.Compartment,
    ];

    /// <summary>Gets or sets the FHIR structures to load, default is all.</summary>
    [ConfigOption(
        ArgName = "--export-structures",
        EnvName = "Export_Structures",
        Description = "Types of FHIR structures to export.",
        ArgArity = "0..*")]
    public FhirArtifactClassEnum[] ExportStructures { get; set; } = _defaultExportStructures;

    //public HashSet<FhirArtifactClassEnum> ProcessStructures { get; set; } = new();

    private static ConfigurationOption ExportStructuresParameter { get; } = new()
    {
        Name = "ExportStructures",
        EnvVarName = "Export_Structures",
        DefaultValue = _defaultExportStructures,
        CliOption = new System.CommandLine.Option<FhirArtifactClassEnum[]>("--export-structures", "Types of FHIR structures to export.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the export keys.</summary>
    [ConfigOption(
        ArgName = "--export-keys",
        EnvName = "Export_Keys",
        Description = "Keys of FHIR structures to export (e.g., Patient), empty means all.",
        ArgArity = "0..*")]
    public HashSet<string> ExportKeys { get; set; } = [];

    private static ConfigurationOption ExportKeysParameter { get; } = new()
    {
        Name = "ExportKeys",
        EnvVarName = "Export_Keys",
        DefaultValue = new HashSet<string>(),
        CliOption = new System.CommandLine.Option<HashSet<string>>("--export-keys", "Keys of FHIR structures to export (e.g., Patient), empty means all.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };


    [ConfigOption(
        ArgName = "--load-canonical-examples",
        EnvName = "Load_Canonical_Examples",
        Description = "Load canonical examples from packages.")]
    public bool LoadCanonicalExamples { get; set; } = false;

    private static ConfigurationOption LoadCanonicalExamplesParameter { get; } = new()
    {
        Name = "LoadCanonicalExamples",
        EnvVarName = "Load_Canonical_Examples",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--load-canonical-examples", "Load canonical examples from packages.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the offline mode.
    /// </summary>
    [ConfigOption(
        ArgName = "--offline",
        EnvName = "Offline",
        Description = "Offline mode (will not download missing packages).")]
    public bool OfflineMode { get; set; } = false;

    /// <summary>
    /// Gets or sets the configuration option for the offline mode.
    /// </summary>
    private static ConfigurationOption OfflineModeParameter { get; } = new()
    {
        Name = "OfflineMode",
        EnvVarName = "Offline",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--offline", "Offline mode (will not download missing packages).")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--fhir-version",
        EnvName = "Fhir_Version",
        Description = "FHIR version to use.")]
    public string FhirVersion { get; set; } = string.Empty;

    private static ConfigurationOption FhirVersionParameter { get; } = new()
    {
        Name = "FhirVersion",
        EnvVarName = "Fhir_Version",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--fhir-version", "FHIR version to use.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets all the configuration options.
    /// </summary>
    private static readonly ConfigurationOption[] _options =
    [
        FhirCacheDirectoryParameter,
        UseOfficialRegistriesParameter,
        AdditionalFhirRegistryUrlsParameter,
        AdditionalNpmRegistryUrlsParameter,
        OutputDirectoryParameter,
        OutputFilenameParameter,
        PackagesParameter,
        AutoLoadExpansionsParameter,
        ResolvePackageDependenciesParameter,
        LoadStructuresParameter,
        ExportKeysParameter,
        LoadCanonicalExamplesParameter,
        OfflineModeParameter,
        FhirVersionParameter,
    ];

    /// <summary>
    /// Gets the array of configuration options.
    /// </summary>
    public virtual ConfigurationOption[] GetOptions() => _options;

    internal T GetOpt<T>(
        System.CommandLine.Parsing.ParseResult parseResult,
        System.CommandLine.Option opt,
        T defaultValue)
    {
        if (!parseResult.HasOption(opt))
        {
            return defaultValue;
        }

        object? parsed = parseResult.GetValueForOption(opt);

        if ((parsed != null) &&
            (parsed is T typed))
        {
            return typed;
        }

        return defaultValue;
    }

    internal T[] GetOptArray<T>(
        System.CommandLine.Parsing.ParseResult parseResult,
        System.CommandLine.Option opt,
        T[] defaultValue)
    {
        if (!parseResult.HasOption(opt))
        {
            return defaultValue;
        }

        object? parsed = parseResult.GetValueForOption(opt);

        if (parsed == null)
        {
            return defaultValue;
        }

        List<T> values = [];

        if (parsed is T[] array)
        {
            return array;
        }
        else if (parsed is IEnumerator genericEnumerator) 
        {
            // use the enumerator to add values to the array
            while (genericEnumerator.MoveNext())
            {
                if (genericEnumerator.Current is T tValue)
                {
                    values.Add(tValue);
                }
                else
                {
                    throw new Exception("Should not be here!");
                }
            }
        }
        else if (parsed is IEnumerator<T> enumerator)
        {
            // use the enumerator to add values to the array
            while (enumerator.MoveNext())
            {
                values.Add(enumerator.Current);
            }
        }
        else
        {
            throw new Exception("Should not be here!");
        }

        // if no values were added, return the default - parser cannot tell the difference between no values and default values
        if (values.Count == 0)
        {
            return defaultValue;
        }

        return [.. values];
    }

    internal HashSet<T> GetOptHash<T>(
        System.CommandLine.Parsing.ParseResult parseResult,
        System.CommandLine.Option opt,
        HashSet<T> defaultValue)
    {
        if (!parseResult.HasOption(opt))
        {
            return defaultValue;
        }

        object? parsed = parseResult.GetValueForOption(opt);

        if (parsed == null)
        {
            return defaultValue;
        }

        HashSet<T> values = [];

        if (parsed is IEnumerator<T> typed)
        {
            // use the enumerator to add values to the array
            while (typed.MoveNext())
            {
                values.Add(typed.Current);
            }
        }
        else
        {
            throw new Exception("Should not be here!");
        }

        // if no values were added, return the default - parser cannot tell the difference between no values and default values
        if (values.Count == 0)
        {
            return defaultValue;
        }

        return values;
    }

    internal string FindRelativeDir(
        string startDir,
        string dirName,
        bool throwIfNotFound = true)
    {
        string currentDir;

        if (string.IsNullOrEmpty(startDir))
        {
            if (dirName.StartsWith('~'))
            {
                currentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

                if (dirName.Length > 1)
                {
                    dirName = dirName[2..];
                }
                else
                {
                    dirName = string.Empty;
                }
            }
            else
            {
                currentDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
            }
        }
        else if (startDir.StartsWith('~'))
        {
            // check if the path was only the user dir or the user dir plus a separator
            if ((startDir.Length == 1) || (startDir.Length == 2))
            {
                currentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            else
            {
                // skip the separator
                currentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), startDir[2..]);
            }
        }
        else
        {
            currentDir = startDir;
        }

        string testDir = Path.Combine(currentDir, dirName);

        while (!Directory.Exists(testDir))
        {
            currentDir = Path.GetFullPath(Path.Combine(currentDir, ".."));

            if (currentDir == Path.GetPathRoot(currentDir))
            {
                if (throwIfNotFound)
                {
                    throw new DirectoryNotFoundException($"Could not find directory {dirName}!");
                }

                return string.Empty;
            }

            testDir = Path.Combine(currentDir, dirName);
        }

        return Path.GetFullPath(testDir);
    }


    /// <summary>Parses the given parse result.</summary>
    /// <param name="parseResult">The parse result.</param>
    public virtual void Parse(System.CommandLine.Parsing.ParseResult parseResult)
    {
        foreach (ConfigurationOption opt in _options)
        {
            switch (opt.Name)
            {
                case "FhirCache":
                    {
                        string? dir = GetOpt(parseResult, opt.CliOption, FhirCacheDirectory);

                        //if (string.IsNullOrEmpty(dir))
                        //{
                        //    dir = FindRelativeDir(string.Empty, ".fhir");
                        //}
                        //else if (!Path.IsPathRooted(dir))
                        //{
                        //    dir = FindRelativeDir(string.Empty, dir);
                        //}

                        FhirCacheDirectory = string.IsNullOrEmpty(dir) ? null : dir;
                    }
                    break;
                case "UseOfficialRegistries":
                    UseOfficialRegistries = GetOpt(parseResult, opt.CliOption, UseOfficialRegistries);
                    break;
                case "AdditionalFhirRegistryUrls":
                    AdditionalFhirRegistryUrls = GetOptArray(parseResult, opt.CliOption, AdditionalFhirRegistryUrls);
                    break;
                case "AdditionalNpmRegistryUrls":
                    AdditionalNpmRegistryUrls = GetOptArray(parseResult, opt.CliOption, AdditionalNpmRegistryUrls);
                    break;
                case "OutputPath":
                    {
                        string dir = GetOpt(parseResult, opt.CliOption, OutputDirectory);

                        if (string.IsNullOrEmpty(dir))
                        {
                            dir = FindRelativeDir(string.Empty, ".");
                        }
                        else if (!Path.IsPathRooted(dir))
                        {
                            dir = FindRelativeDir(string.Empty, dir);
                        }

                        OutputDirectory = dir;
                    }
                    break;
                case "OutputFilename":
                    OutputFilename = GetOpt(parseResult, opt.CliOption, OutputFilename);
                    break;
                case "Packages":
                    Packages = GetOptArray(parseResult, opt.CliOption, Packages);
                    break;
                case "AutoLoadExpansions":
                    AutoLoadExpansions = GetOpt(parseResult, opt.CliOption, AutoLoadExpansions);
                    break;
                case "LoadStructures":
                    LoadStructures = GetOptArray(parseResult, opt.CliOption, LoadStructures);
                    break;
                case "ExportStructures":
                    ExportStructures = GetOptArray(parseResult, opt.CliOption, ExportStructures);
                    break;
                case "ExportKeys":
                    ExportKeys = GetOptHash(parseResult, opt.CliOption, ExportKeys);
                    break;
                case "LoadCanonicalExamples":
                    LoadCanonicalExamples = GetOpt(parseResult, opt.CliOption, LoadCanonicalExamples);
                    break;
                case "OfflineMode":
                    OfflineMode = GetOpt(parseResult, opt.CliOption, OfflineMode);
                    break;
                case "ResolvePackageDependencies":
                    ResolvePackageDependencies = GetOpt(parseResult, opt.CliOption, ResolvePackageDependencies);
                    break;
                case "FhirVersion":
                    FhirVersion = GetOpt(parseResult, opt.CliOption, FhirVersion);
                    break;
            }
        }
    }

    //[ConfigOption(
    //    ArgAliases = new[] { "--terminology-server", "--tx" },
    //    EnvName = "Terminology_Server",
    //    ArgArity = "0..*",
    //    Description = "FHIR URL for a terminology server to use")]
    //public string[] TxServers { get; set; } = Array.Empty<string>();
}
