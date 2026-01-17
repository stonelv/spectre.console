using Shouldly;
using Xunit;
using System.IO;

namespace Spectre.Console.Tests.Unit;

public sealed class DiffTests
{
    [Fact]
    public void Should_Show_Added_And_Deleted_Lines()
    {
        // Given
        var oldText = "line1\nline2\nline3";
        var newText = "line1\nline4\nline3";
        var console = new TestConsole();
        var diff = new Diff(oldText, newText);

        // When
        console.Write(diff);

        // Then
        console.Output.ShouldContain(" line1");
        console.Output.ShouldContain("-line2");
        console.Output.ShouldContain("+line4");
    }

    [Fact]
    public void Test_Diff_Algorithm()
    {
        // Given
        var oldText = "line1\nline2\nline3";
        var newText = "line1\nline4\nline3";
        
        // When
        var diffLines = DiffAlgorithm.ComputeDiff(oldText, newText);
        
        // Then
        diffLines.ShouldNotBeEmpty();
        diffLines.Count.ShouldBe(4);
        diffLines[0].Type.ShouldBe(DiffLineType.Unchanged);
        diffLines[0].Content.ShouldBe("line1");
        diffLines[1].Type.ShouldBe(DiffLineType.Deleted);
        diffLines[1].Content.ShouldBe("line2");
        diffLines[2].Type.ShouldBe(DiffLineType.Added);
        diffLines[2].Content.ShouldBe("line4");
        diffLines[3].Type.ShouldBe(DiffLineType.Unchanged);
        diffLines[3].Content.ShouldBe("line3");
    }

    [Fact]
    public void Should_Support_Side_By_Side_Mode()
    {
        // Given
        var oldText = "line1\nline2\nline3";
        var newText = "line1\nline4\nline3";
        var console = new TestConsole();
        var diff = new Diff(oldText, newText)
        {
            RenderMode = DiffRenderMode.SideBySide
        };

        // When
        console.Write(diff);

        // Then
        console.Output.ShouldNotBeEmpty();
    }

    [Fact]
    public void Should_Demo_With_Files()
    {
        // Given
        var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
        var oldText = File.ReadAllText(Path.Combine(projectRoot, "test_old.txt"));
        var newText = File.ReadAllText(Path.Combine(projectRoot, "test_new.txt"));
        var console = new TestConsole();
        
        // When - Test inline mode
        console.WriteLine("=== Inline Mode ===");
        var diffInline = new Diff(oldText, newText);
        console.Write(diffInline);
        
        console.WriteLine();
        console.WriteLine("=" + string.Empty.PadRight(78, '=') + "=");
        
        // When - Test side-by-side mode
        console.WriteLine("=== Side-by-Side Mode ===");
        var diffSideBySide = new Diff(oldText, newText)
        {
            RenderMode = DiffRenderMode.SideBySide
        };
        console.Write(diffSideBySide);

        // Then
        console.Output.ShouldNotBeEmpty();
        console.Output.ShouldContain("Inline Mode");
        console.Output.ShouldContain("Side-by-Side Mode");
        
        // Print the actual output to console for manual inspection
        System.Console.WriteLine("\n" + new string('=', 80));
        System.Console.WriteLine("DIFF COMPONENT DEMO OUTPUT:");
        System.Console.WriteLine(new string('=', 80));
        System.Console.WriteLine(console.Output);
    }
}
