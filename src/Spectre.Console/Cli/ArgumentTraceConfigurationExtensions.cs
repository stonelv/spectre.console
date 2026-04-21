namespace Spectre.Console.Cli;

/// <summary>
/// Provides configuration extensions for argument tracing.
/// </summary>
public static class ArgumentTraceConfigurationExtensions
{
    /// <summary>
    /// Enables argument tracing for the command application.
    /// </summary>
    /// <param name="configurator">The command app configurator.</param>
    /// <param name="configure">Optional configuration for the argument trace interceptor.</param>
    /// <returns>The same <see cref="IConfigurator"/> instance so that multiple calls can be chained.</returns>
    public static IConfigurator UseArgumentTracing(
        this IConfigurator configurator,
        Action<ArgumentTraceOptions>? configure = null)
    {
        var options = new ArgumentTraceOptions();
        configure?.Invoke(options);

        var interceptor = CreateInterceptor(options);

        configurator.SetInterceptor(interceptor);

        if (options.EnableExceptionHandling)
        {
            configurator.SetExceptionHandler((ex, typeResolver) =>
            {
                if (options.AlwaysShowTrace || options.ShowTraceOnError)
                {
                    var console = options.Console ?? AnsiConsole.Console;
                    RenderExceptionTrace(console, ex, options);
                }

                return -1;
            });
        }

        return configurator;
    }

    /// <summary>
    /// Creates an argument trace interceptor with the specified options.
    /// </summary>
    /// <param name="options">The options for the interceptor.</param>
    /// <returns>An <see cref="ArgumentTraceInterceptor"/> instance.</returns>
    public static ArgumentTraceInterceptor CreateArgumentTraceInterceptor(ArgumentTraceOptions options)
    {
        return CreateInterceptor(options);
    }

    private static ArgumentTraceInterceptor CreateInterceptor(ArgumentTraceOptions options)
    {
        var interceptor = new ArgumentTraceInterceptor(
            console: options.Console,
            alwaysShowTrace: options.AlwaysShowTrace,
            showDetails: options.ShowDetails,
            showSummary: options.ShowSummary);

        if (options.EnableEnvironmentVariables)
        {
            var envProvider = options.EnvironmentVariablePrefix != null
                ? new EnvironmentVariableValueProvider(options.EnvironmentVariablePrefix)
                : new EnvironmentVariableValueProvider();
            foreach (var mapping in options.EnvironmentVariableMappings)
            {
                envProvider.Map(mapping.Key, mapping.Value);
            }
            interceptor.AddValueProvider(envProvider);
        }

        if (options.EnableJsonConfig && !string.IsNullOrEmpty(options.JsonConfigPath))
        {
            var jsonProvider = new JsonConfigValueProvider(options.JsonConfigPath);
            foreach (var mapping in options.JsonConfigMappings)
            {
                jsonProvider.Map(mapping.Key, mapping.Value);
            }
            interceptor.AddValueProvider(jsonProvider);
        }

        foreach (var provider in options.AdditionalValueProviders)
        {
            interceptor.AddValueProvider(provider);
        }

        return interceptor;
    }

    private static void RenderExceptionTrace(IAnsiConsole console, Exception ex, ArgumentTraceOptions options)
    {
        console.WriteLine();
        console.Write(new Rule("[bold red]Command Execution Error[/]")
        {
            Justification = Justify.Left,
            Style = Style.Plain
        });
        console.WriteLine();

        var exceptionPanel = new Panel(new Markup($"[red]{ex.Message.EscapeMarkup()}[/]"))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Red),
            Header = new PanelHeader("[bold red]Error Details[/]", Justify.Left)
        };

        console.Write(exceptionPanel);
        console.WriteLine();

        if (options.ShowStackTrace && ex.StackTrace != null)
        {
            var stackTracePanel = new Panel(new Markup($"[grey]{ex.StackTrace.EscapeMarkup()}[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[grey]Stack Trace[/]", Justify.Left)
            };

            console.Write(stackTracePanel);
            console.WriteLine();
        }
    }
}

