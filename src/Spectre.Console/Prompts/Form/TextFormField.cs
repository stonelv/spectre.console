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
}
