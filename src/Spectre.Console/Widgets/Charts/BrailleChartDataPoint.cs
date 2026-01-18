namespace Spectre.Console;

public sealed class BrailleChartDataPoint : IBrailleChartDataPoint
{
    public double X { get; }
    public double Y { get; }

    public BrailleChartDataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }
}
