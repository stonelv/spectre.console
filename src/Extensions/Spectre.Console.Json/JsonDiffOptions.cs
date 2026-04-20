namespace Spectre.Console.Json;

/// <summary>
/// Options for configuring JSON diff behavior.
/// </summary>
public class JsonDiffOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore the order of properties.
    /// </summary>
    public bool IgnorePropertyOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore case when comparing property names.
    /// </summary>
    public bool IgnoreCase { get; set; }
}
