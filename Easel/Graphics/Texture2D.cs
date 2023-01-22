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
    public Texture2D(string path, SamplerState samplerState = null, bool autoDispose = true)
        : this(new Bitmap(path), samplerState, autoDispose) { }

    /// <summary>
    /// Create a new <see cref="Texture2D"/> from the given <see cref="Bitmap"/>. Useful for doing threaded loading.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to load from.</param>
    /// <param name="autoDispose">If <see langword="true"/>, this <see cref="Texture2D"/> will be automatically disposed
    /// on scene change.</param>
    public Texture2D(Bitmap bitmap, SamplerState samplerState = null, bool autoDispose = true)
        : this(bitmap.Size.Width, bitmap.Size.Height, bitmap.Data, samplerState, bitmap.Format, autoDispose) { }

    public Texture2D(int width, int height, byte[] data, SamplerState samplerState = null, 
        PixelFormat format = PixelFormat.R8G8B8A8_UNorm, bool autoDispose = true) 
        : base(samplerState ?? SamplerState.AnisotropicRepeat, autoDispose)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        TextureDescription description =
            new TextureDescription(TextureType.Texture2D, width, height, format, 0, 1, TextureUsage.ShaderResource);
        PieTexture = device.CreateTexture(description, data);
        device.GenerateMipmaps(PieTexture);
    }

    public void SetData<T>(int x, int y, int width, int height, T[] data) where T : unmanaged
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        device.UpdateTexture(PieTexture, x, y, (uint) width, (uint) height, data);
    }

    public static readonly Texture2D White = new Texture2D(1, 1, new byte[] { 255, 255, 255, 255 }, autoDispose: false);

    public static readonly Texture2D Black = new Texture2D(1, 1, new byte[] { 0, 0, 0, 255 }, autoDispose: false);

    public static readonly Texture2D EmptyNormal =
        new Texture2D(1, 1, new byte[] { 128, 128, 255, 255 }, autoDispose: false);

    public static readonly Texture2D Missing = new Texture2D(128, 128, Bitmap.GetMissingBitmap(128, 128), autoDispose: false);
}