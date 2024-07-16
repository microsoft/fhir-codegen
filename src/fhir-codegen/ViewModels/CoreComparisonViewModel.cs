// <copyright file="CoreComparisonViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hl7.Fhir.Utility;
using Material.Icons;
using Microsoft.Health.Fhir.CodeGen._ForPackages;
using Microsoft.Health.Fhir.CodeGen.Configuration;

namespace fhir_codegen.ViewModels;

public partial class CoreComparisonViewModel : ViewModelBase, INavigableViewModel
{
    public static string Label => "Compare FHIR Releases";
    public static MaterialIconKind IconKind => MaterialIconKind.Compare;
    //public static StreamGeometry? IconGeometry => (Application.Current?.TryGetResource("book_question_mark_regular", out object? icon) ?? false) && icon is StreamGeometry sg
    //    ? sg
    //    : null;

    [ObservableProperty]
    private string _header = "Compare FHIR Core Releases";

    [ObservableProperty]
    private string? _errorMessage = null;

    [ObservableProperty]
    private bool _processing = false;

    [ObservableProperty]
    private bool _onlyReleaseVersions = true;

    partial void OnOnlyReleaseVersionsChanged(bool value)
    {
        CorePackages = value ? _releasedCorePackages : _allCorePackages;
    }

    [ObservableProperty]
    private string _crossVersionDirectory = "git/fhir-cross-version";

    [ObservableProperty]
    private int _sourceIndex = 0;

    [ObservableProperty]
    private int _targetIndex = 0;

    [ObservableProperty]
    private List<string> _corePackages = [];

    private List<string> _releasedCorePackages = [];
    private List<string> _allCorePackages = [];

    private static readonly Regex _corePackageRegex = new Regex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.(core)$", RegexOptions.Compiled);
    private static readonly HashSet<string> _releaseVersions = [ "1.0.2", "3.0.2", "4.0.1", "4.3.0", "5.0.0", ];

    public CoreComparisonViewModel()
    {
        // get the current configuration
        ConfigGui? config = Gui.RunningConfiguration;

        if (config == null)
        {
            throw new InvalidOperationException("No configuration found");
        }

        DiskPackageCache cache = new(config.FhirCacheDirectory);

        // first, we need to get the installed package references
        IEnumerable<Firely.Fhir.Packages.PackageReference> internalReferences = cache.GetPackageReferences().Result;

        // iterate over the internal references and convert them to the public references
        foreach (Firely.Fhir.Packages.PackageReference pr in internalReferences)
        {
            if (string.IsNullOrEmpty(pr.Name) || string.IsNullOrEmpty(pr.Version))
            {
                continue;
            }

            if (!_corePackageRegex.IsMatch(pr.Name))
            {
                continue;
            }

            if (_releaseVersions.Contains(pr.Version))
            {
                _releasedCorePackages.Add(pr.Moniker);
            }

            _allCorePackages.Add(pr.Moniker);
        }

        CorePackages = _onlyReleaseVersions ? _releasedCorePackages : _allCorePackages;
    }

    [RelayCommand]
    private void RunComparison()
    {
        if (Processing == true)
        {
            return;
        }

        Processing = true;
        ErrorMessage = null;

        if (SourceIndex == TargetIndex)
        {
            ErrorMessage = "Source and target cannot be the same";
            Processing = false;
            return;
        }

        ConfigCompare compareOptions = new()
        {
            Packages = [ CorePackages[SourceIndex] ],
            ComparePackages = [ CorePackages[TargetIndex] ],
            CrossVersionMapSourcePath = CrossVersionDirectory,
            MapSaveStyle = ConfigCompare.ComparisonMapSaveStyle.None,
            NoOutput = true,
        };
    }

}
