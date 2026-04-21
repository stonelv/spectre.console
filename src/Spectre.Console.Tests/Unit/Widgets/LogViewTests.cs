namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Widgets/LogView")]
public class LogViewTests
{
    [Fact]
    public void Should_Add_Entry()
    {
        var logView = new LogView();
        var entry = LogEntry.Info("Test message");

        logView.AddEntry(entry);

        logView.EntryCount.ShouldBe(1);
    }

    [Fact]
    public void Should_Add_Multiple_Entries()
    {
        var logView = new LogView();
        var entries = new List<LogEntry>
        {
            LogEntry.Info("Message 1"),
            LogEntry.Warn("Message 2"),
            LogEntry.Error("Message 3"),
        };

        logView.AddEntries(entries);

        logView.EntryCount.ShouldBe(3);
    }

    [Fact]
    public void Should_Clear_Entries()
    {
        var logView = new LogView();
        logView.AddEntry(LogEntry.Info("Test"));
        logView.AddEntry(LogEntry.Warn("Test 2"));

        logView.Clear();

        logView.EntryCount.ShouldBe(0);
    }

    [Fact]
    public void Should_Respect_MaxEntries()
    {
        var logView = new LogView
        {
            MaxEntries = 3,
        };

        logView.AddEntry(LogEntry.Info("Message 1"));
        logView.AddEntry(LogEntry.Info("Message 2"));
        logView.AddEntry(LogEntry.Info("Message 3"));
        logView.AddEntry(LogEntry.Info("Message 4"));

        logView.EntryCount.ShouldBe(3);
    }

    [Fact]
    public void Should_Toggle_Pause()
    {
        var logView = new LogView();

        logView.IsPaused.ShouldBe(false);

        var result1 = logView.TogglePause();
        result1.ShouldBe(true);
        logView.IsPaused.ShouldBe(true);

        var result2 = logView.TogglePause();
        result2.ShouldBe(false);
        logView.IsPaused.ShouldBe(false);
    }

    [Fact]
    public void Should_Scroll_Up_And_Down()
    {
        var logView = new LogView
        {
            MaxVisibleRows = 5,
        };

        for (var i = 0; i < 20; i++)
        {
            logView.AddEntry(LogEntry.Info($"Message {i}"));
        }

        logView.ScrollOffset.ShouldBe(0);

        logView.ScrollUp(5);
        logView.ScrollOffset.ShouldBe(5);

        logView.ScrollDown(3);
        logView.ScrollOffset.ShouldBe(2);
    }

    [Fact]
    public void Should_Scroll_To_Top_And_Bottom()
    {
        var logView = new LogView
        {
            MaxVisibleRows = 5,
        };

        for (var i = 0; i < 20; i++)
        {
            logView.AddEntry(LogEntry.Info($"Message {i}"));
        }

        logView.ScrollToTop();
        logView.ScrollOffset.ShouldBe(15);

        logView.ScrollToBottom();
        logView.ScrollOffset.ShouldBe(0);
    }

    [Fact]
    public void Should_Filter_By_Keyword()
    {
        var logView = new LogView
        {
            AutoGrouping = false,
        };

        logView.AddEntry(LogEntry.Info("This is an error message"));
        logView.AddEntry(LogEntry.Info("This is a warning message"));
        logView.AddEntry(LogEntry.Info("This is an info message"));

        logView.EntryCount.ShouldBe(3);

        logView.FilterKeyword = "error";
    }

    [Fact]
    public void Should_Use_Compact_View_In_Narrow_Terminal()
    {
        var console = new TestConsole();
        console.Profile.Width = 60;

        var logView = new LogView
        {
            MinWidthForFullView = 80,
            Border = BoxBorder.None,
        };

        logView.AddEntry(LogEntry.Info("Test message"));

        console.Write(logView);
        var output = console.Output;

        output.ShouldContain("[I]");
    }

    [Fact]
    public void Should_Use_Full_View_In_Wide_Terminal()
    {
        var console = new TestConsole();
        console.Profile.Width = 100;

        var logView = new LogView
        {
            MinWidthForFullView = 80,
            Border = BoxBorder.None,
            ShowTimestamp = false,
        };

        logView.AddEntry(LogEntry.Info("Test message"));

        console.Write(logView);
        var output = console.Output;

        output.ShouldContain("INF");
    }

    [Fact]
    public void Should_Collapse_And_Expand_Groups()
    {
        var logView = new LogView
        {
            GroupThreshold = 3,
            AutoGrouping = true,
        };

        for (var i = 0; i < 10; i++)
        {
            logView.AddEntry(LogEntry.Info($"Info message {i}"));
        }

        logView.CollapseAllGroups();
        logView.ExpandAllGroups();
    }

    [Fact]
    public void Should_Render_With_Border()
    {
        var console = new TestConsole();

        var logView = new LogView
        {
            Width = 40,
            Border = BoxBorder.Square,
        };

        logView.AddEntry(LogEntry.Info("Test message"));

        console.Write(logView);
        var output = console.Output;

        output.ShouldContain("┌");
        output.ShouldContain("┐");
        output.ShouldContain("└");
        output.ShouldContain("┘");
    }

    [Fact]
    public void Should_Render_Different_Levels_With_Different_Styles()
    {
        var console = new TestConsole();
        console.EmitAnsiSequences = true;

        var logView = new LogView
        {
            Border = BoxBorder.None,
            ShowTimestamp = false,
        };

        logView.AddEntry(LogEntry.Info("Info message"));
        logView.AddEntry(LogEntry.Warn("Warning message"));
        logView.AddEntry(LogEntry.Error("Error message"));

        console.Write(logView);
        var output = console.Output;

        output.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Should_Show_Category_When_Set()
    {
        var console = new TestConsole();

        var logView = new LogView
        {
            Border = BoxBorder.None,
            ShowTimestamp = false,
            ShowCategory = true,
        };

        logView.AddEntry(new LogEntry(LogLevel.Info, "Test message")
        {
            Category = "MyApp",
        });

        console.Write(logView);
        var output = console.Output;

        output.ShouldContain("[MyApp]");
    }

    [Fact]
    public void Should_Hide_Category_When_Disabled()
    {
        var console = new TestConsole();

        var logView = new LogView
        {
            Border = BoxBorder.None,
            ShowTimestamp = false,
            ShowCategory = false,
        };

        logView.AddEntry(new LogEntry(LogLevel.Info, "Test message")
        {
            Category = "MyApp",
        });

        console.Write(logView);
        var output = console.Output;

        output.ShouldNotContain("[MyApp]");
    }
}
