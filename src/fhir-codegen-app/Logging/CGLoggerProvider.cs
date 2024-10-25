// <copyright file="CGLoggerProvider.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace fhir_codegen_app.Logging;

[ProviderAlias("CGLogger")]
public class CGLoggerProvider : ILoggerProvider, IDisposable
{
    /// <summary>
    /// A thread-safe dictionary to hold loggers.
    /// </summary>
    protected readonly ConcurrentDictionary<string, CGLogger> _loggers = new();

    /// <summary>
    /// Indicates whether the object has been disposed.
    /// </summary>
    private bool _disposedValue;

    /// <summary>
    /// Creates a logger for the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>An instance of <see cref="ILogger"/>.</returns>
    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new CGLogger(name));

    /// <summary>
    /// Releases the unmanaged resources used by the CGLoggerProvider and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                _loggers.Clear();
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer.
            // Set large fields to null.
            _disposedValue = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the CGLoggerProvider.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
