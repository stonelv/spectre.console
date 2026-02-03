using Spectre.Console.Rendering;

namespace Spectre.Console;

/// <summary>
/// A renderable diff component for visualizing differences between two texts.
/// </summary>
public sealed class Diff : Renderable
{
    private readonly string _oldText;
    private readonly string _newText;
    private DiffResult? _result;

    /// <summary>
    /// Gets or sets the display mode for the diff.
    /// </summary>
    public DiffMode Mode { get; set; } = DiffMode.Inline;

    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    public bool ShowLineNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets the style for inserted (added) lines.
    /// </summary>
    public Style InsertedStyle { get; set; } = new Style(foreground: Color.Green);

    /// <summary>
    /// Gets or sets the style for deleted (removed) lines.
    /// </summary>
    public Style DeletedStyle { get; set; } = new Style(foreground: Color.Red);

    /// <summary>
    /// Gets or sets the style for unchanged lines.
    /// </summary>
    public Style UnchangedStyle { get; set; } = Style.Plain;

    /// <summary>
    /// Gets or sets the style for line numbers.
    /// </summary>
    public Style LineNumberStyle { get; set; } = new Style(foreground: Color.Grey);

    /// <summary>
    /// Gets or sets the prefix for inserted lines.
    /// </summary>
    public string InsertedPrefix { get; set; } = "+ ";

    /// <summary>
    /// Gets or sets the prefix for deleted lines.
    /// </summary>
    public string DeletedPrefix { get; set; } = "- ";

    /// <summary>
    /// Gets or sets the prefix for unchanged lines.
    /// </summary>
    public string UnchangedPrefix { get; set; } = "  ";

    /// <summary>
    /// Gets or sets the width of the left column in side-by-side mode.
    /// </summary>
    public int? LeftColumnWidth { get; set; }

