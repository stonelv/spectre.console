namespace Spectre.Console;

/// <summary>
/// A builder for creating <see cref="BraillePlot"/> instances.
/// </summary>
public sealed class BraillePlotBuilder
{
    private readonly BraillePlot _plot;

    /// <summary>
    /// Initializes a new instance of the <see cref="BraillePlotBuilder"/> class.
    /// </summary>
    public BraillePlotBuilder()
    {
        _plot = new BraillePlot();
    }

    /// <summary>
    /// Sets the width of the plot.
    /// </summary>
    /// <param name="width">The width of the plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder Width(int width)
    {
        _plot.Width = width;
        return this;
    }

    /// <summary>
    /// Sets the height of the plot.
    /// </summary>
    /// <param name="height">The height of the plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder Height(int height)
    {
        _plot.Height = height;
        return this;
    }

    /// <summary>
    /// Sets the title of the plot.
    /// </summary>
    /// <param name="title">The title of the plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder Title(string title)
    {
        _plot.Title = title;
        return this;
    }

    /// <summary>
    /// Shows the X axis.
    /// </summary>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder ShowXAxis()
    {
        _plot.ShowXAxis = true;
        return this;
    }

    /// <summary>
    /// Hides the X axis.
    /// </summary>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder HideXAxis()
    {
        _plot.ShowXAxis = false;
        return this;
    }

    /// <summary>
    /// Shows the Y axis.
    /// </summary>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder ShowYAxis()
    {
        _plot.ShowYAxis = true;
        return this;
    }

    /// <summary>
    /// Hides the Y axis.
    /// </summary>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder HideYAxis()
    {
        _plot.ShowYAxis = false;
        return this;
    }

    /// <summary>
    /// Sets the number of X axis ticks.
    /// </summary>
    /// <param name="ticks">The number of X axis ticks.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder XAxisTicks(int ticks)
    {
        _plot.XAxisTicks = ticks;
        return this;
    }

    /// <summary>
    /// Sets the number of Y axis ticks.
    /// </summary>
    /// <param name="ticks">The number of Y axis ticks.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder YAxisTicks(int ticks)
    {
        _plot.YAxisTicks = ticks;
        return this;
    }

    /// <summary>
    /// Sets the line color.
    /// </summary>
    /// <param name="color">The line color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder LineColor(Color color)
    {
        _plot.LineColor = color;
        return this;
    }

    /// <summary>
    /// Sets the X range.
    /// </summary>
    /// <param name="min">The minimum X value.</param>
    /// <param name="max">The maximum X value.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder XRange(double min, double max)
    {
        _plot.XMin = min;
        _plot.XMax = max;
        return this;
    }

    /// <summary>
    /// Sets the Y range.
    /// </summary>
    /// <param name="min">The minimum Y value.</param>
    /// <param name="max">The maximum Y value.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder YRange(double min, double max)
    {
        _plot.YMin = min;
        _plot.YMax = max;
        return this;
    }

    /// <summary>
    /// Adds a data point to the plot.
    /// </summary>
    /// <param name="x">The X value.</param>
    /// <param name="y">The Y value.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddPoint(double x, double y)
    {
        _plot.Data.Add(new BraillePlotItem(x, y));
        return this;
    }

    /// <summary>
    /// Adds a data point to the plot.
    /// </summary>
    /// <param name="x">The X value.</param>
    /// <param name="y">The Y value.</param>
    /// <param name="color">The point color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddPoint(double x, double y, Color color)
    {
        _plot.Data.Add(new BraillePlotItem(x, y, color));
        return this;
    }

    /// <summary>
    /// Adds a data point to the plot.
    /// </summary>
    /// <param name="point">The data point.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddPoint(IBraillePlotItem point)
    {
        if (point is null)
        {
            throw new ArgumentNullException(nameof(point));
        }

        _plot.Data.Add(point);
        return this;
    }

    /// <summary>
    /// Adds multiple data points to the plot.
    /// </summary>
    /// <param name="points">The data points.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddPoints(IEnumerable<IBraillePlotItem> points)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        _plot.Data.AddRange(points);
        return this;
    }

    /// <summary>
    /// Adds multiple data points to the plot.
    /// </summary>
    /// <param name="points">The data points as tuples.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddPoints(IEnumerable<(double x, double y)> points)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        foreach (var (x, y) in points)
        {
            _plot.Data.Add(new BraillePlotItem(x, y));
        }

        return this;
    }

    /// <summary>
    /// Adds a sine wave to the plot.
    /// </summary>
    /// <param name="amplitude">The amplitude of the wave.</param>
    /// <param name="frequency">The frequency of the wave.</param>
    /// <param name="points">The number of points.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddSineWave(double amplitude = 1, double frequency = 1, int points = 100)
    {
        for (var i = 0; i < points; i++)
        {
            var x = i * 2 * Math.PI / points;
            var y = amplitude * Math.Sin(frequency * x);
            _plot.Data.Add(new BraillePlotItem(x, y));
        }

        return this;
    }

    /// <summary>
    /// Adds a cosine wave to the plot.
    /// </summary>
    /// <param name="amplitude">The amplitude of the wave.</param>
    /// <param name="frequency">The frequency of the wave.</param>
    /// <param name="points">The number of points.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddCosineWave(double amplitude = 1, double frequency = 1, int points = 100)
    {
        for (var i = 0; i < points; i++)
        {
            var x = i * 2 * Math.PI / points;
            var y = amplitude * Math.Cos(frequency * x);
            _plot.Data.Add(new BraillePlotItem(x, y));
        }

        return this;
    }

    /// <summary>
    /// Adds a sawtooth wave to the plot.
    /// </summary>
    /// <param name="amplitude">The amplitude of the wave.</param>
    /// <param name="frequency">The frequency of the wave.</param>
    /// <param name="points">The number of points.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public BraillePlotBuilder AddSawtoothWave(double amplitude = 1, double frequency = 1, int points = 100)
    {
        for (var i = 0; i < points; i++)
        {
            var x = i * 2 * Math.PI / points;
            var y = amplitude * (2 * (x * frequency / (2 * Math.PI) - Math.Floor(0.5 + x * frequency / (2 * Math.PI))));
            _plot.Data.Add(new BraillePlotItem(x, y));
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="BraillePlot"/>.
    /// </summary>
    /// <returns>A new <see cref="BraillePlot"/> instance.</returns>
    public BraillePlot Build()
    {
        return _plot;
    }

    /// <summary>
    /// Implicitly converts a <see cref="BraillePlotBuilder"/> to a <see cref="BraillePlot"/>.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>A new <see cref="BraillePlot"/> instance.</returns>
    public static implicit operator BraillePlot(BraillePlotBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.Build();
    }
}
