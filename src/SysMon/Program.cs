using Spectre.Console;
using SysMon.Views;

namespace SysMon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 显示欢迎界面
            AnsiConsole.Write(new FigletText("SysMon")
                .Color(Color.FromName("cyan"))
                .LeftJustified());

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("系统监控工具");
            AnsiConsole.WriteLine();

            // 主循环
            while (true)
            {
                var choice = MainMenu.Show();
                
                switch (choice)
                {
                    case "系统信息":
                        SystemInfoView.Show();
                        break;
                    case "进程列表":
                        ProcessListView.Show();
                        break;
                    case "进度演示":
                        await ProgressDemoView.ShowAsync();
                        break;
                    case "帮助":
                        HelpView.Show();
                        break;
                    case "退出":
                        AnsiConsole.WriteLine("再见!");
                        return;
                }
            }
        }
    }
}