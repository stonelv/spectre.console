namespace Spectre.Console;

public static class BrailleChartExtensions
{
    public static BrailleChart AddPoint(this BrailleChart chart, double x, double y)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.Data.Add(new BrailleChartDataPoint(x, y));
        return chart;
    }

    public static BrailleChart AddPoint<T>(this BrailleChart chart, T point)
        where T : IBrailleChartDataPoint
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        if (point is BrailleChartDataPoint dataPoint)
        {
            chart.Data.Add(dataPoint);
        }
        else
        {
            chart.Data.Add(new BrailleChartDataPoint(point.X, point.Y));
        }

        return chart;
    }

    public static BrailleChart AddPoints<T>(this BrailleChart chart, IEnumerable<T> points)
        where T : IBrailleChartDataPoint
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        foreach (var point in points)
        {
            AddPoint(chart, point);
        }

        return chart;
    }

    public static BrailleChart AddPoints(this BrailleChart chart, IEnumerable<(double X, double Y)> points)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        foreach (var (x, y) in points)
        {
            chart.Data.Add(new BrailleChartDataPoint(x, y));
        }

        return chart;
    }

    public static BrailleChart SetColor(this BrailleChart chart, Color color)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.Color = color;
        return chart;
    }

    public static BrailleChart SetSize(this BrailleChart chart, int width, int height)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.Width = width;
        chart.Height = height;
        return chart;
    }

    public static BrailleChart SetWidth(this BrailleChart chart, int width)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.Width = width;
        return chart;
    }

    public static BrailleChart SetHeight(this BrailleChart chart, int height)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.Height = height;
        return chart;
    }

    public static BrailleChart ShowAxes(this BrailleChart chart, bool show = true)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.ShowAxes = show;
        return chart;
    }

    public static BrailleChart ShowLabels(this BrailleChart chart, bool show = true)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.ShowLabels = show;
        return chart;
    }

    public static BrailleChart ShowGrid(this BrailleChart chart, bool show = true)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.ShowGrid = show;
        return chart;
    }

    public static BrailleChart SetXAxisTickCount(this BrailleChart chart, int count)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.XAxisTickCount = count;
        return chart;
    }

    public static BrailleChart SetYAxisTickCount(this BrailleChart chart, int count)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.YAxisTickCount = count;
        return chart;
    }

    public static BrailleChart UseXLabelFormatter(this BrailleChart chart, Func<double, CultureInfo, string>? formatter)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.XLabelFormatter = formatter;
        return chart;
    }

    public static BrailleChart UseYLabelFormatter(this BrailleChart chart, Func<double, CultureInfo, string>? formatter)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.YLabelFormatter = formatter;
        return chart;
    }

    public static BrailleChart UseCulture(this BrailleChart chart, CultureInfo culture)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        chart.Culture = culture;
        return chart;
    }
}
