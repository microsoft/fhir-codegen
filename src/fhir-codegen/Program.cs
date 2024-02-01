// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using cli = System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using fhir_codegen.Components;
using fhir_codegen.Runtime;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Health.Fhir.CodeGen.Lanugage;
using System.Reflection;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using System.CommandLine;

namespace fhir_codegen;

/// <summary>A program.</summary>
public class Program
{
    /// <summary>Language exporters by name.</summary>
    private static Dictionary<string, ILanguage> _languagesByName = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Main entry-point for this application.</summary>
    /// <param name="args">An array of command-line argument strings.</param>
    /// <returns>
    /// An asynchronous result that yields exit-code for the process - 0 for success, else an error
    /// code.
    /// </returns>
    public static async Task<int> Main(string[] args)
    {
        // setup our configuration (command line > environment > appsettings.json)
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // create our root command
        cli.RootCommand rootCommand = new("A utility for processing FHIR packages into other formats/languages.");
        foreach (cli.Option option in BuildRootOptions(configuration))
        {
            rootCommand.AddGlobalOption(option);
        }

        // create our generate command
        cli.Command generateCommand = new("generate", "Generate output from a FHIR package and exit.");
        foreach (cli.Option option in BuildGenerateOptions(configuration))
        {
            // note that 'global' here is just recursive DOWNWARD
            generateCommand.AddGlobalOption(option);
        }

        // iterate through languages and add them as subcommands
        LoadExportLanguages();
        foreach (ILanguage language in _languagesByName.Values)
        {
            cli.Command languageCommand = new(language.Name.ToLowerInvariant(), $"Generate {language.Name}");
            foreach (cli.Option lOpt in language.LanguageOptions.Values)
            {
                languageCommand.AddOption(lOpt);
            }

            generateCommand.AddCommand(languageCommand);
        }

        rootCommand.AddCommand(generateCommand);

        // create our interactive command
        cli.Command interactiveCommand = new("interactive", "Launch into an interactive console.");
        //foreach (cli.Option option in BuildRootOptions(configuration))
        //{
        //    // note that 'global' here is just recursive DOWNWARD
        //    generateCommand.AddGlobalOption(option);
        //}

        rootCommand.AddCommand(interactiveCommand);

        // create our generate command
        cli.Command webCommand = new("web", "Launch into a locally-hosted web UI.");
        //foreach (cli.Option option in BuildRootOptions(configuration))
        //{
        //    // note that 'global' here is just recursive DOWNWARD
        //    generateCommand.AddGlobalOption(option);
        //}

        rootCommand.AddCommand(webCommand);

        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>Enumerates build root options in this collection.</summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process build root options in this collection.
    /// </returns>
    private static IEnumerable<cli.Option> BuildRootOptions(IConfiguration configuration)
    {
        yield return new cli.Option<string>(
            name: "--fhir-cache",
            getDefaultValue: () => configuration.GetValue("Fhir_Cache", string.Empty) ?? string.Empty,
            "Location of the FHIR cache. Default is ~/.fhir.");

        yield return new cli.Option<string?>(
            aliases: new string[] { "--output-path", "-o" },
            getDefaultValue: () => string.Empty,
            "File or directory to write output.");

        yield return new cli.Option<string[]>(
            name: "--package",
            getDefaultValue: () => configuration.GetValue("Package", Array.Empty<string>()) ?? Array.Empty<string>(),
            "Package to load, can be specified multiple times.")
        {
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true,
        };
    }

    /// <summary>Enumerates build generate options in this collection.</summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process build generate options in this
    /// collection.
    /// </returns>
    private static IEnumerable<cli.Option> BuildGenerateOptions(IConfiguration configuration)
    {
        yield return new cli.Option<FhirArtifactClassEnum?>(
            name: "--export-structures",
            getDefaultValue: () => null,
            "Types of FHIR structures to export, default is all.")
        {
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true,
        };

        //yield return new cli.Option<FhirArtifactClassEnum[]>(
        //    name: "--export-structures",
        //    getDefaultValue: () => configuration.GetValue("Export_Structures", Array.Empty<FhirArtifactClassEnum>()) ?? Array.Empty<FhirArtifactClassEnum>(),
        //    "Types of FHIR structures to export, default is all.")
        //{
        //    Arity = ArgumentArity.ZeroOrMore,
        //    AllowMultipleArgumentsPerToken = true,
        //};
    }

    /// <summary>Loads export languages.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    public static void LoadExportLanguages()
    {
        if (_languagesByName.Any())
        {
            return;
        }

        IEnumerable<Type> lTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ILanguage)));

        foreach (Type localType in lTypes)
        {
            ILanguage? language = (ILanguage?)Activator.CreateInstance(localType);

            if (language == null)
            {
                throw new Exception($"Could not create instance of {localType.Name}");
            }

            if (_languagesByName.ContainsKey(language.Name))
            {
                continue;
            }

            _languagesByName.Add(language.Name, language);
        }

        lTypes = typeof(ILanguage).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ILanguage)));

        foreach (Type localType in lTypes)
        {
            ILanguage? language = (ILanguage?)Activator.CreateInstance(localType);

            if (language == null)
            {
                throw new Exception($"Could not create instance of {localType.Name}");
            }

            if (_languagesByName.ContainsKey(language.Name))
            {
                continue;
            }

            _languagesByName.Add(language.Name, language);
        }
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
