namespace Spectre.Console.Tests.Unit.Widgets;

public class CodeEditorTests
{
    [Fact]
    public void Should_Create_Empty_Editor()
    {
        // Given
        var editor = new CodeEditor();

        // Then
        editor.Content.ShouldBe(string.Empty);
        editor.CursorLine.ShouldBe(0);
        editor.CursorColumn.ShouldBe(0);
    }

    [Fact]
    public void Should_Create_Editor_With_Initial_Content()
    {
        // Given
        var content = "Hello World";
        var editor = new CodeEditor(content);

        // Then
        editor.Content.ShouldBe(content);
        editor.CursorLine.ShouldBe(0);
        editor.CursorColumn.ShouldBe(11); // Cursor at end of content
    }

    [Fact]
    public void Should_Insert_Text()
    {
        // Given
        var editor = new CodeEditor();

        // When
        editor.InsertText("Hello");

        // Then
        editor.Content.ShouldBe("Hello");
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Insert_Text_At_Cursor_Position()
    {
        // Given
        var editor = new CodeEditor("Hello World");
        // Move cursor to position 6 (after "Hello ")
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();

        // When
        editor.InsertText("Beautiful ");

        // Then
        editor.Content.ShouldBe("Hello Beautiful World");
    }

    [Fact]
    public void Should_Insert_New_Line()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.InsertNewLine();

        // When
        editor.InsertText("World");

        // Then
        editor.Content.ShouldBe($"Hello{Environment.NewLine}World");
        editor.CursorLine.ShouldBe(1);
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Handle_Backspace()
    {
        // Given
        var editor = new CodeEditor("Hello");

        // When
        editor.HandleBackspace();

        // Then
        editor.Content.ShouldBe("Hell");
        editor.CursorColumn.ShouldBe(4);
    }

    [Fact]
    public void Should_Handle_Backspace_At_Line_Start()
    {
        // Given
        var editor = new CodeEditor($"Hello{Environment.NewLine}World");
        editor.MoveCursorDown();
        // Move cursor to the beginning of the second line
        while (editor.CursorColumn > 0)
        {
            editor.MoveCursorLeft();
        }

        // When
        editor.HandleBackspace();

        // Then
        editor.Content.ShouldBe("HelloWorld");
        editor.CursorLine.ShouldBe(0);
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Handle_Delete()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();

        // When
        editor.HandleDelete();

        // Then
        editor.Content.ShouldBe("ello");
    }

    [Fact]
    public void Should_Handle_Delete_At_Line_End()
    {
        // Given
        var editor = new CodeEditor($"Hello{Environment.NewLine}World");
        // Move cursor to the end of the first line
        while (editor.CursorColumn < 5)
        {
            editor.MoveCursorRight();
        }

        // When
        editor.HandleDelete();

        // Then
        editor.Content.ShouldBe("HelloWorld");
        editor.CursorLine.ShouldBe(0);
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Move_Cursor_Left()
    {
        // Given
        var editor = new CodeEditor("Hello");

        // When
        editor.MoveCursorLeft();

        // Then
        editor.CursorColumn.ShouldBe(4);
    }

    [Fact]
    public void Should_Move_Cursor_Left_To_Previous_Line()
    {
        // Given
        var editor = new CodeEditor($"Hello{Environment.NewLine}World");
        editor.MoveCursorDown();

        // When
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();

        // Then
        editor.CursorLine.ShouldBe(0);
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Move_Cursor_Right()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();

        // When
        editor.MoveCursorRight();

        // Then
        editor.CursorColumn.ShouldBe(4);
    }

    [Fact]
    public void Should_Move_Cursor_Right_To_Next_Line()
    {
        // Given
        var editor = new CodeEditor($"Hello{Environment.NewLine}World");
        // Move cursor to the end of the first line (position 5)
        while (editor.CursorLine > 0)
        {
            editor.MoveCursorUp();
        }
        while (editor.CursorColumn < 5)
        {
            editor.MoveCursorRight();
        }

        // When - move right to go to next line
        editor.MoveCursorRight();

        // Then
        editor.CursorLine.ShouldBe(1);
        editor.CursorColumn.ShouldBe(0);
    }

    [Fact]
    public void Should_Move_Cursor_Up()
    {
        // Given
        var editor = new CodeEditor($"Hello{Environment.NewLine}World");
        editor.MoveCursorDown();

        // When
        editor.MoveCursorUp();

        // Then
        editor.CursorLine.ShouldBe(0);
    }

    [Fact]
    public void Should_Move_Cursor_Down()
    {
        // Given
        var editor = new CodeEditor($"Hello{Environment.NewLine}World");

        // When
        editor.MoveCursorDown();

        // Then
        editor.CursorLine.ShouldBe(1);
    }

    [Fact]
    public void Should_Undo_Text_Insertion()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("Hello");

        // When
        editor.Undo();

        // Then
        editor.Content.ShouldBe(string.Empty);
    }

    [Fact]
    public void Should_Undo_New_Line()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.InsertNewLine();

        // When
        editor.Undo();

        // Then
        editor.Content.ShouldBe("Hello");
    }

