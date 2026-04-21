namespace Spectre.Console;

public abstract class FormField<T> : IFormField
{
    private T? _value;
    private bool _hasValue;

    public string Id { get; }
    public string Label { get; }
    public abstract FormFieldType FieldType { get; }
    public bool IsRequired { get; set; }
    public string? ValidationErrorMessage { get; set; } = "[red]Invalid input[/]";
    public Func<T?, ValidationResult>? Validator { get; set; }
    public Style? LabelStyle { get; set; }

    public T? Value
    {
        get => _value;
        set
        {
            _value = value;
            _hasValue = true;
        }
    }

    object? IFormField.Value => Value;

    Func<object?, ValidationResult>? IFormField.Validator
    {
        get => Validator != null ? (obj => Validator((T?)obj)) : null;
        set => throw new NotSupportedException("Use the generic Validator property instead.");
    }

    protected FormField(string id, string label)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    public virtual ValidationResult Validate()
    {
        if (IsRequired && !_hasValue)
        {
            return ValidationResult.Error(ValidationErrorMessage);
        }

        if (IsRequired && _hasValue)
        {
            if (Value is string strValue && string.IsNullOrWhiteSpace(strValue))
            {
                return ValidationResult.Error(ValidationErrorMessage);
            }
            if (Value is ICollection<string> collection && collection.Count == 0)
            {
                return ValidationResult.Error(ValidationErrorMessage);
            }
        }

        if (Validator != null)
        {
            var result = Validator(Value);
            if (!result.Successful)
            {
                return ValidationResult.Error(result.Message ?? ValidationErrorMessage);
            }
        }

        return ValidationResult.Success();
    }

    public void SetValue(object? value)
    {
        if (value is T typedValue)
        {
            Value = typedValue;
        }
        else if (value == null)
        {
            _value = default;
            _hasValue = false;
        }
        else
        {
            throw new InvalidCastException($"Cannot convert value of type {value.GetType().Name} to {typeof(T).Name}");
        }
    }

    public virtual object? GetTypedValue() => Value;
}
