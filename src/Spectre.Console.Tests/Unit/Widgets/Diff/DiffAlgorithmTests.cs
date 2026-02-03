namespace Spectre.Console.Tests.Unit;

public sealed class DiffAlgorithmTests
{
    [Fact]
    public void Compute_EmptyTexts_Should_Return_Empty_Result()
    {
        // Given
        var oldText = "";
        var newText = "";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.ShouldBeEmpty();
        result.InsertedCount.ShouldBe(0);
        result.DeletedCount.ShouldBe(0);
        result.UnchangedCount.ShouldBe(0);
    }

    [Fact]
    public void Compute_EmptyOldText_Should_Return_All_Inserted()
    {
        // Given
        var oldText = "";
        var newText = "Line1\nLine2\nLine3";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.Count.ShouldBe(3);
        result.InsertedCount.ShouldBe(3);
        result.DeletedCount.ShouldBe(0);
        result.UnchangedCount.ShouldBe(0);

        result.Lines[0].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[0].Text.ShouldBe("Line1");
        result.Lines[0].OldLineNumber.ShouldBeNull();
        result.Lines[0].NewLineNumber.ShouldBe(1);

        result.Lines[1].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[1].Text.ShouldBe("Line2");
        result.Lines[1].NewLineNumber.ShouldBe(2);

        result.Lines[2].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[2].Text.ShouldBe("Line3");
        result.Lines[2].NewLineNumber.ShouldBe(3);
    }

    [Fact]
    public void Compute_EmptyNewText_Should_Return_All_Deleted()
    {
        // Given
        var oldText = "Line1\nLine2\nLine3";
        var newText = "";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.Count.ShouldBe(3);
        result.InsertedCount.ShouldBe(0);
        result.DeletedCount.ShouldBe(3);
        result.UnchangedCount.ShouldBe(0);

        result.Lines[0].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[0].Text.ShouldBe("Line1");
        result.Lines[0].OldLineNumber.ShouldBe(1);
        result.Lines[0].NewLineNumber.ShouldBeNull();

        result.Lines[1].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[1].OldLineNumber.ShouldBe(2);

        result.Lines[2].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[2].OldLineNumber.ShouldBe(3);
    }

    [Fact]
    public void Compute_IdenticalTexts_Should_Return_All_Unchanged()
    {
        // Given
        var oldText = "Line1\nLine2\nLine3";
        var newText = "Line1\nLine2\nLine3";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.Count.ShouldBe(3);
        result.InsertedCount.ShouldBe(0);
        result.DeletedCount.ShouldBe(0);
        result.UnchangedCount.ShouldBe(3);

        for (int i = 0; i < 3; i++)
        {
            result.Lines[i].Type.ShouldBe(DiffLineType.Unchanged);
            result.Lines[i].OldLineNumber.ShouldBe(i + 1);
            result.Lines[i].NewLineNumber.ShouldBe(i + 1);
        }
    }

    [Fact]
    public void Compute_PureInsertion_Should_Detect_Inserted_Lines()
    {
        // Given
        var oldText = "Line1\nLine2";
        var newText = "Line1\nLine2\nLine3\nLine4";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.Count.ShouldBe(4);
        result.InsertedCount.ShouldBe(2);
        result.DeletedCount.ShouldBe(0);
        result.UnchangedCount.ShouldBe(2);

        result.Lines[0].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[0].Text.ShouldBe("Line1");

        result.Lines[1].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[1].Text.ShouldBe("Line2");

        result.Lines[2].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[2].Text.ShouldBe("Line3");

        result.Lines[3].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[3].Text.ShouldBe("Line4");
    }

    [Fact]
    public void Compute_PureDeletion_Should_Detect_Deleted_Lines()
    {
        // Given
        var oldText = "Line1\nLine2\nLine3\nLine4";
        var newText = "Line1\nLine2";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.Count.ShouldBe(4);
        result.InsertedCount.ShouldBe(0);
        result.DeletedCount.ShouldBe(2);
        result.UnchangedCount.ShouldBe(2);

        result.Lines[0].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[0].Text.ShouldBe("Line1");

        result.Lines[1].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[1].Text.ShouldBe("Line2");

        result.Lines[2].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[2].Text.ShouldBe("Line3");

        result.Lines[3].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[3].Text.ShouldBe("Line4");
    }

