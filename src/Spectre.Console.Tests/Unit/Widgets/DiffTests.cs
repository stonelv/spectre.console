namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Widgets/Diff")]
public sealed class DiffTests
{
    [Fact]
    [Expectation("Render_Inline_Identical")]
    public Task Should_Render_Diff_Inline_With_Identical_Text()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Hello World\nLine 2\nLine 3";
        var newText = "Hello World\nLine 2\nLine 3";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Inline_Inserted")]
    public Task Should_Render_Diff_Inline_With_Inserted_Lines()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 3";
        var newText = "Line 1\nLine 2\nLine 3";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Inline_Deleted")]
    public Task Should_Render_Diff_Inline_With_Deleted_Lines()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2\nLine 3";
        var newText = "Line 1\nLine 3";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Inline_Mixed")]
    public Task Should_Render_Diff_Inline_With_Mixed_Changes()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nOld Line 2\nLine 3\nOld Line 4";
        var newText = "Line 1\nNew Line 2\nLine 3\nNew Line 4\nLine 5";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Inline_NoLineNumbers")]
    public Task Should_Render_Diff_Inline_Without_LineNumbers()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nModified\nLine 2";

        // When
        console.Write(new Diff(oldText, newText)
        {
            ShowLineNumbers = false,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_SideBySide_Identical")]
    public Task Should_Render_Diff_SideBySide_With_Identical_Text()
    {
        // Given
        var console = new TestConsole().Width(80);
        var oldText = "Hello World\nLine 2\nLine 3";
        var newText = "Hello World\nLine 2\nLine 3";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Format = DiffFormat.SideBySide,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_SideBySide_Inserted")]
    public Task Should_Render_Diff_SideBySide_With_Inserted_Lines()
    {
        // Given
        var console = new TestConsole().Width(80);
        var oldText = "Line 1\nLine 3";
        var newText = "Line 1\nLine 2\nLine 3";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Format = DiffFormat.SideBySide,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_SideBySide_Deleted")]
    public Task Should_Render_Diff_SideBySide_With_Deleted_Lines()
    {
        // Given
        var console = new TestConsole().Width(80);
        var oldText = "Line 1\nLine 2\nLine 3";
        var newText = "Line 1\nLine 3";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Format = DiffFormat.SideBySide,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_SideBySide_Mixed")]
    public Task Should_Render_Diff_SideBySide_With_Mixed_Changes()
    {
        // Given
        var console = new TestConsole().Width(80);
        var oldText = "Line 1\nOld Line 2\nLine 3\nOld Line 4";
        var newText = "Line 1\nNew Line 2\nLine 3\nNew Line 4\nLine 5";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Format = DiffFormat.SideBySide,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_SideBySide_NoLineNumbers")]
    public Task Should_Render_Diff_SideBySide_Without_LineNumbers()
    {
        // Given
        var console = new TestConsole().Width(80);
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nModified\nLine 2";

        // When
        console.Write(new Diff(oldText, newText)
        {
            Format = DiffFormat.SideBySide,
            ShowLineNumbers = false,
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Empty_OldText")]
    public Task Should_Render_Diff_With_Empty_Old_Text()
    {
        // Given
        var console = new TestConsole();
        var oldText = string.Empty;
        var newText = "Line 1\nLine 2";

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Empty_NewText")]
    public Task Should_Render_Diff_With_Empty_New_Text()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = string.Empty;

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Both_Empty")]
    public Task Should_Render_Diff_With_Both_Empty_Texts()
    {
        // Given
        var console = new TestConsole();
        var oldText = string.Empty;
        var newText = string.Empty;

        // When
        console.Write(new Diff(oldText, newText));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_Null_Texts")]
    public Task Should_Render_Diff_With_Null_Texts()
    {
        // Given
        var console = new TestConsole();

        // When
        console.Write(new Diff((string)null, (string)null));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_IEnumerable")]
    public Task Should_Render_Diff_With_IEnumerable_Lines()
    {
        // Given
        var console = new TestConsole();
        var oldLines = new List<string> { "Line 1", "Line 2" };
        var newLines = new List<string> { "Line 1", "Modified", "Line 2" };

        // When
        console.Write(new Diff(oldLines, newLines));

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    [Expectation("Render_CustomStyles")]
    public Task Should_Render_Diff_With_Custom_Styles()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nOld Line 2";
        var newText = "Line 1\nNew Line 2";

        // When
        console.Write(new Diff(oldText, newText)
        {
            UnchangedStyle = new Style(Color.Blue),
            InsertedStyle = new Style(Color.Yellow),
            DeletedStyle = new Style(Color.Magenta),
        });

        // Then
        return Verifier.Verify(console.Output);
    }

    [Fact]
    public void Should_Have_Correct_Line_States()
    {
        // Given
        var oldText = "Line 1\nLine 2\nLine 3";
        var newText = "Line 1\nModified\nLine 3\nAdded";

        // When
        var diff = new Diff(oldText, newText);

        // Then
        diff.Lines.Count.ShouldBe(5);
        diff.Lines[0].State.ShouldBe(DiffLineState.Unchanged);
        diff.Lines[0].Text.ShouldBe("Line 1");
        diff.Lines[0].OldLineNumber.ShouldBe(1);
        diff.Lines[0].NewLineNumber.ShouldBe(1);

        diff.Lines[1].State.ShouldBe(DiffLineState.Deleted);
        diff.Lines[1].Text.ShouldBe("Line 2");
        diff.Lines[1].OldLineNumber.ShouldBe(2);
        diff.Lines[1].NewLineNumber.ShouldBeNull();

        diff.Lines[2].State.ShouldBe(DiffLineState.Inserted);
        diff.Lines[2].Text.ShouldBe("Modified");
        diff.Lines[2].OldLineNumber.ShouldBeNull();
        diff.Lines[2].NewLineNumber.ShouldBe(2);

        diff.Lines[3].State.ShouldBe(DiffLineState.Unchanged);
        diff.Lines[3].Text.ShouldBe("Line 3");
        diff.Lines[3].OldLineNumber.ShouldBe(3);
        diff.Lines[3].NewLineNumber.ShouldBe(3);

        diff.Lines[4].State.ShouldBe(DiffLineState.Inserted);
        diff.Lines[4].Text.ShouldBe("Added");
        diff.Lines[4].OldLineNumber.ShouldBeNull();
        diff.Lines[4].NewLineNumber.ShouldBe(4);
    }

    [Fact]
    public void Should_Set_Format_Correctly()
    {
        // Given
        var diff = new Diff("a", "b");

        // When
        diff.Format = DiffFormat.SideBySide;

        // Then
        diff.Format.ShouldBe(DiffFormat.SideBySide);
    }

    [Fact]
    public void Should_Set_ShowLineNumbers_Correctly()
    {
        // Given
        var diff = new Diff("a", "b");

        // When
        diff.ShowLineNumbers = false;

        // Then
        diff.ShowLineNumbers.ShouldBeFalse();
    }

    [Fact]
    public void Should_Set_Styles_Correctly()
    {
        // Given
        var diff = new Diff("a", "b");
        var customStyle = new Style(Color.Cyan1);

        // When
        diff.UnchangedStyle = customStyle;
        diff.InsertedStyle = customStyle;
        diff.DeletedStyle = customStyle;

        // Then
        diff.UnchangedStyle.ShouldBe(customStyle);
        diff.InsertedStyle.ShouldBe(customStyle);
        diff.DeletedStyle.ShouldBe(customStyle);
    }
}
