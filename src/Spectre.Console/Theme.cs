namespace Spectre.Console;

/// <summary>
/// Represents a theme that can be applied to the console.
/// </summary>
public sealed class Theme
{
    /// <summary>
    /// Gets the box border style.
    /// </summary>
    public BoxBorder BoxBorder { get; }

    /// <summary>
    /// Gets the table border style.
    /// </summary>
    public TableBorder TableBorder { get; }

    /// <summary>
    /// Gets the tree guide style.
    /// </summary>
    public TreeGuide TreeGuide { get; }

    /// <summary>
    /// Gets the color system for this theme.
    /// </summary>
    public ColorSystem ColorSystem { get; }

    /// <summary>
    /// Gets a value indicating whether this theme uses Unicode.
    /// </summary>
    public bool UsesUnicode { get; }

    /// <summary>
    /// Gets a value indicating whether this theme uses emoji.
    /// </summary>
    public bool UsesEmoji { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Theme"/> class.
    /// </summary>
    /// <param name="boxBorder">The box border style.</param>
    /// <param name="tableBorder">The table border style.</param>
    /// <param name="treeGuide">The tree guide style.</param>
    /// <param name="colorSystem">The color system.</param>
    /// <param name="usesUnicode">Whether to use Unicode characters.</param>
    /// <param name="usesEmoji">Whether to use emoji characters.</param>
    public Theme(
        BoxBorder? boxBorder = null,
        TableBorder? tableBorder = null,
        TreeGuide? treeGuide = null,
        ColorSystem? colorSystem = null,
        bool usesUnicode = true,
        bool usesEmoji = false)
    {
        BoxBorder = boxBorder ?? BoxBorder.Square;
        TableBorder = tableBorder ?? TableBorder.Square;
        TreeGuide = treeGuide ?? TreeGuide.Line;
        ColorSystem = colorSystem ?? ColorSystem.TrueColor;
        UsesUnicode = usesUnicode;
        UsesEmoji = usesEmoji;
    }

    /// <summary>
    /// Gets the default modern theme with full Unicode support.
    /// </summary>
    public static Theme Modern => new Theme(
        boxBorder: BoxBorder.Rounded,
        tableBorder: TableBorder.Rounded,
        treeGuide: TreeGuide.Line,
        usesUnicode: true,
        usesEmoji: true);

    /// <summary>
    /// Gets the default classic theme with Unicode support but no emoji.
    /// </summary>
    public static Theme Classic => new Theme(
        boxBorder: BoxBorder.Square,
        tableBorder: TableBorder.Square,
        treeGuide: TreeGuide.Line,
        usesUnicode: true,
        usesEmoji: false);

    /// <summary>
    /// Gets the plain ASCII theme for maximum compatibility.
    /// </summary>
    public static Theme Plain => new Theme(
        boxBorder: BoxBorder.Ascii,
        tableBorder: TableBorder.Ascii,
        treeGuide: TreeGuide.Ascii,
        usesUnicode: false,
        usesEmoji: false);

    /// <summary>
    /// Gets the minimal theme.
    /// </summary>
    public static Theme Minimal => new Theme(
        boxBorder: BoxBorder.None,
        tableBorder: TableBorder.Minimal,
        treeGuide: TreeGuide.Line,
        usesUnicode: true,
        usesEmoji: false);

    /// <summary>
    /// Creates a theme based on the specified capabilities.
    /// </summary>
    /// <param name="capabilities">The console capabilities.</param>
    /// <returns>A theme optimized for the given capabilities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if capabilities is null.</exception>
    public static Theme Create(IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        return Create(
            capabilities.Unicode,
            capabilities.Emoji,
            capabilities.ColorSystem);
    }

    /// <summary>
    /// Creates a theme based on the specified parameters.
    /// </summary>
    /// <param name="supportsUnicode">Whether Unicode is supported.</param>
    /// <param name="supportsEmoji">Whether emoji is supported.</param>
    /// <param name="colorSystem">The supported color system.</param>
    /// <returns>A theme optimized for the given parameters.</returns>
    public static Theme Create(
        bool supportsUnicode,
        bool supportsEmoji = false,
        ColorSystem? colorSystem = null)
    {
        if (!supportsUnicode)
        {
            return new Theme(
                boxBorder: BoxBorder.Ascii,
                tableBorder: TableBorder.Ascii,
                treeGuide: TreeGuide.Ascii,
                colorSystem: colorSystem ?? ColorSystem.Legacy,
                usesUnicode: false,
                usesEmoji: false);
        }

        if (supportsEmoji)
        {
            return new Theme(
                boxBorder: BoxBorder.Rounded,
                tableBorder: TableBorder.Rounded,
                treeGuide: TreeGuide.Line,
                colorSystem: colorSystem ?? ColorSystem.TrueColor,
                usesUnicode: true,
                usesEmoji: true);
        }

        return new Theme(
            boxBorder: BoxBorder.Square,
            tableBorder: TableBorder.Square,
            treeGuide: TreeGuide.Line,
            colorSystem: colorSystem ?? ColorSystem.EightBit,
            usesUnicode: true,
            usesEmoji: false);
    }
}
