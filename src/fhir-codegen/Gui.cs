// <copyright file="Gui.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using fhir_codegen.Models;
using fhir_codegen.ViewModels;
using fhir_codegen.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.CodeGen.Configuration;

namespace fhir_codegen;

internal class Gui
{
    [STAThread]
    public static int RunGui(System.CommandLine.Parsing.ParseResult pr)
    {
        ConfigGui config = new();
        config.Parse(pr);

        try
        {
            ServiceCollection services = new();
            ServiceProvider provider = services
                .AddSingleton<ConfigGui>(config)
                .AddSingleton<ComparisonUiModel>()
                //.AddTransient<HomePageViewModel>()
                //.AddTransient<HomePageView>()
                //.AddTransient<TextPageViewModel>()
                //.AddTransient<ChatPageViewModel>()
                //.AddTransient<ChatPageView>()
                .BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            BuildAvaloniaApp().StartWithClassicDesktopLifetime([], Avalonia.Controls.ShutdownMode.OnMainWindowClose);
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            // This is a known issue with Avalonia and Material
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }

        return 0;
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
}
