// <copyright file="CGLogger.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace fhir_codegen_app.Logging;

/// <summary>  
/// A custom logger implementation for the FHIR code generation application.  
/// </summary>  
/// <remarks>  
/// This logger stores log entries in a custom data store and supports basic logging functionality.  
/// </remarks>  
public class CGLogger(string? name = null, Func<CGLoggerConfiguration>? getCurrentConfig = null, ICGLogDataStore? dataStore = null) : ILogger
{
    private readonly ICGLogDataStore _dataStore = dataStore ?? new CGLogDataStore();
    private readonly string _name = name ?? string.Empty;
    private readonly Func<CGLoggerConfiguration> _getCurrentConfig = getCurrentConfig ?? (() => new CGLoggerConfiguration());

    /// <summary>  
    /// Begins a logical operation scope.  
    /// </summary>  
    /// <typeparam name="TState">The type of the state.</typeparam>  
    /// <param name="state">The identifier for the scope.</param>  
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>  
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    /// <summary>  
    /// Checks if the given log level is enabled.  
    /// </summary>  
    /// <param name="logLevel">The log level to check.</param>  
    /// <returns>True if the log level is enabled; otherwise, false.</returns>  
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>  
    /// Logs a message with the specified log level and event ID.  
    /// </summary>  
    /// <typeparam name="TState">The type of the state.</typeparam>  
    /// <param name="logLevel">The log level.</param>  
    /// <param name="eventId">The event ID.</param>  
    /// <param name="state">The state to log.</param>  
    /// <param name="exception">The exception to log, if any.</param>  
    /// <param name="formatter">The function to create a log message from the state and exception.</param>  
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _dataStore.AddLogEntry(new CGLogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            State = state,
            Exception = exception,
        });
    }
}
