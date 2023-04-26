using System;
using Easel.Graphics.Renderers;
using Pie;

namespace Easel.Graphics;

public sealed class Renderer : IDisposable
{
    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

    public Renderer(GraphicsDevice device, in RendererSettings settings)
    {
        Device = device;

        Instance = this;
    }
    
    public static Renderer Instance { get; private set; }

    public void Dispose()
    {
        SpriteRenderer.Dispose();
        Device.Dispose();
    }
}