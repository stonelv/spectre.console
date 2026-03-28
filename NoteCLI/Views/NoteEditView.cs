using NoteCLI.Models;
using NoteCLI.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoteCLI.Views
{
    public class NoteEditView
    {
        private readonly NoteStorageService _storageService;

        public NoteEditView(NoteStorageService storageService)
        {
            _storageService = storageService;
        }

        public void Create()
        {
            var note = new Note();
            note.Id = Guid.NewGuid();
            note.Title = AnsiConsole.Prompt(new TextPrompt<string>("请输入标题:"));
            note.Content = AnsiConsole.Prompt(new TextPrompt<string>("请输入内容:"));
            note.Category = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("请选择分类:")
                .AddChoices("工作", "学习", "生活", "娱乐", "健康"));
            note.Tags = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                .Title("请选择标签:")
                .AddChoices("重要", "紧急", "待办", "阅读", "写作", "编程", "健身", "旅行"));
            note.CreatedAt = DateTime.Now;
            note.UpdatedAt = DateTime.Now;

            _storageService.SaveNote(note);
            AnsiConsole.MarkupLine("[green]笔记创建成功！[/]");
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }

        public void EditOrDelete()
        {
            var notes = _storageService.GetAllNotes();
            if (!notes.Any())
            {
                AnsiConsole.MarkupLine("[red]暂无笔记，请先添加笔记。[/]");
                AnsiConsole.WriteLine("按任意键返回主菜单...");
                Console.ReadKey();
                return;
            }

            var noteSelection = AnsiConsole.Prompt(
                new SelectionPrompt<Note>()
                    .Title("请选择要编辑或删除的笔记:")
                    .AddChoices(notes)
                    .UseConverter(n => n.Title));

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择操作:")
                    .AddChoices("编辑", "删除"));

            if (action == "编辑")
            {
                noteSelection.Title = AnsiConsole.Prompt(new TextPrompt<string>("请输入标题:").DefaultValue(noteSelection.Title));
                noteSelection.Content = AnsiConsole.Prompt(new TextPrompt<string>("请输入内容:").DefaultValue(noteSelection.Content));
                noteSelection.Category = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("请选择分类:")
                    .AddChoices("工作", "学习", "生活", "娱乐", "健康")
                    .DefaultValue(noteSelection.Category));
                noteSelection.Tags = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                    .Title("请选择标签:")
                    .AddChoices("重要", "紧急", "待办", "阅读", "写作", "编程", "健身", "旅行")
                    .DefaultValue(noteSelection.Tags));
                noteSelection.UpdatedAt = DateTime.Now;

                _storageService.SaveNote(noteSelection);
                AnsiConsole.MarkupLine("[green]笔记更新成功！[/]");
            }
            else
            {
                var confirm = AnsiConsole.Confirm("确定要删除这条笔记吗？");
                if (confirm)
                {
                    _storageService.DeleteNote(noteSelection.Id);
                    AnsiConsole.MarkupLine("[green]笔记删除成功！[/]");
                }
            }

            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}