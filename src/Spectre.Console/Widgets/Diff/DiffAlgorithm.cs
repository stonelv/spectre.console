using System.Diagnostics;

namespace Spectre.Console;

/// <summary>
/// Implements the diff algorithm for comparing two texts.
/// Uses the Longest Common Subsequence (LCS) algorithm.
/// </summary>
internal static class DiffAlgorithm
{
    /// <summary>
    /// Computes the diff between two texts.
    /// </summary>
    /// <param name="oldText">The old text.</param>
    /// <param name="newText">The new text.</param>
    /// <returns>The diff result.</returns>
    public static DiffResult Compute(string oldText, string newText)
    {
        if (oldText == null)
        {
            throw new ArgumentNullException(nameof(oldText));
        }

        if (newText == null)
        {
            throw new ArgumentNullException(nameof(newText));
        }

        var oldLines = oldText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var newLines = newText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        return Compute(oldLines, newLines);
    }

    /// <summary>
    /// Computes the diff between two line arrays.
    /// </summary>
    /// <param name="oldLines">The old lines.</param>
    /// <param name="newLines">The new lines.</param>
    /// <returns>The diff result.</returns>
    public static DiffResult Compute(string[] oldLines, string[] newLines)
    {
        if (oldLines == null)
        {
            throw new ArgumentNullException(nameof(oldLines));
        }

        if (newLines == null)
        {
            throw new ArgumentNullException(nameof(newLines));
        }

        var lcs = ComputeLCS(oldLines, newLines);
        var result = BuildDiff(oldLines, newLines, lcs);

        return new DiffResult(result);
    }

    /// <summary>
    /// Computes the Longest Common Subsequence using dynamic programming.
    /// </summary>
    private static int[,] ComputeLCS(string[] oldLines, string[] newLines)
    {
        var m = oldLines.Length;
        var n = newLines.Length;
        var lcs = new int[m + 1, n + 1];

        for (var i = 1; i <= m; i++)
        {
            for (var j = 1; j <= n; j++)
            {
                if (string.Equals(oldLines[i - 1], newLines[j - 1], StringComparison.Ordinal))
                {
                    lcs[i, j] = lcs[i - 1, j - 1] + 1;
                }
                else
                {
                    lcs[i, j] = Math.Max(lcs[i - 1, j], lcs[i, j - 1]);
                }
            }
        }

        return lcs;
    }

    /// <summary>
    /// Builds the diff result by backtracking through the LCS matrix.
    /// </summary>
    private static List<DiffLine> BuildDiff(string[] oldLines, string[] newLines, int[,] lcs)
    {
        var result = new List<DiffLine>();
        var i = oldLines.Length;
        var j = newLines.Length;

        // Stack to reverse the order (we build from end to start)
        var stack = new Stack<DiffLine>();

        while (i > 0 || j > 0)
        {
            if (i > 0 && j > 0 && string.Equals(oldLines[i - 1], newLines[j - 1], StringComparison.Ordinal))
            {
                // Lines are equal - unchanged
                stack.Push(new DiffLine(DiffLineType.Unchanged, oldLines[i - 1], i, j));
                i--;
                j--;
            }
            else if (j > 0 && (i == 0 || lcs[i, j - 1] >= lcs[i - 1, j]))
            {
                // Line was inserted
                stack.Push(new DiffLine(DiffLineType.Inserted, newLines[j - 1], null, j));
                j--;
            }
            else if (i > 0 && (j == 0 || lcs[i, j - 1] < lcs[i - 1, j]))
            {
                // Line was deleted
                stack.Push(new DiffLine(DiffLineType.Deleted, oldLines[i - 1], i, null));
                i--;
            }
        }

        // Unstack to get correct order
        while (stack.Count > 0)
        {
            result.Add(stack.Pop());
        }

        return result;
    }
}
