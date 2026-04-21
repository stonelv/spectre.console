namespace Spectre.Console;

public sealed class SelectionFormField : FormField<string>
{
    private readonly List<SelectionChoice> _choices = new List<SelectionChoice>();

    public override FormFieldType FieldType => FormFieldType.Selection;

    public IReadOnlyList<SelectionChoice> Choices => _choices;

    public int PageSize { get; set; } = 10;
    public bool WrapAround { get; set; }
    public int? DefaultIndex { get; set; }
    public string? MoreChoicesText { get; set; }

    public SelectionFormField(string id, string label)
        : base(id, label)
    {
    }

    public SelectionFormField AddChoice(string value, string? displayText = null)
    {
        _choices.Add(new SelectionChoice(value, displayText ?? value));
        return this;
    }

    public SelectionFormField AddChoices(IEnumerable<string> choices)
    {
        foreach (var choice in choices)
        {
            AddChoice(choice);
        }
        return this;
    }
}

public sealed class SelectionChoice
{
    public string Value { get; }
    public string DisplayText { get; }

    public SelectionChoice(string value, string displayText)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        DisplayText = displayText ?? throw new ArgumentNullException(nameof(displayText));
    }
}
