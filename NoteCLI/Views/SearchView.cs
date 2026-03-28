using NoteCLI.Models;
using NoteCLI.Services;
using Spectre.Console;
using System;
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
            var keyword = AnsiConsole.Prompt(new TextPrompt<string>("请输入搜索关键词:"));
            var tags = _storageService.GetAllNotes().SelectMany(n => n.Tags).Distinct().ToList();
            tags.Insert(0, "不限制标签");
            var tagFilter = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择标签（可选）:")
                    .AddChoices(tags));

            var notes = _storageService.GetAllNotes();
            var filteredNotes = notes.Where(n => 
                n.Title.Contains(keyword) || 
                n.Content.Contains(keyword) || 
                (tagFilter != "不限制标签" && n.Tags.Contains(tagFilter))
            ).ToList();

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
                var highlightedTitle = note.Title.Replace(keyword, $"[yellow]{keyword}[/]");
                var highlightedContent = note.Content.Replace(keyword, $"[yellow]{keyword}[/]");

                table.AddRow(
                    note.Id.ToString().Substring(0, 8),
                    highlightedTitle,
                    note.Category,
                    string.Join(", ", note.Tags),
                    note.CreatedAt.ToString("yyyy-MM-dd")
                );