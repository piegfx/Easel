using System;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie;

namespace Easel.Graphics;

public sealed class Renderer : IDisposable
{
    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

    public RenderTarget2D MainBuffer;

    public Renderer(GraphicsDevice device, in RendererSettings settings)
    {
        Device = device;

        MainBuffer = new RenderTarget2D(settings.Resolution ?? (Size<int>) device.Swapchain.Size);

        SpriteRenderer = new SpriteRenderer(this);

        Instance = this;
    }
    
    public static Renderer Instance { get; private set; }

    public void Dispose()
    {
        MainBuffer.Dispose();
        SpriteRenderer.Dispose();
        Device.Dispose();
    }

    internal const string AssemblyName = "Easel.Graphics.Shaders";
}