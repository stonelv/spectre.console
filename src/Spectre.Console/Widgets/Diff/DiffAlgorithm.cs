namespace Spectre.Console;

internal static class DiffAlgorithm
{
    public static List<DiffChange> ComputeDiff(string oldText, string newText)
    {
        var oldLines = oldText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var newLines = newText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        return ComputeDiff(oldLines, newLines);
    }

    public static List<DiffChange> ComputeDiff(string[] oldLines, string[] newLines)
    {
        var lcs = ComputeLCS(oldLines, newLines);
        var result = new List<DiffChange>();

        int oldIndex = 0;
        int newIndex = 0;
        int lcsIndex = 0;

        while (oldIndex < oldLines.Length || newIndex < newLines.Length)
        {
            if (lcsIndex < lcs.Count && oldIndex < oldLines.Length && oldLines[oldIndex] == lcs[lcsIndex])
            {
                if (newIndex < newLines.Length && newLines[newIndex] == lcs[lcsIndex])
                {
                    result.Add(new DiffChange(DiffChangeType.Unchanged, oldLines[oldIndex], oldIndex + 1, newIndex + 1));
                    oldIndex++;
                    newIndex++;
                    lcsIndex++;
                }
                else
                {
                    result.Add(new DiffChange(DiffChangeType.Inserted, newLines[newIndex], null, newIndex + 1));
                    newIndex++;
                }
            }
            else if (lcsIndex < lcs.Count && newIndex < newLines.Length && newLines[newIndex] == lcs[lcsIndex])
            {
                result.Add(new DiffChange(DiffChangeType.Deleted, oldLines[oldIndex], oldIndex + 1, null));
                oldIndex++;
            }
            else
            {
                if (oldIndex < oldLines.Length)
                {
                    result.Add(new DiffChange(DiffChangeType.Deleted, oldLines[oldIndex], oldIndex + 1, null));
                    oldIndex++;
                }

                if (newIndex < newLines.Length)
                {
                    result.Add(new DiffChange(DiffChangeType.Inserted, newLines[newIndex], null, newIndex + 1));
                    newIndex++;
                }
            }
        }

        return result;
    }

    private static List<string> ComputeLCS(string[] oldLines, string[] newLines)
    {
        var m = oldLines.Length;
        var n = newLines.Length;

        var dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (oldLines[i - 1] == newLines[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        var lcs = new List<string>();
        int x = m, y = n;

        while (x > 0 && y > 0)
        {
            if (oldLines[x - 1] == newLines[y - 1])
            {
                lcs.Add(oldLines[x - 1]);
                x--;
                y--;
            }
            else if (dp[x - 1, y] > dp[x, y - 1])
            {
                x--;
            }
            else
            {
                y--;
            }
        }

        lcs.Reverse();
        return lcs;
    }
}
