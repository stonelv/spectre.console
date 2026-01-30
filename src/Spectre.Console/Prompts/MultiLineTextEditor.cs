using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Internal;

namespace Spectre.Console;

/// <summary>
/// Represents a multi-line text editor component.
/// </summary>
public sealed class MultiLineTextEditor : Renderable, IHasBorder, IExpandable
{
    private readonly List<string> _lines = new();
    private readonly Stack<EditorState> _undoStack = new();
    private int _cursorLine;
    private int _cursorColumn;
    private int _scrollTop;
    private string _clipboard = string.Empty;
    private bool _isDirty;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public BoxBorder Border { get; set; } = BoxBorder.Rounded;

    /// <inheritdoc/>
    public bool UseSafeBorder { get; set; } = true;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public Style? BorderStyle { get; set; }

    /// <summary>
    /// Gets or sets the title of the editor.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the height of the editor.
    /// </summary>
    public int Height { get; set; } = 10;

    /// <summary>
    /// Gets or sets the width of the editor.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of undo operations (default: 10).
    /// </summary>
    public int MaxUndoLevels { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether or not the object should
    /// expand to the available space. If <c>false</c>, the object's
    /// width will be auto calculated.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text
    {
        get => string.Join(Environment.NewLine, _lines);
        set
        {
            _lines.Clear();
            if (!string.IsNullOrEmpty(value))
            {
                _lines.AddRange(value.Split(Environment.NewLine));
            }
            
            if (_lines.Count == 0)
            {
                _lines.Add(string.Empty);
            }
            
            _cursorLine = 0;
            _cursorColumn = 0;
            _scrollTop = 0;
            _isDirty = false;
            ClearUndoStack();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLineTextEditor"/> class.
    /// </summary>
    public MultiLineTextEditor()
    {
        _lines.Add(string.Empty);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLineTextEditor"/> class.
    /// </summary>
    /// <param name="text">The initial text content.</param>
    public MultiLineTextEditor(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Renders the editor to the console.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="maxWidth">The maximum width.</param>
    /// <returns>The measurement.</returns>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var width = Width ?? maxWidth;
        return new Measurement(width, width);
    }

    /// <summary>
    /// Renders the editor.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="maxWidth">The maximum width.</param>
    /// <returns>The rendered content.</returns>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var width = Width ?? maxWidth;
        var content = new Markup(GetContent(options));
        var panel = new Panel(content)
        {
            Border = Border,
            BorderStyle = BorderStyle,
            Header = !string.IsNullOrEmpty(Title) ? new PanelHeader(Title) : null,
            Width = width,
            Height = Height
        };

        return ((IRenderable)panel).Render(options, maxWidth);
    }

    /// <summary>
    /// Runs the editor interactively.
    /// </summary>
    /// <param name="console">The console to run the editor in.</param>
    /// <returns>The edited text.</returns>
    public async Task<string> ShowAsync(IAnsiConsole console)
    {
        return await ShowAsync(console, CancellationToken.None);
    }

    /// <summary>
    /// Runs the editor interactively.
    /// </summary>
    /// <param name="console">The console to run the editor in.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The edited text.</returns>
    public async Task<string> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        SaveState();

        return await console.RunExclusive(async () =>
        {
            while (true)
            {
                RenderEditor(console);
                
                var keyInfo = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                if (keyInfo == null)
                {
                    continue;
                }

                var handled = await HandleKey(console, keyInfo.Value);
                if (handled)
                {
                    break;
                }
            }

            return Text;
        }).ConfigureAwait(false);
    }

    private async Task<bool> HandleKey(IAnsiConsole console, ConsoleKeyInfo keyInfo)
    {
        // Debug output to understand what keys are being pressed
        if (OperatingSystem.IsMacOS() && (keyInfo.Key == ConsoleKey.Z || keyInfo.Key == ConsoleKey.C || keyInfo.Key == ConsoleKey.V || keyInfo.Key == ConsoleKey.Y))
        {
            console.WriteLine($"Key: {keyInfo.Key}, Modifiers: {keyInfo.Modifiers}, KeyChar: {keyInfo.KeyChar}");
        }
        
        // Handle special keys
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                SaveState();
                InsertNewLine();
                break;
                
            case ConsoleKey.Backspace:
                SaveState();
                HandleBackspace();
                break;
                
            case ConsoleKey.Delete:
                SaveState();
                HandleDelete();
                break;
                
            case ConsoleKey.UpArrow:
                MoveCursorUp();
                break;
                
            case ConsoleKey.DownArrow:
                MoveCursorDown();
                break;
                
            case ConsoleKey.LeftArrow:
                MoveCursorLeft();
                break;
                
            case ConsoleKey.RightArrow:
                MoveCursorRight();
                break;
                
            case ConsoleKey.Home:
                _cursorColumn = 0;
                break;
                
            case ConsoleKey.End:
                _cursorColumn = _lines[_cursorLine].Length;
                break;
                
            case ConsoleKey.PageUp:
                _cursorLine = Math.Max(0, _cursorLine - Height / 2);
                _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
                break;
                
            case ConsoleKey.PageDown:
                _cursorLine = Math.Min(_lines.Count - 1, _cursorLine + Height / 2);
                _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
                break;
                
            case ConsoleKey.Z:
                // Handle undo with Ctrl+Z on Windows/Linux or Cmd+Z on Mac
                if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    Undo();
                }
                break;
                
            case ConsoleKey.Y:
                // Handle redo with Ctrl+Y on Windows/Linux or Cmd+Y on Mac
                if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    Redo();
                }
                break;
                
            case ConsoleKey.C:
                // Handle copy with Ctrl+C on Windows/Linux or Cmd+C on Mac
                if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    Copy();
                }
                break;
                
