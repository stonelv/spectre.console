using System;

class DiffDemo
{
    static void Main()
    {
        var oldText = @"Hello World
This is the original text
Line 3
Line 4
Line 5
Some important content
End of file";

        var newText = @"Hello World
This is the modified text
Line 3
A new line inserted here
Line 5
Some updated content
Another new line
End of file";

        Console.WriteLine("=== Original Text (old.txt) ===");
        Console.WriteLine(oldText);
        Console.WriteLine();

        Console.WriteLine("=== Modified Text (new.txt) ===");
        Console.WriteLine(newText);
        Console.WriteLine();

        Console.WriteLine("=== Expected Diff Output (Inline Mode) ===");
        Console.WriteLine("  Hello World");
        Console.WriteLine("- This is the original text");
        Console.WriteLine("+ This is the modified text");
        Console.WriteLine("  Line 3");
        Console.WriteLine("+ A new line inserted here");
        Console.WriteLine("- Line 4");
        Console.WriteLine("  Line 5");
        Console.WriteLine("- Some important content");
        Console.WriteLine("+ Some updated content");
        Console.WriteLine("+ Another new line");
        Console.WriteLine("  End of file");
        Console.WriteLine();

        Console.WriteLine("=== Expected Diff Output (Side-by-Side Mode) ===");
        Console.WriteLine("1 Hello World              1 Hello World");
        Console.WriteLine("2 This is the original text 2 This is the modified text");
        Console.WriteLine("3 Line 3                    3 Line 3");
        Console.WriteLine("4 Line 4                    4 A new line inserted here");
        Console.WriteLine("5 Line 5                    5 Line 5");
        Console.WriteLine("6 Some important content    6 Some updated content");
        Console.WriteLine("                           7 Another new line");
        Console.WriteLine("7 End of file               8 End of file");
    }
}
