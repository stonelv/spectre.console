using System.Reflection;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Spectre.Console.Cli;

/// <summary>
/// Default implementation of <see cref="IArgumentTraceCollector"/> that uses reflection to collect trace information.
/// </summary>
[UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode")]
[UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2070:RequiresUnreferencedCode")]
[UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2075:RequiresUnreferencedCode")]
[UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL3050:RequiresUnreferencedCode")]
public class ArgumentTraceCollector : IArgumentTraceCollector
{
    private readonly IAnsiConsole? _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTraceCollector"/> class.
    /// </summary>
    public ArgumentTraceCollector()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTraceCollector"/> class.
    /// </summary>
    /// <param name="console">The console to use for output.</param>
    public ArgumentTraceCollector(IAnsiConsole? console)
    {
        _console = console;
    }

    /// <inheritdoc/>
    public ArgumentTraceContext Collect(
        CommandSettings settings,
        string? commandName = null,
        IEnumerable<IArgumentValueProvider>? valueProviders = null)
    {
        var context = new ArgumentTraceContext(commandName);
        var providers = valueProviders?.ToList() ?? new List<IArgumentValueProvider>();

        var settingsType = settings.GetType();
        var properties = settingsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var traceInfo = CreateTraceInfo(settings, property, providers);
            if (traceInfo != null)
            {
                context.AddArgument(traceInfo);
            }
        }

        return context;
    }

    /// <inheritdoc/>
    public ArgumentTraceContext Collect(
        CommandContext context,
        CommandSettings settings,
        IEnumerable<IArgumentValueProvider>? valueProviders = null)
    {
        var traceContext = Collect(settings, context.Name, valueProviders);
        return traceContext;
    }

    /// <summary>
    /// Creates trace information for a single property.
    /// </summary>
    protected virtual ArgumentTraceInfo? CreateTraceInfo(
        CommandSettings settings,
        PropertyInfo property,
        IReadOnlyList<IArgumentValueProvider> valueProviders)
    {
        var value = property.GetValue(settings);
        var isOption = false;
        var position = default(int?);
        var aliases = new List<string>();
        var description = string.Empty;
        var hasDefaultValue = false;
        object? defaultValue = null;

        var commandOptionAttr = property.GetCustomAttribute<CommandOptionAttribute>();
        if (commandOptionAttr != null)
        {
            isOption = true;
            aliases.AddRange(ParseOptionNames(commandOptionAttr.ValueNames));
        }

        var commandArgumentAttr = property.GetCustomAttribute<CommandArgumentAttribute>();
        if (commandArgumentAttr != null)
        {
            position = commandArgumentAttr.Position;
            aliases.Add(commandArgumentAttr.ValueName);
        }

        var descriptionAttr = property.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttr != null)
        {
            description = descriptionAttr.Description;
        }

        var defaultValueAttr = property.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValueAttr != null)
        {
            hasDefaultValue = true;
            defaultValue = defaultValueAttr.Value;
        }

        var source = DetermineSource(settings, property, value, valueProviders, hasDefaultValue, defaultValue);
        var sourceDetails = GetSourceDetails(settings, property, source, valueProviders);

        return new ArgumentTraceInfo
        {
            Name = property.Name,
            Aliases = aliases.Count > 0 ? aliases : new List<string> { property.Name },
            Value = value,
            ValueType = property.PropertyType,
            Source = source,
            SourceDetails = sourceDetails,
            Description = description,
            IsOption = isOption,
            Position = position,
            HasDefaultValue = hasDefaultValue,
            DefaultValue = defaultValue,
            IsRequired = DetermineIfRequired(commandArgumentAttr, commandOptionAttr)
        };
    }

    /// <summary>
    /// Determines the source of a property value.
    /// </summary>
    protected virtual ArgumentSource DetermineSource(
        CommandSettings settings,
        PropertyInfo property,
        object? currentValue,
        IReadOnlyList<IArgumentValueProvider> valueProviders,
        bool hasDefaultValue,
        object? defaultValue)
    {
        if (currentValue == null && !hasDefaultValue)
        {
            return ArgumentSource.NotProvided;
        }

        foreach (var provider in valueProviders.OrderByDescending(p => p.Priority))
        {
            if (provider.TryGetValue(property.Name, null, out var providerValue))
            {
                if (ValuesEqual(currentValue, providerValue))
                {
                    return provider.Source;
                }
            }
        }

        if (hasDefaultValue && ValuesEqual(currentValue, defaultValue))
        {
            return ArgumentSource.DefaultValue;
        }

        return ArgumentSource.CommandLine;
    }

    /// <summary>
    /// Gets source details for a property.
    /// </summary>
    protected virtual string? GetSourceDetails(
        CommandSettings settings,
        PropertyInfo property,
        ArgumentSource source,
        IReadOnlyList<IArgumentValueProvider> valueProviders)
    {
        foreach (var provider in valueProviders.Where(p => p.Source == source))
        {
            var details = provider.GetSourceDetails(property.Name);
            if (!string.IsNullOrEmpty(details))
            {
                return details;
            }
        }

        return null;
    }

    /// <summary>
    /// Determines if an argument is required.
    /// </summary>
    protected virtual bool DetermineIfRequired(
        CommandArgumentAttribute? argumentAttr,
        CommandOptionAttribute? optionAttr)
    {
        if (argumentAttr != null)
        {
            return argumentAttr.ValueName.StartsWith("<") && argumentAttr.ValueName.EndsWith(">");
        }

        return false;
    }

    /// <summary>
    /// Parses option names from the option attribute.
    /// </summary>
    protected virtual List<string> ParseOptionNames(string? valueNames)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(valueNames))
        {
            return result;
        }

        var parts = valueNames.Split('|');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                var nameOnly = trimmed.Split(' ')[0].Trim();
                result.Add(nameOnly);
            }
        }

        return result;
    }

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    protected virtual bool ValuesEqual(object? value1, object? value2)
    {
        if (value1 == null && value2 == null)
        {
            return true;
        }

        if (value1 == null || value2 == null)
        {
            return false;
        }

        return EqualityComparer<object>.Default.Equals(value1, value2);
    }
}

