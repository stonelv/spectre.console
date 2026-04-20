namespace Spectre.Console;

/// <summary>
/// Represents a read-only view of a <see cref="CancellableProgressTask"/> for rendering purposes.
/// </summary>
public sealed class CancellableProgressTaskView
{
    private readonly CancellableProgressTask _task;

    /// <summary>
    /// Gets the task ID.
    /// </summary>
    public int Id => _task.Id;

    /// <summary>
    /// Gets the task description.
    /// </summary>
    public string Description => _task.Description;

    /// <summary>
    /// Gets the max value of the task.
    /// </summary>
    public double MaxValue => _task.MaxValue;

    /// <summary>
    /// Gets the current value of the task.
    /// </summary>
    public double Value => _task.Value;

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
    public bool IsFailed => _task.IsFailed;

    /// <summary>
    /// Gets the error that caused the task to fail.
    /// </summary>
    public Exception? Error => _task.Error;

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
    /// Gets a value indicating whether the ProgressBar shows
    /// actual values or generic, continuous progress feedback.
    /// </summary>
    public bool IsIndeterminate => _task.IsIndeterminate;

    internal CancellableProgressTaskView(CancellableProgressTask task)
    {
        _task = task ?? throw new ArgumentNullException(nameof(task));
    }
}