    [Fact]
    public void Compute_MixedChanges_Should_Detect_All_Types()
    {
        // Given
        var oldText = "Line1\nOldLine2\nLine3\nOldLine4";
        var newText = "Line1\nNewLine2\nLine3\nNewLine4";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then
        result.Lines.Count.ShouldBe(6);
        result.InsertedCount.ShouldBe(2);
        result.DeletedCount.ShouldBe(2);
        result.UnchangedCount.ShouldBe(2);

        result.Lines[0].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[0].Text.ShouldBe("Line1");

        result.Lines[1].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[1].Text.ShouldBe("OldLine2");

        result.Lines[2].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[2].Text.ShouldBe("NewLine2");

        result.Lines[3].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[3].Text.ShouldBe("Line3");

        result.Lines[4].Type.ShouldBe(DiffLineType.Deleted);
        result.Lines[4].Text.ShouldBe("OldLine4");

        result.Lines[5].Type.ShouldBe(DiffLineType.Inserted);
        result.Lines[5].Text.ShouldBe("NewLine4");
    }

    [Fact]
    public void Compute_WithEmptyLines_Should_Handle_Correctly()
    {
        // Given
        var oldText = "Line1\n\nLine3";
        var newText = "Line1\nLine2\n\nLine4";

        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then - verify the diff contains expected changes
        // Note: LCS algorithm may produce different but valid diff sequences
        result.Lines.Count.ShouldBe(5);

        // Line1 should be unchanged
        result.Lines[0].Type.ShouldBe(DiffLineType.Unchanged);
        result.Lines[0].Text.ShouldBe("Line1");

        // Verify overall counts
        result.UnchangedCount.ShouldBe(2); // Line1 and one empty line
        result.InsertedCount.ShouldBe(2);  // Line2 and Line4
        result.DeletedCount.ShouldBe(1);   // Line3

        // Verify all expected content is present
        var texts = result.Lines.Select(l => l.Text).ToList();
        texts.ShouldContain("Line1");
        texts.ShouldContain("Line2");
        texts.ShouldContain("Line3");
        texts.ShouldContain("Line4");
        texts.ShouldContain(""); // empty line
    }

    [Theory]
    [InlineData("Line1\r\nLine2", "Line1\nLine2")]
    [InlineData("Line1\rLine2", "Line1\nLine2")]
    [InlineData("Line1\nLine2", "Line1\nLine2")]
    public void Compute_DifferentLineEndings_Should_Handle_Correctly(string oldText, string newText)
    {
        // When
        var result = DiffAlgorithm.Compute(oldText, newText);

        // Then - should treat same content as unchanged regardless of line ending
        if (oldText.Replace("\r\n", "\n").Replace("\r", "\n") ==
            newText.Replace("\r\n", "\n").Replace("\r", "\n"))
        {
            result.UnchangedCount.ShouldBe(2);
        }
    }

    [Fact]
    public void Compute_NullOldText_Should_Throw_ArgumentNullException()
    {
        // Given, When, Then
        Should.Throw<ArgumentNullException>(() => DiffAlgorithm.Compute(null!, "text"));
    }

    [Fact]
    public void Compute_NullNewText_Should_Throw_ArgumentNullException()
    {
        // Given, When, Then
        Should.Throw<ArgumentNullException>(() => DiffAlgorithm.Compute("text", null!));
    }

    [Fact]
    public void Compute_WithNullLines_Should_Throw_ArgumentNullException()
    {
        // Given, When, Then
        Should.Throw<ArgumentNullException>(() => DiffAlgorithm.Compute(null!, new[] { "line" }));
        Should.Throw<ArgumentNullException>(() => DiffAlgorithm.Compute(new[] { "line" }, null!));
    }
}
