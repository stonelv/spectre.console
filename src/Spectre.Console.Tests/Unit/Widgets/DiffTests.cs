using Spectre.Console.Testing;

namespace Spectre.Console.Tests.Unit;

public sealed class DiffTests
{
    public sealed class TheConstructor
    {
        [Fact]
        public void Should_Throw_If_Old_Text_Is_Null()
        {
            // Given
            string oldText = null;
            var newText = "new";

            // When
            var result = Record.Exception(() => new Diff(oldText, newText));

            // Then
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Throw_If_New_Text_Is_Null()
        {
            // Given
            var oldText = "old";
            string newText = null;

            // When
            var result = Record.Exception(() => new Diff(oldText, newText));

            // Then
            result.ShouldNotBeNull();
        }
    }

    public sealed class TheInlineMode
    {
        [Fact]
        public void Should_Render_Inline_Diff()
        {
            // Given
            var console = new TestConsole();
            var oldText = "Line 1\nLine 2\nLine 3";
            var newText = "Line 1\nLine 2 modified\nLine 3";
            var diff = new Diff(oldText, newText).Mode(DiffMode.Inline);

            // When
            console.Write(diff);

            // Then
            console.Output.ShouldContain("Line 1");
            console.Output.ShouldContain("Line 2");
            console.Output.ShouldContain("Line 2 modified");
            console.Output.ShouldContain("Line 3");
        }

        [Fact]
        public void Should_Show_Deleted_Lines_In_Red()
        {
            // Given
            var console = new TestConsole();
            var oldText = "Line 1\nLine 2\nLine 3";
            var newText = "Line 1\nLine 3";
            var diff = new Diff(oldText, newText).Mode(DiffMode.Inline);

            // When
            console.Write(diff);

            // Then
            console.Output.ShouldContain("-");
        }

        [Fact]
        public void Should_Show_Inserted_Lines_In_Green()
        {
            // Given
            var console = new TestConsole();
            var oldText = "Line 1\nLine 3";
            var newText = "Line 1\nLine 2\nLine 3";
            var diff = new Diff(oldText, newText).Mode(DiffMode.Inline);

            // When
            console.Write(diff);

            // Then
            console.Output.ShouldContain("+");
        }
    }

    public sealed class TheSideBySideMode
    {
        [Fact]
        public void Should_Render_SideBySide_Diff()
        {
            // Given
            var console = new TestConsole();
            var oldText = "Line 1\nLine 2\nLine 3";
            var newText = "Line 1\nLine 2 modified\nLine 3";
            var diff = new Diff(oldText, newText).Mode(DiffMode.SideBySide);

            // When
            console.Write(diff);

            // Then
            console.Output.ShouldContain("Line 1");
            console.Output.ShouldContain("Line 2");
            console.Output.ShouldContain("Line 2 modified");
            console.Output.ShouldContain("Line 3");
        }
    }

    public sealed class TheExtensionMethods
    {
        [Fact]
        public void Should_Set_Deleted_Color()
        {
            // Given
            var oldText = "Line 1\nLine 2";
            var newText = "Line 1";
            var diff = new Diff(oldText, newText);

            // When
            var result = diff.DeletedColor(Color.Yellow);

            // Then
            result.ShouldBeSameAs(diff);
            diff.DeletedStyle.Foreground.ShouldBe(Color.Yellow);
        }

        [Fact]
        public void Should_Set_Inserted_Color()
        {
            // Given
            var oldText = "Line 1";
            var newText = "Line 1\nLine 2";
            var diff = new Diff(oldText, newText);

            // When
            var result = diff.InsertedColor(Color.Yellow);

            // Then
            result.ShouldBeSameAs(diff);
            diff.InsertedStyle.Foreground.ShouldBe(Color.Yellow);
        }

        [Fact]
        public void Should_Set_Unchanged_Color()
        {
            // Given
            var oldText = "Line 1\nLine 2";
            var newText = "Line 1\nLine 2";
            var diff = new Diff(oldText, newText);

            // When
            var result = diff.UnchangedColor(Color.Yellow);

            // Then
            result.ShouldBeSameAs(diff);
            diff.UnchangedStyle.Foreground.ShouldBe(Color.Yellow);
        }

