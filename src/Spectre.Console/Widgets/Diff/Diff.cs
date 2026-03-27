using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Rendering;

namespace Spectre.Console;

/// <summary>
/// A renderable diff component that compares two strings and visualizes the differences.
/// </summary>
public sealed class Diff : Renderable, IAlignable
{
    private readonly List<DiffLine> _diffLines;

    /// <summary>
    /// Gets the diff lines.
    /// </summary>
    public IReadOnlyList<DiffLine> Lines => _diffLines;

    /// <summary>
    /// Gets or sets the diff mode.
    /// </summary>
    public DiffMode Mode { get; set; } = DiffMode.Inline;

    /// <summary>
    /// Gets or sets the style for added lines.
    /// </summary>
    public Style AddedStyle { get; set; } = new Style(Color.Green);

    /// <summary>
    /// Gets or sets the style for removed lines.
    /// </summary>
    public Style RemovedStyle { get; set; } = new Style(Color.Red);

    /// <summary>
    /// Gets or sets the style for unchanged lines.
    /// </summary>
    public Style UnchangedStyle { get; set; } = new Style(Color.Blue);

    /// <summary>
    /// Gets or sets the style for modified lines.
    /// </summary>
    public Style ModifiedStyle { get; set; } = new Style(Color.Yellow);

    /// <summary>
    /// Gets or sets the indicator for added lines.
    /// </summary>
    public string AddedIndicator { get; set; } = "+";

    /// <summary>
    /// Gets or sets the indicator for removed lines.
    /// </summary>
    public string RemovedIndicator { get; set; } = "-";

    /// <summary>
    /// Gets or sets the indicator for unchanged lines.
    /// </summary>
    public string UnchangedIndicator { get; set; } = " ";

    /// <summary>
    /// Gets or sets the indicator for modified lines.
    /// </summary>
    public string ModifiedIndicator { get; set; } = "~";

    /// <summary>
    /// Gets or sets the width of the line number column.
    /// </summary>
    public int LineNumberWidth { get; set; } = 4;

    /// <summary>
    /// Gets or sets a value indicating whether line numbers should be shown.
    /// </summary>
    public bool ShowLineNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets the alignment.
    /// </summary>
    public Justify? Alignment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show headers.
    /// </summary>
    public bool ShowHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets the header for the original text.
    /// </summary>
    public string OriginalHeader { get; set; } = "Original";

