namespace Spectre.Console;

/// <summary>
/// Represents a cancellable progress task that can track errors.
/// </summary>
public sealed class CancellableProgressTask : IProgress<double>
{
    private readonly ProgressTask _task;
    private readonly object _lock;
    private Exception? _error;
    private bool _isFailed;

    /// <summary>
    /// Gets the task ID.
    /// </summary>
    public int Id => _task.Id;

    /// <summary>
    /// Gets or sets the task description.
    /// </summary>
    public string Description
    {
        get => _task.Description;
        set => _task.Description = value;
    }

    /// <summary>
    /// Gets or sets the max value of the task.
    /// </summary>
    public double MaxValue
    {
        get => _task.MaxValue;
        set => _task.MaxValue = value;
    }

    /// <summary>
    /// Gets or sets the value of the task.
    /// </summary>
    public double Value
    {
        get => _task.Value;
        set => _task.Value = value;
    }

    /// <summary>
    /// Gets the start time of the task.
    /// </summary>
    public DateTime? StartTime => _task.StartTime;

    /// <summary>
    /// Gets the stop time of the task.
    /// </summary>
    public DateTime? StopTime => _task.StopTime;

    /// <summary>
    /// Gets the task state.
    /// </summary>
    public ProgressTaskState State => _task.State;

    /// <summary>
    /// Gets a value indicating whether or not the task has started.
    /// </summary>
    public bool IsStarted => _task.IsStarted;

    /// <summary>
    /// Gets a value indicating whether or not the task has finished.
    /// </summary>
    public bool IsFinished => _task.IsFinished;

    /// <summary>
    /// Gets a value indicating whether or not the task has failed.
    /// </summary>
    public bool IsFailed
    {
        get
        {
            lock (_lock)
            {
                return _isFailed;
            }
        }
    }

    /// <summary>
    /// Gets the error that caused the task to fail.
    /// </summary>
    public Exception? Error
    {
        get
        {
            lock (_lock)
            {
                return _error;
            }
        }
    }

    /// <summary>
    /// Gets the percentage done of the task.
    /// </summary>
    public double Percentage => _task.Percentage;

    /// <summary>
    /// Gets the speed measured in steps/second.
    /// </summary>
    public double? Speed => _task.Speed;

    /// <summary>
    /// Gets the elapsed time.
    /// </summary>
    public TimeSpan? ElapsedTime => _task.ElapsedTime;

    /// <summary>
    /// Gets the remaining time.
    /// </summary>
    public TimeSpan? RemainingTime => _task.RemainingTime;

    /// <summary>
    /// Gets or sets a value indicating whether the ProgressBar shows
    /// actual values or generic, continuous progress feedback.
    /// </summary>
    public bool IsIndeterminate
    {
        get => _task.IsIndeterminate;
        set => _task.IsIndeterminate = value;
    }

    /// <summary>
    /// Gets the underlying ProgressTask.
    /// </summary>
    internal ProgressTask UnderlyingTask => _task;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellableProgressTask"/> class.
    /// </summary>
    /// <param name="task">The underlying progress task.</param>
    internal CancellableProgressTask(ProgressTask task)
    {
        _task = task ?? throw new ArgumentNullException(nameof(task));
        _lock = new object();
    }

    /// <summary>
    /// Starts the task.
    /// </summary>
    public void StartTask() => _task.StartTask();

    /// <summary>
    /// Stops and marks the task as finished.
    /// </summary>
    public void StopTask() => _task.StopTask();

    /// <summary>
    /// Increments the task's value.
    /// </summary>
    /// <param name="value">The value to increment with.</param>
    public void Increment(double value) => _task.Increment(value);

    /// <summary>
    /// Marks the task as failed with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    public void Fail(Exception error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        lock (_lock)
        {
            _isFailed = true;
            _error = error;
        }

        _task.StopTask();
    }

    /// <summary>
    /// Marks the task as failed with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message that caused the failure.</param>
    public void Fail(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Error message cannot be empty.", nameof(errorMessage));
        }

        Fail(new InvalidOperationException(errorMessage));
    }

    /// <inheritdoc />
    void IProgress<double>.Report(double value) => ((IProgress<double>)_task).Report(value);
}
