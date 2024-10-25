// <copyright file="CGLogEntry.cs" company="Microsoft Corporation">
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
/// Represents a log entry with a timestamp, log level, event ID, state, and message.
/// </summary>
public record class CGLogEntry
{
    /// <summary>
    /// Gets the timestamp of the log entry.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets the log level of the log entry.
    /// </summary>
    public required LogLevel LogLevel { get; init; }

    /// <summary>
    /// Gets the event ID of the log entry.
    /// </summary>
    public required EventId EventId { get; init; }

    /// <summary>
    /// Gets the state associated with the log entry.
    /// </summary>
    public required object? State { get; init; }

    /// <summary>
    /// Gets the message of the log entry.
    /// </summary>
    public required Exception? Exception { get; init; }
}