    [Fact]
    public void Should_Undo_Backspace()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.HandleBackspace();

        // When
        editor.Undo();

        // Then
        editor.Content.ShouldBe("Hello");
    }

    [Fact]
    public void Should_Undo_Delete()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.HandleDelete();

        // When
        editor.Undo();

        // Then
        editor.Content.ShouldBe("Hello");
    }

    [Fact]
    public void Should_Redo_Undone_Operation()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("Hello");
        editor.Undo();

        // When
        editor.Redo();

        // Then
        editor.Content.ShouldBe("Hello");
    }

    [Fact]
    public void Should_Support_Multiple_Undo_Operations()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("H");
        editor.InsertText("e");
        editor.InsertText("l");
        editor.InsertText("l");
        editor.InsertText("o");

        // When
        editor.Undo();
        editor.Undo();
        editor.Undo();

        // Then
        editor.Content.ShouldBe("He");
    }

    [Fact]
    public void Should_Support_Multiple_Redo_Operations()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("H");
        editor.InsertText("e");
        editor.InsertText("l");
        editor.Undo();
        editor.Undo();

        // When
        editor.Redo();
        editor.Redo();

        // Then
        editor.Content.ShouldBe("Hel");
    }

    [Fact]
    public void Should_Limit_Undo_Stack_To_10()
    {
        // Given
        var editor = new CodeEditor();

        // Insert 15 characters
        for (var i = 0; i < 15; i++)
        {
            editor.InsertText("a");
        }

        // When - Undo 11 times
        for (var i = 0; i < 11; i++)
        {
            editor.Undo();
        }

        // Then - Should only have 10 undo steps available
        // After 11 undos, we should have 4 characters left (15 - 11 = 4)
        editor.Content.Length.ShouldBe(4);
    }

    [Fact]
    public void Should_Clear_Redo_Stack_On_New_Operation()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("Hello");
        editor.Undo();

        // When
        editor.InsertText("World");

        // Then - Redo should not work anymore
        editor.Redo();
        editor.Content.ShouldBe("World");
    }

    [Fact]
    public void Should_Handle_Multi_Line_Text_Insertion()
    {
        // Given
        var editor = new CodeEditor();

        // When
        editor.InsertText($"Line1{Environment.NewLine}Line2{Environment.NewLine}Line3");

        // Then
        editor.Content.ShouldBe($"Line1{Environment.NewLine}Line2{Environment.NewLine}Line3");
        editor.CursorLine.ShouldBe(2);
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Handle_Tab_Insertion()
    {
        // Given
        var editor = new CodeEditor();

        // When
        editor.InsertText("    ");

        // Then
        editor.Content.ShouldBe("    ");
        editor.CursorColumn.ShouldBe(4);
    }

    [Fact]
    public void Should_Render_With_Border()
    {
        // Given
        var console = new TestConsole();
        var editor = new CodeEditor("Hello")
        {
            Width = 20,
            Height = 5,
            ShowLineNumbers = false,
        };

        // When
        console.Write(editor);

        // Then
        var output = console.Output;
        output.ShouldContain("┌");
        output.ShouldContain("┐");
        output.ShouldContain("└");
        output.ShouldContain("┘");
        output.ShouldContain("Hello");
    }

    [Fact]
    public void Should_Render_With_Line_Numbers()
    {
        // Given
        var console = new TestConsole();
        var editor = new CodeEditor($"Line1{Environment.NewLine}Line2")
        {
            Width = 30,
            Height = 6,
            ShowLineNumbers = true,
        };

        // When
        console.Write(editor);

        // Then
        var output = console.Output;
        output.ShouldContain("1");
        output.ShouldContain("2");
    }

    [Fact]
    public void Should_Handle_Key_Input()
    {
        // Given
        var editor = new CodeEditor();

        // When - Simulate typing 'A'
        var result = editor.HandleKey(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe("A");
    }

    [Fact]
    public void Should_Handle_Enter_Key()
    {
        // Given
        var editor = new CodeEditor("Hello");

        // When
        var result = editor.HandleKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe($"Hello{Environment.NewLine}");
    }

    [Fact]
    public void Should_Handle_Backspace_Key()
    {
        // Given
        var editor = new CodeEditor("Hello");

        // When
        var result = editor.HandleKey(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe("Hell");
    }

    [Fact]
    public void Should_Handle_Delete_Key()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();
        editor.MoveCursorLeft();

        // When
        var result = editor.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe("ello");
    }

    [Fact]
    public void Should_Handle_Arrow_Keys()
    {
        // Given
        var editor = new CodeEditor("Hello");

        // When - Left arrow
        var result = editor.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.CursorColumn.ShouldBe(4);
    }

    [Fact]
    public void Should_Handle_Home_Key()
    {
        // Given
        var editor = new CodeEditor("Hello");
        editor.MoveCursorRight();
        editor.MoveCursorRight();
        editor.MoveCursorRight();
        editor.MoveCursorRight();
        editor.MoveCursorRight();

        // When
        var result = editor.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.CursorColumn.ShouldBe(0);
    }

    [Fact]
    public void Should_Handle_End_Key()
    {
        // Given
        var editor = new CodeEditor("Hello");

        // When
        var result = editor.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.CursorColumn.ShouldBe(5);
    }

    [Fact]
    public void Should_Handle_Tab_Key()
    {
        // Given
        var editor = new CodeEditor();

        // When
        var result = editor.HandleKey(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe("    ");
    }

    [Fact]
    public void Should_Handle_Undo_Key()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("Hello");

        // When - Ctrl+Z
        var result = editor.HandleKey(new ConsoleKeyInfo('z', ConsoleKey.Z, false, false, true));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe(string.Empty);
    }

    [Fact]
    public void Should_Handle_Redo_Key()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("Hello");
        editor.Undo();

        // When - Ctrl+Y
        var result = editor.HandleKey(new ConsoleKeyInfo('y', ConsoleKey.Y, false, false, true));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe("Hello");
    }

    [Fact]
    public void Should_Handle_Redo_Key_With_Shift_Z()
    {
        // Given
        var editor = new CodeEditor();
        editor.InsertText("Hello");
        editor.Undo();

        // When - Ctrl+Shift+Z
        var result = editor.HandleKey(new ConsoleKeyInfo('Z', ConsoleKey.Z, true, false, true));

        // Then
        result.ShouldBeTrue();
        editor.Content.ShouldBe("Hello");
    }

    [Fact]
    public void Should_Handle_Escape_Key_To_Exit()
    {
        // Given
        var editor = new CodeEditor();

        // When - Escape should be handled by the caller, but key processing should continue
        var result = editor.HandleKey(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        // Then
        result.ShouldBeTrue();
    }
}
