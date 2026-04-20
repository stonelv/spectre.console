namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Live/CancellableProgress")]
public sealed class CancellableProgressTests
{
    [Fact]
    public void Should_Support_Cancellation_Token()
    {
        // Given
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        // When & Then
        Should.Throw<OperationCanceledException>(() =>
        {
            progress.Start(ctx =>
            {
                ctx.ThrowIfCancellationRequested();
            }, cts.Token);
        });
    }

    [Fact]
    public void Should_Throw_OperationCanceledException_Before_Action_When_Token_Already_Cancelled()
    {
        // Given
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        var actionCalled = false;

        // When & Then
        var exception = Should.Throw<OperationCanceledException>(() =>
        {
            progress.Start(ctx =>
            {
                actionCalled = true;
            }, cts.Token);
        });

        actionCalled.ShouldBe(false);
    }

    [Fact]
    public void Should_Stop_Unfinished_Tasks_When_Cancelled()
    {
        // Given
        using var cts = new CancellationTokenSource();

        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false)
            .ShowErrorSummary(false);

        CancellableProgressTask? task1 = null;
        CancellableProgressTask? task2 = null;

        // When
        progress.Start(ctx =>
        {
            task1 = ctx.AddTask("Task 1");
            task2 = ctx.AddTask("Task 2");

            task1.Value = task1.MaxValue;

            cts.Cancel();

            Thread.Sleep(10);
        }, cts.Token);

