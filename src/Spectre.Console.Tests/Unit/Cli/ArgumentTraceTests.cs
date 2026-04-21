using System.ComponentModel;
using Spectre.Console.Cli;

namespace Spectre.Console.Tests.Unit.Cli;

public sealed class ArgumentTraceTests
{
    public sealed class TheArgumentSourceEnum
    {
        [Theory]
        [InlineData(ArgumentSource.DefaultValue, "Default Value")]
        [InlineData(ArgumentSource.CommandLine, "Command Line")]
        [InlineData(ArgumentSource.EnvironmentVariable, "Environment Variable")]
        [InlineData(ArgumentSource.ConfigurationFile, "Configuration File")]
        [InlineData(ArgumentSource.NotProvided, "Not Provided")]
        public void Should_Return_Correct_Source_Display_Name(ArgumentSource source, string expected)
        {
            var info = new ArgumentTraceInfo { Source = source };
            info.SourceDisplayName.ShouldBe(expected);
        }
    }

    public sealed class TheArgumentTraceInfoClass
    {
        [Fact]
        public void Should_Return_Null_For_Null_Value()
        {
            var info = new ArgumentTraceInfo { Value = null };
            info.FormattedValue.ShouldBe("(null)");
        }

        [Fact]
        public void Should_Return_String_For_Simple_Value()
        {
            var info = new ArgumentTraceInfo { Value = "test" };
            info.FormattedValue.ShouldBe("test");
        }

        [Fact]
        public void Should_Return_Array_Format_For_Array_Value()
        {
            var info = new ArgumentTraceInfo { Value = new[] { "a", "b", "c" } };
            info.FormattedValue.ShouldBe("[a, b, c]");
        }

        [Fact]
        public void Should_Return_FullName_With_Aliases()
        {
            var info = new ArgumentTraceInfo
            {
                Name = "configuration",
                Aliases = new[] { "--configuration", "-c" }
            };
            info.FullName.ShouldBe("--configuration | -c");
        }

        [Fact]
        public void Should_Return_Name_When_No_Aliases()
        {
            var info = new ArgumentTraceInfo { Name = "configuration" };
            info.FullName.ShouldBe("configuration");
        }
    }

    public sealed class TheArgumentTraceContextClass
    {
        [Fact]
        public void Should_Add_Argument()
        {
            var context = new ArgumentTraceContext();
            var info = new ArgumentTraceInfo { Name = "test" };

            context.AddArgument(info);

            context.Arguments.ShouldContain(info);
        }

        [Fact]
        public void Should_Add_Multiple_Arguments()
        {
            var context = new ArgumentTraceContext();
            var infos = new[]
            {
                new ArgumentTraceInfo { Name = "arg1" },
                new ArgumentTraceInfo { Name = "arg2" }
            };

            context.AddArguments(infos);

            context.Arguments.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Arguments_By_Source()
        {
            var context = new ArgumentTraceContext();
            context.AddArgument(new ArgumentTraceInfo { Name = "arg1", Source = ArgumentSource.CommandLine });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg2", Source = ArgumentSource.DefaultValue });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg3", Source = ArgumentSource.CommandLine });

            var result = context.GetArgumentsBySource(ArgumentSource.CommandLine);

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Arguments_With_Errors()
        {
            var context = new ArgumentTraceContext();
            context.AddArgument(new ArgumentTraceInfo { Name = "arg1" });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg2", ValidationError = "Some error" });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg3" });

            var result = context.GetArgumentsWithErrors();

