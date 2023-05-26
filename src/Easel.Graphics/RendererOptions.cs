using Easel.Math;

namespace Easel.Graphics;

/// <summary>
/// Options used when initializing a <see cref="Renderer"/>.
/// </summary>
public struct RendererOptions
{
    public Size<int>? Size;

    public VSyncMode VSyncMode;

    public RendererOptions()
    {
        Size = null;
        VSyncMode = VSyncMode.DoubleBuffer;
    }
}