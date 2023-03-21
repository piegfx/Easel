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
    private Rectangle<int> _viewport;

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

    public RenderTarget MainTarget;

    public IRenderer Renderer;

    public SpriteRenderer SpriteRenderer;

    /// <summary>
    /// If enabled, the game will synchronize with the monitor's vertical refresh rate.
    /// </summary>
    public bool VSync;

    /// <summary>
    /// Get or set the graphics viewport. If set, <see cref="ViewportResized"/> is invoked.
    /// </summary>
    public Rectangle<int> Viewport
    {
        get => _viewport;
        set
        {
            // TODO: ????????
            //if (value == _viewport)
            //    return;
            _viewport = new Rectangle<int>(value.X, value.Y, value.Width, value.Height);
            PieGraphics.Viewport = (System.Drawing.Rectangle) _viewport;
            //if (value == _viewport)
            //    return;
            ViewportResized?.Invoke(_viewport);
        }
    }

    public EaselGraphics(GraphicsDevice pieDevice, RenderOptions options)
    {
        Instance = this;
        
        PieGraphics = pieDevice;

        Viewport = new Rectangle<int>(Vector2T<int>.Zero, (Size<int>) pieDevice.Swapchain.Size);

        MainTarget = new RenderTarget(Viewport.Size, autoDispose: false);

        if (options.Deferred)
            throw new NotImplementedException("Deferred rendering has currently not been implemented.");

        // TODO: Move SpriteRenderer to generic batch renderer for both 2D and 3D?
        SpriteRenderer = new SpriteRenderer(PieGraphics);

        Renderer = new ForwardRenderer(this, Viewport.Size);

        VSync = true;
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
        PieGraphics.SetFramebuffer(target == null ? MainTarget.PieBuffer : target.PieBuffer);
        Viewport = new Rectangle<int>(Vector2T<int>.Zero, target?.Size ?? (Size<int>) PieGraphics.Swapchain.Size);
    }

    public void Dispose()
    {
        PieGraphics?.Dispose();
        Logger.Debug("Graphics disposed.");
    }
    
    public void ResizeGraphics(Size<int> size)
    {
        if (size == Size<int>.Zero)
            return;
        PieGraphics.ResizeSwapchain((System.Drawing.Size) size);
        MainTarget.Dispose();
        MainTarget = new RenderTarget(size, autoDispose: false);
        SetRenderTarget(null);
        SwapchainResized?.Invoke(size);
        Viewport = new Rectangle<int>(Vector2T<int>.Zero, size);
    }

    public void Present()
    {
        PieGraphics.SetFramebuffer(null);
        SpriteRenderer.Begin(blendState: BlendState.DisabledRgbMask);
        SpriteRenderer.Draw(MainTarget, Vector2T<float>.Zero, null, Color.White, 0, Vector2T<float>.Zero,
            Vector2T<float>.One);
        SpriteRenderer.End();
        PieGraphics.Present(VSync ? 1 : 0);
    }

    public delegate void OnViewportResized(Rectangle<int> viewport);
    
    public delegate void OnSwapchainResized(Size<int> size);

    internal static EaselGraphics Instance;
}