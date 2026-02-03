namespace Spectre.Console;

/// <summary>
/// Represents the result of a diff operation.
/// </summary>
public sealed class DiffResult
{
    private readonly List<DiffLine> _lines;

    /// <summary>
    /// Gets the collection of diff lines.
    /// </summary>
    public IReadOnlyList<DiffLine> Lines => _lines;

    /// <summary>
    /// Gets the number of inserted lines.
    /// </summary>
    public int InsertedCount { get; }

    /// <summary>
    /// Gets the number of deleted lines.
    /// </summary>
    public int DeletedCount { get; }

    /// <summary>
    /// Gets the number of unchanged lines.
    /// </summary>
    public int UnchangedCount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffResult"/> class.
    /// </summary>
    /// <param name="lines">The diff lines.</param>
    public DiffResult(IEnumerable<DiffLine> lines)
    {
        _lines = lines?.ToList() ?? new List<DiffLine>();

        foreach (var line in _lines)
        {
            switch (line.Type)
            {
                case DiffLineType.Inserted:
                    InsertedCount++;
                    break;
                case DiffLineType.Deleted:
                    DeletedCount++;
                    break;
                case DiffLineType.Unchanged:
                    UnchangedCount++;
                    break;
            }
        }
    }
}
