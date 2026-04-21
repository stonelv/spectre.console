namespace Spectre.Console.Cli;

/// <summary>
/// A value provider that retrieves values from environment variables.
/// </summary>
public class EnvironmentVariableValueProvider : IArgumentValueProvider
{
    private readonly string? _prefix;
    private readonly Dictionary<string, string> _argumentToEnvVarMap;
    private readonly StringComparer _comparer;

    /// <inheritdoc/>
    public string Name => "EnvironmentVariable";

    /// <inheritdoc/>
    public int Priority { get; set; } = 100;

    /// <inheritdoc/>
    public ArgumentSource Source => ArgumentSource.EnvironmentVariable;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentVariableValueProvider"/> class.
    /// </summary>
    public EnvironmentVariableValueProvider()
    {
        _argumentToEnvVarMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _comparer = StringComparer.OrdinalIgnoreCase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentVariableValueProvider"/> class.
    /// </summary>
    /// <param name="prefix">The prefix to use for environment variable names (e.g., "MYAPP_").</param>
    public EnvironmentVariableValueProvider(string prefix)
        : this()
    {
        _prefix = prefix;
    }

    /// <summary>
    /// Maps an argument name to an environment variable name.
    /// </summary>
    /// <param name="argumentName">The argument name (e.g., "configuration").</param>
    /// <param name="envVarName">The environment variable name (e.g., "MYAPP_CONFIGURATION").</param>
    /// <returns>The same <see cref="EnvironmentVariableValueProvider"/> instance so that multiple calls can be chained.</returns>
    public EnvironmentVariableValueProvider Map(string argumentName, string envVarName)
    {
        _argumentToEnvVarMap[argumentName] = envVarName;
        return this;
    }

    /// <inheritdoc/>
    public bool TryGetValue(string argumentName, IEnumerable<string>? aliases, out object? value)
    {
        value = null;

        var envVarNames = GetPossibleEnvVarNames(argumentName, aliases);

        foreach (var envVarName in envVarNames)
        {
            var envValue = Environment.GetEnvironmentVariable(envVarName);
            if (envValue != null)
            {
                value = envValue;
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public string? GetSourceDetails(string argumentName)
    {
        var envVarNames = GetPossibleEnvVarNames(argumentName, null);

        foreach (var envVarName in envVarNames)
        {
            var envValue = Environment.GetEnvironmentVariable(envVarName);
            if (envValue != null)
            {
                return $"Environment variable: {envVarName}";
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all possible environment variable names for an argument.
    /// </summary>
    protected virtual IEnumerable<string> GetPossibleEnvVarNames(string argumentName, IEnumerable<string>? aliases)
    {
        var names = new List<string>();

        if (_argumentToEnvVarMap.TryGetValue(argumentName, out var mappedName))
        {
            names.Add(mappedName);
        }

        var prefixedName = GetPrefixedName(argumentName);
        if (!names.Contains(prefixedName, _comparer))
        {
            names.Add(prefixedName);
        }

        var upperName = argumentName.ToUpperInvariant();
        var prefixedUpperName = GetPrefixedName(upperName);
        if (!names.Contains(prefixedUpperName, _comparer))
        {
            names.Add(prefixedUpperName);
        }

        if (aliases != null)
        {
            foreach (var alias in aliases)
            {
                var cleanAlias = alias.TrimStart('-');
                var prefixedAlias = GetPrefixedName(cleanAlias);
                if (!names.Contains(prefixedAlias, _comparer))
                {
                    names.Add(prefixedAlias);
                }

                var upperAlias = cleanAlias.ToUpperInvariant();
                var prefixedUpperAlias = GetPrefixedName(upperAlias);
                if (!names.Contains(prefixedUpperAlias, _comparer))
                {
                    names.Add(prefixedUpperAlias);
                }
            }
        }

        return names;
    }

    /// <summary>
    /// Gets the prefixed name of an environment variable.
    /// </summary>
    protected virtual string GetPrefixedName(string name)
    {
        return string.IsNullOrEmpty(_prefix) ? name : $"{_prefix}{name}";
    }
}
