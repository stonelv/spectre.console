namespace Spectre.Console.Json;

/// <summary>
/// Computes the difference between two JSON documents.
/// </summary>
public sealed class JsonDiffer
{
    private readonly JsonDiffOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDiffer"/> class.
    /// </summary>
    /// <param name="options">The diff options.</param>
    public JsonDiffer(JsonDiffOptions? options = null)
    {
        _options = options ?? new JsonDiffOptions();
    }

    /// <summary>
    /// Computes the difference between two JSON strings.
    /// </summary>
    /// <param name="leftJson">The left JSON string.</param>
    /// <param name="rightJson">The right JSON string.</param>
    /// <returns>A <see cref="JsonDiffNode"/> representing the difference.</returns>
    public JsonDiffNode Diff(string leftJson, string rightJson)
    {
        var parser = JsonParser.Shared;
        var left = parser.Parse(leftJson);
        var right = parser.Parse(rightJson);
        return Diff(left, right);
    }

    /// <summary>
    /// Computes the difference between two JSON syntax nodes.
    /// </summary>
    /// <param name="left">The left JSON syntax node.</param>
    /// <param name="right">The right JSON syntax node.</param>
    /// <returns>A <see cref="JsonDiffNode"/> representing the difference.</returns>
    public JsonDiffNode Diff(JsonSyntax left, JsonSyntax right)
    {
        return CompareSyntax(left, right);
    }

    private JsonDiffNode CompareSyntax(JsonSyntax? left, JsonSyntax? right)
    {
        if (left == null && right == null)
        {
            return new JsonDiffNode(JsonDiffType.Unchanged, null, null);
        }

        if (left == null)
        {
            return new JsonDiffNode(JsonDiffType.Added, null, right, GetChildrenForAddedOrDeleted(right));
        }

        if (right == null)
        {
            return new JsonDiffNode(JsonDiffType.Deleted, left, null, GetChildrenForAddedOrDeleted(left));
        }

        if (left.GetType() != right.GetType())
        {
            return new JsonDiffNode(JsonDiffType.Modified, left, right);
        }

        return left switch
        {
            JsonObject leftObj => CompareObjects(leftObj, (JsonObject)right),
            JsonArray leftArr => CompareArrays(leftArr, (JsonArray)right),
            JsonString leftStr => CompareValues(leftStr, (JsonString)right),
            JsonNumber leftNum => CompareValues(leftNum, (JsonNumber)right),
            JsonBoolean leftBool => CompareValues(leftBool, (JsonBoolean)right),
            JsonNull leftNull => CompareValues(leftNull, (JsonNull)right),
            _ => new JsonDiffNode(JsonDiffType.Unchanged, left, right),
        };
    }

    private static List<JsonDiffNode>? GetChildrenForAddedOrDeleted(JsonSyntax? syntax)
    {
        if (syntax is JsonObject obj)
        {
            var children = new List<JsonDiffNode>();
            foreach (var member in obj.Members)
            {
                var valueChildren = GetChildrenForAddedOrDeleted(member.Value);
                children.Add(new JsonDiffNode(JsonDiffType.Added, null, member, valueChildren));
            }
            return children;
        }

        if (syntax is JsonArray arr)
        {
            var children = new List<JsonDiffNode>();
            foreach (var item in arr.Items)
            {
                children.Add(new JsonDiffNode(JsonDiffType.Added, null, item, GetChildrenForAddedOrDeleted(item)));
            }
            return children;
        }

        return null;
    }

    private JsonDiffNode CompareObjects(JsonObject left, JsonObject right)
    {
        var children = new List<JsonDiffNode>();

        if (_options.IgnorePropertyOrder)
        {
            var leftMembers = _options.IgnoreCase
                ? left.Members.ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase)
                : left.Members.ToDictionary(m => m.Name);

            var rightMembers = _options.IgnoreCase
                ? right.Members.ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase)
                : right.Members.ToDictionary(m => m.Name);

