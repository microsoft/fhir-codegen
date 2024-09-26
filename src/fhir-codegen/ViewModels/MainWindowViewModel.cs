// <copyright file="MainWindowViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using fhir_codegen.Models;
using fhir_codegen.Views;
using Material.Icons;
using Material.Styles.Themes;
using Microsoft.Health.Fhir.CodeGen.Configuration;

namespace fhir_codegen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private ViewModelBase _currentPage = new WelcomePageViewModel();

    [ObservableProperty]
    private NavigationItemTemplate? _selectedNavigationItem;

    public ConfigGui? Config { get; private set; } = null;

    public MainWindowViewModel(object? args = null)
        :base()
    {
        Config = (args is ConfigGui c)
            ? c
            : Ioc.Default.GetService<ConfigGui>();
    }

    partial void OnSelectedNavigationItemChanged(NavigationItemTemplate? value)
    {
        if (value == null)
        {
            return;
        }

        if (value.RequiresComparison)
        {
            ComparisonUiModel? comparisonUiModel = Ioc.Default.GetService<ComparisonUiModel>();
            if (comparisonUiModel?.Results != null)
            {
                NavigateTo(value.Target);
            }

            return;
        }

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
        templateForViewModel<CoreComparisonViewModel>(),
        templateForViewModel<CompareDetailsValueSetsViewModel>(requiresComparison: true),
    };
    private bool _disposedValue;

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
