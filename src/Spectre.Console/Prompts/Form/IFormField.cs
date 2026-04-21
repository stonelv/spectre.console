namespace Spectre.Console;

public interface IFormField
{
    string Id { get; }
    string Label { get; }
    FormFieldType FieldType { get; }
    object? Value { get; }
    bool IsRequired { get; set; }
    string? ValidationErrorMessage { get; set; }
    Func<object?, ValidationResult>? Validator { get; set; }
    Style? LabelStyle { get; set; }

    ValidationResult Validate();
    void SetValue(object? value);
    object? GetTypedValue();
}
