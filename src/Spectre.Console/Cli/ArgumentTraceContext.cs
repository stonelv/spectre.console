namespace Spectre.Console.Cli;

/// <summary>
/// Represents the context for argument tracing, containing all trace information for a command.
/// </summary>
public class ArgumentTraceContext
{
    private readonly List<ArgumentTraceInfo> _arguments = new();

    /// <summary>
    /// Gets the command name.
    /// </summary>
    public string? CommandName { get; }

    /// <summary>
    /// Gets the command description.
    /// </summary>
    public string? CommandDescription { get; }

    /// <summary>
    /// Gets all traced arguments.
    /// </summary>
    public IReadOnlyList<ArgumentTraceInfo> Arguments => _arguments;

    /// <summary>
    /// Gets or sets a value indicating whether tracing was requested.
    /// </summary>
    public bool TraceRequested { get; set; }

    /// <summary>
    /// Gets or sets the parsing error message (if any).
    /// </summary>
    public string? ParsingError { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTraceContext"/> class.
    /// </summary>
    public ArgumentTraceContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTraceContext"/> class.
    /// </summary>
    /// <param name="commandName">The command name.</param>
    /// <param name="commandDescription">The command description.</param>
    public ArgumentTraceContext(string? commandName, string? commandDescription = null)
    {
        CommandName = commandName;
        CommandDescription = commandDescription;
    }

    /// <summary>
    /// Adds an argument trace info to the context.
    /// </summary>
    /// <param name="info">The argument trace info to add.</param>
    public void AddArgument(ArgumentTraceInfo info)
    {
        _arguments.Add(info);
    }

    /// <summary>
    /// Adds multiple argument trace infos to the context.
    /// </summary>
    /// <param name="infos">The argument trace infos to add.</param>
    public void AddArguments(IEnumerable<ArgumentTraceInfo> infos)
    {
        _arguments.AddRange(infos);
    }

    /// <summary>
    /// Gets arguments by their source.
    /// </summary>
    /// <param name="source">The source to filter by.</param>
    /// <returns>The arguments from the specified source.</returns>
    public IEnumerable<ArgumentTraceInfo> GetArgumentsBySource(ArgumentSource source)
    {
        return _arguments.Where(a => a.Source == source);
    }

    /// <summary>
    /// Gets arguments that have validation errors.
    /// </summary>
    /// <returns>The arguments with validation errors.</returns>
    public IEnumerable<ArgumentTraceInfo> GetArgumentsWithErrors()
    {
        return _arguments.Where(a => !string.IsNullOrEmpty(a.ValidationError));
    }

    /// <summary>
    /// Gets options (--flag style arguments).
    /// </summary>
    /// <returns>The options.</returns>
    public IEnumerable<ArgumentTraceInfo> GetOptions()
    {
        return _arguments.Where(a => a.IsOption);
    }

    /// <summary>
    /// Gets positional arguments.
    /// </summary>
    /// <returns>The positional arguments.</returns>
    public IEnumerable<ArgumentTraceInfo> GetPositionalArguments()
    {
        return _arguments.Where(a => !a.IsOption).OrderBy(a => a.Position);
    }

    /// <summary>
    /// Gets a summary of argument sources.
    /// </summary>
    /// <returns>A dictionary mapping sources to their counts.</returns>
    public Dictionary<ArgumentSource, int> GetSourceSummary()
    {
        return _arguments
            .GroupBy(a => a.Source)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
