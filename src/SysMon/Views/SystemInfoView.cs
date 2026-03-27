using Spectre.Console;
using SysMon.Services;

namespace SysMon.Views
{
    public static class SystemInfoView
    {
        public static void Show()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("系统信息").RuleStyle(Spectre.Console.Style.Parse("cyan")));
            AnsiConsole.WriteLine();

            // 获取系统信息
            var systemInfo = SystemInfoService.GetSystemInfo();

            // 使用Panel展示系统信息
            var panel = new Panel(new Markup($"[bold]CPU核心数:[/] {systemInfo.CpuCores}\n\n" +
                $"[bold]内存信息:[/]\n  总内存: {systemInfo.TotalMemory} GB\n  已用内存: {systemInfo.UsedMemory} GB\n  可用内存: {systemInfo.FreeMemory} GB\n\n" +
                $"[bold]操作系统:[/] {systemInfo.OSName}"))
                .Header("系统摘要")
                .BorderColor(Color.FromName("cyan"))
                .Padding(1, 1);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}