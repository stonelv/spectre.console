using Spectre.Console.Rendering;

namespace Spectre.Console;

/// <summary>
/// Represents a renderable diff that compares two texts.
/// </summary>
public sealed class Diff : Renderable
{
    private readonly List<DiffLine> _lines;
    private DiffFormat _format = DiffFormat.Inline;
    private bool _showLineNumbers = true;

    /// <summary>
    /// Gets or sets the style for unchanged lines.
    /// </summary>
    public Style UnchangedStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the style for inserted lines.
    /// </summary>
    public Style InsertedStyle { get; set; } = new Style(Color.Green);

    /// <summary>
    /// Gets or sets the style for deleted lines.
    /// </summary>
    public Style DeletedStyle { get; set; } = new Style(Color.Red);

    /// <summary>
    /// Gets or sets the format of the diff output.
    /// </summary>
    public DiffFormat Format
    {
        get => _format;
        set => _format = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => _showLineNumbers;
        set => _showLineNumbers = value;
    }

    /// <summary>
    /// Gets the diff lines.
    /// </summary>
    public IReadOnlyList<DiffLine> Lines => _lines;

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="oldText">The old text to compare.</param>
    /// <param name="newText">The new text to compare.</param>
    public Diff(string oldText, string newText)
    {
        if (oldText == null && newText == null)
        {
            _lines = new List<DiffLine>();
            return;
        }

        var oldLines = oldText?.Split(new[] { '\r', '\n' }, StringSplitOptions.None) ?? Array.Empty<string>();
        var newLines = newText?.Split(new[] { '\r', '\n' }, StringSplitOptions.None) ?? Array.Empty<string>();

        _lines = ComputeDiff(oldLines, newLines);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="oldLines">The old lines to compare.</param>
    /// <param name="newLines">The new lines to compare.</param>
    public Diff(IEnumerable<string> oldLines, IEnumerable<string> newLines)
    {
        var oldList = oldLines?.ToList() ?? new List<string>();
        var newList = newLines?.ToList() ?? new List<string>();

        _lines = ComputeDiff(oldList, newList);
    }

    private static List<DiffLine> ComputeDiff(IReadOnlyList<string> oldLines, IReadOnlyList<string> newLines)
    {
        var lcsLength = ComputeLcsLength(oldLines, newLines);
        return Backtrack(lcsLength, oldLines, newLines, oldLines.Count, newLines.Count);
    }

    private static int[,] ComputeLcsLength(IReadOnlyList<string> oldLines, IReadOnlyList<string> newLines)
    {
        var m = oldLines.Count;
        var n = newLines.Count;

        var dp = new int[m + 1, n + 1];

        for (var i = 1; i <= m; i++)
        {
            for (var j = 1; j <= n; j++)
            {
                if (oldLines[i - 1] == newLines[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        return dp;
    }

    private static List<DiffLine> Backtrack(int[,] lcsLength, IReadOnlyList<string> oldLines, IReadOnlyList<string> newLines, int i, int j)
    {
        var result = new List<DiffLine>();
        var oldLineNum = 1;
        var newLineNum = 1;

        var stack = new Stack<(int i, int j)>();
        stack.Push((i, j));

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            i = current.i;
            j = current.j;

            if (i > 0 && j > 0 && oldLines[i - 1] == newLines[j - 1])
            {
                stack.Push((i - 1, j - 1));
            }
            else if (j > 0 && (i == 0 || lcsLength[i, j - 1] >= lcsLength[i - 1, j]))
            {
                stack.Push((i, j - 1));
            }
            else if (i > 0)
            {
                stack.Push((i - 1, j));
            }
        }

        var operations = new List<(int i, int j, string op)>();
        i = oldLines.Count;
        j = newLines.Count;

        while (i > 0 || j > 0)
        {
            if (i > 0 && j > 0 && oldLines[i - 1] == newLines[j - 1])
            {
                operations.Add((i - 1, j - 1, "equal"));
                i--;
                j--;
            }
            else if (j > 0 && (i == 0 || lcsLength[i, j - 1] >= lcsLength[i - 1, j]))
            {
                operations.Add((i, j - 1, "insert"));
                j--;
            }
            else if (i > 0)
            {
                operations.Add((i - 1, j, "delete"));
                i--;
            }
        }

        operations.Reverse();

        oldLineNum = 1;
        newLineNum = 1;

        foreach (var op in operations)
        {
            switch (op.op)
            {
                case "equal":
                    result.Add(new DiffLine(oldLines[op.i], DiffLineState.Unchanged, oldLineNum, newLineNum));
                    oldLineNum++;
                    newLineNum++;
                    break;
                case "insert":
                    result.Add(new DiffLine(newLines[op.j], DiffLineState.Inserted, null, newLineNum));
                    newLineNum++;
                    break;
                case "delete":
                    result.Add(new DiffLine(oldLines[op.i], DiffLineState.Deleted, oldLineNum, null));
                    oldLineNum++;
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Renders the diff.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="maxWidth">The maximum width.</param>
    /// <returns>The rendered segments.</returns>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return _format switch
        {
            DiffFormat.SideBySide => RenderSideBySide(options, maxWidth),
            _ => RenderInline(options, maxWidth),
        };
    }

    private IEnumerable<Segment> RenderInline(RenderOptions options, int maxWidth)
    {
        var segments = new List<Segment>();
        var lineNumberWidth = _lines.Count > 0 ? _lines.Max(l => Math.Max(l.OldLineNumber ?? 0, l.NewLineNumber ?? 0)).ToString().Length : 1;

        foreach (var line in _lines)
        {
            if (_showLineNumbers)
            {
                var oldNum = line.OldLineNumber?.ToString().PadLeft(lineNumberWidth) ?? new string(' ', lineNumberWidth);
                var newNum = line.NewLineNumber?.ToString().PadLeft(lineNumberWidth) ?? new string(' ', lineNumberWidth);

                segments.Add(new Segment(oldNum, GetLineNumberStyle(line.State)));
                segments.Add(new Segment(" ", Style.Plain));
                segments.Add(new Segment(newNum, GetLineNumberStyle(line.State)));
                segments.Add(new Segment(" | ", Style.Plain));
            }

            var prefix = GetPrefix(line.State);
            var style = GetStyle(line.State);

            segments.Add(new Segment(prefix, style));
            segments.Add(new Segment(line.Text, style));
            segments.Add(Segment.LineBreak);
        }

        return segments;
    }

    private IEnumerable<Segment> RenderSideBySide(RenderOptions options, int maxWidth)
    {
        var segments = new List<Segment>();
        var lineNumberWidth = _lines.Count > 0 ? _lines.Max(l => Math.Max(l.OldLineNumber ?? 0, l.NewLineNumber ?? 0)).ToString().Length : 1;
        var columnWidth = (maxWidth - (lineNumberWidth * 2 + 6)) / 2;

        var oldLines = new List<DiffLine>();
        var newLines = new List<DiffLine>();

        foreach (var line in _lines)
        {
            if (line.State == DiffLineState.Unchanged)
            {
                oldLines.Add(line);
                newLines.Add(line);
            }
            else if (line.State == DiffLineState.Deleted)
            {
                oldLines.Add(line);
                newLines.Add(new DiffLine(string.Empty, DiffLineState.Unchanged, null, null));
            }
            else
            {
                oldLines.Add(new DiffLine(string.Empty, DiffLineState.Unchanged, null, null));
                newLines.Add(line);
            }
        }

        for (var i = 0; i < oldLines.Count; i++)
        {
            var oldLine = oldLines[i];
            var newLine = newLines[i];

            if (_showLineNumbers)
            {
                var oldNum = oldLine.OldLineNumber?.ToString().PadLeft(lineNumberWidth) ?? new string(' ', lineNumberWidth);
                segments.Add(new Segment(oldNum, GetLineNumberStyle(oldLine.State)));
                segments.Add(new Segment(" | ", Style.Plain));
            }

            var oldPrefix = GetPrefix(oldLine.State);
            var oldStyle = GetStyle(oldLine.State);
            var oldText = TruncateText(oldLine.Text, columnWidth - 1);

            segments.Add(new Segment(oldPrefix, oldStyle));
            segments.Add(new Segment(oldText.PadRight(columnWidth - 1), oldStyle));
            segments.Add(new Segment(" │ ", Style.Plain));

            if (_showLineNumbers)
            {
                var newNum = newLine.NewLineNumber?.ToString().PadLeft(lineNumberWidth) ?? new string(' ', lineNumberWidth);
                segments.Add(new Segment(newNum, GetLineNumberStyle(newLine.State)));
                segments.Add(new Segment(" | ", Style.Plain));
            }

            var newPrefix = GetPrefix(newLine.State);
            var newStyle = GetStyle(newLine.State);
            var newText = TruncateText(newLine.Text, columnWidth - 1);

            segments.Add(new Segment(newPrefix, newStyle));
            segments.Add(new Segment(newText.PadRight(columnWidth - 1), newStyle));
            segments.Add(Segment.LineBreak);
        }

        return segments;
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return text;
        }

        return text.Substring(0, maxLength - 1) + "…";
    }

    private static string GetPrefix(DiffLineState state)
    {
        return state switch
        {
            DiffLineState.Inserted => "+",
            DiffLineState.Deleted => "-",
            _ => " ",
        };
    }

    private Style GetStyle(DiffLineState state)
    {
        return state switch
        {
            DiffLineState.Inserted => InsertedStyle,
            DiffLineState.Deleted => DeletedStyle,
            _ => UnchangedStyle,
        };
    }

    private static Style GetLineNumberStyle(DiffLineState state)
    {
        return state switch
        {
            DiffLineState.Inserted => new Style(Color.Green),
            DiffLineState.Deleted => new Style(Color.Red),
            _ => new Style(Color.Grey),
        };
    }
}
