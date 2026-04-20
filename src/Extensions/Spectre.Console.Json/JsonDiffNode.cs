namespace Spectre.Console.Json;

/// <summary>
/// Represents a node in the JSON difference tree.
/// </summary>
public sealed class JsonDiffNode
{
    /// <summary>
    /// Gets the type of difference.
    /// </summary>
    public JsonDiffType DiffType { get; }

    /// <summary>
    /// Gets the left (original) JSON syntax node.
    /// </summary>
    public JsonSyntax? Left { get; }

    /// <summary>
    /// Gets the right (modified) JSON syntax node.
    /// </summary>
    public JsonSyntax? Right { get; }

    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    public IReadOnlyList<JsonDiffNode> Children { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDiffNode"/> class.
    /// </summary>
    /// <param name="diffType">The type of difference.</param>
    /// <param name="left">The left JSON syntax node.</param>
    /// <param name="right">The right JSON syntax node.</param>
    /// <param name="children">The child nodes.</param>
    public JsonDiffNode(
        JsonDiffType diffType,
        JsonSyntax? left,
        JsonSyntax? right,
        List<JsonDiffNode>? children = null)
    {
        DiffType = diffType;
        Left = left;
        Right = right;
        Children = children ?? new List<JsonDiffNode>();
    }
}
