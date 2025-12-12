using Spectre.Console;
using SysMon.Services;

namespace SysMon.Views;

/// <summary>
/// 进度演示视图
/// </summary>
public class ProgressDemoView
{
    public async Task Show()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold blue]进度演示[/]").DoubleBorder().LeftJustified());
        AnsiConsole.WriteLine();

        // 并发执行3个模拟任务
        await AnsiConsole.Progress()
            .AutoRefresh(true)
            .HideCompleted(true)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
            })
            .StartAsync(async ctx =>
            {
                var tasks = new List<ProgressTask>
                {
                    ctx.AddTask("[green]任务 1: 数据加载[/]"),
                    ctx.AddTask("[blue]任务 2: 文件下载[/]"),
                    ctx.AddTask("[yellow]任务 3: 图像处理[/]"),
                    ctx.AddTask("[purple]任务 4: 备份数据[/]"),
                    ctx.AddTask("[cyan]任务 5: 同步更新[/]")
                };

                var random = new Random();
                while (!ctx.IsFinished)
                {
                    foreach (var task in tasks)
                    {
                        if (!task.IsCompleted)
                        {
                            task.Increment(random.Next(1, 5));
                        }
                    }

                    await Task.Delay(100);
                }
            });

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]所有任务已完成！[/]");
    }
}