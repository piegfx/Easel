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
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size(result.Width, result.Height);
        Format = PixelFormat.R8G8B8A8_UNorm;
    }
}