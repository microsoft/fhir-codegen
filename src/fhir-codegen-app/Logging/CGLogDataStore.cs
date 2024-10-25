// <copyright file="CGLogDataStore.cs" company="Microsoft Corporation">
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
using CommunityToolkit.Mvvm.ComponentModel;

namespace fhir_codegen_app.Logging;

public class CGLogDataStore : ICGLogDataStore
{
    private static readonly ObservableCollection<CGLogEntry> _backing = [];

    /// <summary>
    /// Gets the backing collection of log entries.
    /// </summary>
    public ObservableCollection<CGLogEntry> Backing => _backing;

    /// <summary>
    /// Adds a log entry to the backing collection asynchronously.
    /// </summary>
    /// <param name="entry">The log entry to add.</param>
    public async void AddLogEntry(CGLogEntry entry)
    {
        await Dispatcher.UIThread.InvokeAsync(() => Backing.Add(entry));
    }

    /// <summary>
    /// Clears all log entries from the backing collection asynchronously.
    /// </summary>
    public async void Clear()
    {
        await Dispatcher.UIThread.InvokeAsync(() => Backing.Clear());
    }
}
