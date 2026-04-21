namespace Spectre.Console;

/// <summary>
/// Determines Unicode support.
/// </summary>
public enum UnicodeSupport
{
    /// <summary>
    /// Unicode support should be
    /// detected by the system.
    /// </summary>
    Detect = 0,

    /// <summary>
    /// Unicode is supported.
    /// </summary>
    Yes = 1,

    /// <summary>
    /// Unicode is not supported.
    /// </summary>
    No = 2,
}
