// <copyright file="ICGLogDataStore.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace fhir_codegen_app.Logging;

/// <summary>
/// Interface for a log data store that manages log entries.
/// </summary>
public interface ICGLogDataStore
{
    /// <summary>
    /// Gets the backing collection of log entries.
    /// </summary>
    ObservableCollection<CGLogEntry> Backing { get; }

    /// <summary>
    /// Adds a log entry to the backing collection asynchronously.
    /// </summary>
    /// <param name="entry">The log entry to add.</param>
    void AddLogEntry(CGLogEntry entry);

    /// <summary>
    /// Clears all log entries from the backing collection asynchronously.
    /// </summary>
    void Clear();
}
