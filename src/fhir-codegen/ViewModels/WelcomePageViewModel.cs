// <copyright file="WelcomePageViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using Microsoft.Health.Fhir.CodeGen._ForPackages;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using static Microsoft.Health.Fhir.CodeGen._ForPackages.DiskPackageCache;

namespace fhir_codegen.ViewModels;

public partial class WelcomePageViewModel : ViewModelBase, INavigableViewModel
{
    public record class InstalledPackageInfoRecord
    {
        public required Firely.Fhir.Packages.PackageReference PackageRef { get; init; }
        public required PackageManifest? Manifest { get; init; }


        public required string Moniker { get; init; }
        public required string Name { get; init; }
        public required string Version { get; init; }
        public required string ManifestVersion { get; init; }
        public required string PackageDate { get; init; }
        public required string FhirVersion { get; init; }
        public required string Description { get; init; }

    }

    public static string Label => "Home";
    public static MaterialIconKind IconKind => MaterialIconKind.Home;
    //public static StreamGeometry? IconGeometry => (Application.Current?.TryGetResource("home_regular", out object? icon) ?? false) && icon is StreamGeometry sg
    //    ? sg
    //    : null;

    [ObservableProperty]
    private string _header = "FHIR Codegen - FHIR Cache Contents";

    [ObservableProperty]
    private string _tableFilter = string.Empty;

    partial void OnTableFilterChanged(string value) => RunUpdate(null, value);

    [ObservableProperty]
    private ObservableCollection<InstalledPackageInfoRecord> _filteredInstalledPackages = new();

    [ObservableProperty]
    private List<InstalledPackageInfoRecord> _installedPackages;

    partial void OnInstalledPackagesChanged(List<InstalledPackageInfoRecord> value) => RunUpdate(value, null);

    private void RunUpdate(List<InstalledPackageInfoRecord>? packages, string? filterValue)
    {
        Task.Run(() => UpdateFilteredPackageCollection(packages, filterValue));
    }

    private void UpdateFilteredPackageCollection(List<InstalledPackageInfoRecord>? packages, string? filterValue)
    {
        if (packages == null)
        {
            packages = InstalledPackages;
        }

        if (filterValue == null)
        {
            filterValue = TableFilter;
        }

        if (!packages.Any())
        {
            FilteredInstalledPackages.Clear();
            return;
        }

        if (string.IsNullOrWhiteSpace(filterValue))
        {
            FilteredInstalledPackages = new(packages);
            return;
        }

        FilteredInstalledPackages = new(packages.Where(p => p.Moniker.Contains(filterValue, StringComparison.OrdinalIgnoreCase)));
    }

    public WelcomePageViewModel()
    {
        // get the current configuration
        ConfigGui? config = Gui.RunningConfiguration;

        if (config == null)
        {
            throw new InvalidOperationException("No configuration found");
        }

        DiskPackageCache cache = new(config.FhirCacheDirectory);

        List<InstalledPackageInfoRecord> installedPackages = new();

        // first, we need to get the installed package references
        IEnumerable<Firely.Fhir.Packages.PackageReference> internalReferences = cache.GetPackageReferences().Result;

        // iterate over the internal references and convert them to the public references
        foreach (Firely.Fhir.Packages.PackageReference pr in internalReferences)
        {
            PackageManifest? manifest = cache.ReadManifestEx(pr).Result;

            installedPackages.Add(new()
            {
                PackageRef = pr,
                Manifest = manifest,
                Moniker = pr.Moniker,
                Name = pr.Name ?? string.Empty,
                Version = pr.Version ?? string.Empty,
                ManifestVersion = manifest?.Version ?? string.Empty,
                PackageDate = manifest?.Date ?? string.Empty,
                FhirVersion = string.Join(", ", manifest?.AllFhirVersions ?? Enumerable.Empty<string>()),
                Description = manifest?.Description ?? string.Empty
            });
        }

        InstalledPackages = new(installedPackages);
    }
}
