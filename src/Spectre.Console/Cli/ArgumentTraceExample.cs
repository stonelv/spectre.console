using System.ComponentModel;

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
    /// Demonstrates how to configure argument tracing with the CommandApp.
    /// </summary>
    /// <remarks>
    /// This example shows the code you would write in your Program.cs to enable argument tracing.
    /// Note: This requires the actual Spectre.Console.Cli NuGet package to compile and run.
    /// </remarks>
    public static void RunIntegrationExample()
    {
        System.Console.WriteLine(@"
=== Integration Example (Program.cs) ===

// In your Program.cs, you would write:

using Spectre.Console.Cli;

namespace MyApp;

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp();
        
        app.Configure(config =>
        {
            // Enable argument tracing
            config.UseArgumentTracing(options =>
            {
                // Enable environment variable support with prefix
                options.EnableEnvironmentVariables = true;
                options.EnvironmentVariablePrefix = ""MYAPP_"";
                
                // Map specific arguments to environment variables
                options.MapEnvironmentVariable(""configuration"", ""MYAPP_CONFIG"");
                options.MapEnvironmentVariable(""verbose"", ""MYAPP_VERBOSE"");
                
                // Enable JSON config file support
                options.EnableJsonConfig = true;
                options.JsonConfigPath = ""appsettings.json"";
                
                // Map specific arguments to config paths
                options.MapJsonConfig(""output"", ""build.outputPath"");
                options.MapJsonConfig(""logLevel"", ""logging.logLevel"");
                
                // Show trace on error (default: true)
                options.ShowTraceOnError = true;
                
                // Always show trace (useful for debugging)
                // options.AlwaysShowTrace = true;
                
                // Show detailed information
                options.ShowDetails = true;
                options.ShowSummary = true;
                
                // Add custom value providers
                // options.AddValueProvider(new MyCustomValueProvider());
            });
            
            // Register your commands
            config.AddCommand<BuildCommand>(""build"");
            config.AddCommand<DeployCommand>(""deploy"");
        });
        
        return app.Run(args);
    }
}

=== Command Settings Example ===

public class BuildSettings : CommandSettings
{
    [CommandOption(""--configuration|-c <CONFIGURATION>"")]
    [Description(""The configuration to build (Debug/Release)"")]
    [DefaultValue(""Debug"")]
    public string? Configuration { get; set; }
    
    [CommandOption(""--verbose|-v"")]
    [Description(""Enable verbose output"")]
    public bool Verbose { get; set; }
    
    [CommandOption(""--output|-o <OUTPUT>"")]
    [Description(""The output directory"")]
    public string? Output { get; set; }
    
    [CommandArgument(0, ""[PROJECT]"")]
    [Description(""The project file to build"")]
    public string? Project { get; set; }
}

=== Usage Examples ===

1. Show argument trace with --debug-args flag:
   myapp build --configuration Release --debug-args ./src/MyApp.csproj

2. Show argument trace on parsing error:
   myapp deploy --dry-run
   (Missing required argument 'environment' → shows trace)

3. Environment variable fallback:
   export MYAPP_CONFIG=Release
   export MYAPP_VERBOSE=true
   myapp build ./src/MyApp.csproj
   (Configuration and Verbose will show as coming from environment variables)

4. JSON config file (appsettings.json):
   {
     ""build"": {
       ""outputPath"": ""./dist""
     },
     ""logging"": {
       ""logLevel"": ""Information""
     }
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

// Implement IArgumentValueProvider to support custom value sources

public class IniConfigValueProvider : IArgumentValueProvider
{
    private readonly string _filePath;
    private readonly Dictionary<string, string> _mappings;
    private Dictionary<string, string>? _configCache;

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
        EnsureLoaded();
        
        if (_mappings.TryGetValue(argumentName, out var iniPath))
        {
            if (_configCache?.TryGetValue(iniPath, out var iniValue) == true)
            {
                value = iniValue;
                return true;
            }
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

    private void EnsureLoaded()
    {
        if (_configCache != null) return;
        
        // Parse INI file and populate _configCache
        // Example iniPath format: ""section.key""
    }
}

// Usage in Program.cs:
config.UseArgumentTracing(options =>
{
    options.AddValueProvider(new IniConfigValueProvider(""config.ini"")
        .Map(""configuration"", ""build.config"")
        .Map(""output"", ""build.output""));
});
");
    }

    /// <summary>
    /// Demonstrates how to use the ArgumentTraceInterceptor directly.
    /// </summary>
    public static void RunInterceptorExample()
    {
        System.Console.WriteLine(@"
=== Interceptor Direct Usage Example ===

// You can also use the interceptor directly for more control

public static int Main(string[] args)
{
    var app = new CommandApp();
    
    app.Configure(config =>
    {
        // Create and configure the interceptor manually
        var interceptor = new ArgumentTraceInterceptor(
            console: AnsiConsole.Console,
            alwaysShowTrace: false,
            showDetails: true,
            showSummary: true);
        
        // Add value providers
        interceptor.AddValueProvider(new EnvironmentVariableValueProvider(""MYAPP_""));
        interceptor.AddValueProvider(new JsonConfigValueProvider(""appsettings.json""));
        
        // Set the interceptor
        config.SetInterceptor(interceptor);
        
        // Also set up exception handling if needed
        config.SetExceptionHandler((ex, resolver) =>
        {
            // You can access the interceptor's TraceContext here if needed
            AnsiConsole.WriteException(ex);
            return -1;
        });
        
        // Register commands
        config.AddCommand<BuildCommand>(""build"");
    });
    
    return app.Run(args);
}
");
    }
}

/// <summary>
/// Example settings class for demonstration purposes.
/// </summary>
internal class ExampleBuildSettings : CommandSettings
{
    [CommandOption("--configuration|-c <CONFIGURATION>")]
    [Description("The configuration to build (Debug/Release)")]
    [DefaultValue("Debug")]
    public string? Configuration { get; set; }

    [CommandOption("--verbose|-v")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }

    [CommandOption("--output|-o <OUTPUT>")]
    [Description("The output directory")]
    [DefaultValue("./bin")]
    public string? Output { get; set; }

    [CommandOption("--framework|-f <FRAMEWORK>")]
    [Description("The target framework")]
    public string? Framework { get; set; }

    [CommandArgument(0, "[PROJECT]")]
    [Description("The project file to build")]
    public string? Project { get; set; }
}

