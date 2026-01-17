using Spectre.Console.Rendering;

namespace Spectre.Console;

public enum DiffRenderMode
{
    Inline,
    SideBySide
}

public sealed class Diff : Renderable
{
    private readonly List<DiffLine> _diffLines;

    public DiffRenderMode RenderMode { get; set; } = DiffRenderMode.Inline;

    public Style AddedStyle { get; set; } = new Style(foreground: Color.Green);
    public Style DeletedStyle { get; set; } = new Style(foreground: Color.Red);
    public Style UnchangedStyle { get; set; } = new Style(foreground: Color.Grey);

    public Diff(string oldText, string newText)
    {
        _diffLines = DiffAlgorithm.ComputeDiff(oldText, newText);
    }

    public Diff(IEnumerable<DiffLine> diffLines)
    {
        _diffLines = diffLines?.ToList() ?? throw new ArgumentNullException(nameof(diffLines));
    }

    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        if (RenderMode == DiffRenderMode.SideBySide)
        {
            return new Measurement(0, maxWidth);
        }

        int minWidth = 0;
        int maxWidthCalculated = 0;

        foreach (var line in _diffLines)
        {
            int width = line.Content.Length + 1;
            minWidth = Math.Max(minWidth, width);
            maxWidthCalculated = Math.Max(maxWidthCalculated, width);
        }

        return new Measurement(minWidth, Math.Min(maxWidthCalculated, maxWidth));
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (RenderMode == DiffRenderMode.SideBySide)
        {
            return RenderSideBySide(options, maxWidth);
        }

        return RenderInline(options, maxWidth);
    }

    private IEnumerable<Segment> RenderInline(RenderOptions options, int maxWidth)
    {
        foreach (var line in _diffLines)
        {
            char symbol = line.Type switch
            {
                DiffLineType.Added => '+',
                DiffLineType.Deleted => '-',
                _ => ' ',
            };

            Style style = line.Type switch
            {
                DiffLineType.Added => AddedStyle,
                DiffLineType.Deleted => DeletedStyle,
                _ => UnchangedStyle,
            };

            yield return new Segment(symbol.ToString(), style);
            yield return new Segment(line.Content, style);
            yield return Segment.LineBreak;
        }
    }

    private IEnumerable<Segment> RenderSideBySide(RenderOptions options, int maxWidth)
    {
        var (leftLines, rightLines) = SplitSideBySide();
        int columnWidth = maxWidth / 2;

        for (int i = 0; i < Math.Max(leftLines.Count, rightLines.Count); i++)
        {
            var leftLine = i < leftLines.Count ? leftLines[i] : null;
            var rightLine = i < rightLines.Count ? rightLines[i] : null;

            yield return RenderSideBySideLine(leftLine, columnWidth, true);
            yield return new Segment(" ");
            yield return RenderSideBySideLine(rightLine, columnWidth, false);
            yield return Segment.LineBreak;
        }
    }

    private (List<DiffLine> Left, List<DiffLine> Right) SplitSideBySide()
    {
        var left = new List<DiffLine>();
        var right = new List<DiffLine>();

        foreach (var line in _diffLines)
        {
            switch (line.Type)
            {
                case DiffLineType.Unchanged:
                    left.Add(line);
                    right.Add(line);
                    break;
                case DiffLineType.Deleted:
                    left.Add(line);
                    right.Add(new DiffLine(DiffLineType.Unchanged, string.Empty));
                    break;
                case DiffLineType.Added:
                    left.Add(new DiffLine(DiffLineType.Unchanged, string.Empty));
                    right.Add(line);
                    break;
            }
        }

        return (left, right);
    }

    private Segment RenderSideBySideLine(DiffLine? line, int width, bool isLeft)
    {
        if (line == null)
        {
            return Segment.Padding(width);
        }

        char symbol = line.Type switch
        {
            DiffLineType.Added => '+',
            DiffLineType.Deleted => '-',
            _ => ' ',
        };

        Style style = line.Type switch
        {
            DiffLineType.Added => AddedStyle,
            DiffLineType.Deleted => DeletedStyle,
            _ => UnchangedStyle,
        };

        string content = symbol + line.Content;
        if (content.Length < width)
        {
            content = content.PadRight(width);
        }
        else if (content.Length > width)
        {
            content = content.Substring(0, width - 1) + ".";
        }

        return new Segment(content, style);
    }
}
