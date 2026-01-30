using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Rendering;

namespace Spectre.Console.Internal;

internal enum DiffChangeType
{
    Unchanged,
    Added,
    Deleted
}

internal sealed class DiffChange
{
    public DiffChangeType ChangeType { get; }
    public string Content { get; }

    public DiffChange(DiffChangeType changeType, string content)
    {
        ChangeType = changeType;
        Content = content;
    }
}

internal static class DiffAlgorithm
{
    public static List<DiffChange> ComputeDiff(string oldText, string newText)
    {
        var oldLines = oldText.Split('\n');
        var newLines = newText.Split('\n');
        
        var lcs = ComputeLCS(oldLines, newLines);
        var result = new List<DiffChange>();
        
        int oldIndex = 0;
        int newIndex = 0;
        
        foreach (var line in lcs)
        {
            // Add deleted lines from old text
            while (oldIndex < oldLines.Length && oldLines[oldIndex] != line)
            {
                result.Add(new DiffChange(DiffChangeType.Deleted, oldLines[oldIndex]));
                oldIndex++;
            }
            
            // Add added lines from new text
            while (newIndex < newLines.Length && newLines[newIndex] != line)
            {
                result.Add(new DiffChange(DiffChangeType.Added, newLines[newIndex]));
                newIndex++;
            }
            
            // Add the unchanged line
            result.Add(new DiffChange(DiffChangeType.Unchanged, line));
            oldIndex++;
            newIndex++;
        }
        
        // Add remaining deleted lines
        while (oldIndex < oldLines.Length)
        {
            result.Add(new DiffChange(DiffChangeType.Deleted, oldLines[oldIndex]));
            oldIndex++;
        }
        
        // Add remaining added lines
        while (newIndex < newLines.Length)
        {
            result.Add(new DiffChange(DiffChangeType.Added, newLines[newIndex]));
            newIndex++;
        }
        
        return result;
    }
    
    private static List<string> ComputeLCS(string[] a, string[] b)
    {
        var lengths = new int[a.Length + 1, b.Length + 1];
        
        // Fill the lengths matrix
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                if (a[i] == b[j])
                {
                    lengths[i + 1, j + 1] = lengths[i, j] + 1;
                }
                else
                {
                    lengths[i + 1, j + 1] = Math.Max(lengths[i + 1, j], lengths[i, j + 1]);
                }
            }
        }
        
        // Backtrack to find the LCS
        var result = new List<string>();
        int x = a.Length;
        int y = b.Length;
        
        while (x > 0 && y > 0)
        {
            if (a[x - 1] == b[y - 1])
            {
                result.Insert(0, a[x - 1]);
                x--;
                y--;
            }
            else if (lengths[x, y - 1] > lengths[x - 1, y])
            {
                y--;
            }
            else
            {
                x--;
            }
        }
        
        return result;
    }
}