using Spectre.Console;

namespace SysMon.Views;

/// <summary>
/// 帮助视图
/// </summary>
public class HelpView
{
    public Task Show()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold blue]帮助信息[/]").DoubleBorder().LeftJustified());
        AnsiConsole.WriteLine();

        var panel = new Panel(
            new Markup($@"
[green]📊 系统信息:[/] 显示CPU核心数、内存使用情况和操作系统信息

[green]🧵 进程列表:[/] 展示当前运行的进程列表，包含进程名称、PID和内存占用

[green]⏳ 进度演示:[/] 展示5个并发任务的进度条动画

[green]❓ 帮助:[/] 显示此帮助信息

[green]🚪 退出:[/] 退出SysMon工具

[bold yellow]操作说明:[/]
- 使用方向键上下选择菜单选项
- 按回车键确认选择
- 查看完信息后按任意键返回主菜单")
        )
        {
            Border = BoxBorder.Double,
            BorderStyle = Style.Parse("green"),
            Padding = new Padding(4, 4, 4, 4),
            Title = new PanelTitle("操作指南")
        };

        AnsiConsole.Write(panel);

        return Task.CompletedTask;
    }
}