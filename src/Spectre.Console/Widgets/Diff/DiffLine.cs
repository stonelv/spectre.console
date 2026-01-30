namespace Spectre.Console;

/// <summary>
/// Represents a line in a diff.
/// </summary>
public sealed class DiffLine
{
    /// <summary>
    /// Gets the text of the line.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the state of the line (unchanged, inserted, or deleted).
    /// </summary>
    public DiffLineState State { get; }

    /// <summary>
    /// Gets the line number in the old text.
    /// </summary>
    public int? OldLineNumber { get; }

    /// <summary>
    /// Gets the line number in the new text.
    /// </summary>
    public int? NewLineNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffLine"/> class.
    /// </summary>
    /// <param name="text">The text of the line.</param>
    /// <param name="state">The state of the line.</param>
    /// <param name="oldLineNumber">The line number in the old text.</param>
    /// <param name="newLineNumber">The line number in the new text.</param>
    public DiffLine(string text, DiffLineState state, int? oldLineNumber, int? newLineNumber)
    {
        Text = text ?? string.Empty;
        State = state;
        OldLineNumber = oldLineNumber;
        NewLineNumber = newLineNumber;
    }
}
