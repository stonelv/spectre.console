using Spectre.Console;
using System;

namespace NoteCLI.Views
{
    public class MainMenu
    {
        public int Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("📝 NoteCLI 笔记管理器").Centered().RuleStyle(Style.Parse("green")));

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择操作:")
                    .PageSize(10)
                    .AddChoices(
                        "笔记列表",
                        "新增笔记",
                        "编辑/删除",
                        "搜索",
                        "导出",
                        "帮助",
                        "退出"
                    ));

            return selection switch
            {
                "笔记列表" => 1,
                "新增笔记" => 2,
                "编辑/删除" => 3,
                "搜索" => 4,
                "导出" => 5,
                "帮助" => 6,
                "退出" => 0,
                _ => -1
            };
        }
    }
}