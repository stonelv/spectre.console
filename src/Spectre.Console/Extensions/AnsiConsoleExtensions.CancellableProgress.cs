namespace Spectre.Console;

/// <summary>
/// Contains extension methods for <see cref="IAnsiConsole"/>.
/// </summary>
public static partial class AnsiConsoleExtensions
{
    /// <summary>
    /// Creates a new <see cref="CancellableProgress"/> instance for the console.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <returns>A <see cref="CancellableProgress"/> instance.</returns>
    public static CancellableProgress CancellableProgress(this IAnsiConsole console)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        return new CancellableProgress(console);
    }
}
