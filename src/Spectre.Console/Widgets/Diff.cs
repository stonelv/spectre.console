using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Rendering;

namespace Spectre.Console.Widgets;

/// <summary>
/// Represents the rendering mode for a diff.
/// </summary>
public enum DiffMode
{
    /// <summary>
    /// Renders the diff inline.
    /// </summary>
    Inline,
    
    /// <summary>
    /// Renders the diff side by side.
    /// </summary>
    SideBySide
}

/// <summary>
/// A widget that displays a diff between two texts.
/// </summary>
public sealed class Diff : Renderable
{
    private readonly string _oldText;
    private readonly string _newText;
    private readonly DiffMode _mode;

    /// <summary>
    /// Gets or sets the style for unchanged lines.
    /// </summary>
    public Style? UnchangedStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the style for added lines.
    /// </summary>
    public Style? AddedStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the style for deleted lines.
    /// </summary>
    public Style? DeletedStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the width of the diff.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diff"/> class.
    /// </summary>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    /// <param name="mode">The rendering mode.</param>
    public Diff(string oldText, string newText, DiffMode mode = DiffMode.Inline)
    {
        _oldText = oldText ?? throw new ArgumentNullException(nameof(oldText));
        _newText = newText ?? throw new ArgumentNullException(nameof(newText));
        _mode = mode;
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        if (_mode == DiffMode.Inline)
        {
            // For inline mode, the width is the maximum line length
            var lines = _oldText.Split('\n').Concat(_newText.Split('\n'));
            var maxLineLength = lines.Any() ? lines.Max(line => line.Length) : 0;
            
            // Add space for the prefix
            var width = 2 + maxLineLength;
            return new Measurement(width, width);
        }
        else
        {
            // For side-by-side mode, we need to calculate the width for both columns
            var oldLines = _oldText.Split('\n');
            var newLines = _newText.Split('\n');
            
            var maxOldLineLength = oldLines.Length > 0 ? oldLines.Max(line => line.Length) : 0;
            var maxNewLineLength = newLines.Length > 0 ? newLines.Max(line => line.Length) : 0;
            
            // Add space for the prefix and separator
            var totalWidth = 1 + maxOldLineLength + 3 + maxNewLineLength + 1;
            var width = Width ?? Math.Min(totalWidth, maxWidth);
            
            return new Measurement(width, width);
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var changes = Internal.DiffAlgorithm.ComputeDiff(_oldText, _newText);
        
        if (_mode == DiffMode.Inline)
        {
            return RenderInline(changes, options, maxWidth);
        }
        else
        {
            return RenderSideBySide(changes, options, maxWidth);
        }
    }
    
    private IEnumerable<Segment> RenderInline(List<Internal.DiffChange> changes, RenderOptions options, int maxWidth)
    {
        var result = new List<Segment>();
        
        // Handle empty text case
        if (changes.Count == 0 || (changes.Count == 1 && string.IsNullOrEmpty(changes[0].Content)))
        {
            result.Add(Segment.LineBreak);
            return result;
        }
        
        foreach (var change in changes)
        {
            // Skip empty changes
            if (string.IsNullOrEmpty(change.Content) && change.ChangeType != Internal.DiffChangeType.Unchanged)
            {
                continue;
            }
            
            var prefix = GetPrefix(change.ChangeType);
            var style = GetStyle(change.ChangeType);
            
            // Add the prefix with appropriate style
            result.Add(new Segment(prefix, style));
            
            // Add the content with appropriate style
            result.Add(new Segment(change.Content, style));
            
            // Add line break
            result.Add(Segment.LineBreak);
        }
        
        return result;
    }
    
    private IEnumerable<Segment> RenderSideBySide(List<Internal.DiffChange> changes, RenderOptions options, int maxWidth)
    {
        var result = new List<Segment>();
        var oldLines = new List<string>();
        var newLines = new List<string>();
        var oldLineTypes = new List<Internal.DiffChangeType>();
        var newLineTypes = new List<Internal.DiffChangeType>();
        
        // Handle empty text case
        if (changes.Count == 0 || (changes.Count == 1 && string.IsNullOrEmpty(changes[0].Content)))
        {
            result.Add(Segment.LineBreak);
            return result;
        }
        
        // Separate the changes into old and new lines
        foreach (var change in changes)
        {
            switch (change.ChangeType)
            {
                case Internal.DiffChangeType.Unchanged:
                    oldLines.Add(change.Content);
                    newLines.Add(change.Content);
                    oldLineTypes.Add(Internal.DiffChangeType.Unchanged);
                    newLineTypes.Add(Internal.DiffChangeType.Unchanged);
                    break;
                case Internal.DiffChangeType.Deleted:
                    oldLines.Add(change.Content);
                    newLines.Add(string.Empty);
                    oldLineTypes.Add(Internal.DiffChangeType.Deleted);
                    newLineTypes.Add(Internal.DiffChangeType.Unchanged);
                    break;
                case Internal.DiffChangeType.Added:
                    oldLines.Add(string.Empty);
                    newLines.Add(change.Content);
                    oldLineTypes.Add(Internal.DiffChangeType.Unchanged);
                    newLineTypes.Add(Internal.DiffChangeType.Added);
                    break;
            }
        }
        
        // Calculate column widths
        var oldColumnWidth = oldLines.Count > 0 ? oldLines.Max(line => line.Length) : 0;
        var newColumnWidth = newLines.Count > 0 ? newLines.Max(line => line.Length) : 0;
        
        // Apply width constraint if specified
        var totalWidth = 1 + oldColumnWidth + 3 + newColumnWidth + 1;
        var maxWidthAvailable = Width ?? maxWidth;
        
        if (totalWidth > maxWidthAvailable)
        {
            // Distribute the available width between the two columns
            var availableContentWidth = maxWidthAvailable - 4; // Subtract separator and padding
            oldColumnWidth = availableContentWidth / 2;
            newColumnWidth = availableContentWidth - oldColumnWidth;
        }
        
        // Render the lines side by side
        var maxLines = Math.Max(oldLines.Count, newLines.Count);
        for (int i = 0; i < maxLines; i++)
        {
            var oldLine = i < oldLines.Count ? oldLines[i] : string.Empty;
            var newLine = i < newLines.Count ? newLines[i] : string.Empty;
            var oldLineType = i < oldLineTypes.Count ? oldLineTypes[i] : Internal.DiffChangeType.Unchanged;
            var newLineType = i < newLineTypes.Count ? newLineTypes[i] : Internal.DiffChangeType.Unchanged;
            
            // Render old line
            var oldStyle = GetStyle(oldLineType);
            if (!string.IsNullOrEmpty(oldLine))
            {
                var oldPrefix = GetSideBySidePrefix(oldLineType);
                result.Add(new Segment(oldPrefix, oldStyle));
                result.Add(new Segment(oldLine.PadRight(oldColumnWidth), oldStyle));
            }
            else if (oldLineType == Internal.DiffChangeType.Deleted)
            {
                // For deleted lines, just add the prefix
                result.Add(new Segment("-", oldStyle));
                result.Add(new Segment(new string(' ', oldColumnWidth), oldStyle));
            }
            else
            {
                // Just add spaces for empty lines
                result.Add(new Segment(new string(' ', oldColumnWidth + 1), oldStyle));
            }
            
            // Render separator
            result.Add(new Segment(" | ", Style.Plain));
            
            // Render new line
            var newStyle = GetStyle(newLineType);
            if (!string.IsNullOrEmpty(newLine))
            {
                var newPrefix = GetSideBySidePrefix(newLineType);
                result.Add(new Segment(newPrefix, newStyle));
                result.Add(new Segment(newLine.PadRight(newColumnWidth), newStyle));
            }
            else if (newLineType == Internal.DiffChangeType.Added)
            {
                // For added lines, just add the prefix
                result.Add(new Segment("+", newStyle));
                result.Add(new Segment(new string(' ', newColumnWidth), newStyle));
            }
            else if (oldLineType == Internal.DiffChangeType.Deleted)
            {
                // If the old line was deleted, don't add spaces for the empty new line
                // This ensures the output matches the expected format
            }
            else
            {
                // Just add spaces for empty lines
                result.Add(new Segment(new string(' ', newColumnWidth + 1), newStyle));
            }
            
            // Add line break
            result.Add(Segment.LineBreak);
        }
        
        return result;
    }
    
    private string GetPrefix(Internal.DiffChangeType changeType)
    {
        return changeType switch
        {
            Internal.DiffChangeType.Added => "+ ",
            Internal.DiffChangeType.Deleted => "- ",
            Internal.DiffChangeType.Unchanged => "  ",
            _ => "  ",
        };
    }

    private string GetSideBySidePrefix(Internal.DiffChangeType changeType)
    {
        return changeType switch
        {
            Internal.DiffChangeType.Added => "+",
            Internal.DiffChangeType.Deleted => "-",
            Internal.DiffChangeType.Unchanged => " ",
            _ => " ",
        };
    }

    private Style GetStyle(Internal.DiffChangeType changeType)
    {
        return changeType switch
        {
            Internal.DiffChangeType.Added => AddedStyle ?? new Style(Color.Green),
            Internal.DiffChangeType.Deleted => DeletedStyle ?? new Style(Color.Red),
            Internal.DiffChangeType.Unchanged => UnchangedStyle ?? Style.Plain,
            _ => Style.Plain,
        };
    }
}