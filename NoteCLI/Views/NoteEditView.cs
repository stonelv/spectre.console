using NoteCLI.Models;
using NoteCLI.Services;
using NoteCLI.Utils;
using Spectre.Console;
using System.Collections.Generic;

namespace NoteCLI.Views
{
    public class NoteEditView
    {
        private readonly NoteStorageService _storageService;

        public NoteEditView(NoteStorageService storageService)
        {
            _storageService = storageService;
        }

        public void AddNote()
        {
            ConsoleUtils.ShowHeader("新增笔记");

            Note note = new Note
            {
                Title = AnsiConsole.Prompt(
                    new TextPrompt<string>("请输入标题：")
                        .PromptStyle("cyan")
                        .ValidationErrorMessage("[red]标题不能为空[/]")
                        .Validate(title => string.IsNullOrWhiteSpace(title) ? ValidationResult.Error("标题不能为空") : ValidationResult.Success())
                ),
                Content = AnsiConsole.Prompt(
                    new TextPrompt<string>("请输入内容：")
                        .PromptStyle("cyan")
                ),
                Category = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("选择分类：")
                        .AddChoices("工作", "学习", "生活", "娱乐", "健康", "其他")
                ),
                Tags = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("选择标签：")
                        .AddChoices("重要", "紧急", "待办", "已完成", "想法", "灵感")
                        .PageSize(10)
                        .MoreChoicesText("[grey]（使用上下箭头导航，空格选择，回车确认）[/]")
                )
            };

            _storageService.SaveNote(note);
            AnsiConsole.MarkupLine("[green]笔记新增成功！[/]");
            ConsoleUtils.ClearAndWait();
        }

        public void EditDeleteNote()
        {
            ConsoleUtils.ShowHeader("编辑/删除笔记");

            List<Note> notes = _storageService.GetAllNotes();

            if (notes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]暂无笔记[/]");
                ConsoleUtils.ClearAndWait();
                return;
            }

            Note selectedNote = AnsiConsole.Prompt(
                new SelectionPrompt<Note>()
                    .Title("选择要编辑/删除的笔记：")
                    .PageSize(10)
                    .MoreChoicesText("[grey]（使用上下箭头导航）[/]")
                    .AddChoices(notes)
                    .UseConverter(note => $"{note.Title} - {note.Category}")
            );

            string action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("选择操作：")
                    .AddChoices("编辑", "删除", "取消")
            );

            if (action == "编辑")
            {
                selectedNote.Title = AnsiConsole.Prompt(
                    new TextPrompt<string>("请输入新标题：")
                        .PromptStyle("cyan")
                        .DefaultValue(selectedNote.Title)
                );

                selectedNote.Content = AnsiConsole.Prompt(
                    new TextPrompt<string>("请输入新内容：")
                        .PromptStyle("cyan")
                        .DefaultValue(selectedNote.Content)
                );

                selectedNote.Category = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("选择新分类：")
                        .AddChoices("工作", "学习", "生活", "娱乐", "健康", "其他")
                        .DefaultValue(selectedNote.Category)
                );

                selectedNote.Tags = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("选择新标签：")
                        .AddChoices("重要", "紧急", "待办", "已完成", "想法", "灵感")
                        .PageSize(10)
                        .MoreChoicesText("[grey]（使用上下箭头导航，空格选择，回车确认）[/]")
                        .DefaultValues(selectedNote.Tags)
                );

                _storageService.SaveNote(selectedNote);
                AnsiConsole.MarkupLine("[green]笔记编辑成功！[/]");
            }
            else if (action == "删除")
            {
                bool confirm = AnsiConsole.Prompt(
                    new ConfirmationPrompt("确定要删除这条笔记吗？")
                        .DefaultValue(false)
                );

                if (confirm)
                {
                    _storageService.DeleteNote(selectedNote.Id);
                    AnsiConsole.MarkupLine("[green]笔记删除成功！[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]已取消删除[/]");
                }
            }

            ConsoleUtils.ClearAndWait();
        }
    }
}