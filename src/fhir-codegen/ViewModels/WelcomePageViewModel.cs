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
using CommunityToolkit.Mvvm.DependencyInjection;
using Fhir.CodeGen.Packages.CacheClients;
using Fhir.CodeGen.Packages.Models;
using Material.Icons;
using Fhir.CodeGen.Lib.Configuration;

namespace fhir_codegen.ViewModels;

public partial class WelcomePageViewModel : ViewModelBase, INavigableViewModel
{
    public static string Label => "Home";
    public static MaterialIconKind IconKind => MaterialIconKind.Home;
    public static bool Indented => false;

    public record class InstalledPackageInfoRecord
    {
        public required CachedPackageRecord PackageRef { get; init; }
        public required PackageManifest? Manifest { get; init; }
        public required string DirectiveLiteral { get; init; }
        public required string PackageId { get; init; }
        public required string Version { get; init; }
        public required string ManifestVersion { get; init; }
        public required DateTimeOffset? PackageDate { get; init; }
        public required string FhirVersion { get; init; }
        public required string Description { get; init; }

    }

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

        FilteredInstalledPackages = new(packages.Where(p => p.DirectiveLiteral.Contains(filterValue, StringComparison.OrdinalIgnoreCase)));
    }

    public WelcomePageViewModel(object? args = null)
        :base()
    {
        // get the current configuration
        ConfigGui? config = (args is ConfigGui c)
            ? c
            : Ioc.Default.GetService<ConfigGui>();

        if (config == null)
        {
            throw new InvalidOperationException("No configuration found");
        }

        IFhirCacheClient cache = new DiskCacheClient(config.FhirCacheDirectory);

        List<InstalledPackageInfoRecord> installedPackages = new();

        // first, we need to get the installed package references
        IEnumerable<CachedPackageRecord> internalReferences = cache.ListCachedPackages().Result;

        // iterate over the internal references and convert them to the public references
        foreach (CachedPackageRecord cachedPackage in internalReferences)
        {
            PackageManifest? manifest = cachedPackage.Manifest;

            if (manifest is null)
            {
                continue;
            }

            installedPackages.Add(new()
            {
                PackageRef = cachedPackage,
                Manifest = manifest,
                DirectiveLiteral = cachedPackage.Directive.AnyDirective,
                PackageId = cachedPackage.Directive.PackageId ?? string.Empty,
                Version = cachedPackage.Directive.FhirCacheVersion?.ToString()
                    ?? cachedPackage.Directive.ResolvedVersion?.ToString()
                    ?? cachedPackage.Directive.RequestedVersion
                    ?? string.Empty,
                ManifestVersion = manifest?.Version ?? string.Empty,
                PackageDate = manifest?.PublicationDate,
                FhirVersion = string.Join(", ", manifest?.AnyFhirVersions ?? Enumerable.Empty<string>()),
                Description = manifest?.Description ?? string.Empty
            });
        }

        InstalledPackages = new(installedPackages);
    }
}
