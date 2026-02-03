namespace Spectre.Console;

/// <summary>
/// Represents the type of a diff line.
/// </summary>
public enum DiffLineType
{
    /// <summary>
    /// The line is unchanged.
    /// </summary>
    Unchanged,

    /// <summary>
    /// The line was inserted (added).
    /// </summary>
    Inserted,

    /// <summary>
    /// The line was deleted (removed).
    /// </summary>
    Deleted,
}
