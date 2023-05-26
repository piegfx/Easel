namespace Easel.Graphics;

/// <summary>
/// Options used when initializing a <see cref="Renderer"/>.
/// </summary>
public struct RendererOptions
{
    public VSyncMode VSyncMode;

    public RendererOptions()
    {
        VSyncMode = VSyncMode.DoubleBuffer;
    }
}