using Spectre.Console;
using SysMon.Services;
using static Spectre.Console.Color;

namespace SysMon.Views
{
    public static class ProgressDemoView
    {
        public static async Task ShowAsync()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("进度演示").RuleStyle(Spectre.Console.Style.Parse("cyan")));
            AnsiConsole.WriteLine();

            // 使用Progress组件展示并发任务
            await AnsiConsole.Progress()
                .AutoClear(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    // 创建3个模拟任务
                    var tasks = new List<ProgressTask>
                    {
                        ctx.AddTask("任务 1: 初始化"),
                        ctx.AddTask("任务 2: 处理数据"),
                        ctx.AddTask("任务 3: 生成报告")
                    };

                    // 模拟任务进度
                    while (!ctx.IsFinished)
                    {
                        foreach (var task in tasks)
                        {
                            if (!task.IsFinished)
                            {
                                task.Increment(1);
                            }
                        }
                        await Task.Delay(50);
                    }

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[green]任务完成![/]");
                    AnsiConsole.WriteLine();
                });

            AnsiConsole.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}
