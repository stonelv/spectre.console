using Spectre.Console;
using SysMon.Services;

namespace SysMon.Views;

/// <summary>
/// 进程列表视图
/// </summary>
public class ProcessListView
{
    private readonly ProcessService _processService = new ProcessService();

    public Task Show()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold blue]进程列表[/]").DoubleBorder().LeftJustified());
        AnsiConsole.WriteLine();

        var processes = _processService.GetProcesses();

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn(new TableColumn("[green]进程名称[/]").Centered());
        table.AddColumn(new TableColumn("[green]PID[/]").Centered());
        table.AddColumn(new TableColumn("[green]内存占用 (MB)[/]").Centered());

        foreach (var process in processes)
        {
            table.AddRow(
                $"[bold]{process.Name}[/]",
                $"[blue]{process.Pid}[/]",
                $"[yellow]{process.MemoryUsage}[/]"
            );
        }

        AnsiConsole.Write(table);

        return Task.CompletedTask;
    }
}