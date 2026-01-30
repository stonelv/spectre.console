using System;
using Spectre.Console;
using Spectre.Console.Widgets;

namespace Generator.Commands.Samples;

public class DiffSample : BaseSample
{
    public override void Run(IAnsiConsole console)
    {
        console.Write(new Rule("Diff Example (Inline)").LeftJustified());
        console.WriteLine();
        
        var oldText = "Line 1\nLine 2\nLine 3\nLine 4";
        var newText = "Line 1\nModified Line 2\nLine 3\nLine 5";
        
        // Inline diff
        var inlineDiff = new Diff(oldText, newText, DiffMode.Inline);
        console.Write(inlineDiff);
        
        console.WriteLine();
        console.Write(new Rule("Diff Example (Side by Side)").LeftJustified());
        console.WriteLine();
        
        // Side by side diff
        var sideBySideDiff = new Diff(oldText, newText, DiffMode.SideBySide);
        console.Write(sideBySideDiff);
        
        console.WriteLine();
        console.Write(new Rule("Diff Example (Custom Styles)").LeftJustified());
        console.WriteLine();
        
        // Custom styles
        var customDiff = new Diff(oldText, newText, DiffMode.Inline)
        {
            UnchangedStyle = new Style(Color.Blue),
            AddedStyle = new Style(Color.Green, null, Decoration.Bold),
            DeletedStyle = new Style(Color.Red, null, Decoration.Italic)
        };
        console.Write(customDiff);
    }
}