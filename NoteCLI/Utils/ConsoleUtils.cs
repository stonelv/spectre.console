using Spectre.Console;
using System;

namespace NoteCLI.Utils
{
    public static class ConsoleUtils
    {
        public static void ClearAndWait()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]按任意键继续...[/]");
            Console.ReadKey(true);
            AnsiConsole.Clear();
        }

        public static void ShowHeader(string title)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[bold blue]{title}[/]").Centered());
            AnsiConsole.WriteLine();
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }
    }
}