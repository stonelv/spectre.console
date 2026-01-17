namespace Spectre.Console;

/// <summary>
/// Contains extension methods for <see cref="Diff"/>.
/// </summary>
public static class DiffExtensions
{
    /// <summary>
    /// Sets the rendering mode for the diff.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="mode">The rendering mode.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff Mode(this Diff diff, DiffMode mode)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.Mode = mode;
        return diff;
    }

    /// <summary>
    /// Sets the style for deleted lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff DeletedStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.DeletedStyle = style ?? Style.Plain;
        return diff;
    }

    /// <summary>
    /// Sets the style for inserted lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff InsertedStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.InsertedStyle = style ?? Style.Plain;
        return diff;
    }

    /// <summary>
    /// Sets the style for unchanged lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff UnchangedStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.UnchangedStyle = style ?? Style.Plain;
        return diff;
    }

    /// <summary>
    /// Sets the marker for deleted lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="marker">The marker.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff DeletedMarker(this Diff diff, string marker)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.DeletedMarker = marker ?? "-";
        return diff;
    }

    /// <summary>
    /// Sets the marker for inserted lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="marker">The marker.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff InsertedMarker(this Diff diff, string marker)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.InsertedMarker = marker ?? "+";
        return diff;
    }

    /// <summary>
    /// Sets the marker for unchanged lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="marker">The marker.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff UnchangedMarker(this Diff diff, string marker)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.UnchangedMarker = marker ?? " ";
        return diff;
    }

    /// <summary>
    /// Sets the color for deleted lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="color">The color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff DeletedColor(this Diff diff, Color color)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.DeletedStyle = new Style(color);
        return diff;
    }

    /// <summary>
    /// Sets the color for inserted lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="color">The color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff InsertedColor(this Diff diff, Color color)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.InsertedStyle = new Style(color);
        return diff;
    }

    /// <summary>
    /// Sets the color for unchanged lines.
    /// </summary>
    /// <param name="diff">The diff instance.</param>
    /// <param name="color">The color.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff UnchangedColor(this Diff diff, Color color)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.UnchangedStyle = new Style(color);
        return diff;
    }
}
