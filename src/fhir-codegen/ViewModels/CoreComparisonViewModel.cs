// <copyright file="CoreComparisonViewModel.cs" company="Microsoft Corporation">
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
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace fhir_codegen.ViewModels;

public partial class CoreComparisonViewModel : ViewModelBase, INavigableViewModel
{
    public static string Label => "Compare FHIR Releases";
    public static StreamGeometry? IconGeometry => (Application.Current?.TryGetResource("book_question_mark_regular", out object? icon) ?? false) && icon is StreamGeometry sg
        ? sg
        : null;

    [ObservableProperty]
    private string _sourcePackageDirective = "";

    [ObservableProperty]
    private string _targetPackageDirective = "";
}
