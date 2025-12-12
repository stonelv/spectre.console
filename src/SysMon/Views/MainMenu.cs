using Spectre.Console;
using SysMon.Views;

namespace SysMon;

/// <summary>
/// 主菜单视图
/// </summary>
public class MainMenu
{
    public async Task Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]SysMon - 系统监控工具[/]").DoubleBorder().LeftJustified());
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择操作:")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "📊 系统信息", 
                        "🧵 进程列表", 
                        "⏳ 进度演示", 
                        "❓ 帮助", 
                        "🚪 退出"
                    })
            );

            switch (choice)
            {
                case "📊 系统信息":
                    await new SystemInfoView().Show();
                    break;
                case "🧵 进程列表":
                    await new ProcessListView().Show();
                    break;
                case "⏳ 进度演示":
                    await new ProgressDemoView().Show();
                    break;
                case "❓ 帮助":
                    await new HelpView().Show();
                    break;
                case "🚪 退出":
                    if (AnsiConsole.Confirm("确定要退出吗？"))
                    {
                        AnsiConsole.MarkupLine("[bold green]感谢使用 SysMon！[/]");
                        return;
                    }
                    break;
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[gray]按任意键返回主菜单...[/]");
            Console.ReadKey(true);
        }
    }
}