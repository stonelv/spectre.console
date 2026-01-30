using System;
using System.Linq;
using Xunit;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using Spectre.Console.Widgets;

namespace Spectre.Console.Tests.Unit.Widgets;

public class DiffTests
{
    [Fact]
    public void Should_Throw_When_OldText_Is_Null()
    {
        // Given
        var newText = "New text";
        
        // When
        var exception = Record.Exception(() => new Diff(null, newText));
        
        // Then
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal("oldText", ((ArgumentNullException)exception).ParamName);
    }
    
    [Fact]
    public void Should_Throw_When_NewText_Is_Null()
    {
        // Given
        var oldText = "Old text";
        
        // When
        var exception = Record.Exception(() => new Diff(oldText, null));
        
        // Then
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal("newText", ((ArgumentNullException)exception).ParamName);
    }
    
    [Fact]
    public void Should_Render_Empty_Texts_Correctly()
    {
        // Given
        var console = new TestConsole();
        var diff = new Diff("", "");
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe("\n");
    }
    
    [Fact]
    public void Should_Render_Unchanged_Text_Correctly_In_Inline_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nLine 2";
        var diff = new Diff(oldText, newText, DiffMode.Inline);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe("  Line 1\n  Line 2\n");
    }
    
    [Fact]
    public void Should_Render_Added_Text_Correctly_In_Inline_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1";
        var newText = "Line 1\nLine 2";
        var diff = new Diff(oldText, newText, DiffMode.Inline);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe("  Line 1\n+ Line 2\n");
    }
    
    [Fact]
    public void Should_Render_Deleted_Text_Correctly_In_Inline_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1";
        var diff = new Diff(oldText, newText, DiffMode.Inline);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe("  Line 1\n- Line 2\n");
    }
    
    [Fact]
    public void Should_Render_Mixed_Changes_Correctly_In_Inline_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nLine 3";
        var diff = new Diff(oldText, newText, DiffMode.Inline);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe("  Line 1\n- Line 2\n+ Line 3\n");
    }
    
    [Fact]
    public void Should_Render_Unchanged_Text_Correctly_In_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nLine 2";
        var diff = new Diff(oldText, newText, DiffMode.SideBySide);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe(" Line 1 |  Line 1\n Line 2 |  Line 2\n");
    }
    
    [Fact]
    public void Should_Render_Added_Text_Correctly_In_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1";
        var newText = "Line 1\nLine 2";
        var diff = new Diff(oldText, newText, DiffMode.SideBySide);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe(" Line 1 |  Line 1\n        | +Line 2\n");
    }
    
    [Fact]
    public void Should_Render_Deleted_Text_Correctly_In_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1";
        var diff = new Diff(oldText, newText, DiffMode.SideBySide);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe(" Line 1 |  Line 1\n-Line 2 | \n");
    }
    
    [Fact]
    public void Should_Render_Mixed_Changes_Correctly_In_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nLine 3";
        var diff = new Diff(oldText, newText, DiffMode.SideBySide);
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe(" Line 1 |  Line 1\n-Line 2 | \n        | +Line 3\n");
    }
    
    [Fact]
    public void Should_Render_Custom_Styles_Correctly()
    {
        // Given
        var console = new TestConsole().Colors(ColorSystem.Standard).EmitAnsiSequences();
        var oldText = "Line 1\nLine 2";
        var newText = "Line 1\nLine 3";
        var diff = new Diff(oldText, newText)
        {
            AddedStyle = new Style(Color.Blue),
            DeletedStyle = new Style(Color.Yellow),
            UnchangedStyle = new Style(Color.Magenta)
        };
        
        // When
        console.Write(diff);
        
        // Then
        console.Output.ShouldBe("\u001b[95m  Line 1\u001b[0m\n\u001b[93m- Line 2\u001b[0m\n\u001b[94m+ Line 3\u001b[0m\n");
    }
    
    [Fact]
    public void Should_Measure_Width_Correctly_In_Inline_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nA very long line that is quite long";
        var newText = "Line 1\nShort line";
        var diff = new Diff(oldText, newText, DiffMode.Inline);
        
        // When
        var options = RenderOptions.Create(console, console.Profile.Capabilities);
        var measurement = ((IRenderable)diff).Measure(options, 100);
        
        // Then
        measurement.Min.ShouldBe(2 + "A very long line that is quite long".Length);
        measurement.Max.ShouldBe(2 + "A very long line that is quite long".Length);
    }
    
    [Fact]
    public void Should_Measure_Width_Correctly_In_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nA very long line that is quite long";
        var newText = "Line 1\nShort line";
        var diff = new Diff(oldText, newText, DiffMode.SideBySide);
        
        // When
        var options = RenderOptions.Create(console, console.Profile.Capabilities);
        var measurement = ((IRenderable)diff).Measure(options, 100);
        
        // Then
        // Width should be: 1 (old prefix) + max old line length + 3 (separator) + max new line length + 1 (new prefix)
        var expectedWidth = 1 + "A very long line that is quite long".Length + 3 + "Short line".Length + 1;
        measurement.Min.ShouldBe(expectedWidth);
        measurement.Max.ShouldBe(expectedWidth);
    }
    
    [Fact]
    public void Should_Respect_Width_Constraint_In_SideBySide_Mode()
    {
        // Given
        var console = new TestConsole();
        var oldText = "Line 1\nA very long line that is quite long";
        var newText = "Line 1\nAnother very long line that is also quite long";
        var diff = new Diff(oldText, newText, DiffMode.SideBySide) { Width = 50 };
        
        // When
        var options = RenderOptions.Create(console, console.Profile.Capabilities);
        var measurement = ((IRenderable)diff).Measure(options, 100);
        
        // Then
        measurement.Min.ShouldBe(50);
        measurement.Max.ShouldBe(50);
    }
}