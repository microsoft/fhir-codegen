// <copyright file="PackageDiffWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using FhirCodeGenBlazor.Models;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Services;

/// <summary>A service for accessing package difference webs information.</summary>
public class PackageDiffWebService : IDisposable, IHostedService, IPackageDiffWebService
{
    /// <summary>True if has disposed, false if not.</summary>
    private bool _hasDisposed;

    /// <summary>Occurs when On Difference Completed.</summary>
    public event EventHandler<DiffCompletedEventArgs>? OnDiffCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageDiffWebService"/> class.
    /// </summary>
    public PackageDiffWebService()
    {
        _hasDisposed = false;
    }

    /// <summary>Initializes this object.</summary>
    public void Init()
    {
    }

    /// <summary>Difference has completed.</summary>
    /// <param name="packageKeyA">The package key a.</param>
    /// <param name="packageKeyB">The package key b.</param>
    /// <param name="results">    The results.</param>
    public void DiffHasCompleted(string packageKeyA, string packageKeyB, DiffResults? results)
    {
        EventHandler<DiffCompletedEventArgs>? handler = OnDiffCompleted;

        if (handler != null)
        {
            handler(this, new(packageKeyA, packageKeyB, results));
        }
    }

    /// <summary>Request difference.</summary>
    /// <param name="A">      An IPackageExportable to process.</param>
    /// <param name="B">      An IPackageExportable to process.</param>
    /// <param name="options">Options for controlling the operation.</param>
    /// <returns>An asynchronous result.</returns>
    public Task RequestDiff(IPackageExportable A, IPackageExportable B, DifferOptions options)
    {
        Task diffTask = new Task(() =>
        {
            Differ differ = new Differ(A, B, options);
            DiffResults results = differ.GenerateDiff();
            DiffHasCompleted(
                A.PackageName + "#" + A.VersionString,
                B.PackageName + "#" + B.VersionString,
                results);
        });

        return diffTask;
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
