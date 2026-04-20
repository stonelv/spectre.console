namespace Spectre.Console.Json;

/// <summary>
/// Represents the type of difference between two JSON elements.
/// </summary>
public enum JsonDiffType
{
    /// <summary>
    /// The elements are unchanged.
    /// </summary>
    Unchanged,

    /// <summary>
    /// The element was added in the right JSON.
    /// </summary>
    Added,

    /// <summary>
    /// The element was deleted from the left JSON.
    /// </summary>
    Deleted,

    /// <summary>
    /// The element was modified.
    /// </summary>
    Modified,
}
