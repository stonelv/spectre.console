namespace Spectre.Console;

public sealed class MultiSelectionFormField : FormField<List<string>>
{
    private readonly List<SelectionChoice> _choices = new List<SelectionChoice>();

    public override FormFieldType FieldType => FormFieldType.MultiSelection;

    public IReadOnlyList<SelectionChoice> Choices => _choices;

    public int PageSize { get; set; } = 10;
    public bool WrapAround { get; set; }
    public bool Required { get; set; } = true;
    public int? MinSelections { get; set; }
    public int? MaxSelections { get; set; }
    public List<int>? DefaultIndexes { get; set; }
    public string? MoreChoicesText { get; set; }
    public string? InstructionsText { get; set; }

    public MultiSelectionFormField(string id, string label)
        : base(id, label)
    {
        Value = new List<string>();
    }

    public MultiSelectionFormField AddChoice(string value, string? displayText = null)
    {
        _choices.Add(new SelectionChoice(value, displayText ?? value));
        return this;
    }

    public MultiSelectionFormField AddChoices(IEnumerable<string> choices)
    {
        foreach (var choice in choices)
        {
            AddChoice(choice);
        }
        return this;
    }

    public override ValidationResult Validate()
    {
        var baseResult = base.Validate();
        if (!baseResult.Successful)
        {
            return baseResult;
        }

        if (Required && (Value == null || Value.Count == 0))
        {
            return ValidationResult.Error(ValidationErrorMessage);
        }

        if (Value != null)
        {
            if (MinSelections.HasValue && Value.Count < MinSelections)
            {
                return ValidationResult.Error($"Please select at least {MinSelections} options");
            }

            if (MaxSelections.HasValue && Value.Count > MaxSelections)
            {
                return ValidationResult.Error($"Please select at most {MaxSelections} options");
            }
        }

        return ValidationResult.Success();
    }

    public override object? GetTypedValue()
    {
        return Value?.AsReadOnly();
    }
}
