namespace Spectre.Console.Cli;

/// <summary>
/// Represents the styles used for argument trace rendering.
/// </summary>
public class ArgumentTraceStyles
{
    /// <summary>
    /// Gets or sets the style for the header.
    /// </summary>
    public Style HeaderStyle { get; set; } = new Style(Color.Yellow);

    /// <summary>
    /// Gets or sets the style for the argument name.
    /// </summary>
    public Style NameStyle { get; set; } = new Style(Color.Cyan);

    /// <summary>
    /// Gets or sets the style for the argument value.
    /// </summary>
    public Style ValueStyle { get; set; } = new Style(Color.Green);

    /// <summary>
    /// Gets or sets the style for the source.
    /// </summary>
    public Style SourceStyle { get; set; } = new Style(Color.Magenta);

    /// <summary>
    /// Gets or sets the style for the description.
    /// </summary>
    public Style DescriptionStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the style for error messages.
    /// </summary>
    public Style ErrorStyle { get; set; } = new Style(Color.Red);

    /// <summary>
    /// Gets or sets the style for required indicators.
    /// </summary>
    public Style RequiredStyle { get; set; } = new Style(Color.Yellow);

    /// <summary>
    /// Gets or sets the style for default values.
    /// </summary>
    public Style DefaultValueStyle { get; set; } = new Style(Color.Grey);

    /// <summary>
    /// Gets or sets the border style for panels and tables.
    /// </summary>
    public Style BorderStyle { get; set; } = new Style(Color.Blue);

    /// <summary>
    /// Gets the style for a specific argument source.
    /// </summary>
    /// <param name="source">The argument source.</param>
    /// <returns>The style for the specified source.</returns>
    public virtual Style GetSourceStyle(ArgumentSource source)
    {
        return source switch
        {
            ArgumentSource.DefaultValue => DefaultValueStyle,
            ArgumentSource.CommandLine => new Style(Color.Green),
            ArgumentSource.EnvironmentVariable => new Style(Color.Magenta),
            ArgumentSource.ConfigurationFile => new Style(Color.Cyan),
            ArgumentSource.NotProvided => new Style(Color.Grey),
            _ => SourceStyle
        };
    }

    /// <summary>
    /// Gets the default argument trace styles.
    /// </summary>
    public static ArgumentTraceStyles Default { get; } = new ArgumentTraceStyles();
}
