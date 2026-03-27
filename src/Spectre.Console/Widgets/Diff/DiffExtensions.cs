using System;

namespace Spectre.Console;

/// <summary>
/// Contains extension methods for <see cref="Diff"/>.
/// </summary>
public static class DiffExtensions
{
    /// <summary>
    /// Writes a diff to the console.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="original">The original text.</param>
    /// <param name="modified">The modified text.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static IAnsiConsole WriteDiff(this IAnsiConsole console, string original, string modified)
    {
        if (console == null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        console.Write(new Diff(original, modified));
        return console;
    }

    /// <summary>
    /// Writes a diff to the console.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="original">The original lines.</param>
    /// <param name="modified">The modified lines.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static IAnsiConsole WriteDiff(this IAnsiConsole console, string[] original, string[] modified)
    {
        if (console == null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        console.Write(new Diff(original, modified));
        return console;
    }

    /// <summary>
    /// Sets the diff mode.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="mode">The diff mode.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithMode(this Diff diff, DiffMode mode)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.Mode = mode;
        return diff;
    }

    /// <summary>
    /// Sets the style for added lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithAddedStyle(this Diff diff, Style style)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        diff.AddedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the style for removed lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithRemovedStyle(this Diff diff, Style style)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        diff.RemovedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the style for unchanged lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithUnchangedStyle(this Diff diff, Style style)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        diff.UnchangedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the style for modified lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="style">The style.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithModifiedStyle(this Diff diff, Style style)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        diff.ModifiedStyle = style;
        return diff;
    }

    /// <summary>
    /// Sets the indicator for added lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="indicator">The indicator.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithAddedIndicator(this Diff diff, string indicator)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (indicator == null)
        {
            throw new ArgumentNullException(nameof(indicator));
        }

        diff.AddedIndicator = indicator;
        return diff;
    }

    /// <summary>
    /// Sets the indicator for removed lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="indicator">The indicator.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithRemovedIndicator(this Diff diff, string indicator)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (indicator == null)
        {
            throw new ArgumentNullException(nameof(indicator));
        }

        diff.RemovedIndicator = indicator;
        return diff;
    }

    /// <summary>
    /// Sets the indicator for unchanged lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="indicator">The indicator.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithUnchangedIndicator(this Diff diff, string indicator)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (indicator == null)
        {
            throw new ArgumentNullException(nameof(indicator));
        }

        diff.UnchangedIndicator = indicator;
        return diff;
    }

    /// <summary>
    /// Sets the indicator for modified lines.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="indicator">The indicator.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithModifiedIndicator(this Diff diff, string indicator)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (indicator == null)
        {
            throw new ArgumentNullException(nameof(indicator));
        }

        diff.ModifiedIndicator = indicator;
        return diff;
    }

    /// <summary>
    /// Sets the width of the line number column.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="width">The width.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithLineNumberWidth(this Diff diff, int width)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 1.");
        }

        diff.LineNumberWidth = width;
        return diff;
    }

    /// <summary>
    /// Sets whether line numbers should be shown.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="show">Whether line numbers should be shown.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithLineNumbers(this Diff diff, bool show = true)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.ShowLineNumbers = show;
        return diff;
    }

    /// <summary>
    /// Sets the alignment.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="alignment">The alignment.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithAlignment(this Diff diff, Justify alignment)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.Alignment = alignment;
        return diff;
    }

    /// <summary>
    /// Sets whether headers should be shown.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="show">Whether headers should be shown.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithHeaders(this Diff diff, bool show = true)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        diff.ShowHeaders = show;
        return diff;
    }

    /// <summary>
    /// Sets the header for the original text.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="header">The header.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithOriginalHeader(this Diff diff, string header)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (header == null)
        {
            throw new ArgumentNullException(nameof(header));
        }

        diff.OriginalHeader = header;
        return diff;
    }

    /// <summary>
    /// Sets the header for the modified text.
    /// </summary>
    /// <param name="diff">The diff.</param>
    /// <param name="header">The header.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static Diff WithModifiedHeader(this Diff diff, string header)
    {
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        if (header == null)
        {
            throw new ArgumentNullException(nameof(header));
        }

        diff.ModifiedHeader = header;
        return diff;
    }
}