using Spectre.Console;

namespace Spectre.Console;

public static class DiffTest
{
    public static void Run()
    {
        var oldText = File.ReadAllText("old.txt");
        var newText = File.ReadAllText("new.txt");

        AnsiConsole.MarkupLine("[bold yellow]=== Inline Mode ===[/]");
        var inlineDiff = new Diff(oldText, newText)
            .Mode(DiffMode.Inline);
        AnsiConsole.Write(inlineDiff);

        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold yellow]=== Side-by-Side Mode ===[/]");
        var sideBySideDiff = new Diff(oldText, newText)
            .Mode(DiffMode.SideBySide);
        AnsiConsole.Write(sideBySideDiff);

        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold yellow]=== Custom Colors ===[/]");
        var customDiff = new Diff(oldText, newText)
            .Mode(DiffMode.Inline)
            .DeletedColor(Color.Yellow)
            .InsertedColor(Color.Cyan)
            .UnchangedColor(Color.Grey);
        AnsiConsole.Write(customDiff);
    }
}
