using Spectre.Console;
using System.Runtime.InteropServices;

// CodeEditor Test Program
// This program demonstrates the functionality of the CodeEditor component

var isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
var modifierKey = isMacOS ? "Cmd" : "Ctrl";

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule("[yellow]CodeEditor Test Program[/]").RuleStyle("grey"));
AnsiConsole.WriteLine();

AnsiConsole.MarkupLine($"[green]Detected OS: [/][cyan]{RuntimeInformation.OSDescription}[/]");
AnsiConsole.MarkupLine($"[green]Modifier key: [/][cyan]{modifierKey}[/]");
AnsiConsole.WriteLine();

AnsiConsole.MarkupLine("[green]Features to test:[/]");
AnsiConsole.MarkupLine("  • [cyan]Text Input[/] - Type any text");
AnsiConsole.MarkupLine("  • [cyan]Backspace[/] - Press Backspace to delete character before cursor");
AnsiConsole.MarkupLine("  • [cyan]Delete[/] - Press Delete key to delete character after cursor");
AnsiConsole.MarkupLine("  • [cyan]Arrow Keys[/] - Navigate with Up/Down/Left/Right arrows");
AnsiConsole.MarkupLine("  • [cyan]Home/End[/] - Jump to start/end of line");
AnsiConsole.MarkupLine("  • [cyan]Enter[/] - Create new line");
AnsiConsole.MarkupLine("  • [cyan]Tab[/] - Insert 4 spaces");

if (isMacOS)
{
    AnsiConsole.MarkupLine("  • [yellow]macOS Shortcuts:[/]");
    AnsiConsole.MarkupLine("    - [cyan]Option+Z[/] - Undo (up to 10 steps)");
    AnsiConsole.MarkupLine("    - [cyan]Option+Y[/] or [cyan]Option+Shift+Z[/] - Redo");
    AnsiConsole.MarkupLine("    - [cyan]Option+C[/] - Copy current line to clipboard");
    AnsiConsole.MarkupLine("    - [cyan]Option+V[/] - Paste from clipboard");
}
else
{
    AnsiConsole.MarkupLine("  • [yellow]Windows/Linux Shortcuts:[/]");
    AnsiConsole.MarkupLine("    - [cyan]Ctrl+Z[/] - Undo (up to 10 steps)");
    AnsiConsole.MarkupLine("    - [cyan]Ctrl+Y[/] or [cyan]Ctrl+Shift+Z[/] - Redo");
    AnsiConsole.MarkupLine("    - [cyan]Ctrl+C[/] - Copy current line to clipboard");
    AnsiConsole.MarkupLine("    - [cyan]Ctrl+V[/] - Paste from clipboard");
}

AnsiConsole.MarkupLine("  • [cyan]Escape[/] - Exit editor");
AnsiConsole.WriteLine();

AnsiConsole.MarkupLine("[yellow]Press any key to start the editor...[/]");
Console.ReadKey(true);

// Create and run the code editor
var editor = new CodeEditor
{
    Width = 80,
    Height = 20,
    ShowLineNumbers = true,
    BorderStyle = new Style(foreground: Color.Blue),
    TextStyle = new Style(foreground: Color.White),
    LineNumberStyle = new Style(foreground: Color.Grey),
};

// Optional: Set initial content
editor.InsertText("// Welcome to CodeEditor!");
editor.InsertNewLine();
editor.InsertText("// Type your code here...");
editor.InsertNewLine();
editor.InsertNewLine();

var result = editor.Run(AnsiConsole.Console);

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule("[yellow]Editor Closed[/]").RuleStyle("grey"));
AnsiConsole.WriteLine();

AnsiConsole.MarkupLine("[green]Final Content:[/]");
AnsiConsole.WriteLine();

// Display the result in a panel
var panel = new Panel(result.EscapeMarkup())
    .Header("Output")
    .Border(BoxBorder.Rounded)
    .BorderStyle(new Style(foreground: Color.Green));

AnsiConsole.Write(panel);

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine("[grey]Press any key to exit...[/]");
Console.ReadKey(true);
