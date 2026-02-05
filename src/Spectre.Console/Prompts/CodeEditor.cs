namespace Spectre.Console;

/// <summary>
/// Represents a multi-line code editor prompt.
/// </summary>
public sealed class CodeEditor : IPrompt<string>
{
    private readonly string _prompt;

    /// <summary>
    /// Gets or sets the prompt style.
    /// </summary>
    public Style? PromptStyle { get; set; }

    /// <summary>
    /// Gets or sets the editor style.
    /// </summary>
    public Style? EditorStyle { get; set; }

    /// <summary>
    /// Gets or sets the number of visible lines.
    /// </summary>
    public int VisibleLines { get; set; } = 10;

    /// <summary>
    /// Gets or sets the initial text.
    /// </summary>
    public string InitialText { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeEditor"/> class.
    /// </summary>
    /// <param name="prompt">The prompt markup text.</param>
    public CodeEditor(string prompt)
    {
        _prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
    }

    /// <summary>
    /// Shows the prompt and requests input from the user.
    /// </summary>
    /// <param name="console">The console to show the prompt in.</param>
    /// <returns>The user input.</returns>
    public string Show(IAnsiConsole console)
    {
        return ShowAsync(console, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public async Task<string> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        return await console.RunExclusive(async () =>
        {
            var promptStyle = PromptStyle ?? Style.Plain;
            var editorStyle = EditorStyle ?? Style.Plain;
            var state = new CodeEditorState(InitialText);
            var undoStack = new CodeEditorUndoStack();
            var clipboardText = string.Empty;

            console.Markup(_prompt + "\n");

            while (true)
            {
                RenderEditor(console, state, editorStyle);

                var rawKey = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                if (rawKey == null)
                {
                    continue;
                }

                var key = rawKey.Value;

                if (key.Key == ConsoleKey.Enter && !key.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    SaveStateForUndo(state, undoStack);
                    state.InsertNewLine();
                    continue;
                }

                if (key.Key == ConsoleKey.Enter && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    console.WriteLine();
                    return state.GetText();
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    console.WriteLine();
                    return state.GetText();
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    state.MoveCursorUp();
                    continue;
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    state.MoveCursorDown();
                    continue;
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    state.MoveCursorLeft();
                    continue;
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    state.MoveCursorRight();
                    continue;
                }

                if (key.Key == ConsoleKey.Home)
                {
                    state.MoveCursorToStartOfLine();
                    continue;
                }

                if (key.Key == ConsoleKey.End)
                {
                    state.MoveCursorToEndOfLine();
                    continue;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    SaveStateForUndo(state, undoStack);
                    state.DeleteBackward();
                    continue;
                }

                if (key.Key == ConsoleKey.Delete)
                {
                    SaveStateForUndo(state, undoStack);
                    state.DeleteForward();
                    continue;
                }

                if (key.Key == ConsoleKey.Z && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    Undo(state, undoStack);
                    continue;
                }

                if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    clipboardText = state.GetSelectedText();
                    continue;
                }

                if (key.Key == ConsoleKey.V && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    SaveStateForUndo(state, undoStack);
                    state.PasteText(clipboardText);
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    SaveStateForUndo(state, undoStack);
                    state.InsertChar(key.KeyChar);
                    continue;
                }
            }
        }).ConfigureAwait(false);
    }

    private void SaveStateForUndo(CodeEditorState state, CodeEditorUndoStack undoStack)
    {
        undoStack.Push(state.GetText());
    }

    private void Undo(CodeEditorState state, CodeEditorUndoStack undoStack)
    {
        var previousState = undoStack.Pop();
        if (previousState != null)
        {
            var newState = new CodeEditorState(previousState);
            state.RestoreFrom(newState);
        }
    }

    private void RenderEditor(IAnsiConsole console, CodeEditorState state, Style editorStyle)
    {
        var cursorRow = state.CursorRow;
        var visibleLines = VisibleLines;
        var totalLines = state.LineCount;

        var startRow = Math.Max(0, cursorRow - visibleLines / 2);
        var endRow = Math.Min(totalLines, startRow + visibleLines);

        if (endRow - startRow < visibleLines && startRow > 0)
        {
            startRow = Math.Max(0, endRow - visibleLines);
        }

        console.Write("\u001b[2J\u001b[H");

        console.Markup(_prompt + "\n");

        for (int i = startRow; i < endRow; i++)
        {
            var line = state.Lines[i];
            var isCursorRow = i == cursorRow;
            var cursorColumn = state.CursorColumn;

            if (isCursorRow)
            {
                var beforeCursor = line.Substring(0, cursorColumn);
                var afterCursor = line.Substring(cursorColumn);

                console.Write(beforeCursor, editorStyle);
                console.Write("▌", new Style(foreground: Color.Green));
                console.Write(afterCursor, editorStyle);
            }
            else
            {
                console.Write(line, editorStyle);
            }

            console.WriteLine();
        }

        console.Markup("[grey]Ctrl+Enter: Submit | Ctrl+C: Copy | Ctrl+V: Paste | Ctrl+Z: Undo | Esc: Submit[/]");
    }
}
