namespace Spectre.Console;

public enum DiffLineType
{
    Unchanged,
    Added,
    Deleted
}

public sealed class DiffLine
{
    public DiffLineType Type { get; }
    public string Content { get; }

    public DiffLine(DiffLineType type, string content)
    {
        Type = type;
        Content = content ?? string.Empty;
    }
}
