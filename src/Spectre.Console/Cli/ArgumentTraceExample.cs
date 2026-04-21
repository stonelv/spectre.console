namespace Spectre.Console.Cli;

/// <summary>
/// Provides example usage of the argument tracing functionality.
/// </summary>
public static class ArgumentTraceExample
{
    /// <summary>
    /// Demonstrates how to create and display an argument trace context.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    public static void RunBasicExample(IAnsiConsole console)
    {
        var context = new ArgumentTraceContext("build", "Builds the project");

        context.AddOption(
            name: "--configuration",
            aliases: new[] { "--configuration", "-c" },
            value: "Release",
            source: ArgumentSource.CommandLine,
            description: "The configuration to build",
            isRequired: false,
            defaultValue: "Debug",
            sourceDetails: "Command line argument: -c Release");

        context.AddOption(
            name: "--verbose",
            aliases: new[] { "--verbose", "-v" },
            value: true,
            source: ArgumentSource.EnvironmentVariable,
            description: "Enable verbose output",
            isRequired: false,
            defaultValue: false,
            sourceDetails: "Environment variable: BUILD_VERBOSE=true");

        context.AddOption(
            name: "--output",
            aliases: new[] { "--output", "-o" },
            value: "./bin",
            source: ArgumentSource.ConfigurationFile,
            description: "The output directory",
            isRequired: false,
            defaultValue: "./out",
            sourceDetails: "Config file: build.json -> outputPath");

        context.AddOption(
            name: "--framework",
            aliases: new[] { "--framework", "-f" },
            value: "net8.0",
            source: ArgumentSource.DefaultValue,
            description: "The target framework",
            isRequired: false,
            defaultValue: "net8.0",
            sourceDetails: null);

        context.AddArgument(
            name: "project",
            position: 0,
            value: "./src/MyApp.csproj",
            source: ArgumentSource.CommandLine,
            description: "The project file to build",
            isRequired: true,
            defaultValue: null,
            sourceDetails: "Positional argument: ./src/MyApp.csproj");

        console.WriteLine("[bold yellow]=== Basic Argument Trace Example ===[/]");
        console.WriteLine();
        console.WriteArgumentTrace(context, showDetails: true, showSummary: true);
    }

    /// <summary>
    /// Demonstrates how to display an argument trace when parsing fails.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    public static void RunParsingErrorExample(IAnsiConsole console)
    {
        var context = new ArgumentTraceContext("deploy", "Deploys the application")
        {
            ParsingError = "Missing required argument: 'environment'",
            TraceRequested = true
        };

        context.AddOption(
            name: "--dry-run",
            aliases: new[] { "--dry-run" },
            value: true,
            source: ArgumentSource.CommandLine,
            description: "Simulate deployment without making changes",
            isRequired: false,
            defaultValue: false,
            sourceDetails: "Command line flag: --dry-run");

        context.AddOption(
            name: "--timeout",
            aliases: new[] { "--timeout", "-t" },
            value: 30,
            source: ArgumentSource.DefaultValue,
            description: "Deployment timeout in seconds",
            isRequired: false,
            defaultValue: 30,
            sourceDetails: null);

        context.AddArgument(
            name: "environment",
            position: 0,
            value: null,
            source: ArgumentSource.NotProvided,
            description: "The target environment (dev/staging/prod)",
            isRequired: true,
            defaultValue: null,
            sourceDetails: null,
            validationError: "Required argument was not provided");

        context.AddArgument(
            name: "version",
            position: 1,
            value: "1.2.3",
            source: ArgumentSource.CommandLine,
            description: "The version to deploy",
            isRequired: false,
            defaultValue: "latest",
            sourceDetails: "Positional argument: 1.2.3");

        console.WriteLine("[bold red]=== Parsing Error Example ===[/]");
        console.WriteLine();
        console.WriteArgumentTrace(context, showDetails: true, showSummary: true);
    }