    /// <summary>
    /// Gets or sets the header for the modified text.
    /// </summary>
    public string ModifiedHeader { get; set; } = "Modified";

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    public Diff()
    {
        _diffLines = new List<DiffLine>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="original">The original text.</param>
    /// <param name="modified">The modified text.</param>
    public Diff(string original, string modified)
        : this()
    {
        _diffLines.AddRange(DiffAlgorithm.Compare(original, modified));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="originalLines">The original lines.</param>
    /// <param name="modifiedLines">The modified lines.</param>
    public Diff(string[] originalLines, string[] modifiedLines)
        : this()
    {
        _diffLines.AddRange(DiffAlgorithm.Compare(originalLines, modifiedLines));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="diffLines">The diff lines.</param>
    public Diff(IEnumerable<DiffLine> diffLines)
    {
        _diffLines = diffLines.ToList();
    }

    /// <summary>
    /// Adds a diff line to the diff.
    /// </summary>
    /// <param name="line">The diff line to add.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public Diff AddLine(DiffLine line)
    {
        if (line == null)
        {
            throw new ArgumentNullException(nameof(line));
        }

        _diffLines.Add(line);
        return this;
    }

    /// <summary>
    /// Adds multiple diff lines to the diff.
    /// </summary>
    /// <param name="lines">The diff lines to add.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public Diff AddLines(IEnumerable<DiffLine> lines)
    {
        if (lines == null)
        {
            throw new ArgumentNullException(nameof(lines));
        }

        _diffLines.AddRange(lines);
        return this;
    }

    /// <summary>
    /// Clears all diff lines.
    /// </summary>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public Diff Clear()
    {
        _diffLines.Clear();
        return this;
    }

    /// <summary>
    /// Compares two strings and adds the differences to the diff.
    /// </summary>
    /// <param name="original">The original text.</param>
    /// <param name="modified">The modified text.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public Diff Compare(string original, string modified)
    {
        _diffLines.AddRange(DiffAlgorithm.Compare(original, modified));
        return this;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (Mode == DiffMode.Inline)
        {
            return RenderInline(options, maxWidth);
        }

        return RenderSideBySide(options, maxWidth);
    }

    private IEnumerable<Segment> RenderInline(RenderOptions options, int maxWidth)
    {
        var lineNumberStyle = new Style(Color.Grey);

        foreach (var line in _diffLines)
        {
            var style = GetLineStyle(line.Type);
            var indicator = GetIndicator(line.Type);

            var segments = new List<Segment>();

            // Add indicator
            segments.Add(new Segment($"{indicator}", style));

            // Add line numbers
            if (ShowLineNumbers)
            {
                var originalLineNumber = line.OriginalLineNumber > 0 ? line.OriginalLineNumber.ToString().PadLeft(LineNumberWidth) : new string(' ', LineNumberWidth);
                segments.Add(new Segment($" {originalLineNumber} ", lineNumberStyle));
            }

            // Add the line text
            var text = line.Type switch
            {
                DiffChangeType.Removed => line.Original,
                DiffChangeType.Modified => line.Modified,
                DiffChangeType.Added => line.Modified,
                _ => line.Original,
            };

            if (text != null)
            {
                segments.Add(new Segment(text, style));
            }

            yield return Segment.LineBreak;
            foreach (var segment in segments)
            {
                yield return segment;
            }
        }

        yield return Segment.LineBreak;
    }

    private IEnumerable<Segment> RenderSideBySide(RenderOptions options, int maxWidth)
    {
        var lineNumberStyle = new Style(Color.Grey);

        // Calculate column widths
        var headerStyle = new Style(decoration: Decoration.Bold);
        var separator = new Segment(" | ", new Style(Color.Grey));

        var availableWidth = maxWidth - separator.CellCount();
        if (ShowLineNumbers)
        {
            availableWidth -= (LineNumberWidth + 1) * 2;
        }

        var columnWidth = availableWidth / 2;

        // Render header
        if (ShowHeaders)
        {
            if (ShowLineNumbers)
            {
                yield return new Segment(new string(' ', LineNumberWidth + 1), lineNumberStyle);
            }

            yield return new Segment(OriginalHeader.PadRight(columnWidth), headerStyle);
            yield return separator;

            if (ShowLineNumbers)
            {
                yield return new Segment(new string(' ', LineNumberWidth + 1), lineNumberStyle);
            }

            yield return new Segment(ModifiedHeader.PadRight(columnWidth), headerStyle);
            yield return Segment.LineBreak;
        }

        // Render lines
        foreach (var line in _diffLines)
        {
            var segments = new List<Segment>();

            // Original column
            var originalStyle = line.Type == DiffChangeType.Removed || line.Type == DiffChangeType.Modified ? RemovedStyle : UnchangedStyle;

            if (ShowLineNumbers)
            {
                var originalLineNumber = line.OriginalLineNumber > 0 ? line.OriginalLineNumber.ToString().PadLeft(LineNumberWidth) : new string(' ', LineNumberWidth);
                segments.Add(new Segment($"{originalLineNumber} ", lineNumberStyle));
            }

            var originalText = line.Original ?? string.Empty;
            if (originalText.Length > columnWidth)
            {
                originalText = originalText.Substring(0, columnWidth - 3) + "...";
            }

            segments.Add(new Segment(originalText.PadRight(columnWidth), originalStyle));

            // Separator
            segments.Add(separator);

            // Modified column
            var modifiedStyle = line.Type == DiffChangeType.Added || line.Type == DiffChangeType.Modified ? AddedStyle : UnchangedStyle;

            if (ShowLineNumbers)
            {
                var modifiedLineNumber = line.ModifiedLineNumber > 0 ? line.ModifiedLineNumber.ToString().PadLeft(LineNumberWidth) : new string(' ', LineNumberWidth);
                segments.Add(new Segment($"{modifiedLineNumber} ", lineNumberStyle));
            }

            var modifiedText = line.Modified ?? string.Empty;
            if (modifiedText.Length > columnWidth)
            {
                modifiedText = modifiedText.Substring(0, columnWidth - 3) + "...";
            }

            segments.Add(new Segment(modifiedText.PadRight(columnWidth), modifiedStyle));

            yield return Segment.LineBreak;
            foreach (var segment in segments)
            {
                yield return segment;
            }
        }

        yield return Segment.LineBreak;
    }

    private Style GetLineStyle(DiffChangeType type)
    {
        return type switch
        {
            DiffChangeType.Added => AddedStyle,
            DiffChangeType.Removed => RemovedStyle,
            DiffChangeType.Modified => ModifiedStyle,
            _ => UnchangedStyle,
        };
    }

    private string GetIndicator(DiffChangeType type)
    {
        return type switch
        {
            DiffChangeType.Added => AddedIndicator,
            DiffChangeType.Removed => RemovedIndicator,
            DiffChangeType.Modified => ModifiedIndicator,
            _ => UnchangedIndicator,
        };
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        return new Measurement(10, maxWidth);
    }
}
