namespace Spectre.Console;

public sealed class TextFormField : FormField<string>
{
    public override FormFieldType FieldType => FormFieldType.Text;

    public string? Placeholder { get; set; }
    public int? MaxLength { get; set; }
    public bool AllowEmpty { get; set; }
    public string? DefaultValue { get; set; }

    public TextFormField(string id, string label)
        : base(id, label)
    {
    }

    public override ValidationResult Validate()
    {
        if (IsRequired)
        {
            if (Value == null)
            {
                return ValidationResult.Error(ValidationErrorMessage);
            }

            if (!AllowEmpty && string.IsNullOrWhiteSpace(Value))
            {
                return ValidationResult.Error(ValidationErrorMessage);
            }
        }

        if (MaxLength.HasValue && Value != null && Value.Length > MaxLength)
        {
            return ValidationResult.Error($"Input must not exceed {MaxLength} characters");
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
}
