using System;
using Easel.Math;
using Easel.Scenes;
using Pie;

namespace Easel.Graphics;

// TODO: Think of a better name that isn't Texture.
/// <summary>
/// The base texture class, for objects that can be textured.
/// </summary>
public abstract class TextureObject : IDisposable
{
    /// <summary>
    /// Returns <see langword="true"/> if this <see cref="TextureObject"/> has been disposed.
    /// </summary>
    public bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The native Pie <see cref="Texture"/>.
    /// </summary>
    public Texture PieTexture { get; protected set; }

    /// <summary>
    /// The size (resolution), in pixels of the texture.
    /// </summary>
    public Size Size => (Size) PieTexture.Size;

    protected TextureObject(bool autoDispose)
    {
        if (autoDispose)
            SceneManager.ActiveScene.GarbageCollections.Add(this);
    }

    public virtual void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        PieTexture.Dispose();
    }
}