        [Fact]
        public void Should_Set_Deleted_Style()
        {
            // Given
            var oldText = "Line 1\nLine 2";
            var newText = "Line 1";
            var diff = new Diff(oldText, newText);
            var style = new Style(Color.Yellow, Color.Black);

            // When
            var result = diff.DeletedStyle(style);

            // Then
            result.ShouldBeSameAs(diff);
            diff.DeletedStyle.ShouldBe(style);
        }

        [Fact]
        public void Should_Set_Inserted_Style()
        {
            // Given
            var oldText = "Line 1";
            var newText = "Line 1\nLine 2";
            var diff = new Diff(oldText, newText);
            var style = new Style(Color.Yellow, Color.Black);

            // When
            var result = diff.InsertedStyle(style);

            // Then
            result.ShouldBeSameAs(diff);
            diff.InsertedStyle.ShouldBe(style);
        }

        [Fact]
        public void Should_Set_Unchanged_Style()
        {
            // Given
            var oldText = "Line 1\nLine 2";
            var newText = "Line 1\nLine 2";
            var diff = new Diff(oldText, newText);
            var style = new Style(Color.Yellow, Color.Black);

            // When
            var result = diff.UnchangedStyle(style);

            // Then
            result.ShouldBeSameAs(diff);
            diff.UnchangedStyle.ShouldBe(style);
        }

        [Fact]
        public void Should_Set_Deleted_Marker()
        {
            // Given
            var oldText = "Line 1\nLine 2";
            var newText = "Line 1";
            var diff = new Diff(oldText, newText);

            // When
            var result = diff.DeletedMarker("<");

            // Then
            result.ShouldBeSameAs(diff);
            diff.DeletedMarker.ShouldBe("<");
        }

        [Fact]
        public void Should_Set_Inserted_Marker()
        {
            // Given
            var oldText = "Line 1";
            var newText = "Line 1\nLine 2";
            var diff = new Diff(oldText, newText);

            // When
            var result = diff.InsertedMarker(">");

            // Then
            result.ShouldBeSameAs(diff);
            diff.InsertedMarker.ShouldBe(">");
        }

        [Fact]
        public void Should_Set_Unchanged_Marker()
        {
            // Given
            var oldText = "Line 1\nLine 2";
            var newText = "Line 1\nLine 2";
            var diff = new Diff(oldText, newText);

            // When
            var result = diff.UnchangedMarker("=");

            // Then
            result.ShouldBeSameAs(diff);
            diff.UnchangedMarker.ShouldBe("=");
        }
    }

    public sealed class TheDiffAlgorithm
    {
        [Fact]
        public void Should_Detect_Inserted_Lines()
        {
            // Given
            var oldText = "Line 1\nLine 3";
            var newText = "Line 1\nLine 2\nLine 3";
            var diff = new Diff(oldText, newText);

            // When
            var changes = diff.GetType().GetField("_changes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(diff) as List<DiffChange>;

            // Then
            changes.ShouldNotBeNull();
            changes.Any(c => c.Type == DiffChangeType.Inserted && c.Line == "Line 2").ShouldBeTrue();
        }

        [Fact]
        public void Should_Detect_Deleted_Lines()
        {
            // Given
            var oldText = "Line 1\nLine 2\nLine 3";
            var newText = "Line 1\nLine 3";
            var diff = new Diff(oldText, newText);

            // When
            var changes = diff.GetType().GetField("_changes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(diff) as List<DiffChange>;

            // Then
            changes.ShouldNotBeNull();
            changes.Any(c => c.Type == DiffChangeType.Deleted && c.Line == "Line 2").ShouldBeTrue();
        }

        [Fact]
        public void Should_Detect_Unchanged_Lines()
        {
            // Given
            var oldText = "Line 1\nLine 2\nLine 3";
            var newText = "Line 1\nLine 2\nLine 3";
            var diff = new Diff(oldText, newText);

            // When
            var changes = diff.GetType().GetField("_changes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(diff) as List<DiffChange>;

            // Then
            changes.ShouldNotBeNull();
            changes.Count(c => c.Type == DiffChangeType.Unchanged).ShouldBe(3);
        }
    }
}