    /// <summary>
    /// Gets or sets the width of the right column in side-by-side mode.
    /// </summary>
    public int? RightColumnWidth { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    public Diff(string oldText, string newText)
    {
        _oldText = oldText ?? throw new ArgumentNullException(nameof(oldText));
        _newText = newText ?? throw new ArgumentNullException(nameof(newText));
    }

    /// <summary>
    /// Gets the diff result.
    /// </summary>
    private DiffResult GetResult()
    {
        return _result ??= DiffAlgorithm.Compute(_oldText, _newText);
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var result = GetResult();
        var maxLineLength = result.Lines.Max(l => l.Text.Length);

        // Account for prefixes and line numbers
        var prefixWidth = 2; // "+ " or "- " or "  "
        var lineNumberWidth = ShowLineNumbers ? 8 : 0; // "123: "

        var totalWidth = maxLineLength + prefixWidth + lineNumberWidth;

        return new Measurement(
            Math.Min(totalWidth, maxWidth),
            Math.Min(totalWidth, maxWidth));
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var result = GetResult();

        return Mode switch
        {
            DiffMode.Inline => RenderInline(result, options, maxWidth),
            DiffMode.SideBySide => RenderSideBySide(result, options, maxWidth),
            _ => RenderInline(result, options, maxWidth),
        };
    }

    private IEnumerable<Segment> RenderInline(DiffResult result, RenderOptions options, int maxWidth)
    {
        var segments = new List<Segment>();
        var maxLineNumber = Math.Max(
            result.Lines.Max(l => l.OldLineNumber ?? 0),
            result.Lines.Max(l => l.NewLineNumber ?? 0));
        var lineNumberWidth = ShowLineNumbers ? maxLineNumber.ToString().Length + 1 : 0;

        foreach (var line in result.Lines)
        {
            // Skip unchanged lines if they are empty and surrounded by changes
            // (optional optimization, currently showing all lines)

            // Add line number if enabled
            if (ShowLineNumbers)
            {
                var oldNum = line.OldLineNumber?.ToString() ?? "";
                var newNum = line.NewLineNumber?.ToString() ?? "";
                var lineNumStr = $"{oldNum.PadLeft(lineNumberWidth / 2)}{newNum.PadLeft(lineNumberWidth / 2 + 1)} ";
                segments.Add(new Segment(lineNumStr, LineNumberStyle));
            }

            // Add prefix and line content based on type
            switch (line.Type)
            {
                case DiffLineType.Inserted:
                    segments.Add(new Segment(InsertedPrefix, InsertedStyle));
                    segments.Add(new Segment(line.Text, InsertedStyle));
                    break;

                case DiffLineType.Deleted:
                    segments.Add(new Segment(DeletedPrefix, DeletedStyle));
                    segments.Add(new Segment(line.Text, DeletedStyle));
                    break;

                case DiffLineType.Unchanged:
                    segments.Add(new Segment(UnchangedPrefix, UnchangedStyle));
                    segments.Add(new Segment(line.Text, UnchangedStyle));
                    break;
            }

            segments.Add(Segment.LineBreak);
        }

        return segments;
    }

    private IEnumerable<Segment> RenderSideBySide(DiffResult result, RenderOptions options, int maxWidth)
    {
        var segments = new List<Segment>();
        var gutterWidth = 3; // " | "
        var prefixWidth = 2; // "+ " or "- "

        // Calculate column widths
        var availableWidth = maxWidth - gutterWidth;
        var leftWidth = LeftColumnWidth ?? availableWidth / 2;
        var rightWidth = RightColumnWidth ?? availableWidth / 2;

        // Ensure we don't exceed max width
        if (leftWidth + rightWidth + gutterWidth > maxWidth)
        {
            leftWidth = (maxWidth - gutterWidth) / 2;
            rightWidth = (maxWidth - gutterWidth) / 2;
        }

        // Build side-by-side view
        var oldLineData = new List<(string Text, Style Style)>();
        var newLineData = new List<(string Text, Style Style)>();

        foreach (var line in result.Lines)
        {
            switch (line.Type)
            {
                case DiffLineType.Inserted:
                    oldLineData.Add(("", Style.Plain));
                    newLineData.Add((line.Text, InsertedStyle));
                    break;

                case DiffLineType.Deleted:
                    oldLineData.Add((line.Text, DeletedStyle));
                    newLineData.Add(("", Style.Plain));
                    break;

                case DiffLineType.Unchanged:
                    oldLineData.Add((line.Text, UnchangedStyle));
                    newLineData.Add((line.Text, UnchangedStyle));
                    break;
            }
        }

        // Render rows
        var maxRows = Math.Max(oldLineData.Count, newLineData.Count);
        for (var i = 0; i < maxRows; i++)
        {
            var oldLine = i < oldLineData.Count ? oldLineData[i] : (Text: "", Style: Style.Plain);
            var newLine = i < newLineData.Count ? newLineData[i] : (Text: "", Style: Style.Plain);

            // Left column (old)
            var leftText = Truncate(oldLine.Text, leftWidth - prefixWidth);
            var leftPrefix = GetPrefixForStyle(oldLine.Style);
            segments.Add(new Segment(leftPrefix, oldLine.Style));
            segments.Add(new Segment(leftText.PadRight(leftWidth - prefixWidth), oldLine.Style));

            // Gutter
            segments.Add(new Segment(" | ", LineNumberStyle));

            // Right column (new)
            var rightText = Truncate(newLine.Text, rightWidth - prefixWidth);
            var rightPrefix = GetPrefixForStyle(newLine.Style);
            segments.Add(new Segment(rightPrefix, newLine.Style));
            segments.Add(new Segment(rightText.PadRight(rightWidth - prefixWidth), newLine.Style));

            segments.Add(Segment.LineBreak);
        }

        return segments;
    }

    private string GetPrefixForStyle(Style style)
    {
        if (style.Equals(InsertedStyle))
        {
            return InsertedPrefix;
        }

        if (style.Equals(DeletedStyle))
        {
            return DeletedPrefix;
        }

        return UnchangedPrefix;
    }

    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text ?? string.Empty;
        }

        return text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
    }
}
