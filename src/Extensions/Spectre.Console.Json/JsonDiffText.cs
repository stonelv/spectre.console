namespace Spectre.Console.Json;

/// <summary>
/// A renderable component that displays the difference between two JSON documents.
/// </summary>
public sealed class JsonDiffText : JustInTimeRenderable
{
    private readonly string _leftJson;
    private readonly string _rightJson;
    private JsonDiffNode? _diffRoot;
    private IJsonParser? _parser;

    /// <summary>
    /// Gets or sets the styles used for rendering.
    /// </summary>
    public JsonDiffStyles Styles { get; set; } = new JsonDiffStyles();

    /// <summary>
    /// Gets or sets the options for diff behavior.
    /// </summary>
    public JsonDiffOptions Options { get; set; } = new JsonDiffOptions();

    /// <summary>
    /// Gets or sets the style used for braces.
    /// </summary>
    public Style? BracesStyle
    {
        get => Styles.BracesStyle;
        set => Styles.BracesStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for brackets.
    /// </summary>
    public Style? BracketsStyle
    {
        get => Styles.BracketsStyle;
        set => Styles.BracketsStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for member names.
    /// </summary>
    public Style? MemberStyle
    {
        get => Styles.MemberStyle;
        set => Styles.MemberStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for colons.
    /// </summary>
    public Style? ColonStyle
    {
        get => Styles.ColonStyle;
        set => Styles.ColonStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for commas.
    /// </summary>
    public Style? CommaStyle
    {
        get => Styles.CommaStyle;
        set => Styles.CommaStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for string literals.
    /// </summary>
    public Style? StringStyle
    {
        get => Styles.StringStyle;
        set => Styles.StringStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for number literals.
    /// </summary>
    public Style? NumberStyle
    {
        get => Styles.NumberStyle;
        set => Styles.NumberStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for boolean literals.
    /// </summary>
    public Style? BooleanStyle
    {
        get => Styles.BooleanStyle;
        set => Styles.BooleanStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for null literals.
    /// </summary>
    public Style? NullStyle
    {
        get => Styles.NullStyle;
        set => Styles.NullStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for added elements.
    /// </summary>
    public Style? AddedStyle
    {
        get => Styles.AddedStyle;
        set => Styles.AddedStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for deleted elements.
    /// </summary>
    public Style? DeletedStyle
    {
        get => Styles.DeletedStyle;
        set => Styles.DeletedStyle = value;
    }

    /// <summary>
    /// Gets or sets the style used for modified elements.
    /// </summary>
    public Style? ModifiedStyle
    {
        get => Styles.ModifiedStyle;
        set => Styles.ModifiedStyle = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore the order of properties.
    /// </summary>
    public bool IgnorePropertyOrder
    {
        get => Options.IgnorePropertyOrder;
        set => Options.IgnorePropertyOrder = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore case when comparing property names.
    /// </summary>
    public bool IgnoreCase
    {
        get => Options.IgnoreCase;
        set => Options.IgnoreCase = value;
    }

    /// <summary>
    /// Gets or sets the JSON parser.
    /// </summary>
    public IJsonParser? Parser
    {
        get => _parser;
        set
        {
            _diffRoot = null;
            _parser = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDiffText"/> class.
    /// </summary>
    /// <param name="leftJson">The left (original) JSON string.</param>
    /// <param name="rightJson">The right (modified) JSON string.</param>
    public JsonDiffText(string leftJson, string rightJson)
    {
        _leftJson = leftJson ?? throw new ArgumentNullException(nameof(leftJson));
        _rightJson = rightJson ?? throw new ArgumentNullException(nameof(rightJson));
    }

    /// <inheritdoc/>
    protected override IRenderable Build()
    {
        if (_diffRoot == null)
        {
            var parser = Parser ?? JsonParser.Shared;
            var left = parser.Parse(_leftJson);
            var right = parser.Parse(_rightJson);
            var differ = new JsonDiffer(Options);
            _diffRoot = differ.Diff(left, right);
        }

        var defaultStyles = new JsonDiffStyles
        {
            BracesStyle = BracesStyle ?? Color.Grey,
            BracketsStyle = BracketsStyle ?? Color.Grey,
            MemberStyle = MemberStyle ?? Color.Blue,
            ColonStyle = ColonStyle ?? Color.Yellow,
            CommaStyle = CommaStyle ?? Color.Grey,
            StringStyle = StringStyle ?? Color.Red,
            NumberStyle = NumberStyle ?? Color.Green,
            BooleanStyle = BooleanStyle ?? Color.Green,
            NullStyle = NullStyle ?? Color.Grey,
            AddedStyle = AddedStyle ?? new Style(Color.Green),
            DeletedStyle = DeletedStyle ?? new Style(Color.Red),
            ModifiedStyle = ModifiedStyle ?? new Style(Color.Yellow),
        };

        return BuildTree(_diffRoot, defaultStyles);
    }

    private Tree BuildTree(JsonDiffNode root, JsonDiffStyles styles)
    {
        var rootLabel = GetRootLabel(root, styles);
        var tree = new Tree(rootLabel);
        AddNodes(tree.Nodes, root.Children, styles);
        return tree;
    }

    private IRenderable GetRootLabel(JsonDiffNode root, JsonDiffStyles styles)
    {
        var markup = new Markup("[bold]JSON Diff[/]");
        return markup;
    }

    private void AddNodes(List<TreeNode> parentNodes, IReadOnlyList<JsonDiffNode> children, JsonDiffStyles styles)
    {
        foreach (var child in children)
        {
            var nodeContent = RenderNodeContent(child, styles);
            var treeNode = new TreeNode(nodeContent);
            parentNodes.Add(treeNode);

            if (child.Children.Count > 0)
            {
                AddNodes(treeNode.Nodes, child.Children, styles);
            }
        }
    }

    private IRenderable RenderNodeContent(JsonDiffNode node, JsonDiffStyles styles)
    {
        var paragraph = new Paragraph();

        var prefix = GetDiffPrefix(node.DiffType);
        var prefixStyle = GetDiffStyle(node.DiffType, styles);

        if (!string.IsNullOrEmpty(prefix))
        {
            paragraph.Append(prefix, prefixStyle);
            paragraph.Append(" ");
        }

        if (node.Left is JsonMember leftMember && node.Right is JsonMember rightMember)
        {
            RenderMember(paragraph, leftMember, rightMember, node.DiffType, styles);
        }
        else if (node.Left is JsonMember member)
        {
            RenderMemberSingle(paragraph, member, node.DiffType, styles, isLeft: true);
        }
        else if (node.Right is JsonMember memberRight)
        {
            RenderMemberSingle(paragraph, memberRight, node.DiffType, styles, isLeft: false);
        }
        else
        {
            RenderValue(paragraph, node, styles);
        }

        return paragraph;
    }

    private void RenderMember(Paragraph paragraph, JsonMember left, JsonMember right, JsonDiffType diffType, JsonDiffStyles styles)
    {
        var style = GetDiffStyle(diffType, styles);

        paragraph.Append(left.Name, style ?? styles.MemberStyle);
        paragraph.Append(": ", styles.ColonStyle);

        var leftIsComplex = left.Value is JsonObject or JsonArray;
        var rightIsComplex = right.Value is JsonObject or JsonArray;

        if (diffType == JsonDiffType.Modified)
        {
            if (leftIsComplex || rightIsComplex)
            {
                paragraph.Append("(", style);
                RenderComplexValueInline(paragraph, left.Value, styles, isDeleted: true);
                paragraph.Append(" → ", style);
                RenderComplexValueInline(paragraph, right.Value, styles, isAdded: true);
                paragraph.Append(")", style);
            }
            else
            {
                paragraph.Append("(", style);
                RenderValueInline(paragraph, left.Value, styles, isDeleted: true);
                paragraph.Append(" → ", style);
                RenderValueInline(paragraph, right.Value, styles, isAdded: true);
                paragraph.Append(")", style);
            }
        }
        else
        {
            if (rightIsComplex)
            {
                RenderComplexValueInline(paragraph, right.Value, styles);
            }
            else
            {
                RenderValueInline(paragraph, right.Value, styles);
            }
        }
    }

    private void RenderMemberSingle(Paragraph paragraph, JsonMember member, JsonDiffType diffType, JsonDiffStyles styles, bool isLeft)
    {
        var style = GetDiffStyle(diffType, styles);

        paragraph.Append(member.Name, style ?? styles.MemberStyle);
        paragraph.Append(": ", styles.ColonStyle);
        
        var isComplex = member.Value is JsonObject or JsonArray;
        
        if (isLeft)
        {
            if (isComplex)
            {
                RenderComplexValueInline(paragraph, member.Value, styles, isDeleted: true);
            }
            else
            {
                RenderValueInline(paragraph, member.Value, styles, isDeleted: true);
            }
        }
        else
        {
            if (isComplex)
            {
                RenderComplexValueInline(paragraph, member.Value, styles, isAdded: true);
            }
            else
            {
                RenderValueInline(paragraph, member.Value, styles, isAdded: true);
            }
        }
    }

    private void RenderComplexValueInline(Paragraph paragraph, JsonSyntax value, JsonDiffStyles styles, bool isDeleted = false, bool isAdded = false)
    {
        var style = isDeleted ? styles.DeletedStyle : isAdded ? styles.AddedStyle : null;

        if (value is JsonObject)
        {
            paragraph.Append("{...}", style ?? styles.BracesStyle);
        }
        else if (value is JsonArray)
        {
            paragraph.Append("[...]", style ?? styles.BracketsStyle);
        }
    }

    private void RenderValue(Paragraph paragraph, JsonDiffNode node, JsonDiffStyles styles)
    {
        var style = GetDiffStyle(node.DiffType, styles);

        if (node.Left is JsonObject || node.Right is JsonObject)
        {
            paragraph.Append("{...}", style ?? styles.BracesStyle);
        }
        else if (node.Left is JsonArray || node.Right is JsonArray)
        {
            paragraph.Append("[...]", style ?? styles.BracketsStyle);
        }
        else if (node.DiffType == JsonDiffType.Modified && node.Left != null && node.Right != null)
        {
            paragraph.Append("(", style);
            RenderValueInline(paragraph, node.Left, styles, isDeleted: true);
            paragraph.Append(" → ", style);
            RenderValueInline(paragraph, node.Right, styles, isAdded: true);
            paragraph.Append(")", style);
        }
        else if (node.Left != null)
        {
            RenderValueInline(paragraph, node.Left, styles, isDeleted: node.DiffType == JsonDiffType.Deleted);
        }
        else if (node.Right != null)
        {
            RenderValueInline(paragraph, node.Right, styles, isAdded: node.DiffType == JsonDiffType.Added);
        }
    }

    private void RenderValueInline(Paragraph paragraph, JsonSyntax value, JsonDiffStyles styles, bool isDeleted = false, bool isAdded = false)
    {
        var style = isDeleted ? styles.DeletedStyle : isAdded ? styles.AddedStyle : null;

        switch (value)
        {
            case JsonString str:
                paragraph.Append(str.Lexeme, style ?? styles.StringStyle);
                break;
            case JsonNumber num:
                paragraph.Append(num.Lexeme, style ?? styles.NumberStyle);
                break;
            case JsonBoolean boolean:
                paragraph.Append(boolean.Lexeme, style ?? styles.BooleanStyle);
                break;
            case JsonNull nullVal:
                paragraph.Append(nullVal.Lexeme, style ?? styles.NullStyle);
                break;
        }
    }

    private static string GetDiffPrefix(JsonDiffType diffType)
    {
        return diffType switch
        {
            JsonDiffType.Added => "+",
            JsonDiffType.Deleted => "-",
            JsonDiffType.Modified => "~",
            _ => "",
        };
    }

    private static Style? GetDiffStyle(JsonDiffType diffType, JsonDiffStyles styles)
    {
        return diffType switch
        {
            JsonDiffType.Added => styles.AddedStyle,
            JsonDiffType.Deleted => styles.DeletedStyle,
            JsonDiffType.Modified => styles.ModifiedStyle,
            _ => null,
        };
    }
}
