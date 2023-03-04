/*using System.IO;
using Easel.Core;
using Easel.Math;
using Pie;
using StbImageSharp;

namespace Easel.Graphics;

public struct BitmapF
{
    /// <summary>
    /// The byte data of this bitmap. Its size is width * height * 4.
    /// </summary>
    public readonly float[] Data;

    /// <summary>
    /// The size (resolution), in pixels, of this bitmap.
    /// </summary>
    public readonly Size<int> Size;

    /// <summary>
    /// The pixel format of this bitmap.
    /// </summary>
    public readonly Format Format;

    public BitmapF(string path)
    {
        if (!File.Exists(path))
            Logger.Fatal($"Failed to find path \"{path}\".");

        ImageResultFloat result = ImageResultFloat.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size<int>(result.Width, result.Height);
        Format = Format.R8G8B8A8_UNorm;
    }

    public BitmapF(byte[] fileData)
    {
        ImageResult result = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size<int>(result.Width, result.Height);
        Format = Format.R8G8B8A8_UNorm;
    }

    public BitmapF(int width, int height, Format format, byte[] data)
    {
        Size = new Size<int>(width, height);
        Format = format;
        Data = data;
    }
}*/