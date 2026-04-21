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

    internal bool IsGrid { get; set; }
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
        var totalCellWidth = measurer.CalculateTotalCellWidth(maxWidth);
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
        var totalCellWidth = measurer.CalculateTotalCellWidth(maxWidth);
        var columnWidths = measurer.CalculateColumnWidths(totalCellWidth);
        var tableWidth = columnWidths.Sum() + measurer.GetNonColumnWidth();
        var rows = GetRenderableRows();

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
            var columnText = TableSortHelper.GetPlainText(_columns[i].Header);
            if (columnText.Equals(columnName, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
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
                var sorted = _sortDescending
                    ? dataRows.Select(row => (Row: row, Key: TableSortHelper.CreateSortKey(row, columnIndex)))
                        .OrderByDescending(x => x.Key, TableSortKeyComparer.Default)
                    : dataRows.Select(row => (Row: row, Key: TableSortHelper.CreateSortKey(row, columnIndex)))
                        .OrderBy(x => x.Key, TableSortKeyComparer.Default);

                dataRows = sorted.Select(x => x.Row).ToList();
            }
        }

        rows.AddRange(dataRows);

        if (ShowFooters && _columns.Any(c => c.Footer != null))
        {
            rows.Add(TableRow.Footer(_columns.Select(c => c.Footer ?? Text.Empty)));
        }

        return rows;
    }
}

internal enum TableSortKeyType
{
    Empty = 0,
    Numeric = 1,
    DateTime = 2,
    String = 3,
}

internal readonly struct TableSortKey : IEquatable<TableSortKey>
{
    public TableSortKeyType Type { get; }
    public string RawText { get; }
    public decimal? NumericValue { get; }
    public DateTimeOffset? DateTimeValue { get; }

    private TableSortKey(TableSortKeyType type, string rawText, decimal? numericValue = null, DateTimeOffset? dateTimeValue = null)
    {
        Type = type;
        RawText = rawText;
        NumericValue = numericValue;
        DateTimeValue = dateTimeValue;
    }

    public static TableSortKey Empty => new(TableSortKeyType.Empty, string.Empty);

    public static TableSortKey ForString(string text)
    {
        return new TableSortKey(TableSortKeyType.String, text);
    }

    public static TableSortKey ForNumeric(decimal value, string rawText)
    {
        return new TableSortKey(TableSortKeyType.Numeric, rawText, numericValue: value);
    }

    public static TableSortKey ForDateTime(DateTimeOffset value, string rawText)
    {
        return new TableSortKey(TableSortKeyType.DateTime, rawText, dateTimeValue: value);
    }

    public bool Equals(TableSortKey other)
    {
        if (Type != other.Type)
            return false;

        return Type switch
        {
            TableSortKeyType.Empty => true,
            TableSortKeyType.Numeric => NumericValue == other.NumericValue,
            TableSortKeyType.DateTime => DateTimeValue == other.DateTimeValue,
            TableSortKeyType.String => RawText == other.RawText,
            _ => false,
        };
    }

    public override bool Equals(object? obj)
    {
        return obj is TableSortKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return Type switch
            {
                TableSortKeyType.Empty => 0,
                TableSortKeyType.Numeric => ((int)Type * 397) ^ (NumericValue?.GetHashCode() ?? 0),
                TableSortKeyType.DateTime => ((int)Type * 397) ^ (DateTimeValue?.GetHashCode() ?? 0),
                TableSortKeyType.String => ((int)Type * 397) ^ (RawText?.GetHashCode() ?? 0),
                _ => 0,
            };
        }
    }

    public static bool operator ==(TableSortKey left, TableSortKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TableSortKey left, TableSortKey right)
    {
        return !(left == right);
    }
}

internal sealed class TableSortKeyComparer : IComparer<TableSortKey>
{
    public static TableSortKeyComparer Default { get; } = new();

    public int Compare(TableSortKey x, TableSortKey y)
    {
        if (x.Type != y.Type)
        {
            return x.Type.CompareTo(y.Type);
        }

        return x.Type switch
        {
            TableSortKeyType.Empty => 0,
            TableSortKeyType.Numeric => (x.NumericValue ?? 0).CompareTo(y.NumericValue ?? 0),
            TableSortKeyType.DateTime => (x.DateTimeValue ?? default).CompareTo(y.DateTimeValue ?? default),
            TableSortKeyType.String => StringComparer.Ordinal.Compare(x.RawText, y.RawText),
            _ => 0,
        };
    }
}

internal static class TableSortHelper
{
    private static readonly RenderOptions SortRenderOptions = new(
        SortCapabilities.Default,
        new Size(int.MaxValue, int.MaxValue))
    {
        SingleLine = true
    };

    public static string GetPlainText(IRenderable? renderable)
    {
        if (renderable == null)
        {
            return string.Empty;
        }

        try
        {
            var segments = renderable.Render(SortRenderOptions, int.MaxValue);
            var textBuilder = new StringBuilder();

            foreach (var segment in segments)
            {
                if (segment != null && !segment.IsControlCode)
                {
                    textBuilder.Append(segment.Text);
                }
            }

            return textBuilder.ToString();
        }
        catch
        {
            return renderable.ToString() ?? string.Empty;
        }
    }

    public static TableSortKey CreateSortKey(TableRow row, int columnIndex)
    {
        if (columnIndex >= row.Count)
        {
            return TableSortKey.Empty;
        }

        var cell = row[columnIndex];
        var text = GetPlainText(cell);

        if (string.IsNullOrWhiteSpace(text))
        {
            return TableSortKey.Empty;
        }

        var trimmedText = text.Trim();

        if (decimal.TryParse(trimmedText, out var decimalValue))
        {
            return TableSortKey.ForNumeric(decimalValue, trimmedText);
        }

        if (DateTimeOffset.TryParse(trimmedText, out var dateTimeValue))
        {
            return TableSortKey.ForDateTime(dateTimeValue, trimmedText);
        }

        return TableSortKey.ForString(trimmedText);
    }
}

internal sealed class SortCapabilities : IReadOnlyCapabilities
{
    public static SortCapabilities Default { get; } = new();

    public ColorSystem ColorSystem => ColorSystem.NoColors;
    public bool Ansi => false;
    public bool Links => false;
    public bool Legacy => false;
    public bool IsTerminal => false;
    public bool Interactive => false;
    public bool Unicode => true;
}
