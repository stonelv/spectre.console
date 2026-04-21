namespace Spectre.Console;

/// <summary>
/// Contains extension methods for <see cref="IReadOnlyCapabilities"/>.
/// </summary>
public static class CapabilitiesExtensions
{
    /// <summary>
    /// Gets a value indicating whether or not the console supports alternate buffers.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns><c>true</c> if alternate buffers are supported; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method checks for the <c>AlternateBuffer</c> property on the capabilities
    /// implementation using reflection. If not found, it falls back to checking
    /// if ANSI is supported and it's not a legacy console.
    /// </remarks>
    public static bool SupportsAlternateBuffer(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        if (capabilities is Capabilities caps)
        {
            return caps.AlternateBuffer;
        }

        var alternateBufferProp = capabilities.GetType().GetProperty("AlternateBuffer");
        if (alternateBufferProp != null && alternateBufferProp.PropertyType == typeof(bool))
        {
            var value = alternateBufferProp.GetValue(capabilities);
            if (value is bool boolValue)
            {
                return boolValue;
            }
        }

        return capabilities.Ansi && !capabilities.Legacy;
    }

    /// <summary>
    /// Gets a value indicating whether or not the console supports emoji.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns><c>true</c> if emoji is supported; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method checks for the <c>Emoji</c> property on the capabilities
    /// implementation using reflection. If not found, it returns <c>false</c>.
    /// </remarks>
    public static bool SupportsEmoji(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        if (capabilities is Capabilities caps)
        {
            return caps.Emoji;
        }

        var emojiProp = capabilities.GetType().GetProperty("Emoji");
        if (emojiProp != null && emojiProp.PropertyType == typeof(bool))
        {
            var value = emojiProp.GetValue(capabilities);
            if (value is bool boolValue)
            {
                return boolValue;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the default box border based on capabilities.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns>The appropriate box border for the capabilities.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>With emoji support: Returns <see cref="BoxBorder.Rounded"/></description></item>
    /// <item><description>With Unicode but no emoji: Returns <see cref="BoxBorder.Square"/></description></item>
    /// <item><description>Without Unicode: Returns <see cref="BoxBorder.Ascii"/></description></item>
    /// </list>
    /// </remarks>
    public static BoxBorder GetDefaultBoxBorder(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        if (capabilities.SupportsEmoji())
        {
            return BoxBorder.Rounded;
        }

        if (capabilities.Unicode)
        {
            return BoxBorder.Square;
        }

        return BoxBorder.Ascii;
    }

    /// <summary>
    /// Gets the default table border based on capabilities.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns>The appropriate table border for the capabilities.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>With emoji support: Returns <see cref="TableBorder.Rounded"/></description></item>
    /// <item><description>With Unicode but no emoji: Returns <see cref="TableBorder.Square"/></description></item>
    /// <item><description>Without Unicode: Returns <see cref="TableBorder.Ascii"/></description></item>
    /// </list>
    /// </remarks>
    public static TableBorder GetDefaultTableBorder(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        if (capabilities.SupportsEmoji())
        {
            return TableBorder.Rounded;
        }

        if (capabilities.Unicode)
        {
            return TableBorder.Square;
        }

        return TableBorder.Ascii;
    }

    /// <summary>
    /// Gets the default tree guide based on capabilities.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns>The appropriate tree guide for the capabilities.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>With Unicode support: Returns <see cref="TreeGuide.Line"/></description></item>
    /// <item><description>Without Unicode: Returns <see cref="TreeGuide.Ascii"/></description></item>
    /// </list>
    /// </remarks>
    public static TreeGuide GetDefaultTreeGuide(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        if (capabilities.Unicode)
        {
            return TreeGuide.Line;
        }

        return TreeGuide.Ascii;
    }

    /// <summary>
    /// Gets the maximum supported color depth.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns>The maximum color system supported.</returns>
    /// <remarks>
    /// This is the same as <see cref="IReadOnlyCapabilities.ColorSystem"/>,
    /// but provided for consistency with other extension methods.
    /// </remarks>
    public static ColorSystem GetColorSystem(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        return capabilities.ColorSystem;
    }

    /// <summary>
    /// Determines if the capabilities support TrueColor (24-bit color).
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns><c>true</c> if TrueColor is supported; otherwise, <c>false</c>.</returns>
    public static bool SupportsTrueColor(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        return capabilities.ColorSystem >= ColorSystem.TrueColor;
    }

    /// <summary>
    /// Determines if the capabilities support 256 colors.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns><c>true</c> if 256 colors are supported; otherwise, <c>false</c>.</returns>
    public static bool Supports256Colors(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        return capabilities.ColorSystem >= ColorSystem.EightBit;
    }

    /// <summary>
    /// Determines if the capabilities support any colors.
    /// </summary>
    /// <param name="capabilities">The capabilities.</param>
    /// <returns><c>true</c> if colors are supported; otherwise, <c>false</c>.</returns>
    public static bool SupportsColors(this IReadOnlyCapabilities capabilities)
    {
        if (capabilities is null)
        {
            throw new ArgumentNullException(nameof(capabilities));
        }

        return capabilities.ColorSystem > ColorSystem.NoColors;
    }
}
