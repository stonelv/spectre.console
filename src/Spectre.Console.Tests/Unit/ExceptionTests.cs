namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Exception")]
public sealed class ExceptionTests
{
    [Fact]
    [Expectation("Default")]
    public Task Should_Write_Exception()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.MethodThatThrows(null));

        // When
        var result = console.WriteNormalizedException(dex);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("ShortenedTypes")]
    public Task Should_Write_Exception_With_Shortened_Types()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.MethodThatThrows(null));

        // When
        var result = console.WriteNormalizedException(dex, ExceptionFormats.ShortenTypes);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("ShortenedMethods")]
    public Task Should_Write_Exception_With_Shortened_Methods()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.MethodThatThrows(null));

        // When
        var result = console.WriteNormalizedException(dex, ExceptionFormats.ShortenMethods);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("InnerException")]
    public Task Should_Write_Exception_With_Inner_Exception()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.ThrowWithInnerException());

        // When
        var result = console.WriteNormalizedException(dex);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("CallSite")]
    public Task Should_Write_Exceptions_With_Generic_Type_Parameters_In_Callsite_As_Expected()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.ThrowWithGenericInnerException());

        // When
        var result = console.WriteNormalizedException(dex);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("OutParam")]
    public Task Should_Write_Exception_With_Output_Param()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.GenericMethodWithOutThatThrows<int>(out _));

        // When
        var result = console.WriteNormalizedException(dex, ExceptionFormats.ShortenTypes);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("Tuple")]
    public Task Should_Write_Exception_With_Tuple_Return()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.GetTuplesWithInnerException<int>((0, "value")));

        // When
        var result = console.WriteNormalizedException(dex, ExceptionFormats.ShortenTypes);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("NoStackTrace")]
    public Task Should_Write_Exception_With_No_StackTrace()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(TestExceptions.ThrowWithInnerException);

        // When
        var result = console.WriteNormalizedException(dex, ExceptionFormats.NoStackTrace);

        // Then
        return Verifier.Verify(result);
    }

    [Theory]
    [InlineData(ExceptionFormats.Default)]
    [InlineData(ExceptionFormats.ShortenTypes)]
    [InlineData(ExceptionFormats.ShortenMethods)]
    [InlineData(ExceptionFormats.ShortenEverything)]
    [Expectation("GenericException")]
    public Task Should_Write_GenericException(ExceptionFormats exceptionFormats)
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.MethodThatThrowsGenericException<IAnsiConsole>());

        // When
        var result = console.WriteNormalizedException(dex, exceptionFormats);

        // Then
        return Verifier.Verify(result).UseParameters(exceptionFormats);
    }

    [Fact]
    [Expectation("NoInnerExceptionsViaFormat")]
    public Task Should_Write_Exception_Without_Inner_Exceptions_When_Using_NoInnerExceptions_Format()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.ThrowWithInnerException());

        // When
        var result = console.WriteNormalizedException(dex, ExceptionFormats.NoInnerExceptions);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    [Expectation("NoInnerExceptionsViaSettings")]
    public Task Should_Write_Exception_Without_Inner_Exceptions_When_ShowInnerExceptions_Is_False()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.ThrowWithInnerException());
        var settings = new ExceptionSettings
        {
            ShowInnerExceptions = false
        };

        // When
        var result = console.WriteNormalizedException(dex, settings);

        // Then
        return Verifier.Verify(result);
    }

    [Fact]
    public void Should_Write_Exception_With_Limited_Inner_Exception_Depth()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = TestExceptions.CreateDeeplyNestedException(5);

        // When - unlimited depth
        var resultUnlimited = console.WriteNormalizedException(dex, new ExceptionSettings());
        var level5CountUnlimited = CountOccurrences(resultUnlimited, "Level 5");
        var level1CountUnlimited = CountOccurrences(resultUnlimited, "Level 1");

        // Reset console
        console = new TestConsole().Width(1024);

        // When - limited to depth 2
        var resultLimited = console.WriteNormalizedException(dex, new ExceptionSettings
        {
            MaxInnerExceptionDepth = 2
        });
        var level5CountLimited = CountOccurrences(resultLimited, "Level 5");
        var level1CountLimited = CountOccurrences(resultLimited, "Level 1");

        // Then
        level5CountUnlimited.ShouldBe(1);
        level1CountUnlimited.ShouldBe(1);
        level5CountLimited.ShouldBe(1);
        level1CountLimited.ShouldBe(0);
    }

    [Fact]
    public void Should_Not_Cause_StackOverflow_With_Cyclic_Reference()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = TestExceptions.CreateCyclicReferenceException();

        // When
        var exception = Record.Exception(() => console.WriteNormalizedException(dex));

        // Then - should not throw StackOverflowException
        exception.ShouldBeNull();
    }

    [Fact]
    public void Default_Behavior_Should_Show_Inner_Exceptions()
    {
        // Given
        var console = new TestConsole().Width(1024);
        var dex = GetException(() => TestExceptions.ThrowWithInnerException());

        // When
        var result = console.WriteNormalizedException(dex);

        // Then - should contain both outer and inner exception messages
        result.ShouldContain("Something threw!");
        result.ShouldContain("Throwing!");
    }

    private static int CountOccurrences(string text, string pattern)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(pattern, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += pattern.Length;
        }
        return count;
    }

    public static Exception GetException(Action action)
    {
        try
        {
            action?.Invoke();
        }
        catch (Exception e)
        {
            return e;
        }

        throw new InvalidOperationException("Exception harness failed");
    }
}