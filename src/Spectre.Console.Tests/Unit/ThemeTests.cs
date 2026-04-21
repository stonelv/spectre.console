namespace Spectre.Console.Tests.Unit;

public sealed class ThemeTests
{
    [Fact]
    public void Modern_Theme_Should_Have_Correct_Values()
    {
        // When
        var theme = Theme.Modern;

        // Then
        theme.BoxBorder.ShouldBe(BoxBorder.Rounded);
        theme.TableBorder.ShouldBe(TableBorder.Rounded);
        theme.TreeGuide.ShouldBe(TreeGuide.Line);
        theme.ColorSystem.ShouldBe(ColorSystem.TrueColor);
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeTrue();
    }

    [Fact]
    public void Classic_Theme_Should_Have_Correct_Values()
    {
        // When
        var theme = Theme.Classic;

        // Then
        theme.BoxBorder.ShouldBe(BoxBorder.Square);
        theme.TableBorder.ShouldBe(TableBorder.Square);
        theme.TreeGuide.ShouldBe(TreeGuide.Line);
        theme.ColorSystem.ShouldBe(ColorSystem.TrueColor);
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeFalse();
    }

    [Fact]
    public void Plain_Theme_Should_Have_Correct_Values()
    {
        // When
        var theme = Theme.Plain;

        // Then
        theme.BoxBorder.ShouldBe(BoxBorder.Ascii);
        theme.TableBorder.ShouldBe(TableBorder.Ascii);
        theme.TreeGuide.ShouldBe(TreeGuide.Ascii);
        theme.ColorSystem.ShouldBe(ColorSystem.TrueColor);
        theme.UsesUnicode.ShouldBeFalse();
        theme.UsesEmoji.ShouldBeFalse();
    }

    [Fact]
    public void Minimal_Theme_Should_Have_Correct_Values()
    {
        // When
        var theme = Theme.Minimal;

        // Then
        theme.BoxBorder.ShouldBe(BoxBorder.None);
        theme.TableBorder.ShouldBe(TableBorder.Minimal);
        theme.TreeGuide.ShouldBe(TreeGuide.Line);
        theme.ColorSystem.ShouldBe(ColorSystem.TrueColor);
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeFalse();
    }

    [Theory]
    [InlineData(true, true, true, true)]
    [InlineData(true, false, true, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, false, false)]
    public void Create_From_Parameters_Should_Set_Correct_Values(
        bool supportsUnicode, bool supportsEmoji,
        bool expectsUnicode, bool expectsEmojiResult)
    {
        // When
        var theme = Theme.Create(supportsUnicode, supportsEmoji);

        // Then
        theme.UsesUnicode.ShouldBe(expectsUnicode);
        theme.UsesEmoji.ShouldBe(expectsEmojiResult);
    }

    [Fact]
    public void Create_From_Capabilities_With_Full_Support_Should_Return_Modern_Like_Theme()
    {
        // Given
        var capabilities = new TestCapabilities
        {
            ColorSystem = ColorSystem.TrueColor,
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
        var theme = Theme.Create(capabilities);

        // Then
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeTrue();
        theme.ColorSystem.ShouldBe(ColorSystem.TrueColor);
    }

    [Fact]
    public void Create_From_Capabilities_With_Unicode_Only_Should_Return_Classic_Like_Theme()
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
            Emoji = false
        };

        // When
        var theme = Theme.Create(capabilities);

        // Then
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeFalse();
        theme.ColorSystem.ShouldBe(ColorSystem.EightBit);
    }

    [Fact]
    public void Create_From_Capabilities_Without_Unicode_Should_Return_Plain_Like_Theme()
    {
        // Given
        var capabilities = new TestCapabilities
        {
            ColorSystem = ColorSystem.Legacy,
            Ansi = false,
            Links = false,
            Legacy = true,
            IsTerminal = false,
            Interactive = false,
            Unicode = false,
            AlternateBuffer = false,
            Emoji = false
        };

        // When
        var theme = Theme.Create(capabilities);

        // Then
        theme.UsesUnicode.ShouldBeFalse();
        theme.UsesEmoji.ShouldBeFalse();
        theme.BoxBorder.ShouldBe(BoxBorder.Ascii);
        theme.TableBorder.ShouldBe(TableBorder.Ascii);
        theme.TreeGuide.ShouldBe(TreeGuide.Ascii);
    }

    [Fact]
    public void Create_With_Null_Capabilities_Should_Throw()
    {
        // Given
        IReadOnlyCapabilities? capabilities = null;

        // When
        var exception = Record.Exception(() => Theme.Create(capabilities!));

        // Then
        exception.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Default_Constructor_Should_Have_Reasonable_Defaults()
    {
        // When
        var theme = new Theme();

        // Then
        theme.BoxBorder.ShouldBe(BoxBorder.Square);
        theme.TableBorder.ShouldBe(TableBorder.Square);
        theme.TreeGuide.ShouldBe(TreeGuide.Line);
        theme.ColorSystem.ShouldBe(ColorSystem.TrueColor);
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeFalse();
    }

    [Fact]
    public void Custom_Theme_Should_Use_Provided_Values()
    {
        // When
        var theme = new Theme(
            boxBorder: BoxBorder.Double,
            tableBorder: TableBorder.Heavy,
            treeGuide: TreeGuide.DoubleLine,
            colorSystem: ColorSystem.EightBit,
            usesUnicode: true,
            usesEmoji: true);

        // Then
        theme.BoxBorder.ShouldBe(BoxBorder.Double);
        theme.TableBorder.ShouldBe(TableBorder.Heavy);
        theme.TreeGuide.ShouldBe(TreeGuide.DoubleLine);
        theme.ColorSystem.ShouldBe(ColorSystem.EightBit);
        theme.UsesUnicode.ShouldBeTrue();
        theme.UsesEmoji.ShouldBeTrue();
    }
}
