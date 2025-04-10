using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.FluentUI.AspNetCore.Components;
using xver_editor.Components;
using xver_editor.Config;
using fhir_codegen_shared;
using System.CommandLine;
using Antlr4.Runtime.Misc;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using System.Threading;
using xver_editor.Services;

namespace xver_editor;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        // setup our configuration defaults (environment > appsettings.json) - args will supersede
        IConfiguration envConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // create our root command
        RootCommand rootCommand = new("FHIR XVer DB Editor.");
        foreach (Option option in LaunchUtils.BuildCliOptions(typeof(XverAppConfig), envConfig: envConfig))
        {
            // note that 'global' here is just recursive DOWNWARD
            rootCommand.AddGlobalOption(option);
            LaunchUtils.TrackIfEnum(option);
        }

        // in order to process help correctly we have to build a parser independent of the command
        Parser parser = LaunchUtils.BuildParser(envConfig, rootCommand);

        // attempt a parse
        ParseResult pr = parser.Parse(args);

        return await RunServer(pr);
    }

    public static async Task<int> RunServer(
        ParseResult pr,
        CancellationToken? cancellationToken = null)
    {
        try
        {
            XverAppConfig config = new();
            config.Parse(pr);

            WebApplicationBuilder builder = null!;

            // when packaging as a dotnet tool, we need to do some directory shenanigans for the static content root
            string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location ?? AppContext.BaseDirectory) ?? string.Empty;
            if (!string.IsNullOrEmpty(root))
            {
                string webRoot = config.FindRelativeDir(root, "staticwebassets", false);

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

            string appCacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "xver-editor-key-store");
            if (!Directory.Exists(appCacheDir))
            {
                Directory.CreateDirectory(appCacheDir);
            }

            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
            builder.WebHost.UseStaticWebAssets();

            builder.Services.AddDataProtection()
                .SetApplicationName("xver_editor")
                .PersistKeysToFileSystem(new DirectoryInfo(appCacheDir));
            builder.Services.AddCors();

            // add our configuration
            builder.Services.AddSingleton(config);

            builder.Services.AddSingleton(typeof(IXverService), new XVerService(config));
            builder.Services.AddHostedService<IXverService>(sp => (IXverService)sp.GetRequiredService(typeof(IXverService)));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddFluentUIComponents();

            string localUrl = $"http://*:{config.ListenPort}";

            builder.WebHost.UseUrls(localUrl);

            WebApplication app = builder.Build();

            // we want to essentially disable CORS
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders([ "Content-Location", "Location", "Etag", "Last-Modified" ]));

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAntiforgery();
            app.MapControllers();

            // this is developer tooling - always respond with as much detail as we can
            app.UseDeveloperExceptionPage();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // perform service initialization
            //IXverDbService dbService = app.Services.GetRequiredService<IXverDbService>();

            // perform slow initialization of services
            //dbService.Init();

            // run the server
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
            Console.WriteLine($"xver_editor <<< caught exception: {ex.Message}");
            return -1;
        }
    }

    /// <summary>After server start.</summary>
    /// <param name="app">   The application.</param>
    /// <param name="config">The configuration.</param>
    private static void AfterServerStart(WebApplication app, XverAppConfig config)
    {
        Console.WriteLine("Press CTRL+C to exit");

        if (config.OpenBrowser == true)
        {
            string url = $"http://localhost:{config.ListenPort}";

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
}
