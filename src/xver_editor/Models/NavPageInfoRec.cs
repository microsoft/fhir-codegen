namespace xver_editor.Models;

/// <summary>Information about the navigation page information.</summary>
public readonly record struct NavPageInfoRec
{
    /// <summary>Gets or initializes the display.</summary>
    public required string Display { get; init; }

    /// <summary>Gets or initializes the link.</summary>
    public required string Link { get; init; }
}
