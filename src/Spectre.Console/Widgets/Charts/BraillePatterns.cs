namespace Spectre.Console;

internal static class BraillePatterns
{
    private static readonly char[] BrailleChars = new char[256];

    static BraillePatterns()
    {
        for (var i = 0; i < 256; i++)
        {
            BrailleChars[i] = (char)(0x2800 + i);
        }
    }

    public static char GetBrailleChar(bool[,] pixels)
    {
        if (pixels.GetLength(0) != 2 || pixels.GetLength(1) != 4)
        {
            throw new ArgumentException("Pixels must be 2x4 array", nameof(pixels));
        }

        var index = 0;
        if (pixels[0, 0]) index |= 1 << 0;
        if (pixels[1, 0]) index |= 1 << 3;
        if (pixels[0, 1]) index |= 1 << 1;
        if (pixels[1, 1]) index |= 1 << 4;
        if (pixels[0, 2]) index |= 1 << 2;
        if (pixels[1, 2]) index |= 1 << 5;
        if (pixels[0, 3]) index |= 1 << 6;
        if (pixels[1, 3]) index |= 1 << 7;

        return BrailleChars[index];
    }

    public static char GetBrailleChar(int index)
    {
        if (index < 0 || index > 255)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 255");
        }

        return BrailleChars[index];
    }
}