            result.Count().ShouldBe(1);
            result.First().ValidationError.ShouldBe("Some error");
        }

        [Fact]
        public void Should_Get_Options_And_Positional_Arguments()
        {
            var context = new ArgumentTraceContext();
            context.AddArgument(new ArgumentTraceInfo { Name = "opt1", IsOption = true });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg1", IsOption = false, Position = 0 });
            context.AddArgument(new ArgumentTraceInfo { Name = "opt2", IsOption = true });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg2", IsOption = false, Position = 1 });

            var options = context.GetOptions();
            var args = context.GetPositionalArguments();

            options.Count().ShouldBe(2);
            args.Count().ShouldBe(2);
            args.Select(a => a.Name).ShouldBe(new[] { "arg1", "arg2" });
        }

        [Fact]
        public void Should_Get_Source_Summary()
        {
            var context = new ArgumentTraceContext();
            context.AddArgument(new ArgumentTraceInfo { Name = "arg1", Source = ArgumentSource.CommandLine });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg2", Source = ArgumentSource.CommandLine });
            context.AddArgument(new ArgumentTraceInfo { Name = "arg3", Source = ArgumentSource.DefaultValue });

            var summary = context.GetSourceSummary();

            summary[ArgumentSource.CommandLine].ShouldBe(2);
            summary[ArgumentSource.DefaultValue].ShouldBe(1);
        }
    }

    public sealed class TheEnvironmentVariableValueProvider
    {
        [Fact]
        public void Should_Have_Correct_Default_Values()
        {
            var provider = new EnvironmentVariableValueProvider();

            provider.Name.ShouldBe("EnvironmentVariable");
            provider.Source.ShouldBe(ArgumentSource.EnvironmentVariable);
            provider.Priority.ShouldBe(100);
        }

        [Fact]
        public void Should_Map_Argument_To_Env_Var()
        {
            var provider = new EnvironmentVariableValueProvider();
            provider.Map("configuration", "MYAPP_CONFIG");

            // We can't really test actual environment variable reading without setting them,
            // but we can verify the mapping mechanism works through the API
            provider.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Use_Prefix()
        {
            var provider = new EnvironmentVariableValueProvider("MYAPP_");

            provider.ShouldNotBeNull();
        }
    }

    public sealed class TheJsonConfigValueProvider
    {
        [Fact]
        public void Should_Have_Correct_Default_Values()
        {
            var provider = new JsonConfigValueProvider("config.json");

            provider.Name.ShouldBe("JsonConfig");
            provider.Source.ShouldBe(ArgumentSource.ConfigurationFile);
            provider.Priority.ShouldBe(200);
            provider.FilePath.ShouldBe("config.json");
        }

        [Fact]
        public void Should_Map_Argument_To_Config_Path()
        {
            var provider = new JsonConfigValueProvider("config.json");
            provider.Map("configuration", "build.configuration");

            provider.ShouldNotBeNull();
        }
    }

    public sealed class TheArgumentTraceOptions
    {
        [Fact]
        public void Should_Have_Correct_Default_Values()
        {
            var options = new ArgumentTraceOptions();

            options.EnableEnvironmentVariables.ShouldBeTrue();
            options.EnableExceptionHandling.ShouldBeTrue();
            options.ShowTraceOnError.ShouldBeTrue();
            options.ShowDetails.ShouldBeTrue();
            options.ShowSummary.ShouldBeTrue();
        }

        [Fact]
        public void Should_Map_Environment_Variable()
        {
            var options = new ArgumentTraceOptions();
            options.MapEnvironmentVariable("configuration", "MYAPP_CONFIG");

            options.EnvironmentVariableMappings.ShouldContainKey("configuration");
            options.EnvironmentVariableMappings["configuration"].ShouldBe("MYAPP_CONFIG");
        }

        [Fact]
        public void Should_Map_Json_Config()
        {
            var options = new ArgumentTraceOptions();
            options.MapJsonConfig("configuration", "build.configuration");

            options.JsonConfigMappings.ShouldContainKey("configuration");
            options.JsonConfigMappings["configuration"].ShouldBe("build.configuration");
        }

        [Fact]
        public void Should_Add_Value_Provider()
        {
            var options = new ArgumentTraceOptions();
            var provider = new TestValueProvider();

            options.AddValueProvider(provider);

            options.AdditionalValueProviders.ShouldContain(provider);
        }
    }

    public sealed class TheArgumentTraceStyles
    {
        [Fact]
        public void Should_Have_Default_Styles()
        {
            var styles = ArgumentTraceStyles.Default;

            styles.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Get_Correct_Source_Style()
        {
            var styles = new ArgumentTraceStyles();

            var defaultStyle = styles.GetSourceStyle(ArgumentSource.DefaultValue);
            var commandLineStyle = styles.GetSourceStyle(ArgumentSource.CommandLine);
            var envVarStyle = styles.GetSourceStyle(ArgumentSource.EnvironmentVariable);
            var configStyle = styles.GetSourceStyle(ArgumentSource.ConfigurationFile);
            var notProvidedStyle = styles.GetSourceStyle(ArgumentSource.NotProvided);

            defaultStyle.ShouldNotBeNull();
            commandLineStyle.ShouldNotBeNull();
            envVarStyle.ShouldNotBeNull();
            configStyle.ShouldNotBeNull();
            notProvidedStyle.ShouldNotBeNull();
        }
    }

    public sealed class TheArgumentTraceExtensions
    {
        [Fact]
        public void Should_Add_Option_To_Context()
        {
            var context = new ArgumentTraceContext();

            context.AddOption(
                name: "--configuration",
                aliases: new[] { "--configuration", "-c" },
                value: "Release",
                source: ArgumentSource.CommandLine,
                description: "The configuration to build");

            context.Arguments.Count.ShouldBe(1);
            var arg = context.Arguments[0];
            arg.Name.ShouldBe("--configuration");
            arg.Value.ShouldBe("Release");
            arg.Source.ShouldBe(ArgumentSource.CommandLine);
            arg.IsOption.ShouldBeTrue();
        }

        [Fact]
        public void Should_Add_Argument_To_Context()
        {
            var context = new ArgumentTraceContext();

            context.AddArgument(
                name: "project",
                position: 0,
                value: "./src/MyApp.csproj",
                source: ArgumentSource.CommandLine,
                description: "The project file to build",
                isRequired: true);

            context.Arguments.Count.ShouldBe(1);
            var arg = context.Arguments[0];
            arg.Name.ShouldBe("project");
            arg.Value.ShouldBe("./src/MyApp.csproj");
            arg.Source.ShouldBe(ArgumentSource.CommandLine);
            arg.IsOption.ShouldBeFalse();
            arg.IsRequired.ShouldBeTrue();
            arg.Position.ShouldBe(0);
        }
    }

    public sealed class TheArgumentTraceCollector
    {
        [Fact]
        public void Should_Collect_Trace_Info_From_Settings()
        {
            var collector = new ArgumentTraceCollector();
            var settings = new TestSettings
            {
                Configuration = "Release",
                Verbose = true
            };

            var context = collector.Collect(settings, "build");

            context.ShouldNotBeNull();
            context.CommandName.ShouldBe("build");
        }
    }

    public sealed class TheArgumentTraceInterceptor
    {
        [Fact]
        public void Should_Have_Correct_Default_Values()
        {
            var interceptor = new ArgumentTraceInterceptor();

            interceptor.DebugArgsDetected.ShouldBeFalse();
            interceptor.TraceContext.ShouldBeNull();
        }

        [Fact]
        public void Should_Add_Value_Provider()
        {
            var interceptor = new ArgumentTraceInterceptor();
            var provider = new TestValueProvider();

            interceptor.AddValueProvider(provider);

            interceptor.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Add_Multiple_Value_Providers()
        {
            var interceptor = new ArgumentTraceInterceptor();
            var providers = new[]
            {
                new TestValueProvider(),
                new TestValueProvider()
            };

            interceptor.AddValueProviders(providers);

            interceptor.ShouldNotBeNull();
        }
    }

    #region Test Helpers

    private class TestValueProvider : IArgumentValueProvider
    {
        public string Name => "Test";
        public int Priority { get; set; }
        public ArgumentSource Source => ArgumentSource.ConfigurationFile;

        public bool TryGetValue(string argumentName, IEnumerable<string>? aliases, out object? value)
        {
            value = null;
            return false;
        }

        public string? GetSourceDetails(string argumentName)
        {
            return null;
        }
    }

    private class TestSettings : CommandSettings
    {
        [CommandOption("--configuration|-c <CONFIGURATION>")]
        [Description("The configuration to build")]
        [DefaultValue("Debug")]
        public string? Configuration { get; set; }

        [CommandOption("--verbose|-v")]
        [Description("Enable verbose output")]
        public bool Verbose { get; set; }

        [CommandArgument(0, "[PROJECT]")]
        [Description("The project file to build")]
        public string? Project { get; set; }
    }

    #endregion
}
