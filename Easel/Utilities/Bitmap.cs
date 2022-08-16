using System;
using System.Drawing;
using System.IO;
using Pie;
using StbImageSharp;

namespace Easel.Utilities;

public class Bitmap
{
    public readonly byte[] Data;

    public readonly Size Size;

    public readonly PixelFormat Format;

    public Bitmap(string path)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path));
        Data = result.Data;
        Size = new Size(result.Width, result.Height);
        Format = result.Comp switch
        {
            ColorComponents.RedGreenBlue => PixelFormat.RGB8,
            ColorComponents.RedGreenBlueAlpha => PixelFormat.RGBA8,
            _ => throw new NotSupportedException("The given image uses an unsupported pixel format.")
        };
    }
}