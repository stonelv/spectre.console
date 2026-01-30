using System;
using Spectre.Console;

namespace DiffDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example 1: Inline diff with mixed changes
            AnsiConsole.MarkupLine("[bold]Example 1: Inline Diff[/]");
            var oldText1 = "Line 1\nLine 2\nLine 3";
            var newText1 = "Line 1\nModified Line 2\nLine 4";
            var diff1 = new Diff(oldText1, newText1, DiffMode.Inline);
            AnsiConsole.Write(diff1);
            
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            
            // Example 2: Side-by-side diff with mixed changes
            AnsiConsole.MarkupLine("[bold]Example 2: Side-by-Side Diff[/]");
            var oldText2 = "Line 1\nLine 2\nLine 3";
            var newText2 = "Line 1\nModified Line 2\nLine 4";
            var diff2 = new Diff(oldText2, newText2, DiffMode.SideBySide);
            AnsiConsole.Write(diff2);
            
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            
            // Example 3: Custom styles
            AnsiConsole.MarkupLine("[bold]Example 3: Custom Styles[/]");
            var oldText3 = "Line 1\nLine 2\nLine 3";
            var newText3 = "Line 1\nLine 4";
            var diff3 = new Diff(oldText3, newText3)
            {
                AddedStyle = new Style(Color.Blue),
                DeletedStyle = new Style(Color.Yellow),
                UnchangedStyle = new Style(Color.Magenta)
            };
            AnsiConsole.Write(diff3);
            
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            
            // Example 4: Empty text
            AnsiConsole.MarkupLine("[bold]Example 4: Empty Text[/]");
            var diff4 = new Diff("", "");
            AnsiConsole.Write(diff4);
        }
    }
}