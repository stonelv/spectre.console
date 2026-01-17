using System.Collections.Generic;
using System.Linq;

namespace Spectre.Console;

/// <summary>
/// Implements the Longest Common Subsequence (LCS) algorithm for diff comparison.
/// </summary>
internal static class DiffAlgorithm
{
    /// <summary>
    /// Compares two strings and returns the differences.
    /// </summary>
    /// <param name="original">The original string.</param>
    /// <param name="modified">The modified string.</param>
    /// <returns>A list of <see cref="DiffLine"/> objects representing the differences.</returns>
    public static List<DiffLine> Compare(string original, string modified)
    {
        var originalLines = original.Split(new[] { '\n' }, StringSplitOptions.None);
        var modifiedLines = modified.Split(new[] { '\n' }, StringSplitOptions.None);

        return Compare(originalLines, modifiedLines);
    }

    /// <summary>
    /// Compares two string arrays and returns the differences.
    /// </summary>
    /// <param name="originalLines">The original lines.</param>
    /// <param name="modifiedLines">The modified lines.</param>
    /// <returns>A list of <see cref="DiffLine"/> objects representing the differences.</returns>
    public static List<DiffLine> Compare(string[] originalLines, string[] modifiedLines)
    {
        var result = new List<DiffLine>();

        if (originalLines == null || originalLines.Length == 0)
        {
            // All modified lines are added
            for (var i = 0; i < modifiedLines.Length; i++)
            {
                result.Add(new DiffLine(DiffChangeType.Added, null, modifiedLines[i])
                {
                    OriginalLineNumber = 0,
                    ModifiedLineNumber = i + 1,
                });
            }
            return result;
        }

        if (modifiedLines == null || modifiedLines.Length == 0)
        {
            // All original lines are removed
            for (var i = 0; i < originalLines.Length; i++)
            {
                result.Add(new DiffLine(DiffChangeType.Removed, originalLines[i], null)
                {
                    OriginalLineNumber = i + 1,
                    ModifiedLineNumber = 0,
                });
            }
            return result;
        }

        var m = originalLines.Length;
        var n = modifiedLines.Length;

        // Build LCS table
        var lcsTable = BuildLCSTable(originalLines, modifiedLines);

        // Backtrack to find differences
        var originalIndex = m - 1;
        var modifiedIndex = n - 1;

        while (originalIndex >= 0 || modifiedIndex >= 0)
        {
            if (originalIndex >= 0 && modifiedIndex >= 0 && originalLines[originalIndex] == modifiedLines[modifiedIndex])
            {
                // Unchanged line
                result.Insert(0, new DiffLine(DiffChangeType.Unchanged, originalLines[originalIndex], modifiedLines[modifiedIndex])
                {
                    OriginalLineNumber = originalIndex + 1,
                    ModifiedLineNumber = modifiedIndex + 1,
                });
                originalIndex--;
                modifiedIndex--;
            }
            else if (modifiedIndex >= 0 && (originalIndex < 0 || lcsTable[originalIndex + 1][modifiedIndex] >= lcsTable[originalIndex + 1][modifiedIndex + 1]))
            {
                // Added line
                result.Insert(0, new DiffLine(DiffChangeType.Added, null, modifiedLines[modifiedIndex])
                {
                    OriginalLineNumber = 0,
                    ModifiedLineNumber = modifiedIndex + 1,
                });
                modifiedIndex--;
            }
            else if (originalIndex >= 0 && (modifiedIndex < 0 || lcsTable[originalIndex][modifiedIndex + 1] >= lcsTable[originalIndex + 1][modifiedIndex + 1]))
            {
                // Removed line
                result.Insert(0, new DiffLine(DiffChangeType.Removed, originalLines[originalIndex], null)
                {
                    OriginalLineNumber = originalIndex + 1,
                    ModifiedLineNumber = 0,
                });
                originalIndex--;
            }
            else
            {
                // Modified line
                result.Insert(0, new DiffLine(DiffChangeType.Modified, originalLines[originalIndex], modifiedLines[modifiedIndex])
                {
                    OriginalLineNumber = originalIndex + 1,
                    ModifiedLineNumber = modifiedIndex + 1,
                });
                originalIndex--;
                modifiedIndex--;
            }
        }

        return result;
    }

    private static int[][] BuildLCSTable(string[] originalLines, string[] modifiedLines)
    {
        var m = originalLines.Length;
        var n = modifiedLines.Length;

        var lcsTable = new int[m + 1][];
        for (var i = 0; i <= m; i++)
        {
            lcsTable[i] = new int[n + 1];
        }

        for (var i = m - 1; i >= 0; i--)
        {
            for (var j = n - 1; j >= 0; j--)
            {
                if (originalLines[i] == modifiedLines[j])
                {
                    lcsTable[i][j] = lcsTable[i + 1][j + 1] + 1;
                }
                else
                {
                    lcsTable[i][j] = Math.Max(lcsTable[i + 1][j], lcsTable[i][j + 1]);
                }
            }
        }

        return lcsTable;
    }
}
