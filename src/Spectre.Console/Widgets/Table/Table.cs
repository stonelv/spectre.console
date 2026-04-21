using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Spectre.Console.Rendering;

namespace Spectre.Console;

/// <summary>
/// A renderable table.
/// </summary>
public sealed class Table : Renderable, IHasTableBorder, IExpandable, IAlignable
{
    private readonly List<TableColumn> _columns;
    private int? _sortColumnIndex;
    private bool _sortDescending;
    private IComparer<TableRow>? _sortComparer;

    /// <summary>
    /// Gets the table columns.
    /// </summary>
    public IReadOnlyList<TableColumn> Columns => _columns;

    /// <summary>
    /// Gets the table rows.
    /// </summary>
    public TableRowCollection Rows { get; }

    internal int? SortColumnIndex => _sortColumnIndex;
    internal bool SortDescending => _sortDescending;
    internal IComparer<TableRow>? SortComparer => _sortComparer;

    /// <inheritdoc/>
    public TableBorder Border { get; set; } = TableBorder.Square;

    /// <inheritdoc/>
    public Style? BorderStyle { get; set; }

    /// <inheritdoc/>
    public bool UseSafeBorder { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not table headers should be shown.
    /// </summary>
    public bool ShowHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not row separators should be shown.
    /// </summary>
    public bool ShowRowSeparators { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not table footers should be shown.
    /// </summary>
    public bool ShowFooters { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the table should
    /// fit the available space. If <c>false</c>, the table width will be
    /// auto calculated. Defaults to <c>false</c>.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// Gets or sets the width of the table.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the table title.
    /// </summary>
    public TableTitle? Title { get; set; }

    /// <summary>
    /// Gets or sets the table footnote.
    /// </summary>
    public TableTitle? Caption { get; set; }

    /// <inheritdoc/>
    [Obsolete("Use the Align widget instead. This property will be removed in a later release.")]
    public Justify? Alignment { get; set; }

    // Whether this is a grid or not.
    internal bool IsGrid { get; set; }

    // Whether or not the most right cell should be padded.
    // This is almost always the case, unless we're rendering
    // a grid without explicit padding in the last cell.
    internal bool PadRightCell { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Table"/> class.
    /// </summary>
    public Table()
    {
        _columns = new List<TableColumn>();
        Rows = new TableRowCollection(this);
    }

    /// <summary>
    /// Adds a column to the table.
    /// </summary>
    /// <param name="column">The column to add.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public Table AddColumn(TableColumn column)
    {
        if (column is null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        if (Rows.Count > 0)
        {
            throw new InvalidOperationException("Cannot add new columns to table with existing rows.");
        }

        _columns.Add(column);
        return this;
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var measurer = new TableMeasurer(this, options);

        // Calculate the total cell width
        var totalCellWidth = measurer.CalculateTotalCellWidth(maxWidth);

        // Calculate the minimum and maximum table width
        var measurements = _columns.Select(column => measurer.MeasureColumn(column, totalCellWidth));
        var minTableWidth = measurements.Sum(x => x.Min) + measurer.GetNonColumnWidth();
        var maxTableWidth = Width ?? measurements.Sum(x => x.Max) + measurer.GetNonColumnWidth();
        return new Measurement(minTableWidth, maxTableWidth);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var measurer = new TableMeasurer(this, options);

        // Calculate the column and table width
        var totalCellWidth = measurer.CalculateTotalCellWidth(maxWidth);
        var columnWidths = measurer.CalculateColumnWidths(totalCellWidth);
        var tableWidth = columnWidths.Sum() + measurer.GetNonColumnWidth();

        // Get the rows to render
        var rows = GetRenderableRows();

        // Render the table
        return TableRenderer.Render(
            new TableRendererContext(this, options, rows, tableWidth, maxWidth),
            columnWidths);
    }

    internal void SetSort(int columnIndex, bool descending = false)
    {
        if (columnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), "Column index cannot be negative.");
        }

        _sortColumnIndex = columnIndex;
        _sortDescending = descending;
        _sortComparer = null;
    }

    internal void SetSort(string columnName, bool descending = false)
    {
        if (columnName is null)
        {
            throw new ArgumentNullException(nameof(columnName));
        }

        var index = FindColumnIndex(columnName);
        if (index == -1)
        {
            throw new ArgumentException($"Column '{columnName}' not found in the table.", nameof(columnName));
        }

        SetSort(index, descending);
    }

    internal void SetSort(IComparer<TableRow> comparer)
    {
        _sortComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        _sortColumnIndex = null;
        _sortDescending = false;
    }

    internal void ClearSort()
    {
        _sortColumnIndex = null;
        _sortDescending = false;
        _sortComparer = null;
    }

    private int FindColumnIndex(string columnName)
    {
        for (int i = 0; i < _columns.Count; i++)
        {
            var columnText = GetRenderableText(_columns[i].Header);
            if (columnText.Equals(columnName, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static string GetRenderableText(IRenderable? renderable)
    {
        if (renderable == null)
        {
            return string.Empty;
        }

        if (renderable is Text text)
        {
            return GetTextFromTextWidget(text);
        }

        if (renderable is Markup markup)
        {
            return GetTextFromMarkupWidget(markup);
        }

        return renderable.ToString() ?? string.Empty;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern",
        Justification = "Text and Markup types are well-known and preserved in the library.")]
    private static string GetTextFromTextWidget(Text text)
    {
        var paragraphField = typeof(Text).GetField("_paragraph", BindingFlags.Instance | BindingFlags.NonPublic);
        if (paragraphField == null)
        {
            return text.ToString() ?? string.Empty;
        }

        var paragraph = paragraphField.GetValue(text);
        return ExtractTextFromParagraph(paragraph);
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern",
        Justification = "Text and Markup types are well-known and preserved in the library.")]
    private static string GetTextFromMarkupWidget(Markup markup)
    {
        var paragraphField = typeof(Markup).GetField("_paragraph", BindingFlags.Instance | BindingFlags.NonPublic);
        if (paragraphField == null)
        {
            return markup.ToString() ?? string.Empty;
        }

        var paragraph = paragraphField.GetValue(markup);
        return ExtractTextFromParagraph(paragraph);
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern",
        Justification = "Paragraph and Segment types are well-known and preserved in the library.")]
    private static string ExtractTextFromParagraph(object? paragraph)
    {
        if (paragraph == null)
        {
            return string.Empty;
        }

        var linesField = typeof(Paragraph).GetField("_lines", BindingFlags.Instance | BindingFlags.NonPublic);
        if (linesField == null)
        {
            return paragraph.ToString() ?? string.Empty;
        }

        var lines = linesField.GetValue(paragraph) as List<SegmentLine>;
        if (lines == null)
        {
            return string.Empty;
        }

        var textBuilder = new StringBuilder();
        foreach (var line in lines)
        {
            if (line == null) continue;

            foreach (var segment in line)
            {
                if (segment != null && segment.Text != null)
                {
                    textBuilder.Append(segment.Text);
                }
            }
        }

        return textBuilder.ToString();
    }

    private List<TableRow> GetRenderableRows()
    {
        var rows = new List<TableRow>();

        if (ShowHeaders)
        {
            rows.Add(TableRow.Header(_columns.Select(c => c.Header)));
        }

        var dataRows = Rows.ToList();

        if (_sortComparer != null)
        {
            dataRows.Sort(_sortComparer);
        }
        else if (_sortColumnIndex.HasValue)
        {
            var columnIndex = _sortColumnIndex.Value;
            if (columnIndex < _columns.Count)
            {
                dataRows = _sortDescending
                    ? dataRows.OrderByDescending(row => GetSortKey(row, columnIndex)).ToList()
                    : dataRows.OrderBy(row => GetSortKey(row, columnIndex)).ToList();
            }
        }

        rows.AddRange(dataRows);

        if (ShowFooters && _columns.Any(c => c.Footer != null))
        {
            rows.Add(TableRow.Footer(_columns.Select(c => c.Footer ?? Text.Empty)));
        }

        return rows;
    }

    private static object GetSortKey(TableRow row, int columnIndex)
    {
        if (columnIndex >= row.Count)
        {
            return string.Empty;
        }

        var cell = row[columnIndex];
        var text = GetRenderableText(cell);

        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (int.TryParse(text, out var intValue))
        {
            return intValue;
        }

        if (long.TryParse(text, out var longValue))
        {
            return longValue;
        }

        if (decimal.TryParse(text, out var decimalValue))
        {
            return decimalValue;
        }

        if (DateTime.TryParse(text, out var dateValue))
        {
            return dateValue;
        }

        if (DateTimeOffset.TryParse(text, out var dateOffsetValue))
        {
            return dateOffsetValue;
        }

        return text;
    }
}