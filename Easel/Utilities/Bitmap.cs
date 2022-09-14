using System;
using System.IO;
using Easel.Math;
using Pie;
using StbImageSharp;

namespace Easel.Utilities;

/// <summary>
/// A helper class for loading bitmap images, supporting popular image formats such as png, jpg, and bmp.
/// Unlike a <see cref="Graphics.Texture2D"/>, this does not allocate any GPU memory, and is therefore recommended
/// for long-term storage of bitmap images.
/// </summary>
public class Bitmap
{
    /// <summary>
    /// The byte data of this bitmap. Its size is width * height * 4.
    /// </summary>
    public readonly byte[] Data;

    /// <summary>
    /// The size (resolution), in pixels, of this bitmap.
    /// </summary>
    public readonly Size Size;

    /// <summary>
    /// The pixel format of this bitmap.
    /// </summary>
    public readonly PixelFormat Format;

    public Bitmap(string path)
    {
        if (EaselGame.Instance.AllowMissing && !File.Exists(path))
        {
            Data = GetMissingBitmap(128, 128);
            Size = new Size(128, 128);
            Format = PixelFormat.R8G8B8A8_UNorm;
            Logging.Error($"Failed to find path \"{path}\".");
            return;
        }
        else if (!File.Exists(path))
            Logging.Critical($"Failed to find path \"{path}\".");

        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size(result.Width, result.Height);
        Format = PixelFormat.R8G8B8A8_UNorm;
    }

    public Bitmap(byte[] fileData)
    {
        ImageResult result = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size(result.Width, result.Height);
        Format = PixelFormat.R8G8B8A8_UNorm;
    }

    public Bitmap(int width, int height, byte[] data)
    {
        Size = new Size(width, height);
        Data = data;
    }
    
    internal static byte[] GetMissingBitmap(int width, int height)
    {
        byte[] data = new byte[width * height * 4];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int pos = (y * width + x) * 4;
                if ((x < width / 2 && y < height / 2) || (x > width / 2 && y > height / 2))
                {
                    data[pos] = 255;
                    data[pos + 1] = 0;
                    data[pos + 2] = 255;
                }

                data[pos + 3] = 255;
            }
        }

        return data;
    }
}