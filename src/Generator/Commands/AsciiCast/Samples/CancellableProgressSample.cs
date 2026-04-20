using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace Generator.Commands.Samples;

internal class CancellableProgressSample : BaseSample
{
    public override (int Cols, int Rows) ConsoleSize => (base.ConsoleSize.Cols, 15);

    public override void Run(IAnsiConsole console)
    {
        RunAsync(console).GetAwaiter().GetResult();
    }

    private async Task RunAsync(IAnsiConsole console)
    {
        using var cts = new CancellationTokenSource();

        console.CancellableProgress()
            .AutoClear(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .ShowErrorSummary(true)
            .ErrorSummaryTitle("Failed Tasks")
            .StartAsync(async ctx =>
            {
                var random = new Random(42);

                var task1 = ctx.AddTask("Downloading file A");
                var task2 = ctx.AddTask("Processing data");
                var task3 = ctx.AddTask("Uploading results");
                var task4 = ctx.AddTask("Sending notification", autoStart: false);

                var tasks = new List<Task>
                {
                    SimulateTaskAsync(task1, random, 10, ctx.CancellationToken),
                    SimulateTaskWithErrorAsync(task2, "Network timeout", random, 5),
                    SimulateTaskAsync(task3, random, 15, ctx.CancellationToken),
                };

                await Task.WhenAll(tasks);

                if (!ctx.HasFailedTasks)
                {
                    task4.StartTask();
                    await SimulateTaskAsync(task4, random, 5, ctx.CancellationToken);
                }
            }, cts.Token);
    }

    private static async Task SimulateTaskAsync(
        CancellableProgressTask task,
        Random random,
        int iterations,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < iterations; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            task.Increment(100.0 / iterations);
            task.Description = $"{task.Description.Split(' ')[0]} step {i + 1}";

            await Task.Delay(random.Next(50, 150), cancellationToken);
        }
    }

    private static async Task SimulateTaskWithErrorAsync(
        CancellableProgressTask task,
        string errorMessage,
        Random random,
        int iterations)
    {
        for (var i = 0; i < iterations; i++)
        {
            if (i == iterations / 2)
            {
                await Task.Delay(random.Next(50, 150));
                task.Fail(errorMessage);
                return;
            }

            task.Increment(100.0 / iterations);
            await Task.Delay(random.Next(50, 150));
        }
    }
}