            var allKeys = new HashSet<string>(
                leftMembers.Keys.Concat(rightMembers.Keys),
                _options.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

            foreach (var key in allKeys)
            {
                leftMembers.TryGetValue(key, out var leftMember);
                rightMembers.TryGetValue(key, out var rightMember);

                if (leftMember != null && rightMember != null)
                {
                    var valueDiff = CompareSyntax(leftMember.Value, rightMember.Value);
                    children.Add(new JsonDiffNode(
                        valueDiff.DiffType,
                        leftMember,
                        rightMember,
                        valueDiff.Children.Count > 0 ? valueDiff.Children.ToList() : null));
                }
                else if (leftMember != null)
                {
                    var valueChildren = GetChildrenForAddedOrDeleted(leftMember.Value);
                    children.Add(new JsonDiffNode(JsonDiffType.Deleted, leftMember, null, valueChildren));
                }
                else if (rightMember != null)
                {
                    var valueChildren = GetChildrenForAddedOrDeleted(rightMember.Value);
                    children.Add(new JsonDiffNode(JsonDiffType.Added, null, rightMember, valueChildren));
                }
            }
        }
        else
        {
            var maxCount = Math.Max(left.Members.Count, right.Members.Count);
            for (var i = 0; i < maxCount; i++)
            {
                var leftMember = i < left.Members.Count ? left.Members[i] : null;
                var rightMember = i < right.Members.Count ? right.Members[i] : null;

                if (leftMember != null && rightMember != null)
                {
                    var namesMatch = _options.IgnoreCase
                        ? string.Equals(leftMember.Name, rightMember.Name, StringComparison.OrdinalIgnoreCase)
                        : leftMember.Name == rightMember.Name;

                    if (namesMatch)
                    {
                        var valueDiff = CompareSyntax(leftMember.Value, rightMember.Value);
                        children.Add(new JsonDiffNode(
                            valueDiff.DiffType,
                            leftMember,
                            rightMember,
                            valueDiff.Children.Count > 0 ? valueDiff.Children.ToList() : null));
                    }
                    else
                    {
                        var leftValueChildren = GetChildrenForAddedOrDeleted(leftMember.Value);
                        children.Add(new JsonDiffNode(JsonDiffType.Deleted, leftMember, null, leftValueChildren));
                        
                        var rightValueChildren = GetChildrenForAddedOrDeleted(rightMember.Value);
                        children.Add(new JsonDiffNode(JsonDiffType.Added, null, rightMember, rightValueChildren));
                    }
                }
                else if (leftMember != null)
                {
                    var valueChildren = GetChildrenForAddedOrDeleted(leftMember.Value);
                    children.Add(new JsonDiffNode(JsonDiffType.Deleted, leftMember, null, valueChildren));
                }
                else if (rightMember != null)
                {
                    var valueChildren = GetChildrenForAddedOrDeleted(rightMember.Value);
                    children.Add(new JsonDiffNode(JsonDiffType.Added, null, rightMember, valueChildren));
                }
            }
        }

        var diffType = children.Any(c => c.DiffType != JsonDiffType.Unchanged)
            ? JsonDiffType.Modified
            : JsonDiffType.Unchanged;

        return new JsonDiffNode(diffType, left, right, children);
    }

    private JsonDiffNode CompareArrays(JsonArray left, JsonArray right)
    {
        var children = new List<JsonDiffNode>();
        var maxCount = Math.Max(left.Items.Count, right.Items.Count);

        for (var i = 0; i < maxCount; i++)
        {
            var leftItem = i < left.Items.Count ? left.Items[i] : null;
            var rightItem = i < right.Items.Count ? right.Items[i] : null;

            children.Add(CompareSyntax(leftItem, rightItem));
        }

        var diffType = children.Any(c => c.DiffType != JsonDiffType.Unchanged)
            ? JsonDiffType.Modified
            : JsonDiffType.Unchanged;

        return new JsonDiffNode(diffType, left, right, children);
    }

    private JsonDiffNode CompareValues(JsonSyntax left, JsonSyntax right)
    {
        var leftLexeme = GetLexeme(left);
        var rightLexeme = GetLexeme(right);

        var areEqual = _options.IgnoreCase
            ? string.Equals(leftLexeme, rightLexeme, StringComparison.OrdinalIgnoreCase)
            : leftLexeme == rightLexeme;

        return new JsonDiffNode(
            areEqual ? JsonDiffType.Unchanged : JsonDiffType.Modified,
            left,
            right);
    }

    private static string? GetLexeme(JsonSyntax? syntax)
    {
        return syntax switch
        {
            JsonString s => s.Lexeme,
            JsonNumber n => n.Lexeme,
            JsonBoolean b => b.Lexeme,
            JsonNull n => n.Lexeme,
            _ => null,
        };
    }
}
