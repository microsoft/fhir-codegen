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
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using fhir_codegen.Models;
using Hl7.Fhir.Utility;
using Material.Icons;
using Microsoft.Health.Fhir.CodeGen._ForPackages;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace fhir_codegen.ViewModels;

public partial class CoreComparisonViewModel : ViewModelBase, INavigableViewModel
{
    public static string Label => "Compare FHIR Definitions";
    public static MaterialIconKind IconKind => MaterialIconKind.Compare;
    public static bool Indented => false;

    [ObservableProperty]
    private string _header = "Compare FHIR Core Releases";

    [ObservableProperty]
    private string? _message = null;

    [ObservableProperty]
    private bool _processing = false;

    [ObservableProperty]
    private bool _onlyListFhirCore = true;

    partial void OnOnlyListFhirCoreChanged(bool value)
    {
        Packages = value ? _corePackages : _installedPackages;
    }

    [ObservableProperty]
    private string _crossVersionDirectory = "git/fhir-cross-version";

    [ObservableProperty]
    private int _sourceReleaseIndex = 0;

    [ObservableProperty]
    private int _targetReleaseIndex = 0;

    [ObservableProperty]
    private string _sourceDirectory = string.Empty;     // "C:\\git\\version-modeling\\20191231\\hl7.fhir.r5.core#4.2.0";

    [ObservableProperty]
    private int _sourceDirectoryFhirVersionIndex = 2;

    [ObservableProperty]
    private string _targetDirectory = string.Empty;     // "C:\\git\\version-modeling\\20181227\\hl7.fhir.r4.core#4.0.1";

    [ObservableProperty]
    private int _targetDirectoryFhirVersionIndex = 2;

    [ObservableProperty]
    private string[] _fhirVersions = [ "DSTU2", "STU3", "R4", "R4B", "R5", "R6" ];

    [ObservableProperty]
    private string _saveDirectory = string.Empty;       // "C:\\git\\version-modeling\\20191231\\_prev_02_down";

    [ObservableProperty]
    private List<ValueSetComparison> _valueSetComparisons = [];

    [ObservableProperty]
    private List<PrimitiveTypeComparison> _primitiveComparisons = [];

    [ObservableProperty]
    private List<StructureComparison> _complexTypeComparisons = [];

    [ObservableProperty]
    private List<StructureComparison> _resourceComparisons = [];

    [ObservableProperty]
    private List<StructureComparison> _extensionComparisons = [];

    [ObservableProperty]
    private List<string> _packages = [];

    private List<string> _corePackages = [];
    private List<string> _installedPackages = [];

    private static readonly Regex _corePackageRegex = new Regex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.(core)$", RegexOptions.Compiled);
    private static readonly HashSet<string> _releaseVersions = [ "1.0.2", "3.0.2", "4.0.1", "4.3.0", "5.0.0", ];

    public CoreComparisonViewModel(object? args = null)
        : base()
    {
        // get the current configuration
        ConfigGui? config = (args is ConfigGui c)
            ? c
            : Ioc.Default.GetService<ConfigGui>();

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

            if (_corePackageRegex.IsMatch(pr.Name))
            {
                _corePackages.Add(pr.Moniker);
            }

            _installedPackages.Add(pr.Moniker);
        }

