namespace Spectre.Console;

/// <summary>
/// Represents the severity level of a log entry.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Verbose logging.
    /// </summary>
    Verbose,

    /// <summary>
    /// Debug logging.
    /// </summary>
    Debug,

    /// <summary>
    /// Informational logging.
    /// </summary>
    Info,

    /// <summary>
    /// Warning logging.
    /// </summary>
    Warn,

    /// <summary>
    /// Error logging.
    /// </summary>
    Error,

    /// <summary>
    /// Fatal/critical logging.
    /// </summary>
    Fatal,
}
