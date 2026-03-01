using System.Threading;
using System.Threading.Tasks;

namespace Spectre.Console;

/// <summary>
/// Contains extensions for <see cref="AnsiConsole"/>.
/// </summary>
public static partial class AnsiConsoleExtensions
{
    /// <summary>
    /// Shows a multi-line code editor prompt.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="prompt">The prompt text.</param>
    /// <returns>The user input as a string.</returns>
    public static string MultiLineCodeEditor(this IAnsiConsole console, string prompt)
    {
        return console.MultiLineCodeEditor(new MultiLineCodeEditor(prompt));
    }

    /// <summary>
    /// Shows a multi-line code editor prompt.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="editor">The multi-line code editor.</param>
    /// <returns>The user input as a string.</returns>
    public static string MultiLineCodeEditor(this IAnsiConsole console, MultiLineCodeEditor editor)
    {
        if (console is null)
        {
            throw new System.ArgumentNullException(nameof(console));
        }

        if (editor is null)
        {
            throw new System.ArgumentNullException(nameof(editor));
        }

        return editor.Show(console);
    }

    /// <summary>
    /// Shows a multi-line code editor prompt asynchronously.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="prompt">The prompt text.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The user input as a string.</returns>
    public static Task<string> MultiLineCodeEditorAsync(this IAnsiConsole console, string prompt, CancellationToken cancellationToken = default)
    {
        return console.MultiLineCodeEditorAsync(new MultiLineCodeEditor(prompt), cancellationToken);
    }

    /// <summary>
    /// Shows a multi-line code editor prompt asynchronously.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="editor">The multi-line code editor.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The user input as a string.</returns>
    public static Task<string> MultiLineCodeEditorAsync(this IAnsiConsole console, MultiLineCodeEditor editor, CancellationToken cancellationToken = default)
    {
        if (console is null)
        {
            throw new System.ArgumentNullException(nameof(console));
        }

        if (editor is null)
        {
            throw new System.ArgumentNullException(nameof(editor));
        }

        return editor.ShowAsync(console, cancellationToken);
    }
}
