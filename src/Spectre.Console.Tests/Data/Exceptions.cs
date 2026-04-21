namespace Spectre.Console.Tests.Data;

public static class TestExceptions
{
    public static bool MethodThatThrows(int? number) => throw new InvalidOperationException("Throwing!");

    public static bool GenericMethodThatThrows<T0, T1, TRet>(int? number) => throw new InvalidOperationException("Throwing!");

    public static bool MethodThatThrowsGenericException<T>() => throw new GenericException<T>("Throwing!", default!);

    public static void ThrowWithInnerException()
    {
        try
        {
            MethodThatThrows(null);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Something threw!", ex);
        }
    }

    public static void ThrowWithGenericInnerException()
    {
        try
        {
            GenericMethodThatThrows<int, float, double>(null);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Something threw!", ex);
        }
    }

    public static List<T> GenericMethodWithOutThatThrows<T>(out List<T> firstFewItems)
    {
        firstFewItems = new List<T>();
        throw new InvalidOperationException("Throwing!");
    }

    public static (string Key, List<T> Values) GetTuplesWithInnerException<T>((int First, string Second) myValue)
    {
        MethodThatThrows(0);
        return ("key", new List<T>());
    }

    public static Exception CreateDeeplyNestedException(int depth)
    {
        if (depth <= 0)
        {
            return new InvalidOperationException("Innermost exception");
        }

        return new InvalidOperationException($"Level {depth}", CreateDeeplyNestedException(depth - 1));
    }

    public static Exception CreateCyclicReferenceException()
    {
        var inner = new InvalidOperationException("Inner exception");
        var outer = new InvalidOperationException("Outer exception", inner);
        
        var field = typeof(Exception).GetField("_innerException", BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(inner, outer);

        return outer;
    }
}

#pragma warning disable CS9113 // Parameter is unread.
public class GenericException<T>(string message, T value) : Exception(message);