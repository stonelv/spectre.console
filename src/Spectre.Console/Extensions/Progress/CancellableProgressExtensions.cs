namespace Spectre.Console;

/// <summary>
/// Contains extension methods for <see cref="CancellableProgress"/>.
/// </summary>
public static class CancellableProgressExtensions
{
    /// <summary>
    /// Sets the columns to be used for an <see cref="CancellableProgress"/> instance.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="columns">The columns to use.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress Columns(this CancellableProgress progress, params ProgressColumn[] columns)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        if (columns is null)
        {
            throw new ArgumentNullException(nameof(columns));
        }

        if (!columns.Any())
        {
            throw new InvalidOperationException("At least one column must be specified.");
        }

        progress.Columns.Clear();
        progress.Columns.AddRange(columns);

        return progress;
    }

    /// <summary>
    /// Sets an optional hook to intercept rendering.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="renderHook">The custom render function.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress UseRenderHook(this CancellableProgress progress, Func<IRenderable, IReadOnlyList<CancellableProgressTask>, IRenderable> renderHook)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        progress.RenderHook = renderHook;

        return progress;
    }

    /// <summary>
    /// Sets whether or not auto refresh is enabled.
    /// If disabled, you will manually have to refresh the progress.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="enabled">Whether or not auto refresh is enabled.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress AutoRefresh(this CancellableProgress progress, bool enabled)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        progress.AutoRefresh = enabled;

        return progress;
    }

    /// <summary>
    /// Sets whether or not auto clear is enabled.
    /// If enabled, the task table will be removed once
    /// all tasks have completed.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="enabled">Whether or not auto clear is enabled.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress AutoClear(this CancellableProgress progress, bool enabled)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        progress.AutoClear = enabled;

        return progress;
    }

    /// <summary>
    /// Sets whether or not hide completed is enabled.
    /// If enabled, the task table will be removed once it is
    /// completed.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="enabled">Whether or not hide completed is enabled.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress HideCompleted(this CancellableProgress progress, bool enabled)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        progress.HideCompleted = enabled;

        return progress;
    }

    /// <summary>
    /// Sets whether or not to show an error summary when any task fails.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="enabled">Whether or not to show the error summary.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress ShowErrorSummary(this CancellableProgress progress, bool enabled)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        progress.ShowErrorSummary = enabled;

        return progress;
    }

    /// <summary>
    /// Sets the title for the error summary.
    /// </summary>
    /// <param name="progress">The <see cref="CancellableProgress"/> instance.</param>
    /// <param name="title">The error summary title.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static CancellableProgress ErrorSummaryTitle(this CancellableProgress progress, string title)
    {
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(progress));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        }

        progress.ErrorSummaryTitle = title;

        return progress;
    }
}