        // Then
        task1.ShouldNotBeNull();
        task2.ShouldNotBeNull();
        task1.IsFinished.ShouldBe(true);
        task2.IsFinished.ShouldBe(true);
    }

    [Fact]
    public void Should_Track_Failed_Tasks()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false)
            .ShowErrorSummary(false);

        CancellableProgressTask? failedTask = null;
        IReadOnlyList<CancellableProgressTask>? failedTasks = null;

        // When
        progress.Start(ctx =>
        {
            var task1 = ctx.AddTask("Task 1");
            failedTask = ctx.AddTask("Task 2");
            var task3 = ctx.AddTask("Task 3");

            task1.Value = task1.MaxValue;
            failedTask.Fail("Something went wrong");
            task3.Value = task3.MaxValue;

            failedTasks = ctx.FailedTasks;
        });

        // Then
        failedTask.ShouldNotBeNull();
        failedTask.IsFailed.ShouldBe(true);
        failedTask.Error.ShouldNotBeNull();
        failedTask.Error.Message.ShouldBe("Something went wrong");

        failedTasks.ShouldNotBeNull();
        failedTasks.Count.ShouldBe(1);
        failedTasks[0].Description.ShouldBe("Task 2");
    }

    [Fact]
    public void Should_Update_Task_Description_Dynamically()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        CancellableProgressTask? task = null;

        // When
        progress.Start(ctx =>
        {
            task = ctx.AddTask("Initial description");
            task.Description = "Updated description";
        });

        // Then
        task.ShouldNotBeNull();
        task.Description.ShouldBe("Updated description");
    }

    [Fact]
    public async Task Should_Support_Async_Operations()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        // When
        async Task RunAsync()
        {
            await progress.StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Async task");
                await Task.Yield();
                task.Value = task.MaxValue;
            });
        }

        // Then
        var exception = await Record.ExceptionAsync(RunAsync);
        exception.ShouldBeNull();
    }

    [Fact]
    public void Should_HasFailedTasks_Be_True_When_Task_Fails()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false)
            .ShowErrorSummary(false);

        var hasFailedTasks = false;

        // When
        progress.Start(ctx =>
        {
            var task = ctx.AddTask("Task");
            task.Fail("Error");
            hasFailedTasks = ctx.HasFailedTasks;
        });

        // Then
        hasFailedTasks.ShouldBe(true);
    }

    [Fact]
    public void Should_GetTasks_Return_All_Tasks()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        IReadOnlyList<CancellableProgressTask>? tasks = null;

        // When
        progress.Start(ctx =>
        {
            ctx.AddTask("Task 1");
            ctx.AddTask("Task 2");
            ctx.AddTask("Task 3");
            tasks = ctx.GetTasks();
        });

        // Then
        tasks.ShouldNotBeNull();
        tasks.Count.ShouldBe(3);
        tasks[0].Description.ShouldBe("Task 1");
        tasks[1].Description.ShouldBe("Task 2");
        tasks[2].Description.ShouldBe("Task 3");
    }

    [Fact]
    public void Should_Support_AddTaskAt()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        IReadOnlyList<CancellableProgressTask>? tasks = null;

        // When
        progress.Start(ctx =>
        {
            ctx.AddTask("Task 1");
            ctx.AddTask("Task 3");
            ctx.AddTaskAt("Task 2", 1);
            tasks = ctx.GetTasks();
        });

        // Then
        tasks.ShouldNotBeNull();
        tasks.Count.ShouldBe(3);
        tasks[0].Description.ShouldBe("Task 1");
        tasks[1].Description.ShouldBe("Task 2");
        tasks[2].Description.ShouldBe("Task 3");
    }

    [Fact]
    public void Should_Fail_Task_With_Exception()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false)
            .ShowErrorSummary(false);

        var expectedException = new InvalidOperationException("Custom exception");
        CancellableProgressTask? task = null;

        // When
        progress.Start(ctx =>
        {
            task = ctx.AddTask("Task");
            task.Fail(expectedException);
        });

        // Then
        task.ShouldNotBeNull();
        task.IsFailed.ShouldBe(true);
        task.Error.ShouldBeSameAs(expectedException);
        task.IsFinished.ShouldBe(true);
    }

    [Fact]
    public void Should_Throw_When_Fail_With_Null_Exception()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        // When & Then
        Should.Throw<ArgumentNullException>(() =>
        {
            progress.Start(ctx =>
            {
                var task = ctx.AddTask("Task");
                task.Fail((Exception)null!);
            });
        });
    }

    [Fact]
    public void Should_Throw_When_Fail_With_Null_Or_Empty_Message()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        // When & Then
        Should.Throw<ArgumentException>(() =>
        {
            progress.Start(ctx =>
            {
                var task = ctx.AddTask("Task");
                task.Fail(string.Empty);
            });
        });
    }

    [Fact]
    public void Should_IsCancellationRequested_Be_True_When_Cancelled()
    {
        // Given
        using var cts = new CancellationTokenSource();

        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        var isCancellationRequestedBefore = true;
        var isCancellationRequestedAfter = false;

        // When
        progress.Start(ctx =>
        {
            isCancellationRequestedBefore = ctx.IsCancellationRequested;
            cts.Cancel();
            isCancellationRequestedAfter = ctx.IsCancellationRequested;
        }, cts.Token);

        // Then
        isCancellationRequestedBefore.ShouldBe(false);
        isCancellationRequestedAfter.ShouldBe(true);
    }

    [Fact]
    public void Should_CancellationToken_Match_Provided_Token()
    {
        // Given
        using var cts = new CancellationTokenSource();

        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        CancellationToken actualToken = default;

        // When
        progress.Start(ctx =>
        {
            actualToken = ctx.CancellationToken;
        }, cts.Token);

        // Then
        actualToken.ShouldBe(cts.Token);
    }

    [Fact]
    public void Should_StartAsync_With_CancellationToken_Work()
    {
        // Given
        using var cts = new CancellationTokenSource();

        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        // When
        async Task RunAsync()
        {
            await progress.StartAsync(ctx =>
            {
                var task = ctx.AddTask("Test");
                task.Value = task.MaxValue;
                return Task.CompletedTask;
            }, cts.Token);
        }

        // Then
        Should.NotThrow(RunAsync);
    }

    [Fact]
    public void Should_RenderHook_Receive_ReadOnly_Views()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        IReadOnlyList<CancellableProgressTaskView>? receivedViews = null;

        progress.RenderHook = (renderable, tasks) =>
        {
            receivedViews = tasks;
            return renderable;
        };

        // When
        progress.Start(ctx =>
        {
            var task = ctx.AddTask("Task 1");
            task.Value = task.MaxValue;
            ctx.Refresh();
        });

        // Then
        receivedViews.ShouldNotBeNull();
        receivedViews.Count.ShouldBe(1);
        receivedViews[0].Description.ShouldBe("Task 1");

        var viewType = receivedViews[0].GetType();
        viewType.GetMethod("Fail").ShouldBeNull();
        viewType.GetMethod("StartTask").ShouldBeNull();
        viewType.GetMethod("StopTask").ShouldBeNull();
    }

    [Fact]
    public void Should_RenderHook_Receive_Same_View_Instance_For_Same_Task()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var progress = new CancellableProgress(console)
            .Columns(new[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(false);

        CancellableProgressTaskView? firstView = null;
        CancellableProgressTaskView? secondView = null;
        var callCount = 0;

        progress.RenderHook = (renderable, tasks) =>
        {
            callCount++;
            if (callCount == 1 && tasks.Count > 0)
            {
                firstView = tasks[0];
            }
            else if (callCount == 2 && tasks.Count > 0)
            {
                secondView = tasks[0];
            }
            return renderable;
        };

        // When
        progress.Start(ctx =>
        {
            var task = ctx.AddTask("Task 1");
            ctx.Refresh();
            task.Increment(50);
            ctx.Refresh();
        });

        // Then
        firstView.ShouldNotBeNull();
        secondView.ShouldNotBeNull();
        ReferenceEquals(firstView, secondView).ShouldBe(true);
    }

    [Fact]
    public void Should_ErrorSummary_Render_With_Errors()
    {
        // Given
        var console = new TestConsole()
            .Interactive();

        var errorSummary = new ErrorSummary();
        errorSummary.AddError("Task 1", new InvalidOperationException("Error 1"));
        errorSummary.AddError("Task 2", new ArgumentException("Error 2"));

        // When
        var segments = errorSummary.Render(RenderOptions.Create(console), 80);
        var output = string.Join("", segments.Select(s => s.Text));

        // Then
        errorSummary.HasErrors.ShouldBe(true);
        output.ShouldContain("Error Summary");
        output.ShouldContain("Task 1");
        output.ShouldContain("Task 2");
        output.ShouldContain("Error 1");
        output.ShouldContain("Error 2");
    }

    [Fact]
    public void Should_ErrorSummary_HasErrors_Be_False_When_Empty()
    {
        // Given
        var errorSummary = new ErrorSummary();

        // Then
        errorSummary.HasErrors.ShouldBe(false);
    }

    [Fact]
    public void Should_ErrorSummary_Title_Be_Customizable()
    {
        // Given
        var errorSummary = new ErrorSummary
        {
            Title = "My Errors"
        };
        errorSummary.AddError("Task", new Exception("Error"));

        var console = new TestConsole().Interactive();

        // When
        var segments = errorSummary.Render(RenderOptions.Create(console), 80);
        var output = string.Join("", segments.Select(s => s.Text));

        // Then
        output.ShouldContain("My Errors");
    }
}