            case ConsoleKey.V:
                // Handle paste with Ctrl+V on Windows/Linux or Cmd+V on Mac
                if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    SaveState();
                    Paste();
                }
                break;
                
            case ConsoleKey.Escape:
                return true; // Exit editor
                
            default:
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    SaveState();
                    InsertCharacter(keyInfo.KeyChar);
                }
                break;
        }

        UpdateScrollPosition();
        return false;
    }

    // Simplified Mac shortcut detection method
    private static bool IsMacShortcut(ConsoleKeyInfo keyInfo)
    {
        if (OperatingSystem.IsMacOS())
        {
            // On Mac, Command key shortcuts are detected by Control modifier
            // This works for Cmd+Z, Cmd+C, Cmd+V, Cmd+Y
            return keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
        }
        
        // On Windows/Linux, use Control key
        return keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
    }

    // Helper method to detect Ctrl key on Windows/Linux or Cmd key on Mac
    private static bool IsControlOrCommand(ConsoleKeyInfo keyInfo)
    {
        // On Mac, Command key is also reported as Control
        // We need to check if it's actually a Command key on Mac
        if (OperatingSystem.IsMacOS())
        {
            // On Mac, we consider both Control and Command as valid for our shortcuts
            // This allows both Ctrl+Z and Cmd+Z to work
            return keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
        }
        
        // On Windows/Linux, we only want Control key
        return keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
    }

    // Alternative method to detect Mac Command key
    private static bool IsMacCommand(ConsoleKeyInfo keyInfo)
    {
        // On Mac, Command key might be reported differently
        // Let's try to detect it based on the key combination
        if (OperatingSystem.IsMacOS())
        {
            // Check for common Mac Command key combinations
            // Try different combinations of modifiers
            return (keyInfo.Key == ConsoleKey.Z || keyInfo.Key == ConsoleKey.C || 
                    keyInfo.Key == ConsoleKey.V || keyInfo.Key == ConsoleKey.Y) &&
                   (keyInfo.Modifiers == ConsoleModifiers.Control || 
                    keyInfo.Modifiers == (ConsoleModifiers.Control | ConsoleModifiers.Alt) ||
                    keyInfo.Modifiers == (ConsoleModifiers.Control | ConsoleModifiers.Shift));
        }
        
        return false;
    }

    private void RenderEditor(IAnsiConsole console)
    {
        console.Clear();
        
        // Create a panel with the editor content
        var panel = new Panel(GetContent(RenderOptions.Create(console)))
        {
            Border = Border,
            BorderStyle = BorderStyle,
            Header = !string.IsNullOrEmpty(Title) ? new PanelHeader(Title) : null,
            Width = Width ?? console.Profile.Width,
            Height = Height
        };

        console.Write(panel);
        
        // Position cursor
        var cursorRow = _cursorLine - _scrollTop;
        var cursorCol = _cursorColumn;
        console.Cursor.SetPosition(cursorCol + 1, cursorRow + 1);
    }

    private string GetContent(RenderOptions options)
    {
        var width = (Width ?? options.ConsoleSize.Width) - 2; // Account for border
        var height = Height - 2; // Account for border
        var sb = new StringBuilder();
        
        var visibleLines = _lines.Skip(_scrollTop).Take(height).ToList();
        
        for (int i = 0; i < visibleLines.Count; i++)
        {
            var line = visibleLines[i];
            var isCursorLine = i + _scrollTop == _cursorLine;
            
            if (isCursorLine)
            {
                // Add cursor indicator
                var beforeCursor = line.Substring(0, Math.Min(_cursorColumn, line.Length));
                var cursorChar = _cursorColumn < line.Length ? line[_cursorColumn] : ' ';
                var afterCursor = _cursorColumn + 1 < line.Length ? line.Substring(_cursorColumn + 1) : string.Empty;
                
                sb.Append("[reverse]");
                sb.Append(beforeCursor);
                sb.Append("[/]");
                sb.Append("[black on white]");
                sb.Append(cursorChar);
                sb.Append("[/]");
                sb.Append(afterCursor);
            }
            else
            {
                sb.Append(line);
            }
            
            if (i < visibleLines.Count - 1)
            {
                sb.AppendLine();
            }
        }
        
        // Add empty lines if needed
        while (visibleLines.Count < height)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            
            if (visibleLines.Count - 1 == _cursorLine - _scrollTop)
            {
                sb.Append("[black on white] [/]");
            }
            
            visibleLines.Add(string.Empty);
        }
        
        return sb.ToString();
    }

    private void InsertCharacter(char c)
    {
        var line = _lines[_cursorLine];
        _lines[_cursorLine] = line.Insert(_cursorColumn, c.ToString());
        _cursorColumn++;
        _isDirty = true;
    }

    private void InsertNewLine()
    {
        var line = _lines[_cursorLine];
        var beforeCursor = line.Substring(0, _cursorColumn);
        var afterCursor = _cursorColumn < line.Length ? line.Substring(_cursorColumn) : string.Empty;
        
        _lines[_cursorLine] = beforeCursor;
        _lines.Insert(_cursorLine + 1, afterCursor);
        
        _cursorLine++;
        _cursorColumn = 0;
        _isDirty = true;
    }

    private void HandleBackspace()
    {
        if (_cursorColumn > 0)
        {
            var line = _lines[_cursorLine];
            _lines[_cursorLine] = line.Remove(_cursorColumn - 1, 1);
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
        _isDirty = true;
    }

    private void HandleDelete()
    {
        var line = _lines[_cursorLine];
        
        if (_cursorColumn < line.Length)
        {
            _lines[_cursorLine] = line.Remove(_cursorColumn, 1);
        }
        else if (_cursorLine < _lines.Count - 1)
        {
            _lines[_cursorLine] += _lines[_cursorLine + 1];
            _lines.RemoveAt(_cursorLine + 1);
        }
        _isDirty = true;
    }

    private void MoveCursorUp()
    {
        if (_cursorLine > 0)
        {
            _cursorLine--;
            _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
        }
    }

    private void MoveCursorDown()
    {
        if (_cursorLine < _lines.Count - 1)
        {
            _cursorLine++;
            _cursorColumn = Math.Min(_cursorColumn, _lines[_cursorLine].Length);
        }
    }

    private void MoveCursorLeft()
    {
        if (_cursorColumn > 0)
        {
            _cursorColumn--;
        }
        else if (_cursorLine > 0)
        {
            _cursorLine--;
            _cursorColumn = _lines[_cursorLine].Length;
        }
    }

    private void MoveCursorRight()
    {
        if (_cursorColumn < _lines[_cursorLine].Length)
        {
            _cursorColumn++;
        }
        else if (_cursorLine < _lines.Count - 1)
        {
            _cursorLine++;
            _cursorColumn = 0;
        }
    }

    private void UpdateScrollPosition()
    {
        var height = Height - 2; // Account for border
        
        if (_cursorLine < _scrollTop)
        {
            _scrollTop = _cursorLine;
        }
        else if (_cursorLine >= _scrollTop + height)
        {
            _scrollTop = _cursorLine - height + 1;
        }
    }

    private void SaveState()
    {
        if (_isDirty)
        {
            _undoStack.Push(new EditorState(
                _lines.ToList(),
                _cursorLine,
                _cursorColumn,
                _scrollTop));
            
            // Limit undo stack size
            while (_undoStack.Count > MaxUndoLevels)
            {
                _undoStack.Pop();
            }
            
            _isDirty = false;
        }
    }

    private void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var state = _undoStack.Pop();
            _lines.Clear();
            _lines.AddRange(state.Lines);
            _cursorLine = state.CursorLine;
            _cursorColumn = state.CursorColumn;
            _scrollTop = state.ScrollTop;
            _isDirty = false;
        }
    }

    private void Redo()
    {
        // Note: In a more complete implementation, we would maintain a redo stack
        // For now, we'll just provide a placeholder that does nothing
        // In a real application, you would implement a proper redo functionality
    }

    private void ClearUndoStack()
    {
        _undoStack.Clear();
    }

    private void Copy()
    {
        if (_cursorLine < _lines.Count)
        {
            var line = _lines[_cursorLine];
            if (_cursorColumn < line.Length)
            {
                _clipboard = line[_cursorColumn].ToString();
            }
        }
    }

    private void Paste()
    {
        if (!string.IsNullOrEmpty(_clipboard))
        {
            var line = _lines[_cursorLine];
            _lines[_cursorLine] = line.Insert(_cursorColumn, _clipboard);
            _cursorColumn += _clipboard.Length;
            _isDirty = true;
        }
    }

    private record EditorState(
        List<string> Lines,
        int CursorLine,
        int CursorColumn,
        int ScrollTop);
}