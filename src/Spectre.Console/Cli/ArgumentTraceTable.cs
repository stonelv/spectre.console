namespace Spectre.Console.Cli;

/// <summary>
/// A renderable table that displays argument trace information.
/// </summary>
public class ArgumentTraceTable : Renderable
{
    private readonly ArgumentTraceContext _context;
    private readonly ArgumentTraceStyles _styles;
    private readonly bool _showDetails;

    /// <summary>
    /// Gets or sets the table border.
    /// </summary>
    public TableBorder Border { get; set; } = TableBorder.Square;

    /// <summary>
    /// Gets or sets a value indicating whether the table should expand to fit the available space.
    /// </summary>
    public bool Expand { get; set; } = true;

    /// <summary>
    /// Gets or sets the title of the table.
    /// </summary>
    public TableTitle? Title { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentTraceTable"/> class.
    /// </summary>
    /// <param name="context">The argument trace context.</param>
    /// <param name="styles">The styles to use for rendering.</param>
    /// <param name="showDetails">Whether to show detailed information.</param>
    public ArgumentTraceTable(
        ArgumentTraceContext context,
        ArgumentTraceStyles? styles = null,
        bool showDetails = false)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _styles = styles ?? ArgumentTraceStyles.Default;
        _showDetails = showDetails;
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var table = BuildTable();
        return ((IRenderable)table).Measure(options, maxWidth);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var table = BuildTable();
        return ((IRenderable)table).Render(options, maxWidth);
    }

    private Table BuildTable()
    {
        var table = new Table
        {
            Border = Border,
            BorderStyle = _styles.BorderStyle,
            Expand = Expand,
            Title = Title
        };

        table.AddColumn(new TableColumn(new Markup("Argument", _styles.HeaderStyle)));
        table.AddColumn(new TableColumn(new Markup("Value", _styles.HeaderStyle)));
        table.AddColumn(new TableColumn(new Markup("Source", _styles.HeaderStyle)));

        if (_showDetails)
        {
            table.AddColumn(new TableColumn(new Markup("Details", _styles.HeaderStyle)));
        }

        foreach (var arg in _context.Arguments)
        {
            var cells = new List<IRenderable>();

            var nameBuilder = new StringBuilder();
            nameBuilder.Append($"[{_styles.NameStyle.ToMarkup()}]{arg.FullName.EscapeMarkup()}[/]");

            if (arg.IsRequired)
            {
                nameBuilder.Append($" [{_styles.RequiredStyle.ToMarkup()}]*[/]");
            }

            cells.Add(new Markup(nameBuilder.ToString()));

            var valueText = arg.ValidationError != null
                ? $"[{_styles.ErrorStyle.ToMarkup()}]{arg.FormattedValue.EscapeMarkup()}[/]"
                : $"[{_styles.ValueStyle.ToMarkup()}]{arg.FormattedValue.EscapeMarkup()}[/]";
            cells.Add(new Markup(valueText));

            var sourceStyle = _styles.GetSourceStyle(arg.Source);
            var sourceText = $"[{sourceStyle.ToMarkup()}]{arg.SourceDisplayName}[/]";
            cells.Add(new Markup(sourceText));

            if (_showDetails)
            {
                var detailsBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(arg.Description))
                {
                    detailsBuilder.AppendLine($"[{_styles.DescriptionStyle.ToMarkup()}]{arg.Description.EscapeMarkup()}[/]");
                }

                if (!string.IsNullOrEmpty(arg.SourceDetails))
                {
                    detailsBuilder.AppendLine($"[{_styles.DescriptionStyle.ToMarkup()}]From: {arg.SourceDetails.EscapeMarkup()}[/]");
                }

                if (arg.HasDefaultValue && arg.Source != ArgumentSource.DefaultValue)
                {
                    var defaultValue = arg.DefaultValue?.ToString() ?? "(null)";
                    detailsBuilder.AppendLine($"[{_styles.DefaultValueStyle.ToMarkup()}]Default: {defaultValue.EscapeMarkup()}[/]");
                }

                if (!string.IsNullOrEmpty(arg.ValidationError))
                {
                    detailsBuilder.AppendLine($"[{_styles.ErrorStyle.ToMarkup()}]Error: {arg.ValidationError.EscapeMarkup()}[/]");
                }

                cells.Add(new Markup(detailsBuilder.ToString().TrimEnd()));
            }

            table.AddRow(cells.ToArray());
        }

        return table;
    }
}
