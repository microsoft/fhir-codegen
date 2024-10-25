// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.CommandLine.Parsing;
using Avalonia;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using fhir_codegen_app.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Language;
using static fhir_codegen_shared.LaunchUtils;

namespace fhir_codegen_app;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // setup our configuration defaults (environment > appsettings.json) - args will supersede
        IConfiguration envConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // in order to process help correctly we have to build a parser independent of the command
        Parser parser = BuildParser(envConfig);

        // attempt a parse
        ParseResult pr = parser.Parse(args);

        // any language subcommand is a generate command
        string command = pr.CommandResult.Command.Name;
        if (LanguageManager.HasLanguage(command))
        {
            command = "generate";
        }

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
            command = "help";
        }

        ICodeGenConfig config = ParseConfig(pr, command, new CGLoggerFactory());

        ServiceCollection services = new();
        ServiceProvider provider = services
            .AddSingleton<ICodeGenConfig>(config)
            .AddLogging(builder => builder.AddCGLogger())
            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(provider);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime([], Avalonia.Controls.ShutdownMode.OnMainWindowClose);
            //.StartWithClassicDesktopLifetime(args);
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            //.LogToTrace()
            .UseReactiveUI();
}
