using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrailleChartTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Braille Chart Data Visualization Demo");
            Console.WriteLine("====================================");
            Console.WriteLine();

            TestSineWave();
            TestMultipleFunctions();
            TestDataScatter();
            TestMathematicalFunctions();

            Console.WriteLine("\nAll tests completed!");
        }

        static void TestSineWave()
        {
            AnsiConsole.MarkupLine("[cyan]Test 1: Sine Wave[/]");
            
            var chart = new BrailleChart
            {
                Title = "Sine Wave Demo",
                Width = 160,
                Height = 40,
            };

            var data = new List<(double X, double Y)>();
            for (double x = 0; x < Math.PI * 4; x += 0.1)
            {
                data.Add((x, Math.Sin(x)));
            }

            chart.AddSeries("sin(x)", Color.Green, data);

            AnsiConsole.Write(chart);
            AnsiConsole.WriteLine();
        }

        static void TestMultipleFunctions()
        {
            AnsiConsole.MarkupLine("[cyan]Test 2: Multiple Functions[/]");

            var chart = new BrailleChart
            {
                Title = "Trigonometric Functions",
                Width = 160,
                Height = 40,
            };

            var sineData = new List<(double X, double Y)>();
            var cosineData = new List<(double X, double Y)>();
            var tangentData = new List<(double X, double Y)>();

            for (double x = 0; x < Math.PI * 2; x += 0.05)
            {
                sineData.Add((x, Math.Sin(x)));
                cosineData.Add((x, Math.Cos(x)));
                if (Math.Abs(Math.Cos(x)) > 0.1)
                {
                    tangentData.Add((x, Math.Tan(x)));
                }
            }

            chart.AddSeries("sin(x)", Color.Green, sineData);
            chart.AddSeries("cos(x)", Color.Blue, cosineData);
            chart.AddSeries("tan(x)", Color.Red, tangentData);

            AnsiConsole.Write(chart);
            AnsiConsole.WriteLine();
        }

        static void TestDataScatter()
        {
            AnsiConsole.MarkupLine("[cyan]Test 3: Scatter Plot[/]");

            var chart = new BrailleChart
            {
                Title = "Random Data Points",
                Width = 160,
                Height = 40,
                ShowGrid = true,
            };

            var rnd = new Random(42);
            var data = new List<(double X, double Y)>();

            for (int i = 0; i < 100; i++)
            {
                double x = rnd.NextDouble() * 100;
                double y = rnd.NextDouble() * 100;
                data.Add((x, y));
            }

            chart.AddSeries("Random Points", Color.Yellow, data);

            AnsiConsole.Write(chart);
            AnsiConsole.WriteLine();
        }

        static void TestMathematicalFunctions()
        {
            AnsiConsole.MarkupLine("[cyan]Test 4: Mathematical Functions[/]");

            var chart = new BrailleChart
            {
                Title = "Complex Functions",
                Width = 160,
                Height = 40,
            };

            var expData = new List<(double X, double Y)>();
            var logData = new List<(double X, double Y)>();
            var parabolaData = new List<(double X, double Y)>();

            for (double x = 0.1; x < 5; x += 0.05)
            {
                expData.Add((x, Math.Exp(-x) * Math.Cos(x * Math.PI * 2)));
                logData.Add((x, Math.Log(x)));
            }

            for (double x = -5; x < 5; x += 0.1)
            {
                parabolaData.Add((x, -0.1 * x * x + 2));
            }

            chart.AddSeries("exp(-x)cos(2πx)", Color.Green, expData);
            chart.AddSeries("log(x)", Color.Blue, logData);
            chart.AddSeries("-0.1x² + 2", Color.Red, parabolaData);

            AnsiConsole.Write(chart);
            AnsiConsole.WriteLine();
        }

        static void TestPerformanceData()
        {
            AnsiConsole.MarkupLine("[cyan]Test 5: Performance Metrics[/]");

            var chart = new BrailleChart
            {
                Title = "System Performance",
                Width = 160,
                Height = 40,
            };

            var cpuData = new List<(double X, double Y)>();
            var memoryData = new List<(double X, double Y)>();
            var networkData = new List<(double X, double Y)>();

            var rnd = new Random(123);
            double cpu = 50;
            double memory = 60;
            double network = 30;

            for (int i = 0; i < 100; i++)
            {
                cpu = Math.Clamp(cpu + (rnd.NextDouble() - 0.5) * 10, 10, 90);
                memory = Math.Clamp(memory + (rnd.NextDouble() - 0.5) * 5, 40, 95);
                network = Math.Clamp(network + (rnd.NextDouble() - 0.5) * 20, 0, 100);

                cpuData.Add((i, cpu));
                memoryData.Add((i, memory));
                networkData.Add((i, network));
            }

            chart.AddSeries("CPU %", Color.Red, cpuData);
            chart.AddSeries("Memory %", Color.Yellow, memoryData);
            chart.AddSeries("Network %", Color.Green, networkData);

            AnsiConsole.Write(chart);
            AnsiConsole.WriteLine();
        }
    }
}
