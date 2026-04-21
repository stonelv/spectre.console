namespace Spectre.Console.Cli;

/// <summary>
/// Represents the source of a command-line argument value.
/// </summary>
public enum ArgumentSource
{
    /// <summary>
    /// The value comes from the default value defined in the settings.
    /// </summary>
    DefaultValue,

    /// <summary>
    /// The value comes from the command-line arguments.
    /// </summary>
    CommandLine,

    /// <summary>
    /// The value comes from an environment variable.
    /// </summary>
    EnvironmentVariable,

    /// <summary>
    /// The value comes from a configuration file.
    /// </summary>
    ConfigurationFile,

    /// <summary>
    /// The value was not provided and has no default.
    /// </summary>
    NotProvided,
}
