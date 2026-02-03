namespace Spectre.Console;

/// <summary>
/// Represents a single line in a diff.
/// </summary>
public sealed class DiffLine
{
    /// <summary>
    /// Gets the type of the diff line.
    /// </summary>
    public DiffLineType Type { get; }

    /// <summary>
    /// Gets the text content of the line.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the line number in the old text (for deleted lines).
    /// </summary>
    public int? OldLineNumber { get; }

    /// <summary>
    /// Gets the line number in the new text (for inserted lines).
    /// </summary>
    public int? NewLineNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffLine"/> class.
    /// </summary>
    /// <param name="type">The type of the diff line.</param>
    /// <param name="text">The text content.</param>
    /// <param name="oldLineNumber">The line number in the old text.</param>
    /// <param name="newLineNumber">The line number in the new text.</param>
    public DiffLine(DiffLineType type, string text, int? oldLineNumber = null, int? newLineNumber = null)
    {
        Type = type;
        Text = text ?? string.Empty;
        OldLineNumber = oldLineNumber;
        NewLineNumber = newLineNumber;
    }
}
