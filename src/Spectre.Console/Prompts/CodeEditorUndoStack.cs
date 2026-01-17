namespace Spectre.Console;

internal sealed class CodeEditorUndoStack
{
    private const int MaxUndoSteps = 10;
    private readonly List<string> _stack;

    public int Count => _stack.Count;

    public CodeEditorUndoStack()
    {
        _stack = new List<string>(MaxUndoSteps);
    }

    public void Push(string state)
    {
        if (_stack.Count >= MaxUndoSteps)
        {
            _stack.RemoveAt(0);
        }
        _stack.Add(state);
    }

    public string? Pop()
    {
        if (_stack.Count == 0)
        {
            return null;
        }
        var index = _stack.Count - 1;
        var state = _stack[index];
        _stack.RemoveAt(index);
        return state;
    }

    public void Clear()
    {
        _stack.Clear();
    }
}
