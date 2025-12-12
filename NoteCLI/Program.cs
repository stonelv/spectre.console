using NoteCLI.Services;
using NoteCLI.Views;
using Spectre.Console;
using System;

namespace NoteCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageService = new NoteStorageService();
            var dataGenerator = new NoteDataGenerator();

            // 初始化模拟数据
            var notes = storageService.GetAllNotes();
            if (!notes.Any())
            {
                var mockNotes = dataGenerator.GenerateMockNotes(20);
                foreach (var note in mockNotes)
                {
                    storageService.SaveNote(note);
                }
            }

            var mainMenu = new MainMenu();
            var noteListView = new NoteListView(storageService);
            var noteEditView = new NoteEditView(storageService);
            var searchView = new SearchView(storageService);
            var exportView = new ExportView(storageService);
            var helpView = new HelpView();

            while (true)
            {
                var choice = mainMenu.Show();

                switch (choice)
                {
                    case 1:
                        noteListView.Show();
                        break;
                    case 2:
                        noteEditView.Create();
                        break;
                    case 3:
                        noteEditView.EditOrDelete();
                        break;
                    case 4:
                        searchView.Show();
                        break;
                    case 5:
                        exportView.Show();
                        break;
                    case 6:
                        helpView.Show();
                        break;
                    case 0:
                        var confirm = AnsiConsole.Confirm("确定要退出吗？");
                        if (confirm)
                        {
                            AnsiConsole.MarkupLine("[green]再见！[/]");
                            return;
                        }
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]无效选择，请重新选择。[/]");
                        break;
                }
            }
        }
    }
}