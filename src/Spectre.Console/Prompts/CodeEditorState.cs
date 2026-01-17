namespace Spectre.Console;

internal sealed class CodeEditorState
{
    private readonly List<string> _lines;
    private int _cursorRow;
    private int _cursorColumn;

    public IReadOnlyList<string> Lines => _lines.AsReadOnly();
    public int CursorRow => _cursorRow;
    public int CursorColumn => _cursorColumn;
    public int LineCount => _lines.Count;

    public CodeEditorState(string initialText = "")
    {
        _lines = string.IsNullOrEmpty(initialText) 
            ? new List<string> { "" } 
            : initialText.Split('\n').ToList();
        _cursorRow = 0;
        _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorRow].Length);
    }

    public string GetText()
    {
        return string.Join('\n', _lines);
    }

    public void InsertChar(char c)
    {
        var line = _lines[_cursorRow];
        _lines[_cursorRow] = line.Insert(_cursorColumn, c.ToString());
        _cursorColumn++;
    }

    public void InsertNewLine()
    {
        var line = _lines[_cursorRow];
        var beforeCursor = line.Substring(0, _cursorColumn);
        var afterCursor = line.Substring(_cursorColumn);

        _lines[_cursorRow] = beforeCursor;
        _lines.Insert(_cursorRow + 1, afterCursor);

        _cursorRow++;
        _cursorColumn = 0;
    }

    public void DeleteBackward()
    {
        if (_cursorColumn > 0)
        {
            var line = _lines[_cursorRow];
            _lines[_cursorRow] = line.Remove(_cursorColumn - 1, 1);
            _cursorColumn--;
        }
        else if (_cursorRow > 0)
        {
            var currentLine = _lines[_cursorRow];
            var previousLine = _lines[_cursorRow - 1];
            _lines[_cursorRow - 1] = previousLine + currentLine;
            _lines.RemoveAt(_cursorRow);
            _cursorRow--;
            _cursorColumn = previousLine.Length;
        }
    }

    public void DeleteForward()
    {
        if (_cursorColumn < _lines[_cursorRow].Length)
        {
            var line = _lines[_cursorRow];
            _lines[_cursorRow] = line.Remove(_cursorColumn, 1);
        }
        else if (_cursorRow < _lines.Count - 1)
        {
            var currentLine = _lines[_cursorRow];
            var nextLine = _lines[_cursorRow + 1];
            _lines[_cursorRow] = currentLine + nextLine;
            _lines.RemoveAt(_cursorRow + 1);
        }
    }

    public void MoveCursorUp()
    {
        if (_cursorRow > 0)
        {
            _cursorRow--;
            _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorRow].Length);
        }
    }

    public void MoveCursorDown()
    {
        if (_cursorRow < _lines.Count - 1)
        {
            _cursorRow++;
            _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorRow].Length);
        }
    }

    public void MoveCursorLeft()
    {
        if (_cursorColumn > 0)
        {
            _cursorColumn--;
        }
        else if (_cursorRow > 0)
        {
            _cursorRow--;
            _cursorColumn = _lines[_cursorRow].Length;
        }
    }

    public void MoveCursorRight()
    {
        if (_cursorColumn < _lines[_cursorRow].Length)
        {
            _cursorColumn++;
        }
        else if (_cursorRow < _lines.Count - 1)
        {
            _cursorRow++;
            _cursorColumn = 0;
        }
    }

    public void MoveCursorToStartOfLine()
    {
        _cursorColumn = 0;
    }

    public void MoveCursorToEndOfLine()
    {
        _cursorColumn = _lines[_cursorRow].Length;
    }

    public void PasteText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var lines = text.Split('\n');
        if (lines.Length == 1)
        {
            var line = _lines[_cursorRow];
            _lines[_cursorRow] = line.Insert(_cursorColumn, lines[0]);
            _cursorColumn += lines[0].Length;
        }
        else
        {
            var currentLine = _lines[_cursorRow];
            var beforeCursor = currentLine.Substring(0, _cursorColumn);
            var afterCursor = currentLine.Substring(_cursorColumn);

            _lines[_cursorRow] = beforeCursor + lines[0];
            for (int i = 1; i < lines.Length - 1; i++)
            {
                _lines.Insert(_cursorRow + i, lines[i]);
            }
            _lines.Insert(_cursorRow + lines.Length - 1, lines[lines.Length - 1] + afterCursor);

            _cursorRow += lines.Length - 1;
            _cursorColumn = lines[lines.Length - 1].Length;
        }
    }

    public string GetSelectedText()
    {
        return _lines[_cursorRow];
    }

    public CodeEditorState Clone()
    {
        var clone = new CodeEditorState();
        clone._lines.Clear();
        clone._lines.AddRange(_lines);
        clone._cursorRow = _cursorRow;
        clone._cursorColumn = _cursorColumn;
        return clone;
    }

    public void RestoreFrom(CodeEditorState state)
    {
        _lines.Clear();
        _lines.AddRange(state._lines);
        _cursorRow = state._cursorRow;
        _cursorColumn = state._cursorColumn;
    }
}