    /// <summary>
    /// Demonstrates how to use custom styles for argument tracing.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    public static void RunCustomStylesExample(IAnsiConsole console)
    {
        var context = new ArgumentTraceContext("config", "Manages configuration");

        context.AddOption(
            name: "--key",
            aliases: new[] { "--key", "-k" },
            value: "connectionString",
            source: ArgumentSource.CommandLine,
            description: "The configuration key");

        context.AddOption(
            name: "--value",
            aliases: new[] { "--value", "-v" },
            value: "Server=localhost;Database=app",
            source: ArgumentSource.EnvironmentVariable,
            description: "The configuration value",
            sourceDetails: "Environment variable: CONFIG_VALUE");

        var customStyles = new ArgumentTraceStyles
        {
            HeaderStyle = new Style(Color.Green),
            NameStyle = new Style(Color.Yellow),
            ValueStyle = new Style(Color.Cyan),
            BorderStyle = new Style(Color.Green)
        };

        console.WriteLine("[bold green]=== Custom Styles Example ===[/]");
        console.WriteLine();

        var panel = new ArgumentTracePanel(context, customStyles, showDetails: true, showSummary: false)
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader("[bold green]Custom Styled Argument Trace[/]", Justify.Center)
        };

        console.Write(panel);
    }

    /// <summary>
    /// Demonstrates how to use the table component directly.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    public static void RunTableExample(IAnsiConsole console)
    {
        var context = new ArgumentTraceContext("test", "Runs tests");

        context.AddOption(
            name: "--filter",
            aliases: new[] { "--filter", "-f" },
            value: "UnitTests",
            source: ArgumentSource.CommandLine,
            description: "Test filter expression");

        context.AddOption(
            name: "--collect",
            aliases: new[] { "--collect" },
            value: "XPlat Code Coverage",
            source: ArgumentSource.ConfigurationFile,
            description: "Data collectors to enable",
            sourceDetails: "runsettings.xml -> DataCollectors");

        context.AddOption(
            name: "--no-build",
            aliases: new[] { "--no-build" },
            value: false,
            source: ArgumentSource.DefaultValue,
            description: "Do not build the project before testing",
            defaultValue: false);

        context.AddArgument(
            name: "assembly",
            position: 0,
            value: "./tests/MyApp.Tests.dll",
            source: ArgumentSource.CommandLine,
            description: "The test assembly to run");

        console.WriteLine("[bold cyan]=== Table Only Example ===[/]");
        console.WriteLine();

        var table = new ArgumentTraceTable(context, showDetails: true)
        {
            Border = TableBorder.Markdown,
            Title = new TableTitle("[bold cyan]Argument Trace Table[/]")
        };

        console.Write(table);
    }

    /// <summary>
    /// Demonstrates how to manually record argument sources at assignment time.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    public static void RunManualRecordingExample(IAnsiConsole console)
    {
        console.WriteLine(@"
=== Manual Source Recording Example ===

This example shows how to manually record argument sources at assignment time,
rather than guessing them later. This is the recommended approach because it:

1. Provides accurate source information
2. Doesn't rely on reflection or heuristics
3. Works with any CLI library

Example workflow:

// 1. Create a trace context
var context = new ArgumentTraceContext(""mycommand"");

// 2. Try to get values from different sources
string? config = null;
ArgumentSource configSource = ArgumentSource.NotProvided;
string? configSourceDetails = null;

// Check command line first (highest priority)
if (args.Contains(""--configuration"") || args.Contains(""-c""))
{
    config = GetValueFromArgs(args, ""--configuration"", ""-c"");
    configSource = ArgumentSource.CommandLine;
    configSourceDetails = $""Command line: {config}"";
}
// Then check environment variables
else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(""MYAPP_CONFIG"")))
{
    config = Environment.GetEnvironmentVariable(""MYAPP_CONFIG"");
    configSource = ArgumentSource.EnvironmentVariable;
    configSourceDetails = ""Environment variable: MYAPP_CONFIG"";
}
// Then check config file
else if (ConfigFile.HasValue(""build.configuration""))
{
    config = ConfigFile.GetValue(""build.configuration"");
    configSource = ArgumentSource.ConfigurationFile;
    configSourceDetails = ""Config file: appsettings.json -> build.configuration"";
}
// Finally use default
else
{
    config = ""Debug"";
    configSource = ArgumentSource.DefaultValue;
}

