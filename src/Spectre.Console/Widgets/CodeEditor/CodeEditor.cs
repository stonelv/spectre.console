namespace Spectre.Console;

public sealed class CodeEditor : IRenderable
{
    private List<string> _lines = new List<string> { string.Empty };
    private int _cursorLine = 0;
    private int _cursorColumn = 0;
    private readonly Stack<EditorState> _undoStack = new Stack<EditorState>();
    private readonly Stack<EditorState> _redoStack = new Stack<EditorState>();
    private const int MaxUndoSteps = 10;
    private bool _hasFocus = false;

    public int Width { get; set; } = 80;
    public int Height { get; set; } = 20;
    public Style? EditorStyle { get; set; }
    public Style? CursorStyle { get; set; }
    public bool ShowLineNumbers { get; set; } = true;

    public CodeEditor()
    {
        EditorStyle = Style.Plain;
        CursorStyle = new Style(foreground: Color.White, background: Color.Blue);
    }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return new Measurement(Width, Width);
    }

    public string GetText()
    {
        return string.Join(Environment.NewLine, _lines);
    }

    public void SetText(string text)
    {
        _lines = new List<string>(text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
        if (_lines.Count == 0)
        {
            _lines.Add(string.Empty);
        }
        _cursorLine = 0;
        _cursorColumn = 0;
        _undoStack.Clear();
        _redoStack.Clear();
    }

    public void Focus()
    {
        _hasFocus = true;
    }

    public void Blur()
    {
        _hasFocus = false;
    }

    public async Task<string> Show(IAnsiConsole console, CancellationToken cancellationToken = default)
    {
        if (console == null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        return await console.RunExclusive(async () =>
        {
            Focus();
            console.Clear();

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    console.Cursor.SetPosition(0, 0);
                    console.Write(this);

                    var key = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                    if (key == null)
                    {
                        continue;
                    }

                    var result = HandleKey(key.Value);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            finally
            {
                Blur();
            }
        }).ConfigureAwait(false);
    }

    private string? HandleKey(ConsoleKeyInfo key)
    {
        _redoStack.Clear();

        if (key.Modifiers == ConsoleModifiers.Control)
        {
            switch (key.Key)
            {
                case ConsoleKey.C:
                    CopyToClipboard();
                    return null;
                case ConsoleKey.V:
                    PasteFromClipboard();
                    return null;
                case ConsoleKey.Z:
                    Undo();
                    return null;
                case ConsoleKey.Y:
                    Redo();
                    return null;
            }
        }

        switch (key.Key)
        {
            case ConsoleKey.Enter:
                SaveState();
                InsertNewLine();
                break;

            case ConsoleKey.Backspace:
                SaveState();
                if (_cursorColumn > 0)
                {
                    _lines[_cursorLine] = _lines[_cursorLine].Remove(_cursorColumn - 1, 1);
                    _cursorColumn--;
                }
                else if (_cursorLine > 0)
                {
                    var currentLine = _lines[_cursorLine];
                    _lines.RemoveAt(_cursorLine);
                    _cursorLine--;
                    _cursorColumn = _lines[_cursorLine].Length;
                    _lines[_cursorLine] += currentLine;
                }
                break;

            case ConsoleKey.Delete:
                SaveState();
                if (_cursorColumn < _lines[_cursorLine].Length)
                {
                    _lines[_cursorLine] = _lines[_cursorLine].Remove(_cursorColumn, 1);
                }
                else if (_cursorLine < _lines.Count - 1)
                {
                    var nextLine = _lines[_cursorLine + 1];
                    _lines.RemoveAt(_cursorLine + 1);
                    _lines[_cursorLine] += nextLine;
                }
                break;

            case ConsoleKey.LeftArrow:
                if (_cursorColumn > 0)
                {
                    _cursorColumn--;
                }
                else if (_cursorLine > 0)
                {
                    _cursorLine--;
                    _cursorColumn = _lines[_cursorLine].Length;
                }
                break;

            case ConsoleKey.RightArrow:
                if (_cursorColumn < _lines[_cursorLine].Length)
                {
                    _cursorColumn++;
                }
                else if (_cursorLine < _lines.Count - 1)
                {
                    _cursorLine++;
                    _cursorColumn = 0;
                }
                break;

            case ConsoleKey.UpArrow:
                if (_cursorLine > 0)
                {
                    _cursorLine--;
                    _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
                }
                break;

            case ConsoleKey.DownArrow:
                if (_cursorLine < _lines.Count - 1)
                {
                    _cursorLine++;
                    _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
                }
                break;

            case ConsoleKey.Home:
                _cursorColumn = 0;
                break;

            case ConsoleKey.End:
                _cursorColumn = _lines[_cursorLine].Length;
                break;

            case ConsoleKey.Escape:
                return GetText();

            default:
                if (!char.IsControl(key.KeyChar))
                {
                    SaveState();
                    _lines[_cursorLine] = _lines[_cursorLine].Insert(_cursorColumn, key.KeyChar.ToString());
                    _cursorColumn++;
                }
                break;
        }

        return null;
    }

    private void InsertNewLine()
    {
        var currentLine = _lines[_cursorLine];
        var beforeCursor = currentLine.Substring(0, _cursorColumn);
        var afterCursor = currentLine.Substring(_cursorColumn);

        _lines[_cursorLine] = beforeCursor;
        _lines.Insert(_cursorLine + 1, afterCursor);

        _cursorLine++;
        _cursorColumn = 0;
    }

    private void SaveState()
    {
        var state = new EditorState
        {
            Lines = new List<string>(_lines),
            CursorLine = _cursorLine,
            CursorColumn = _cursorColumn
        };

        _undoStack.Push(state);

        if (_undoStack.Count > MaxUndoSteps)
        {
            var tempList = _undoStack.ToList();
            tempList.RemoveAt(tempList.Count - 1);
            _undoStack.Clear();
            foreach (var s in tempList.AsEnumerable().Reverse())
            {
                _undoStack.Push(s);
            }
        }
    }

    private void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }

        var currentState = new EditorState
        {
            Lines = new List<string>(_lines),
            CursorLine = _cursorLine,
            CursorColumn = _cursorColumn
        };
        _redoStack.Push(currentState);

        var previousState = _undoStack.Pop();
        _lines = previousState.Lines;
        _cursorLine = previousState.CursorLine;
        _cursorColumn = previousState.CursorColumn;
    }

    private void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }

        var currentState = new EditorState
        {
            Lines = new List<string>(_lines),
            CursorLine = _cursorLine,
            CursorColumn = _cursorColumn
        };
        _undoStack.Push(currentState);

        var nextState = _redoStack.Pop();
        _lines = nextState.Lines;
        _cursorLine = nextState.CursorLine;
        _cursorColumn = nextState.CursorColumn;
    }

    private void CopyToClipboard()
    {
        var text = GetText();
        try
        {
            Clipboard.SetText(text);
        }
        catch {}
    }

    private void PasteFromClipboard()
    {
        try
        {
            var text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                SaveState();
                var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (lines.Length > 0)
                {
                    _lines[_cursorLine] = _lines[_cursorLine].Insert(_cursorColumn, lines[0]);
                    _cursorColumn += lines[0].Length;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        _lines.Insert(_cursorLine + 1, lines[i]);
                        _cursorLine++;
                        _cursorColumn = lines[i].Length;
                    }
                }
            }
        }
        catch {}
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var result = new List<Segment>();
        var style = EditorStyle ?? Style.Plain;
        var cursorStyle = CursorStyle ?? new Style(foreground: Color.White, background: Color.Blue);

        int lineNumberWidth = ShowLineNumbers ? (_lines.Count.ToString().Length + 2) : 0;
        int contentWidth = (Width - lineNumberWidth);

        int startLine = Math.Max(0, _cursorLine - Height / 2);
        int endLine = Math.Min(_lines.Count, startLine + Height);

        for (int i = startLine; i < endLine; i++)
        {
            if (ShowLineNumbers)
            {
                var lineNumber = (i + 1).ToString().PadRight(lineNumberWidth);
                result.Add(new Segment(lineNumber, style));
            }

            string line = _lines[i];
            if (line.Length > contentWidth)
            {
                line = line.Substring(0, contentWidth);
            }

            if (i == _cursorLine && _hasFocus)
            {
                int cursorPosInLine = Math.Min(_cursorColumn, line.Length);

                if (cursorPosInLine > 0)
                {
                    result.Add(new Segment(line.Substring(0, cursorPosInLine), style));
                }

                if (cursorPosInLine < line.Length)
                {
                    result.Add(new Segment(line[cursorPosInLine].ToString(), cursorStyle));
                    result.Add(new Segment(line.Substring(cursorPosInLine + 1), style));
                }
                else
                {
                    result.Add(new Segment(" ", cursorStyle));
                }
            }
            else
            {
                result.Add(new Segment(line, style));
            }

            if (line.Length < contentWidth)
            {
                result.Add(new Segment(new string(' ', contentWidth - line.Length), style));
            }

            result.Add(Segment.LineBreak);
        }

        return result;
    }

    private class EditorState
    {
        public List<string> Lines { get; init; } = new List<string>();
        public int CursorLine { get; init; }
        public int CursorColumn { get; init; }
    }
}

public static class Clipboard
{
    public static void SetText(string text)
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "pbcopy",
                RedirectStandardInput = true,
                UseShellExecute = false
            }
        };
        process.Start();
        process.StandardInput.Write(text);
        process.StandardInput.Close();
        process.WaitForExit();
    }

    public static string GetText()
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "pbpaste",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };
        process.Start();
        string text = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return text;
    }
}
