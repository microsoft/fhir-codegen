// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using SCL = System.CommandLine; // this is present to disambiguate Option from System.CommandLine and Microsoft.FluentUI.AspNetCore.Components
using System.Diagnostics;
using System.Runtime.InteropServices;
//using fhir_codegen.Components;
using Microsoft.Health.Fhir.CodeGen.Configuration;
//using Microsoft.AspNetCore.Hosting.StaticWebAssets;
//using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Health.Fhir.CodeGen.Language;
using System.Reflection;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using System.CommandLine;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using System.CommandLine.Builder;
using System.Text;
using Microsoft.Extensions.Primitives;
using System.CommandLine.Parsing;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.Collections;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using System.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Health.Fhir.CodeGen.Net;
using Microsoft.Health.Fhir.CodeGenCommon.Smart;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.XVer;

namespace fhir_codegen;

/// <summary>A program.</summary>
public class Program
{
    private static List<SCL.Option> _optsWithEnums = [];

    private static HashSet<string> _packageAliases =
    [
        "--package", "--load-package", "-p"
    ];

    /// <summary>Main entry-point for this application.</summary>
    /// <param name="args">An array of command-line argument strings.</param>
    /// <returns>
    /// An asynchronous result that yields exit-code for the process - 0 for success, else an error
    /// code.
    /// </returns>
    public static async Task<int> Main(string[] args)
    {
        // setup our configuration defaults (environment > appsettings.json) - args will supersede
        IConfiguration envConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // in order to process help correctly we have to build a parser independent of the command
        SCL.Parsing.Parser parser = BuildParser(envConfig);

        // attempt a parse
        SCL.Parsing.ParseResult pr = parser.Parse(args);

        // check for invalid arguments, help, a generate command with no subcommand, or a generate with no packages to trigger the nicely formatted help
        if (pr.UnmatchedTokens.Any() ||
            !pr.Tokens.Any() ||
            (!pr.CommandResult.Command.Parents?.Any() ?? false) ||
            pr.Tokens.Any(t => t.Value.Equals("-?", StringComparison.Ordinal)) ||
            pr.Tokens.Any(t => t.Value.Equals("-h", StringComparison.Ordinal)) ||
            pr.Tokens.Any(t => t.Value.Equals("--help", StringComparison.Ordinal)) ||
            pr.Tokens.Any(t => t.Value.Equals("help", StringComparison.Ordinal)) ||
            pr.CommandResult.Command.Name.Equals("generate", StringComparison.Ordinal))

        {
            return await parser.InvokeAsync(args);
        }

        // check for a generate command with no packages
        if ((pr.CommandResult.Command.Parents?.FirstOrDefault()?.Name.Equals("generate", StringComparison.Ordinal) ?? false) &&
            (!pr.Tokens.Any(t => _packageAliases.Contains(t.Value))))
        {
            Console.WriteLine("Error: generate command requires at least one package to process.");

            return await parser.InvokeAsync(args.Append("--help").ToArray());
        }

        // any language subcommand is a generate command
        string command = pr.CommandResult.Command.Name;
        if (LanguageManager.HasLanguage(command))
        {
            command = "generate";
        }

        return command switch
        {
            "generate" => await DoGenerate(pr),
            "compare" => await DoCompare(pr),
            "xver" => await DoXVer(pr),
            //"cross-version" => await CrossVersionInteractive.DoCrossVersionReview(pr),
            "gui" => Gui.RunGui(pr),
            //case "interactive":
            //    return await DoInteractive(pr);
            //case "web":
            //    return await DoWeb(pr);
            _ => await parser.InvokeAsync(args),
        };
    }

