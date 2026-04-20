using Spectre.Console;
using Spectre.Console.Json;

namespace Generator.Commands.Samples;

internal class JsonDiffSample : BaseSample
{
    public override (int Cols, int Rows) ConsoleSize => (80, 30);

    public override void Run(IAnsiConsole console)
    {
        var leftJson = """
            {
                "id": 1,
                "name": "John Smith",
                "status": "active",
                "age": 30,
                "roles": ["admin", "user"],
                "profile": {
                    "email": "john@example.com",
                    "phone": "123-456-7890"
                }
            }
            """;

        var rightJson = """
            {
                "id": 1,
                "name": "John Doe",
                "age": 31,
                "roles": ["admin", "editor", "user"],
                "profile": {
                    "email": "john.doe@example.com",
                    "address": "123 Main St"
                }
            }
            """;

        var diff = new JsonDiffText(leftJson, rightJson)
            .IgnorePropertyOrder()
            .AddedColor(Color.Green)
            .DeletedColor(Color.Red)
            .ModifiedColor(Color.Yellow)
            .MemberColor(Color.Blue)
            .StringColor(Color.Cyan1)
            .NumberColor(Color.Green);

        console.Write(
            new Panel(diff)
                .Header("JSON Diff Highlight")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

        console.WriteLine();
        console.MarkupLine("[bold]Legend:[/]");
        console.MarkupLine("[green]+ Added[/]");
        console.MarkupLine("[red]- Deleted[/]");
        console.MarkupLine("[yellow]~ Modified[/]");
    }
}
