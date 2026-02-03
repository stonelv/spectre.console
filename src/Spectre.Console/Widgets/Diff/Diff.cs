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

        // Handle empty result
        if (result.Lines.Count == 0)
        {
            return new Measurement(0, 0);
        }

        var maxLineLength = 0;
        foreach (var line in result.Lines)
        {
            if (line.Text != null && line.Text.Length > maxLineLength)
            {
                maxLineLength = line.Text.Length;
            }
        }

        // Account for prefixes and line numbers
        var prefixWidth = 2; // "+ " or "- " or "  "
        var lineNumberWidth = ShowLineNumbers ? 10 : 0; // "  - 1 + 2 "

        var totalWidth = maxLineLength + prefixWidth + lineNumberWidth;
        var minWidth = Math.Min(totalWidth, maxWidth);

        return new Measurement(minWidth, minWidth);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var result = GetResult();

        // Handle empty result
        if (result.Lines.Count == 0)
        {
            return Array.Empty<Segment>();
        }

        // Ensure minimum width for rendering
        var minRenderWidth = 10;
        var effectiveMaxWidth = Math.Max(maxWidth, minRenderWidth);

        return Mode switch
        {
            DiffMode.Inline => RenderInline(result, options, effectiveMaxWidth),
            DiffMode.SideBySide => RenderSideBySide(result, options, effectiveMaxWidth),
            _ => RenderInline(result, options, effectiveMaxWidth),
        };
    }

    private IEnumerable<Segment> RenderInline(DiffResult result, RenderOptions options, int maxWidth)
    {
        var segments = new List<Segment>();

        // Calculate line number column widths
        var maxOldLineNumber = 0;
        var maxNewLineNumber = 0;
        foreach (var line in result.Lines)
        {
            if (line.OldLineNumber.HasValue && line.OldLineNumber.Value > maxOldLineNumber)
            {
                maxOldLineNumber = line.OldLineNumber.Value;
            }
            if (line.NewLineNumber.HasValue && line.NewLineNumber.Value > maxNewLineNumber)
            {
                maxNewLineNumber = line.NewLineNumber.Value;
            }
        }

        var oldLineNumWidth = maxOldLineNumber.ToString().Length;
        var newLineNumWidth = maxNewLineNumber.ToString().Length;
        var lineNumWidth = Math.Max(oldLineNumWidth, newLineNumWidth);

        foreach (var line in result.Lines)
        {
            // Add line number if enabled
            if (ShowLineNumbers)
            {
                var oldNumStr = line.OldLineNumber?.ToString() ?? string.Empty;
                var newNumStr = line.NewLineNumber?.ToString() ?? string.Empty;
                var lineNumStr = $"{oldNumStr.PadLeft(lineNumWidth)} {newNumStr.PadLeft(lineNumWidth)} ";
                segments.Add(new Segment(lineNumStr, LineNumberStyle));
            }

            // Add prefix and line content based on type
            var lineText = line.Text ?? string.Empty;
            switch (line.Type)
            {
                case DiffLineType.Inserted:
                    segments.Add(new Segment(InsertedPrefix, InsertedStyle));
                    segments.Add(new Segment(lineText, InsertedStyle));
                    break;

                case DiffLineType.Deleted:
                    segments.Add(new Segment(DeletedPrefix, DeletedStyle));
                    segments.Add(new Segment(lineText, DeletedStyle));
                    break;

                case DiffLineType.Unchanged:
                    segments.Add(new Segment(UnchangedPrefix, UnchangedStyle));
                    segments.Add(new Segment(lineText, UnchangedStyle));
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
        var minContentWidth = 5; // Minimum content width to render anything meaningful

        // Calculate column widths with safety checks
        var availableWidth = maxWidth - gutterWidth;
        if (availableWidth < minContentWidth * 2)
        {
            // Not enough space, fall back to inline mode
            return RenderInline(result, options, maxWidth);
        }

        var leftWidth = LeftColumnWidth ?? availableWidth / 2;
        var rightWidth = RightColumnWidth ?? availableWidth / 2;

        // Ensure we don't exceed max width and each column has minimum width
        if (leftWidth + rightWidth + gutterWidth > maxWidth)
        {
            var halfWidth = (maxWidth - gutterWidth) / 2;
            leftWidth = halfWidth;
            rightWidth = halfWidth;
        }

        // Ensure minimum column width
        leftWidth = Math.Max(leftWidth, minContentWidth);
        rightWidth = Math.Max(rightWidth, minContentWidth);

        // Re-adjust if needed
        if (leftWidth + rightWidth + gutterWidth > maxWidth)
        {
            var halfWidth = (maxWidth - gutterWidth) / 2;
            leftWidth = halfWidth;
            rightWidth = halfWidth;
        }

        // Build side-by-side view data structure with explicit type information
        var lineData = new List<(string OldText, string NewText, DiffLineType Type)>();

        foreach (var line in result.Lines)
        {
            switch (line.Type)
            {
                case DiffLineType.Inserted:
                    lineData.Add((string.Empty, line.Text ?? string.Empty, DiffLineType.Inserted));
                    break;

                case DiffLineType.Deleted:
                    lineData.Add((line.Text ?? string.Empty, string.Empty, DiffLineType.Deleted));
                    break;

                case DiffLineType.Unchanged:
                    lineData.Add((line.Text ?? string.Empty, line.Text ?? string.Empty, DiffLineType.Unchanged));
                    break;
            }
        }

        // Render rows
        foreach (var row in lineData)
        {
            // Left column (old)
            var leftStyle = GetStyleForLineType(row.Type == DiffLineType.Inserted ? DiffLineType.Unchanged : row.Type);
            var leftPrefix = row.Type == DiffLineType.Inserted ? UnchangedPrefix :
                             row.Type == DiffLineType.Deleted ? DeletedPrefix : UnchangedPrefix;
            var leftText = Truncate(row.OldText, leftWidth - prefixWidth);
            segments.Add(new Segment(leftPrefix, leftStyle));
            segments.Add(new Segment(leftText.PadRight(leftWidth - prefixWidth), leftStyle));

            // Gutter
            segments.Add(new Segment(" | ", LineNumberStyle));

            // Right column (new)
            var rightStyle = GetStyleForLineType(row.Type == DiffLineType.Deleted ? DiffLineType.Unchanged : row.Type);
            var rightPrefix = row.Type == DiffLineType.Inserted ? InsertedPrefix :
                              row.Type == DiffLineType.Deleted ? UnchangedPrefix : UnchangedPrefix;
            var rightText = Truncate(row.NewText, rightWidth - prefixWidth);
            segments.Add(new Segment(rightPrefix, rightStyle));
            segments.Add(new Segment(rightText.PadRight(rightWidth - prefixWidth), rightStyle));

            segments.Add(Segment.LineBreak);
        }

        return segments;
    }

    private Style GetStyleForLineType(DiffLineType type)
    {
        return type switch
        {
            DiffLineType.Inserted => InsertedStyle,
            DiffLineType.Deleted => DeletedStyle,
            _ => UnchangedStyle,
        };
    }

    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (text.Length <= maxLength)
        {
            return text;
        }

        if (maxLength <= 3)
        {
            return text.Substring(0, maxLength);
        }

        return text.Substring(0, maxLength - 3) + "...";
    }
}
