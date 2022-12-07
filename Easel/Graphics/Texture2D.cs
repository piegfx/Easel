using System;
using Easel.Utilities;
using Pie;

namespace Easel.Graphics;

/// <summary>
/// <see cref="Texture2D"/>s are used to texture most 3D meshes, and 2D sprites.
/// </summary>
public class Texture2D : Texture
{
    /// <summary>
    /// Create a new <see cref="Texture2D"/> from the given path.
    /// </summary>
    /// <param name="path">The path to load from.</param>
    /// <param name="autoDispose">If <see langword="true"/>, this <see cref="Texture2D"/> will be automatically disposed
    /// on scene change.</param>
    public Texture2D(string path, bool autoDispose = true) : this(new Bitmap(path), autoDispose)
    {
        
    }

    /// <summary>
    /// Create a new <see cref="Texture2D"/> from the given <see cref="Bitmap"/>. Useful for doing threaded loading.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to load from.</param>
    /// <param name="autoDispose">If <see langword="true"/>, this <see cref="Texture2D"/> will be automatically disposed
    /// on scene change.</param>
    public Texture2D(Bitmap bitmap, bool autoDispose = true) : this(bitmap.Size.Width,
        bitmap.Size.Height, bitmap.Data, bitmap.Format, autoDispose)
    {
        
    }

    public Texture2D(int width, int height, byte[] data, PixelFormat format = PixelFormat.R8G8B8A8_UNorm, bool autoDispose = true) : base(autoDispose)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        TextureDescription description =
            new TextureDescription(TextureType.Texture2D, width, height, format, 0, 1, TextureUsage.ShaderResource);
        PieTexture = device.CreateTexture(description, new [] { new TextureData(data) });
        device.GenerateMipmaps(PieTexture);
    }

    public void SetData(int x, int y, int width, int height, byte[] data)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        device.UpdateTexture(PieTexture, x, y, (uint) width, (uint) height, new TextureData(data));
    }

    public static readonly Texture2D Blank = new Texture2D(1, 1, new byte[] { 255, 255, 255, 255 }, autoDispose: false);

    public static readonly Texture2D Void = new Texture2D(1, 1, new byte[] { 0, 0, 0, 255 }, autoDispose: false);

    public static readonly Texture2D Missing = new Texture2D(128, 128, Bitmap.GetMissingBitmap(128, 128), autoDispose: false);
}