namespace Spectre.Console;

public sealed class PasswordFormField : FormField<string>
{
    public override FormFieldType FieldType => FormFieldType.Password;

    public char? Mask { get; set; } = '*';
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }

    public PasswordFormField(string id, string label)
        : base(id, label)
    {
        IsRequired = true;
    }

    public override ValidationResult Validate()
    {
        if (IsRequired)
        {
            if (Value == null)
            {
                return ValidationResult.Error(ValidationErrorMessage);
            }

            if (string.IsNullOrEmpty(Value))
            {
                return ValidationResult.Error(ValidationErrorMessage);
            }
        }

        if (MinLength.HasValue && Value != null && Value.Length < MinLength)
        {
            return ValidationResult.Error($"Password must be at least {MinLength} characters");
        }

        if (MaxLength.HasValue && Value != null && Value.Length > MaxLength)
        {
            return ValidationResult.Error($"Password must not exceed {MaxLength} characters");
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
