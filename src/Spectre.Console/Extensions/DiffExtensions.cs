namespace Spectre.Console;

/// <summary>
/// Extension methods for <see cref="Diff"/>.
/// </summary>
public static class DiffExtensions
{
    /// <summary>
    /// Sets the display mode for the diff.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="mode">The display mode.</param>
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
    /// Sets whether to show line numbers.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="show">Whether to show line numbers.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff ShowLineNumbers(this Diff diff, bool show)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.ShowLineNumbers = show;
        return diff;
    }

    /// <summary>
    /// Sets the style for inserted (added) lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff InsertedStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.InsertedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the style for deleted (removed) lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff DeletedStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.DeletedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the style for unchanged lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff UnchangedStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.UnchangedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the style for line numbers.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff LineNumberStyle(this Diff diff, Style style)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.LineNumberStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the column widths for side-by-side mode.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="leftWidth">The left column width.</param>
    /// <param name="rightWidth">The right column width.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff ColumnWidths(this Diff diff, int leftWidth, int rightWidth)
    {
        if (diff is null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.LeftColumnWidth = leftWidth;
        diff.RightColumnWidth = rightWidth;
        return diff;
    }
}
