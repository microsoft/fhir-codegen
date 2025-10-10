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
using Fhir.CodeGen.Lib.Configuration;

namespace fhir_codegen.ViewModels;

/// <summary>
/// ViewModel for the main window, handling navigation and pane toggling.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether the pane is open.
    /// </summary>
    [ObservableProperty]
    private bool _isPaneOpen;

    /// <summary>
    /// Gets or sets the current page being displayed.
    /// </summary>
    [ObservableProperty]
    private ViewModelBase _currentPage = new WelcomePageViewModel();

    /// <summary>
    /// Gets or sets the selected navigation item.
    /// </summary>
    [ObservableProperty]
    private NavigationItemTemplate? _selectedNavigationItem;

    /// <summary>
    /// Gets the configuration for the GUI.
    /// </summary>
    public ConfigGui? Config { get; private set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="args">Optional arguments for initialization.</param>
    public MainWindowViewModel(object? args = null)
        : base()
    {
        Config = (args is ConfigGui c)
            ? c
            : Ioc.Default.GetService<ConfigGui>();
    }

    /// <summary>
    /// Handles changes to the selected navigation item.
    /// </summary>
    /// <param name="value">The new selected navigation item.</param>
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

    /// <summary>
    /// Navigates to the specified target ViewModel.
    /// </summary>
    /// <param name="targetViewModel">The target ViewModel type.</param>
    /// <param name="args">Optional arguments for the target ViewModel.</param>
    public void NavigateTo(Type targetViewModel, object? args = null)
    {
        ViewModelBase? target = (ViewModelBase?)Activator.CreateInstance(targetViewModel, args);
        if (target == null)
        {
            return;
        }

        CurrentPage = target;
    }

    /// <summary>
    /// Gets or sets the list of navigation items.
    /// </summary>
    [ObservableProperty]
    private List<NavigationItemTemplate> _navigationItems = new List<NavigationItemTemplate>
       {
           templateForViewModel<WelcomePageViewModel>(),
           templateForViewModel<CoreComparisonViewModel>(),
           templateForViewModel<CompareDetailsValueSetsViewModel>(requiresComparison: true),
       };
    private bool _disposedValue;

    /// <summary>
    /// Creates a navigation item template for the specified ViewModel type.
    /// </summary>
    /// <typeparam name="T">The ViewModel type.</typeparam>
    /// <param name="visible">Indicates whether the item is visible.</param>
    /// <param name="requiresComparison">Indicates whether the item requires comparison.</param>
    /// <returns>A new instance of <see cref="NavigationItemTemplate"/>.</returns>
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

    /// <summary>
    /// Toggles the pane open or closed.
    /// </summary>
    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    /// <summary>
    /// Disposes the resources used by the ViewModel.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
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

    /// <summary>
    /// Disposes the ViewModel.
    /// </summary>
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
