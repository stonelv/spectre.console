using NoteCLI.Models;
using NoteCLI.Services;
using NoteCLI.Utils;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace NoteCLI.Views
{
    public class SearchView
    {
        private readonly NoteStorageService _storageService;

        public SearchView(NoteStorageService storageService)
        {
            _storageService = storageService;
        }

        public void Show()
        {
            ConsoleUtils.ShowHeader("搜索笔记");

            string keyword = AnsiConsole.Prompt(
                new TextPrompt<string>("请输入搜索关键词：")
                    .PromptStyle("cyan")
            );

            List<Note> allNotes = _storageService.GetAllNotes();
            var tags = allNotes.SelectMany(n => n.Tags).Distinct().ToList();

            string selectedTag = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("选择标签：")
                    .AddChoices("全部").AddChoices(tags)
            );

            List<Note> results = allNotes.Where(n =>
                (string.IsNullOrWhiteSpace(keyword) || n.Title.Contains(keyword) || n.Content.Contains(keyword)) &&
                (selectedTag == "全部" || n.Tags.Contains(selectedTag))
            ).ToList();

            if (results.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]没有找到匹配的笔记[/]");
                ConsoleUtils.ClearAndWait();
                return;
            }

            AnsiConsole.MarkupLine($"[green]找到 {results.Count} 条匹配的笔记[/]");
            AnsiConsole.WriteLine();

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("标题");
            table.AddColumn("内容");
            table.AddColumn("分类");
            table.AddColumn("标签");

            foreach (var note in results)
            {
                string title = string.IsNullOrWhiteSpace(keyword) ? note.Title : HighlightKeyword(note.Title, keyword);
                string content = string.IsNullOrWhiteSpace(keyword) ? note.Content : HighlightKeyword(note.Content, keyword);

                table.AddRow(
                    note.Id.ToString().Substring(0, 8),
                    title,
                    content,
                    note.Category,
                    string.Join(", ", note.Tags)
                );
            }

            AnsiConsole.Write(table);
            ConsoleUtils.ClearAndWait();
        }

        private string HighlightKeyword(string text, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return text;

            int index = text.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return text;

            return $"{text.Substring(0, index)}[bold yellow]{text.Substring(index, keyword.Length)}[/]{text.Substring(index + keyword.Length)}";
        }
    }
}