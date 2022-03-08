// <copyright file="PackageManagerApiService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.PackageManager;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Server.Services;

/// <summary>A service for accessing package manager apis information.</summary>
public class PackageManagerApiService : IDisposable, IHostedService
{
    /// <summary>True if has disposed, false if not.</summary>
    private bool _hasDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirManager"/> class.
    /// </summary>
    public PackageManagerApiService()
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
        FhirManager.Init();

        //List<string> packageDirectives = new()
        //{
        //    "hl7.fhir.r4.core#latest",
        //};

        //// load FHIR versions
        //FhirManager.Current.LoadPackages(
        //    packageDirectives,
        //    false,
        //    true,
        //    true,
        //    false,
        //    string.Empty,
        //    out List<string> failedPackages);
    }

    /// <summary>Request package load.</summary>
    /// <param name="directive">The directive.</param>
    /// <param name="requestId">[out] Identifier for the request.</param>
    public void RequestPackageLoad(string directive, out PackageLoadStateEnum requestState)
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

    /// <summary>State for request.</summary>
    /// <param name="directive">The directive.</param>
    /// <returns>A RequestStateEnum.</returns>
    public PackageLoadStateEnum StateForRequest(string directive)
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

    /// <summary>Loads directive task.</summary>
    /// <param name="directive">The directive.</param>
    /// <param name="requestId">[out] Identifier for the request.</param>
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

        return Task.CompletedTask;
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
}
