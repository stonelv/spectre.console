namespace Spectre.Console;

/// <summary>
/// Represents a multi-line code editor component.
/// </summary>
public sealed class CodeEditor : Renderable
{
    private readonly List<string> _lines;
    private readonly UndoManager _undoManager;
    private int _cursorLine;
    private int _cursorColumn;
    private int _viewportTop;
    private int _viewportLeft;

    /// <summary>
    /// Gets or sets the width of the editor.
    /// </summary>
    public int Width { get; set; } = 80;

    /// <summary>
    /// Gets or sets the height of the editor.
    /// </summary>
    public int Height { get; set; } = 24;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public Style BorderStyle { get; set; } = Style.Plain;

    /// <summary>
    /// Gets or sets the text style.
    /// </summary>
    public Style TextStyle { get; set; } = Style.Plain;

    /// <summary>
    /// Gets or sets the cursor style.
    /// </summary>
    public Style CursorStyle { get; set; } = new Style(foreground: Color.Black, background: Color.White);

    /// <summary>
    /// Gets or sets the line number style.
    /// </summary>
    public Style LineNumberStyle { get; set; } = Style.Plain;

    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    public bool ShowLineNumbers { get; set; } = true;

    /// <summary>
    /// Gets the current cursor line.
    /// </summary>
    public int CursorLine => _cursorLine;

    /// <summary>
    /// Gets the current cursor column.
    /// </summary>
    public int CursorColumn => _cursorColumn;

