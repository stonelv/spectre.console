using System.Collections;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace Spectre.Console.Cli;

/// <summary>
/// A value provider that retrieves values from a JSON configuration file.
/// </summary>
[UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode")]
[UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL3050:RequiresUnreferencedCode")]
public class JsonConfigValueProvider : IArgumentValueProvider
{
    private readonly string _filePath;
    private readonly Dictionary<string, string> _argumentToConfigPathMap;
    private readonly StringComparer _comparer;
    private Dictionary<string, object?>? _configCache;
    private bool _loaded;

    /// <inheritdoc/>
    public string Name => "JsonConfig";

    /// <inheritdoc/>
    public int Priority { get; set; } = 200;

    /// <inheritdoc/>
    public ArgumentSource Source => ArgumentSource.ConfigurationFile;

    /// <summary>
    /// Gets the path to the JSON configuration file.
    /// </summary>
    public string FilePath => _filePath;

    /// <summary>
    /// Gets or sets the JSON serializer options.
    /// </summary>
    public JsonSerializerOptions? JsonOptions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConfigValueProvider"/> class.
    /// </summary>
    /// <param name="filePath">The path to the JSON configuration file.</param>
    public JsonConfigValueProvider(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _argumentToConfigPathMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _comparer = StringComparer.OrdinalIgnoreCase;
        _loaded = false;
    }

    /// <summary>
    /// Maps an argument name to a configuration path.
    /// </summary>
    /// <param name="argumentName">The argument name (e.g., "configuration").</param>
    /// <param name="configPath">The configuration path (e.g., "build.configuration" or "Logging:LogLevel:Default").</param>
    /// <returns>The same <see cref="JsonConfigValueProvider"/> instance so that multiple calls can be chained.</returns>
    public JsonConfigValueProvider Map(string argumentName, string configPath)
    {
        _argumentToConfigPathMap[argumentName] = configPath;
        return this;
    }

    /// <inheritdoc/>
    public bool TryGetValue(string argumentName, IEnumerable<string>? aliases, out object? value)
    {
        value = null;

        EnsureLoaded();

        if (_configCache == null)
        {
            return false;
        }

        var configPaths = GetPossibleConfigPaths(argumentName, aliases);

        foreach (var configPath in configPaths)
        {
            if (TryGetValueFromPath(_configCache, configPath, out var configValue))
            {
                value = configValue;
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public string? GetSourceDetails(string argumentName)
    {
        EnsureLoaded();

        if (_configCache == null)
        {
            return null;
        }

        var configPaths = GetPossibleConfigPaths(argumentName, null);

        foreach (var configPath in configPaths)
        {
            if (TryGetValueFromPath(_configCache, configPath, out _))
            {
                return $"Config file: {_filePath} -> {configPath}";
            }
        }

        return null;
    }

    /// <summary>
    /// Ensures the configuration file is loaded.
    /// </summary>
    protected virtual void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;

        if (!File.Exists(_filePath))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var options = JsonOptions ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _configCache = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, options);
        }
        catch
        {
            _configCache = null;
        }
    }

    /// <summary>
    /// Gets all possible configuration paths for an argument.
    /// </summary>
    protected virtual IEnumerable<string> GetPossibleConfigPaths(string argumentName, IEnumerable<string>? aliases)
    {
        var paths = new List<string>();

        if (_argumentToConfigPathMap.TryGetValue(argumentName, out var mappedPath))
        {
            paths.Add(mappedPath);
        }

        paths.Add(argumentName);
        paths.Add(ToCamelCase(argumentName));
        paths.Add(ToKebabCase(argumentName));

        if (aliases != null)
        {
            foreach (var alias in aliases)
            {
                var cleanAlias = alias.TrimStart('-');
                paths.Add(cleanAlias);
                paths.Add(ToCamelCase(cleanAlias));
                paths.Add(ToKebabCase(cleanAlias));
            }
        }

        return paths.Distinct(_comparer);
    }

    /// <summary>
    /// Tries to get a value from a nested path in the configuration.
    /// </summary>
    protected virtual bool TryGetValueFromPath(Dictionary<string, object?> config, string path, out object? value)
    {
        value = null;

        var segments = path.Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries);
        object? current = config;

        for (int i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];

            if (current is IDictionary<string, object?> dict)
            {
                if (dict.TryGetValue(segment, out var nextValue) ||
                    dict.TryGetValue(ToCamelCase(segment), out nextValue) ||
                    dict.TryGetValue(segment.ToLowerInvariant(), out nextValue) ||
                    dict.TryGetValue(segment.ToUpperInvariant(), out nextValue))
                {
                    current = nextValue;

                    if (i == segments.Length - 1)
                    {
                        value = current;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (current is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    if (jsonElement.TryGetProperty(segment, out var nextElement) ||
                        jsonElement.TryGetProperty(ToCamelCase(segment), out nextElement) ||
                        jsonElement.TryGetProperty(segment.ToLowerInvariant(), out nextElement) ||
                        jsonElement.TryGetProperty(segment.ToUpperInvariant(), out nextElement))
                    {
                        current = nextElement;

                        if (i == segments.Length - 1)
                        {
                            value = JsonElementToObject(nextElement);
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Converts a JsonElement to an object.
    /// </summary>
    protected virtual object? JsonElementToObject(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    protected virtual string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    protected virtual string ToKebabCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var result = new StringBuilder();
        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && value[i - 1] != '-')
                {
                    result.Append('-');
                }
                result.Append(char.ToLowerInvariant(c));
            }
            else if (c == '_')
            {
                result.Append('-');
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
