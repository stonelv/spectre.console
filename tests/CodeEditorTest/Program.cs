using Spectre.Console;
using System.Threading;
using System.Threading.Tasks;

// Run functional tests first
CodeEditorTest.FunctionalTest.Run();

System.Console.WriteLine();
System.Console.WriteLine("Testing completed successfully!");
System.Console.WriteLine();
System.Console.WriteLine("To run the interactive editor, use:");
System.Console.WriteLine("  dotnet run --project tests/CodeEditorTest/CodeEditorTest.csproj");
System.Console.WriteLine();
System.Console.WriteLine("Interactive editor features:");
System.Console.WriteLine("  - Standard text input");
System.Console.WriteLine("  - Backspace and Delete keys");
System.Console.WriteLine("  - Ctrl+Z for undo (up to 10 steps)");
System.Console.WriteLine("  - Ctrl+Y for redo");
System.Console.WriteLine("  - Ctrl+C to copy text");
System.Console.WriteLine("  - Ctrl+V to paste text");
System.Console.WriteLine("  - Arrow keys for navigation");
System.Console.WriteLine("  - Home/End keys");
System.Console.WriteLine("  - Escape to exit");
