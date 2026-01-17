using Spectre.Console;
using System.Threading;
using System.Threading.Tasks;

var editor = new CodeEditor
{
    Width = 100,
    Height = 30,
    ShowLineNumbers = true,
    EditorStyle = Style.Plain,
    CursorStyle = new Style(foreground: Color.Black, background: Color.Cyan1)
};

var welcomeMessage = @"Welcome to Spectre.Console Code Editor!

Features:
- Standard text input
- Backspace and Delete keys for text removal
- Ctrl+Z for undo (up to 10 steps)
- Ctrl+Y for redo
- Ctrl+C to copy all text to clipboard
- Ctrl+V to paste text from clipboard
- Arrow keys for navigation
- Escape key to exit and save

Start typing to test the editor...
";

editor.SetText(welcomeMessage);

Console.WriteLine("Code Editor Test");
Console.WriteLine("Press Escape to exit");
Console.WriteLine();

var result = await editor.Show(AnsiConsole.Console);

Console.Clear();
Console.WriteLine("Editor closed!");
Console.WriteLine();
Console.WriteLine("Your text:");
Console.WriteLine();
Console.WriteLine(result);
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey(true);
