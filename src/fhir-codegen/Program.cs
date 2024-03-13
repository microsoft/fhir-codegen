// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using SCL = System.CommandLine; // this is present to disambuite Option from System.CommandLine and Microsoft.FluentUI.AspNetCore.Components
using System.Diagnostics;
using System.Runtime.InteropServices;
using fhir_codegen.Components;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Health.Fhir.CodeGen.Lanugage;
using System.Reflection;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using System.CommandLine;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using System.CommandLine.Builder;
using System.Text;
using Microsoft.Extensions.Primitives;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using static Hl7.Fhir.Model.Group;
using System.Collections;

namespace fhir_codegen;

/// <summary>A program.</summary>
public class Program
{
    private static List<SCL.Option> _optsWithEnums = new();

    private static HashSet<string> _packageAliases = new()
    {
        "--package", "--load-package", "-p"
    };


    /// <summary>Main entry-point for this application.</summary>
    /// <param name="args">An array of command-line argument strings.</param>
    /// <returns>
    /// An asynchronous result that yields exit-code for the process - 0 for success, else an error
    /// code.
    /// </returns>
    public static async Task<int> Main(string[] args)
    {
        // setup our configuration defaults (environment > appsettings.json) - args will supercede
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

        switch (command)
        {
            case "generate":
                return await DoGenerate(pr);

            //case "interactive":
            //    return await DoInteractive(pr);

            //case "web":
            //    return await DoWeb(pr);

            default:
                return await parser.InvokeAsync(args);
        }
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
                    if (option.Aliases.Any())
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
                        IEnumerable<DescriptionAttribute> attrs = mem.GetCustomAttributes<DescriptionAttribute>(false);

                        sb.AppendLine($"  opt: {mem.Name}");
                        if (attrs.Any())
                        {
                            sb.AppendLine($"       {attrs.First().Description}");
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

            foreach (SCL.Option option in BuildCliOptions(LanguageManager.ConfigTypeForLanguage(language.Name), envConfig: envConfig, languageName: language.Name))
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

        // create our generate command
        SCL.Command webCommand = new("web", "Launch into a locally-hosted web UI.");
        foreach (SCL.Option option in BuildCliOptions(typeof(ConfigFluentUi), typeof(ConfigRoot), envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            webCommand.AddGlobalOption(option);
            TrackIfEnum(option);
        }

        // TODO(ginoc): Set the command handler
        rootCommand.AddCommand(webCommand);

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

    private static Dictionary<string, LanguageOptionInfo> _configMapsByLang = new();

    private record class LanguageOptionInfo
    {
        public required string Name { get; set; }

        public required Type ConfigType { get; set; }

        public List<PropertyOptionTuple> Properties { get; set; } = new();
    }

    private record class PropertyOptionTuple
    {
        public required PropertyInfo ConfigProp { get; set; }

        public required SCL.Option CommandOpt { get; set; }
    }

    /// <summary>Enumerates build CLI options in this collection.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="forType">       The Configuration type to generate options for.</param>
    /// <param name="exludeFromType">(Optional) Type of the exlude from.</param>
    /// <param name="envConfig">     (Optional) The environment configuration.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process build CLI options in this collection.
    /// </returns>
    private static IEnumerable<SCL.Option> BuildCliOptions(
        Type forType,
        Type? exludeFromType = null,
        IConfiguration? envConfig = null,
        string languageName = "")
    {
        LanguageOptionInfo? langConfigMap = null;

        if (!string.IsNullOrEmpty(languageName))
        {
            if (!_configMapsByLang.TryGetValue(languageName, out langConfigMap))
            {
                langConfigMap = new LanguageOptionInfo()
                {
                    Name = languageName,
                    ConfigType = forType,
                };

                _configMapsByLang.Add(languageName, langConfigMap);
            }
            else
            {
                // ensure we don' duplicate on multiple calls
                langConfigMap.Properties.Clear();
            }
        }

        HashSet<string> inheritedPropNames = new();

        if (exludeFromType != null)
        {
            PropertyInfo[] exProps = exludeFromType.GetProperties();
            foreach (PropertyInfo exProp in exProps)
            {
                inheritedPropNames.Add(exProp.Name);
            }
        }

        object? configDefault = null;
        if (!forType.IsAbstract)
        {
            configDefault = Activator.CreateInstance(forType);
        }

        // reflect over the forConfig object and get properties that have the ConfigOptionAttribute
        PropertyInfo[] props = forType.GetProperties();
        foreach (PropertyInfo prop in props)
        {
            // exclude inherited properties
            if (inheritedPropNames.Contains(prop.Name))
            {
                continue;
            }

            ConfigOptionAttribute? attr = prop.GetCustomAttribute<ConfigOptionAttribute>();
            if (attr == null)
            {
                continue;
            }

            // get the type of the property so we can create an argument of the matching type
            Type propType = prop.PropertyType;

            //if (propType.IsGenericType)
            //{
            //    propType = propType.GetGenericArguments().First();
            //}

            // create the base option
            SCL.Option? option = (SCL.Option?)Activator.CreateInstance(
                typeof(SCL.Option<>).MakeGenericType(propType),
                NameOrAlias(attr),
                attr.Description);

            if (option == null)
            {
                throw new Exception($"Could not create option for {prop.Name}");
            }

            // set additional properties
            option.Arity = ArityFromCard(attr.ArgArity);

            if (attr.ArgArity.StartsWith('1'))
            {
                option.IsRequired = true;
            }

            //if (attr.ArgArity.EndsWith('*'))
            //{
            //    option.AllowMultipleArgumentsPerToken = true;
            //}

            if (propType.IsGenericType || propType.IsArray)
            {
                if ((envConfig != null) &&
                    (!string.IsNullOrEmpty(attr.EnvName)))
                {
                    option.SetDefaultValueFactory(() => envConfig.GetSection(attr.EnvName).GetChildren().Select(c => c.Value));
                }
                else if (configDefault != null)
                {
                    option.SetDefaultValue(prop.GetValue(configDefault));
                }
            }
            else
            {
                if ((envConfig != null) &&
                    (!string.IsNullOrEmpty(attr.EnvName)))
                {
                    option.SetDefaultValueFactory(() => envConfig.GetValue(propType, attr.EnvName));

                    if (attr.EnvName.Equals("Load_Package"))
                    {
                        Console.Write("");
                    }
                }
                else if (configDefault != null)
                {
                    option.SetDefaultValue(prop.GetValue(configDefault));
                }
            }

            langConfigMap?.Properties.Add(new PropertyOptionTuple()
            {
                ConfigProp = prop,
                CommandOpt = option,
            });

            // add the option to the collection
            yield return option;
        }

        object NameOrAlias(ConfigOptionAttribute a) => string.IsNullOrEmpty(a.ArgName) ? a.ArgAliases : a.ArgName;

        ArgumentArity ArityFromCard(string c) => c switch
        {
            "0..1" => ArgumentArity.ZeroOrOne,
            "0..*" => ArgumentArity.ZeroOrMore,
            "1..1" => ArgumentArity.ExactlyOne,
            "1..*" => ArgumentArity.OneOrMore,
            _ => ArgumentArity.ZeroOrOne,
        };
    }

    public static async Task<int> DoGenerate(SCL.Parsing.ParseResult pr)
    {
        try
        {
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
            object? langConfig = Activator.CreateInstance(langConfigMap.ConfigType);

            if (langConfig is null)
            {
                throw new Exception($"Could not create configuration object for {languageName}");
            }

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

    /// <summary>web UI.</summary>
    /// <param name="config">           The configuration.</param>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields an int.</returns>
    public static async Task<int> RunServer(ConfigFluentUi config, CancellationToken cancellationToken)
    {
        try
        {
            WebApplicationBuilder builder = null!;

            // when packaging as a dotnet tool, we need to do some directory shenanigans for the static content root
            string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location ?? AppContext.BaseDirectory) ?? string.Empty;
            if (!string.IsNullOrEmpty(root))
            {
                string webRoot = FindRelativeDir(root, "staticwebassets", false);

                if ((!string.IsNullOrEmpty(webRoot)) && Directory.Exists(webRoot))
                {
                    builder = WebApplication.CreateBuilder(new WebApplicationOptions()
                    {
                        WebRootPath = webRoot,
                    });
                }
            }

            if (builder == null)
            {
                builder = WebApplication.CreateBuilder();
            }

            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

            builder.WebHost.UseStaticWebAssets();

            // add our configuration before starting things up
            builder.Services.AddSingleton(config);

            // add any locally-defined we want injected
            //builder.Services.AddSingleton<IExample, Example>();
            //builder.Services.AddHostedService<IExample>(sp => sp.GetRequiredService<IExample>());

            // Add services - do individually for control over middleware order
            builder.Services.AddCors();
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

            builder.Services.AddFluentUIComponents();

            // use the requested default port
            string localUrl = $"http://*:{config.UiListenPort}";

            builder.WebHost.UseUrls(localUrl);

            WebApplication app = builder.Build();

            // we want to essentially disable CORS
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders(new[] { "Content-Location", "Location", "Etag", "Last-Modified" }));

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            // this is developer tooling - always respond with as much detail as we can
            app.UseDeveloperExceptionPage();

            // add UI components
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapFallbackToPage("/_Host");

            // perform initialization of services
            //IExample example = app.Services.GetRequiredService<IExample>();
            //ExampleScenario.Init();

            // start the webserver
            _ = app.StartAsync();

            // perform any post-startup actions
            AfterServerStart(app, config);

            // wait for the server to shutdown
            await app.WaitForShutdownAsync(cancellationToken);

            // return success
            return 0;
        }
        catch (OperationCanceledException)
        {
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"fhir-candle <<< caught exception: {ex.Message}");
            return -1;
        }
    }

    /// <summary>Actions to take after the server has started.</summary>
    /// <param name="app">   The application.</param>
    /// <param name="config">The configuration.</param>
    private static void AfterServerStart(WebApplication app, ConfigFluentUi config)
    {
        Console.WriteLine("Press CTRL+C to exit");

        if (config.OpenBrowser == true)
        {
            string url = $"http://localhost:{config.UiListenPort}";

            LaunchBrowser(url);
        }
    }

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


    /// <summary>Searches for the FHIR specification directory.</summary>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="dirName">       The name of the directory we are searching for.</param>
    /// <param name="throwIfNotFound">(Optional) True to throw if not found.</param>
    /// <returns>The found FHIR directory.</returns>
    public static string FindRelativeDir(
        string startDir,
        string dirName,
        bool throwIfNotFound = true)
    {
        string currentDir = string.IsNullOrEmpty(startDir) ? Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty : startDir;
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

        return testDir;
    }
}
