namespace Spectre.Console;

/// <summary>
/// Represents a data point in a Braille plot.
/// </summary>
public interface IBraillePlotItem
{
    /// <summary>
    /// Gets the X value.
    /// </summary>
    double X { get; }

    /// <summary>
    /// Gets the Y value.
    /// </summary>
    double Y { get; }

    /// <summary>
    /// Gets the point color.
    /// </summary>
    Color? Color { get; }
}
