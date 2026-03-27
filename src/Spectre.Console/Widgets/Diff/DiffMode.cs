namespace Spectre.Console;

/// <summary>
/// Specifies the diff mode.
/// </summary>
public enum DiffMode
{
    /// <summary>
    /// Inline diff mode. Shows changes in a single column with indicators.
    /// </summary>
    Inline = 0,

    /// <summary>
    /// Side-by-side diff mode. Shows original and modified text in separate columns.
    /// </summary>
    SideBySide = 1,
}
