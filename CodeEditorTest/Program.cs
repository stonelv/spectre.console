using System;
using System.Threading;
using Spectre.Console;

namespace CodeEditorTest;

class Program
{
    static void Main(string[] args)
    {
        AnsiConsole.Write(new FigletText("Code Editor Test"));
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold green]Welcome to the Spectre.Console Code Editor![/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold]Features:[/]");
        AnsiConsole.MarkupLine("• Multi-line text editing");
        AnsiConsole.MarkupLine("• Standard text input with cursor movement");
        AnsiConsole.MarkupLine("• Backspace and Delete support");
        AnsiConsole.MarkupLine("• Undo operations (max 10 steps)");
        AnsiConsole.MarkupLine("• Copy (Ctrl+C) and Paste (Ctrl+V) support");
        AnsiConsole.MarkupLine("• Submit with Ctrl+Enter or Esc");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold yellow]Test the following features:[/]");
        AnsiConsole.WriteLine();

        // Test 1: Basic text input
        AnsiConsole.MarkupLine("[cyan]Test 1: Basic text input[/]");
        var basicText = AnsiConsole.Prompt(
            new CodeEditor("Type some text (Ctrl+Enter or Esc to continue):")
            {
                InitialText = "Hello, World!",
                VisibleLines = 5
            });
        AnsiConsole.MarkupLine($"[green]✓[/] You entered: {basicText}");
        AnsiConsole.WriteLine();

        // Test 2: Multi-line editing
        AnsiConsole.MarkupLine("[cyan]Test 2: Multi-line editing[/]");
        var multiLineText = AnsiConsole.Prompt(
            new CodeEditor("Edit this multi-line text (use arrow keys to navigate):")
            {
                InitialText = "Line 1\nLine 2\nLine 3",
                VisibleLines = 8
            });
        AnsiConsole.MarkupLine($"[green]✓[/] You entered {multiLineText.Split('\n').Length} lines");
        AnsiConsole.WriteLine();

        // Test 3: Code editing
        AnsiConsole.MarkupLine("[cyan]Test 3: Code editing[/]");
        var codeText = AnsiConsole.Prompt(
            new CodeEditor("Edit this C# code (try undo with Ctrl+Z):")
            {
                InitialText = "public class Program\n{\n    public static void Main()\n    {\n        Console.WriteLine(\"Hello\");\n    }\n}",
                VisibleLines = 12
            });
        AnsiConsole.MarkupLine($"[green]✓[/] Code edited successfully");
        AnsiConsole.WriteLine();

        // Test 4: Copy and Paste
        AnsiConsole.MarkupLine("[cyan]Test 4: Copy and Paste[/]");
        AnsiConsole.MarkupLine("[yellow]Instructions:[/]");
        AnsiConsole.MarkupLine("1. Select some text with the cursor");
        AnsiConsole.MarkupLine("2. Press Ctrl+C to copy");
        AnsiConsole.MarkupLine("3. Move cursor to another position");
        AnsiConsole.MarkupLine("4. Press Ctrl+V to paste");
        AnsiConsole.WriteLine();

        var copyPasteText = AnsiConsole.Prompt(
            new CodeEditor("Test copy and paste functionality:")
            {
                InitialText = "This is a test for copy and paste.\nCopy this line and paste it below.",
                VisibleLines = 8
            });
        AnsiConsole.MarkupLine($"[green]✓[/] Copy and paste test completed");
        AnsiConsole.WriteLine();

        // Final test: Free editing
        AnsiConsole.MarkupLine("[cyan]Test 5: Free editing[/]");
        var freeText = AnsiConsole.Prompt(
            new CodeEditor("Type whatever you want (test all features):")
            {
                VisibleLines = 15
            });

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Panel(freeText)
        {
            Header = new PanelHeader("Final Result"),
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse("blue"),
            Padding = new Padding(1, 1, 1, 1)
        });

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]All tests completed successfully![/]");
    }
}
