using Spectre.Console;
using System;

namespace NoteCLI.Views
{
    public class MainMenu
    {
        public enum MenuOption
        {
            NoteList,
            AddNote,
            EditDelete,
            Search,
            Export,
            Help,
            Exit
        }

        public MenuOption Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold green]NoteCLI 笔记管理器[/]").Centered());
            AnsiConsole.WriteLine();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOption>()
                    .Title("请选择操作：")
                    .PageSize(10)
                    .MoreChoicesText("[grey]（使用上下箭头导航）[/]")
                    .AddChoices(
                        MenuOption.NoteList,
                        MenuOption.AddNote,
                        MenuOption.EditDelete,
                        MenuOption.Search,
                        MenuOption.Export,
                        MenuOption.Help,
                        MenuOption.Exit)
                    .UseConverter(opt => opt switch
                    {
                        MenuOption.NoteList => "📝 笔记列表",
                        MenuOption.AddNote => "✏️ 新增笔记",
                        MenuOption.EditDelete => "✏️ 编辑/删除",
                        MenuOption.Search => "🔍 搜索",
                        MenuOption.Export => "📤 导出",
                        MenuOption.Help => "❓ 帮助",
                        MenuOption.Exit => "🚪 退出",
                        _ => opt.ToString()
                    })
            );

            return selection;
        }
    }
}