using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Hl7.Fhir.Model;

namespace xver_editor.Config;

public class XverAppConfig: ICodeGenConfig
{
    private const int _defaultListenPort = 8000;

    public string? LaunchCommand { get; set; } = null;

    public ILoggerFactory LogFactory { get; set; } = LoggerFactory.Create(builder => builder.AddConsole());

    /// <summary>Gets or sets the listen port.</summary>
    [ConfigOption(
        ArgName = "--port",
        EnvName = "Listen_Port",
        Description = "TCP port to listen on")]
    public int ListenPort { get; set; } = _defaultListenPort;

    /// <summary>Gets the listen port option.</summary>
    private static ConfigurationOption ListenPortParameter { get; } = new()
    {
        Name = "ListenPort",
        EnvVarName = "Listen_Port",
        DefaultValue = _defaultListenPort,
        CliOption = new System.CommandLine.Option<int?>("--port", "TCP port to listen on")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the browser should be opened.</summary>
    [ConfigOption(
        ArgAliases = ["--open-browser", "-o"],
        EnvName = "Open_Browser",
        Description = "Open the browser to the public URL once the server starts")]
    public bool OpenBrowser { get; set; } = false;

    /// <summary>Gets the open browser option.</summary>
    private static ConfigurationOption OpenBrowserParameter { get; } = new()
    {
        Name = "OpenBrowser",
        EnvVarName = "Open_Browser",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>(["--open-browser", "-o"], "Open the browser to the public URL once the server starts")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--db",
        EnvName = "Comparison_Database_Path",
        ArgArity = "0..1",
        Description = "Path or filename for the comparison database FHIR maps to load or export.")]
    public string CrossVersionDbPath { get; set; } = string.Empty;

    private static ConfigurationOption CrossVersionDbPathParameter => new()
    {
        Name = "Comparison_Database_Path",
        EnvVarName = "Comparison_Database_Path",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string?>("--db", "Path or filename for the comparison database FHIR maps to load or export.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    [ConfigOption(
    ArgName = "--user",
        EnvName = "User_Name",
        ArgArity = "0..1",
        Description = "Default username to use when marking comparisons as reviewed.")]
    public string UserName { get; set; } = string.Empty;

