namespace Spectre.Console;

/// <summary>
/// Represents the state of a diff line.
/// </summary>
public enum DiffLineState
{
    /// <summary>
    /// The line is unchanged.
    /// </summary>
    Unchanged,

    /// <summary>
    /// The line was inserted.
    /// </summary>
    Inserted,

    /// <summary>
    /// The line was deleted.
    /// </summary>
    Deleted,
}
