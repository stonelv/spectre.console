using Spectre.Console;

namespace SysMon.Views
{
    public static class MainMenu
    {
        public static string Show()
        {
            // 创建主菜单
            var prompt = new SelectionPrompt<string>()
                .Title("\n请选择操作:")
                .PageSize(10)
                .AddChoices(
                    "系统信息",
                    "进程列表",
                    "进度演示",
                    "帮助",
                    "退出")
                .HighlightStyle(new Style(Color.FromName("blue"), Color.FromName("black"), Decoration.Bold));

            // 显示菜单并获取选择
            return AnsiConsole.Prompt(prompt);
        }
    }
}