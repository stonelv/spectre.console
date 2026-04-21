namespace Spectre.Console;

/// <summary>
/// Determines hyperlink support.
/// </summary>
public enum LinksSupport
{
    /// <summary>
    /// Hyperlink support should be
    /// detected by the system.
    /// </summary>
    Detect = 0,

    /// <summary>
    /// Hyperlinks are supported.
    /// </summary>
    Yes = 1,

    /// <summary>
    /// Hyperlinks are not supported.
    /// </summary>
    No = 2,
}
