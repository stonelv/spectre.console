namespace Spectre.Console.Json;

/// <summary>
/// Contains extension methods for <see cref="JsonDiffText"/>.
/// </summary>
public static class JsonDiffTextExtensions
{
    /// <summary>
    /// Sets the style used for braces.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText BracesStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.BracesStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for brackets.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText BracketStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.BracketsStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for member names.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText MemberStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.MemberStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for colons.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText ColonStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.ColonStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for commas.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText CommaStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.CommaStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for string literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText StringStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.StringStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for number literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText NumberStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.NumberStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for boolean literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText BooleanStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.BooleanStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for null literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText NullStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.NullStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for added elements.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText AddedStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.AddedStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for deleted elements.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText DeletedStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.DeletedStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the style used for modified elements.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="style">The style to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText ModifiedStyle(this JsonDiffText text, Style? style)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.ModifiedStyle = style;
        return text;
    }

    /// <summary>
    /// Sets the color used for braces.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText BracesColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.BracesStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for brackets.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText BracketColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.BracketsStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for member names.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText MemberColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.MemberStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for colons.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText ColonColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.ColonStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for commas.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText CommaColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.CommaStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for string literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText StringColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.StringStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for number literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText NumberColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.NumberStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for boolean literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText BooleanColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.BooleanStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for null literals.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText NullColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.NullStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for added elements.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText AddedColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.AddedStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for deleted elements.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText DeletedColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.DeletedStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets the color used for modified elements.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText ModifiedColor(this JsonDiffText text, Color color)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.ModifiedStyle = new Style(color);
        return text;
    }

    /// <summary>
    /// Sets whether to ignore the order of properties.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="ignore">Whether to ignore property order.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText IgnorePropertyOrder(this JsonDiffText text, bool ignore = true)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.IgnorePropertyOrder = ignore;
        return text;
    }

    /// <summary>
    /// Sets whether to ignore case when comparing property names.
    /// </summary>
    /// <param name="text">The JSON diff text instance.</param>
    /// <param name="ignore">Whether to ignore case.</param>
    /// <returns>The same instance so that multiple calls can be chained.</returns>
    public static JsonDiffText IgnoreCase(this JsonDiffText text, bool ignore = true)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        text.IgnoreCase = ignore;
        return text;
    }
}
