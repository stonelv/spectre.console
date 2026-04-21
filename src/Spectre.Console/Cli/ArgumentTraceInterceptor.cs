namespace Spectre.Console.Cli;

/// <summary>
/// Provides interception capabilities for command execution, allowing argument tracing.
/// </summary>
/// <remarks>
/// This is a placeholder interface to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, implement Spectre.Console.Cli.ICommandInterceptor from the Spectre.Console.Cli NuGet package.
/// </remarks>
public interface ICommandInterceptor
{
    /// <summary>
    /// Called after settings are bound but before the command is executed.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings.</param>
    void Intercept(CommandContext context, CommandSettings settings);

    /// <summary>
    /// Called after the command is executed.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings.</param>
    /// <param name="exitCode">The exit code returned by the command.</param>
    void InterceptResult(CommandContext context, CommandSettings settings, ref int exitCode);
}

/// <summary>
/// An interceptor that collects and displays argument trace information during command execution.
/// </summary>
public class ArgumentTraceInterceptor : ICommandInterceptor
{
    private readonly IAnsiConsole _console;
    private readonly IArgumentTraceCollector _collector;
    private readonly List<IArgumentValueProvider> _valueProviders;
    private readonly bool _alwaysShowTrace;
    private readonly bool _showDetails;
    private readonly bool _showSummary;

    /// <summary>
    /// Gets or sets a value indicating whether the debug args flag was detected.
    /// </summary>
    public bool DebugArgsDetected { get; protected set; }

    /// <summary>
    /// Gets the trace context that was collected during interception.
    /// </summary>
    public ArgumentTraceContext? TraceContext { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTraceInterceptor"/> class.
    /// </summary>
    /// <param name="console">The console to use for output.</param>
    /// <param name="alwaysShowTrace">Whether to always show trace information, regardless of the --debug-args flag.</param>
    /// <param name="showDetails">Whether to show detailed information in the trace output.</param>
    /// <param name="showSummary">Whether to show a source summary in the trace output.</param>
    public ArgumentTraceInterceptor(
        IAnsiConsole? console = null,
        bool alwaysShowTrace = false,
        bool showDetails = false,
        bool showSummary = true)
    {
        _console = console ?? AnsiConsole.Console;
        _collector = new ArgumentTraceCollector(_console);
        _valueProviders = new List<IArgumentValueProvider>();
        _alwaysShowTrace = alwaysShowTrace;
        _showDetails = showDetails;
        _showSummary = showSummary;
    }

    /// <summary>
    /// Adds a value provider to the interceptor.
    /// </summary>
    /// <param name="provider">The value provider to add.</param>
    /// <returns>The same <see cref="ArgumentTraceInterceptor"/> instance so that multiple calls can be chained.</returns>
    public ArgumentTraceInterceptor AddValueProvider(IArgumentValueProvider provider)
    {
        _valueProviders.Add(provider);
        return this;
    }

    /// <summary>
    /// Adds multiple value providers to the interceptor.
    /// </summary>
    /// <param name="providers">The value providers to add.</param>
    /// <returns>The same <see cref="ArgumentTraceInterceptor"/> instance so that multiple calls can be chained.</returns>
    public ArgumentTraceInterceptor AddValueProviders(IEnumerable<IArgumentValueProvider> providers)
    {
        _valueProviders.AddRange(providers);
        return this;
    }

    /// <inheritdoc/>
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        DebugArgsDetected = CheckForDebugArgsFlag(context);

        TraceContext = _collector.Collect(context, settings, _valueProviders);
        TraceContext.TraceRequested = DebugArgsDetected || _alwaysShowTrace;

        if (TraceContext.TraceRequested)
        {
            RenderTrace(TraceContext);
        }
    }

    /// <inheritdoc/>
    public void InterceptResult(CommandContext context, CommandSettings settings, ref int exitCode)
    {
    }

    /// <summary>
    /// Checks if the --debug-args flag is present in the remaining arguments.
    /// </summary>
    protected virtual bool CheckForDebugArgsFlag(CommandContext context)
    {
        if (context.Remaining == null)
        {
            return false;
        }

        foreach (var arg in context.Remaining.Parsed.Concat(context.Remaining.Remaining))
        {
            if (arg.Equals("--debug-args", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("--debugargs", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Renders the trace information to the console.
    /// </summary>
    protected virtual void RenderTrace(ArgumentTraceContext context)
    {
        _console.WriteLine();
        _console.Write(new Rule("[bold yellow]Argument Trace[/]")
        {
            Justification = Justify.Left,
            Style = Style.Plain
        });
        _console.WriteLine();

        var panel = new ArgumentTracePanel(context, showDetails: _showDetails, showSummary: _showSummary)
        {
            Border = BoxBorder.Rounded
        };

        _console.Write(panel);
        _console.WriteLine();
    }
}
