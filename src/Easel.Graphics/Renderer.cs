using System;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie;

namespace Easel.Graphics;

/// <summary>
/// Provides the base utils required for rendering graphics.
/// </summary>
public sealed class Renderer : IDisposable
{
    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

    public VSyncMode VSyncMode;
    
    /// <summary>
    /// Create a new <see cref="Renderer"/>, for use with rendering.
    /// </summary>
    /// <param name="device">The graphics device to use.</param>
    /// <param name="options">The renderer options.</param>
    /// <remarks>The renderer assumes ownership over the <paramref name="device"/>. As such, you should not rely on its
    /// state if using it outside of a <see cref="Renderer"/>.</remarks>
    public Renderer(GraphicsDevice device, in RendererOptions options)
    {
        Instance = this;

        VSyncMode = options.VSyncMode;
        
        Device = device;
        SpriteRenderer = new SpriteRenderer(device);
    }

    public void Resize(Size<int> newSize)
    {
        Device.ResizeSwapchain((System.Drawing.Size) newSize);
    }

    public void Present()
    {
        Device.Present(VSyncMode == VSyncMode.DoubleBuffer ? 1 : 0);
    }

    /// <summary>
    /// Dispose of this <see cref="Renderer"/>.
    /// </summary>
    public void Dispose()
    {
        SpriteRenderer.Dispose();
        Device.Dispose();
    }
    
    public static Renderer Instance { get; private set; }

    public const string ShaderNamespace = "Easel.Graphics.Shaders";
}