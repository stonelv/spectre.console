using Spectre.Console;

namespace SysMon.Views
{
    public static class HelpView
    {
        public static void Show()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("帮助信息").RuleStyle(Spectre.Console.Style.Parse("cyan")));
            AnsiConsole.WriteLine();

            // 使用Panel展示帮助信息
            var panel = new Panel(new Markup("[bold]欢迎使用 SysMon 系统监控工具![/]\n\n" +
                "[bold]功能说明:[/]\n" +
                "[yellow]系统信息[/]: 显示当前系统的 CPU 核心数、内存使用情况和操作系统名称。\n" +
                "[yellow]进程列表[/]: 以表格形式展示当前运行的进程信息,包括进程名称、PID 和内存占用。\n" +
                "[yellow]进度演示[/]: 展示并发任务的进度条动画,模拟多任务处理过程。\n" +
                "[yellow]帮助[/]: 显示本帮助信息。\n" +
                "[yellow]退出[/]: 退出 SysMon 工具。\n\n" +
                "[bold]操作说明:[/]\n" +
                "- 使用上下箭头键选择菜单选项\n" +
                "- 按 Enter 键确认选择\n" +
                "- 在任何页面按任意键可返回主菜单"))
                .Header("帮助文档")
                .BorderColor(Color.FromName("cyan"))
                .Padding(2, 2);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}