namespace Spectre.Console;

/// <summary>
/// Contains extension methods for <see cref="BraillePlot"/>.
/// </summary>
public static class BraillePlotExtensions
{
    /// <summary>
    /// Adds a data point to the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="x">The X value.</param>
    /// <param name="y">The Y value.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot AddPoint(this BraillePlot plot, double x, double y)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.Data.Add(new BraillePlotItem(x, y));
        return plot;
    }

    /// <summary>
    /// Adds a data point to the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="x">The X value.</param>
    /// <param name="y">The Y value.</param>
    /// <param name="color">The point color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot AddPoint(this BraillePlot plot, double x, double y, Color color)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.Data.Add(new BraillePlotItem(x, y, color));
        return plot;
    }

    /// <summary>
    /// Adds a data point to the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="point">The data point.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot AddPoint(this BraillePlot plot, IBraillePlotItem point)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        if (point is null)
        {
            throw new ArgumentNullException(nameof(point));
        }

        plot.Data.Add(point);
        return plot;
    }

    /// <summary>
    /// Adds multiple data points to the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="points">The data points.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot AddPoints(this BraillePlot plot, IEnumerable<IBraillePlotItem> points)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        plot.Data.AddRange(points);
        return plot;
    }

    /// <summary>
    /// Adds multiple data points to the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="points">The data points as tuples.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot AddPoints(this BraillePlot plot, IEnumerable<(double x, double y)> points)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        foreach (var (x, y) in points)
        {
            plot.Data.Add(new BraillePlotItem(x, y));
        }

        return plot;
    }

    /// <summary>
    /// Sets the width of the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="width">The width.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot Width(this BraillePlot plot, int? width)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.Width = width;
        return plot;
    }

    /// <summary>
    /// Sets the height of the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="height">The height.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot Height(this BraillePlot plot, int? height)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.Height = height;
        return plot;
    }

    /// <summary>
    /// Sets the title of the plot.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="title">The title.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot Title(this BraillePlot plot, string? title)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.Title = title;
        return plot;
    }

    /// <summary>
    /// Shows the X axis.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot ShowXAxis(this BraillePlot plot)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.ShowXAxis = true;
        return plot;
    }

    /// <summary>
    /// Hides the X axis.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot HideXAxis(this BraillePlot plot)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.ShowXAxis = false;
        return plot;
    }

    /// <summary>
    /// Shows the Y axis.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot ShowYAxis(this BraillePlot plot)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.ShowYAxis = true;
        return plot;
    }

    /// <summary>
    /// Hides the Y axis.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot HideYAxis(this BraillePlot plot)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }

        plot.ShowYAxis = false;
        return plot;
    }

    /// <summary>
    /// Sets the line color.
    /// </summary>
    /// <param name="plot">The Braille plot.</param>
    /// <param name="color">The color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static BraillePlot LineColor(this BraillePlot plot, Color color)
    {
        if (plot is null)
        {
            throw new ArgumentNullException(nameof(plot));
        }
        plot.LineColor = color;
        return plot;
    }

    /// <summary>
    /// Creates a new Braille plot builder.
    /// </summary>
    /// <returns>A new <see cref="BraillePlotBuilder"/> instance.</returns>
    public static BraillePlotBuilder BraillePlot()
    {
        return new BraillePlotBuilder();
    }
}
