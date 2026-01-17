using System.IO;
using Spectre.Console;

var original = File.ReadAllText("original.txt");
var modified = File.ReadAllText("modified.txt");

AnsiConsole.MarkupLine("[bold]Diff 组件测试[/]");
AnsiConsole.MarkupLine("[underline]内联模式：[/]");
AnsiConsole.Write(new Diff(original, modified));

AnsiConsole.MarkupLine("\n[underline]并排模式：[/]");
AnsiConsole.Write(new Diff(original, modified)
    .WithMode(DiffMode.SideBySide)
    .WithLineNumbers(true)
    .WithHeaders(true));
