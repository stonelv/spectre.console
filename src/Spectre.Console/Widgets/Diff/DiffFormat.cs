namespace Spectre.Console;

/// <summary>
/// Represents the format of the diff output.
/// </summary>
public enum DiffFormat
{
    /// <summary>
    /// Inline format shows changes inline.
    /// </summary>
    Inline,

    /// <summary>
    /// SideBySide format shows old and new content side by side.
    /// </summary>
    SideBySide,
}
