using NoteCLI.Models;
using NoteCLI.Services;
using NoteCLI.Utils;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace NoteCLI.Views
{
    public class NoteListView
    {
        private readonly NoteStorageService _storageService;

        public NoteListView(NoteStorageService storageService)
        {
            _storageService = storageService;
        }

        public void Show()
        {
            ConsoleUtils.ShowHeader("笔记列表");

            List<Note> allNotes = _storageService.GetAllNotes();

            if (allNotes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]暂无笔记[/]");
                ConsoleUtils.ClearAndWait();
                return;
            }

            // 获取所有分类和标签
            var categories = allNotes.Select(n => n.Category).Distinct().ToList();
            var tags = allNotes.SelectMany(n => n.Tags).Distinct().ToList();

            // 过滤选项
            string selectedCategory = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("选择分类：")
                    .AddChoices("全部").AddChoices(categories)
            );

            string selectedTag = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("选择标签：")
                    .AddChoices("全部").AddChoices(tags)
            );

            // 应用过滤
            List<Note> filteredNotes = allNotes.Where(n =>
                (selectedCategory == "全部" || n.Category == selectedCategory) &&
                (selectedTag == "全部" || n.Tags.Contains(selectedTag))
            ).ToList();

            if (filteredNotes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]没有匹配的笔记[/]");
                ConsoleUtils.ClearAndWait();
                return;
            }

            // 分页显示
            int pageSize = 10;
            int pageCount = (filteredNotes.Count + pageSize - 1) / pageSize;

            for (int page = 0; page < pageCount; page++)
            {
                AnsiConsole.Clear();
                ConsoleUtils.ShowHeader($"笔记列表 - 第 {page + 1}/{pageCount} 页");

                var pageNotes = filteredNotes.Skip(page * pageSize).Take(pageSize).ToList();

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("标题");
                table.AddColumn("分类");
                table.AddColumn("标签");
                table.AddColumn("创建时间");

                foreach (var note in pageNotes)
                {
                    table.AddRow(
                        note.Id.ToString().Substring(0, 8),
                        note.Title,
                        note.Category,
                        string.Join(", ", note.Tags),
                        ConsoleUtils.FormatDateTime(note.CreatedAt)
                    );
                }

                AnsiConsole.Write(table);

                if (page < pageCount - 1)
                {
                    AnsiConsole.MarkupLine("[yellow]按任意键查看下一页...[/]");
                    System.Console.ReadKey(true);
                }
            }

            ConsoleUtils.ClearAndWait();
        }
    }
}