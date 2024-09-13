// <copyright file="UiBlazor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SCL = System.CommandLine; // this is present to disambiguate Option from System.CommandLine and Microsoft.FluentUI.AspNetCore.Components

namespace fhir_codegen.WebUi;

public static class FluentUi
{
    public static int ListenPort { get; private set; } = 0;


    /// <summary>Executes the server operation.</summary>
    /// <param name="config">           The configuration.</param>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields an int.</returns>
    public static async Task<int> RunServer(
        SCL.Parsing.ParseResult pr,
        CancellationToken? cancellationToken = null)
    {
        try
        {
            ConfigFluentUi config = new();
            config.Parse(pr);

            WebApplicationBuilder builder = null!;

            // when packaging as a dotnet tool, we need to do some directory shenanigans for the static content root
            string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location ?? AppContext.BaseDirectory) ?? string.Empty;
            if (!string.IsNullOrEmpty(root))
            {
                string webRoot = FindRelativeDir(root, "staticwebassets", false);

                if (!string.IsNullOrEmpty(webRoot) && Directory.Exists(webRoot))
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

            builder.Services.AddCors();

            // add our configuration
            builder.Services.AddSingleton(config);

            //// add a FHIR-Store singleton, then register as a hosted service
            //builder.Services.AddSingleton<IFhirStoreManager, FhirStoreManager>();
            //builder.Services.AddHostedService<IFhirStoreManager>(sp => sp.GetRequiredService<IFhirStoreManager>());

            //// add a notification manager singleton, then register as a hosted service
            //builder.Services.AddSingleton<INotificationManager, NotificationManager>();
            //builder.Services.AddHostedService<INotificationManager>(sp => sp.GetRequiredService<INotificationManager>());

            //// add a package service singleton, then register as a hosted service
            //builder.Services.AddSingleton<IFhirPackageService, FhirPackageService>();
            //builder.Services.AddHostedService<IFhirPackageService>(sp => sp.GetRequiredService<IFhirPackageService>());

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddFluentUIComponents();

            string localUrl = $"http://localhost:{config.UiListenPort}";
            builder.WebHost.UseUrls(localUrl);
            //builder.WebHost.UseStaticWebAssets();

            WebApplication app = builder.Build();

            // we want to essentially disable CORS
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders(new[] { "Content-Location", "Location", "Etag", "Last-Modified" }));

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAntiforgery();
            app.MapControllers();

            // this is developer tooling - always respond with as much detail as we can
            app.UseDeveloperExceptionPage();

            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            //IFhirPackageService ps = app.Services.GetRequiredService<IFhirPackageService>();
            //IFhirStoreManager sm = app.Services.GetRequiredService<IFhirStoreManager>();
            //ISmartAuthManager am = app.Services.GetRequiredService<ISmartAuthManager>();

            // perform slow initialization of services
            //ps.Init();          // store manager requires Package Service to be initialized
            //sm.Init();          // store manager may need to download packages
            //am.Init();          // spin up authorization manager

            // run the server
            //await app.RunAsync(cancellationToken);
            _ = app.StartAsync();

            cancellationToken ??= new CancellationToken();

            AfterServerStart(app, config);
            await app.WaitForShutdownAsync((CancellationToken)cancellationToken);

            return 0;
        }
        catch (OperationCanceledException)
        {
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"fhir-codegen <<< caught exception: {ex.Message}");
            return -1;
        }
    }


    /// <summary>After server start.</summary>
    /// <param name="app">   The application.</param>
    /// <param name="config">The configuration.</param>
    private static void AfterServerStart(WebApplication app, ConfigFluentUi config)
    {
        app.Urls.ForEach(url => Console.WriteLine($"<<< Listening on: {url}"));

        if (config.UiListenPort == 0 && app.Urls.Count > 0)
        {
            // we need to extract the port from the URL
            string url = app.Urls.First();
            Match m = Regex.Match(url, @":(\d+)$");

            if (m.Success)
            {
                ListenPort = int.Parse(m.Groups[1].Value);
            }
        }

        Console.WriteLine("Press CTRL+C to exit");

        if (config.OpenBrowser == true)
        {
            string url = $"http://*:{config.UiListenPort}";

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
        if (dirName.Contains('~'))
        {
            // we have a relative path from the user directory
            dirName = dirName.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        if (Directory.Exists(dirName))
        {
            return Path.GetFullPath(dirName);
        }

        string currentDir = string.IsNullOrEmpty(startDir)
            ? Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty
            : startDir;
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
