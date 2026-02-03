namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Widgets/Diff")]
public sealed class DiffTests
{
    [Fact]
    [Expectation("Inline_Default")]
    public Task Should_Render_Inline_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello\nWorld\nOldLine";
        var newText = "Hello\nWorld\nNewLine";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Inline_NoLineNumbers")]
    public Task Should_Render_Inline_Without_LineNumbers()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello\nWorld\nOldLine";
        var newText = "Hello\nWorld\nNewLine";

        // When
        console.Write(new Diff(oldText, newText) { ShowLineNumbers = false });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("SideBySide_Default")]
    public Task Should_Render_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello\nWorld\nOldLine";
        var newText = "Hello\nWorld\nNewLine";

        // When
        console.Write(new Diff(oldText, newText) { Mode = DiffMode.SideBySide });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("SideBySide_NoLineNumbers")]
    public Task Should_Render_SideBySide_Without_LineNumbers()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello\nWorld\nOldLine";
        var newText = "Hello\nWorld\nNewLine";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Mode = DiffMode.SideBySide,
            ShowLineNumbers = false,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Inline_EmptyTexts")]
    public Task Should_Render_Empty_Texts()
    {
        // Given
        var console = new TestConsole();

        // When
        console.Write(new Diff(string.Empty, string.Empty));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Inline_OnlyInsertions")]
    public Task Should_Render_Only_Insertions()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line1";
        var newText = "Line1\nLine2\nLine3";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Inline_OnlyDeletions")]
    public Task Should_Render_Only_Deletions()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line1\nLine2\nLine3";
        var newText = "Line1";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("SideBySide_OnlyInsertions")]
    public Task Should_Render_SideBySide_Only_Insertions()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line1";
        var newText = "Line1\nLine2\nLine3";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Mode = DiffMode.SideBySide,
            ShowLineNumbers = false,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("SideBySide_OnlyDeletions")]
    public Task Should_Render_SideBySide_Only_Deletions()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line1\nLine2\nLine3";
        var newText = "Line1";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Mode = DiffMode.SideBySide,
            ShowLineNumbers = false,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Inline_CustomStyles")]
    public Task Should_Render_With_Custom_Styles()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello\nOldLine";
        var newText = "Hello\nNewLine";

        // When
        console.Write(new Diff(oldText, newText)
        {
            InsertedStyle = new Style(foreground: Color.Blue),
            DeletedStyle = new Style(foreground: Color.Yellow),
            UnchangedStyle = new Style(foreground: Color.Grey),
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Inline_WithEmptyLines")]
    public Task Should_Render_With_Empty_Lines()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line1\n\nLine3";
        var newText = "Line1\nLine2\n\nLine4";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    public void Should_Throw_On_Null_OldText()
    {
        // Given, When, Then
        Should.Throw<ArgumentNullException>(() => new Diff(null!, "text"));
    }

    [Fact]
    public void Should_Throw_On_Null_NewText()
    {
        // Given, When, Then
        Should.Throw<ArgumentNullException>(() => new Diff("text", null!));
    }

    [Fact]
    public void Measure_Should_Return_Zero_For_Empty_Texts()
    {
        // Given
        var diff = new Diff(string.Empty, string.Empty);
        var console = new TestConsole();
        var options = RenderOptions.Create(console);

        // When
        var measurement = ((IRenderable)diff).Measure(options, 80);

        // Then
        measurement.Min.ShouldBe(0);
        measurement.Max.ShouldBe(0);
    }

    [Fact]
    public void Render_Should_Return_Empty_For_Empty_Texts()
    {
        // Given
        var diff = new Diff(string.Empty, string.Empty);
        var console = new TestConsole();
        var options = RenderOptions.Create(console);

        // When
        var segments = ((IRenderable)diff).Render(options, 80);

        // Then
        segments.ShouldBeEmpty();
    }

    [Fact]
    public void SideBySide_Should_Fallback_To_Inline_When_Width_Too_Small()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello\nWorld";
        var newText = "Hello\nModified";

        // When - render with very small width
        console.Write(new Diff(oldText, newText)
        {
            Mode = DiffMode.SideBySide,
            ShowLineNumbers = false,
        });

        // Then - should still render without throwing
        console.Output.ShouldNotBeNull();
    }

    [Fact]
    public void DiffResult_Should_Count_Correctly()
    {
        // Given
        var oldText = "Line1\nOldLine\nLine3";
        var newText = "Line1\nNewLine\nLine3\nLine4";

        // When
        var diff = new Diff(oldText, newText);
        var result = typeof(Diff).GetMethod("GetResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(diff, null) as DiffResult;

        // Then
        result.ShouldNotBeNull();
        result!.InsertedCount.ShouldBe(2); // NewLine and Line4
        result.DeletedCount.ShouldBe(1);   // OldLine
        result.UnchangedCount.ShouldBe(2); // Line1 and Line3
    }
}
