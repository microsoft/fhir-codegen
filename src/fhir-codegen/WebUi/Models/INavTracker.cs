// <copyright file="INavTracker.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace fhir_codegen.WebUi.Models;

public readonly record struct NavPageInfoRec
{
    /// <summary>Gets or initializes the display.</summary>
    public required string Display { get; init; }

    /// <summary>Gets or initializes the link.</summary>
    public required string Link { get; init; }
}


/// <summary>Interface for navigation tracker.</summary>
public interface INavTracker
{
    /// <summary>Occurs when On Theme Changed.</summary>
    event EventHandler<EventArgs>? OnThemeChanged;

    /// <summary>Gets a value indicating whether this object is dark mode.</summary>
    bool IsDarkMode { get; }

    /// <summary>Notifies a navigation.</summary>
    /// <param name="pages">The pages.</param>
    void NotifyNav(NavPageInfoRec[] pages);

    /// <summary>Logs to the JavaScript console.</summary>
    /// <param name="message">The message.</param>
    /// <remarks>This call does not need to be awaited, but needs to be async so that it can resolve the JS runtime call.</remarks>
    Task JsLogAsync(string message);

    /// <summary>Uses JS navigator.clipboard.writeText to copy contents to the clipboard.</summary>
    /// <param name="content">The content.</param>
    /// <returns>An asynchronous result.</returns>
    Task JsClipboardCopy(string content);
}
