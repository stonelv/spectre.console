using System.Globalization;
using System.Text;

namespace Spectre.Console;

public sealed class BrailleChart : IRenderable
    {
    private readonly List<BrailleChartSeries> _series;
    private readonly BrailleCanvas _canvas;
    private bool _autoScale = true;
    private double _minX = 0;
    private double _maxX = 100;
    private double _minY = 0;
    private double _maxY = 100;
    private bool _showXAxis = true;
    private bool _showYAxis = true;
    private int _xAxisTicks = 5;
    private int _yAxisTicks = 5;
    private bool _showGrid = true;

    public string? Title { get; set; }
    public Justify TitleAlignment { get; set; } = Justify.Center;
    public Color TitleColor { get; set; } = Color.Blue;
    public CultureInfo? Culture { get; set; }
    public int Width { get; set; } = 80;
    public int Height { get; set; } = 40;
    public Color GridColor { get; set; } = Color.Grey;
    public Color AxisColor { get; set; } = Color.White;
    public Color LabelColor { get; set; } = Color.Silver;

    public bool AutoScale
    {
        get => _autoScale;
        set => _autoScale = value;
    }

    public double MinX
    {
        get => _minX;
        set
        {
            _minX = value;
            _autoScale = false;
        }
    }

    public double MaxX
    {
        get => _maxX;
        set
        {
            _maxX = value;
            _autoScale = false;
        }
    }

    public double MinY
    {
        get => _minY;
        set
        {
            _minY = value;
            _autoScale = false;
        }
    }

    public double MaxY
    {
        get => _maxY;
        set
        {
            _maxY = value;
            _autoScale = false;
        }
    }

    public bool ShowXAxis
    {
        get => _showXAxis;
        set => _showXAxis = value;
    }

    public bool ShowYAxis
    {
        get => _showYAxis;
        set => _showYAxis = value;
    }

    public int XAxisTicks
    {
        get => _xAxisTicks;
        set => _xAxisTicks = Math.Max(2, value);
    }

    public int YAxisTicks
    {
        get => _yAxisTicks;
        set => _yAxisTicks = Math.Max(2, value);
    }

    public bool ShowGrid
    {
        get => _showGrid;
        set => _showGrid = value;
    }

    public BrailleChart()
    {
        _series = new List<BrailleChartSeries>();
        _canvas = new BrailleCanvas(Width, Height);
    }

    public BrailleChart AddSeries(string name, Color color, IEnumerable<(double X, double Y)> data)
    {
        _series.Add(new BrailleChartSeries(name, color, data.ToList()));
        return this;
    }

    public BrailleChart AddPoint(double x, double y, Color color)
    {
        if (_series.Count == 0)
        {
            _series.Add(new BrailleChartSeries("Points", color, new List<(double, double)>()));
        }
        _series[0].Data.Add((x, y));
        return this;
    }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return new Measurement(Width, Width);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (_series.Count == 0)
        {
            yield break;
        }

        _canvas.Clear();

        if (_autoScale)
        {
            CalculateAutoScale();
        }

        if (_showGrid)
        {
            DrawGrid();
        }

        foreach (var series in _series)
        {
            DrawSeries(series);
        }

        var lines = _canvas.Render().ToList();
        var labelWidth = _showYAxis ? 12 : 0;
        var titleOffset = labelWidth + 1;

        if (!string.IsNullOrWhiteSpace(Title))
        {
            var title = Title.PadLeft((lines[0].Length - Title.Length) / 2 + titleOffset);
            yield return new Segment(title, new Style().Foreground(TitleColor));
            yield return Segment.LineBreak;
        }

        var xAxisOffset = _showYAxis ? 14 : 2;
        var chartLines = new List<string>();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var prefix = string.Empty;

            if (_showYAxis)
            {
                var yValue = _maxY - (i / (double)lines.Count) * (_maxY - _minY);
                prefix = FormatValue(yValue).PadLeft(10) + " ";
            }

            chartLines.Add(prefix + line);
        }

        foreach (var line in chartLines)
        {
            yield return new Segment(line);
            yield return Segment.LineBreak;
        }

        if (_showXAxis)
        {
            var xLabels = new List<string>();
            for (int i = 0; i <= _xAxisTicks; i++)
            {
                var xValue = _minX + (i / (double)_xAxisTicks) * (_maxX - _minX);
                var label = FormatValue(xValue);
                var position = (int)(i * (lines[0].Length - label.Length) / _xAxisTicks);
                xLabels.Add($"{new string(' ', position)}{label}");
            }

            var mergedLabel = new string(' ', lines[0].Length);
            foreach (var label in xLabels)
            {
                for (int i = 0; i < label.Length && i < mergedLabel.Length; i++)
                {
                    if (label[i] != ' ')
                    {
                        mergedLabel = mergedLabel.Remove(i, 1).Insert(i, label[i].ToString());
                    }
                }
            }

            yield return new Segment(new string(' ', labelWidth) + mergedLabel, new Style().Foreground(LabelColor));
        }

        if (_series.Count > 1)
        {
            yield return Segment.LineBreak;
            var legend = new List<string>();
            foreach (var series in _series)
            {
                legend.Add($"{series.Color.ToMarkup()}▀[/] {series.Name}");
            }
            yield return new Segment(string.Join("  ", legend));
        }
    }

    private void CalculateAutoScale()
    {
        var allPoints = _series.SelectMany(s => s.Data);
        if (!allPoints.Any())
        {
            return;
        }

        _minX = allPoints.Min(p => p.X);
        _maxX = allPoints.Max(p => p.X);
        _minY = allPoints.Min(p => p.Y);
        _maxY = allPoints.Max(p => p.Y);

        var xRange = _maxX - _minX;
        var yRange = _maxY - _minY;

        if (xRange < 0.0001)
        {
            _minX -= 1;
            _maxX += 1;
        }
        else
        {
            var padding = xRange * 0.05;
            _minX -= padding;
            _maxX += padding;
        }

        if (yRange < 0.0001)
        {
            _minY -= 1;
            _maxY += 1;
        }
        else
        {
            var padding = yRange * 0.05;
            _minY -= padding;
            _maxY += padding;
        }
    }

    private void DrawGrid()
    {
        var xStep = (_maxX - _minX) / _xAxisTicks;
        var yStep = (_maxY - _minY) / _yAxisTicks;

        for (int i = 0; i <= _xAxisTicks; i++)
        {
            var x = _minX + i * xStep;
            DrawVerticalLine(x, GridColor);
        }

        for (int i = 0; i <= _yAxisTicks; i++)
        {
            var y = _minY + i * yStep;
            DrawHorizontalLine(y, GridColor);
        }
    }

    private void DrawVerticalLine(double x, Color color)
    {
        var canvasX = MapXToCanvas(x);
        for (int y = 0; y < _canvas.Height; y++)
        {
            _canvas.SetPixel(canvasX, y, color);
        }
    }

    private void DrawHorizontalLine(double y, Color color)
    {
        var canvasY = MapYToCanvas(y);
        for (int x = 0; x < _canvas.Width; x++)
        {
            _canvas.SetPixel(x, canvasY, color);
        }
    }

    private void DrawSeries(BrailleChartSeries series)
    {
        var points = series.Data.OrderBy(p => p.X).ToList();

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];
            DrawLine(p1.X, p1.Y, p2.X, p2.Y, series.Color);
        }

        foreach (var point in points)
        {
            var canvasX = MapXToCanvas(point.X);
            var canvasY = MapYToCanvas(point.Y);
            _canvas.SetPixel(canvasX, canvasY, series.Color);
            if (canvasX > 0)
                _canvas.SetPixel(canvasX - 1, canvasY, series.Color);
            if (canvasX < _canvas.Width - 1)
                _canvas.SetPixel(canvasX + 1, canvasY, series.Color);
            if (canvasY > 0)
                _canvas.SetPixel(canvasX, canvasY - 1, series.Color);
            if (canvasY < _canvas.Height - 1)
                _canvas.SetPixel(canvasX, canvasY + 1, series.Color);
        }
    }

    private void DrawLine(double x1, double y1, double x2, double y2, Color color)
    {
        var canvasX1 = MapXToCanvas(x1);
        var canvasY1 = MapYToCanvas(y1);
        var canvasX2 = MapXToCanvas(x2);
        var canvasY2 = MapYToCanvas(y2);

        var dx = Math.Abs(canvasX2 - canvasX1);
        var dy = Math.Abs(canvasY2 - canvasY1);
        var sx = canvasX1 < canvasX2 ? 1 : -1;
        var sy = canvasY1 < canvasY2 ? 1 : -1;
        var err = dx - dy;

        var x = canvasX1;
        var y = canvasY1;

        while (true)
        {
            _canvas.SetPixel(x, y, color);

            if (x == canvasX2 && y == canvasY2)
                break;

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

    private int MapXToCanvas(double x)
    {
        var ratio = (x - _minX) / (_maxX - _minX);
        var canvasX = (int)(ratio * _canvas.Width);
        return Math.Clamp(canvasX, 0, _canvas.Width - 1);
    }

    private int MapYToCanvas(double y)
    {
        var ratio = 1 - (y - _minY) / (_maxY - _minY);
        var canvasY = (int)(ratio * _canvas.Height);
        return Math.Clamp(canvasY, 0, _canvas.Height - 1);
    }

    private string FormatValue(double value)
    {
        var culture = Culture ?? CultureInfo.InvariantCulture;
        if (Math.Abs(value) >= 1000000)
        {
            return value.ToString("0.00e0", culture);
        }
        else if (Math.Abs(value) < 0.001)
        {
            return value.ToString("0.000e0", culture);
        }
        else if (Math.Abs(value) < 1)
        {
            return value.ToString("0.000", culture);
        }
        else
        {
            return value.ToString("0.00", culture);
        }
    }

    private sealed class BrailleChartSeries
    {
        public string Name { get; }
        public Color Color { get; }
        public List<(double X, double Y)> Data { get; }

        public BrailleChartSeries(string name, Color color, List<(double X, double Y)> data)
        {
            Name = name;
            Color = color;
            Data = data;
        }
    }
}
