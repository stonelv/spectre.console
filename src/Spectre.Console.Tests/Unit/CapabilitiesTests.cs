namespace Spectre.Console.Tests.Unit;

public sealed class CapabilitiesTests
{
    [Fact]
    public void Should_Have_Default_Values()
    {
        // Given
        var output = new AnsiConsoleOutput(new StringWriter());

        // When
        var capabilities = new Capabilities(output);

        // Then
        capabilities.ColorSystem.ShouldBe(default(ColorSystem));
        capabilities.Ansi.ShouldBeFalse();
        capabilities.Links.ShouldBeFalse();
        capabilities.Legacy.ShouldBeFalse();
        capabilities.Interactive.ShouldBeFalse();
        capabilities.Unicode.ShouldBeFalse();
        capabilities.AlternateBuffer.ShouldBeFalse();
        capabilities.Emoji.ShouldBeFalse();
    }

    [Fact]
    public void IReadOnlyCapabilities_Should_Expose_All_Properties()
    {
        // Given
        var output = new AnsiConsoleOutput(new StringWriter());
        var capabilities = new Capabilities(output)
        {
            ColorSystem = ColorSystem.TrueColor,
            Ansi = true,
            Links = true,
            Legacy = false,
            Interactive = true,
            Unicode = true,
            AlternateBuffer = true,
            Emoji = true
        };

        // When
        IReadOnlyCapabilities readOnly = capabilities;

        // Then
        readOnly.ColorSystem.ShouldBe(ColorSystem.TrueColor);
        readOnly.Ansi.ShouldBeTrue();
        readOnly.Links.ShouldBeTrue();
        readOnly.Legacy.ShouldBeFalse();
        readOnly.Interactive.ShouldBeTrue();
        readOnly.Unicode.ShouldBeTrue();
        readOnly.AlternateBuffer.ShouldBeTrue();
        readOnly.Emoji.ShouldBeTrue();
    }

    [Fact]
    public void TestCapabilities_Should_Implement_IReadOnlyCapabilities()
    {
        // Given
        var capabilities = new TestCapabilities
        {
            ColorSystem = ColorSystem.EightBit,
            Ansi = true,
            Links = true,
            Legacy = false,
            IsTerminal = true,
            Interactive = true,
            Unicode = true,
            AlternateBuffer = true,
            Emoji = true
        };

        // When
        IReadOnlyCapabilities readOnly = capabilities;

        // Then
        readOnly.ColorSystem.ShouldBe(ColorSystem.EightBit);
        readOnly.Ansi.ShouldBeTrue();
        readOnly.Links.ShouldBeTrue();
        readOnly.Legacy.ShouldBeFalse();
#pragma warning disable CS0618 // 类型或成员已过时
        readOnly.IsTerminal.ShouldBeTrue();
#pragma warning restore CS0618 // 类型或成员已过时
        readOnly.Interactive.ShouldBeTrue();
        readOnly.Unicode.ShouldBeTrue();
        readOnly.AlternateBuffer.ShouldBeTrue();
        readOnly.Emoji.ShouldBeTrue();
    }

    [Theory]
    [InlineData(UnicodeSupport.Detect, 0)]
    [InlineData(UnicodeSupport.Yes, 1)]
    [InlineData(UnicodeSupport.No, 2)]
    public void UnicodeSupport_Values_Should_Be_Correct(UnicodeSupport support, int expectedValue)
    {
        // When
        var result = (int)support;

        // Then
        result.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(EmojiSupport.Detect, 0)]
    [InlineData(EmojiSupport.Yes, 1)]
    [InlineData(EmojiSupport.No, 2)]
    public void EmojiSupport_Values_Should_Be_Correct(EmojiSupport support, int expectedValue)
    {
        // When
        var result = (int)support;

        // Then
        result.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(LinksSupport.Detect, 0)]
    [InlineData(LinksSupport.Yes, 1)]
    [InlineData(LinksSupport.No, 2)]
    public void LinksSupport_Values_Should_Be_Correct(LinksSupport support, int expectedValue)
    {
        // When
        var result = (int)support;

        // Then
        result.ShouldBe(expectedValue);
    }

    [Fact]
    public void AnsiConsoleSettings_Should_Have_Default_Values()
    {
        // When
        var settings = new AnsiConsoleSettings();

        // Then
        settings.Ansi.ShouldBe(default(AnsiSupport));
        settings.ColorSystem.ShouldBe(ColorSystemSupport.Detect);
        settings.Interactive.ShouldBe(default(InteractionSupport));
        settings.Unicode.ShouldBe(default(UnicodeSupport));
        settings.Emoji.ShouldBe(default(EmojiSupport));
        settings.Links.ShouldBe(default(LinksSupport));
        settings.Out.ShouldBeNull();
        settings.ExclusivityMode.ShouldBeNull();
        settings.Enrichment.ShouldNotBeNull();
        settings.EnvironmentVariables.ShouldBeNull();
    }
}
