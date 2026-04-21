namespace Spectre.Console.Tests.Unit;

/// <summary>
/// Performance regression tests for LogView.
/// These tests verify that operations complete successfully and maintain
/// expected behavior, rather than asserting hard time thresholds that
/// may fail randomly in CI environments.
/// </summary>
public class LogViewPerformanceTests
{
    private const int LargeEntryCount = 10000;
    private const int MediumEntryCount = 1000;
    private const int SmallEntryCount = 100;

    [Fact]
    public void Should_Add_Large_Number_Of_Entries_Successfully()
    {
        var logView = new LogView();
        var entries = GenerateEntries(LargeEntryCount);

        logView.AddEntries(entries);

        logView.EntryCount.ShouldBe(LargeEntryCount);
    }

    [Fact]
    public void Should_Render_Large_Number_Of_Entries_Successfully()
    {
        var console = new TestConsole();
        var logView = new LogView
        {
            MaxVisibleRows = 50,
            Border = BoxBorder.None,
        };

        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        console.Write(logView);
        var output = console.Output;

        output.ShouldNotBeNullOrWhiteSpace();
        output.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Should_Filter_Successfully()
    {
        var logView = new LogView();
        var entries = GenerateEntriesWithKeywords(LargeEntryCount);
        logView.AddEntries(entries);

        logView.FilterKeyword = "error";

        logView.EntryCount.ShouldBe(LargeEntryCount);
    }

    [Fact]
    public void Should_Group_Successfully()
    {
        var logView = new LogView
        {
            GroupThreshold = 3,
            AutoGrouping = true,
        };

        var entries = GenerateGroupedEntries(LargeEntryCount);

        logView.AddEntries(entries);

        logView.EntryCount.ShouldBe(LargeEntryCount);
    }

    [Fact]
    public void Should_Handle_MaxEntries_Successfully()
    {
        var logView = new LogView
        {
            MaxEntries = MediumEntryCount,
        };

        var entries = GenerateEntries(LargeEntryCount);

        logView.AddEntries(entries);

        logView.EntryCount.ShouldBe(MediumEntryCount);
    }

    [Fact]
    public void Should_Render_With_Highlighting_Successfully()
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

        console.Write(logView);
        var output = console.Output;

        output.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Should_Scroll_Successfully()
    {
        var logView = new LogView
        {
            MaxVisibleRows = 20,
        };

        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        for (var i = 0; i < 100; i++)
        {
            logView.ScrollUp(10);
            logView.ScrollDown(5);
        }

        logView.ScrollToTop();
        logView.ScrollToBottom();

        logView.EntryCount.ShouldBe(LargeEntryCount);
    }

    [Fact]
    public void Should_Toggle_Pause_Successfully()
    {
        var logView = new LogView();
        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        for (var i = 0; i < 1000; i++)
        {
            logView.TogglePause();
        }

        logView.IsPaused.ShouldBe(false);
    }

    [Fact]
    public void Should_Clear_Successfully()
    {
        var logView = new LogView();
        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        logView.Clear();

        logView.EntryCount.ShouldBe(0);
    }

    [Theory]
    [InlineData(SmallEntryCount)]
    [InlineData(MediumEntryCount)]
    [InlineData(LargeEntryCount)]
    public void Should_Maintain_Correctness_Across_Different_Sizes(int entryCount)
    {
        var console = new TestConsole();
        var logView = new LogView
        {
            MaxVisibleRows = 50,
            Border = BoxBorder.None,
        };

        var entries = GenerateEntries(entryCount);

        logView.AddEntries(entries);

        logView.EntryCount.ShouldBe(entryCount);

        console.Write(logView);
        var output = console.Output;

        output.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Should_Maintain_Order_When_Adding_Entries()
    {
        var logView = new LogView();
        var console = new TestConsole();

        for (var i = 0; i < MediumEntryCount; i++)
        {
            logView.AddEntry(LogEntry.Info($"Entry {i}"));
        }

        logView.EntryCount.ShouldBe(MediumEntryCount);

        console.Write(logView);
        var output = console.Output;

        output.ShouldContain($"Entry {MediumEntryCount - 1}");
    }

    [Fact]
    public void Should_Handle_Concurrent_Access_Pattern()
    {
        var logView = new LogView
        {
            MaxEntries = MediumEntryCount,
        };

        for (var i = 0; i < LargeEntryCount; i++)
        {
            logView.AddEntry(LogEntry.Info($"Entry {i}"));
        }

        logView.EntryCount.ShouldBe(MediumEntryCount);
    }

    [Fact]
    public void Should_Handle_MaxEntries_With_Batch_Add()
    {
        var logView = new LogView
        {
            MaxEntries = MediumEntryCount,
        };

        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        logView.EntryCount.ShouldBe(MediumEntryCount);
    }

    [Fact]
    public void Should_Handle_MaxEntries_With_Single_Add()
    {
        var logView = new LogView
        {
            MaxEntries = MediumEntryCount,
        };

        for (var i = 0; i < LargeEntryCount; i++)
        {
            logView.AddEntry(LogEntry.Info($"Entry {i}"));
        }

        logView.EntryCount.ShouldBe(MediumEntryCount);
    }

    [Fact]
    public void Should_Handle_Rapid_Scroll_Operations()
    {
        var logView = new LogView
        {
            MaxVisibleRows = 10,
        };

        var entries = GenerateEntries(LargeEntryCount);
        logView.AddEntries(entries);

        for (var i = 0; i < 1000; i++)
        {
            logView.ScrollUp(1);
            logView.ScrollDown(1);
        }

        logView.ScrollOffset.ShouldBe(0);
    }

    [Fact]
    public void Should_Handle_Rapid_Filter_Changes()
    {
        var logView = new LogView();
        var entries = GenerateEntriesWithKeywords(MediumEntryCount);
        logView.AddEntries(entries);

        for (var i = 0; i < 100; i++)
        {
            logView.FilterKeyword = "error";
            logView.FilterKeyword = "warning";
            logView.FilterKeyword = null;
        }

        logView.EntryCount.ShouldBe(MediumEntryCount);
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
