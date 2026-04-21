using System.Text;

namespace Spectre.Console;

public sealed class FormPrompt : IPrompt<FormResult>
{
    private readonly List<IFormField> _fields = new List<IFormField>();
    private readonly Dictionary<string, FormFieldState> _fieldStates = new Dictionary<string, FormFieldState>();

    public string? Title { get; set; }
    public Style? TitleStyle { get; set; }
    public string? SubmitText { get; set; } = "[green]Submit[/]";
    public Style? FocusStyle { get; set; }
    public Style? ErrorStyle { get; set; }
    public string? InstructionsText { get; set; } = "[grey](Use <Tab>/<Shift+Tab> to move between fields, <Up>/<Down> to select options, <Enter> to submit)[/]";

    public IReadOnlyList<IFormField> Fields => _fields.AsReadOnly();

    public FormPrompt()
    {
    }

    public FormPrompt AddField(IFormField field)
    {
        _fields.Add(field);
        _fieldStates[field.Id] = new FormFieldState();

        if (field is TextFormField textField)
        {
            if (textField.DefaultValue != null)
            {
                _fieldStates[field.Id].InputText = textField.DefaultValue;
                field.SetValue(textField.DefaultValue);
            }
        }
        else if (field is SelectionFormField selectionField)
        {
            if (selectionField.DefaultIndex.HasValue &&
                selectionField.DefaultIndex >= 0 &&
                selectionField.DefaultIndex < selectionField.Choices.Count)
            {
                _fieldStates[field.Id].SelectedIndex = selectionField.DefaultIndex.Value;
                field.SetValue(selectionField.Choices[selectionField.DefaultIndex.Value].Value);
            }
        }
        else if (field is MultiSelectionFormField multiField)
        {
            if (multiField.DefaultIndexes != null && multiField.DefaultIndexes.Count > 0)
            {
                var values = new List<string>();
                foreach (var idx in multiField.DefaultIndexes)
                {
                    if (idx >= 0 && idx < multiField.Choices.Count)
                    {
                        values.Add(multiField.Choices[idx].Value);
                    }
                }
                _fieldStates[field.Id].SelectedValues = values;
                _fieldStates[field.Id].SelectedIndex = multiField.DefaultIndexes.FirstOrDefault();
                field.SetValue(values);
            }
        }

        return this;
    }

    public FormPrompt AddFields(IEnumerable<IFormField> fields)
    {
        foreach (var field in fields)
        {
            AddField(field);
        }
        return this;
    }

    public FormResult Show(IAnsiConsole console)
    {
        return ShowAsync(console, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task<FormResult> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (!console.Profile.Capabilities.Interactive)
        {
            throw new NotSupportedException(
                "Cannot show form prompt since the current terminal isn't interactive.");
        }

        if (!console.Profile.Capabilities.Ansi)
        {
            throw new NotSupportedException(
                "Cannot show form prompt since the current terminal does not support ANSI escape sequences.");
        }

        if (_fields.Count == 0)
        {
            throw new InvalidOperationException("Cannot show an empty form. Please add fields first.");
        }

        var currentFieldIndex = 0;
        var focusStyle = FocusStyle ?? Color.Blue;
        var errorStyle = ErrorStyle ?? Color.Red;

        return await console.RunExclusive(async () =>
        {
            var hook = new FormRenderHook(console, () => BuildRenderable(currentFieldIndex, focusStyle, errorStyle));

            using (new RenderHookScope(console, hook))
            {
                console.Cursor.Hide();
                hook.Refresh();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var rawKey = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
                    if (rawKey == null)
                    {
                        continue;
                    }

                    var key = rawKey.Value;
                    var currentField = _fields[currentFieldIndex];
                    var state = _fieldStates[currentField.Id];

                    var result = HandleInput(key, currentFieldIndex, state);

                    if (result == FormInputResult.Submit)
                    {
                        var allValid = true;
                        foreach (var field in _fields)
                        {
                            var fieldState = _fieldStates[field.Id];
                            var validationResult = field.Validate();
                            fieldState.ValidationResult = validationResult;

                            if (!validationResult.Successful)
                            {
                                allValid = false;
                            }
                        }

                        if (allValid)
                        {
                            break;
                        }

                        hook.Refresh();
                        continue;
                    }

                    if (result == FormInputResult.NextField)
                    {
                        if (currentFieldIndex < _fields.Count - 1)
                        {
                            currentFieldIndex++;
                        }
                        else
                        {
                            currentFieldIndex = 0;
                        }
                    }
                    else if (result == FormInputResult.PreviousField)
                    {
                        if (currentFieldIndex > 0)
                        {
                            currentFieldIndex--;
                        }
                        else
                        {
                            currentFieldIndex = _fields.Count - 1;
                        }
                    }

                    if (state.HasChanged)
                    {
                        UpdateFieldValue(currentField, state);

                        var validationResult = currentField.Validate();
                        state.ValidationResult = validationResult;
                        state.HasChanged = false;
                    }

                    hook.Refresh();
                }
            }

            hook.Clear();
            console.Cursor.Show();

            var formResult = new FormResult();
            foreach (var field in _fields)
            {
                formResult.AddValue(field.Id, field.GetTypedValue());
            }

            return formResult;
        }).ConfigureAwait(false);
    }

