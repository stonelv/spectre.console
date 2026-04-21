namespace Spectre.Console;

/// <summary>
/// Determines emoji support.
/// </summary>
public enum EmojiSupport
{
    /// <summary>
    /// Emoji support should be
    /// detected by the system.
    /// </summary>
    Detect = 0,

    /// <summary>
    /// Emoji is supported.
    /// </summary>
    Yes = 1,

    /// <summary>
    /// Emoji is not supported.
    /// </summary>
    No = 2,
}
