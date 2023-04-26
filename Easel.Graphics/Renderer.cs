using System;
using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie;
using Color = System.Drawing.Color;

namespace Easel.Graphics;

public sealed class Renderer : IDisposable
{
    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

    public RenderTarget2D MainBuffer;

    public Renderer(GraphicsDevice device, in RendererSettings settings)
    {
        Instance = this;
        
        Device = device;

        MainBuffer = new RenderTarget2D(settings.Resolution ?? (Size<int>) device.Swapchain.Size);

        SpriteRenderer = new SpriteRenderer(this);
    }
    
    public static Renderer Instance { get; private set; }

    public void BeginFrame()
    {
        Device.SetFramebuffer(MainBuffer.DeviceBuffer);
    }

    public void Present()
    {
        // Draw main target to screen.
        Device.SetFramebuffer(null);
        Device.Clear(Color.Black);
        SpriteRenderer.Begin();
        SpriteRenderer.Draw(MainBuffer, Vector2.Zero, null, Math.Color.White, 0, Vector2.Zero, Vector2.One);
        SpriteRenderer.End();

        // TODO: VSync controls.
        Device.Present(1);
    }

    public void Dispose()
    {
        MainBuffer.Dispose();
        SpriteRenderer.Dispose();
        Device.Dispose();
    }

    internal const string AssemblyName = "Easel.Graphics.Shaders";
}