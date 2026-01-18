using System.Collections.Generic;
using System;
using Spectre.Console;

namespace Generator.Commands.Samples;

internal class BraillePlotSample : BaseSample
{
    public override (int Cols, int Rows) ConsoleSize => (base.ConsoleSize.Cols, 30);

    public override void Run(IAnsiConsole console)
    {
        console.WriteLine("Braille Plot Samples");
        console.WriteLine(new string('=', 60));

        var samples = new List<(string Name, BraillePlot Plot)>()
        {
            (
                "Sine Wave",
                new BraillePlotBuilder()
                    .Width(60)
                    .Height(10)
                    .Title("Sine Wave")
                    .AddSineWave()
                    .Build()
            ),
            (
                "Cosine Wave",
                new BraillePlotBuilder()
                    .Width(60)
                    .Height(10)
                    .Title("Cosine Wave")
                    .AddCosineWave()
                    .Build()
            ),
            (
                "Sine + Cosine",
                new BraillePlotBuilder()
                    .Width(60)
                    .Height(10)
                    .Title("Sine + Cosine")
                    .AddSineWave()
                    .AddCosineWave()
                    .Build()
            ),
            (
                "Sawtooth Wave",
                new BraillePlotBuilder()
                    .Width(60)
                    .Height(10)
                    .Title("Sawtooth Wave")
                    .AddSawtoothWave()
                    .Build()
            ),
            (
                "Heart Curve",
                new BraillePlot()
                    .Width(60)
                    .Height(10)
                    .Title("Heart Curve (x² + y² - 1)³ - x²y³ = 0")
                    .LineColor(Color.Red)
            ),
            (
                "Random Walk",
                new BraillePlot()
                    .Width(60)
                    .Height(10)
                    .Title("Random Walk")
                    .LineColor(Color.Green)
            ),
        };

        var heartPlot = samples[4].Plot;
        for (var t = 0; t < 100; t++)
        {
            var theta = t * 2 * Math.PI / 100;
            var xh = 16 * Math.Pow(Math.Sin(theta), 3);
            var yh = 13 * Math.Cos(theta) - 5 * Math.Cos(2 * theta) - 2 * Math.Cos(3 * theta) - Math.Cos(4 * theta);
            heartPlot.AddPoint(xh, yh);
        }

        var random = new Random(42);
        double x = 0;
        double y = 0;
        var randomWalkPlot = samples[5].Plot;
        for (var i = 0; i < 500; i++)
        {
            randomWalkPlot.AddPoint(x, y);
            x += 0.1 * (random.NextDouble() - 0.5);
            y += 0.1 * (random.NextDouble() - 0.5);
        }

        foreach (var (name, plot) in samples)
        {
            console.WriteLine();
            console.WriteLine($"[bold]{name}[/]");
            console.WriteLine();
            console.Write(plot);
            console.WriteLine();
            console.WriteLine(new string('-', 60));
        }
    }
}
