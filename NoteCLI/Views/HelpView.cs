using Spectre.Console;
using System;

namespace NoteCLI.Views
{
    public class HelpView
    {
        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("📖 NoteCLI 使用帮助").Centered().RuleStyle(Style.Parse("blue")));

            var panel = new Panel(
                new Markup(
                    "[bold]NoteCLI 是一个基于 Spectre.Console 的命令行笔记管理器。[/]\n\n"
                    + "[green]主要功能：[/]\n"
                    + "- 笔记列表：查看所有笔记，支持按分类和标签过滤\n"
                    + "- 新增笔记：创建新笔记，输入标题、内容、分类和标签\n"
                    + "- 编辑/删除：编辑或删除已有的笔记\n"
                    + "- 搜索：按关键词和标签搜索笔记\n"
                    + "- 导出：将笔记导出为 Markdown 或 JSON 格式\n"
                    + "- 帮助：查看使用说明\n"
                    + "- 退出：退出程序\n\n"
                    + "[yellow]快捷键：[/]\n"
                    + "- 使用上下箭头选择菜单选项\n"
                    + "- 按 Enter 键确认选择\n"
                    + "- 按任意键返回主菜单\n\n"
                    + "[cyan]示例：[/]\n"
                    + "1. 选择'新增笔记'创建一条新笔记\n"
                    + "2. 输入标题、内容，选择分类和标签\n"
                    + "3. 在'笔记列表'中查看所有笔记\n"
                    + "4. 使用'搜索'功能查找特定内容的笔记\n"
                    + "5. 导出笔记到桌面备份\n"
                )
            );
            panel.Header = new PanelHeader("使用说明");
            panel.Border = BoxBorder.Rounded;

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}