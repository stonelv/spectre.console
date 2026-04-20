namespace Spectre.Console.Json;

/// <summary>
/// Styles for rendering JSON diff.
/// </summary>
public sealed class JsonDiffStyles
{
    /// <summary>
    /// Gets or sets the style used for braces.
    /// </summary>
    public Style? BracesStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for brackets.
    /// </summary>
    public Style? BracketsStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for member names.
    /// </summary>
    public Style? MemberStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for colons.
    /// </summary>
    public Style? ColonStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for commas.
    /// </summary>
    public Style? CommaStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for string literals.
    /// </summary>
    public Style? StringStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for number literals.
    /// </summary>
    public Style? NumberStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for boolean literals.
    /// </summary>
    public Style? BooleanStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for null literals.
    /// </summary>
    public Style? NullStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for added elements.
    /// </summary>
    public Style? AddedStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for deleted elements.
    /// </summary>
    public Style? DeletedStyle { get; set; }

    /// <summary>
    /// Gets or sets the style used for modified elements.
    /// </summary>
    public Style? ModifiedStyle { get; set; }
}
