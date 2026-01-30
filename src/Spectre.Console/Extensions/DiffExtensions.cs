using System;
using Spectre.Console.Widgets;

namespace Spectre.Console.Extensions;

/// <summary>
/// Extension methods for the <see cref="Diff"/> widget.
/// </summary>
public static class DiffExtensions
{
    /// <summary>
    /// Writes a diff between two texts to the console.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    /// <param name="mode">The rendering mode.</param>
    /// <returns>A <see cref="Diff"/> widget that can be further configured.</returns>
    public static Diff Diff(this IAnsiConsole console, string oldText, string newText, DiffMode mode = DiffMode.Inline)
    {
        return new Diff(oldText, newText, mode);
    }
    
    /// <summary>
    /// Writes a diff between two texts to the console.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    /// <param name="mode">The rendering mode.</param>
    public static void WriteDiff(this IAnsiConsole console, string oldText, string newText, DiffMode mode = DiffMode.Inline)
    {
        var diff = new Diff(oldText, newText, mode);
        console.Write(diff);
    }
    
    /// <summary>
    /// Writes a diff between two texts to the console with custom styles.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    /// <param name="mode">The rendering mode.</param>
    /// <param name="unchangedStyle">The style for unchanged lines.</param>
    /// <param name="addedStyle">The style for added lines.</param>
    /// <param name="deletedStyle">The style for deleted lines.</param>
    public static void WriteDiff(
        this IAnsiConsole console, 
        string oldText, 
        string newText, 
        DiffMode mode = DiffMode.Inline,
        Style? unchangedStyle = null,
        Style? addedStyle = null,
        Style? deletedStyle = null)
    {
        var diff = new Diff(oldText, newText, mode)
        {
            UnchangedStyle = unchangedStyle,
            AddedStyle = addedStyle,
            DeletedStyle = deletedStyle
        };
        console.Write(diff);
    }
}