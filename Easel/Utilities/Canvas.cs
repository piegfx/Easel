using System.Runtime.CompilerServices;
using Easel.Math;
using Pie;

namespace Easel.Utilities;

/// <summary>
/// Software rendered canvas.
/// </summary>
public class Canvas
{
    private byte[] _backBuffer;

    public readonly Size Size;

    public Rectangle Scissor;

    public Canvas(Size size)
    {
        _backBuffer = new byte[size.Width * size.Height * 4];

        Size = size;
        Scissor = new Rectangle(Point.Zero, Size);
    }

    private Canvas(Size size, byte[] data)
    {
        Size = size;
        _backBuffer = data;
    }

    public void Clear(Color color)
    {
        for (int i = 0; i < _backBuffer.Length; i += 4)
        {
            _backBuffer[i + 0] = color.Rb;
            _backBuffer[i + 1] = color.Gb;
            _backBuffer[i + 2] = color.Bb;
            _backBuffer[i + 3] = color.Ab;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawPixel(int x, int y, Color color)
    {
        int pos = (y * Size.Width + x) * 4;
        _backBuffer[pos + 0] = color.Rb;
        _backBuffer[pos + 1] = color.Gb;
        _backBuffer[pos + 2] = color.Bb;
    }

    public void FillRectangle(int x, int y, int width, int height, Color color)
    {
        for (int pY = 0; pY < height; pY++)
        {
            for (int pX = 0; pX < width; pX++)
            {
                DrawPixel(x + pX, y + pY, color);
            }
        }
    }

    public Bitmap ToBitmap() => new Bitmap(Size.Width, Size.Height, PixelFormat.R8G8B8A8_UNorm, _backBuffer);
    
    public static Canvas FromBitmap(Bitmap bitmap)
    {
        return new Canvas(bitmap.Size, bitmap.Data);
    }
}