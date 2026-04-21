using System.ComponentModel;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Spectre.Console.Tests.Unit.Cli;

[ExpectationPath("Cli/ArgumentTrace")]
public sealed class ArgumentTraceRenderTests
{
    [Fact]
    [Expectation("BasicTable")]
    public Task Should_Render_Basic_Argument_Trace_Table()
    {
        var console = new TestConsole().Width(100);
        var context = CreateBasicTraceContext();

        var table = context.ToTraceTable(showDetails: false);
        console.Write(table);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("DetailedTable")]
    public Task Should_Render_Detailed_Argument_Trace_Table()
    {
        var console = new TestConsole().Width(120);
        var context = CreateBasicTraceContext();

        var table = context.ToTraceTable(showDetails: true);
        console.Write(table);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("BasicPanel")]
    public Task Should_Render_Basic_Argument_Trace_Panel()
    {
        var console = new TestConsole().Width(100);
        var context = CreateBasicTraceContext();

        var panel = context.ToTracePanel(showDetails: false, showSummary: true);
        console.Write(panel);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("DetailedPanel")]
    public Task Should_Render_Detailed_Argument_Trace_Panel()
    {
        var console = new TestConsole().Width(120);
        var context = CreateBasicTraceContext();

        var panel = context.ToTracePanel(showDetails: true, showSummary: true);
        console.Write(panel);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("WithErrors")]
    public Task Should_Render_Argument_Trace_With_Errors()
    {
        var console = new TestConsole().Width(100);
        var context = CreateTraceContextWithErrors();

        var panel = context.ToTracePanel(showDetails: true, showSummary: true);
        console.Write(panel);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("MixedSources")]
    public Task Should_Render_Argument_Trace_With_Mixed_Sources()
    {
        var console = new TestConsole().Width(100);
        var context = CreateTraceContextWithMixedSources();

        var panel = context.ToTracePanel(showDetails: true, showSummary: true);
        console.Write(panel);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("EmptyContext")]
    public Task Should_Render_Empty_Argument_Trace_Context()
    {
        var console = new TestConsole().Width(80);
        var context = new ArgumentTraceContext("empty-command", "A command with no arguments");

        var panel = context.ToTracePanel(showDetails: false, showSummary: false);
        console.Write(panel);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("WriteExtensionMethod")]
    public Task Should_Use_WriteArgumentTrace_Extension_Method()
    {
        var console = new TestConsole().Width(100);
        var context = CreateBasicTraceContext();

        console.WriteArgumentTrace(context, showDetails: true, showSummary: true);

        return Verifier.Verify(console.Output);
    }

    #region Test Helpers

    private static ArgumentTraceContext CreateBasicTraceContext()
    {
        var context = new ArgumentTraceContext("build", "Builds the project");

        context.AddOption(
            name: "--configuration",
            aliases: new[] { "--configuration", "-c" },
            value: "Release",
            source: ArgumentSource.CommandLine,
            description: "The configuration to build (Debug/Release)",
            isRequired: false,
            defaultValue: "Debug",
            sourceDetails: "Command line: -c Release");

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
            sourceDetails: "Positional argument");

        return context;
    }

    private static ArgumentTraceContext CreateTraceContextWithErrors()
    {
        var context = new ArgumentTraceContext("deploy", "Deploys the application")
        {
            ParsingError = "Missing required argument: 'environment'"
        };

        context.AddOption(
            name: "--dry-run",
            aliases: new[] { "--dry-run" },
            value: true,
            source: ArgumentSource.CommandLine,
            description: "Simulate deployment without making changes");

        context.AddOption(
            name: "--timeout",
            aliases: new[] { "--timeout", "-t" },
            value: 30,
            source: ArgumentSource.DefaultValue,
            description: "Deployment timeout in seconds",
            defaultValue: 30);

        context.AddArgument(
            name: "environment",
            position: 0,
            value: null,
            source: ArgumentSource.NotProvided,
            description: "The target environment (dev/staging/prod)",
            isRequired: true,
            validationError: "Required argument was not provided");

        context.AddArgument(
            name: "version",
            position: 1,
            value: "1.2.3",
            source: ArgumentSource.CommandLine,
            description: "The version to deploy",
            isRequired: false,
            defaultValue: "latest");

        return context;
    }

    private static ArgumentTraceContext CreateTraceContextWithMixedSources()
    {
        var context = new ArgumentTraceContext("run", "Runs the application");

        context.AddOption(
            name: "--url",
            aliases: new[] { "--url", "-u" },
            value: "http://localhost:8080",
            source: ArgumentSource.CommandLine,
            description: "The URL to listen on",
            defaultValue: "http://localhost:5000",
            sourceDetails: "Command line: --url http://localhost:8080");

        context.AddOption(
            name: "--environment",
            aliases: new[] { "--environment", "-e" },
            value: "Development",
            source: ArgumentSource.EnvironmentVariable,
            description: "The hosting environment",
            defaultValue: "Production",
            sourceDetails: "Environment variable: ASPNETCORE_ENVIRONMENT=Development");

        context.AddOption(
            name: "--log-level",
            aliases: new[] { "--log-level", "-l" },
            value: "Information",
            source: ArgumentSource.ConfigurationFile,
            description: "The minimum log level",
            defaultValue: "Warning",
            sourceDetails: "Config file: appsettings.json -> Logging:LogLevel:Default");

        context.AddOption(
            name: "--no-https",
            aliases: new[] { "--no-https" },
            value: false,
            source: ArgumentSource.DefaultValue,
            description: "Disable HTTPS redirection",
            defaultValue: false);

        context.AddArgument(
            name: "assembly",
            position: 0,
            value: "./MyApp.dll",
            source: ArgumentSource.CommandLine,
            description: "The assembly to run",
            isRequired: true);

        context.AddArgument(
            name: "args",
            position: 1,
            value: new[] { "--extra", "value" },
            source: ArgumentSource.NotProvided,
            description: "Extra arguments to pass to the application",
            isRequired: false);

        return context;
    }

    #endregion
}
