namespace Spectre.Console;

internal static class DiffAlgorithm
{
    public static List<DiffLine> ComputeDiff(string oldText, string newText)
    {
        var oldLines = SplitLines(oldText);
        var newLines = SplitLines(newText);

        return ComputeLcsDiff(oldLines, newLines);
    }

    private static List<string> SplitLines(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new List<string>();
        }

        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
        if (text.EndsWith("\r\n") || text.EndsWith("\n"))
        {
            lines.Add(string.Empty);
        }

        return lines;
    }

    private static List<DiffLine> ComputeLcsDiff(List<string> oldLines, List<string> newLines)
    {
        int n = oldLines.Count;
        int m = newLines.Count;

        var dp = new int[n + 1, m + 1];

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
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

        var result = new List<DiffLine>();
        int x = n;
        int y = m;

        while (x > 0 || y > 0)
        {
            if (x > 0 && y > 0 && oldLines[x - 1] == newLines[y - 1])
            {
                result.Insert(0, new DiffLine(DiffLineType.Unchanged, oldLines[x - 1]));
                x--;
                y--;
            }
            else if (y > 0 && (x == 0 || dp[x, y - 1] >= dp[x - 1, y]))
            {
                result.Insert(0, new DiffLine(DiffLineType.Added, newLines[y - 1]));
                y--;
            }
            else if (x > 0)
            {
                result.Insert(0, new DiffLine(DiffLineType.Deleted, oldLines[x - 1]));
                x--;
            }
        }

        return result;
    }
}
