// <copyright file="PackageManagerWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace FhirCodeGenBlazor.Services;

/// <summary>A service for accessing package manager webs information.</summary>
public class PackageManagerWebService : IDisposable, IHostedService, IPackageManagerWebService
{
    /// <summary>True if has disposed, false if not.</summary>
    private bool _hasDisposed;

    /// <summary>
    /// Gets an enumerable collection that contains the keys in the read-only dictionary.
    /// </summary>
    /// <typeparam name="string">            Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord">Type of the package cache record.</typeparam>
    IEnumerable<string> IReadOnlyDictionary<string, PackageCacheRecord>.Keys =>
        FhirCacheService.Current.PackagesByDirective.Keys;

    /// <summary>
    /// Gets an enumerable collection that contains the values in the read-only dictionary.
    /// </summary>
    /// <typeparam name="string">            Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord">Type of the package cache record.</typeparam>
    IEnumerable<PackageCacheRecord> IReadOnlyDictionary<string, PackageCacheRecord>.Values =>
        FhirCacheService.Current.PackagesByDirective.Values;

    /// <summary>Gets the number of elements in the collection.</summary>
    /// <typeparam name="string">             Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord>">Type of the package cache record></typeparam>
    int IReadOnlyCollection<KeyValuePair<string, PackageCacheRecord>>.Count =>
        FhirCacheService.Current.PackagesByDirective.Count;

    /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
    /// <typeparam name="string">            Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord">Type of the package cache record.</typeparam>
    /// <param name="key">The key to locate.</param>
    /// <returns>The element that has the specified key in the read-only dictionary.</returns>
    PackageCacheRecord IReadOnlyDictionary<string, PackageCacheRecord>.this[string key] =>
        FhirCacheService.Current.PackagesByDirective[key];

    /// <summary>Gets the published releases.</summary>
    public List<FhirPackageCommon.PublishedReleaseInformation> PublishedReleases => FhirPackageCommon.CoreVersions;

    /// <summary>Occurs when On Changed.</summary>
    public event EventHandler<EventArgs>? OnChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageManagerWebService"/> class.
    /// </summary>
    public PackageManagerWebService()
    {
        _hasDisposed = false;
    }

    /// <summary>Initializes this object.</summary>
    public void Init()
    {
        string packageDirectory = string.Empty;

        if (string.IsNullOrEmpty(packageDirectory))
        {
            packageDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".fhir");
        }

        // initialize the internal FHIR Manager
        FhirCacheService.Init(packageDirectory);

