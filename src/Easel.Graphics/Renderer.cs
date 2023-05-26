using System;
using Pie;

namespace Easel.Graphics;

/// <summary>
/// Provides the base utils required for rendering graphics.
/// </summary>
public sealed class Renderer : IDisposable
{
    public GraphicsDevice Device;
    
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
        
        Device = device;
    }

    /// <summary>
    /// Dispose of this <see cref="Renderer"/>.
    /// </summary>
    public void Dispose()
    {
        Device.Dispose();
    }
    
    public static Renderer Instance { get; private set; }
}