    private static SCL.Parsing.Parser BuildParser(IConfiguration envConfig)
    {
        SCL.RootCommand command = BuildCommand(envConfig);

        SCL.Parsing.Parser parser = new CommandLineBuilder(command)
            .UseExceptionHandler((ex, ctx) =>
            {
                Console.WriteLine($"Error: {ex.Message}");
                ctx.ExitCode = 1;
            })
            .UseDefaults()
            .UseHelp(ctx =>
            {
                foreach (SCL.Option option in _optsWithEnums)
                {
                    StringBuilder sb = new();
                    if (option.Aliases.Count != 0)
                    {
                        sb.AppendLine(string.Join(", ", option.Aliases));
                    }
                    else
                    {
                        sb.AppendLine(option.Name);
                    }

                    Type et = option.ValueType;

                    if (option.ValueType.IsGenericType)
                    {
                        et = option.ValueType.GenericTypeArguments.First();
                    }

                    if (option.ValueType.IsArray)
                    {
                        et = option.ValueType.GetElementType()!;
                    }

                    foreach (MemberInfo mem in et.GetMembers(BindingFlags.Public | BindingFlags.Static).Where(m => m.DeclaringType == et).OrderBy(m => m.Name))
                    {
                        IEnumerable<DescriptionAttribute> attributes = mem.GetCustomAttributes<DescriptionAttribute>(false);

                        sb.AppendLine($"  opt: {mem.Name}");
                        if (attributes.Any())
                        {
                            sb.AppendLine($"       {attributes.First().Description}");
                        }
                    }

                    ctx.HelpBuilder.CustomizeSymbol(
                        option,
                        firstColumnText: (ctx) => sb.ToString());
                    //secondColumnText: (ctx) => option.Description);
                }
            })
            .Build();

        return parser;
    }

