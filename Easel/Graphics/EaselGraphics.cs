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

    public I3DRenderer Renderer;

    public I2DRenderer Renderer2D;

    public PostProcessor PostProcessor;

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

        _renderables = new Dictionary<Mesh, GCRenderable>();

        window.Resize += WindowOnResize;
    }

    internal void Initialize(I3DRenderer renderer, I2DRenderer renderer2D)
    {
        EffectManager = new EffectManager(PieGraphics);

        Renderer = renderer;
        Renderer2D = renderer2D;
        SpriteRenderer = new SpriteRenderer(PieGraphics);

        PostProcessor.PostProcessorSettings settings = new PostProcessor.PostProcessorSettings();
        PostProcessor = new PostProcessor(ref settings, this, null);
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

    public Renderable CreateRenderable(in Mesh mesh)
    {
        Renderable renderable = new Renderable();
        renderable.VertexBuffer = PieGraphics.CreateBuffer(BufferType.VertexBuffer, mesh.Vertices);
        renderable.IndexBuffer = PieGraphics.CreateBuffer(BufferType.IndexBuffer, mesh.Indices);
        renderable.IndicesLength = (uint) mesh.Indices.Length;
        renderable.Material = mesh.Material;
        return renderable;
    }

    private Dictionary<Mesh, GCRenderable> _renderables;

    public void DrawMesh(in Mesh mesh, in Matrix4x4 world)
    {
        if (!_renderables.TryGetValue(mesh, out GCRenderable renderable))
        {
            Logger.Debug("Creating new mesh...");
            _renderables.Add(mesh, renderable = new GCRenderable(CreateRenderable(mesh)));
        }

        Renderer.DrawOpaque(renderable.Get(), world);
    }

    public void CleanMeshes()
    {
        foreach ((Mesh mesh, GCRenderable renderable) in _renderables)
        {
            if (renderable.TryDispose())
            {
                Logger.Debug("Disposing unused mesh...");
                _renderables.Remove(mesh);
            }
        }
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