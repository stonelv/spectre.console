using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        AnsiConsole.MarkupLine("[bold yellow]Spectre.Console Braille Chart Demo[/]");
        AnsiConsole.WriteLine();

        SimpleSineWave();
        SimpleExponential();
        SimpleLinear();
    }

    static void SimpleSineWave()
    {
        AnsiConsole.MarkupLine("[bold cyan]Example 1: Sine Wave[/]");
        AnsiConsole.WriteLine();

        var chart = new BrailleChart()
            .SetSize(60, 15)
            .SetColor(Color.Cyan1)
            .SetXAxisTickCount(6)
            .SetYAxisTickCount(5);

        for (var x = 0; x <= 20; x++)
        {
            var radians = x * Math.PI / 10;
            var y = Math.Sin(radians);
            chart.AddPoint(x, y);
        }

        AnsiConsole.Write(chart);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    static void SimpleExponential()
    {
        AnsiConsole.MarkupLine("[bold green]Example 2: Exponential Growth[/]");
        AnsiConsole.WriteLine();

        var chart = new BrailleChart()
            .SetSize(60, 15)
            .SetColor(Color.Green1)
            .SetXAxisTickCount(6)
            .SetYAxisTickCount(6);

        for (var x = 0; x <= 10; x++)
        {
            var y = Math.Pow(1.5, x);
            chart.AddPoint(x, y);
        }

        AnsiConsole.Write(chart);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    static void SimpleLinear()
    {
        AnsiConsole.MarkupLine("[bold magenta]Example 3: Linear Function[/]");
        AnsiConsole.WriteLine();

        var chart = new BrailleChart()
            .SetSize(60, 15)
            .SetColor(Color.Magenta)
            .SetXAxisTickCount(6)
            .SetYAxisTickCount(5);

        for (var x = 0; x <= 10; x++)
        {
            var y = x * 0.8;
            chart.AddPoint(x, y);
        }

        AnsiConsole.Write(chart);
        AnsiConsole.WriteLine();
    }
}
