using System;
using Easel.Utilities;
using Pie;

namespace Easel.Graphics;

/// <summary>
/// <see cref="Texture2D"/>s are used to texture most 3D meshes, and 2D sprites.
/// </summary>
public class Texture2D : TextureObject
{
    /// <summary>
    /// Create a new <see cref="Texture2D"/> from the given path.
    /// </summary>
    /// <param name="path">The path to load from.</param>
    /// <param name="autoDispose">If <see langword="true"/>, this <see cref="Texture2D"/> will be automatically disposed
    /// on scene change.</param>
    public Texture2D(string path, bool autoDispose = true) : this(new Bitmap(path), autoDispose) { }

    /// <summary>
    /// Create a new <see cref="Texture2D"/> from the given <see cref="Bitmap"/>. Useful for doing threaded loading.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to load from.</param>
    /// <param name="autoDispose">If <see langword="true"/>, this <see cref="Texture2D"/> will be automatically disposed
    /// on scene change.</param>
    public Texture2D(Bitmap bitmap, bool autoDispose = true) : base(autoDispose)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        PieTexture = device.CreateTexture(bitmap.Size.Width, bitmap.Size.Height, bitmap.Format, bitmap.Data);
    }
}