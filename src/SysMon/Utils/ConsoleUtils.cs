using Spectre.Console;

namespace SysMon.Utils;

/// <summary>
/// 控制台工具类
/// </summary>
public static class ConsoleUtils
{
    /// <summary>
    /// 显示标题
    /// </summary>
    public static void ShowTitle(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[bold blue]{title}[/]").DoubleBorder().LeftJustified());
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// 显示成功消息
    /// </summary>
    public static void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[bold green]✓[/] {message}");
    }

    /// <summary>
    /// 显示错误消息
    /// </summary>
    public static void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[bold red]✗[/] {message}");
    }

    /// <summary>
    /// 显示提示消息
    /// </summary>
    public static void ShowInfo(string message)
    {
        AnsiConsole.MarkupLine($"[bold blue]ℹ[/] {message}");
    }

    /// <summary>
    /// 等待用户按键
    /// </summary>
    public static void WaitForKey(string message = "按任意键继续...")
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[gray]{message}[/]");
        Console.ReadKey(true);
    }
}