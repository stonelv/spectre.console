namespace Spectre.Console;

/// <summary>
/// Represents a single log entry in the log viewer.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the log entry.
    /// </summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category/logger name.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the exception associated with this log entry.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets or sets custom properties for this log entry.
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    public LogEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The log message.</param>
    public LogEntry(LogLevel level, string message)
    {
        Level = level;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The log message.</param>
    /// <param name="timestamp">The timestamp.</param>
    public LogEntry(LogLevel level, string message, DateTime timestamp)
        : this(level, message)
    {
        Timestamp = timestamp;
    }

    /// <summary>
    /// Creates a log entry with <see cref="LogLevel.Verbose"/> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <returns>A new <see cref="LogEntry"/> instance.</returns>
    public static LogEntry Verbose(string message) => new(LogLevel.Verbose, message);

    /// <summary>
    /// Creates a log entry with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <returns>A new <see cref="LogEntry"/> instance.</returns>
    public static LogEntry Debug(string message) => new(LogLevel.Debug, message);

    /// <summary>
    /// Creates a log entry with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <returns>A new <see cref="LogEntry"/> instance.</returns>
    public static LogEntry Info(string message) => new(LogLevel.Info, message);

    /// <summary>
    /// Creates a log entry with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <returns>A new <see cref="LogEntry"/> instance.</returns>
    public static LogEntry Warn(string message) => new(LogLevel.Warn, message);

    /// <summary>
    /// Creates a log entry with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <returns>A new <see cref="LogEntry"/> instance.</returns>
    public static LogEntry Error(string message) => new(LogLevel.Error, message);

    /// <summary>
    /// Creates a log entry with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <returns>A new <see cref="LogEntry"/> instance.</returns>
    public static LogEntry Fatal(string message) => new(LogLevel.Fatal, message);
}
