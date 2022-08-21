using System;
using System.Drawing;
using Pie;

namespace Easel.Graphics;

// TODO: Think of a better name that isn't Texture.
public abstract class TextureObject : IDisposable
{
    public bool IsDisposed { get; protected set; }
    
    public Texture PieTexture { get; protected set; }

    public Size Size => PieTexture.Size;

    public TextureObject(bool autoDispose)
    {
        // TODO: Auto disposing
    }

    public virtual void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        PieTexture.Dispose();
    }
}