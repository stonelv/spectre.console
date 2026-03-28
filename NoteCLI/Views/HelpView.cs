using Spectre.Console;
using NoteCLI.Utils;

namespace NoteCLI.Views
{
    public class HelpView
    {
        public void Show()
        {
            ConsoleUtils.ShowHeader("帮助中心");

            var panel = new Panel(
                new Markup(
                    "[bold yellow]NoteCLI 笔记管理器[/]\n\n"
                    + "[green]功能说明：[/]\n"
                    + "  📝 笔记列表 - 查看所有笔记，支持按分类和标签过滤\n"
                    + "  ✏️ 新增笔记 - 创建新的笔记\n"
                    + "  ✏️ 编辑/删除 - 修改或删除现有笔记\n"
                    + "  🔍 搜索 - 按关键词和标签组合搜索笔记\n"
                    + "  📤 导出 - 将笔记导出为Markdown或JSON格式\n"
                    + "  ❓ 帮助 - 查看使用说明\n"
                    + "  🚪 退出 - 退出程序\n\n"
                    + "[blue]快捷键：[/]\n"
                    + "  上下箭头 - 导航菜单\n"
                    + "  空格 - 多选标签时选择/取消\n"
                    + "  回车 - 确认选择\n"
                    + "  任意键 - 继续操作\n\n"
                    + "[cyan]示例：[/]\n"
                    + "  1. 新增笔记时可以选择多个标签\n"
                    + "  2. 搜索时支持模糊匹配标题和内容\n"
                    + "  3. 导出文件会保存在当前目录下"
                )
            )
            .Header("使用说明")
            .RoundedBorder()
            .BorderColor(Color.Blue);

            AnsiConsole.Write(panel);
            ConsoleUtils.ClearAndWait();
        }
    }
}