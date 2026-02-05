using System.Diagnostics;
using System.Text;
using System.Globalization;

namespace Spectre.Console;

public sealed class MultiLineTextPrompt : IPrompt<List<string>>
{
    private readonly string _prompt;
    private readonly int _maxUndoSteps = 10;
    private readonly Stack<(List<string> Lines, int CursorX, int CursorY)> _undoStack = new Stack<(List<string>, int, int)>();
    
    private List<string> _lines = new List<string> { string.Empty };
    private int _cursorX = 0;
    private int _cursorY = 0;
    
    public Style? PromptStyle { get; set; }
    public string? DefaultValue { get; set; }
    public Style? DefaultValueStyle { get; set; }
    public bool AllowEmpty { get; set; }

    public MultiLineTextPrompt(string prompt)
    {
        _prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
    }

    public List<string> Show(IAnsiConsole console)
    {
        return ShowAsync(console, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task<List<string>> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        return await console.RunExclusive(async () =>
        {
            var promptStyle = PromptStyle ?? Style.Plain;
            
            WritePrompt(console);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rawKey = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                if (rawKey == null)
                {
                    continue;
                }

                var key = rawKey.Value;

                if (key.Key == ConsoleKey.Enter && key.Modifiers == ConsoleModifiers.Control)
                {
                    console.WriteLine();
                    return _lines;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    SaveState();
                    _lines.Insert(_cursorY + 1, string.Empty);
                    _cursorY++;
                    _cursorX = 0;
                    console.WriteLine();
                    continue;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (_cursorX > 0)
                    {
                        SaveState();
                        _lines[_cursorY] = _lines[_cursorY].Substring(0, _cursorX - 1) + _lines[_cursorY].Substring(_cursorX);
                        _cursorX--;
                        console.Write("\b \\b");
                    }
                    else if (_cursorY > 0)
                    {
                        SaveState();
                        var currentLineLength = _lines[_cursorY].Length;
                        _lines[_cursorY - 1] += _lines[_cursorY];
                        _lines.RemoveAt(_cursorY);
                        _cursorY--;
                        _cursorX = _lines[_cursorY].Length - currentLineLength;
                    console.Cursor.SetPosition(0, console.Profile.Height);
                    console.Cursor.Move(CursorDirection.Up, 1);
                    console.Write(_lines[_cursorY].Substring(_cursorX));
                    console.Write(" ");
                    console.Cursor.SetPosition(_cursorX, console.Profile.Height);
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.Delete)
                {
                    if (_cursorX < _lines[_cursorY].Length)
                    {
                        SaveState();
                        _lines[_cursorY] = _lines[_cursorY].Substring(0, _cursorX) + _lines[_cursorY].Substring(_cursorX + 1);
                        console.Write(_lines[_cursorY].Substring(_cursorX));
                        console.Write(" ");
                        console.Cursor.Move(CursorDirection.Left, _lines[_cursorY].Length - _cursorX + 1);
                    }
                    else if (_cursorY < _lines.Count - 1)
                    {
                        SaveState();
                        _lines[_cursorY] += _lines[_cursorY + 1];
                        _lines.RemoveAt(_cursorY + 1);
                        console.Write(_lines[_cursorY].Substring(_cursorX));
                        console.Write(" ");
                        console.Cursor.Move(CursorDirection.Left, _lines[_cursorY].Length - _cursorX + 1);
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.LeftArrow && _cursorX > 0)
                {
                    _cursorX--;
                    console.Write("\b");
                    continue;
                }

                if (key.Key == ConsoleKey.RightArrow && _cursorX < _lines[_cursorY].Length)
                {
                    _cursorX++;
                    console.Write("\\u001b[C");
                    continue;
                }

                if (key.Key == ConsoleKey.UpArrow && _cursorY > 0)
                {
                    _cursorY--;
                    _cursorX = Math.Min(_cursorX, _lines[_cursorY].Length);
                        console.Cursor.Move(CursorDirection.Up, 1);
                        console.Cursor.SetPosition(_cursorX, console.Profile.Height - (_lines.Count - _cursorY - 1));
                    continue;
                }

                if (key.Key == ConsoleKey.DownArrow && _cursorY < _lines.Count - 1)
                {
                    _cursorY++;
                    _cursorX = Math.Min(_cursorX, _lines[_cursorY].Length);
                        console.Cursor.Move(CursorDirection.Down, 1);
                        console.Cursor.SetPosition(_cursorX, console.Profile.Height - (_lines.Count - _cursorY - 1));
                    continue;
                }

                if (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control)
                {
                    var text = string.Join("\n", _lines);
                    try
                    {
                        if (OperatingSystem.IsWindows())
                        {
                            // Use PowerShell to set clipboard text on Windows
                            var process = new Process();
                            process.StartInfo.FileName = "powershell";
                            process.StartInfo.Arguments = $"Set-Clipboard -Value '{text.Replace("'","''")}'";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                            process.WaitForExit();
                        }
                        else if (OperatingSystem.IsMacOS())
                        {
                            var process = new Process();
                            process.StartInfo.FileName = "pbcopy";
                            process.StartInfo.RedirectStandardInput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            process.StandardInput.Write(text);
                            process.StandardInput.Close();
                            process.WaitForExit();
                        }
                        else if (OperatingSystem.IsLinux())
                        {
                            var process = new Process();
                            process.StartInfo.FileName = "xclip";
                            process.StartInfo.Arguments = "-selection clipboard";
                            process.StartInfo.RedirectStandardInput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            process.StandardInput.Write(text);
                            process.StandardInput.Close();
                            process.WaitForExit();
                        }
                    }
                    catch {}
                    continue;
                }

                if (key.Key == ConsoleKey.V && key.Modifiers == ConsoleModifiers.Control)
                {
                    string text = string.Empty;
                    try
                    {
                        if (OperatingSystem.IsWindows())
                        {
                            // Use PowerShell to get clipboard text on Windows
                            var process = new Process();
                            process.StartInfo.FileName = "powershell";
                            process.StartInfo.Arguments = "Get-Clipboard";
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                            text = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                        }
                        else if (OperatingSystem.IsMacOS())
                        {
                            var process = new Process();
                            process.StartInfo.FileName = "pbpaste";
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            text = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                        }
                        else if (OperatingSystem.IsLinux())
                        {
                            var process = new Process();
                            process.StartInfo.FileName = "xclip";
                            process.StartInfo.Arguments = "-selection clipboard -o";
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            text = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                        }
                    }
                    catch {}
                    
                    if (!string.IsNullOrEmpty(text))
                    {
                        SaveState();
                        var newLines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                        if (newLines.Length > 1)
                        {
                            _lines[_cursorY] = _lines[_cursorY].Substring(0, _cursorX) + newLines[0];
                            _lines.InsertRange(_cursorY + 1, newLines.Skip(1));
                            _cursorY += newLines.Length - 1;
                            _cursorX = newLines[^1].Length;
                        }
                        else
                        {
                            _lines[_cursorY] = _lines[_cursorY].Substring(0, _cursorX) + newLines[0] + _lines[_cursorY].Substring(_cursorX);
                            _cursorX += newLines[0].Length;
                        }
                        console.WriteLine();
                        for (var i = 0; i < _lines.Count; i++)
                        {
                            console.Write(_lines[i]);
                            if (i < _lines.Count - 1)
                            {
                                console.WriteLine();
                            }
                        }
                        console.Cursor.SetPosition(_cursorX, console.Profile.Height);
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.Z && key.Modifiers == ConsoleModifiers.Control)
                {
                    if (_undoStack.Count > 0)
                    {
                        var state = _undoStack.Pop();
                        _lines = state.Lines;
                        _cursorX = state.CursorX;
                        _cursorY = state.CursorY;
                        console.WriteLine();
                        for (var i = 0; i < _lines.Count; i++)
                        {
                            console.Write(_lines[i]);
                            if (i < _lines.Count - 1)
                            {
                                console.WriteLine();
                            }
                        }
                        console.Cursor.SetPosition(_cursorX, console.Profile.Height);
                    }
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    SaveState();
                    _lines[_cursorY] = _lines[_cursorY].Insert(_cursorX, key.KeyChar.ToString());
                    console.Write(_lines[_cursorY].Substring(_cursorX));
                        _cursorX++;
                        console.Cursor.Move(CursorDirection.Left, _lines[_cursorY].Length - _cursorX);
                }
            }
        }).ConfigureAwait(false);
    }

    private void SaveState()
    {
        var newLines = new List<string>(_lines);
        _undoStack.Push((newLines, _cursorX, _cursorY));
        if (_undoStack.Count > _maxUndoSteps)
        {
            _undoStack.Pop();
        }
    }

    private void WritePrompt(IAnsiConsole console)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        var builder = new StringBuilder();
        builder.Append(_prompt.TrimEnd());

        if (DefaultValue != null)
        {
            var defaultValueStyle = DefaultValueStyle?.ToMarkup() ?? "green";
            builder.AppendFormat(CultureInfo.InvariantCulture, " [{0}]({1})[/]", defaultValueStyle, DefaultValue);
        }

        var markup = builder.ToString().Trim();
        console.Markup(markup + " ");
    }
}
