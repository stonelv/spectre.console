namespace Spectre.Console;

/// <summary>
/// Represents the styles used by the <see cref="LogView"/> widget.
/// </summary>
public sealed class LogViewStyles
{
    /// <summary>
    /// Gets or sets the style for the timestamp.
    /// </summary>
    public Style TimestampStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the style for the verbose log level.
    /// </summary>
    public Style VerboseStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the style for the debug log level.
    /// </summary>
    public Style DebugStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the style for the info log level.
    /// </summary>
    public Style InfoStyle { get; set; } = new Style(Color.Blue);

    /// <summary>
    /// Gets or sets the style for the warn log level.
    /// </summary>
    public Style WarnStyle { get; set; } = new Style(Color.Yellow);

    /// <summary>
    /// Gets or sets the style for the error log level.
    /// </summary>
    public Style ErrorStyle { get; set; } = new Style(Color.Red);

    /// <summary>
    /// Gets or sets the style for the fatal log level.
    /// </summary>
    public Style FatalStyle { get; set; } = new Style(Color.Red, decoration: Decoration.Bold);

    /// <summary>
    /// Gets or sets the style for the category.
    /// </summary>
    public Style CategoryStyle { get; set; } = new Style(Color.Cyan);

    /// <summary>
    /// Gets or sets the style for the message.
    /// </summary>
    public Style MessageStyle { get; set; } = Style.Plain;

    /// <summary>
    /// Gets or sets the style for highlighted text (when filtering).
    /// </summary>
    public Style HighlightStyle { get; set; } = new Style(Color.Yellow, Color.Blue);

    /// <summary>
    /// Gets or sets the style for the border.
    /// </summary>
    public Style? BorderStyle { get; set; }

    /// <summary>
    /// Gets or sets the style for the header.
    /// </summary>
    public Style? HeaderStyle { get; set; }

    /// <summary>
    /// Gets or sets the style for collapsed group indicators.
    /// </summary>
    public Style CollapsedGroupStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the style for expanded group indicators.
    /// </summary>
    public Style ExpandedGroupStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets the style for a specific log level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <returns>The style for the specified log level.</returns>
    public Style GetLevelStyle(LogLevel level)
    {
        return level switch
        {
            LogLevel.Verbose => VerboseStyle,
            LogLevel.Debug => DebugStyle,
            LogLevel.Info => InfoStyle,
            LogLevel.Warn => WarnStyle,
            LogLevel.Error => ErrorStyle,
            LogLevel.Fatal => FatalStyle,
            _ => MessageStyle,
        };
    }
}
