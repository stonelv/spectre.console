namespace Spectre.Console.Tests.Unit;

public class LogViewPerformanceTests
{
    private const int LargeEntryCount = 10000;
    private const int MediumEntryCount = 1000;
    private const int SmallEntryCount = 100;

    [Fact]
    public void Should_Add_Large_Number_Of_Entries_Quickly()
    {
        var logView = new LogView();
        var entries = GenerateEntries(LargeEntryCount);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        logView.AddEntries(entries);
        stopwatch.Stop();

        logView.EntryCount.ShouldBe(LargeEntryCount);

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 1000, $"Adding {LargeEntryCount} entries took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Render_Large_Number_Of_Entries_Quickly()
    {
        var console = new TestConsole();
        var logView = new LogView
        {
            MaxVisibleRows = 50,
            Border = BoxBorder.None,
        };

        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        console.Write(logView);
        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 500, $"Rendering {LargeEntryCount} entries took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Filter_Quickly()
    {
        var logView = new LogView();
        var entries = GenerateEntriesWithKeywords(LargeEntryCount);
        logView.AddEntries(entries);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        logView.FilterKeyword = "error";
        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 100, $"Filtering {LargeEntryCount} entries took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Group_Quickly()
    {
        var logView = new LogView
        {
            GroupThreshold = 3,
            AutoGrouping = true,
        };

        var entries = GenerateGroupedEntries(LargeEntryCount);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        logView.AddEntries(entries);
        stopwatch.Stop();

        logView.EntryCount.ShouldBe(LargeEntryCount);

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 1000, $"Grouping {LargeEntryCount} entries took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Handle_MaxEntries_Efficiently()
    {
        var logView = new LogView
        {
            MaxEntries = MediumEntryCount,
        };

        var entries = GenerateEntries(LargeEntryCount);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        logView.AddEntries(entries);
        stopwatch.Stop();

        logView.EntryCount.ShouldBe(MediumEntryCount);

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 500, $"Adding {LargeEntryCount} entries with MaxEntries={MediumEntryCount} took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Render_With_Highlighting_Quickly()
    {
        var console = new TestConsole();
        var logView = new LogView
        {
            MaxVisibleRows = 50,
            Border = BoxBorder.None,
            FilterKeyword = "test",
        };

        var entries = GenerateEntriesWithKeywords(LargeEntryCount);
        logView.AddEntries(entries);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        console.Write(logView);
        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 1000, $"Rendering with highlighting took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Scroll_Quickly()
    {
        var logView = new LogView
        {
            MaxVisibleRows = 20,
        };

        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (var i = 0; i < 100; i++)
        {
            logView.ScrollUp(10);
            logView.ScrollDown(5);
        }

        logView.ScrollToTop();
        logView.ScrollToBottom();

        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 100, $"Scrolling operations took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Toggle_Pause_Quickly()
    {
        var logView = new LogView();
        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (var i = 0; i < 1000; i++)
        {
            logView.TogglePause();
        }

        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 10, $"Toggling pause 1000 times took {elapsedMs}ms, which is too slow.");
    }

    [Fact]
    public void Should_Clear_Quickly()
    {
        var logView = new LogView();
        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        logView.Clear();
        stopwatch.Stop();

        logView.EntryCount.ShouldBe(0);

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        Assert.True(elapsedMs < 10, $"Clearing {LargeEntryCount} entries took {elapsedMs}ms, which is too slow.");
    }

    [Theory]
    [InlineData(SmallEntryCount)]
    [InlineData(MediumEntryCount)]
    [InlineData(LargeEntryCount)]
    public void Should_Maintain_Performance_Across_Different_Sizes(int entryCount)
    {
        var console = new TestConsole();
        var logView = new LogView
        {
            MaxVisibleRows = 50,
            Border = BoxBorder.None,
        };

        var entries = GenerateEntries(entryCount);

        var addStopwatch = System.Diagnostics.Stopwatch.StartNew();
        logView.AddEntries(entries);
        addStopwatch.Stop();

        logView.EntryCount.ShouldBe(entryCount);

        var renderStopwatch = System.Diagnostics.Stopwatch.StartNew();
        console.Write(logView);
        renderStopwatch.Stop();

        var addMs = addStopwatch.ElapsedMilliseconds;
        var renderMs = renderStopwatch.ElapsedMilliseconds;

        var maxAddMs = entryCount switch
        {
            SmallEntryCount => 10,
            MediumEntryCount => 100,
            LargeEntryCount => 1000,
            _ => int.MaxValue,
        };

        var maxRenderMs = entryCount switch
        {
            SmallEntryCount => 10,
            MediumEntryCount => 100,
            LargeEntryCount => 500,
            _ => int.MaxValue,
        };

        Assert.True(addMs < maxAddMs, $"Adding {entryCount} entries took {addMs}ms, expected < {maxAddMs}ms");
        Assert.True(renderMs < maxRenderMs, $"Rendering {entryCount} entries took {renderMs}ms, expected < {maxRenderMs}ms");
    }

    private static List<LogEntry> GenerateEntries(int count)
    {
        var entries = new List<LogEntry>(count);
        var random = new Random(42);
        var levels = new[] { LogLevel.Verbose, LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal };

        for (var i = 0; i < count; i++)
        {
            var level = levels[random.Next(levels.Length)];
            entries.Add(new LogEntry(level, $"This is log entry #{i} with some random content to make it longer and test wrapping behavior properly.", DateTime.Now.AddSeconds(-i))
            {
                Category = i % 2 == 0 ? "App" : "System",
            });
        }

        return entries;
    }

    private static List<LogEntry> GenerateEntriesWithKeywords(int count)
    {
        var entries = new List<LogEntry>(count);
        var keywords = new[] { "error", "warning", "info", "debug", "success", "failure", "test", "message" };
        var random = new Random(42);

        for (var i = 0; i < count; i++)
        {
            var keyword = keywords[random.Next(keywords.Length)];
            entries.Add(new LogEntry(LogLevel.Info, $"This is a {keyword} log entry #{i} that contains the keyword: {keyword}", DateTime.Now.AddSeconds(-i)));
        }

        return entries;
    }

    private static List<LogEntry> GenerateGroupedEntries(int count)
    {
        var entries = new List<LogEntry>(count);
        var groups = count / 10;

        for (var g = 0; g < groups; g++)
        {
            var level = (LogLevel)(g % 6);
            for (var i = 0; i < 10; i++)
            {
                entries.Add(new LogEntry(level, $"Group {g}, entry {i}: This is a {level} log message.", DateTime.Now.AddSeconds(-g * 10 - i)));
            }
        }

        return entries;
    }
}
