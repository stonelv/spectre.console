using Spectre.Console;
using SysMon.Services;

namespace SysMon.Views;

/// <summary>
/// 系统信息视图
/// </summary>
public class SystemInfoView
{
    private readonly SystemInfoService _systemInfoService = new SystemInfoService();

    public Task Show()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold blue]系统信息[/]").DoubleBorder().LeftJustified());
        AnsiConsole.WriteLine();

        var panel = new Panel(
            new Markup($"[green]CPU核心数:[/] [bold]{_systemInfoService.GetCpuCores()}[/]\n[green]总内存:[/] [bold]{_systemInfoService.GetTotalMemory()} MB[/]\n[green]已用内存:[/] [bold]{_systemInfoService.GetUsedMemory()} MB[/]\n[green]操作系统:[/] [bold]{_systemInfoService.GetOsName()}[/]")
        )
        {
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse("blue"),
            Padding = new Padding(2, 2, 2, 2),
            Title = new PanelTitle("硬件信息")
        };

        AnsiConsole.Write(panel);

        return Task.CompletedTask;
    }
}