using System;
using System.Threading.Tasks;
using Spectre.Console;

namespace Spectre.Console.Samples
{
    public static class SimpleEditorTest
    {
        public static async Task Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold green]Spectre.Console 多行代码编辑器测试[/]");
            AnsiConsole.MarkupLine("[grey]按 ESC 键退出编辑器。[/]");
            AnsiConsole.MarkupLine("[grey]在Mac系统上，可以使用 Cmd+Z 或 Ctrl+Z 来撤销操作。[/]");
            AnsiConsole.MarkupLine("[grey]此版本包含调试输出，显示按键信息。[/]");
            AnsiConsole.WriteLine();

            var editor = new MultiLineTextEditor
            {
                Title = "多行代码编辑器测试",
                Height = 12,
                Width = 80,
                Text = "using System;\n\npublic class Program\n{\n    public static void Main()\n    {\n        Console.WriteLine(\"Hello, World!\");\n    }\n}"
            };

            try
            {
                var result = await editor.ShowAsync(AnsiConsole.Console);
                
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[bold green]编辑完成！[/]");
                AnsiConsole.MarkupLine($"[green]编辑结果:[/]");
                AnsiConsole.MarkupLine($"[cyan]{result.Replace("\n", "[/]\\n[cyan]")}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]错误: {ex.Message}[/]");
            }
        }
    }
}