namespace Spectre.Console;

/// <summary>
/// A renderable log view widget with support for coloring, filtering, and grouping.
/// </summary>
public sealed class LogView : Renderable, IHasBoxBorder, IHasBorder, IExpandable
{
    private readonly LinkedList<LogEntry> _entries = new();
    private readonly List<LogGroup> _groups = new();
    private readonly object _lock = new();
    private int? _maxEntries;
    private string? _filterKeyword;
    private bool _isPaused;
    private int _scrollOffset;
    private bool _autoGrouping = true;
    private int _groupThreshold = 5;
    private int? _maxVisibleRows;
    private BoxBorder _border = BoxBorder.Square;
    private bool _showTimestamp = true;
    private bool _showLevel = true;
    private bool _showCategory = true;
    private string _timestampFormat = "HH:mm:ss";
    private int _minWidthForFullView = 80;

    /// <summary>
    /// Gets or sets the maximum number of entries to keep in memory.
    /// </summary>
    public int? MaxEntries
    {
        get => _maxEntries;
        set => _maxEntries = value > 0 ? value : null;
    }

    /// <summary>
    /// Gets or sets the filter keyword for filtering log entries.
    /// </summary>
    public string? FilterKeyword
    {
        get => _filterKeyword;
        set
        {
            _filterKeyword = value;
            RegenerateGroups();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether scrolling is paused.
    /// </summary>
    public bool IsPaused
    {
        get => _isPaused;
        set => _isPaused = value;
    }

    /// <summary>
    /// Gets or sets the scroll offset from the bottom.
    /// </summary>
    public int ScrollOffset
    {
        get => _scrollOffset;
        set => _scrollOffset = Math.Max(0, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically group consecutive entries with the same level.
    /// </summary>
    public bool AutoGrouping
    {
        get => _autoGrouping;
        set
        {
            _autoGrouping = value;
            RegenerateGroups();
        }
    }

    /// <summary>
    /// Gets or sets the threshold for grouping (minimum number of consecutive entries required to form a group).
    /// </summary>
    public int GroupThreshold
    {
        get => _groupThreshold;
        set
        {
            _groupThreshold = Math.Max(2, value);
            RegenerateGroups();
        }
    }

    /// <summary>
    /// Gets or sets the maximum number of visible rows.
    /// </summary>
    public int? MaxVisibleRows
    {
        get => _maxVisibleRows;
        set => _maxVisibleRows = value > 0 ? value : null;
    }

    /// <summary>
    /// Gets or sets the border.
    /// </summary>
    public BoxBorder Border
    {
        get => _border;
        set => _border = value ?? BoxBorder.None;
    }

    /// <inheritdoc/>
    public bool UseSafeBorder { get; set; } = true;

    /// <inheritdoc/>
    public Style? BorderStyle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the log view should fit the available space.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// Gets or sets the width of the log view.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the log view.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show timestamps.
    /// </summary>
    public bool ShowTimestamp
    {
        get => _showTimestamp;
        set => _showTimestamp = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show log levels.
    /// </summary>
    public bool ShowLevel
    {
        get => _showLevel;
        set => _showLevel = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show categories.
    /// </summary>
    public bool ShowCategory
    {
        get => _showCategory;
        set => _showCategory = value;
    }

    /// <summary>
    /// Gets or sets the timestamp format.
    /// </summary>
    public string TimestampFormat
    {
        get => _timestampFormat;
        set => _timestampFormat = value ?? "HH:mm:ss";
    }

    /// <summary>
    /// Gets or sets the minimum width for full view (below this, a compact view is used).
    /// </summary>
    public int MinWidthForFullView
    {
        get => _minWidthForFullView;
        set => _minWidthForFullView = Math.Max(40, value);
    }

    /// <summary>
    /// Gets the styles used for rendering.
    /// </summary>
    public LogViewStyles Styles { get; } = new();

    /// <summary>
    /// Gets the number of entries in the log view.
    /// </summary>
    public int EntryCount => _entries.Count;

    /// <summary>
    /// Gets the number of groups in the log view.
    /// </summary>
    public int GroupCount
    {
        get
        {
            lock (_lock)
            {
                return _groups.Count;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogView"/> class.
    /// </summary>
    public LogView()
    {
    }

    /// <summary>
    /// Adds a log entry to the view.
    /// </summary>
    /// <param name="entry">The log entry to add.</param>
    public void AddEntry(LogEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        lock (_lock)
        {
            _entries.AddLast(entry);

            if (_maxEntries.HasValue && _entries.Count > _maxEntries.Value)
            {
                _entries.RemoveFirst();
                RegenerateGroups();
            }
            else
            {
                AddToGroup(entry);
            }

            if (!_isPaused)
            {
                _scrollOffset = 0;
            }
        }
    }

    /// <summary>
    /// Adds multiple log entries to the view.
    /// </summary>
    /// <param name="entries">The log entries to add.</param>
    public void AddEntries(IEnumerable<LogEntry> entries)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        lock (_lock)
        {
            var entryList = entries.ToList();
            if (entryList.Count == 0)
            {
                return;
            }

            foreach (var entry in entryList)
            {
                _entries.AddLast(entry);
            }

            if (_maxEntries.HasValue && _entries.Count > _maxEntries.Value)
            {
                var excessCount = _entries.Count - _maxEntries.Value;
                for (var i = 0; i < excessCount; i++)
                {
                    _entries.RemoveFirst();
                }

                RegenerateGroups();
            }
            else
            {
                foreach (var entry in entryList)
                {
                    AddToGroup(entry);
                }
            }

            if (!_isPaused)
            {
                _scrollOffset = 0;
            }
        }
    }

    /// <summary>
    /// Clears all log entries.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _entries.Clear();
            _groups.Clear();
            _scrollOffset = 0;
        }
    }

    /// <summary>
    /// Toggles the pause state.
    /// </summary>
    /// <returns><c>true</c> if scrolling is now paused; otherwise, <c>false</c>.</returns>
    public bool TogglePause()
    {
        _isPaused = !_isPaused;
        if (!_isPaused)
        {
            _scrollOffset = 0;
        }
        return _isPaused;
    }

    /// <summary>
    /// Scrolls up by the specified number of lines.
    /// </summary>
    /// <param name="lines">The number of lines to scroll up.</param>
    public void ScrollUp(int lines = 1)
    {
        _scrollOffset += lines;
        var maxOffset = GetTotalDisplayLines();
        if (_scrollOffset > maxOffset)
        {
            _scrollOffset = maxOffset;
        }
    }

    /// <summary>
    /// Scrolls down by the specified number of lines.
    /// </summary>
    /// <param name="lines">The number of lines to scroll down.</param>
    public void ScrollDown(int lines = 1)
    {
        _scrollOffset = Math.Max(0, _scrollOffset - lines);
    }

    /// <summary>
    /// Scrolls to the top.
    /// </summary>
    public void ScrollToTop()
    {
        var totalLines = GetTotalDisplayLines();
        if (_maxVisibleRows.HasValue && totalLines > _maxVisibleRows.Value)
        {
            _scrollOffset = totalLines - _maxVisibleRows.Value;
        }
        else
        {
            _scrollOffset = 0;
        }
    }

    /// <summary>
    /// Scrolls to the bottom.
    /// </summary>
    public void ScrollToBottom()
    {
        _scrollOffset = 0;
    }

    /// <summary>
    /// Toggles the collapse state of a specific group.
    /// </summary>
    /// <param name="groupIndex">The index of the group to toggle.</param>
    [Obsolete("Use ToggleGroupByLevel or GetGroups for more stable group selection.")]
    public void ToggleGroup(int groupIndex)
    {
        lock (_lock)
        {
            if (groupIndex >= 0 && groupIndex < _groups.Count)
            {
                _groups[groupIndex].ToggleCollapse();
            }
        }
    }

    /// <summary>
    /// Toggles the collapse state of all groups with the specified log level.
    /// </summary>
    /// <param name="level">The log level of the groups to toggle.</param>
    /// <returns>The number of groups that were toggled.</returns>
    public int ToggleGroupsByLevel(LogLevel level)
    {
        lock (_lock)
        {
            var count = 0;
            foreach (var group in _groups)
            {
                if (group.Level == level)
                {
                    group.ToggleCollapse();
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Collapses all groups.
    /// </summary>
    public void CollapseAllGroups()
    {
        lock (_lock)
        {
            foreach (var group in _groups)
            {
                group.IsCollapsed = true;
            }
        }
    }

    /// <summary>
    /// Collapses all groups with the specified log level.
    /// </summary>
    /// <param name="level">The log level of the groups to collapse.</param>
    /// <returns>The number of groups that were collapsed.</returns>
    public int CollapseGroupsByLevel(LogLevel level)
    {
        lock (_lock)
        {
            var count = 0;
            foreach (var group in _groups)
            {
                if (group.Level == level && !group.IsCollapsed)
                {
                    group.IsCollapsed = true;
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Expands all groups.
    /// </summary>
    public void ExpandAllGroups()
    {
        lock (_lock)
        {
            foreach (var group in _groups)
            {
                group.IsCollapsed = false;
            }
        }
    }

    /// <summary>
    /// Expands all groups with the specified log level.
    /// </summary>
    /// <param name="level">The log level of the groups to expand.</param>
    /// <returns>The number of groups that were expanded.</returns>
    public int ExpandGroupsByLevel(LogLevel level)
    {
        lock (_lock)
        {
            var count = 0;
            foreach (var group in _groups)
            {
                if (group.Level == level && group.IsCollapsed)
                {
                    group.IsCollapsed = false;
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Gets information about all groups.
    /// </summary>
    /// <returns>An enumerable of group information.</returns>
    public IEnumerable<LogGroupInfo> GetGroups()
    {
        lock (_lock)
        {
            return _groups.Select(g => new LogGroupInfo
            {
                Level = g.Level,
                Count = g.Count,
                IsCollapsed = g.IsCollapsed,
                FirstEntry = g.FirstEntry,
                LastEntry = g.LastEntry,
            }).ToList();
        }
    }

    /// <summary>
    /// Gets information about all groups with the specified log level.
    /// </summary>
    /// <param name="level">The log level to filter by.</param>
    /// <returns>An enumerable of group information for the specified level.</returns>
    public IEnumerable<LogGroupInfo> GetGroupsByLevel(LogLevel level)
    {
        lock (_lock)
        {
            return _groups
                .Where(g => g.Level == level)
                .Select(g => new LogGroupInfo
                {
                    Level = g.Level,
                    Count = g.Count,
                    IsCollapsed = g.IsCollapsed,
                    FirstEntry = g.FirstEntry,
                    LastEntry = g.LastEntry,
                }).ToList();
        }
    }

    /// <summary>
    /// Gets the count of groups with the specified log level.
    /// </summary>
    /// <param name="level">The log level to count.</param>
    /// <returns>The number of groups with the specified level.</returns>
    public int GetGroupCountByLevel(LogLevel level)
    {
        lock (_lock)
        {
            return _groups.Count(g => g.Level == level);
        }
    }

    private void AddToGroup(LogEntry entry)
    {
        if (!_autoGrouping)
        {
            return;
        }

        if (PassesFilter(entry))
        {
            if (_groups.Count == 0 || _groups[_groups.Count - 1].Level != entry.Level)
            {
                _groups.Add(new LogGroup(entry.Level));
            }

            _groups[_groups.Count - 1].AddEntry(entry);
        }
    }

    private void RegenerateGroups()
    {
        _groups.Clear();

        if (!_autoGrouping)
        {
            return;
        }

        LogGroup? currentGroup = null;

        foreach (var entry in _entries)
        {
            if (!PassesFilter(entry))
            {
                currentGroup = null;
                continue;
            }

            if (currentGroup == null || currentGroup.Level != entry.Level)
            {
                currentGroup = new LogGroup(entry.Level);
                _groups.Add(currentGroup);
            }

            currentGroup.AddEntry(entry);
        }
    }

    private bool PassesFilter(LogEntry entry)
    {
        if (string.IsNullOrEmpty(_filterKeyword))
        {
            return true;
        }

        return entry.Message.Contains(_filterKeyword, StringComparison.OrdinalIgnoreCase) ||
               (entry.Category != null && entry.Category.Contains(_filterKeyword, StringComparison.OrdinalIgnoreCase));
    }

    private int GetTotalDisplayLines()
    {
        lock (_lock)
        {
            if (!_autoGrouping)
            {
                return _entries.Count(e => PassesFilter(e));
            }

            var totalLines = 0;
            foreach (var group in _groups)
            {
                if (group.Count < _groupThreshold || !group.IsCollapsed)
                {
                    totalLines += group.Count;
                }
                else
                {
                    totalLines += 1;
                }
            }

            return totalLines;
        }
    }

    /// <inheritdoc/>
    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        var width = Width ?? maxWidth;
        var actualWidth = Math.Min(width, maxWidth);
        return new Measurement(actualWidth, actualWidth);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var border = options.GetSafeBorder(this);
        var showBorder = border is not NoBoxBorder;
        var edgeWidth = showBorder ? 2 : 0;
        var innerWidth = maxWidth - edgeWidth;
        var useCompactView = innerWidth < _minWidthForFullView;

        var result = new List<Segment>();
        var borderStyle = BorderStyle ?? Style.Plain;

        if (showBorder)
        {
            AddTopBorder(result, border, borderStyle, maxWidth);
        }

        var lines = GetRenderLines(innerWidth, useCompactView, options);
        var maxHeight = _maxVisibleRows ?? Height ?? options.Height;
        var visibleLines = GetVisibleLines(lines, maxHeight);

        foreach (var line in visibleLines)
        {
            if (showBorder)
            {
                result.Add(new Segment(border.GetPart(BoxBorderPart.Left), borderStyle));
            }

            result.AddRange(line);

            var lineLength = line.Sum(s => s.CellCount());
            if (lineLength < innerWidth)
            {
                result.Add(Segment.Padding(innerWidth - lineLength));
            }

            if (showBorder)
            {
                result.Add(new Segment(border.GetPart(BoxBorderPart.Right), borderStyle));
            }

            result.Add(Segment.LineBreak);
        }

        if (showBorder)
        {
            AddBottomBorder(result, border, borderStyle, maxWidth);
        }

        return result;
    }

    private List<List<Segment>> GetRenderLines(int innerWidth, bool useCompactView, RenderOptions options)
    {
        var lines = new List<List<Segment>>();

        lock (_lock)
        {
            if (!_autoGrouping)
            {
                foreach (var entry in _entries)
                {
                    if (PassesFilter(entry))
                    {
                        lines.Add(RenderEntry(entry, innerWidth, useCompactView));
                    }
                }
            }
            else
            {
                foreach (var group in _groups)
                {
                    if (group.Count < _groupThreshold || !group.IsCollapsed)
                    {
                        foreach (var entry in group.Entries)
                        {
                            lines.Add(RenderEntry(entry, innerWidth, useCompactView));
                        }
                    }
                    else
                    {
                        lines.Add(RenderCollapsedGroup(group, innerWidth));
                    }
                }
            }
        }

        return lines;
    }

    private List<List<Segment>> GetVisibleLines(List<List<Segment>> allLines, int? maxHeight)
    {
        if (allLines.Count == 0)
        {
            return new List<List<Segment>>();
        }

        var visibleCount = maxHeight ?? allLines.Count;
        if (visibleCount > allLines.Count)
        {
            visibleCount = allLines.Count;
        }

        var startIndex = Math.Max(0, allLines.Count - visibleCount - _scrollOffset);
        var result = new List<List<Segment>>();

        for (var i = startIndex; i < startIndex + visibleCount && i < allLines.Count; i++)
        {
            result.Add(allLines[i]);
        }

        return result;
    }

    private List<Segment> RenderEntry(LogEntry entry, int width, bool useCompactView)
    {
        var segments = new List<Segment>();
        var levelStyle = Styles.GetLevelStyle(entry.Level);

        if (useCompactView)
        {
            if (_showLevel)
            {
                var levelAbbr = GetLevelAbbreviation(entry.Level);
                segments.Add(new Segment($"[{levelAbbr}] ", levelStyle));
            }

            segments.AddRange(RenderMessageWithHighlight(entry.Message, Styles.MessageStyle));
        }
        else
        {
            if (_showTimestamp && entry.Timestamp.HasValue)
            {
                var timestamp = entry.Timestamp.Value.ToString(_timestampFormat);
                segments.Add(new Segment($"{timestamp} ", Styles.TimestampStyle));
            }

            if (_showLevel)
            {
                var levelName = GetLevelName(entry.Level).PadRight(7);
                segments.Add(new Segment($"{levelName} ", levelStyle));
            }

            if (_showCategory && !string.IsNullOrEmpty(entry.Category))
            {
                segments.Add(new Segment($"[{entry.Category}] ", Styles.CategoryStyle));
            }

            segments.AddRange(RenderMessageWithHighlight(entry.Message, Styles.MessageStyle));
        }

        return segments;
    }

    private List<Segment> RenderMessageWithHighlight(string message, Style defaultStyle)
    {
        var segments = new List<Segment>();

        if (string.IsNullOrEmpty(_filterKeyword))
        {
            segments.Add(new Segment(message, defaultStyle));
            return segments;
        }

        var remaining = message;
        var comparison = StringComparison.OrdinalIgnoreCase;

        while (!string.IsNullOrEmpty(remaining))
        {
            var index = remaining.IndexOf(_filterKeyword, comparison);
            if (index < 0)
            {
                segments.Add(new Segment(remaining, defaultStyle));
                break;
            }

            if (index > 0)
            {
                segments.Add(new Segment(remaining[..index], defaultStyle));
            }

            segments.Add(new Segment(remaining.Substring(index, _filterKeyword.Length), Styles.HighlightStyle));
            remaining = remaining[(index + _filterKeyword.Length)..];
        }

        return segments;
    }

    private List<Segment> RenderCollapsedGroup(LogGroup group, int width)
    {
        var segments = new List<Segment>();
        var levelStyle = Styles.GetLevelStyle(group.Level);

        segments.Add(new Segment("▶ ", Styles.CollapsedGroupStyle));
        segments.Add(new Segment($"{GetLevelName(group.Level)} ({group.Count} entries) ", levelStyle));

        if (group.FirstEntry != null)
        {
            segments.Add(new Segment($"- {group.FirstEntry.Message}", Styles.MessageStyle));
        }

        return segments;
    }

    private static string GetLevelName(LogLevel level)
    {
        return level switch
        {
            LogLevel.Verbose => "VRB",
            LogLevel.Debug => "DBG",
            LogLevel.Info => "INF",
            LogLevel.Warn => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Fatal => "FTL",
            _ => "???",
        };
    }

    private static string GetLevelAbbreviation(LogLevel level)
    {
        return level switch
        {
            LogLevel.Verbose => "V",
            LogLevel.Debug => "D",
            LogLevel.Info => "I",
            LogLevel.Warn => "W",
            LogLevel.Error => "E",
            LogLevel.Fatal => "F",
            _ => "?",
        };
    }

    private void AddTopBorder(List<Segment> result, BoxBorder border, Style borderStyle, int width)
    {
        result.Add(new Segment(border.GetPart(BoxBorderPart.TopLeft), borderStyle));
        result.Add(new Segment(border.GetPart(BoxBorderPart.Top).Repeat(width - 2), borderStyle));
        result.Add(new Segment(border.GetPart(BoxBorderPart.TopRight), borderStyle));
        result.Add(Segment.LineBreak);
    }

    private void AddBottomBorder(List<Segment> result, BoxBorder border, Style borderStyle, int width)
    {
        result.Add(new Segment(border.GetPart(BoxBorderPart.BottomLeft), borderStyle));
        result.Add(new Segment(border.GetPart(BoxBorderPart.Bottom).Repeat(width - 2), borderStyle));
        result.Add(new Segment(border.GetPart(BoxBorderPart.BottomRight), borderStyle));
        result.Add(Segment.LineBreak);
    }
}

/// <summary>
/// Provides information about a log group for external access.
/// </summary>
public sealed class LogGroupInfo
{
    /// <summary>
    /// Gets the log level of this group.
    /// </summary>
    public LogLevel Level { get; init; }

    /// <summary>
    /// Gets the number of entries in this group.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets a value indicating whether this group is collapsed.
    /// </summary>
    public bool IsCollapsed { get; init; }

    /// <summary>
    /// Gets the first entry in this group.
    /// </summary>
    public LogEntry? FirstEntry { get; init; }

    /// <summary>
    /// Gets the last entry in this group.
    /// </summary>
    public LogEntry? LastEntry { get; init; }
}
