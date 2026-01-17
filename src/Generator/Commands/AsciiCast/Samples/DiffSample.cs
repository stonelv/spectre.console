using Spectre.Console;

namespace Spectre.Console.Samples;

public static class DiffSample
{
    public static void Run()
    {
        AnsiConsole.MarkupLine("[bold yellow]Diff Component Examples[/]");

        var oldText = @"Line 1: This is the original text
Line 2: Some content here
Line 3: More content
Line 4: This line will be deleted
Line 5: Another line";

        var newText = @"Line 1: This is the original text
Line 2: Some modified content here
Line 3: More content
Line 4.5: This is a new line
Line 5: Another line";

        AnsiConsole.MarkupLine("\n[bold]1. Inline Diff Mode (Default Colors)[/]");
        var inlineDiff = new Diff(oldText, newText);
        AnsiConsole.Write(inlineDiff);

        AnsiConsole.MarkupLine("\n[bold]2. Side-by-Side Diff Mode[/]");
        var sideBySideDiff = new Diff(oldText, newText).Mode(DiffMode.SideBySide);
        AnsiConsole.Write(sideBySideDiff);

        AnsiConsole.MarkupLine("\n[bold]3. Custom Colors[/]");
        var customColorDiff = new Diff(oldText, newText)
            .DeletedColor(Color.Yellow)
            .InsertedColor(Color.Cyan)
            .UnchangedColor(Color.Grey)
            .Mode(DiffMode.Inline);
        AnsiConsole.Write(customColorDiff);

        AnsiConsole.MarkupLine("\n[bold]4. Custom Markers[/]");
        var customMarkerDiff = new Diff(oldText, newText)
            .DeletedMarker("❌ ")
            .InsertedMarker("✅ ")
            .UnchangedMarker("  ")
            .Mode(DiffMode.Inline);
        AnsiConsole.Write(customMarkerDiff);

        AnsiConsole.MarkupLine("\n[bold]5. Custom Styles with Decoration[/]");
        var customStyleDiff = new Diff(oldText, newText)
            .DeletedStyle(new Style(Color.Red, decoration: Decoration.Strikethrough))
            .InsertedStyle(new Style(Color.Green, decoration: Decoration.Bold))
            .UnchangedStyle(new Style(Color.Blue))
            .Mode(DiffMode.Inline);
        AnsiConsole.Write(customStyleDiff);

        AnsiConsole.MarkupLine("\n[bold]6. Simple Text Comparison[/]");
        var simpleOld = "Hello World";
        var simpleNew = "Hello Spectre.Console";
        var simpleDiff = new Diff(simpleOld, simpleNew).Mode(DiffMode.Inline);
        AnsiConsole.Write(simpleDiff);
    }
}
