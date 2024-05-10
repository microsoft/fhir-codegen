// <copyright file="ConfigRoot.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using Microsoft.Health.Fhir.CodeGen.Extensions;

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
        Description = "Location of the FHIR cache.")]
    public string FhirCacheDirectory { get; set; } = "~/.fhir";

    /// <summary>
    /// Gets or sets the configuration option for the FHIR cache directory.
    /// </summary>
    private static ConfigurationOption FhirCacheDirectoryParameter { get; } = new()
    {
        Name = "FhirCache",
        EnvVarName = "Fhir_Cache",
        DefaultValue = "~/.fhir",
        CliOption = new System.CommandLine.Option<string>("--fhir-cache", "Location of the FHIR cache.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
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

    /// <summary>
    /// Gets all the configuration options.
    /// </summary>
    private static readonly ConfigurationOption[] _options =
    [
        FhirCacheDirectoryParameter,
        OutputDirectoryParameter,
        PackagesParameter,
        AutoLoadExpansionsParameter,
        ResolvePackageDependenciesParameter,
        OfflineModeParameter,
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
                    dirName = dirName.Substring(2);
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
                currentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), startDir.Substring(2));
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
                        string dir = GetOpt(parseResult, opt.CliOption, FhirCacheDirectory);

                        if (string.IsNullOrEmpty(dir))
                        {
                            dir = FindRelativeDir(string.Empty, ".fhir");
                        }
                        else if (!Path.IsPathRooted(dir))
                        {
                            dir = FindRelativeDir(string.Empty, dir);
                        }

                        FhirCacheDirectory = dir;
                    }
                    break;
                case "OutputPath":
                    {
                        string dir = GetOpt(parseResult, opt.CliOption, OutputDirectory);

                        if (string.IsNullOrEmpty(dir))
                        {
                            dir = FindRelativeDir(string.Empty, ".fhir");
                        }
                        else if (!Path.IsPathRooted(dir))
                        {
                            dir = FindRelativeDir(string.Empty, dir);
                        }

                        OutputDirectory = dir;
                    }
                    break;
                case "Packages":
                    Packages = GetOptArray(parseResult, opt.CliOption, Packages);
                    break;
                case "AutoLoadExpansions":
                    AutoLoadExpansions = GetOpt(parseResult, opt.CliOption, AutoLoadExpansions);
                    break;
                case "OfflineMode":
                    OfflineMode = GetOpt(parseResult, opt.CliOption, OfflineMode);
                    break;
                case "ResolvePackageDependencies":
                    ResolvePackageDependencies = GetOpt(parseResult, opt.CliOption, ResolvePackageDependencies);
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