    private FormInputResult HandleInput(ConsoleKeyInfo key, int fieldIndex, FormFieldState state)
    {
        var field = _fields[fieldIndex];

        if (key.Key == ConsoleKey.Tab)
        {
            return key.Modifiers.HasFlag(ConsoleModifiers.Shift)
                ? FormInputResult.PreviousField
                : FormInputResult.NextField;
        }

        if (key.Key == ConsoleKey.Enter)
        {
            return FormInputResult.Submit;
        }

        if (field is TextFormField || field is PasswordFormField)
        {
            if (key.Key == ConsoleKey.Backspace)
            {
                if (state.CursorPosition > 0)
                {
                    state.InputText = state.InputText.Remove(state.CursorPosition - 1, 1);
                    state.CursorPosition--;
                    state.HasChanged = true;
                }
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                if (state.CursorPosition > 0)
                {
                    state.CursorPosition--;
                }
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                if (state.CursorPosition < state.InputText.Length)
                {
                    state.CursorPosition++;
                }
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.Home)
            {
                state.CursorPosition = 0;
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.End)
            {
                state.CursorPosition = state.InputText.Length;
                return FormInputResult.Refresh;
            }

            if (!char.IsControl(key.KeyChar))
            {
                var textField = field as TextFormField;
                var passwordField = field as PasswordFormField;
                var maxLength = textField?.MaxLength ?? passwordField?.MaxLength;

                if (!maxLength.HasValue || state.InputText.Length < maxLength)
                {
                    state.InputText = state.InputText.Insert(state.CursorPosition, key.KeyChar.ToString());
                    state.CursorPosition++;
                    state.HasChanged = true;
                }
                return FormInputResult.Refresh;
            }
        }
        else if (field is SelectionFormField selectionField)
        {
            var choiceCount = selectionField.Choices.Count;

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (state.SelectedIndex > 0)
                {
                    state.SelectedIndex--;
                    state.HasChanged = true;
                }
                else if (selectionField.WrapAround)
                {
                    state.SelectedIndex = choiceCount - 1;
                    state.HasChanged = true;
                }
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                if (state.SelectedIndex < choiceCount - 1)
                {
                    state.SelectedIndex++;
                    state.HasChanged = true;
                }
                else if (selectionField.WrapAround)
                {
                    state.SelectedIndex = 0;
                    state.HasChanged = true;
                }
                return FormInputResult.Refresh;
            }
        }
        else if (field is MultiSelectionFormField multiField)
        {
            var choiceCount = multiField.Choices.Count;

            if (key.Key == ConsoleKey.Spacebar)
            {
                if (state.SelectedIndex >= 0 && state.SelectedIndex < choiceCount)
                {
                    var choice = multiField.Choices[state.SelectedIndex];
                    if (state.SelectedValues.Contains(choice.Value))
                    {
                        state.SelectedValues.Remove(choice.Value);
                    }
                    else
                    {
                        var maxSelections = multiField.MaxSelections;
                        if (!maxSelections.HasValue || state.SelectedValues.Count < maxSelections)
                        {
                            state.SelectedValues.Add(choice.Value);
                        }
                    }
                    state.HasChanged = true;
                }
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (state.SelectedIndex > 0)
                {
                    state.SelectedIndex--;
                }
                else if (multiField.WrapAround)
                {
                    state.SelectedIndex = choiceCount - 1;
                }
                return FormInputResult.Refresh;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                if (state.SelectedIndex < choiceCount - 1)
                {
                    state.SelectedIndex++;
                }
                else if (multiField.WrapAround)
                {
                    state.SelectedIndex = 0;
                }
                return FormInputResult.Refresh;
            }
        }

