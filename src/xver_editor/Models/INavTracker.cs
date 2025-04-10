namespace xver_editor.Models;


/// <summary>Interface for navigation tracker.</summary>
public interface INavTracker
{
    /// <summary>Occurs when On Theme Changed.</summary>
    event EventHandler<EventArgs>? OnThemeChanged;

    /// <summary>Gets a value indicating whether this object is dark mode.</summary>
    bool IsDarkMode { get; }
    string UserName { get; }

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
