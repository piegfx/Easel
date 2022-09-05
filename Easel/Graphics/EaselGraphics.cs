using System;
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
        get => (Rectangle) PieGraphics.Viewport;
        set
        {
            PieGraphics.Viewport = (System.Drawing.Rectangle) value;
            ViewportResized?.Invoke(value);
        }
    }

    internal EaselGraphics(Window window, GraphicsDeviceOptions options)
    {
        Pie.Logging.DebugLog += PieDebug;
        PieGraphics = window.CreateGraphicsDevice(options);

        window.Resize += WindowOnResize;
    }

    private void PieDebug(LogType logtype, string message)
    {
        switch (logtype)
        {
            case LogType.Debug:
                Logging.Log(message);
                break;
            case LogType.Info:
                Logging.Info(message);
                break;
            case LogType.Warning:
                Logging.Warn(message);
                break;
            case LogType.Error:
                Logging.Error(message);
                break;
            case LogType.Critical:
                Logging.Critical(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logtype), logtype, null);
        }
    }

    /// <summary>
    /// Clear the current render target, clearing color, depth, and stencil.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    public void Clear(Color color)
    {
        PieGraphics.Clear((System.Drawing.Color) color, ClearFlags.Depth | ClearFlags.Stencil);
    }
    
    public void Dispose()
    {
        PieGraphics?.Dispose();
        Logging.Log("Graphics disposed.");
    }
    
    private void WindowOnResize(System.Drawing.Size size)
    {
        PieGraphics.ResizeSwapchain(size);
        Viewport = new Rectangle(Point.Zero, (Size) size);
    }

    public delegate void OnViewportResized(Rectangle viewport);
}