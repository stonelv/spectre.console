using Spectre.Console.Rendering;

namespace Spectre.Console;

/// <summary>
/// Represents a diff visualization for comparing two texts.
/// </summary>
public sealed class Diff : Renderable
{
    private readonly string _oldText;
    private readonly string _newText;
    private readonly List<DiffChange> _changes;

    /// <summary>
    /// Gets or sets the rendering mode.
    /// </summary>
    public DiffMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the style for deleted lines.
    /// </summary>
    public Style DeletedStyle { get; set; }

    /// <summary>
    /// Gets or sets the style for inserted lines.
    /// </summary>
    public Style InsertedStyle { get; set; }

    /// <summary>
    /// Gets or sets the style for unchanged lines.
    /// </summary>
    public Style UnchangedStyle { get; set; }

    /// <summary>
    /// Gets or sets the character used to mark deleted lines.
    /// </summary>
    public string DeletedMarker { get; set; }

    /// <summary>
    /// Gets or sets the character used to mark inserted lines.
    /// </summary>
    public string InsertedMarker { get; set; }

    /// <summary>
    /// Gets or sets the character used to mark unchanged lines.
    /// </summary>
    public string UnchangedMarker { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    public Diff(string oldText, string newText)
    {
        _oldText = oldText ?? string.Empty;
        _newText = newText ?? string.Empty;
        _changes = DiffAlgorithm.ComputeDiff(_oldText, _newText);

        Mode = DiffMode.Inline;
        DeletedStyle = new Style(Color.Red);
        InsertedStyle = new Style(Color.Green);
        UnchangedStyle = new Style(Color.Blue);
        DeletedMarker = "-";
        InsertedMarker = "+";
        UnchangedMarker = " ";
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var maxLineLength = _changes.Max(c => c.Line.Length) + 2;
        return new Measurement(maxLineLength, maxWidth);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return Mode == DiffMode.Inline ? RenderInline(options, maxWidth) : RenderSideBySide(options, maxWidth);
    }

    private IEnumerable<Segment> RenderInline(RenderOptions options, int maxWidth)
    {
        foreach (var change in _changes)
        {
            var style = GetStyle(change.Type);
            var marker = GetMarker(change.Type);

            var prefix = $"{marker} ";
            var line = change.Line;

            yield return new Segment(prefix, style);

            if (line.Length > 0)
            {
                yield return new Segment(line, style);
            }

            yield return Segment.LineBreak;
        }
    }

    private IEnumerable<Segment> RenderSideBySide(RenderOptions options, int maxWidth)
    {
        var oldLines = _oldText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var newLines = _newText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        var maxOldLineNumber = oldLines.Length;
        var maxNewLineNumber = newLines.Length;

        var oldLineNumberWidth = maxOldLineNumber.ToString().Length;
        var newLineNumberWidth = maxNewLineNumber.ToString().Length;

        var separatorWidth = 4;
        var availableWidth = maxWidth - oldLineNumberWidth - newLineNumberWidth - separatorWidth;
        var leftWidth = availableWidth / 2;
        var rightWidth = availableWidth - leftWidth;

        var leftLines = new List<string>();
        var rightLines = new List<string>();

        foreach (var change in _changes)
        {
            switch (change.Type)
            {
                case DiffChangeType.Deleted:
                    leftLines.Add(change.Line);
                    rightLines.Add(string.Empty);
                    break;
                case DiffChangeType.Inserted:
                    leftLines.Add(string.Empty);
                    rightLines.Add(change.Line);
                    break;
                case DiffChangeType.Unchanged:
                    leftLines.Add(change.Line);
                    rightLines.Add(change.Line);
                    break;
            }
        }

        for (int i = 0; i < leftLines.Count; i++)
        {
            var leftLine = leftLines[i];
            var rightLine = rightLines[i];

            var change = _changes[i];
            var leftStyle = change.Type == DiffChangeType.Deleted ? DeletedStyle :
                           change.Type == DiffChangeType.Unchanged ? UnchangedStyle : Style.Plain;
            var rightStyle = change.Type == DiffChangeType.Inserted ? InsertedStyle :
                            change.Type == DiffChangeType.Unchanged ? UnchangedStyle : Style.Plain;

            var leftNumber = change.OldLineNumber?.ToString() ?? string.Empty;
            var rightNumber = change.NewLineNumber?.ToString() ?? string.Empty;

            var leftNumberPadding = new string(' ', oldLineNumberWidth - leftNumber.Length);
            var rightNumberPadding = new string(' ', newLineNumberWidth - rightNumber.Length);

            yield return new Segment(leftNumberPadding + leftNumber + " ", new Style(Color.Grey));
            yield return new Segment(Truncate(leftLine, leftWidth), leftStyle);
            yield return new Segment(new string(' ', separatorWidth), Style.Plain);
            yield return new Segment(rightNumberPadding + rightNumber + " ", new Style(Color.Grey));
            yield return new Segment(Truncate(rightLine, rightWidth), rightStyle);
            yield return Segment.LineBreak;
        }
    }

    private Style GetStyle(DiffChangeType type)
    {
        return type switch
        {
            DiffChangeType.Deleted => DeletedStyle,
            DiffChangeType.Inserted => InsertedStyle,
            DiffChangeType.Unchanged => UnchangedStyle,
            _ => Style.Plain
        };
    }

    private string GetMarker(DiffChangeType type)
    {
        return type switch
        {
            DiffChangeType.Deleted => DeletedMarker,
            DiffChangeType.Inserted => InsertedMarker,
            DiffChangeType.Unchanged => UnchangedMarker,
            _ => " "
        };
    }

    private string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return text.PadRight(maxLength);
        }

        return text.Substring(0, maxLength - 1) + "…";
    }
}
