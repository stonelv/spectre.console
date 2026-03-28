using NoteCLI.Models;
using NoteCLI.Services;
using Spectre.Console;
using System;
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
            var notes = _storageService.GetAllNotes();
            if (!notes.Any())
            {
                AnsiConsole.MarkupLine("[red]暂无笔记，请先添加笔记。[/]");
                AnsiConsole.WriteLine("按任意键返回主菜单...");
                Console.ReadKey();
                return;
            }

            var categories = notes.Select(n => n.Category).Distinct().ToList();
            categories.Insert(0, "全部");

            var categoryFilter = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择分类:")
                    .AddChoices(categories));

            var filteredNotes = categoryFilter == "全部" ? notes : notes.Where(n => n.Category == categoryFilter).ToList();

            var tags = filteredNotes.SelectMany(n => n.Tags).Distinct().ToList();
            tags.Insert(0, "全部标签");

            var tagFilter = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择标签:")
                    .AddChoices(tags));

            if (tagFilter != "全部标签")
            {
                filteredNotes = filteredNotes.Where(n => n.Tags.Contains(tagFilter)).ToList();
            }

            if (!filteredNotes.Any())
            {
                AnsiConsole.MarkupLine("[yellow]没有找到匹配的笔记。[/]");
                AnsiConsole.WriteLine("按任意键返回主菜单...");
                Console.ReadKey();
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("标题");
            table.AddColumn("分类");
            table.AddColumn("标签");
            table.AddColumn("创建时间");

            foreach (var note in filteredNotes)
            {
                table.AddRow(
                    note.Id.ToString().Substring(0, 8),
                    note.Title,
                    note.Category,
                    string.Join(", ", note.Tags),
                    note.CreatedAt.ToString("yyyy-MM-dd")
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}