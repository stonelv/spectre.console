namespace Spectre.Console;

/// <summary>
/// Represents a group of consecutive log entries with the same level.
/// </summary>
internal class LogGroup
{
    /// <summary>
    /// Gets the log level of this group.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the entries in this group.
    /// </summary>
    public List<LogEntry> Entries { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether this group is collapsed.
    /// </summary>
    public bool IsCollapsed { get; set; }

    /// <summary>
    /// Gets the count of entries in this group.
    /// </summary>
    public int Count => Entries.Count;

    /// <summary>
    /// Gets the first entry in this group.
    /// </summary>
    public LogEntry? FirstEntry => Entries.Count > 0 ? Entries[0] : null;

    /// <summary>
    /// Gets the last entry in this group.
    /// </summary>
    public LogEntry? LastEntry => Entries.Count > 0 ? Entries[Entries.Count - 1] : null;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogGroup"/> class.
    /// </summary>
    /// <param name="level">The log level for this group.</param>
    public LogGroup(LogLevel level)
    {
        Level = level;
    }

    /// <summary>
    /// Adds an entry to this group.
    /// </summary>
    /// <param name="entry">The entry to add.</param>
    public void AddEntry(LogEntry entry)
    {
        Entries.Add(entry);
    }

    /// <summary>
    /// Toggles the collapsed state of this group.
    /// </summary>
    public void ToggleCollapse()
    {
        IsCollapsed = !IsCollapsed;
    }
}
