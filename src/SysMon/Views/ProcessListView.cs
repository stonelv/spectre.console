using Spectre.Console;
using SysMon.Services;

namespace SysMon.Views
{
    public static class ProcessListView
    {
        public static void Show()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("进程列表").RuleStyle(Spectre.Console.Style.Parse("cyan")));
            AnsiConsole.WriteLine();

            // 获取进程列表
            var processes = ProcessService.GetProcesses();

            // 创建Table展示进程信息
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.FromName("cyan"))
                .AddColumn("进程名称")
                .AddColumn("PID")
                .AddColumn("内存占用 (MB)");

            // 添加进程数据
            foreach (var process in processes)
            {
                table.AddRow(
                    process.Name,
                    process.Pid.ToString(),
                    process.MemoryUsage.ToString()
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}