        FhirCacheService.Current.OnChanged += FhirCachService_OnChanged;
    }

    /// <summary>FHIR cach service on changed.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">     Event information.</param>
    private void FhirCachService_OnChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    /// <summary>Request download.</summary>
    /// <param name="directive">   The directive.</param>
    /// <param name="branchName">  Name of the branch.</param>
    /// <param name="requestState">[out] State of the request.</param>
    public void RequestDownload(string directive, string branchName, out PackageLoadStateEnum requestState)
    {
        if (FhirCacheService.Current.TryGetPackageState(directive, out PackageLoadStateEnum state))
        {
            switch (state)
            {
                case PackageLoadStateEnum.Queued:
                case PackageLoadStateEnum.InProgress:
                case PackageLoadStateEnum.Loaded:
                case PackageLoadStateEnum.Parsed:
                    requestState = state;
                    return;

                case PackageLoadStateEnum.Unknown:
                case PackageLoadStateEnum.NotLoaded:
                case PackageLoadStateEnum.Failed:
                default:
                    break;
            }
        }

        requestState = PackageLoadStateEnum.Queued;
        Task.Run(() => DownloadDirectiveTask(directive, branchName));
    }

    /// <summary>Request package load.</summary>
    /// <param name="directive">The directive.</param>
    /// <param name="requestId">[out] Identifier for the request.</param>
    public void RequestLoad(string directive, out PackageLoadStateEnum requestState)
    {
        if (FhirCacheService.Current.TryGetPackageState(directive, out PackageLoadStateEnum state))
        {
            switch (state)
            {
                case PackageLoadStateEnum.Queued:
                case PackageLoadStateEnum.InProgress:
                case PackageLoadStateEnum.Loaded:
                case PackageLoadStateEnum.Parsed:
                    requestState = state;
                    return;

                case PackageLoadStateEnum.Unknown:
                case PackageLoadStateEnum.NotLoaded:
                case PackageLoadStateEnum.Failed:
                default:
                    break;
            }
        }

        requestState = PackageLoadStateEnum.Queued;
        Task.Run(() => LoadDirectiveTask(directive));
    }

    /// <summary>State of load request.</summary>
    /// <param name="directive">The directive.</param>
    /// <returns>A RequestStateEnum.</returns>
    public PackageLoadStateEnum LoadRequestStatus(string directive)
    {
        if (FhirCacheService.Current.TryGetPackageState(directive, out PackageLoadStateEnum state))
        {
            return state;
        }

        if (FhirManager.Current.IsLoaded(directive))
        {
            return PackageLoadStateEnum.Loaded;
        }

        return PackageLoadStateEnum.Unknown;
    }

    /// <summary>Gets all package manifests in this collection.</summary>
    /// <returns>
    /// An enumerator that allows foreach to be used to process all package manifests in this
    /// collection.
    /// </returns>
    public IEnumerable<PackageCacheRecord> GetAllPackageManifests()
    {
        return FhirCacheService.Current.PackagesByDirective.Values.ToArray();
    }

    /// <summary>
    /// Attempts to get package manifest a PackageCacheRecord? from the given string.
    /// </summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <param name="record">     [out] The record.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetPackageManifest(string packageName, string version, out PackageCacheRecord? record)
    {
        return TryGetPackageManifest(packageName + "#" + version, out record);
    }

    /// <summary>
    /// Attempts to get package manifest a PackageCacheRecord? from the given string.
    /// </summary>
    /// <param name="directive">The directive.</param>
    /// <param name="record">   [out] The record.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetPackageManifest(string directive, out PackageCacheRecord? record)
    {
        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            record = null;
            return false;
        }

        record = FhirCacheService.Current.PackagesByDirective[directive];
        return true;
    }

    /// <summary>
    /// Attempts to get package manifests an IEnumerable&lt;RegistryPackageManifest&gt; from the
    /// given string.
    /// </summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="manifests">  [out] The manifests.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetRegistryManifests(string packageName, out IEnumerable<RegistryPackageManifest> manifests)
    {
        return FhirCacheService.Current.TryGetPackageManifests(packageName, out manifests);
    }

    /// <summary>
    /// Attempts to get core ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetCoreCiPackageDetails(string branchName, out NpmPackageDetails details)
    {
        return FhirCacheService.Current.TryGetCoreCiPackageDetails(branchName, out details);
    }

    /// <summary>
    /// Attempts to get guide ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetGuideCiPackageDetails(string branchName, out NpmPackageDetails details)
    {
        return FhirCacheService.Current.TryGetGuideCiPackageDetails(branchName, out details);
    }

    /// <summary>Task to download a package based on directive.</summary>
    /// <param name="directive"> The directive.</param>
    /// <param name="branchName">Name of the branch.</param>
    /// <returns>An asynchronous result.</returns>
    private Task DownloadDirectiveTask(string directive, string branchName)
    {
        FhirCacheService.Current.FindOrDownload(directive, out _, false, branchName);

        // notify something has been downloaded
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>Loads directive task.</summary>
    /// <param name="directive">The directive.</param>
    /// <returns>An asynchronous result.</returns>
    private Task LoadDirectiveTask(string directive)
    {
        // load the requested package
        FhirManager.Current.LoadPackages(
            new string[] { directive },
            false,
            true,
            true,
            false,
            string.Empty,
            out _);

        // notify something has been loaded
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>Deletes the package described by directive.</summary>
    /// <param name="directive">The directive.</param>
    public void DeletePackage(string directive)
    {
        if (FhirManager.Current.IsLoaded(directive))
        {
            FhirManager.Current.UnloadPackage(directive);
        }

        FhirCacheService.Current.DeletePackage(directive);

        // notify something has been deleted
        StateHasChanged();
    }

    /// <summary>State has changed.</summary>
    public void StateHasChanged()
    {
        EventHandler<EventArgs>? handler = OnChanged;

        if (handler != null)
        {
            handler(this, new());
        }
    }

    /// <summary>Triggered when the application host is ready to start the service.</summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>An asynchronous result.</returns>
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be
    ///  graceful.</param>
    /// <returns>An asynchronous result.</returns>
    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// FhirModelComparer.Server.Services.FhirManagerService and optionally releases the managed
    /// resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to
    ///  release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_hasDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                FhirCacheService.Current.OnChanged -= FhirCachService_OnChanged;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _hasDisposed = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Determines whether the read-only dictionary contains an element that has the specified key.
    /// </summary>
    /// <typeparam name="string">            Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord">Type of the package cache record.</typeparam>
    /// <param name="key">The key to locate.</param>
    /// <returns>
    /// <see langword="true" /> if the read-only dictionary contains an element that has the
    /// specified key; otherwise, <see langword="false" />.
    /// </returns>
    bool IReadOnlyDictionary<string, PackageCacheRecord>.ContainsKey(string key) =>
        FhirCacheService.Current.PackagesByDirective.ContainsKey(key);

    /// <summary>Gets the value that is associated with the specified key.</summary>
    /// <typeparam name="string">            Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord">Type of the package cache record.</typeparam>
    /// <param name="key">  The key to locate.</param>
    /// <param name="value">[out] When this method returns, the value associated with the specified
    ///  key, if the key is found; otherwise, the default value for the type of the
    ///  <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <see langword="true" /> if the object that implements the
    /// <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an
    /// element that has the specified key; otherwise, <see langword="false" />.
    /// </returns>
    bool IReadOnlyDictionary<string, PackageCacheRecord>.TryGetValue(string key, out PackageCacheRecord value) =>
        FhirCacheService.Current.PackagesByDirective.TryGetValue(key, out value);

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <typeparam name="string">             Type of the string.</typeparam>
    /// <typeparam name="PackageCacheRecord>">Type of the package cache record></typeparam>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator<KeyValuePair<string, PackageCacheRecord>> IEnumerable<KeyValuePair<string, PackageCacheRecord>>.GetEnumerator() =>
        FhirCacheService.Current.PackagesByDirective.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
    /// the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        FhirCacheService.Current.PackagesByDirective.GetEnumerator();
}
