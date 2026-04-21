namespace Spectre.Console;

public sealed class FormResult
{
    private readonly Dictionary<string, object?> _values = new Dictionary<string, object?>();

    public int FieldCount => _values.Count;

    public IReadOnlyCollection<string> FieldIds => _values.Keys.ToList().AsReadOnly();

    internal FormResult()
    {
    }

    internal void AddValue(string id, object? value)
    {
        _values[id] = value;
    }

    public T? GetValue<T>(string id)
    {
        if (_values.TryGetValue(id, out var value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }

            if (value == null)
            {
                return default;
            }

            throw new InvalidCastException($"Field '{id}' is of type {value.GetType().Name}, not {typeof(T).Name}");
        }

        throw new KeyNotFoundException($"Field '{id}' not found in form result");
    }

    public bool TryGetValue<T>(string id, out T? value)
    {
        if (_values.TryGetValue(id, out var objValue))
        {
            if (objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            if (objValue == null)
            {
                value = default;
                return true;
            }
        }

        value = default;
        return false;
    }

    public object? this[string id]
    {
        get
        {
            if (_values.TryGetValue(id, out var value))
            {
                return value;
            }
            throw new KeyNotFoundException($"Field '{id}' not found in form result");
        }
    }

    public bool ContainsField(string id) => _values.ContainsKey(id);
}
