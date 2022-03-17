// <copyright file="PackageManagerApiService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;
using System.Collections;

namespace FhirCodeGenBlazor.Services;

/// <summary>A service for accessing package manager apis information.</summary>
public class SpecManagerWebService : IDisposable, IHostedService, ISpecManagerWebService
{
    /// <summary>True if has disposed, false if not.</summary>
    private bool _hasDisposed;

    /// <summary>
    /// Gets an enumerable collection that contains the keys in the read-only dictionary.
    /// </summary>
    /// <typeparam name="string">         Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo">Type of the FHIR version information.</typeparam>
    IEnumerable<string> IReadOnlyDictionary<string, FhirVersionInfo>.Keys =>
        FhirManager.Current.InfoByDirective.Keys;

    /// <summary>
    /// Gets an enumerable collection that contains the values in the read-only dictionary.
    /// </summary>
    /// <typeparam name="string">         Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo">Type of the FHIR version information.</typeparam>
    IEnumerable<FhirVersionInfo> IReadOnlyDictionary<string, FhirVersionInfo>.Values =>
        FhirManager.Current.InfoByDirective.Values;

    /// <summary>Gets the number of elements in the collection.</summary>
    /// <typeparam name="string">          Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo>">Type of the FHIR version info></typeparam>
    int IReadOnlyCollection<KeyValuePair<string, FhirVersionInfo>>.Count =>
        FhirManager.Current.InfoByDirective.Count;

    /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
    /// <typeparam name="string">         Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo">Type of the FHIR version information.</typeparam>
    /// <param name="key">The key to locate.</param>
    /// <returns>The element that has the specified key in the read-only dictionary.</returns>
    FhirVersionInfo IReadOnlyDictionary<string, FhirVersionInfo>.this[string key] =>
        FhirManager.Current.InfoByDirective[key];

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirManager"/> class.
    /// </summary>
    public SpecManagerWebService()
    {
        _hasDisposed = false;
    }

    /// <summary>Initializes this object.</summary>
    public void Init()
    {
        // initialize the internal FHIR Manager
        FhirManager.Init();
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

    /// <summary>
    /// Determines whether the read-only dictionary contains an element that has the specified key.
    /// </summary>
    /// <typeparam name="string">         Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo">Type of the FHIR version information.</typeparam>
    /// <param name="key">The key to locate.</param>
    /// <returns>
    /// <see langword="true" /> if the read-only dictionary contains an element that has the
    /// specified key; otherwise, <see langword="false" />.
    /// </returns>
    bool IReadOnlyDictionary<string, FhirVersionInfo>.ContainsKey(string key) =>
        FhirManager.Current.InfoByDirective.ContainsKey(key);

    /// <summary>Gets the value that is associated with the specified key.</summary>
    /// <typeparam name="string">         Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo">Type of the FHIR version information.</typeparam>
    /// <param name="key">  The key to locate.</param>
    /// <param name="value">[out] When this method returns, the value associated with the specified
    ///  key, if the key is found; otherwise, the default value for the type of the
    ///  <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <see langword="true" /> if the object that implements the
    /// <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an
    /// element that has the specified key; otherwise, <see langword="false" />.
    /// </returns>
    bool IReadOnlyDictionary<string, FhirVersionInfo>.TryGetValue(string key, out FhirVersionInfo value) =>
        FhirManager.Current.InfoByDirective.TryGetValue(key, out value);

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <typeparam name="string">          Type of the string.</typeparam>
    /// <typeparam name="FhirVersionInfo>">Type of the FHIR version info></typeparam>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator<KeyValuePair<string, FhirVersionInfo>> IEnumerable<KeyValuePair<string, FhirVersionInfo>>.GetEnumerator() =>
        FhirManager.Current.InfoByDirective.GetEnumerator();

    /// <summary>Gets the enumerator.</summary>
    /// <returns>The enumerator.</returns>

    IEnumerator IEnumerable.GetEnumerator() => FhirManager.Current.InfoByDirective.GetEnumerator();
}
