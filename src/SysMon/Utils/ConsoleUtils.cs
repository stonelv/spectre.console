using Spectre.Console;

namespace SysMon.Utils
{
    public static class ConsoleUtils
    {
        /// <summary>
        /// 清空控制台并显示标题
        /// </summary>
        /// <param name="title">标题文本</param>
        public static void ClearAndShowTitle(string title)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText(title)
                .Color(Color.FromName("cyan"))
                .LeftJustified());
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public static void ShowError(string message)
        {
            AnsiConsole.Write(new Panel(new Markup($"[red]{message}[/]"))
                .BorderColor(Color.FromName("red"))
                .Padding(1, 1));
            AnsiConsole.WriteLine();
        }
    }
}