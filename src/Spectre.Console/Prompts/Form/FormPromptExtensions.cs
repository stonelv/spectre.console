namespace Spectre.Console;

public static class FormPromptExtensions
{
    public static FormPrompt Title(this FormPrompt form, string title)
    {
        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        form.Title = title;
        return form;
    }

    public static FormPrompt TitleStyle(this FormPrompt form, Style style)
    {
        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        form.TitleStyle = style;
        return form;
    }

    public static FormPrompt FocusStyle(this FormPrompt form, Style style)
    {
        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        form.FocusStyle = style;
        return form;
    }

    public static FormPrompt ErrorStyle(this FormPrompt form, Style style)
    {
        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        form.ErrorStyle = style;
        return form;
    }

    public static FormPrompt InstructionsText(this FormPrompt form, string text)
    {
        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        form.InstructionsText = text;
        return form;
    }

    public static FormResult Prompt(this IAnsiConsole console, FormPrompt form)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        return form.Show(console);
    }

    public static Task<FormResult> PromptAsync(this IAnsiConsole console, FormPrompt form, CancellationToken cancellationToken = default)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        return form.ShowAsync(console, cancellationToken);
    }
}

public static class TextFormFieldExtensions
{
    public static TextFormField IsRequired(this TextFormField field, bool required = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.IsRequired = required;
        return field;
    }

    public static TextFormField AllowEmpty(this TextFormField field, bool allow = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.AllowEmpty = allow;
        return field;
    }

    public static TextFormField Placeholder(this TextFormField field, string placeholder)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.Placeholder = placeholder;
        return field;
    }

    public static TextFormField MaxLength(this TextFormField field, int maxLength)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MaxLength = maxLength;
        return field;
    }

    public static TextFormField DefaultValue(this TextFormField field, string value)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.DefaultValue = value;
        return field;
    }

    public static TextFormField Validate(this TextFormField field, Func<string?, ValidationResult> validator)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = validator;
        return field;
    }

    public static TextFormField Validate(this TextFormField field, Func<string?, bool> validator, string? errorMessage = null)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = value =>
        {
            if (validator(value))
            {
                return ValidationResult.Success();
            }
            return ValidationResult.Error(errorMessage);
        };
        return field;
    }

    public static TextFormField ValidationErrorMessage(this TextFormField field, string message)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.ValidationErrorMessage = message;
        return field;
    }

    public static TextFormField LabelStyle(this TextFormField field, Style style)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        field.LabelStyle = style;
        return field;
    }
}

public static class PasswordFormFieldExtensions
{
    public static PasswordFormField IsRequired(this PasswordFormField field, bool required = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.IsRequired = required;
        return field;
    }

    public static PasswordFormField Mask(this PasswordFormField field, char? mask)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.Mask = mask;
        return field;
    }

    public static PasswordFormField MinLength(this PasswordFormField field, int minLength)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MinLength = minLength;
        return field;
    }

    public static PasswordFormField MaxLength(this PasswordFormField field, int maxLength)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MaxLength = maxLength;
        return field;
    }

    public static PasswordFormField Validate(this PasswordFormField field, Func<string?, ValidationResult> validator)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = validator;
        return field;
    }

    public static PasswordFormField Validate(this PasswordFormField field, Func<string?, bool> validator, string? errorMessage = null)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = value =>
        {
            if (validator(value))
            {
                return ValidationResult.Success();
            }
            return ValidationResult.Error(errorMessage);
        };
        return field;
    }

    public static PasswordFormField ValidationErrorMessage(this PasswordFormField field, string message)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.ValidationErrorMessage = message;
        return field;
    }

    public static PasswordFormField LabelStyle(this PasswordFormField field, Style style)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        field.LabelStyle = style;
        return field;
    }
}

public static class SelectionFormFieldExtensions
{
    public static SelectionFormField IsRequired(this SelectionFormField field, bool required = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.IsRequired = required;
        return field;
    }

    public static SelectionFormField PageSize(this SelectionFormField field, int pageSize)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.PageSize = pageSize;
        return field;
    }

    public static SelectionFormField WrapAround(this SelectionFormField field, bool wrap = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.WrapAround = wrap;
        return field;
    }

    public static SelectionFormField DefaultIndex(this SelectionFormField field, int index)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.DefaultIndex = index;
        return field;
    }

    public static SelectionFormField MoreChoicesText(this SelectionFormField field, string text)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MoreChoicesText = text;
        return field;
    }

    public static SelectionFormField Validate(this SelectionFormField field, Func<string?, ValidationResult> validator)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = validator;
        return field;
    }

    public static SelectionFormField Validate(this SelectionFormField field, Func<string?, bool> validator, string? errorMessage = null)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = value =>
        {
            if (validator(value))
            {
                return ValidationResult.Success();
            }
            return ValidationResult.Error(errorMessage);
        };
        return field;
    }

    public static SelectionFormField ValidationErrorMessage(this SelectionFormField field, string message)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.ValidationErrorMessage = message;
        return field;
    }

    public static SelectionFormField LabelStyle(this SelectionFormField field, Style style)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        field.LabelStyle = style;
        return field;
    }
}

public static class MultiSelectionFormFieldExtensions
{
    public static MultiSelectionFormField IsRequired(this MultiSelectionFormField field, bool required = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.Required = required;
        field.IsRequired = required;
        return field;
    }

    public static MultiSelectionFormField PageSize(this MultiSelectionFormField field, int pageSize)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.PageSize = pageSize;
        return field;
    }

    public static MultiSelectionFormField WrapAround(this MultiSelectionFormField field, bool wrap = true)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.WrapAround = wrap;
        return field;
    }

    public static MultiSelectionFormField MinSelections(this MultiSelectionFormField field, int min)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MinSelections = min;
        return field;
    }

    public static MultiSelectionFormField MaxSelections(this MultiSelectionFormField field, int max)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MaxSelections = max;
        return field;
    }

    public static MultiSelectionFormField DefaultIndexes(this MultiSelectionFormField field, params int[] indexes)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.DefaultIndexes = new List<int>(indexes);
        return field;
    }

    public static MultiSelectionFormField MoreChoicesText(this MultiSelectionFormField field, string text)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.MoreChoicesText = text;
        return field;
    }

    public static MultiSelectionFormField InstructionsText(this MultiSelectionFormField field, string text)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.InstructionsText = text;
        return field;
    }

    public static MultiSelectionFormField Validate(this MultiSelectionFormField field, Func<List<string>?, ValidationResult> validator)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = validator;
        return field;
    }

    public static MultiSelectionFormField Validate(this MultiSelectionFormField field, Func<List<string>?, bool> validator, string? errorMessage = null)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        field.Validator = value =>
        {
            if (validator(value))
            {
                return ValidationResult.Success();
            }
            return ValidationResult.Error(errorMessage);
        };
        return field;
    }

    public static MultiSelectionFormField ValidationErrorMessage(this MultiSelectionFormField field, string message)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.ValidationErrorMessage = message;
        return field;
    }

    public static MultiSelectionFormField LabelStyle(this MultiSelectionFormField field, Style style)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (style is null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        field.LabelStyle = style;
        return field;
    }
}
