namespace Spectre.Console;

internal sealed class BrailleCanvas
{
    private readonly bool[,] _pixels;
    private readonly int _width;
    private readonly int _height;

    public int Width => _width;
    public int Height => _height;

    public BrailleCanvas(int width, int height)
    {
        if (width < 1)
        {
            throw new ArgumentException("Width must be at least 1", nameof(width));
        }

        if (height < 1)
        {
            throw new ArgumentException("Height must be at least 1", nameof(height));
        }

        _width = width;
        _height = height;
        _pixels = new bool[width * 2, height * 4];
    }

    public void SetPixel(int x, int y)
    {
        if (x < 0 || x >= _width)
        {
            throw new ArgumentOutOfRangeException(nameof(x), $"X coordinate must be between 0 and {_width - 1}");
        }

        if (y < 0 || y >= _height)
        {
            throw new ArgumentOutOfRangeException(nameof(y), $"Y coordinate must be between 0 and {_height - 1}");
        }

        _pixels[x, y] = true;
    }

    public void SetPixel(int x, int y, bool value)
    {
        if (x < 0 || x >= _width * 2)
        {
            throw new ArgumentOutOfRangeException(nameof(x), $"X coordinate must be between 0 and {_width * 2 - 1}");
        }

        if (y < 0 || y >= _height * 4)
        {
            throw new ArgumentOutOfRangeException(nameof(y), $"Y coordinate must be between 0 and {_height * 4 - 1}");
        }

        _pixels[x, y] = value;
    }

    public void DrawLine(int x0, int y0, int x1, int y1)
    {
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            SetPixel(x0, y0);

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public void DrawLineHighRes(int x0, int y0, int x1, int y1)
    {
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            SetPixel(x0, y0, true);

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public string Render()
    {
        var result = new StringBuilder();
        var charWidth = _width;
        var charHeight = _height;

        for (var charY = 0; charY < charHeight; charY++)
        {
            for (var charX = 0; charX < charWidth; charX++)
            {
                var pixels = new bool[2, 4];
                for (var subX = 0; subX < 2; subX++)
                {
                    for (var subY = 0; subY < 4; subY++)
                    {
                        var pixelX = charX * 2 + subX;
                        var pixelY = charY * 4 + subY;
                        pixels[subX, subY] = _pixels[pixelX, pixelY];
                    }
                }

                result.Append(BraillePatterns.GetBrailleChar(pixels));
            }

            result.AppendLine();
        }

        return result.ToString();
    }

    public void Clear()
    {
        Array.Clear(_pixels, 0, _pixels.Length);
    }

    public bool GetPixel(int x, int y)
    {
        if (x < 0 || x >= _width * 2)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0 || y >= _height * 4)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return _pixels[x, y];
    }
}
