using System;
using System.Collections.Generic;
using Easel.Core;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie;
using Pie.Windowing;
using Color = Easel.Math.Color;

namespace Easel.Graphics;

/// <summary>
/// EaselGraphics adds a few QOL features over a Pie graphics device, including viewport resize events, and easier
/// screen clearing.
/// </summary>
public class EaselGraphics : IDisposable
{
    private Rectangle _viewport;
    internal List<IDisposable> Disposables;

    /// <summary>
    /// Is invoked when the <see cref="Viewport"/> is resized.
    /// </summary>
    public event OnViewportResized ViewportResized;

    // Temporary
    public event OnSwapchainResized SwapchainResized;
    
    /// <summary>
    /// Access the Pie graphics device to gain lower-level graphics access.
    /// </summary>
    public readonly GraphicsDevice PieGraphics;

    public IRenderer Renderer;

    public SpriteRenderer SpriteRenderer;

    /// <summary>
    /// Get or set the graphics viewport. If set, <see cref="ViewportResized"/> is invoked.
    /// </summary>
    public Rectangle Viewport
    {
        get => _viewport;
        set
        {
            if (value == _viewport)
                return;
            _viewport = new Rectangle(value.X, value.Y, value.Width, value.Height);
            PieGraphics.Viewport = (System.Drawing.Rectangle) _viewport;
            ViewportResized?.Invoke(_viewport);
        }
    }

    public EaselGraphics(GraphicsDevice pieDevice, RenderOptions options)
    {
        Logging.DebugLog += PieDebug;
        PieGraphics = pieDevice;

        Viewport = new Rectangle(Point.Zero, (Size) pieDevice.Swapchain.Size);
        
        Instance = this;
        Disposables = new List<IDisposable>();

        if (options.Deferred)
            throw new NotImplementedException("Deferred rendering has currently not been implemented.");

        // TODO: Move SpriteRenderer to generic batch renderer for both 2D and 3D?
        SpriteRenderer = new SpriteRenderer(PieGraphics);

        Renderer = new ForwardRenderer(this, Viewport.Size);
    }

    private void PieDebug(LogType logtype, string message)
    {
        if (logtype == LogType.Debug)
            return;
        Logger.Log((Logger.LogType) logtype, message);
    }

    /// <summary>
    /// Clear the current render target, clearing color, depth, and stencil.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    public void Clear(Color color)
    {
        PieGraphics.Clear((System.Drawing.Color) color, ClearFlags.Depth | ClearFlags.Stencil);
    }

    public void SetRenderTarget(RenderTarget target)
    {
        PieGraphics.SetFramebuffer(target?.PieBuffer);
        Viewport = new Rectangle(Point.Zero, target?.Size ?? (Size) PieGraphics.Swapchain.Size);
    }

    public void Dispose()
    {
        PieGraphics?.Dispose();
        Logger.Debug("Graphics disposed.");
    }
    
    public void ResizeGraphics(Size size)
    {
        if (size == Size.Zero)
            return;
        PieGraphics.ResizeSwapchain((System.Drawing.Size) size);
        SwapchainResized?.Invoke(size);
        Viewport = new Rectangle(Point.Zero, size);
    }

    public delegate void OnViewportResized(Rectangle viewport);
    
    public delegate void OnSwapchainResized(Size size);

    internal static EaselGraphics Instance;
}