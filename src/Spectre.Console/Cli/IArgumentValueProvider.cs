namespace Spectre.Console.Cli;

/// <summary>
/// Represents a provider that can retrieve argument values from a specific source.
/// </summary>
public interface IArgumentValueProvider
{
    /// <summary>
    /// Gets the name of the provider (e.g., "EnvironmentVariable", "JsonConfig").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the priority of the provider. Higher priority means the provider is checked first.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the argument source type that this provider represents.
    /// </summary>
    ArgumentSource Source { get; }

    /// <summary>
    /// Tries to get a value for the specified argument name.
    /// </summary>
    /// <param name="argumentName">The name of the argument (e.g., "configuration", "output").</param>
    /// <param name="aliases">The aliases of the argument (e.g., ["--configuration", "-c"]).</param>
    /// <param name="value">When this method returns, contains the value if found; otherwise, null.</param>
    /// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
    bool TryGetValue(string argumentName, IEnumerable<string>? aliases, out object? value);

    /// <summary>
    /// Gets additional details about how the value was retrieved (e.g., environment variable name, config file path).
    /// </summary>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>A string containing additional details, or null if not applicable.</returns>
    string? GetSourceDetails(string argumentName);
}
