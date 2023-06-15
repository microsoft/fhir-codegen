// <copyright file="ServerConnectorWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Services;

public class ServerConnectorWebService : IDisposable, IHostedService, IServerConnectorService
{
    /// <summary>True if has disposed, false if not.</summary>
    private bool _hasDisposed;


    /// <summary>
    /// Initializes a new instance of the <see cref="SpecExporterWebService"/> class.
    /// </summary>
    public ServerConnectorWebService()
    {
        _hasDisposed = false;
    }

    /// <summary>Initializes this object.</summary>
    public void Init()
    {
    }

    /// <summary>Attempts to get server information.</summary>
    /// <param name="serverUrl">      URL of the server.</param>
    /// <param name="resolveExternal">True to resolve external.</param>
    /// <param name="headers">        The headers.</param>
    /// <param name="json">           [out] The JSON.</param>
    /// <param name="serverInfo">     [out] Information describing the server.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetServerInfo(
        string serverUrl,
        bool resolveExternal,
        Dictionary<string, IEnumerable<string>> headers,
        out string json,
        out FhirCapabiltyStatement serverInfo)
    {
        return ServerConnector.TryGetServerInfo(serverUrl, resolveExternal, headers, out json, out serverInfo);
    }

    /// <summary>Parse capability JSON.</summary>
    /// <param name="json">           The JSON.</param>
    /// <param name="smartConfigJson">(Optional) The smart configuration JSON.</param>
    /// <returns>A FhirCapabiltyStatement.</returns>
    public FhirCapabiltyStatement ParseCapabilityJson(string json, string smartConfigJson = "")
    {
        return ServerConnector.ParseCapabilityJson(json, smartConfigJson);
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
