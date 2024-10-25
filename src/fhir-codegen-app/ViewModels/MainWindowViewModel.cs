// <copyright file="MainWindowViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using fhir_codegen_app.Logging;
using fhir_codegen_app.Views;
using Material.Icons;
using Microsoft.Health.Fhir.CodeGen.Configuration;

namespace fhir_codegen_app.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    public ICGLogDataStore DataStore { get; set; } = new CGLogDataStore();

    [ObservableProperty]
    private bool _isLogOpen = false;

    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private ViewModelBase _currentPage = new WelcomePageViewModel();

    [ObservableProperty]
    private NavigationItemTemplate? _selectedNavigationItem;

    public ICodeGenConfig? Config { get; private set; } = null;

    public MainWindowViewModel() : this(null) { }

    public MainWindowViewModel(object? args)
        : base()
    {
        Config = (args is ICodeGenConfig c)
            ? c
            : Ioc.Default.GetService<ICodeGenConfig>();

        switch (Config?.LaunchCommand)
        {
            case "generate":
                break;

            case "compare":
                break;

            case "xver":
                NavigateTo(typeof(XVerHomeViewModel));
                break;

            default:
                break;
        }

        LogWindow lw = new();
        lw.Show();
    }

    partial void OnSelectedNavigationItemChanged(NavigationItemTemplate? value)
    {
        if (value == null)
        {
            return;
        }

        //if (value.RequiresComparison)
        //{
        //    ComparisonUiModel? comparisonUiModel = Ioc.Default.GetService<ComparisonUiModel>();
        //    if (comparisonUiModel?.Results != null)
        //    {
        //        NavigateTo(value.Target);
        //    }

        //    return;
        //}

        NavigateTo(value.Target);
    }

    public void NavigateTo(Type targetViewModel, object? args = null)
    {
        ViewModelBase? target = (ViewModelBase?)Activator.CreateInstance(targetViewModel, args);
        if (target == null)
        {
            return;
        }

        CurrentPage = target;
    }

    [ObservableProperty]
    private List<NavigationItemTemplate> _navigationItems = new List<NavigationItemTemplate>
    {
        templateForViewModel<WelcomePageViewModel>(),
        //templateForViewModel<CoreComparisonViewModel>(),
        //templateForViewModel<CompareDetailsValueSetsViewModel>(requiresComparison: true),
    };

    private static NavigationItemTemplate templateForViewModel<T>(bool visible = true, bool requiresComparison = false)
       where T : INavigableViewModel
    {
        return new NavigationItemTemplate
        {
            Target = typeof(T),
            Label = typeof(T).GetProperty(nameof(INavigableViewModel.Label))?.GetValue(null)?.ToString()!,
            IconKind = (MaterialIconKind)typeof(T).GetProperty(nameof(INavigableViewModel.IconKind))?.GetValue(null)!,
            Indented = (bool)typeof(T).GetProperty(nameof(INavigableViewModel.Indented))?.GetValue(null)!,
            Visible = visible,
            RequiresComparison = requiresComparison,
        };
    }

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class NavigationItemTemplate
{
    public required Type Target { get; init; }

    public required string Label { get; init; }

    public required MaterialIconKind IconKind { get; init; }

    public required bool Indented { get; init; }

    public bool Visible { get; set; } = true;

    public bool RequiresComparison { get; init; }
}
