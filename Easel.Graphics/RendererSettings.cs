using Easel.Math;

namespace Easel.Graphics;

public struct RendererSettings
{
    public RenderMode Mode;

    public Size<int>? Resolution;

    public RendererSettings(RenderMode mode = RenderMode.Deferred, Size<int>? resolution = null)
    {
        Mode = mode;
        Resolution = resolution;
    }
}