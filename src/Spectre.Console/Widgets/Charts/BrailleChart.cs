namespace Spectre.Console;

public sealed class BrailleChart : Renderable, IHasCulture
{
    public List<IBrailleChartDataPoint> Data { get; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int DefaultWidth { get; set; } = 60;

    public int DefaultHeight { get; set; } = 20;

    public Color Color { get; set; } = Color.Cyan1;

    public bool ShowAxes { get; set; } = true;

    public bool ShowLabels { get; set; } = true;

    public bool ShowGrid { get; set; } = true;

    public int XAxisTickCount { get; set; } = 5;

    public int YAxisTickCount { get; set; } = 5;

    public CultureInfo? Culture { get; set; }

    public Func<double, CultureInfo, string>? XLabelFormatter { get; set; }

    public Func<double, CultureInfo, string>? YLabelFormatter { get; set; }

    public BrailleChart()
    {
        Data = new List<IBrailleChartDataPoint>();
    }

    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var width = Math.Min(Width ?? DefaultWidth, maxWidth);
        var height = Height ?? DefaultHeight;
        return new Measurement(width, height);
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var width = Width ?? DefaultWidth;
        var height = Height ?? DefaultHeight;

        if (Data.Count == 0)
        {
            yield return new Segment("No data to display");
            yield return Segment.LineBreak;
            yield break;
        }

        var (minX, maxX, minY, maxY) = CalculateBounds();

        var chartWidth = width;
        var chartHeight = height;

        var canvas = new BrailleCanvas(chartWidth, chartHeight);

        var scaleX = (chartWidth * 2.0 - 1) / (maxX - minX);
        var scaleY = (chartHeight * 4.0 - 1) / (maxY - minY);

        var sortedData = Data.OrderBy(d => d.X).ToList();

        for (var i = 0; i < sortedData.Count - 1; i++)
        {
            var x0 = (int)((sortedData[i].X - minX) * scaleX);
            var y0 = (int)((sortedData[i].Y - minY) * scaleY);
            var x1 = (int)((sortedData[i + 1].X - minX) * scaleX);
            var y1 = (int)((sortedData[i + 1].Y - minY) * scaleY);

            canvas.DrawLineHighRes(x0, y0, x1, y1);
        }

        var renderedChart = canvas.Render();

        var lines = renderedChart.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        var yLabels = GenerateYLabels(minY, maxY, chartHeight);

        var maxLabelWidth = yLabels.Max(l => l.Length);

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];

            if (ShowLabels && lineIndex < yLabels.Count)
            {
                var label = yLabels[lineIndex];
                var paddedLabel = label.PadLeft(maxLabelWidth);
                yield return new Segment(paddedLabel + " ", new Style(foreground: Color.Grey));
            }

            yield return new Segment(line, new Style(foreground: Color));

            if (ShowLabels && lineIndex == 0)
            {
                var maxYLabel = FormatLabel(maxY, YLabelFormatter, true);
                yield return new Segment($" {maxYLabel}", new Style(foreground: Color.Grey));
            }
            else if (ShowLabels && lineIndex == lines.Length - 1)
            {
                var minYLabel = FormatLabel(minY, YLabelFormatter, true);
                yield return new Segment($" {minYLabel}", new Style(foreground: Color.Grey));
            }

            yield return Segment.LineBreak;
        }

        if (ShowLabels)
        {
            var xLabels = GenerateXLabels(minX, maxX, chartWidth);
            var labelLine = new string(' ', maxLabelWidth + 1);

            for (var i = 0; i < xLabels.Count; i++)
            {
                var position = (int)((i / (double)(xLabels.Count - 1)) * (chartWidth - 1));
                var label = xLabels[i];
                var labelStart = position - label.Length / 2;

                if (labelStart + label.Length > labelLine.Length)
                {
                    labelStart = labelLine.Length - label.Length;
                }

                if (labelStart >= 0)
                {
                    var labelChars = label.ToCharArray();
                    for (var j = 0; j < labelChars.Length && labelStart + j < labelLine.Length; j++)
                    {
                        labelLine = labelLine.Remove(labelStart + j, 1).Insert(labelStart + j, labelChars[j].ToString());
                    }
                }
            }

            yield return new Segment(labelLine, new Style(foreground: Color.Grey));
            yield return Segment.LineBreak;
        }
    }

    private (double minX, double maxX, double minY, double maxY) CalculateBounds()
    {
        if (Data.Count == 0)
        {
            return (0, 1, 0, 1);
        }

        var minX = Data.Min(d => d.X);
        var maxX = Data.Max(d => d.X);
        var minY = Data.Min(d => d.Y);
        var maxY = Data.Max(d => d.Y);

        if (minX == maxX)
        {
            minX -= 1;
            maxX += 1;
        }

        if (minY == maxY)
        {
            minY -= 1;
            maxY += 1;
        }

        var xPadding = (maxX - minX) * 0.05;
        var yPadding = (maxY - minY) * 0.05;

        return (minX - xPadding, maxX + xPadding, minY - yPadding, maxY + yPadding);
    }

    private List<string> GenerateYLabels(double minY, double maxY, int height)
    {
        var labels = new List<string>();
        var step = (maxY - minY) / (YAxisTickCount - 1);

        for (var i = 0; i < YAxisTickCount; i++)
        {
            var value = minY + step * i;
            labels.Add(FormatLabel(value, YLabelFormatter, true));
        }

        return labels;
    }

    private List<string> GenerateXLabels(double minX, double maxX, int width)
    {
        var labels = new List<string>();
        var step = (maxX - minX) / (XAxisTickCount - 1);

        for (var i = 0; i < XAxisTickCount; i++)
        {
            var value = minX + step * i;
            labels.Add(FormatLabel(value, XLabelFormatter, false));
        }

        return labels;
    }

    private string FormatLabel(double value, Func<double, CultureInfo, string>? formatter, bool isYAxis)
    {
        var culture = Culture ?? CultureInfo.InvariantCulture;

        if (formatter != null)
        {
            return formatter(value, culture);
        }

        if (isYAxis)
        {
            return value.ToString("F2", culture);
        }

        return value.ToString("F1", culture);
    }
}
