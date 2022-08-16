using System;
using System.Drawing;
using PieTexture = Pie.Texture;

namespace Easel.Graphics;

public abstract class Texture : IDisposable
{
    public bool IsDisposed { get; protected set; }
    
    public PieTexture PieTexture { get; protected set; }

    public Size Size => PieTexture.Size;

    public Texture(bool autoDispose)
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