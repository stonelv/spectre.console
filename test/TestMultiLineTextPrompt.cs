using System;
using System.Collections.Generic;
using Spectre.Console;

namespace TestMultiLineTextPrompt
{
    class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.WriteLine("MultiLineTextPrompt Test");
            AnsiConsole.WriteLine("========================");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Features:");
            AnsiConsole.WriteLine("- Standard text input");
            AnsiConsole.WriteLine("- Backspace and Delete");
            AnsiConsole.WriteLine("- Ctrl+Z to undo (up to 10 steps)");
            AnsiConsole.WriteLine("- Ctrl+C to copy to clipboard");
            AnsiConsole.WriteLine("- Ctrl+V to paste from clipboard");
            AnsiConsole.WriteLine("- Ctrl+Enter to finish and return the input");
            AnsiConsole.WriteLine();

            var prompt = new MultiLineTextPrompt("Enter your text:");
            prompt.PromptStyle = new Style(foreground: Color.Cyan);

            AnsiConsole.WriteLine("Please enter some text (Ctrl+Enter to finish):");
            var result = prompt.Show(AnsiConsole.Console);

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("You entered:");
            AnsiConsole.WriteLine("-----------");
            foreach (var line in result)
            {
                AnsiConsole.WriteLine(line);
            }
            AnsiConsole.WriteLine("-----------");
            AnsiConsole.WriteLine($"Total lines: {result.Count}");
        }
    }
}
