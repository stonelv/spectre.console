namespace Spectre.Console;

public sealed class CodeEditor
{
    private readonly List<string> _lines = new() { "" };
    private readonly Stack<CodeEditorState> _undoStack = new();
    private readonly int _maxUndoSteps = 10;
    private int _cursorLeft;
    private int _cursorTop;
    private int _scrollOffset;
    private string? _clipboardText;
    private bool _isDirty = true;

    public string? Title { get; set; }
    public Style LineNumberStyle { get; set; } = Style.Parse("grey");
    public Style CursorStyle { get; set; } = Style.Parse("white on blue");
    public Style BorderStyle { get; set; } = Style.Parse("blue");
    public int? MaxHeight { get; set; }
    public int? MaxWidth { get; set; }

    public int CursorLeft
    {
        get => _cursorLeft;
        set => _cursorLeft = Math.Max(0, value);
    }

    public int CursorTop
    {
        get => _cursorTop;
        set => _cursorTop = Math.Clamp(value, 0, _lines.Count - 1);
    }

    public IReadOnlyList<string> Lines => _lines.AsReadOnly();

    public string Text => string.Join(Environment.NewLine, _lines);

    public CodeEditor()
    {
    }

    public CodeEditor(string initialText)
    {
        SetText(initialText);
    }