// 3. Record the source information
context.AddOption(
    name: ""--configuration"",
    aliases: new[] { ""--configuration"", ""-c"" },
    value: config,
    source: configSource,
    description: ""The configuration to use"",
    defaultValue: ""Debug"",
    sourceDetails: configSourceDetails);

// 4. Display the trace
console.WriteArgumentTrace(context);
");
    }

    /// <summary>
    /// Demonstrates how to use value providers for automated source detection.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    public static void RunValueProviderExample(IAnsiConsole console)
    {
        console.WriteLine(@"
=== Value Provider Example ===

Value providers can be used to automate source detection.
Note: EnvironmentVariableValueProvider and JsonConfigValueProvider are
provided as optional utilities, but you can also implement your own.

// Create providers
var envProvider = new EnvironmentVariableValueProvider(""MYAPP_"");
envProvider.Map(""configuration"", ""MYAPP_CONFIG"");
envProvider.Map(""output"", ""MYAPP_OUTPUT"");

var jsonProvider = new JsonConfigValueProvider(""appsettings.json"");
jsonProvider.Map(""configuration"", ""build.configuration"");
jsonProvider.Map(""output"", ""build.output"");

// Try to get values in priority order
var providers = new IArgumentValueProvider[] 
{ 
    envProvider,    // Lower priority = checked later
    jsonProvider    // Higher priority = checked first
}.OrderByDescending(p => p.Priority);

var context = new ArgumentTraceContext(""build"");

// Check each provider
foreach (var provider in providers)
{
    if (provider.TryGetValue(""configuration"", null, out var configValue))
    {
        context.AddOption(
            name: ""--configuration"",
            value: configValue,
            source: provider.Source,
            sourceDetails: provider.GetSourceDetails(""configuration""));
        break;
    }
}

// Use default if no provider had a value
if (!context.GetOptions().Any(o => o.Name == ""--configuration""))
{
    context.AddOption(
        name: ""--configuration"",
        value: ""Debug"",
        source: ArgumentSource.DefaultValue);
}
");
    }

    /// <summary>
    /// Demonstrates how to implement a custom value provider.
    /// </summary>
    public static void RunCustomValueProviderExample()
    {
        System.Console.WriteLine(@"
=== Custom Value Provider Example ===

Implement IArgumentValueProvider to support custom value sources:

public class IniConfigValueProvider : IArgumentValueProvider
{
    private readonly string _filePath;
    private readonly Dictionary<string, string> _mappings;

    public string Name => ""IniConfig"";
    public int Priority { get; set; } = 150;
    public ArgumentSource Source => ArgumentSource.ConfigurationFile;

    public IniConfigValueProvider(string filePath)
    {
        _filePath = filePath;
        _mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public IniConfigValueProvider Map(string argumentName, string iniPath)
    {
        _mappings[argumentName] = iniPath;
        return this;
    }

    public bool TryGetValue(string argumentName, IEnumerable<string>? aliases, out object? value)
    {
        value = null;
        
        if (_mappings.TryGetValue(argumentName, out var iniPath))
        {
            // Read from INI file...
            // value = ReadFromIniFile(iniPath);
            return false; // Replace with actual implementation
        }
        
        return false;
    }

    public string? GetSourceDetails(string argumentName)
    {
        if (_mappings.TryGetValue(argumentName, out var iniPath))
        {
            return $""Config file: {_filePath} -> {iniPath}"";
        }
        return null;
    }
}
");
    }
}