    private static ConfigurationOption UserNameParameter => new()
    {
        Name = "User_Name",
        EnvVarName = "User_Name",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string?>("--user", "Default username to use when marking comparisons as reviewed.")
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
    public string OutputDirectory { get; set; } = ".";

    /// <summary>
    /// Gets or sets the configuration option for the output directory.
    /// </summary>
    private static ConfigurationOption OutputDirectoryParameter { get; } = new()
    {
        Name = "OutputPath",
        EnvVarName = "Output_Path",
        DefaultValue = ".",
        CliOption = new System.CommandLine.Option<string>(["--output-path", "--output-directory", "--output-dir"], "File or directory to write output.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>(Immutable) Options for controlling the operation.</summary>
    private static readonly ConfigurationOption[] _options =
    [
        ListenPortParameter,
        OpenBrowserParameter,
        CrossVersionDbPathParameter,
        UserNameParameter,
        OutputDirectoryParameter,
    ];

    /// <summary>
    /// Gets the array of configuration options.
    /// </summary>
    public virtual ConfigurationOption[] GetOptions() => _options;

    internal T GetOpt<T>(
        System.CommandLine.Parsing.ParseResult parseResult,
        ConfigurationOption opt,
        T defaultValue)
    {
        if (!parseResult.HasOption(opt.CliOption))
        {
            return defaultValue;
        }

        object? parsed = parseResult.GetValueForOption(opt.CliOption);

        if (parsed is System.CommandLine.Parsing.Token t)
        {
            switch (defaultValue)
            {
                case bool:
                    return (T)((object?)Convert.ToBoolean(t.Value) ?? defaultValue);
                case int:
                    return (T)((object?)Convert.ToInt32(t.Value) ?? defaultValue);
                case long:
                    return (T)((object?)Convert.ToInt64(t.Value) ?? defaultValue);
                case float:
                    return (T)((object?)Convert.ToSingle(t.Value) ?? defaultValue);
                case double:
                    return (T)((object?)Convert.ToDouble(t.Value) ?? defaultValue);
                case decimal:
                    return (T)((object?)Convert.ToDecimal(t.Value) ?? defaultValue);
                case string:
                    return (T)((object?)Convert.ToString(t.Value) ?? defaultValue);
                default:
                    {
                        if ((t.Value != null) &&
                            (t.Value is T typed))
                        {
                            return typed;
                        }
                    }
                    break;
            }
        }

        switch (parsed)
        {
            case bool:
                return (T)((object?)Convert.ToBoolean(parsed) ?? defaultValue!);
            case int:
                return (T)((object?)Convert.ToInt32(parsed) ?? defaultValue!);
            case long:
                return (T)((object?)Convert.ToInt64(parsed) ?? defaultValue!);
            case float:
                return (T)((object?)Convert.ToSingle(parsed) ?? defaultValue!);
            case double:
                return (T)((object?)Convert.ToDouble(parsed) ?? defaultValue!);
            case decimal:
                return (T)((object?)Convert.ToDecimal(parsed) ?? defaultValue!);
            case string:
                return (T)((object?)Convert.ToString(parsed) ?? defaultValue!);
            default:
                {
                    if ((parsed != null) &&
                        (parsed is T typed))
                    {
                        return typed;
                    }
                }
                break;
        }

        string? envValue = Environment.GetEnvironmentVariable(opt.EnvVarName);
        if (envValue != null)
        {
            return (T)Convert.ChangeType(envValue, typeof(T));
        }

        return defaultValue;
    }

    internal T[] GetOptArray<T>(
        System.CommandLine.Parsing.ParseResult parseResult,
        ConfigurationOption opt,
        T[] defaultValue)
    {
        if (!parseResult.HasOption(opt.CliOption))
        {
            return defaultValue;
        }

        object? parsed = parseResult.GetValueForOption(opt.CliOption);

        if (parsed != null)
        {
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

            if (values.Count != 0)
            {
                return [.. values];
            }
        }

        string? envValue = Environment.GetEnvironmentVariable(opt.EnvVarName);
        if (envValue != null)
        {
            List<T> values = [];

            string[] envValues = envValue.Split(',');
            foreach (string ev in envValues)
            {
                values.Add((T)Convert.ChangeType(envValue, typeof(T)));
            }

            return [.. values];
        }

        return defaultValue;
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
        else if (parsed is IEnumerator generic)
        {
            // use the enumerator to add values to the array
            while (generic.MoveNext())
            {
                if (generic.Current is T tValue)
                {
                    values.Add(tValue);
                }
                else
                {
                    throw new Exception("Should not be here!");
                }
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


    internal string FindRelativeFile(
        string startDir,
        string filename,
        bool throwIfNotFound = true)
    {
        string currentFilename;

        if (string.IsNullOrEmpty(startDir))
        {
            if (filename.StartsWith('~'))
            {
                currentFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

                if (filename.Length > 1)
                {
                    filename = filename[2..];
                }
                else
                {
                    filename = string.Empty;
                }
            }
            else
            {
                currentFilename = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
            }
        }
        else if (startDir.StartsWith('~'))
        {
            // check if the path was only the user dir or the user dir plus a separator
            if ((startDir.Length == 1) || (startDir.Length == 2))
            {
                currentFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            else
            {
                // skip the separator
                currentFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), startDir[2..]);
            }
        }
        else
        {
            currentFilename = startDir;
        }

        string testFilename = Path.Combine(currentFilename, filename);

        while (!File.Exists(testFilename))
        {
            currentFilename = Path.GetFullPath(Path.Combine(currentFilename, ".."));

            if (currentFilename == Path.GetPathRoot(currentFilename))
            {
                if (throwIfNotFound)
                {
                    throw new DirectoryNotFoundException($"Could not find file {filename}!");
                }

                return string.Empty;
            }

            testFilename = Path.Combine(currentFilename, filename);
        }

        return Path.GetFullPath(testFilename);
    }


    /// <summary>Parses the given parse result.</summary>
    /// <param name="parseResult">The parse result.</param>
    public virtual void Parse(System.CommandLine.Parsing.ParseResult parseResult)
    {
        foreach (ConfigurationOption opt in _options)
        {
            switch (opt.Name)
            {
                case "ListenPort":
                    ListenPort = GetOpt(parseResult, opt, ListenPort);
                    break;
                case "OpenBrowser":
                    OpenBrowser = GetOpt(parseResult, opt, OpenBrowser);
                    break;
                case "Comparison_Database_Path":
                    {
                        CrossVersionDbPath = GetOpt(parseResult, opt, CrossVersionDbPath);
                        if (string.IsNullOrEmpty(CrossVersionDbPath))
                        {
                            CrossVersionDbPath = FindRelativeFile(string.Empty, "fhir-comparison.sqlite");
                        }

                        if (string.IsNullOrEmpty(CrossVersionDbPath))
                        {
                            throw new FileNotFoundException($"Database is required! Use the --db option, Comparison_Database_Path environment variable, or run in a directory with a 'fhir-comparison.sqlite' file.");
                        }
                    }
                    break;
                case "User_Name":
                    {
                        UserName = GetOpt(parseResult, opt, UserName);
                        if (string.IsNullOrEmpty(UserName))
                        {
                            UserName = Environment.UserName;
                        }
                    }
                    break;
                case "OutputPath":
                    {
                        string dir = GetOpt(parseResult, opt, OutputDirectory);

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