    /// <summary>
    /// Gets the content of the editor.
    /// </summary>
    public string Content => string.Join(Environment.NewLine, _lines);

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeEditor"/> class.
    /// </summary>
    public CodeEditor()
    {
        _lines = new List<string> { string.Empty };
        _undoManager = new UndoManager(10);
        _cursorLine = 0;
        _cursorColumn = 0;
        _viewportTop = 0;
        _viewportLeft = 0;
        SaveState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeEditor"/> class with initial content.
    /// </summary>
    /// <param name="content">The initial content.</param>
    public CodeEditor(string content)
    {
        _lines = new List<string> { string.Empty };
        _undoManager = new UndoManager(10);
        _viewportTop = 0;
        _viewportLeft = 0;

        if (!string.IsNullOrEmpty(content))
        {
            _lines.Clear();
            _lines.AddRange(content.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None));
        }

        // Set cursor to the end of the last line
        _cursorLine = _lines.Count - 1;
        _cursorColumn = _lines[_cursorLine].Length;

        SaveState();
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        return new Measurement(Math.Min(Width, maxWidth), Math.Min(Width, maxWidth));
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var segments = new List<Segment>();
        var lineNumberWidth = ShowLineNumbers ? GetLineNumberWidth() + 2 : 0;
        var contentWidth = Math.Min(Width, maxWidth) - lineNumberWidth - 2;

        // Top border
        segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.TopLeft), BorderStyle));
        segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.Top).Repeat(Math.Min(Width, maxWidth) - 2), BorderStyle));
        segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.TopRight), BorderStyle));
        segments.Add(Segment.LineBreak);

        // Content lines
        var visibleLines = Math.Min(Height - 2, _lines.Count - _viewportTop);
        for (var i = 0; i < Height - 2; i++)
        {
            var lineIndex = _viewportTop + i;
            var isCursorLine = lineIndex == _cursorLine;

            // Left border
            segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.Left), BorderStyle));

            if (lineIndex < _lines.Count)
            {
                var line = _lines[lineIndex];

                // Line number
                if (ShowLineNumbers)
                {
                    var lineNumber = (lineIndex + 1).ToString().PadLeft(GetLineNumberWidth());
                    segments.Add(new Segment(lineNumber, LineNumberStyle));
                    segments.Add(new Segment(" ", LineNumberStyle));
                    segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.Left), LineNumberStyle));
                }

                // Content
                var visibleLine = GetVisibleLineContent(line, contentWidth);
                for (var j = 0; j < visibleLine.Length; j++)
                {
                    var charIndex = _viewportLeft + j;
                    var isCursorPosition = isCursorLine && charIndex == _cursorColumn;
                    var style = isCursorPosition ? CursorStyle : TextStyle;
                    segments.Add(new Segment(visibleLine[j].ToString(), style));
                }

                // Fill remaining space
                var remainingWidth = contentWidth - visibleLine.Length;
                if (remainingWidth > 0)
                {
                    // Show cursor at end of line if this is the cursor line and cursor is at the end
                    if (isCursorLine && _cursorColumn >= line.Length)
                    {
                        segments.Add(new Segment(" ", CursorStyle));
                        remainingWidth--;
                    }

                    if (remainingWidth > 0)
                    {
                        segments.Add(new Segment(" ".Repeat(remainingWidth), TextStyle));
                    }
                }
            }
            else
            {
                // Empty line
                var emptyWidth = contentWidth + (ShowLineNumbers ? GetLineNumberWidth() + 2 : 0);
                segments.Add(new Segment(" ".Repeat(emptyWidth), TextStyle));
            }

            // Right border
            segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.Right), BorderStyle));
            segments.Add(Segment.LineBreak);
        }

        // Bottom border
        segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.BottomLeft), BorderStyle));
        segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.Bottom).Repeat(Math.Min(Width, maxWidth) - 2), BorderStyle));
        segments.Add(new Segment(BoxBorder.Square.GetPart(BoxBorderPart.BottomRight), BorderStyle));
        segments.Add(Segment.LineBreak);

        return segments;
    }

    /// <summary>
    /// Handles a key press event.
    /// </summary>
    /// <param name="key">The key information.</param>
    /// <returns>True if the editor should continue running; otherwise, false.</returns>
    public bool HandleKey(ConsoleKeyInfo key)
    {
        var isCtrl = (key.Modifiers & ConsoleModifiers.Control) != 0;
        var isShift = (key.Modifiers & ConsoleModifiers.Shift) != 0;
        var isAlt = (key.Modifiers & ConsoleModifiers.Alt) != 0;

        // Alt+Z - Undo (macOS compatible)
        if (isAlt && key.Key == ConsoleKey.Z && !isShift)
        {
            Undo();
            return true;
        }

        // Alt+Y or Alt+Shift+Z - Redo (macOS compatible)
        if ((isAlt && key.Key == ConsoleKey.Y) || (isAlt && isShift && key.Key == ConsoleKey.Z))
        {
            Redo();
            return true;
        }

        // Alt+C - Copy (macOS compatible)
        if (isAlt && key.Key == ConsoleKey.C)
        {
            CopyToClipboard();
            return true;
        }

        // Alt+V - Paste (macOS compatible)
        if (isAlt && key.Key == ConsoleKey.V)
        {
            PasteFromClipboard();
            return true;
        }

        // Ctrl+C - Copy (Windows/Linux)
        if (isCtrl && key.Key == ConsoleKey.C)
        {
            CopyToClipboard();
            return true;
        }

        // Ctrl+V - Paste (Windows/Linux)
        if (isCtrl && key.Key == ConsoleKey.V)
        {
            PasteFromClipboard();
            return true;
        }

        // Ctrl+Z - Undo (Windows/Linux)
        if (isCtrl && key.Key == ConsoleKey.Z && !isShift)
        {
            Undo();
            return true;
        }

        // Ctrl+Y or Ctrl+Shift+Z - Redo (Windows/Linux)
        if ((isCtrl && key.Key == ConsoleKey.Y) || (isCtrl && isShift && key.Key == ConsoleKey.Z))
        {
            Redo();
            return true;
        }

        // Enter - New line
        if (key.Key == ConsoleKey.Enter)
        {
            InsertNewLine();
            return true;
        }

        // Backspace
        if (key.Key == ConsoleKey.Backspace)
        {
            HandleBackspace();
            return true;
        }

        // Delete
        if (key.Key == ConsoleKey.Delete)
        {
            HandleDelete();
            return true;
        }

        // Arrow keys
        if (key.Key == ConsoleKey.UpArrow)
        {
            MoveCursorUp();
            return true;
        }

        if (key.Key == ConsoleKey.DownArrow)
        {
            MoveCursorDown();
            return true;
        }

        if (key.Key == ConsoleKey.LeftArrow)
        {
            MoveCursorLeft();
            return true;
        }

        if (key.Key == ConsoleKey.RightArrow)
        {
            MoveCursorRight();
            return true;
        }

        if (key.Key == ConsoleKey.Home)
        {
            _cursorColumn = 0;
            UpdateViewport();
            return true;
        }

        if (key.Key == ConsoleKey.End)
        {
            _cursorColumn = _lines[_cursorLine].Length;
            UpdateViewport();
            return true;
        }

        // Tab
        if (key.Key == ConsoleKey.Tab)
        {
            InsertText("    ");
            return true;
        }

        // Regular character input
        if (!char.IsControl(key.KeyChar))
        {
            InsertText(key.KeyChar.ToString());
            return true;
        }

        return true;
    }

    /// <summary>
    /// Inserts text at the current cursor position.
    /// </summary>
    /// <param name="text">The text to insert.</param>
    public void InsertText(string text)
    {
        SaveState();

        var lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        var currentLine = _lines[_cursorLine];

        if (lines.Length == 1)
        {
            // Single line insertion
            _lines[_cursorLine] = currentLine.Insert(_cursorColumn, text);
            _cursorColumn += text.Length;
        }
        else
        {
            // Multi-line insertion
            var beforeCursor = currentLine.Substring(0, _cursorColumn);
            var afterCursor = currentLine.Substring(_cursorColumn);

            _lines[_cursorLine] = beforeCursor + lines[0];

            for (var i = 1; i < lines.Length - 1; i++)
            {
                _cursorLine++;
                _lines.Insert(_cursorLine, lines[i]);
            }

            _cursorLine++;
            _lines.Insert(_cursorLine, lines[lines.Length - 1] + afterCursor);
            _cursorColumn = lines[lines.Length - 1].Length;
        }

        UpdateViewport();
    }

    /// <summary>
    /// Inserts a new line at the current cursor position.
    /// </summary>
    public void InsertNewLine()
    {
        SaveState();

        var currentLine = _lines[_cursorLine];
        var beforeCursor = currentLine.Substring(0, _cursorColumn);
        var afterCursor = currentLine.Substring(_cursorColumn);

        _lines[_cursorLine] = beforeCursor;
        _cursorLine++;
        _lines.Insert(_cursorLine, afterCursor);
        _cursorColumn = 0;

        UpdateViewport();
    }

    /// <summary>
    /// Handles the backspace key.
    /// </summary>
    public void HandleBackspace()
    {
        if (_cursorColumn > 0)
        {
            SaveState();
            var currentLine = _lines[_cursorLine];
            _lines[_cursorLine] = currentLine.Remove(_cursorColumn - 1, 1);
            _cursorColumn--;
            UpdateViewport();
        }
        else if (_cursorLine > 0)
        {
            SaveState();
            var currentLine = _lines[_cursorLine];
            var previousLine = _lines[_cursorLine - 1];
            _cursorColumn = previousLine.Length;
            _lines[_cursorLine - 1] = previousLine + currentLine;
            _lines.RemoveAt(_cursorLine);
            _cursorLine--;
            UpdateViewport();
        }
    }

    /// <summary>
    /// Handles the delete key.
    /// </summary>
    public void HandleDelete()
    {
        var currentLine = _lines[_cursorLine];

        if (_cursorColumn < currentLine.Length)
        {
            SaveState();
            _lines[_cursorLine] = currentLine.Remove(_cursorColumn, 1);
            UpdateViewport();
        }
        else if (_cursorLine < _lines.Count - 1)
        {
            SaveState();
            var nextLine = _lines[_cursorLine + 1];
            _lines[_cursorLine] = currentLine + nextLine;
            _lines.RemoveAt(_cursorLine + 1);
            UpdateViewport();
        }
    }

    /// <summary>
    /// Moves the cursor up.
    /// </summary>
    public void MoveCursorUp()
    {
        if (_cursorLine > 0)
        {
            _cursorLine--;
            _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
            UpdateViewport();
        }
    }

    /// <summary>
    /// Moves the cursor down.
    /// </summary>
    public void MoveCursorDown()
    {
        if (_cursorLine < _lines.Count - 1)
        {
            _cursorLine++;
            _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
            UpdateViewport();
        }
    }

    /// <summary>
    /// Moves the cursor left.
    /// </summary>
    public void MoveCursorLeft()
    {
        if (_cursorColumn > 0)
        {
            _cursorColumn--;
            UpdateViewport();
        }
        else if (_cursorLine > 0)
        {
            _cursorLine--;
            _cursorColumn = _lines[_cursorLine].Length;
            UpdateViewport();
        }
    }

    /// <summary>
    /// Moves the cursor right.
    /// </summary>
    public void MoveCursorRight()
    {
        if (_cursorColumn < _lines[_cursorLine].Length)
        {
            _cursorColumn++;
            UpdateViewport();
        }
        else if (_cursorLine < _lines.Count - 1)
        {
            _cursorLine++;
            _cursorColumn = 0;
            UpdateViewport();
        }
    }

    /// <summary>
    /// Copies the current line to the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
        var text = _lines[_cursorLine];
        try
        {
            TextCopy.ClipboardService.SetText(text);
        }
        catch
        {
            // Clipboard not available, ignore
        }
    }

    /// <summary>
    /// Pastes text from the clipboard.
    /// </summary>
    public void PasteFromClipboard()
    {
        try
        {
            var text = TextCopy.ClipboardService.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                InsertText(text);
            }
        }
        catch
        {
            // Clipboard not available, ignore
        }
    }

    /// <summary>
    /// Undoes the last operation.
    /// </summary>
    public void Undo()
    {
        var state = _undoManager.Undo();
        if (state != null)
        {
            RestoreState(state);
        }
    }

    /// <summary>
    /// Redoes the last undone operation.
    /// </summary>
    public void Redo()
    {
        var state = _undoManager.Redo();
        if (state != null)
        {
            RestoreState(state);
        }
    }

    /// <summary>
    /// Runs the code editor interactively.
    /// </summary>
    /// <param name="console">The console to use.</param>
    /// <returns>The final content of the editor.</returns>
    public string Run(IAnsiConsole console)
    {
        return RunAsync(console, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Runs the code editor interactively asynchronously.
    /// </summary>
    /// <param name="console">The console to use.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The final content of the editor.</returns>
    public async Task<string> RunAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        return await console.RunExclusive(async () =>
        {
            console.Clear();
            console.Write(this);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var rawKey = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                if (rawKey == null)
                {
                    continue;
                }

                var key = rawKey.Value;

                // Escape to exit
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }

                // Handle the key
                if (HandleKey(key))
                {
                    // Redraw
                    console.Clear();
                    console.Write(this);
                }
            }

            return Content;
        }).ConfigureAwait(false);
    }

    private void SaveState()
    {
        var state = new EditorState
        {
            Lines = _lines.ToList(), // Create a copy of the lines list
            CursorLine = _cursorLine,
            CursorColumn = _cursorColumn,
        };
        _undoManager.SaveState(state);
    }

    private void RestoreState(EditorState state)
    {
        _lines.Clear();
        _lines.AddRange(state.Lines.ToList()); // Create a copy to avoid reference issues
        _cursorLine = state.CursorLine;
        _cursorColumn = state.CursorColumn;
        UpdateViewport();
    }

    private void UpdateViewport()
    {
        // Update vertical viewport
        if (_cursorLine < _viewportTop)
        {
            _viewportTop = _cursorLine;
        }
        else if (_cursorLine >= _viewportTop + Height - 2)
        {
            _viewportTop = _cursorLine - (Height - 3);
        }

        // Update horizontal viewport
        var lineNumberWidth = ShowLineNumbers ? GetLineNumberWidth() + 2 : 0;
        var contentWidth = Width - lineNumberWidth - 2;

        if (_cursorColumn < _viewportLeft)
        {
            _viewportLeft = _cursorColumn;
        }
        else if (_cursorColumn >= _viewportLeft + contentWidth)
        {
            _viewportLeft = _cursorColumn - contentWidth + 1;
        }
    }

    private int GetLineNumberWidth()
    {
        return _lines.Count.ToString().Length;
    }

    private string GetVisibleLineContent(string line, int maxWidth)
    {
        if (_viewportLeft >= line.Length)
        {
            return string.Empty;
        }

        var start = _viewportLeft;
        var length = Math.Min(maxWidth, line.Length - start);
        return line.Substring(start, length);
    }

    private class EditorState
    {
        public List<string> Lines { get; set; } = new();
        public int CursorLine { get; set; }
        public int CursorColumn { get; set; }
    }

    private class UndoManager
    {
        private readonly LinkedList<EditorState> _undoStack;
        private readonly LinkedList<EditorState> _redoStack;
        private readonly int _maxSize;

        public UndoManager(int maxSize)
        {
            _undoStack = new LinkedList<EditorState>();
            _redoStack = new LinkedList<EditorState>();
            _maxSize = maxSize;
        }

        public void SaveState(EditorState state)
        {
            _undoStack.AddLast(state);
            _redoStack.Clear();

            if (_undoStack.Count > _maxSize)
            {
                _undoStack.RemoveFirst();
            }
        }

        public EditorState? Undo()
        {
            if (_undoStack.Count <= 1)
            {
                return null;
            }

            // Pop current state and push to redo stack
            var currentState = _undoStack.Last!.Value;
            _undoStack.RemoveLast();
            _redoStack.AddLast(currentState);

            // Return the previous state (new top of undo stack)
            return _undoStack.Last!.Value;
        }

        public EditorState? Redo()
        {
            if (_redoStack.Count == 0)
            {
                return null;
            }

            // Pop state from redo stack
            var state = _redoStack.Last!.Value;
            _redoStack.RemoveLast();
            
            // Push to undo stack
            _undoStack.AddLast(state);
            
            return state;
        }
    }
}
