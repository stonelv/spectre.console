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
}
