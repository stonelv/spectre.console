namespace Spectre.Console;

/// <summary>
/// Represents a context that can be used to interact with a <see cref="CancellableProgress"/>.
/// </summary>
public sealed class CancellableProgressContext
{
    private readonly ProgressContext _context;
    private readonly List<CancellableProgressTask> _tasks;
    private readonly Dictionary<int, CancellableProgressTask> _taskCache;
    private readonly Dictionary<int, CancellableProgressTaskView> _viewCache;
    private readonly object _taskLock;
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Gets a value indicating whether or not all started tasks have completed.
    /// </summary>
    public bool IsFinished => _context.IsFinished;

    /// <summary>
    /// Gets a value indicating whether cancellation has been requested.
    /// </summary>
    public bool IsCancellationRequested => _cancellationToken.IsCancellationRequested;

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken => _cancellationToken;

    /// <summary>
    /// Gets a list of all failed tasks.
    /// </summary>
    public IReadOnlyList<CancellableProgressTask> FailedTasks
    {
        get
        {
            lock (_taskLock)
            {
                return _tasks.Where(t => t.IsFailed).ToList().AsReadOnly();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether any task has failed.
    /// </summary>
    public bool HasFailedTasks
    {
        get
        {
            lock (_taskLock)
            {
                return _tasks.Any(t => t.IsFailed);
            }
        }
    }

    internal CancellableProgressContext(ProgressContext context, CancellationToken cancellationToken)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tasks = new List<CancellableProgressTask>();
        _taskCache = new Dictionary<int, CancellableProgressTask>();
        _viewCache = new Dictionary<int, CancellableProgressTaskView>();
        _taskLock = new object();
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Adds a task.
    /// </summary>
    /// <param name="description">The task description.</param>
    /// <param name="autoStart">Whether or not the task should start immediately.</param>
    /// <param name="maxValue">The task's max value.</param>
    /// <returns>The newly created task.</returns>
    public CancellableProgressTask AddTask(string description, bool autoStart = true, double maxValue = 100)
    {
        lock (_taskLock)
        {
            var task = _context.AddTask(description, autoStart, maxValue);
            var cancellableTask = new CancellableProgressTask(task);
            _tasks.Add(cancellableTask);
            _taskCache[task.Id] = cancellableTask;
            return cancellableTask;
        }
    }

    /// <summary>
    /// Adds a task at the specified index.
    /// </summary>
    /// <param name="description">The task description.</param>
    /// <param name="index">The index at which the task should be inserted.</param>
    /// <param name="autoStart">Whether or not the task should start immediately.</param>
    /// <param name="maxValue">The task's max value.</param>
    /// <returns>The newly created task.</returns>
    public CancellableProgressTask AddTaskAt(string description, int index, bool autoStart = true, double maxValue = 100)
    {
        lock (_taskLock)
        {
            var task = _context.AddTaskAt(description, index, autoStart, maxValue);
            var cancellableTask = new CancellableProgressTask(task);
            _tasks.Insert(index, cancellableTask);
            _taskCache[task.Id] = cancellableTask;
            return cancellableTask;
        }
    }

    /// <summary>
    /// Adds a task.
    /// </summary>
    /// <param name="description">The task description.</param>
    /// <param name="settings">The task settings.</param>
    /// <returns>The newly created task.</returns>
    public CancellableProgressTask AddTask(string description, ProgressTaskSettings settings)
    {
        lock (_taskLock)
        {
            var task = _context.AddTask(description, settings);
            var cancellableTask = new CancellableProgressTask(task);
            _tasks.Add(cancellableTask);
            _taskCache[task.Id] = cancellableTask;
            return cancellableTask;
        }
    }

    /// <summary>
    /// Throws a <see cref="OperationCanceledException"/> if cancellation has been requested.
    /// </summary>
    public void ThrowIfCancellationRequested()
    {
        _cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Refreshes the current progress.
    /// </summary>
    public void Refresh()
    {
        _context.Refresh();
    }

    /// <summary>
    /// Gets all tasks.
    /// </summary>
    /// <returns>A read-only list of all tasks.</returns>
    public IReadOnlyList<CancellableProgressTask> GetTasks()
    {
        lock (_taskLock)
        {
            return new List<CancellableProgressTask>(_tasks).AsReadOnly();
        }
    }

    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <returns>The task if found; otherwise, <c>null</c>.</returns>
    internal CancellableProgressTask? GetTaskById(int id)
    {
        lock (_taskLock)
        {
            _taskCache.TryGetValue(id, out var task);
            return task;
        }
    }

    /// <summary>
    /// Gets a read-only view of a task by its ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <returns>The read-only view if found; otherwise, <c>null</c>.</returns>
    internal CancellableProgressTaskView? GetTaskViewById(int id)
    {
        lock (_taskLock)
        {
            if (_viewCache.TryGetValue(id, out var view))
            {
                return view;
            }

            if (_taskCache.TryGetValue(id, out var task))
            {
                view = new CancellableProgressTaskView(task);
                _viewCache[id] = view;
                return view;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the underlying ProgressContext.
    /// </summary>
    internal ProgressContext UnderlyingContext => _context;
}
