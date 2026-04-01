using Spectre.Console;
using SysMon.Views;

namespace SysMon;

/// <summary>
/// SysMon - 系统监控命令行工具
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            AnsiConsole.WriteLine("[bold blue]欢迎使用 SysMon 系统监控工具[/]");
            AnsiConsole.WriteLine();

            var mainMenu = new MainMenu();
            await mainMenu.Show();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]错误:[/] {0}", ex.Message);
        }
    }
}