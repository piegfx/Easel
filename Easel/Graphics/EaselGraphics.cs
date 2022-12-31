using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Easel.Utilities;
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

    public EffectManager EffectManager;

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

    internal EaselGraphics(Window window, GraphicsDeviceOptions options)
    {
        Pie.Logging.DebugLog += PieDebug;
        PieGraphics = window.CreateGraphicsDevice(options);
        Viewport = new Rectangle(0, 0, window.Size.Width, window.Size.Height);

        window.Resize += WindowOnResize;
    }

    internal void Initialize(in RenderOptions options)
    {
        EffectManager = new EffectManager(PieGraphics);

        if (options.Deferred)
            throw new NotImplementedException("Deferred rendering has currently not been implemented.");
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
        Viewport = new Rectangle(Point.Zero, target?.Size ?? (Size) EaselGame.Instance.Window.Size);
    }

    public void Dispose()
    {
        PieGraphics?.Dispose();
        Logger.Debug("Graphics disposed.");
    }
    
    private void WindowOnResize(System.Drawing.Size size)
    {
        if (size == new System.Drawing.Size(0, 0))
            return;
        PieGraphics.ResizeSwapchain(size);
        Viewport = new Rectangle(Point.Zero, (Size) size);
        SwapchainResized?.Invoke((Size) size);
    }

    public delegate void OnViewportResized(Rectangle viewport);
    
    public delegate void OnSwapchainResized(Size size);
}