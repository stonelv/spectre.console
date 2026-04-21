namespace Spectre.Console.Cli;

/// <summary>
/// Represents a collector that can gather argument trace information from command settings.
/// </summary>
public interface IArgumentTraceCollector
{
    /// <summary>
    /// Collects trace information from the specified command settings.
    /// </summary>
    /// <param name="settings">The command settings to collect trace information from.</param>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="valueProviders">The value providers to use for determining value sources.</param>
    /// <returns>An <see cref="ArgumentTraceContext"/> containing the collected trace information.</returns>
    ArgumentTraceContext Collect(
        CommandSettings settings,
        string? commandName = null,
        IEnumerable<IArgumentValueProvider>? valueProviders = null);

    /// <summary>
    /// Collects trace information from the specified command settings and command context.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings to collect trace information from.</param>
    /// <param name="valueProviders">The value providers to use for determining value sources.</param>
    /// <returns>An <see cref="ArgumentTraceContext"/> containing the collected trace information.</returns>
    ArgumentTraceContext Collect(
        CommandContext context,
        CommandSettings settings,
        IEnumerable<IArgumentValueProvider>? valueProviders = null);
}
