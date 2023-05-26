using System;
using System.Drawing;
using System.Numerics;
using Easel.Core;
using Easel.Graphics.Renderers;
using Easel.Graphics.Structs;
using Easel.Math;
using Pie;
using Color = Easel.Math.Color;

namespace Easel.Graphics;

/// <summary>
/// Provides the base utils required for rendering graphics.
/// </summary>
public sealed class Renderer : IDisposable
{
    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

    public VSyncMode VSyncMode;
    
    public RenderTarget2D MainTarget { get; private set; }

    private IRenderer _renderer;
    
    /// <summary>
    /// Create a new <see cref="Renderer"/>, for use with rendering.
    /// </summary>
    /// <param name="device">The graphics device to use.</param>
    /// <param name="options">The renderer options.</param>
    /// <remarks>The renderer assumes ownership over the <paramref name="device"/>. As such, you should not rely on its
    /// state if using it outside of a <see cref="Renderer"/>.</remarks>
    public Renderer(GraphicsDevice device, in RendererOptions options)
    {
        Logger.Debug("Initializing renderer.");
        Instance = this;
        Device = device;

        Size<int> targetSize = options.Size ?? (Size<int>) device.Swapchain.Size;
        Logger.Debug($"Creating main render target with size {targetSize}.");
        MainTarget = new RenderTarget2D(targetSize);

        VSyncMode = options.VSyncMode;
        
        Logger.Debug("Creating sprite renderer.");
        SpriteRenderer = new SpriteRenderer(device);
        
        Logger.Debug("Creating main renderer.");
        _renderer = new DeferredRenderer();
    }

    public void NewFrame()
    {
        SetRenderTarget(null);
    }

    public void EndFrame()
    {
        Device.SetFramebuffer(null);
        
        SpriteRenderer.Begin();
        SpriteRenderer.DrawSprite(MainTarget, Vector2.Zero, null, Color.White, 0, Vector2.One, Vector2.Zero);
        SpriteRenderer.End();
    }

    public void Perform3DPass(in CameraInfo cameraInfo, in SceneInfo sceneInfo, in Rectangle<float> viewport)
    {
        
    }

    public void SetRenderTarget(RenderTarget2D target)
    {
        target ??= MainTarget;
        
        Device.Viewport = new Rectangle(0, 0, target.Size.Width, target.Size.Height);
        Device.SetFramebuffer(target.PieFramebuffer);
    }

    public void Resize(Size<int> newSize)
    {
        Device.ResizeSwapchain((System.Drawing.Size) newSize);
        
        // Recreate our main render target.
        // TODO: A nullable size parameter, which, if set, will NOT resize the main target.
        MainTarget.Dispose();
        MainTarget = new RenderTarget2D(newSize);
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
        MainTarget.Dispose();
        SpriteRenderer.Dispose();
        Device.Dispose();
    }
    
    public static Renderer Instance { get; private set; }

    public const string ShaderNamespace = "Easel.Graphics.Shaders";
}