    public void SetText(string text)
    {
        _lines.Clear();
        if (string.IsNullOrEmpty(text))
        {
            _lines.Add("");
        }
        else
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            _lines.AddRange(lines);
        }
        _cursorLeft = 0;
        _cursorTop = 0;
        _isDirty = true;
    }

    public string Show(IAnsiConsole console)
    {
        return ShowAsync(console, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task<string> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken = default)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (!console.Profile.Capabilities.Interactive)
        {
            throw new NotSupportedException("Cannot show code editor in non-interactive mode.");
        }

        _undoStack.Clear();
        SaveState();

        var liveDisplay = new LiveDisplay(console, new CodeEditorRenderable(this));
        return await liveDisplay.StartAsync(async context =>
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_isDirty)
                {
                    context.UpdateTarget(new CodeEditorRenderable(this));
                    _isDirty = false;
                }

                var key = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                if (key == null)
                {
                    continue;
                }

                var result = HandleKey(key.Value, console);
                if (result != null)
                {
                    return result;
                }

                _isDirty = true;
            }
        }).ConfigureAwait(false);
    }

    private string? HandleKey(ConsoleKeyInfo key, IAnsiConsole console)
    {
        if (key.Modifiers == ConsoleModifiers.Control)
        {
            switch (key.Key)
            {
                case ConsoleKey.C:
                    Copy();
                    console.WriteLine("Copied to clipboard");
                    return null;
                case ConsoleKey.V:
                    Paste();
                    return null;
                case ConsoleKey.Z:
                    Undo();
                    return null;
                case ConsoleKey.D:
                    return Text;
            }
        }

        if (key.Key == ConsoleKey.F5)
        {
            Undo();
            return null;
        }

        switch (key.Key)
        {
            case ConsoleKey.Escape:
                return Text;

            case ConsoleKey.Enter:
                SaveState();
                InsertNewLine();
                return null;

            case ConsoleKey.Backspace:
                SaveState();
                HandleBackspace();
                return null;

            case ConsoleKey.Delete:
                SaveState();
                HandleDelete();
                return null;

            case ConsoleKey.LeftArrow:
                MoveLeft();
                return null;

            case ConsoleKey.RightArrow:
                MoveRight();
                return null;

            case ConsoleKey.UpArrow:
                MoveUp();
                return null;

            case ConsoleKey.DownArrow:
                MoveDown();
                return null;

            case ConsoleKey.Home:
                _cursorLeft = 0;
                return null;

            case ConsoleKey.End:
                _cursorLeft = _lines[_cursorTop].Length;
                return null;

            case ConsoleKey.PageUp:
                _cursorTop = Math.Max(0, _cursorTop - 10);
                EnsureCursorVisible();
                return null;

            case ConsoleKey.PageDown:
                _cursorTop = Math.Min(_lines.Count - 1, _cursorTop + 10);
                EnsureCursorVisible();
                return null;
        }

        if (!char.IsControl(key.KeyChar))
        {
            SaveState();
            InsertChar(key.KeyChar);
        }

        return null;
    }

    private void InsertChar(char c)
    {
        var line = _lines[_cursorTop];
        if (_cursorLeft >= line.Length)
        {
            _lines[_cursorTop] = line + c;
        }
        else
        {
            _lines[_cursorTop] = line.Insert(_cursorLeft, c.ToString());
        }
        _cursorLeft++;
    }

    private void InsertNewLine()
    {
        var currentLine = _lines[_cursorTop];
        var leftPart = currentLine.Substring(0, _cursorLeft);
        var rightPart = currentLine.Substring(_cursorLeft);

        _lines[_cursorTop] = leftPart;
        _lines.Insert(_cursorTop + 1, rightPart);
        _cursorTop++;
        _cursorLeft = 0;
    }

    private void HandleBackspace()
    {
        if (_cursorLeft > 0)
        {
            var line = _lines[_cursorTop];
            _lines[_cursorTop] = line.Remove(_cursorLeft - 1, 1);
            _cursorLeft--;
        }
        else if (_cursorTop > 0)
        {
            var currentLine = _lines[_cursorTop];
            var prevLine = _lines[_cursorTop - 1];
            _cursorLeft = prevLine.Length;
            _lines[_cursorTop - 1] = prevLine + currentLine;
            _lines.RemoveAt(_cursorTop);
            _cursorTop--;
        }
    }

    private void HandleDelete()
    {
        var line = _lines[_cursorTop];
        if (_cursorLeft < line.Length)
        {
            _lines[_cursorTop] = line.Remove(_cursorLeft, 1);
        }
        else if (_cursorTop < _lines.Count - 1)
        {
            var nextLine = _lines[_cursorTop + 1];
            _lines[_cursorTop] = line + nextLine;
            _lines.RemoveAt(_cursorTop + 1);
        }
    }

    private void MoveLeft()
    {
        if (_cursorLeft > 0)
        {
            _cursorLeft--;
        }
        else if (_cursorTop > 0)
        {
            _cursorTop--;
            _cursorLeft = _lines[_cursorTop].Length;
        }
    }

    private void MoveRight()
    {
        var line = _lines[_cursorTop];
        if (_cursorLeft < line.Length)
        {
            _cursorLeft++;
        }
        else if (_cursorTop < _lines.Count - 1)
        {
            _cursorTop++;
            _cursorLeft = 0;
        }
    }

    private void MoveUp()
    {
        if (_cursorTop > 0)
        {
            _cursorTop--;
            _cursorLeft = Math.Min(_cursorLeft, _lines[_cursorTop].Length);
            EnsureCursorVisible();
        }
    }

    private void MoveDown()
    {
        if (_cursorTop < _lines.Count - 1)
        {
            _cursorTop++;
            _cursorLeft = Math.Min(_cursorLeft, _lines[_cursorTop].Length);
            EnsureCursorVisible();
        }
    }

    private void EnsureCursorVisible()
    {
        var visibleHeight = MaxHeight ?? 20;
        if (_cursorTop < _scrollOffset)
        {
            _scrollOffset = _cursorTop;
        }
        else if (_cursorTop >= _scrollOffset + visibleHeight - 2)
        {
            _scrollOffset = _cursorTop - visibleHeight + 3;
        }
    }

    private void Copy()
    {
        _clipboardText = _lines[_cursorTop];
    }

    private void Paste()
    {
        if (string.IsNullOrEmpty(_clipboardText))
        {
            var systemClipboard = GetSystemClipboard();
            if (!string.IsNullOrEmpty(systemClipboard))
            {
                _clipboardText = systemClipboard;
            }
        }

        if (!string.IsNullOrEmpty(_clipboardText))
        {
            SaveState();
            InsertText(_clipboardText);
        }
    }

    private void InsertText(string text)
    {
        var linesToInsert = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        if (linesToInsert.Length == 1)
        {
            var line = _lines[_cursorTop];
            _lines[_cursorTop] = line.Insert(_cursorLeft, linesToInsert[0]);
            _cursorLeft += linesToInsert[0].Length;
        }
        else
        {
            var currentLine = _lines[_cursorTop];
            var leftPart = currentLine.Substring(0, _cursorLeft);
            var rightPart = currentLine.Substring(_cursorLeft);

            _lines[_cursorTop] = leftPart + linesToInsert[0];

            for (var i = 1; i < linesToInsert.Length - 1; i++)
            {
                _lines.Insert(_cursorTop + i, linesToInsert[i]);
            }

            _lines.Insert(_cursorTop + linesToInsert.Length - 1, linesToInsert[^1] + rightPart);
            _cursorTop += linesToInsert.Length - 1;
            _cursorLeft = linesToInsert[^1].Length;
        }
    }

    private string? GetSystemClipboard()
    {
        try
        {
            if (OperatingSystem.IsMacOS())
            {
                using var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "pbpaste",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                return process.StandardOutput.ReadToEnd();
            }
            else if (OperatingSystem.IsLinux())
            {
                using var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "xclip",
                        Arguments = "-selection clipboard -o",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                return process.StandardOutput.ReadToEnd();
            }
        }
        catch
        {
        }
        return null;
    }

    private void SaveState()
    {
        var state = new CodeEditorState(
            _lines.ToList(),
            _cursorLeft,
            _cursorTop,
            _scrollOffset);

        _undoStack.Push(state);

        if (_undoStack.Count > _maxUndoSteps)
        {
            var trimmedList = _undoStack.Reverse().Take(_maxUndoSteps).ToList();
            _undoStack.Clear();
            foreach (var s in trimmedList)
            {
                _undoStack.Push(s);
            }
        }
    }

    private void Undo()
    {
        if (_undoStack.Count > 1)
        {
            _undoStack.Pop();
            var state = _undoStack.Peek();
            _lines.Clear();
            _lines.AddRange(state.Lines);
            _cursorLeft = state.CursorLeft;
            _cursorTop = state.CursorTop;
            _scrollOffset = state.ScrollOffset;
        }
    }

    internal (int Left, int Top) GetRenderPosition(int viewportWidth)
    {
        var lineNumberWidth = _lines.Count.ToString().Length + 1;
        var visibleLeft = _cursorLeft;
        if (visibleLeft > viewportWidth - lineNumberWidth - 4)
        {
            visibleLeft = viewportWidth - lineNumberWidth - 4;
        }
        return (visibleLeft, _cursorTop - _scrollOffset + 1);
    }

    internal List<string> GetVisibleLines(int maxHeight, int maxWidth)
    {
        var lineNumberWidth = _lines.Count.ToString().Length + 1;
        var availableWidth = maxWidth - lineNumberWidth - 3;
        var visibleCount = maxHeight - 2;

        var result = new List<string>();
        for (var i = _scrollOffset; i < Math.Min(_scrollOffset + visibleCount, _lines.Count); i++)
        {
            var line = _lines[i];
            if (line.Length > availableWidth)
            {
                line = line.Substring(0, availableWidth);
            }
            result.Add(line);
        }
        return result;
    }

    private sealed class CodeEditorState
    {
        public List<string> Lines { get; }
        public int CursorLeft { get; }
        public int CursorTop { get; }
        public int ScrollOffset { get; }

        public CodeEditorState(List<string> lines, int cursorLeft, int cursorTop, int scrollOffset)
        {
            Lines = lines;
            CursorLeft = cursorLeft;
            CursorTop = cursorTop;
            ScrollOffset = scrollOffset;
        }
    }

    private sealed class CodeEditorRenderable : IRenderable
    {
        private readonly CodeEditor _editor;

        public CodeEditorRenderable(CodeEditor editor)
        {
            _editor = editor;
        }

        public Measurement Measure(RenderOptions options, int maxWidth)
        {
            var width = _editor.MaxWidth ?? maxWidth;
            return new Measurement(Math.Min(width, maxWidth), Math.Min(width, maxWidth));
        }

        public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
        {
            var width = Math.Min(_editor.MaxWidth ?? maxWidth, maxWidth);
            var height = _editor.MaxHeight ?? 20;
            var lineNumberWidth = _editor.Lines.Count.ToString().Length + 1;

            var segments = new List<Segment>();

            if (!string.IsNullOrEmpty(_editor.Title))
            {
                segments.Add(new Segment("┌─" + _editor.Title + new string('─', width - _editor.Title.Length - 3) + "┐\n", _editor.BorderStyle));
            }
            else
            {
                segments.Add(new Segment("┌" + new string('─', width - 2) + "┐\n", _editor.BorderStyle));
            }

            var visibleLines = _editor.GetVisibleLines(height, width);
            var lineIndex = _editor._scrollOffset;

            foreach (var line in visibleLines)
            {
                segments.Add(new Segment("│ ", _editor.BorderStyle));
                segments.Add(new Segment((lineIndex + 1).ToString().PadLeft(lineNumberWidth - 1) + " ", _editor.LineNumberStyle));
                segments.Add(new Segment("│ ", _editor.BorderStyle));

                var displayLine = line.PadRight(width - lineNumberWidth - 4);

                if (lineIndex == _editor._cursorTop)
                {
                    var cursorPos = Math.Min(_editor._cursorLeft, displayLine.Length - 1);
                    if (cursorPos >= 0 && cursorPos < displayLine.Length)
                    {
                        var beforeCursor = displayLine.Substring(0, cursorPos);
                        var cursorChar = cursorPos < line.Length ? displayLine[cursorPos] : ' ';
                        var afterCursor = cursorPos + 1 < displayLine.Length ? displayLine.Substring(cursorPos + 1) : "";

                        if (!string.IsNullOrEmpty(beforeCursor))
                        {
                            segments.Add(new Segment(beforeCursor));
                        }
                        segments.Add(new Segment(cursorChar.ToString(), _editor.CursorStyle));
                        if (!string.IsNullOrEmpty(afterCursor))
                        {
                            segments.Add(new Segment(afterCursor));
                        }
                    }
                    else
                    {
                        segments.Add(new Segment(displayLine));
                    }
                }
                else
                {
                    segments.Add(new Segment(displayLine));
                }

                segments.Add(new Segment(" │\n", _editor.BorderStyle));
                lineIndex++;
            }

            for (var i = visibleLines.Count; i < height - 2; i++)
            {
                segments.Add(new Segment("│ ", _editor.BorderStyle));
                segments.Add(new Segment(new string(' ', lineNumberWidth), _editor.LineNumberStyle));
                segments.Add(new Segment("│ ", _editor.BorderStyle));
                segments.Add(new Segment(new string(' ', width - lineNumberWidth - 4)));
                segments.Add(new Segment(" │\n", _editor.BorderStyle));
            }

            segments.Add(new Segment("└" + new string('─', width - 2) + "┘\n", _editor.BorderStyle));
            segments.Add(new Segment("按 Ctrl+D 或 ESC 确认退出并返回文本内容\n", Style.Parse("grey")));

            return segments;
        }
    }
}
