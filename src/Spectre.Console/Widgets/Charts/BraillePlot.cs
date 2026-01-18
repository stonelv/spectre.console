using Spectre.Console.Rendering;

namespace Spectre.Console;

/// <summary>
/// A high-precision plot using Braille Unicode characters.
/// Each character is split into a 2x4 pixel matrix for sub-pixel rendering.
/// </summary>
public sealed class BraillePlot : Renderable
{
    private const int BrailleBase = 0x2800;
    private static readonly int[,] Dots = {
        { 0, 3 },
        { 1, 4 },
        { 2, 5 },
        { 6, 7 },
    };

    /// <summary>
    /// Gets the plot data.
    /// </summary>
    public List<IBraillePlotItem> Data { get; }

    /// <summary>
    /// Gets or sets the width of the plot (in characters).
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the plot (in characters).
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the plot title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the X axis.
    /// </summary>
    public bool ShowXAxis { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the Y axis.
    /// </summary>
    public bool ShowYAxis { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of X axis ticks.
    /// </summary>
    public int XAxisTicks { get; set; } = 5;

    /// <summary>
    /// Gets or sets the number of Y axis ticks.
    /// </summary>
    public int YAxisTicks { get; set; } = 5;

    /// <summary>
    /// Gets or sets the color of the plot points.
    /// </summary>
    public Color LineColor { get; set; } = Color.Cyan;

    /// <summary>
    /// Gets or sets the fixed X minimum value.
    /// </summary>
    public double? XMin { get; set; }

    /// <summary>
    /// Gets or sets the fixed X maximum value.
    /// </summary>
    public double? XMax { get; set; }

    /// <summary>
    /// Gets or sets the fixed Y minimum value.
    /// </summary>
    public double? YMin { get; set; }

    /// <summary>
    /// Gets or sets the fixed Y maximum value.
    /// </summary>
    public double? YMax { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BraillePlot"/> class.
    /// </summary>
    public BraillePlot()
    {
        Data = new List<IBraillePlotItem>();
    }

    private (double min, double max) GetXRange()
    {
        if (Data.Count == 0)
        {
            return (0, 1);
        }

        var min = XMin ?? Data.Min(p => p.X);
        var max = XMax ?? Data.Max(p => p.X);

        if (min == max)
        {
            max += 1;
        }

        return (min, max);
    }

    private (double min, double max) GetYRange()
    {
        if (Data.Count == 0)
        {
            return (0, 1);
        }

        var min = YMin ?? Data.Min(p => p.Y);
        var max = YMax ?? Data.Max(p => p.Y);

        if (min == max)
        {
            max += 1;
        }

        return (min, max);
    }

    private (int pixelX, int pixelY) ToPixel(IBraillePlotItem point, int pixelWidth, int pixelHeight, (double min, double max) xRange, (double min, double max) yRange)
    {
        var x = (point.X - xRange.min) / (xRange.max - xRange.min);
        var y = (point.Y - yRange.min) / (yRange.max - yRange.min);

        var pixelX = (int)(x * (pixelWidth - 1));
        var pixelY = (int)((1 - y) * (pixelHeight - 1));

        pixelX = Math.Clamp(pixelX, 0, pixelWidth - 1);
        pixelY = Math.Clamp(pixelY, 0, pixelHeight - 1);

        return (pixelX, pixelY);
    }

    private char GetBrailleChar(bool[,] pixels, int charX, int charY)
    {
        var result = 0;

        for (var row = 0; row < 4; row++)
        {
            for (var col = 0; col < 2; col++)
            {
                var pixelX = charX * 2 + col;
                var pixelY = charY * 4 + row;

                if (pixelX < pixels.GetLength(0) && pixelY < pixels.GetLength(1) && pixels[pixelX, pixelY])
                {
                    result |= 1 << Dots[row, col];
                }
            }
        }

        return (char)(BrailleBase + result);
    }

    private string FormatValue(double value)
    {
        return value switch
        {
            _ when Math.Abs(value) >= 1000 => $"{value:F0}",
            _ when Math.Abs(value) >= 100 => $"{value:F1}",
            _ when Math.Abs(value) >= 10 => $"{value:F2}",
            _ => $"{value:F3}",
        };
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var width = Math.Min(Width ?? maxWidth, maxWidth);
        return new Measurement(width, width);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (Data.Count == 0)
        {
            yield return Segment.Empty;
            yield break;
        }

        var width = Math.Min(Width ?? maxWidth, maxWidth);
        var height = Height ?? 10;

        var xRange = GetXRange();
        var yRange = GetYRange();

        var pixelWidth = (width - (ShowYAxis ? 8 : 0)) * 2;
        var pixelHeight = height * 4;

        if (pixelWidth <= 0 || pixelHeight <= 0)
        {
            yield return Segment.Empty;
            yield break;
        }

        var pixels = new bool[pixelWidth, pixelHeight];

        foreach (var point in Data)
        {
            var (pixelX, pixelY) = ToPixel(point, pixelWidth, pixelHeight, xRange, yRange);
            pixels[pixelX, pixelY] = true;
        }

        var charWidth = pixelWidth / 2;
        var charHeight = pixelHeight / 4;

        var yTicks = new List<double>();
        var yStep = (yRange.max - yRange.min) / (YAxisTicks - 1);
        for (var i = 0; i < YAxisTicks; i++)
        {
            yTicks.Add(yRange.max - (yStep * i));
        }

        var xTicks = new List<double>();
        var xStep = (xRange.max - xRange.min) / (XAxisTicks - 1);
        for (var i = 0; i < XAxisTicks; i++)
        {
            xTicks.Add(xRange.min + (xStep * i));
        }

        if (ShowYAxis)
        {
            for (var row = 0; row < charHeight; row++)
            {
                var tickIndex = (int)Math.Round((double)row / charHeight * (YAxisTicks - 1));
                tickIndex = Math.Clamp(tickIndex, 0, YAxisTicks - 1);
                var tickValue = yTicks[tickIndex];
                var tickLabel = FormatValue(tickValue);

                var padding = Math.Max(0, 6 - tickLabel.Length);
                yield return new Segment(new string(' ', padding));
                yield return new Segment(tickLabel);
                yield return new Segment(" ");
            }
        }

        var lines = new List<List<Segment>>();
        for (var row = 0; row < charHeight; row++)
        {
            var line = new List<Segment>();

            if (ShowYAxis)
            {
                line.Add(new Segment("│ ", new Style().Foreground(Color.Grey)));
            }

            for (var col = 0; col < charWidth; col++)
            {
                var braille = GetBrailleChar(pixels, col, row);
                if (braille != (char)BrailleBase)
                {
                    line.Add(new Segment(braille.ToString(), new Style().Foreground(LineColor)));
                }
                else
                {
                    line.Add(new Segment(" "));
                }
            }

            lines.Add(line);
        }

        foreach (var line in lines)
        {
            foreach (var segment in line)
            {
                yield return segment;
            }
            yield return Segment.LineBreak;
        }

        if (ShowXAxis)
        {
            var xAxisLine = new List<Segment>();

            if (ShowYAxis)
            {
                xAxisLine.Add(new Segment("└─", new Style().Foreground(Color.Grey)));

                for (var i = 0; i < charWidth - 1; i++)
                {
                    xAxisLine.Add(new Segment("─", new Style().Foreground(Color.Grey)));
                }
            }
            else
            {
                xAxisLine.Add(new Segment("└", new Style().Foreground(Color.Grey)));

                for (var i = 0; i < charWidth - 1; i++)
                {
                    xAxisLine.Add(new Segment("─", new Style().Foreground(Color.Grey)));
                }
            }

            foreach (var segment in xAxisLine)
            {
                yield return segment;
            }
            yield return Segment.LineBreak;

            if (ShowYAxis)
            {
                yield return new Segment(new string(' ', 8));
            }

            var tickPositions = new List<int>();
            foreach (var tick in xTicks)
            {
                var (pixelX, _) = ToPixel(new BraillePlotItem(tick, 0), pixelWidth, pixelHeight, xRange, yRange);
                var charPos = pixelX / 2;
                tickPositions.Add(charPos);
            }

            var tickLabels = xTicks.Select(t => FormatValue(t)).ToList();
            var maxLabelWidth = tickLabels.Max(l => l.Length);

            var labelPositions = new List<(int pos, string label)>();
            for (var i = 0; i < tickLabels.Count; i++)
            {
                var pos = tickPositions[i];
                var label = tickLabels[i];
                var start = Math.Max(0, pos - (label.Length / 2));

                var overlaps = labelPositions.Any(l => start < l.pos + l.label.Length && start + label.Length > l.pos);
                if (!overlaps || labelPositions.Count == 0)
                {
                    labelPositions.Add((start, label));
                }
            }

            var labelLine = new char[charWidth];
            for (var i = 0; i < labelLine.Length; i++)
            {
                labelLine[i] = ' ';
            }

            foreach (var (pos, label) in labelPositions)
            {
                for (var i = 0; i < label.Length && pos + i < labelLine.Length; i++)
                {
                    if (pos + i >= 0)
                    {
                        labelLine[pos + i] = label[i];
                    }
                }
            }

            yield return new Segment(new string(labelLine), new Style().Foreground(Color.Grey));
        }

        if (!string.IsNullOrWhiteSpace(Title))
        {
            yield return Segment.LineBreak;
            yield return new Segment(Title, new Style().Decoration(Decoration.Bold));
        }
    }
}
