using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using fhir_codegen.ViewModels;
using fhir_codegen.Views;
using Splat;

namespace fhir_codegen;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Since we use CommunityToolkit, the line below is needed to remove Avalonia data validation - otherwise we get duplicate validation errors
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = Ioc.Default.GetService<MainWindowViewModel>() ?? new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
