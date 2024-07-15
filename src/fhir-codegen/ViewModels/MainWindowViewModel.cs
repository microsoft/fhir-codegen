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
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fhir_codegen.Views;

namespace fhir_codegen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private UserControl _currentPage = new WelcomePageView();

    [ObservableProperty]
    private NavigationItemTemplate? _selectedNavigationItem;

    partial void OnSelectedNavigationItemChanged(NavigationItemTemplate? value)
    {
        if (value == null)
        {
            return;
        }

        UserControl? target = (UserControl?)Activator.CreateInstance(value.Target);

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
            Target = typeof(WelcomePageView),
            Label = WelcomePageViewModel.Label,
            IconGeometry = WelcomePageViewModel.IconGeometry,
        },
        new NavigationItemTemplate
        {
            Target = typeof(CoreComparisonView),
            Label = CoreComparisonViewModel.Label,
            IconGeometry = CoreComparisonViewModel.IconGeometry,
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

    public required StreamGeometry? IconGeometry { get; init; }
}
