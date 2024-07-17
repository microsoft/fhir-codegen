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
using CommunityToolkit.Mvvm.Input;
using fhir_codegen.Views;
using Material.Icons;
using Material.Styles.Themes;

namespace fhir_codegen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private ViewModelBase _currentPage = new WelcomePageViewModel();

    [ObservableProperty]
    private NavigationItemTemplate? _selectedNavigationItem;

    //[ObservableProperty]
    //private MaterialTheme? _theme = Avalonia.Application.Current?.LocateMaterialTheme<MaterialTheme>();

    public MainWindowViewModel(object? args = null)
        :base()
    {
    }

    partial void OnSelectedNavigationItemChanged(NavigationItemTemplate? value)
    {
        if (value == null)
        {
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
        new NavigationItemTemplate
        {
            Target = typeof(WelcomePageViewModel),
            Label = WelcomePageViewModel.Label,
            IconKind = WelcomePageViewModel.IconKind,
        },
        new NavigationItemTemplate
        {
            Target = typeof(CoreComparisonViewModel),
            Label = CoreComparisonViewModel.Label,
            IconKind = CoreComparisonViewModel.IconKind,
        },
    };

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}

public class NavigationItemTemplate
{
    public required Type Target { get; init; }

    public required string Label { get; init; }

    public required MaterialIconKind IconKind { get; init; }
}
