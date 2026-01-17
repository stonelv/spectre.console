namespace Spectre.Console;

/// <summary>
/// Represents a single line in a diff comparison.
/// </summary>
public class DiffLine
{
    /// <summary>
    /// Gets or sets the original line text.
    /// </summary>
    public string? Original { get; set; }

    /// <summary>
    /// Gets or sets the modified line text.
    /// </summary>
    public string? Modified { get; set; }

    /// <summary>
    /// Gets or sets the type of change.
    /// </summary>
    public DiffChangeType Type { get; set; }

    /// <summary>
    /// Gets or sets the original line number.
    /// </summary>
    public int OriginalLineNumber { get; set; }

    /// <summary>
    /// Gets or sets the modified line number.
    /// </summary>
    public int ModifiedLineNumber { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffLine"/> class.
    /// </summary>
    public DiffLine()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffLine"/> class.
    /// </summary>
    /// <param name="type">The type of change.</param>
    /// <param name="original">The original line.</param>
    /// <param name="modified">The modified line.</param>
    public DiffLine(DiffChangeType type, string? original, string? modified)
    {
        Type = type;
        Original = original;
        Modified = modified;
    }
}

/// <summary>
/// Specifies the type of diff change.
/// </summary>
public enum DiffChangeType
{
    /// <summary>
    /// The line is unchanged.
    /// </summary>
    Unchanged = 0,

    /// <summary>
    /// The line was added.
    /// </summary>
    Added = 1,

    /// <summary>
    /// The line was removed.
    /// </summary>
    Removed = 2,

    /// <summary>
    /// The line was modified.
    /// </summary>
    Modified = 3,
}