    /// <summary>Builds command parser.</summary>
    /// <param name="envConfig">  The environment configuration.</param>
    /// <param name="addHandlers">True to add handlers.</param>
    /// <returns>A SCL.Parsing.Parser.</returns>
    private static SCL.RootCommand BuildCommand(IConfiguration envConfig)
    {
        // create our root command
        SCL.RootCommand rootCommand = new("A utility for processing FHIR packages into other formats/languages.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigRoot), envConfig: envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            rootCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // create our generate command
        SCL.Command generateCommand = new("generate", "Generate output from a FHIR package and exit.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigGenerate), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            generateCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // iterate through languages and add them as subcommands
        foreach (ILanguage language in LanguageManager.GetLanguages())
        {
            SCL.Command languageCommand = new(language.Name, $"Generate {language.Name}");
            if (language.Name.Any(char.IsUpper))
            {
                languageCommand.AddAlias(language.Name.ToLowerInvariant());
            }

            foreach (SCL.Option option in BuildCliOptions(LanguageManager.ConfigTypeForLanguage(language.Name), envConfig: envConfig))
            {
                languageCommand.AddOption(option);
                TrackIfEnum(option);
            }

            generateCommand.AddCommand(languageCommand);
        }

        rootCommand.AddCommand(generateCommand);

        // create our interactive command
        SCL.Command interactiveCommand = new("interactive", "Launch into an interactive console.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigInteractive), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            interactiveCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // TODO(ginoc): Set the command handler
        rootCommand.AddCommand(interactiveCommand);

        // create our webserver command
        SCL.Command webCommand = new("web", "Launch into a locally-hosted web UI.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigFluentUi), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            webCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // TODO(ginoc): Set the command handler
        rootCommand.AddCommand(webCommand);

        // create our compare command
        SCL.Command compareCommand = new("compare", "Compare two sets of packages.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigCompare), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            compareCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // TODO(ginoc): Set the command handler
        rootCommand.AddCommand(compareCommand);

        // create our XVer command
        SCL.Command xverCommand = new("xver", "Perform FHIR Core Cross-Version processing.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigXVer), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            xverCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // TODO(ginoc): Set the command handler
        rootCommand.AddCommand(xverCommand);


        //// create our cross-version interactive command
        //SCL.Command cviCommand = new("cross-version", "Interactively review cross-version definitions.");
        //foreach (SCL.Option option in BuildCliOptions(typeof(ConfigCrossVersionInteractive), typeof(ConfigRoot), envConfig))
        //{
        //    // note that 'global' here is just recursive DOWNWARD
        //    cviCommand.AddGlobalOption(option);
        //    TrackIfEnum(option);
        //}

        //// TODO(ginoc): Set the command handler
        //rootCommand.AddCommand(cviCommand);

        // create our UI command
        SCL.Command guiCommand = new("gui", "Launch the GUI.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigGui), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            guiCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // TODO(ginoc): Set the command handler
        rootCommand.AddCommand(guiCommand);


        return rootCommand;

        void TrackIfEnum(SCL.Option option)
        {
            if (option.ValueType.IsEnum)
            {
                _optsWithEnums.Add(option);
                return;
            }

            if (option.ValueType.IsGenericType)
            {
                if (option.ValueType.GenericTypeArguments.First().IsEnum)
                {
                    _optsWithEnums.Add(option);
                }

                return;
            }

            if (option.ValueType.IsArray)
            {
                if (option.ValueType.GetElementType()!.IsEnum)
                {
                    _optsWithEnums.Add(option);
                }

                return;
            }
        }
    }

    //private static SCL.RootCommand BuildCommand(IConfiguration envConfig)
    //{
    //    // create our root command
    //    SCL.RootCommand rootCommand = new("A utility for processing FHIR packages into other formats/languages.");
    //    //foreach (SCL.Option option in BuildCliOptions(typeof(ConfigRoot), envConfig: envConfig))
    //    //{
    //    //    rootCommand.AddOption(option);
    //    //    TrackIfEnum(option);
    //    //}

    //    // create our generate command
    //    SCL.Command generateCommand = new("generate", "Generate output from a FHIR package and exit.");
    //    //foreach (SCL.Option option in BuildCliOptions(typeof(ConfigGenerate), envConfig: envConfig))
    //    //{
    //    //    generateCommand.AddOption(option);
    //    //    TrackIfEnum(option);
    //    //}

    //    // iterate through languages and add them as subcommands
    //    foreach (ILanguage language in LanguageManager.GetLanguages())
    //    {
    //        SCL.Command languageCommand = new(language.Name, $"Generate {language.Name}");
    //        if (language.Name.Any(char.IsUpper))
    //        {
    //            languageCommand.AddAlias(language.Name.ToLowerInvariant());
    //        }

    //        foreach (SCL.Option option in BuildCliOptions(LanguageManager.ConfigTypeForLanguage(language.Name), envConfig: envConfig))
    //        {
    //            languageCommand.AddOption(option);
    //            TrackIfEnum(option);
    //        }

    //        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigGenerate), envConfig: envConfig))
    //        {
    //            languageCommand.AddOption(option);
    //            TrackIfEnum(option);
    //        }

    //        generateCommand.AddCommand(languageCommand);
    //    }

    //    rootCommand.AddCommand(generateCommand);

    //    // create our interactive command
    //    SCL.Command interactiveCommand = new("interactive", "Launch into an interactive console.");
    //    foreach (SCL.Option option in BuildCliOptions(typeof(ConfigInteractive), envConfig: envConfig))
    //    {
    //        // note that 'global' here is just recursive DOWNWARD
    //        interactiveCommand.AddOption(option);
    //        TrackIfEnum(option);
    //    }

    //    // TODO(ginoc): Set the command handler
    //    rootCommand.AddCommand(interactiveCommand);

    //    // create our generate command
    //    SCL.Command webCommand = new("web", "Launch into a locally-hosted web UI.");
    //    foreach (SCL.Option option in BuildCliOptions(typeof(ConfigFluentUi), envConfig: envConfig))
    //    {
    //        // note that 'global' here is just recursive DOWNWARD
    //        webCommand.AddOption(option);
    //        TrackIfEnum(option);
    //    }

    //    // TODO(ginoc): Set the command handler
    //    rootCommand.AddCommand(webCommand);

    //    return rootCommand;

    //    void TrackIfEnum(SCL.Option option)
    //    {
    //        if (option.ValueType.IsEnum)
    //        {
    //            _optsWithEnums.Add(option);
    //            return;
    //        }

    //        if (option.ValueType.IsGenericType)
    //        {
    //            if (option.ValueType.GenericTypeArguments.First().IsEnum)
    //            {
    //                _optsWithEnums.Add(option);
    //            }

    //            return;
    //        }

    //        if (option.ValueType.IsArray)
    //        {
    //            if (option.ValueType.GetElementType()!.IsEnum)
    //            {
    //                _optsWithEnums.Add(option);
    //            }

    //            return;
    //        }
    //    }
    //}

    private static Dictionary<string, LanguageOptionInfo> _configMapsByLang = [];

    private record class LanguageOptionInfo
    {
        public required string Name { get; set; }

        public required Type ConfigType { get; set; }

        public List<PropertyOptionTuple> Properties { get; set; } = [];
    }

    private record class PropertyOptionTuple
    {
        public required PropertyInfo ConfigProp { get; set; }

        public required SCL.Option CommandOpt { get; set; }
    }

    private static IEnumerable<SCL.Option> BuildCliOptions(
        Type forType,
        Type? excludeFromType = null,
        IConfiguration? envConfig = null)
    {
        HashSet<string> inheritedPropNames = [];

        if (excludeFromType != null)
        {
            PropertyInfo[] exProps = excludeFromType.GetProperties();
            foreach (PropertyInfo exProp in exProps)
            {
                inheritedPropNames.Add(exProp.Name);
            }
        }

        object? configDefault = null;
        if (forType.IsAbstract)
        {
            throw new Exception($"Config type cannot be abstract! {forType.Name}");
        }

        configDefault = Activator.CreateInstance(forType);

        if (configDefault is not ICodeGenConfig config)
        {
            throw new Exception("Config type must implement ICodeGenConfig");
        }

        foreach (ConfigurationOption opt in config.GetOptions())
        {
            // need to configure default values
            if ((envConfig != null) &&
                (!string.IsNullOrEmpty(opt.EnvVarName)))
            {
                opt.CliOption.SetDefaultValueFactory(() => envConfig.GetSection(opt.EnvVarName).GetChildren().Select(c => c.Value));
            }
            else
            {
                opt.CliOption.SetDefaultValue(opt.DefaultValue);
            }

            yield return opt.CliOption;
        }
    }

    public static async Task<int> DoGenerate(SCL.Parsing.ParseResult pr)
    {
        try
        {
            string languageName = pr.CommandResult.Command.Name;

            if (!LanguageManager.TryGetLanguage(languageName, out ILanguage? language))
            {
                throw new Exception($"Could not find language type for {languageName}");
            }

            // get our language type
            Type langType = LanguageManager.TypeForLanguage(languageName);

            // get our language configuration type
            Type configType = LanguageManager.ConfigTypeForLanguage(language.Name);

            // create our configuration object
            object? configGeneric = Activator.CreateInstance(configType)
                ?? throw new Exception($"Could not create configuration object for {languageName} ({configType.Name})");

            if (configGeneric is not ICodeGenConfig config)
            {
                throw new Exception($"Config type must implement ICodeGenConfig, {languageName} ({configType.Name})");
            }

            if (config is not ConfigRoot rootConfig)
            {
                throw new Exception("Config type must inherit from ConfigRoot");
            }

            // parse the arguments into the configuration object
            config.Parse(pr);

            object? langObject = Activator.CreateInstance(langType)
                ?? throw new Exception($"Could not create language object for {languageName} ({langType.Name})");

            if (langObject is not ILanguage iLang)
            {
                throw new Exception($"Language type must implement ILanguage, {languageName} ({langType.Name})");
            }

            PackageLoader loader = new(config is ConfigRoot cr ? cr : null, new()
            {
                JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            DefinitionCollection? loaded = await loader.LoadPackages(rootConfig.Packages, null, rootConfig.FhirVersion)
                ?? throw new Exception($"Could not load packages: {string.Join(',', rootConfig.Packages)}");

            // check for a FHIR server URL
            if ((rootConfig is ConfigGenerate genConfig) &&
                !string.IsNullOrEmpty(genConfig.FhirServerUrl))
            {
                // parse any HTTP headers the caller has provided
                Dictionary<string, List<string>> headers = ParseHttpHeaderArgs(genConfig.FhirServerHeaders);

                // create our server connector
                ServerConnector serverConnector = new(genConfig.FhirServerUrl, genConfig.SmartConfigUrl, headers, loader);

                // try to get the capability statement
                if (serverConnector.TryGetCapabilities(out _, out _, out Hl7.Fhir.Model.CapabilityStatement? capStatement, out FhirReleases.FhirSequenceCodes? serverFhirVersion))
                {
                    genConfig.ServerCapabilities = capStatement;

                    if (genConfig.ResolveServerCanonicals)
                    {
                        serverConnector.TryResolveCanonicals(capStatement, loaded, genConfig.ResolveExternalCanonicals, out _);
                    }
                }

                // try to get SMART config
                if (serverConnector.TryGetSmartConfig(out _, out _, out SmartWellKnown? smartConfig))
                {
                    genConfig.ServerSmartConfig = smartConfig;
                }
                else if ((capStatement != null) && serverConnector.TryBuildSmartConfig(capStatement, out smartConfig))
                {
                    genConfig.ServerSmartConfig = smartConfig;
                }
            }

            // call the export method on the language object
            iLang.Export(config, loaded);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"RunGenerate <<< caught: {ex.Message}::{ex.InnerException.Message}");
            }
            else
            {
                Console.WriteLine($"RunGenerate <<< caught: {ex.Message}");
            }
        }

        return 0;
    }

    public static async Task<int> DoCompare(SCL.Parsing.ParseResult pr)
    {
        try
        {
            // create our configuration object
            ConfigCompare config = new();

            // parse the arguments into the configuration object
            config.Parse(pr);

            PackageLoader loaderLeft = new(config, new()
            {
                JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });


            DefinitionCollection? loadedLeft = await loaderLeft.LoadPackages(config.Packages)
                ?? throw new Exception($"Could not load left-hand-side packages: {string.Join(',', config.Packages)}");

            PackageLoader loaderRight = new(config, new()
            {
                JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            DefinitionCollection? loadedRight = await loaderLeft.LoadPackages(config.ComparePackages)
                ?? throw new Exception($"Could not load right-hand-side packages: {string.Join(',', config.Packages)}");

            PackageComparer comparer = new(config, loadedLeft, loadedRight);

            if (comparer.Compare() is PackageComparison pc)
            {
                if (config.SaveComparisonResult)
                {
                    comparer.WriteComparisonResultJson(pc);
                }

                if (config.NoOutput != true)
                {
                    comparer.WriteMarkdownFiles(pc);
                    comparer.WriteCrossVersionExtensionArtifacts(pc);
                }

                if (config.MapSaveStyle != ConfigCompare.ComparisonMapSaveStyle.None)
                {
                    comparer.WriteMapFiles(pc);
                }
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"DoCompare <<< caught: {ex.Message}::{ex.InnerException.Message}");
            }
            else
            {
                Console.WriteLine($"DoCompare <<< caught: {ex.Message}");
            }
        }

        return 0;
    }

    public static async Task<int> DoXVer(SCL.Parsing.ParseResult pr)
    {
        try
        {
            // create our configuration object
            ConfigXVer config = new();

            // parse the arguments into the configuration object
            config.Parse(pr);

            List<DefinitionCollection> packages = [];

            foreach (string directive in config.ComparePackages)
            {
                if (FhirPackageUtils.PackageIsFhirCore(directive))
                {
                    throw new Exception($"Package {directive} is not a FHIR Core package!");
                }

                // create a loader because these are all different FHIR core versions
                PackageLoader loader = new(config, new()
                {
                    JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
                });

                DefinitionCollection loaded = await loader.LoadPackages([directive])
                    ?? throw new Exception($"Could not load package: {directive}");

                packages.Add(loaded);
            }

            XVerProcessor xVerProcessor = new(config, packages);
            xVerProcessor.Compare();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"DoCompare <<< caught: {ex.Message}::{ex.InnerException.Message}");
            }
            else
            {
                Console.WriteLine($"DoCompare <<< caught: {ex.Message}");
            }
        }

        return 0;
    }

    private static Dictionary<string, List<string>> ParseHttpHeaderArgs(List<string> argValues)
    {
        if (argValues.Count == 0)
        {
            return [];
        }

        Dictionary<string, List<string>> headers = [];

        foreach (string header in argValues)
        {
            int separatorLocation = header.IndexOf('=');

            if (separatorLocation == -1)
            {
                // ignore
                continue;
            }

            string key = header[..separatorLocation].Trim();
            string value = header[(separatorLocation + 1)..].Trim();

            if (headers.TryGetValue(key, out List<string>? parsedValues))
            {
                parsedValues.Append(value);
            }
            else
            {
                headers.Add(key, [value]);
            }
        }

        return headers;
    }


    public static async Task<int> DoGenerateReflection(SCL.Parsing.ParseResult pr)
    {
        try
        {
            await Task.Delay(0);

            string languageName = pr.CommandResult.Command.Name;

            if (!_configMapsByLang.TryGetValue(languageName, out LanguageOptionInfo? langConfigMap))
            {
                throw new Exception($"Could not find language config map for {languageName}");
            }

            if (langConfigMap.ConfigType.IsAbstract)
            {
                throw new Exception($"Could not find language config map for {languageName}");
            }

            // create our configuration object
            object? langConfig = Activator.CreateInstance(langConfigMap.ConfigType)
                ?? throw new Exception($"Could not create configuration object for {languageName}");

            // iterate over the properties and get the values from the parse result
            foreach (PropertyOptionTuple map in langConfigMap.Properties)
            {
                string propName = map.ConfigProp.Name;

                object? configValue = map.ConfigProp.GetValue(langConfig);
                object? optValue = pr.GetValueForOption(map.CommandOpt);

                if (map.ConfigProp.PropertyType.IsArray &&
                    (configValue is Array configValueArray) &&
                    (optValue is IEnumerator enumerator))
                {
                    Type valueType = map.ConfigProp.PropertyType.GetElementType()!;
                    //Type valueType = configValueArray.GetType().GetElementType()!;

                    Type listType = typeof(List<>); // Represents List<T>
                    Type concreteType = listType.MakeGenericType(valueType); // Create a concrete type (e.g., List<string>)

                    object? tempList = Activator.CreateInstance(concreteType); // Instantiate the list

                    // use the enumerator to add values to the array
                    while (enumerator.MoveNext())
                    {
                        ((IList)tempList!).Add(enumerator.Current);

                        object? value = enumerator.Current;
                    }

                    Array parsedValues = Array.CreateInstance(valueType, ((IList)tempList!).Count);

                    foreach (object? item in (IList)tempList!)
                    {
                        parsedValues.SetValue(item, ((IList)tempList!).IndexOf(item));
                    }

                    map.ConfigProp.SetValue(langConfig, parsedValues);
                }
                else if (map.ConfigProp.PropertyType.IsGenericType)
                {
                    Type hashGenType = typeof(HashSet<>);

                    if (map.ConfigProp.PropertyType.GetGenericTypeDefinition() == hashGenType)
                    {
                        Type valueType = map.ConfigProp.PropertyType.GetElementType()!;

                        Type concreteType = hashGenType.MakeGenericType(valueType);

                    }

                    Type ieGenericType = typeof(IEnumerable<>);

                }
                else
                {
                    map.ConfigProp.SetValue(langConfig, pr.GetValueForOption(map.CommandOpt));
                }
            }


            Console.WriteLine("In generate....");
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"RunGenerate <<< caught: {ex.Message}::{ex.InnerException.Message}");
            }
            else
            {
                Console.WriteLine($"RunGenerate <<< caught: {ex.Message}");
            }
        }

        return 10;
    }

    ///// <summary>web UI.</summary>
    ///// <param name="config">           The configuration.</param>
    ///// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    ///// <returns>An asynchronous result that yields an int.</returns>
    //public static async Task<int> RunServer(ConfigFluentUi config, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        WebApplicationBuilder builder = null!;

    //        // when packaging as a dotnet tool, we need to do some directory shenanigans for the static content root
    //        string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location ?? AppContext.BaseDirectory) ?? string.Empty;
    //        if (!string.IsNullOrEmpty(root))
    //        {
    //            string webRoot = FindRelativeDir(root, "staticwebassets", false);

    //            if ((!string.IsNullOrEmpty(webRoot)) && Directory.Exists(webRoot))
    //            {
    //                builder = WebApplication.CreateBuilder(new WebApplicationOptions()
    //                {
    //                    WebRootPath = webRoot,
    //                });
    //            }
    //        }

    //        if (builder == null)
    //        {
    //            builder = WebApplication.CreateBuilder();
    //        }

    //        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

    //        builder.WebHost.UseStaticWebAssets();

    //        // add our configuration before starting things up
    //        builder.Services.AddSingleton(config);

    //        // add any locally-defined we want injected
    //        //builder.Services.AddSingleton<IExample, Example>();
    //        //builder.Services.AddHostedService<IExample>(sp => sp.GetRequiredService<IExample>());

    //        // Add services - do individually for control over middleware order
    //        builder.Services.AddCors();
    //        builder.Services.AddControllers();
    //        builder.Services.AddRazorComponents()
    //                .AddInteractiveServerComponents();

    //        //builder.Services.AddFluentUIComponents();

    //        // use the requested default port
    //        string localUrl = $"http://*:{config.UiListenPort}";

    //        builder.WebHost.UseUrls(localUrl);

    //        WebApplication app = builder.Build();

    //        // we want to essentially disable CORS
    //        app.UseCors(builder => builder
    //            .AllowAnyOrigin()
    //            .AllowAnyMethod()
    //            .AllowAnyHeader()
    //            .WithExposedHeaders(new[] { "Content-Location", "Location", "Etag", "Last-Modified" }));

    //        app.UseStaticFiles();
    //        app.UseRouting();
    //        app.MapControllers();

    //        // this is developer tooling - always respond with as much detail as we can
    //        app.UseDeveloperExceptionPage();

    //        // add UI components
    //        app.MapRazorComponents<App>()
    //            .AddInteractiveServerRenderMode();

    //        app.MapFallbackToPage("/_Host");

    //        // perform initialization of services
    //        //IExample example = app.Services.GetRequiredService<IExample>();
    //        //ExampleScenario.Init();

    //        // start the webserver
    //        _ = app.StartAsync();

    //        // perform any post-startup actions
    //        AfterServerStart(app, config);

    //        // wait for the server to shutdown
    //        await app.WaitForShutdownAsync(cancellationToken);

    //        // return success
    //        return 0;
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        return 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"fhir-candle <<< caught exception: {ex.Message}");
    //        return -1;
    //    }
    //}

    ///// <summary>Actions to take after the server has started.</summary>
    ///// <param name="app">   The application.</param>
    ///// <param name="config">The configuration.</param>
    //private static void AfterServerStart(WebApplication app, ConfigFluentUi config)
    //{
    //    Console.WriteLine("Press CTRL+C to exit");

    //    if (config.OpenBrowser == true)
    //    {
    //        string url = $"http://localhost:{config.UiListenPort}";

    //        LaunchBrowser(url);
    //    }
    //}

    /// <summary>Executes the browser operation.</summary>
    /// <param name="url">URL of the resource.</param>
    private static void LaunchBrowser(string url)
    {
        ProcessStartInfo psi = new();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            psi.FileName = "open";
            psi.ArgumentList.Add(url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            psi.FileName = "xdg-open";
            psi.ArgumentList.Add(url);
        }
        else
        {
            psi.FileName = "cmd";
            psi.ArgumentList.Add("/C");
            psi.ArgumentList.Add("start");
            psi.ArgumentList.Add(url);
        }

        Process.Start(psi);
    }


    ///// <summary>Searches for the FHIR specification directory.</summary>
    ///// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    /////  present.</exception>
    ///// <param name="dirName">       The name of the directory we are searching for.</param>
    ///// <param name="throwIfNotFound">(Optional) True to throw if not found.</param>
    ///// <returns>The found FHIR directory.</returns>
    //public static string FindRelativeDir(
    //    string startDir,
    //    string dirName,
    //    bool throwIfNotFound = true)
    //{
    //    string currentDir = string.IsNullOrEmpty(startDir) ? Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty : startDir;
    //    string testDir = Path.Combine(currentDir, dirName);

    //    while (!Directory.Exists(testDir))
    //    {
    //        currentDir = Path.GetFullPath(Path.Combine(currentDir, ".."));

    //        if (currentDir == Path.GetPathRoot(currentDir))
    //        {
    //            if (throwIfNotFound)
    //            {
    //                throw new DirectoryNotFoundException($"Could not find directory {dirName}!");
    //            }

    //            return string.Empty;
    //        }

    //        testDir = Path.Combine(currentDir, dirName);
    //    }

    //    return testDir;
    //}
}
