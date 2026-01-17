using Xunit;
using Spectre.Console;
using System;

namespace CodeEditorTest;

public class CodeEditorUnitTests
{
    [Fact]
    public void Test_SetText_And_GetText()
    {
        var editor = new CodeEditor();
        var testText = "Hello\nWorld\n!";
        
        editor.SetText(testText);
        var result = editor.GetText();
        
        Assert.Equal(testText, result);
    }

    [Fact]
    public void Test_Text_Input()
    {
        var editor = new CodeEditor();
        editor.SetText("");
        
        // Simulate typing "Hello"
        foreach (var c in "Hello")
        {
            var key = new ConsoleKeyInfo(c, ConsoleKey.A, false, false, false);
            editor.HandleKey(key);
        }
        
        Assert.Equal("Hello", editor.GetText());
    }

    [Fact]
    public void Test_Backspace()
    {
        var editor = new CodeEditor();
        editor.SetText("Hello");
        
        // Move cursor to end
        var endKey = new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false);
        editor.HandleKey(endKey);
        
        // Press backspace 2 times
        var backspaceKey = new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false);
        editor.HandleKey(backspaceKey);
        editor.HandleKey(backspaceKey);
        
        Assert.Equal("Hel", editor.GetText());
    }

    [Fact]
    public void Test_Delete()
    {
        var editor = new CodeEditor();
        editor.SetText("Hello");
        
        // Press delete 2 times from position 0
        var deleteKey = new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);
        editor.HandleKey(deleteKey);
        editor.HandleKey(deleteKey);
        
        Assert.Equal("llo", editor.GetText());
    }

    [Fact]
    public void Test_Enter()
    {
        var editor = new CodeEditor();
        editor.SetText("Hello");
        
        // Move to position 2 and press enter
        var rightKey = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
        editor.HandleKey(rightKey);
        editor.HandleKey(rightKey);
        
        var enterKey = new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false);
        editor.HandleKey(enterKey);
        
        Assert.Equal("He\nllo", editor.GetText());
    }

    [Fact]
    public void Test_Undo()
    {
        var editor = new CodeEditor();
        editor.SetText("");
        
        // Type something
        foreach (var c in "Hello")
        {
            var key = new ConsoleKeyInfo(c, ConsoleKey.A, false, false, false);
            editor.HandleKey(key);
        }
        
        // Undo
        var undoKey = new ConsoleKeyInfo('\0', ConsoleKey.Z, false, false, true);
        editor.HandleKey(undoKey);
        
        // Should be empty after undo
        Assert.Equal("", editor.GetText());
    }

    [Fact]
    public void Test_Redo()
    {
        var editor = new CodeEditor();
        editor.SetText("");
        
        // Type something
        foreach (var c in "Hello")
        {
            var key = new ConsoleKeyInfo(c, ConsoleKey.A, false, false, false);
            editor.HandleKey(key);
        }
        
        // Undo
        var undoKey = new ConsoleKeyInfo('\0', ConsoleKey.Z, false, false, true);
        editor.HandleKey(undoKey);
        
        // Redo
        var redoKey = new ConsoleKeyInfo('\0', ConsoleKey.Y, false, false, true);
        editor.HandleKey(redoKey);
        
        // Should be "Hello" after redo
        Assert.Equal("Hello", editor.GetText());
    }

    [Fact]
    public void Test_Undo_Limit_10_Steps()
    {
        var editor = new CodeEditor();
        editor.SetText("");
        
        // Make 15 changes
        for (int i = 0; i < 15; i++)
        {
            var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
            editor.HandleKey(key);
        }
        
        // Try to undo 15 times
        var undoKey = new ConsoleKeyInfo('\0', ConsoleKey.Z, false, false, true);
        for (int i = 0; i < 15; i++)
        {
            editor.HandleKey(undoKey);
        }
        
        // Should only be able to undo 10 steps, leaving 5 'a's
        var result = editor.GetText();
        Assert.Equal("aaaaa", result);
        Assert.Equal(5, result.Length);
    }

    [Fact]
    public void Test_Arrow_Navigation()
    {
        var editor = new CodeEditor();
        editor.SetText("Hello World");
        
        // Test right arrow
        for (int i = 0; i < 5; i++)
        {
            var key = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
            editor.HandleKey(key);
        }
        
        // Should be at position 5 (space)
        // Now test left arrow back
        for (int i = 0; i < 5; i++)
        {
            var key = new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false);
            editor.HandleKey(key);
        }
        
        // Should be at position 0
        // Type 'T' to verify
        var tKey = new ConsoleKeyInfo('T', ConsoleKey.T, false, false, false);
        editor.HandleKey(tKey);
        
        Assert.Equal("THello World", editor.GetText());
    }

    [Fact]
    public void Test_Home_And_End()
    {
        var editor = new CodeEditor();
        editor.SetText("Hello World");
        
        // Move to end
        var endKey = new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false);
        editor.HandleKey(endKey);
        
        // Type '!'
        var exclamationKey = new ConsoleKeyInfo('!', ConsoleKey.Oem1, false, false, false);
        editor.HandleKey(exclamationKey);
        
        Assert.Equal("Hello World!", editor.GetText());
        
        // Move to home
        var homeKey = new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false);
        editor.HandleKey(homeKey);
        
        // Type '>'
        var gtKey = new ConsoleKeyInfo('>', ConsoleKey.OemPeriod, false, false, false);
        editor.HandleKey(gtKey);
        
        Assert.Equal(">Hello World!", editor.GetText());
    }
}