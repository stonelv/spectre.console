namespace Spectre.Console;

/// <summary>
/// Represents a cancellable progress display with error tracking.
/// </summary>
public sealed class CancellableProgress
{
    private readonly IAnsiConsole _console;
    private readonly Progress _progress;
    private Func<IRenderable, IReadOnlyList<CancellableProgressTask>, IRenderable> _renderHook;

    /// <summary>
    /// Gets or sets a optional custom render function.
    /// </summary>
    public Func<IRenderable, IReadOnlyList<CancellableProgressTask>, IRenderable> RenderHook
    {
        get => _renderHook;
        set
        {
            _renderHook = value;
            _progress.RenderHook = CreateAdapter(value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not task list should auto refresh.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool AutoRefresh
    {
        get => _progress.AutoRefresh;
        set => _progress.AutoRefresh = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the task list should
    /// be cleared once it completes.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool AutoClear
    {
        get => _progress.AutoClear;
        set => _progress.AutoClear = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the task list should
    /// only include tasks not completed.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool HideCompleted
    {
        get => _progress.HideCompleted;
        set => _progress.HideCompleted = value;
    }

    /// <summary>
    /// Gets or sets the refresh rate if <c>AutoRefresh</c> is enabled.
    /// Defaults to 10 times/second.
    /// </summary>
    public TimeSpan RefreshRate
    {
        get => _progress.RefreshRate;
        set => _progress.RefreshRate = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show an error summary
    /// when any task fails.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool ShowErrorSummary { get; set; } = true;

    /// <summary>
    /// Gets or sets the title for the error summary.
    /// </summary>
    public string ErrorSummaryTitle { get; set; } = "Error Summary";

    internal List<ProgressColumn> Columns => _progress.Columns;

    internal ProgressRenderer? FallbackRenderer
    {
        get => _progress.FallbackRenderer;
        set => _progress.FallbackRenderer = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellableProgress"/> class.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    public CancellableProgress(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _progress = new Progress(console);
        _renderHook = (renderable, _) => renderable;
    }

    /// <summary>
    /// Starts the progress task list.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Start(Action<CancellableProgressContext> action)
    {
        Start(action, CancellationToken.None);
    }

    /// <summary>
    /// Starts the progress task list.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public void Start(Action<CancellableProgressContext> action, CancellationToken cancellationToken)
    {
        var task = StartAsync(ctx =>
        {
            action(ctx);
            return Task.CompletedTask;
        }, cancellationToken);

        task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Starts the progress task list and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="func">The action to execute.</param>
    /// <returns>The result.</returns>
    public T Start<T>(Func<CancellableProgressContext, T> func)
    {
        return Start(func, CancellationToken.None);
    }

    /// <summary>
    /// Starts the progress task list and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="func">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public T Start<T>(Func<CancellableProgressContext, T> func, CancellationToken cancellationToken)
    {
        var task = StartAsync(ctx => Task.FromResult(func(ctx)), cancellationToken);
        return task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Starts the progress task list.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(Func<CancellableProgressContext, Task> action)
    {
        await StartAsync(action, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the progress task list.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(Func<CancellableProgressContext, Task> action, CancellationToken cancellationToken)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _ = await StartAsync<object?>(async progressContext =>
        {
            await action(progressContext).ConfigureAwait(false);
            return default;
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the progress task list and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type of task.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation.</returns>
    public async Task<T> StartAsync<T>(Func<CancellableProgressContext, Task<T>> action)
    {
        return await StartAsync(action, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the progress task list and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type of task.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation.</returns>
    public async Task<T> StartAsync<T>(Func<CancellableProgressContext, Task<T>> action, CancellationToken cancellationToken)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        CancellableProgressContext? cancellableContext = null;

        var result = await _progress.StartAsync(async ctx =>
        {
            cancellableContext = new CancellableProgressContext(ctx, cancellationToken);
            return await action(cancellableContext).ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (ShowErrorSummary && cancellableContext?.HasFailedTasks == true)
        {
            var errorSummary = new ErrorSummary(cancellableContext.FailedTasks)
            {
                Title = ErrorSummaryTitle
            };
            _console.WriteLine();
            _console.Write(errorSummary);
        }

        return result;
    }

    private static Func<IRenderable, IReadOnlyList<ProgressTask>, IRenderable> CreateAdapter(
        Func<IRenderable, IReadOnlyList<CancellableProgressTask>, IRenderable>? hook)
    {
        if (hook is null)
        {
            return (renderable, _) => renderable;
        }

        return (renderable, tasks) =>
        {
            var cancellableTasks = tasks.Select(t => new CancellableProgressTask(t)).ToList().AsReadOnly();
            return hook(renderable, cancellableTasks);
        };
    }
}
