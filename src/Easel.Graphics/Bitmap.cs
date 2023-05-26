using System.IO;
using Easel.Math;
using Pie;
using StbImageSharp;

namespace Easel.Graphics;

/// <summary>
/// A data structure containing raw image data, its size in pixels, and its format.
/// </summary>
public class Bitmap
{
    public readonly byte[] Data;
    public Size<int> Size;
    public Format Format;

    public Bitmap(string path)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        
        Data = result.Data;
        Size = new Size<int>(result.Width, result.Height);
        Format = Format.R8G8B8A8_UNorm;
    }

    public Bitmap(byte[] data, Size<int> size, Format format)
    {
        Data = data;
        Size = size;
        Format = format;
    }
}