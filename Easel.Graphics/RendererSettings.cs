namespace Easel.Graphics;

public struct RendererSettings
{
    public RenderMode RenderMode;

    public RendererSettings(RenderMode renderMode = RenderMode.Deferred)
    {
        RenderMode = renderMode;
    }
}