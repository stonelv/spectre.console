namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Widgets/JsonDiff")]
public sealed class JsonDiffTextTests
{
    [Fact]
    public Task Should_Render_Identical_Json_As_Unchanged()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\", \"age\": 30 }";
        var json2 = "{ \"name\": \"John\", \"age\": 30 }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Added_Property()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\" }";
        var json2 = "{ \"name\": \"John\", \"age\": 30 }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Deleted_Property()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\", \"age\": 30 }";
        var json2 = "{ \"name\": \"John\" }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Modified_Property()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\", \"age\": 30 }";
        var json2 = "{ \"name\": \"John\", \"age\": 31 }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Modified_String_Property()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\" }";
        var json2 = "{ \"name\": \"Jane\" }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Nested_Objects()
    {
        var console = new TestConsole().Size(new Size(80, 20));
        var json1 = @"{
            ""user"": {
                ""name"": ""John"",
                ""address"": {
                    ""city"": ""New York""
                }
            }
        }";
        var json2 = @"{
            ""user"": {
                ""name"": ""John"",
                ""address"": {
                    ""city"": ""Boston""
                }
            }
        }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Arrays()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"items\": [1, 2, 3] }";
        var json2 = "{ \"items\": [1, 4, 3] }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Ignore_Property_Order_When_Configured()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\", \"age\": 30 }";
        var json2 = "{ \"age\": 30, \"name\": \"John\" }";

        var diff = new JsonDiffText(json1, json2)
        {
            IgnorePropertyOrder = true
        };

        console.Write(diff);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Not_Ignore_Property_Order_By_Default()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\", \"age\": 30 }";
        var json2 = "{ \"age\": 30, \"name\": \"John\" }";

        var diff = new JsonDiffText(json1, json2);

        console.Write(diff);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Ignore_Case_When_Configured()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"Name\": \"John\" }";
        var json2 = "{ \"name\": \"John\" }";

        var diff = new JsonDiffText(json1, json2)
        {
            IgnoreCase = true,
            IgnorePropertyOrder = true
        };

        console.Write(diff);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Not_Ignore_Case_By_Default()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"Name\": \"John\" }";
        var json2 = "{ \"name\": \"John\" }";

        var diff = new JsonDiffText(json1, json2);

        console.Write(diff);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Boolean_Values()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"active\": true }";
        var json2 = "{ \"active\": false }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Null_Values()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"data\": null }";
        var json2 = "{ \"data\": \"value\" }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Render_Complex_Diff()
    {
        var console = new TestConsole().Size(new Size(80, 30));
        var json1 = @"{
            ""id"": 1,
            ""name"": ""John"",
            ""status"": ""active"",
            ""roles"": [""admin"", ""user""],
            ""profile"": {
                ""email"": ""john@example.com"",
                ""phone"": ""123-456""
            }
        }";
        var json2 = @"{
            ""id"": 1,
            ""name"": ""John Doe"",
            ""roles"": [""admin"", ""editor"", ""user""],
            ""profile"": {
                ""email"": ""john.doe@example.com"",
                ""address"": ""123 Main St""
            }
        }";

        console.Write(new JsonDiffText(json1, json2));

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public Task Should_Use_Chained_Extensions()
    {
        var console = new TestConsole().Size(new Size(80, 15));
        var json1 = "{ \"name\": \"John\" }";
        var json2 = "{ \"name\": \"Jane\" }";

        var diff = new JsonDiffText(json1, json2)
            .AddedColor(Color.Green)
            .DeletedColor(Color.Red)
            .ModifiedColor(Color.Yellow)
            .MemberColor(Color.Blue)
            .StringColor(Color.Cyan1);

        console.Write(diff);

        return Verifier.Verify(console.Output);
    }

    [Fact]
    public void JsonDiffer_Should_Detect_Unchanged()
    {
        var differ = new JsonDiffer();
        var left = "{ \"a\": 1 }";
        var right = "{ \"a\": 1 }";

        var result = differ.Diff(left, right);

        result.DiffType.ShouldBe(JsonDiffType.Unchanged);
    }

    [Fact]
    public void JsonDiffer_Should_Detect_Added()
    {
        var differ = new JsonDiffer();
        var left = "{ }";
        var right = "{ \"a\": 1 }";

        var result = differ.Diff(left, right);

        result.DiffType.ShouldBe(JsonDiffType.Modified);
        result.Children.ShouldContain(c => c.DiffType == JsonDiffType.Added);
    }

    [Fact]
    public void JsonDiffer_Should_Detect_Deleted()
    {
        var differ = new JsonDiffer();
        var left = "{ \"a\": 1 }";
        var right = "{ }";

        var result = differ.Diff(left, right);

        result.DiffType.ShouldBe(JsonDiffType.Modified);
        result.Children.ShouldContain(c => c.DiffType == JsonDiffType.Deleted);
    }

    [Fact]
    public void JsonDiffer_Should_Detect_Modified()
    {
        var differ = new JsonDiffer();
        var left = "{ \"a\": 1 }";
        var right = "{ \"a\": 2 }";

        var result = differ.Diff(left, right);

        result.DiffType.ShouldBe(JsonDiffType.Modified);
        result.Children.ShouldContain(c => c.DiffType == JsonDiffType.Modified);
    }

    [Fact]
    public void JsonDiffer_Should_Ignore_Property_Order()
    {
        var options = new JsonDiffOptions { IgnorePropertyOrder = true };
        var differ = new JsonDiffer(options);
        var left = "{ \"a\": 1, \"b\": 2 }";
        var right = "{ \"b\": 2, \"a\": 1 }";

        var result = differ.Diff(left, right);

        result.DiffType.ShouldBe(JsonDiffType.Unchanged);
    }

    [Fact]
    public void JsonDiffer_Should_Ignore_Case()
    {
        var options = new JsonDiffOptions { IgnoreCase = true, IgnorePropertyOrder = true };
        var differ = new JsonDiffer(options);
        var left = "{ \"Name\": \"John\" }";
        var right = "{ \"name\": \"John\" }";

        var result = differ.Diff(left, right);

        result.DiffType.ShouldBe(JsonDiffType.Unchanged);
    }
}