        return FormInputResult.None;
    }

    private void UpdateFieldValue(IFormField field, FormFieldState state)
    {
        if (field is TextFormField || field is PasswordFormField)
        {
            field.SetValue(state.InputText);
        }
        else if (field is SelectionFormField selectionField)
        {
            if (state.SelectedIndex >= 0 && state.SelectedIndex < selectionField.Choices.Count)
            {
                field.SetValue(selectionField.Choices[state.SelectedIndex].Value);
            }
        }
        else if (field is MultiSelectionFormField)
        {
            field.SetValue(new List<string>(state.SelectedValues));
        }
    }

    private IRenderable BuildRenderable(int currentFieldIndex, Style focusStyle, Style errorStyle)
    {
        var list = new List<IRenderable>();

        if (!string.IsNullOrEmpty(Title))
        {
            var titleStyle = TitleStyle ?? Style.Plain;
            list.Add(new Markup(Title, titleStyle));
            list.Add(new Rule());
        }

        for (var i = 0; i < _fields.Count; i++)
        {
            var field = _fields[i];
            var state = _fieldStates[field.Id];
            var isFocused = i == currentFieldIndex;
            var style = isFocused ? focusStyle : (field.LabelStyle ?? Style.Plain);

            var labelPrefix = isFocused ? "> " : "  ";
            var labelBuilder = new StringBuilder();
            labelBuilder.Append(labelPrefix);

            if (field.IsRequired)
            {
                labelBuilder.Append("[red]*[/] ");
            }

            labelBuilder.Append(field.Label.EscapeMarkup());
            labelBuilder.Append(": ");

            var isTextOrPassword = field is TextFormField || field is PasswordFormField;
            var isSelectionWithValue = !isFocused && (field is SelectionFormField || field is MultiSelectionFormField);

            if (isTextOrPassword || isSelectionWithValue)
            {
                var valueText = RenderInlineValue(field, state, isFocused, style);
                list.Add(new Markup(labelBuilder.ToString() + valueText, style));
            }
            else
            {
                list.Add(new Markup(labelBuilder.ToString(), style));
                RenderExpandedSelection(list, field, state, style);
            }

            if (state.ValidationResult != null && !state.ValidationResult.Successful)
            {
                var errorMessage = state.ValidationResult.Message ?? "[red]Invalid input[/]";
                list.Add(new Markup("  " + errorMessage, errorStyle));
            }
        }

        if (!string.IsNullOrEmpty(SubmitText))
        {
            list.Add(Text.Empty);
            list.Add(new Markup("  " + SubmitText));
        }

        if (!string.IsNullOrEmpty(InstructionsText))
        {
            list.Add(Text.Empty);
            list.Add(new Markup(InstructionsText));
        }

        return new Rows(list);
    }

    private string RenderInlineValue(IFormField field, FormFieldState state, bool isFocused, Style style)
    {
        if (field is TextFormField textField)
        {
            return RenderTextInputInline(state, textField.Placeholder, isFocused, false, null);
        }

        if (field is PasswordFormField passwordField)
        {
            return RenderTextInputInline(state, null, isFocused, true, passwordField.Mask);
        }

        if (field is SelectionFormField selectionField)
        {
            if (state.SelectedIndex >= 0 && state.SelectedIndex < selectionField.Choices.Count)
            {
                return selectionField.Choices[state.SelectedIndex].DisplayText.EscapeMarkup();
            }
            return "[grey]-- Select --[/]";
        }

        if (field is MultiSelectionFormField multiField)
        {
            var selected = multiField.Choices.Where(c => state.SelectedValues.Contains(c.Value)).ToList();
            if (selected.Count > 0)
            {
                return string.Join(", ", selected.Select(c => c.DisplayText)).EscapeMarkup();
            }
            return "[grey]-- Select --[/]";
        }

        return string.Empty;
    }

    private void RenderExpandedSelection(List<IRenderable> list, IFormField field, FormFieldState state, Style style)
    {
        if (field is SelectionFormField selectionField)
        {
            for (var i = 0; i < selectionField.Choices.Count; i++)
            {
                var isSelected = i == state.SelectedIndex;
                var itemStyle = isSelected ? style : Style.Plain;
                var prefix = isSelected ? "  > " : "    ";
                list.Add(new Markup(prefix + selectionField.Choices[i].DisplayText.EscapeMarkup(), itemStyle));
            }
        }
        else if (field is MultiSelectionFormField multiField)
        {
            for (var i = 0; i < multiField.Choices.Count; i++)
            {
                var isSelectedItem = i == state.SelectedIndex;
                var isChecked = state.SelectedValues.Contains(multiField.Choices[i].Value);
                var itemStyle = isSelectedItem ? style : Style.Plain;
                var prefix = isSelectedItem ? "  > " : "    ";
                var checkbox = isChecked ? "[[X]] " : "[[ ]] ";
                list.Add(new Markup(prefix + checkbox + multiField.Choices[i].DisplayText.EscapeMarkup(), itemStyle));
            }
        }
    }

    private string RenderTextInputInline(FormFieldState state, string? placeholder, bool isFocused, bool isSecret, char? mask)
    {
        var builder = new StringBuilder();

        if (string.IsNullOrEmpty(state.InputText) && !string.IsNullOrEmpty(placeholder))
        {
            builder.Append($"[grey]{placeholder.EscapeMarkup()}[/]");
        }
        else if (isSecret && mask.HasValue)
        {
            var masked = new string(mask.Value, state.InputText.Length);
            var escapedMasked = masked.EscapeMarkup();

            if (isFocused)
            {
                var escapedCursorPosition = CalculateEscapedCursorPosition(masked, state.CursorPosition);
                var cursorMarkup = "[underline]_[/]";
                if (escapedCursorPosition < escapedMasked.Length)
                {
                    escapedMasked = escapedMasked.Insert(escapedCursorPosition, cursorMarkup);
                }
                else
                {
                    escapedMasked += cursorMarkup;
                }
            }

            builder.Append(escapedMasked);
        }
        else if (isSecret)
        {
            builder.Append(string.Empty);
        }
        else
        {
            var escapedText = state.InputText.EscapeMarkup();
            if (isFocused)
            {
                var escapedCursorPosition = CalculateEscapedCursorPosition(state.InputText, state.CursorPosition);
                var cursorMarkup = "[underline]_[/]";
                if (escapedCursorPosition < escapedText.Length)
                {
                    escapedText = escapedText.Insert(escapedCursorPosition, cursorMarkup);
                }
                else
                {
                    escapedText += cursorMarkup;
                }
            }
            builder.Append(escapedText);
        }

        return builder.ToString();
    }

    private static int CalculateEscapedCursorPosition(string inputText, int cursorPosition)
    {
        if (cursorPosition <= 0)
        {
            return 0;
        }

        var escapedPosition = 0;
        for (var i = 0; i < cursorPosition && i < inputText.Length; i++)
        {
            if (inputText[i] == '[' || inputText[i] == ']')
            {
                escapedPosition += 2;
            }
            else
            {
                escapedPosition += 1;
            }
        }

        return escapedPosition;
    }
}

internal sealed class FormRenderHook : IRenderHook
{
    private readonly IAnsiConsole _console;
    private readonly Func<IRenderable> _builder;
    private readonly LiveRenderable _live;
    private readonly object _lock;
    private bool _dirty;

    public FormRenderHook(IAnsiConsole console, Func<IRenderable> builder)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));

        _live = new LiveRenderable(console);
        _lock = new object();
        _dirty = true;
    }

    public void Clear()
    {
        _console.Write(_live.RestoreCursor());
    }

    public void Refresh()
    {
        _dirty = true;
        _console.Write(new ControlCode(string.Empty));
    }

    public IEnumerable<IRenderable> Process(RenderOptions options, IEnumerable<IRenderable> renderables)
    {
        lock (_lock)
        {
            if (!_live.HasRenderable || _dirty)
            {
                _live.SetRenderable(_builder());
                _dirty = false;
            }

            yield return _live.PositionCursor(options);

            foreach (var renderable in renderables)
            {
                yield return renderable;
            }

            yield return _live;
        }
    }
}
