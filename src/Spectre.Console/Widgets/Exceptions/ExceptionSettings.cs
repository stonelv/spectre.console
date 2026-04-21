namespace Spectre.Console;

/// <summary>
/// Exception settings.
/// </summary>
public sealed class ExceptionSettings
{
    /// <summary>
    /// Gets or sets the exception format.
    /// </summary>
    public ExceptionFormats Format { get; set; }

    /// <summary>
    /// Gets or sets the exception style.
    /// </summary>
    public ExceptionStyle Style { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show inner exceptions.
    /// </summary>
    /// <remarks>
    /// Default value is <c>true</c> for backward compatibility.
    /// </remarks>
    public bool ShowInnerExceptions { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum depth of inner exceptions to show.
    /// </summary>
    /// <remarks>
    /// Set to <c>null</c> for no limit. Default value is <c>null</c>.
    /// This setting only has effect when <see cref="ShowInnerExceptions"/> is <c>true</c>.
    /// </remarks>
    public int? MaxInnerExceptionDepth { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionSettings"/> class.
    /// </summary>
    public ExceptionSettings()
    {
        Format = ExceptionFormats.Default;
        Style = new ExceptionStyle();
    }
}