        Packages = _onlyListFhirCore ? _corePackages : _installedPackages;
    }

    [RelayCommand]
    private void RunPackageComparison()
    {
        if (Processing == true)
        {
            return;
        }

        Processing = true;
        Message = null;

        Task.Run(DoPackageComparison);
    }

    private async void DoPackageComparison()
    {
        Processing = true;
        Message = null;

        if (SourceReleaseIndex == TargetReleaseIndex)
        {
            Message = "Source and target cannot be the same";
            Processing = false;
            return;
        }

        if ((SourceReleaseIndex < 0) || (SourceReleaseIndex >= Packages.Count))
        {
            Message = "Invalid source selection!";
            Processing = false;
            return;
        }

        if ((TargetReleaseIndex < 0) || (TargetReleaseIndex >= Packages.Count))
        {
            Message = "Invalid target selection!";
            Processing = false;
            return;
        }

        string sourceMoniker = Packages[SourceReleaseIndex];
        string targetMoniker = Packages[TargetReleaseIndex];

        PackageLoader loader = new(new(), new());
        DefinitionCollection? source = await loader.LoadPackages([sourceMoniker]);

        if (source == null)
        {
            Message = "Failed to load source definitions";
            Processing = false;
            return;
        }

        DefinitionCollection? target = await loader.LoadPackages([targetMoniker]);

        if (target == null)
        {
            Message = "Failed to load target definitions";
            Processing = false;
            return;
        }

        ConfigCompare compareOptions = new()
        {
            MapSaveStyle = ConfigCompare.ComparisonMapSaveStyle.None,
            NoOutput = true,
        };

        PackageComparer comparer = new(compareOptions, source, target);

        PackageComparison results = comparer.Compare();

        ComparisonUiModel comparisonInfo = Ioc.Default.GetService<ComparisonUiModel>() ?? throw new Exception("Could not get required service ComparisonUiModel!");

        comparisonInfo.Source = source;
        comparisonInfo.Target = target;
        comparisonInfo.Results = results;

        //ValueSetComparisons = results.ValueSets.Values.SelectMany(l => l.Select(v => v)).ToList();
        //PrimitiveComparisons = results.PrimitiveTypes.Values.SelectMany(l => l.Select(v => v)).ToList();
        //ComplexTypeComparisons = results.ComplexTypes.Values.SelectMany(l => l.Select(v => v)).ToList();
        //ResourceComparisons = results.Resources.Values.SelectMany(l => l.Select(v => v)).ToList();
        //ExtensionComparisons = results.Extensions.Values.SelectMany(l => l.Select(v => v)).ToList();

        Processing = false;
        Message = $"Comparison of {sourceMoniker} and {targetMoniker} is complete! See Results tab.";
    }

    [RelayCommand]
    private void RunDirectoryComparison()
    {
        if (Processing == true)
        {
            return;
        }

        Task.Run(DoDirectoryComparison);
    }
    private async void DoDirectoryComparison()
    {
        Processing = true;
        Message = null;
        ValueSetComparisons = [];
        PrimitiveComparisons = [];
        ComplexTypeComparisons = [];
        ResourceComparisons = [];
        ExtensionComparisons = [];

        if (string.IsNullOrEmpty(SourceDirectory) || string.IsNullOrEmpty(TargetDirectory) || (SourceDirectory == TargetDirectory))
        {
            Message = "Source and target are required and cannot be the same";
            Processing = false;
            return;
        }

        PackageLoader loader = new(new());
        DefinitionCollection? source = await loader.LoadPackages([SourceDirectory], fhirVersion: FhirVersions[SourceDirectoryFhirVersionIndex]);

        if (source == null)
        {
            Message = "Failed to load source definitions";
            Processing = false;
            return;
        }

        DefinitionCollection? target = await loader.LoadPackages([TargetDirectory], fhirVersion: FhirVersions[TargetDirectoryFhirVersionIndex]);

        if (target == null)
        {
            Message = "Failed to load target definitions";
            Processing = false;
            return;
        }

        ConfigCompare compareOptions = string.IsNullOrEmpty(SaveDirectory)
            ? new()
            {
                MapSaveStyle = ConfigCompare.ComparisonMapSaveStyle.None,
                NoOutput = true,
            }
            : new()
            {
                OutputDirectory = SaveDirectory,
                MapSaveStyle = ConfigCompare.ComparisonMapSaveStyle.Source,
                NoOutput = false,
            };

        PackageComparer comparer = new(compareOptions, source, target);

        PackageComparison results = comparer.Compare();

        if (!string.IsNullOrEmpty(SaveDirectory))
        {
            //comparer.WriteComparisonResultJson(results);
            comparer.WriteMarkdownFiles(results);
            comparer.WriteMapFiles(results);
        }

        ValueSetComparisons = results.ValueSets.Values.SelectMany(l => l.Select(v => v)).ToList();
        PrimitiveComparisons = results.PrimitiveTypes.Values.SelectMany(l => l.Select(v => v)).ToList();
        ComplexTypeComparisons = results.ComplexTypes.Values.SelectMany(l => l.Select(v => v)).ToList();
        ResourceComparisons = results.Resources.Values.SelectMany(l => l.Select(v => v)).ToList();
        ExtensionComparisons = results.Extensions.Values.SelectMany(l => l.Select(v => v)).ToList();

        Processing = false;
        Message = null;
    }
}
