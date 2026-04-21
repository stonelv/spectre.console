namespace Spectre.Console.Cli;

/// <summary>
/// Extension methods for argument tracing.
/// </summary>
public static class ArgumentTraceExtensions
{
    /// <summary>
    /// Writes the argument trace information to the console.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="context">The argument trace context.</param>
    /// <param name="showDetails">Whether to show detailed information.</param>
    /// <param name="showSummary">Whether to show a summary of argument sources.</param>
    public static void WriteArgumentTrace(
        this IAnsiConsole console,
        ArgumentTraceContext context,
        bool showDetails = false,
        bool showSummary = true)
    {
        if (console == null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var panel = new ArgumentTracePanel(context, showDetails: showDetails, showSummary: showSummary);
        console.Write(panel);
    }

    /// <summary>
    /// Creates a table that displays argument trace information.
    /// </summary>
    /// <param name="context">The argument trace context.</param>
    /// <param name="showDetails">Whether to show detailed information.</param>
    /// <returns>An <see cref="ArgumentTraceTable"/> instance.</returns>
    public static ArgumentTraceTable ToTraceTable(
        this ArgumentTraceContext context,
        bool showDetails = false)
    {
        return new ArgumentTraceTable(context, showDetails: showDetails);
    }

    /// <summary>
    /// Creates a panel that displays argument trace information.
    /// </summary>
    /// <param name="context">The argument trace context.</param>
    /// <param name="showDetails">Whether to show detailed information.</param>
    /// <param name="showSummary">Whether to show a summary of argument sources.</param>
    /// <returns>An <see cref="ArgumentTracePanel"/> instance.</returns>
    public static ArgumentTracePanel ToTracePanel(
        this ArgumentTraceContext context,
        bool showDetails = false,
        bool showSummary = true)
    {
        return new ArgumentTracePanel(context, showDetails: showDetails, showSummary: showSummary);
    }

    /// <summary>
    /// Adds an option trace info to the context.
    /// </summary>
    /// <param name="context">The argument trace context.</param>
    /// <param name="name">The option name.</param>
    /// <param name="aliases">The option aliases.</param>
    /// <param name="value">The option value.</param>
    /// <param name="source">The value source.</param>
    /// <param name="description">The option description.</param>
    /// <param name="isRequired">Whether the option is required.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="sourceDetails">Additional source details.</param>
    /// <returns>The same <see cref="ArgumentTraceContext"/> instance so that multiple calls can be chained.</returns>
    public static ArgumentTraceContext AddOption(
        this ArgumentTraceContext context,
        string name,
        IEnumerable<string>? aliases = null,
        object? value = null,
        ArgumentSource source = ArgumentSource.NotProvided,
        string? description = null,
        bool isRequired = false,
        object? defaultValue = null,
        string? sourceDetails = null)
    {
        var aliasList = aliases?.ToList() ?? new List<string> { name };

        context.AddArgument(new ArgumentTraceInfo
        {
            Name = name,
            Aliases = aliasList,
            Value = value,
            Source = source,
            Description = description,
            IsOption = true,
            IsRequired = isRequired,
            HasDefaultValue = defaultValue != null,
            DefaultValue = defaultValue,
            SourceDetails = sourceDetails
        });

        return context;
    }

    /// <summary>
    /// Adds a positional argument trace info to the context.
    /// </summary>
    /// <param name="context">The argument trace context.</param>
    /// <param name="name">The argument name.</param>
    /// <param name="position">The argument position.</param>
    /// <param name="value">The argument value.</param>
    /// <param name="source">The value source.</param>
    /// <param name="description">The argument description.</param>
    /// <param name="isRequired">Whether the argument is required.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="sourceDetails">Additional source details.</param>
    /// <param name="validationError">The validation error message.</param>
    /// <returns>The same <see cref="ArgumentTraceContext"/> instance so that multiple calls can be chained.</returns>
    public static ArgumentTraceContext AddArgument(
        this ArgumentTraceContext context,
        string name,
        int position,
        object? value = null,
        ArgumentSource source = ArgumentSource.NotProvided,
        string? description = null,
        bool isRequired = false,
        object? defaultValue = null,
        string? sourceDetails = null,
        string? validationError = null)
    {
        context.AddArgument(new ArgumentTraceInfo
        {
            Name = name,
            Position = position,
            Value = value,
            Source = source,
            Description = description,
            IsOption = false,
            IsRequired = isRequired,
            HasDefaultValue = defaultValue != null,
            DefaultValue = defaultValue,
            SourceDetails = sourceDetails,
            ValidationError = validationError
        });

        return context;
    }
}