/// <summary>
/// Represents an attribute that specifies a command option.
/// </summary>
/// <remarks>
/// This is a placeholder attribute to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, use Spectre.Console.Cli.CommandOptionAttribute from the Spectre.Console.Cli NuGet package.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CommandOptionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the value names.
    /// </summary>
    public string ValueNames { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandOptionAttribute"/> class.
    /// </summary>
    /// <param name="valueNames">The value names.</param>
    public CommandOptionAttribute(string valueNames)
    {
        ValueNames = valueNames;
    }
}

/// <summary>
/// Represents an attribute that specifies a command argument.
/// </summary>
/// <remarks>
/// This is a placeholder attribute to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, use Spectre.Console.Cli.CommandArgumentAttribute from the Spectre.Console.Cli NuGet package.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CommandArgumentAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the value name.
    /// </summary>
    public string ValueName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgumentAttribute"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="valueName">The value name.</param>
    public CommandArgumentAttribute(int position, string valueName)
    {
        Position = position;
        ValueName = valueName;
    }
}

/// <summary>
/// Represents the base class for command settings.
/// </summary>
/// <remarks>
/// This is a placeholder class to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, inherit from Spectre.Console.Cli.CommandSettings from the Spectre.Console.Cli NuGet package.
/// </remarks>
public class CommandSettings
{
    /// <summary>
    /// Validates the settings.
    /// </summary>
    /// <returns>A validation result.</returns>
    public virtual ValidationResult Validate()
    {
        return ValidationResult.Success();
    }
}

/// <summary>
/// Represents the context in which a command is executed.
/// </summary>
/// <remarks>
/// This is a placeholder class to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, use Spectre.Console.Cli.CommandContext from the Spectre.Console.Cli NuGet package.
/// </remarks>
public class CommandContext
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string? Name { get; protected set; }

    /// <summary>
    /// Gets the remaining arguments.
    /// </summary>
    public IRemainingArguments? Remaining { get; protected set; }
}

/// <summary>
/// Represents the remaining arguments after parsing.
/// </summary>
/// <remarks>
/// This is a placeholder interface to allow compilation without the full Spectre.Console.Cli package.
/// </remarks>
public interface IRemainingArguments
{
    /// <summary>
    /// Gets the parsed arguments.
    /// </summary>
    IReadOnlyList<string> Parsed { get; }

    /// <summary>
    /// Gets the remaining arguments.
    /// </summary>
    IReadOnlyList<string> Remaining { get; }
}
