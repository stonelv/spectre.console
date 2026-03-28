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
            // 初始化存储服务
            NoteStorageService storageService = new NoteStorageService();

            // 生成模拟数据
            if (storageService.GetAllNotes().Count == 0)
            {
                NoteDataGenerator generator = new NoteDataGenerator();
                var sampleNotes = generator.GenerateSampleNotes(20);
                foreach (var note in sampleNotes)
                {
                    storageService.SaveNote(note);
                }
            }

            // 初始化视图
            MainMenu mainMenu = new MainMenu();
            NoteListView noteListView = new NoteListView(storageService);
            NoteEditView noteEditView = new NoteEditView(storageService);
            SearchView searchView = new SearchView(storageService);
            ExportView exportView = new ExportView(storageService);
            HelpView helpView = new HelpView();

            bool running = true;

            while (running)
            {
                var selection = mainMenu.Show();

                switch (selection)
                {
                    case MainMenu.MenuOption.NoteList:
                        noteListView.Show();
                        break;
                    case MainMenu.MenuOption.AddNote:
                        noteEditView.AddNote();
                        break;
                    case MainMenu.MenuOption.EditDelete:
                        noteEditView.EditDeleteNote();
                        break;
                    case MainMenu.MenuOption.Search:
                        searchView.Show();
                        break;
                    case MainMenu.MenuOption.Export:
                        exportView.Show();
                        break;
                    case MainMenu.MenuOption.Help:
                        helpView.Show();
                        break;
                    case MainMenu.MenuOption.Exit:
                        bool confirmExit = AnsiConsole.Prompt(
                            new ConfirmationPrompt("确定要退出吗？")
                                .DefaultValue(false)
                        );
                        if (confirmExit)
                        {
                            running = false;
                            AnsiConsole.MarkupLine("[yellow]感谢使用NoteCLI！[/]");
                        }
                        break;
                }
            }
        }
    }
}