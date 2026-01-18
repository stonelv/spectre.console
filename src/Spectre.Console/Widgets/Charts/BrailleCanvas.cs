using System.Text;

namespace Spectre.Console;

public sealed class BrailleCanvas
{
    private readonly bool[,] _dots;
    
    public int Width { get; }
    public int Height { get; }
    
    private const int DOTS_PER_CHAR_HORIZONTAL = 2;
    private const int DOTS_PER_CHAR_VERTICAL = 4;
    private const int UNICODE_BRAILLE_OFFSET = 0x2800;
    
    private static readonly int[,] DOT_POSITIONS = {
        { 0, 1 },  // Top-left, Top-right
        { 2, 3 },  // Middle-left, Middle-right
        { 4, 5 },  // Bottom-left, Bottom-right
        { 6, 7 },  // Extra-bottom-left, Extra-bottom-right
    };

    public BrailleCanvas(int width, int height)
    {
        if (width < 1)
            throw new ArgumentException("Width must be greater than 0", nameof(width));
        if (height < 1)
            throw new ArgumentException("Height must be greater than 0", nameof(height));

        Width = width;
        Height = height;
        _dots = new bool[Width, Height];
    }

    public void SetPixel(int x, int y, Color color = default)
    {
        if (x < 0 || x >= Width)
            return;
        if (y < 0 || y >= Height)
            return;

        _dots[x, y] = true;
    }

    public void Clear()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _dots[x, y] = false;
            }
        }
    }

    public bool GetPixel(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;
        return _dots[x, y];
    }

    public IEnumerable<string> Render()
    {
        int charsPerLine = (Width + DOTS_PER_CHAR_HORIZONTAL - 1) / DOTS_PER_CHAR_HORIZONTAL;
        int charLines = (Height + DOTS_PER_CHAR_VERTICAL - 1) / DOTS_PER_CHAR_VERTICAL;

        for (int charY = 0; charY < charLines; charY++)
        {
            var line = new StringBuilder();
            
            for (int charX = 0; charX < charsPerLine; charX++)
            {
                char brailleChar = GetBrailleChar(charX, charY);
                line.Append(brailleChar);
            }
            
            yield return line.ToString();
        }
    }

    private char GetBrailleChar(int charX, int charY)
    {
        int dotValue = 0;

        for (int subY = 0; subY < DOTS_PER_CHAR_VERTICAL; subY++)
        {
            for (int subX = 0; subX < DOTS_PER_CHAR_HORIZONTAL; subX++)
            {
                int x = charX * DOTS_PER_CHAR_HORIZONTAL + subX;
                int y = charY * DOTS_PER_CHAR_VERTICAL + subY;

                if (x < Width && y < Height && _dots[x, y])
                {
                    int bitPosition = DOT_POSITIONS[subY, subX];
                    dotValue |= (1 << bitPosition);
                }
            }
        }

        return (char)(UNICODE_BRAILLE_OFFSET + dotValue);
    }

    public void DrawLine(int x1, int y1, int x2, int y2)
    {
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        int x = x1;
        int y = y1;

        while (true)
        {
            SetPixel(x, y);

            if (x == x2 && y == y2)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

    public void DrawCircle(int cx, int cy, int radius)
    {
        int x = radius;
        int y = 0;
        int err = 0;

        while (x >= y)
        {
            SetPixel(cx + x, cy + y);
            SetPixel(cx + y, cy + x);
            SetPixel(cx - y, cy + x);
            SetPixel(cx - x, cy + y);
            SetPixel(cx - x, cy - y);
            SetPixel(cx - y, cy - x);
            SetPixel(cx + y, cy - x);
            SetPixel(cx + x, cy - y);

            y++;
            if (err <= 0)
            {
                err += 2 * y + 1;
            }
            if (err > 0)
            {
                x--;
                err -= 2 * x + 1;
            }
        }
    }

    public void FillRectangle(int x, int y, int width, int height)
    {
        for (int dy = 0; dy < height; dy++)
        {
            for (int dx = 0; dx < width; dx++)
            {
                SetPixel(x + dx, y + dy);
            }
        }
    }
}
