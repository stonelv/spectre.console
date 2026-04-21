namespace Spectre.Console.Cli;

/// <summary>
/// A renderable panel that displays argument trace information.
/// </summary>
public class ArgumentTracePanel : Renderable
{
    private readonly ArgumentTraceContext _context;
    private readonly ArgumentTraceStyles _styles;
    private readonly bool _showDetails;
    private readonly bool _showSummary;

    /// <summary>
    /// Gets or sets the panel border.
    /// </summary>
    public BoxBorder Border { get; set; } = BoxBorder.Rounded;

    /// <summary>
    /// Gets or sets a value indicating whether the panel should expand to fit the available space.
    /// </summary>
    public bool Expand { get; set; } = true;

    /// <summary>
    /// Gets or sets the header of the panel.
    /// </summary>
    public PanelHeader? Header { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTracePanel"/> class.
    /// </summary>
    /// <param name="context">The argument trace context.</param>
    /// <param name="styles">The styles to use for rendering.</param>
    /// <param name="showDetails">Whether to show detailed information.</param>
    /// <param name="showSummary">Whether to show a summary of argument sources.</param>
    public ArgumentTracePanel(
        ArgumentTraceContext context,
        ArgumentTraceStyles? styles = null,
        bool showDetails = false,
        bool showSummary = true)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _styles = styles ?? ArgumentTraceStyles.Default;
        _showDetails = showDetails;
        _showSummary = showSummary;
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var panel = BuildPanel();
        return ((IRenderable)panel).Measure(options, maxWidth);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var panel = BuildPanel();
        return ((IRenderable)panel).Render(options, maxWidth);
    }

    private Panel BuildPanel()
    {
        var contentItems = new List<IRenderable>();

        if (!string.IsNullOrEmpty(_context.ParsingError))
        {
            var errorPanel = new Panel(
                new Markup($"[{_styles.ErrorStyle.ToMarkup()}]Parsing Error: {_context.ParsingError.EscapeMarkup()}[/]"))
            {
                Border = BoxBorder.None,
                Padding = new Padding(0, 0, 0, 1)
            };
            contentItems.Add(errorPanel);
        }

        var table = new ArgumentTraceTable(_context, _styles, _showDetails)
        {
            Border = TableBorder.Minimal,
            Expand = Expand
        };
        contentItems.Add(table);

        if (_showSummary)
        {
            var summary = BuildSummary();
            if (summary != null)
            {
                contentItems.Add(Text.Empty);
                contentItems.Add(summary);
            }
        }

        var content = new Rows(contentItems);

        var panel = new Panel(content)
        {
            Border = Border,
            BorderStyle = _styles.BorderStyle,
            Expand = Expand,
            Header = Header ?? CreateDefaultHeader()
        };

        return panel;
    }

    private PanelHeader? CreateDefaultHeader()
    {
        var title = _context.CommandName != null
            ? $"Argument Trace: {_context.CommandName}"
            : "Argument Trace";

        return new PanelHeader(
            $"[{_styles.HeaderStyle.ToMarkup()}]{title}[/]",
            Justify.Center);
    }

    private IRenderable? BuildSummary()
    {
        var summary = _context.GetSourceSummary();
        if (summary.Count == 0)
        {
            return null;
        }

        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddColumn(new GridColumn());

        grid.AddRow(
            new Markup($"[{_styles.HeaderStyle.ToMarkup()}]Source Summary[/]"),
            Text.Empty);

        foreach (var (source, count) in summary.OrderBy(s => s.Key))
        {
            var sourceStyle = _styles.GetSourceStyle(source);
            var sourceName = source switch
            {
                ArgumentSource.DefaultValue => "Default Value",
                ArgumentSource.CommandLine => "Command Line",
                ArgumentSource.EnvironmentVariable => "Environment Variable",
                ArgumentSource.ConfigurationFile => "Configuration File",
                ArgumentSource.NotProvided => "Not Provided",
                _ => source.ToString()
            };

            grid.AddRow(
                new Markup($"  [{sourceStyle.ToMarkup()}]• {sourceName}[/]"),
                new Markup($"{count} argument{(count != 1 ? "s" : "")}"));
        }

        return grid;
    }
}
