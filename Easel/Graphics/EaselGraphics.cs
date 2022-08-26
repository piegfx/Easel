using System;
using System.Drawing;
using Pie;
using Pie.Windowing;

namespace Easel.Graphics;

/// <summary>
/// EaselGraphics adds a few QOL features over a Pie graphics device, including viewport resize events, and easier
/// screen clearing.
/// </summary>
public class EaselGraphics : IDisposable
{
    /// <summary>
    /// Is invoked when the <see cref="Viewport"/> is resized.
    /// </summary>
    public event OnViewportResized ViewportResized;
    
    /// <summary>
    /// Access the Pie graphics device to gain lower-level graphics access.
    /// </summary>
    public readonly GraphicsDevice PieGraphics;

    /// <summary>
    /// Get or set the graphics viewport. If set, <see cref="ViewportResized"/> is invoked.
    /// </summary>
    public Rectangle Viewport
    {
        get => PieGraphics.Viewport;
        set
        {
            PieGraphics.Viewport = value;
            ViewportResized?.Invoke(value);
        }
    }

    internal EaselGraphics(Window window)
    {
        PieGraphics = window.CreateGraphicsDevice(GraphicsDeviceCreationFlags.Debug);
        
        window.Resize += WindowOnResize;
    }

    /// <summary>
    /// Clear the current render target, clearing color, depth, and stencil.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    public void Clear(Color color)
    {
        PieGraphics.Clear(color, ClearFlags.Depth | ClearFlags.Stencil);
    }
    
    public void Dispose()
    {
        PieGraphics?.Dispose();
    }
    
    private void WindowOnResize(Size size)
    {
        PieGraphics.ResizeSwapchain(size);
        Viewport = new Rectangle(Point.Empty, size);
    }

    public delegate void OnViewportResized(Rectangle viewport);
}