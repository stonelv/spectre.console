namespace Spectre.Console;

/// <summary>
/// Represents an error summary that displays failed tasks and their errors.
/// </summary>
public sealed class ErrorSummary : IRenderable
{
    private readonly List<(string TaskName, Exception Error)> _errors;
    private readonly ErrorSummaryStyle _style;

    /// <summary>
    /// Gets or sets the title of the error summary.
    /// </summary>
    public string Title { get; set; } = "Error Summary";

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorSummary"/> class.
    /// </summary>
    public ErrorSummary()
    {
        _errors = new List<(string, Exception)>();
        _style = new ErrorSummaryStyle();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorSummary"/> class.
    /// </summary>
    /// <param name="failedTasks">The failed tasks to include in the summary.</param>
    public ErrorSummary(IEnumerable<CancellableProgressTask> failedTasks)
        : this()
    {
        if (failedTasks is null)
        {
            throw new ArgumentNullException(nameof(failedTasks));
        }

        foreach (var task in failedTasks.Where(t => t.IsFailed && t.Error != null))
        {
            _errors.Add((task.Description, task.Error!));
        }
    }

    /// <summary>
    /// Adds a failed task to the error summary.
    /// </summary>
    /// <param name="taskName">The name of the failed task.</param>
    /// <param name="error">The error that caused the failure.</param>
    public void AddError(string taskName, Exception error)
    {
        if (string.IsNullOrWhiteSpace(taskName))
        {
            throw new ArgumentException("Task name cannot be empty.", nameof(taskName));
        }

        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        _errors.Add((taskName, error));
    }

    /// <summary>
    /// Gets a value indicating whether the error summary contains any errors.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <inheritdoc />
    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        var renderable = BuildRenderable();
        return renderable.Measure(options, maxWidth);
    }

    /// <inheritdoc />
    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var renderable = BuildRenderable();
        return renderable.Render(options, maxWidth);
    }

    private IRenderable BuildRenderable()
    {
        if (_errors.Count == 0)
        {
            return new Text(string.Empty);
        }

        var panel = new Panel(BuildContent())
        {
            Header = new PanelHeader(Title),
            Border = BoxBorder.Rounded,
            BorderStyle = _style.BorderColor,
            Padding = new Padding(1, 1),
        };

        return panel;
    }

    private IRenderable BuildContent()
    {
        var grid = new Grid();
        grid.AddColumn(new GridColumn().PadRight(2));
        grid.AddColumn(new GridColumn());

        for (var i = 0; i < _errors.Count; i++)
        {
            var (taskName, error) = _errors[i];

            var marker = i == _errors.Count - 1 ? "└─" : "├─";
            var errorPrefix = new Text(marker + " ", _style.MarkerColor);
            var taskHeader = new Text(taskName, _style.TaskNameColor);

            grid.AddRow(errorPrefix, taskHeader);

            var errorMessage = GetErrorMessage(error);
            var indentedMessage = new Text("   " + errorMessage, _style.ErrorMessageColor);
            grid.AddRow(Text.Empty, indentedMessage);

            if (i < _errors.Count - 1)
            {
                grid.AddRow(Text.Empty, Text.Empty);
            }
        }

        return grid;
    }

    private static string GetErrorMessage(Exception error)
    {
        var message = error.Message;
        if (string.IsNullOrWhiteSpace(message))
        {
            message = error.GetType().Name;
        }

        return message;
    }
}

/// <summary>
/// Represents the style settings for an error summary.
/// </summary>
internal sealed class ErrorSummaryStyle
{
    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    public Style BorderColor { get; set; } = Color.Red;

    /// <summary>
    /// Gets or sets the color of the marker.
    /// </summary>
    public Style MarkerColor { get; set; } = Color.Red;

    /// <summary>
    /// Gets or sets the color of the task name.
    /// </summary>
    public Style TaskNameColor { get; set; } = Color.Yellow;

    /// <summary>
    /// Gets or sets the color of the error message.
    /// </summary>
    public Style ErrorMessageColor { get; set; } = Color.Red;
}
