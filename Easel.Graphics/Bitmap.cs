using System.IO;
using Easel.Math;
using Pie;
using StbImageSharp;

namespace Easel.Graphics;

public class Bitmap
{
    public readonly byte[] Data;
    public readonly Size<int> Size;
    public readonly Format Format;
    
    public Bitmap(string path)
    {
        using FileStream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size<int>(result.Width, result.Height);
        Format = Format.R8G8B8A8_UNorm;
    }

    public Bitmap(byte[] data, Size<int> size, Format format = Format.R8G8B8A8_UNorm)
    {
        Data = data;
        Size = size;
        Format = format;
    }
}