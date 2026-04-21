namespace Spectre.Console;

internal sealed class FormFieldState
{
    public int SelectedIndex { get; set; }
    public List<string> SelectedValues { get; set; } = new List<string>();
    public string InputText { get; set; } = string.Empty;
    public int CursorPosition { get; set; }
    public ValidationResult? ValidationResult { get; set; }
    public bool IsValidating { get; set; }
    public bool HasChanged { get; set; }

    public object? GetValue()
    {
        if (SelectedValues.Count > 0)
        {
            return SelectedValues;
        }
        if (!string.IsNullOrEmpty(InputText))
        {
            return InputText;
        }
        return null;
    }
}
