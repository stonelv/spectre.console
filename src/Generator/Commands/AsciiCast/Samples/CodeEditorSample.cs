using DocExampleGenerator;
using Spectre.Console;

namespace Generator.Commands.Samples;

internal class CodeEditorSample : BaseSample
{
    public override void Run(IAnsiConsole console)
    {
        console.WriteLine();
        console.Write(new Rule("[yellow]Code Editor Test[/]").RuleStyle("grey").LeftJustified());
        console.WriteLine();

        console.MarkupLine("[bold]Testing Code Editor Features:[/]");
        console.MarkupLine("1. Text input and cursor movement");
        console.MarkupLine("2. Backspace and Delete");
        console.MarkupLine("3. Undo (Ctrl+Z, max 10 steps)");
        console.MarkupLine("4. Copy (Ctrl+C) and Paste (Ctrl+V)");
        console.WriteLine();

        var initialCode = "// Welcome to Code Editor\n// Start typing your code here\n\npublic class Example\n{\n    public void Run()\n    {\n        Console.WriteLine(\"Hello World\");\n    }\n}";

        var editor = new CodeEditor("[bold green]Enter your code (Ctrl+Enter or Esc to submit):[/]")
        {
            InitialText = initialCode,
            VisibleLines = 15
        };

        var result = console.Prompt(editor);

        console.WriteLine();
        console.Write(new Rule("[yellow]Result[/]").RuleStyle("grey").LeftJustified());
        console.WriteLine();

        var panel = new Panel(result)
        {
            Header = new PanelHeader("Code Submitted"),
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse("blue"),
            Padding = new Padding(1, 1, 1, 1)
        };

        console.Write(panel);
        console.WriteLine();

        console.MarkupLine($"[green]✓[/] Lines: {result.Split('\n').Length}");
        console.MarkupLine($"[green]✓[/] Characters: {result.Length}");
    }
}