/// <summary>
/// Options for configuring argument tracing.
/// </summary>
public class ArgumentTraceOptions
{
    /// <summary>
    /// Gets or sets the console to use for output.
    /// </summary>
    public IAnsiConsole? Console { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to always show trace information,
    /// regardless of the --debug-args flag.
    /// </summary>
    public bool AlwaysShowTrace { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show trace information when an error occurs.
    /// </summary>
    public bool ShowTraceOnError { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show detailed information in the trace output.
    /// </summary>
    public bool ShowDetails { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show a source summary in the trace output.
    /// </summary>
    public bool ShowSummary { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable exception handling with tracing.
    /// </summary>
    public bool EnableExceptionHandling { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the stack trace in exception output.
    /// </summary>
    public bool ShowStackTrace { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable environment variable value providers.
    /// </summary>
    public bool EnableEnvironmentVariables { get; set; } = true;

    /// <summary>
    /// Gets or sets the prefix for environment variable names.
    /// </summary>
    public string? EnvironmentVariablePrefix { get; set; }

    /// <summary>
    /// Gets the mappings from argument names to environment variable names.
    /// </summary>
    public Dictionary<string, string> EnvironmentVariableMappings { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to enable JSON configuration file value providers.
    /// </summary>
    public bool EnableJsonConfig { get; set; }

    /// <summary>
    /// Gets or sets the path to the JSON configuration file.
    /// </summary>
    public string? JsonConfigPath { get; set; }

    /// <summary>
    /// Gets the mappings from argument names to JSON configuration paths.
    /// </summary>
    public Dictionary<string, string> JsonConfigMappings { get; } = new();

    /// <summary>
    /// Gets the list of additional value providers to use.
    /// </summary>
    public List<IArgumentValueProvider> AdditionalValueProviders { get; } = new();

    /// <summary>
    /// Maps an argument name to an environment variable name.
    /// </summary>
    /// <param name="argumentName">The argument name.</param>
    /// <param name="envVarName">The environment variable name.</param>
    /// <returns>The same <see cref="ArgumentTraceOptions"/> instance so that multiple calls can be chained.</returns>
    public ArgumentTraceOptions MapEnvironmentVariable(string argumentName, string envVarName)
    {
        EnvironmentVariableMappings[argumentName] = envVarName;
        return this;
    }

    /// <summary>
    /// Maps an argument name to a JSON configuration path.
    /// </summary>
    /// <param name="argumentName">The argument name.</param>
    /// <param name="configPath">The JSON configuration path.</param>
    /// <returns>The same <see cref="ArgumentTraceOptions"/> instance so that multiple calls can be chained.</returns>
    public ArgumentTraceOptions MapJsonConfig(string argumentName, string configPath)
    {
        JsonConfigMappings[argumentName] = configPath;
        return this;
    }

    /// <summary>
    /// Adds a custom value provider.
    /// </summary>
    /// <param name="provider">The value provider to add.</param>
    /// <returns>The same <see cref="ArgumentTraceOptions"/> instance so that multiple calls can be chained.</returns>
    public ArgumentTraceOptions AddValueProvider(IArgumentValueProvider provider)
    {
        AdditionalValueProviders.Add(provider);
        return this;
    }
}

/// <summary>
/// Represents a configurator for command applications.
/// </summary>
/// <remarks>
/// This is a placeholder interface to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, use Spectre.Console.Cli.IConfigurator from the Spectre.Console.Cli NuGet package.
/// </remarks>
public interface IConfigurator
{
    /// <summary>
    /// Sets the command interceptor.
    /// </summary>
    /// <param name="interceptor">The interceptor to use.</param>
    /// <returns>The same <see cref="IConfigurator"/> instance so that multiple calls can be chained.</returns>
    IConfigurator SetInterceptor(ICommandInterceptor interceptor);

    /// <summary>
    /// Sets the exception handler.
    /// </summary>
    /// <param name="exceptionHandler">The exception handler to use.</param>
    /// <returns>The same <see cref="IConfigurator"/> instance so that multiple calls can be chained.</returns>
    IConfigurator SetExceptionHandler(Func<Exception, ITypeResolver?, int> exceptionHandler);

    /// <summary>
    /// Propagates exceptions instead of handling them.
    /// </summary>
    /// <returns>The same <see cref="IConfigurator"/> instance so that multiple calls can be chained.</returns>
    IConfigurator PropagateExceptions();

    /// <summary>
    /// Validates examples.
    /// </summary>
    /// <returns>The same <see cref="IConfigurator"/> instance so that multiple calls can be chained.</returns>
    IConfigurator ValidateExamples();
}

/// <summary>
/// Represents a type resolver.
/// </summary>
/// <remarks>
/// This is a placeholder interface to allow compilation without the full Spectre.Console.Cli package.
/// In actual usage, use Spectre.Console.Cli.ITypeResolver from the Spectre.Console.Cli NuGet package.
/// </remarks>
public interface ITypeResolver
{
    /// <summary>
    /// Resolves an instance of the specified type.
    /// </summary>
    /// <param name="type">The type to resolve.</param>
    /// <returns>An instance of the specified type, or null if it could not be resolved.</returns>
    object? Resolve(Type type);
}
