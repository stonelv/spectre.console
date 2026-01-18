namespace Spectre.Console;

/// <summary>
/// Represents a data point in a Braille plot.
/// </summary>
public sealed class BraillePlotItem : IBraillePlotItem
{
    /// <summary>
    /// Gets the X value.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Gets the Y value.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Gets the point color.
    /// </summary>
    public Color? Color { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BraillePlotItem"/> class.
    /// </summary>
    /// <param name="x">The X value.</param>
    /// <param name="y">The Y value.</param>
    /// <param name="color">The point color.</param>
    public BraillePlotItem(double x, double y, Color? color = null)
    {
        X = x;
        Y = y;
        Color = color;
    }
}
