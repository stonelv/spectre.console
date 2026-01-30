using Spectre.Console;

namespace CodeEditorDemo;

class Program
{
    static void Main(string[] args)
    {
        AnsiConsole.MarkupLine("[bold green]Spectre.Console 多行代码编辑器演示[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]功能说明：[/]");
        AnsiConsole.MarkupLine("  - 输入文本字符");
        AnsiConsole.MarkupLine("  - Backspace 删除光标前字符");
        AnsiConsole.MarkupLine("  - Delete 删除光标后字符");
        AnsiConsole.MarkupLine("  - Enter 换行");
        AnsiConsole.MarkupLine("  - 方向键 移动光标");
        AnsiConsole.MarkupLine("  - Ctrl+C 复制当前行");
        AnsiConsole.MarkupLine("  - Ctrl+V 粘贴文本");
        AnsiConsole.MarkupLine("  - Ctrl+Z 或 F5 撤销（最多10步）");
        AnsiConsole.MarkupLine("  - Ctrl+D 或 ESC 确认并退出");
        AnsiConsole.WriteLine();

        var editor = new CodeEditor
        {
            Title = "代码编辑器",
            MaxHeight = 15,
            MaxWidth = 80
        };

        AnsiConsole.MarkupLine("[grey]按任意键开始编辑...[/]");
        Console.ReadKey(true);

        var result = editor.Show(AnsiConsole.Console);

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]编辑完成！[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]您输入的内容：[/]");
        AnsiConsole.WriteLine(new string('-', 80));
        AnsiConsole.Write(result);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine(new string('-', 80));
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]总行数：[/] {editor.Lines.Count} 行");
        AnsiConsole.MarkupLine($"[green]总字符数：[/] {result.Length} 个字符");
    }
}
