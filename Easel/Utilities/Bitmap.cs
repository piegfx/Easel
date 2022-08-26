using System;
using System.Drawing;
using System.IO;
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
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Size = new Size(result.Width, result.Height);
        Format = PixelFormat.R8G8B8A8_UNorm;
    }
}