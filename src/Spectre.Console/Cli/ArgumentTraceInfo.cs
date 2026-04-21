namespace Spectre.Console.Cli;

/// <summary>
/// Represents trace information for a single command-line argument.
/// </summary>
public class ArgumentTraceInfo
{
    /// <summary>
    /// Gets or sets the name of the argument (e.g., "--verbose", "-v", "input").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the argument (for help and error messages).
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the description of the argument.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the type of the argument value.
    /// </summary>
    public Type? ValueType { get; set; }

    /// <summary>
    /// Gets or sets the final resolved value of the argument.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the source of the argument value.
    /// </summary>
    public ArgumentSource Source { get; set; } = ArgumentSource.NotProvided;

    /// <summary>
    /// Gets or sets additional details about the source (e.g., environment variable name, config file path).
    /// </summary>
    public string? SourceDetails { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the argument is required.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the argument has a default value.
    /// </summary>
    public bool HasDefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the default value of the argument.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is an option (--flag) or an argument (positional).
    /// </summary>
    public bool IsOption { get; set; }

    /// <summary>
    /// Gets or sets the order of the argument (for positional arguments).
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// Gets or sets the aliases for the argument (e.g., ["--verbose", "-v"]).
    /// </summary>
    public IReadOnlyList<string>? Aliases { get; set; }

    /// <summary>
    /// Gets or sets any validation error associated with this argument.
    /// </summary>
    public string? ValidationError { get; set; }

    /// <summary>
    /// Gets the display name of the source.
    /// </summary>
    public string SourceDisplayName => Source switch
    {
        ArgumentSource.DefaultValue => "Default Value",
        ArgumentSource.CommandLine => "Command Line",
        ArgumentSource.EnvironmentVariable => "Environment Variable",
        ArgumentSource.ConfigurationFile => "Configuration File",
        ArgumentSource.NotProvided => "Not Provided",
        _ => Source.ToString()
    };

    /// <summary>
    /// Gets the formatted value as a string.
    /// </summary>
    public string FormattedValue
    {
        get
        {
            if (Value == null)
            {
                return "(null)";
            }

            if (Value is Array array)
            {
                var values = new List<string>();
                foreach (var item in array)
                {
                    values.Add(item?.ToString() ?? "(null)");
                }
                return $"[{string.Join(", ", values)}]";
            }

            return Value.ToString() ?? "(null)";
        }
    }

    /// <summary>
    /// Gets the full name including aliases.
    /// </summary>
    public string FullName
    {
        get
        {
            if (Aliases == null || Aliases.Count == 0)
            {
                return Name;
            }

            return string.Join(" | ", Aliases);
        }
    }
}
