using NoteCLI.Models;
using NoteCLI.Services;
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
            var format = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("请选择导出格式:")
                    .AddChoices("Markdown", "JSON"));

            var notes = _storageService.GetAllNotes();
            if (!notes.Any())
            {
                AnsiConsole.MarkupLine("[red]暂无笔记，无法导出。[/]");
                AnsiConsole.WriteLine("按任意键返回主菜单...");
                Console.ReadKey();
                return;
            }

            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"notes.{format.ToLower()}");

            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("导出中...", maxValue: notes.Count);

                    for (int i = 0; i < notes.Count; i++)
                    {
                        task.Increment(1);
                        System.Threading.Thread.Sleep(100);
                    }
                });

            if (format == "JSON")
            {
                var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(outputPath, json);
            }
            else
            {
                var markdown = new System.Text.StringBuilder();
                markdown.AppendLine("# 笔记导出");
                markdown.AppendLine();

                foreach (var note in notes)
                {
                    markdown.AppendLine($"## {note.Title}");
                    markdown.AppendLine($"- 分类: {note.Category}");
                    markdown.AppendLine($"- 标签: {string.Join(", ", note.Tags)}");
                    markdown.AppendLine($"- 创建时间: {note.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")}");
                    markdown.AppendLine($"- 更新时间: {note.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")}");
                    markdown.AppendLine();
                    markdown.AppendLine(note.Content);
                    markdown.AppendLine();
                    markdown.AppendLine("---");
                    markdown.AppendLine();
                }

                File.WriteAllText(outputPath, markdown.ToString());
            }

            AnsiConsole.MarkupLine($"[green]导出成功！[/] 已保存到: {outputPath}");
            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}