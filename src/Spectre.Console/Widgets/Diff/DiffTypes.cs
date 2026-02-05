namespace Spectre.Console;

/// <summary>
/// Represents the type of a diff change.
/// </summary>
public enum DiffChangeType
{
    /// <summary>
    /// The line was deleted.
    /// </summary>
    Deleted,

    /// <summary>
    /// The line was inserted.
    /// </summary>
    Inserted,

    /// <summary>
    /// The line was unchanged.
    /// </summary>
    Unchanged,
}

/// <summary>
/// Represents a single diff change.
/// </summary>
public sealed class DiffChange
{
    /// <summary>
    /// Gets the type of change.
    /// </summary>
    public DiffChangeType Type { get; }

    /// <summary>
    /// Gets the line content.
    /// </summary>
    public string Line { get; }

    /// <summary>
    /// Gets the line number in the old text (if applicable).
    /// </summary>
    public int? OldLineNumber { get; }

    /// <summary>
    /// Gets the line number in the new text (if applicable).
    /// </summary>
    public int? NewLineNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffChange"/> class.
    /// </summary>
    /// <param name="type">The type of change.</param>
    /// <param name="line">The line content.</param>
    /// <param name="oldLineNumber">The line number in the old text.</param>
    /// <param name="newLineNumber">The line number in the new text.</param>
    public DiffChange(DiffChangeType type, string line, int? oldLineNumber = null, int? newLineNumber = null)
    {
        Type = type;
        Line = line;
        OldLineNumber = oldLineNumber;
        NewLineNumber = newLineNumber;
    }
}

/// <summary>
/// Represents the diff rendering mode.
/// </summary>
public enum DiffMode
{
    /// <summary>
    /// Renders the diff inline (deleted and inserted lines are shown sequentially).
    /// </summary>
    Inline,

    /// <summary>
    /// Renders the diff side-by-side (old and new text are shown in two columns).
    /// </summary>
    SideBySide,
}
