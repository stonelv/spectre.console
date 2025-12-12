using NoteCLI.Models;
using NoteCLI.Services;
using NoteCLI.Utils;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NoteCLI.Views
{
    public class ExportView
    {
        private readonly NoteStorageService _storageService;

        public ExportView(NoteStorageService storageService)
        {
            _storageService = storageService;
        }

        public void Show()
        {
            ConsoleUtils.ShowHeader("导出笔记");

            List<Note> notes = _storageService.GetAllNotes();

            if (notes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]暂无笔记可导出[/]");
                ConsoleUtils.ClearAndWait();
                return;
            }

            string format = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("选择导出格式：")
                    .AddChoices("Markdown", "JSON")
            );

            string fileName = AnsiConsole.Prompt(
                new TextPrompt<string>("请输入文件名：")
                    .PromptStyle("cyan")
                    .DefaultValue($"notes_{DateTime.Now:yyyyMMdd}")
            );

            string extension = format == "Markdown" ? ".md" : ".json";
            string fullPath = $"{fileName}{extension}";

            AnsiConsole.MarkupLine($"[yellow]开始导出 {notes.Count} 条笔记到 {fullPath}...[/]");

            // 显示导出进度
            AnsiConsole.Progress()
                .AutoRefresh(true)
                .AutoClear(false)
                .HideCompleted(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new ElapsedTimeColumn()
                })
                .Start(ctx =>
                {
                    var task = ctx.AddTask("导出中...", maxValue: notes.Count);

                    if (format == "Markdown")
                    {
                        ExportToMarkdown(notes, fullPath, task);
                    }
                    else
                    {
                        ExportToJson(notes, fullPath, task);
                    }
                });

            AnsiConsole.MarkupLine($"[green]导出成功！文件已保存到 {fullPath}[/]");
            ConsoleUtils.ClearAndWait();
        }

        private void ExportToMarkdown(List<Note> notes, string filePath, ProgressTask task)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"# 笔记导出 ({DateTime.Now:yyyy-MM-dd HH:mm})");
                writer.WriteLine();
                writer.WriteLine($"共 {notes.Count} 条笔记");
                writer.WriteLine();

                foreach (Note note in notes)
                {
                    writer.WriteLine($"## {note.Title}");
                    writer.WriteLine();
                    writer.WriteLine($"**分类：** {note.Category}");
                    writer.WriteLine($"**标签：** {string.Join(", ", note.Tags)}");
                    writer.WriteLine($"**创建时间：** {ConsoleUtils.FormatDateTime(note.CreatedAt)}");
                    writer.WriteLine($"**更新时间：** {ConsoleUtils.FormatDateTime(note.UpdatedAt)}");
                    writer.WriteLine();
                    writer.WriteLine(note.Content);
                    writer.WriteLine();
                    writer.WriteLine("---");
                    writer.WriteLine();

                    task.Increment(1);
                }
            }
        }

        private void ExportToJson(List<Note> notes, string filePath, ProgressTask task)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(notes, options);
            File.WriteAllText(filePath, json);

            task.Value = notes.Count;
        }
    }
}