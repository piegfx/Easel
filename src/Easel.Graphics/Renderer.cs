using System;
using System.Collections.Generic;
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

    private List<(Renderable renderable, Matrix4x4 world)> _opaques;

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
        // This main target doesn't need a depth texture - nothing requiring depth should ever render to it.
        MainTarget = new RenderTarget2D(targetSize, depthFormat: null);

        VSyncMode = options.VSyncMode;
        
        Logger.Debug("Creating sprite renderer.");
        SpriteRenderer = new SpriteRenderer(device);
        
        Logger.Debug("Creating main renderer.");
        _renderer = new ForwardRenderer(targetSize, this);

        _opaques = new List<(Renderable, Matrix4x4)>();
    }

    public void NewFrame()
    {
        SetRenderTarget(null);
        _opaques.Clear();
    }

    public void EndFrame()
    {
        SetRenderTarget(null);
        SpriteRenderer.Begin();
        SpriteRenderer.DrawSprite(_renderer.MainTarget, Vector2.Zero, null, Color.White, 0, Vector2.One, Vector2.Zero);
        SpriteRenderer.End();
        
        Device.SetFramebuffer(null);
        
        SpriteRenderer.Begin();
        SpriteRenderer.DrawSprite(MainTarget, Vector2.Zero, null, Color.White, 0, Vector2.One, Vector2.Zero);
        SpriteRenderer.End();
    }

    public void Draw(in Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add((renderable, world));
    }

    public void Perform3DPass(in CameraInfo cameraInfo, in SceneInfo sceneInfo, in Rectangle<float> viewport)
    {
        // Sort by depth, front to back. This acts a bit like a depth prepass, the GPU does not try to process fragments
        // that are covered by other fragments. During larger passes this can significantly improve efficiency.
        _opaques.Sort((r1, r2) => MathF.Sign(Vector3.Distance(r1.world.Translation, r2.world.Translation)));
        
        // TODO: perform shadow pass etc

        // Viewport is normalized - an X value of 0 will be the left hand side of the screen, and an X value of 1 will
        // be the right hand side of the screen.
        // Since the device viewport is in screen space, we must do the conversion here.
        Size<int> rendererSize = _renderer.MainTarget.Size;
        Device.Viewport = new Rectangle((int) (viewport.X / rendererSize.Width),
            (int) (viewport.Y / rendererSize.Height), (int) (viewport.Width / rendererSize.Width),
            (int) (viewport.Height / rendererSize.Height));
        
        // Every 3D pass clears the depth-stencil buffer.
        // However the user gets to choose whether they want to clear the color buffer or not.
        if (cameraInfo.ClearColor != null)
            Device.ClearColorBuffer((Vector4) cameraInfo.ClearColor.Value);
        Device.ClearDepthStencilBuffer(ClearFlags.Depth | ClearFlags.Stencil, 1, 0);
        
        _renderer.Begin3DPass(cameraInfo.View, cameraInfo.Projection, cameraInfo.WorldPosition, sceneInfo);
        
        foreach ((Renderable renderable, Matrix4x4 world) in _opaques)
            _renderer.DrawRenderable(renderable, world);

        _renderer.End3DPass();
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
        MainTarget = new RenderTarget2D(newSize, depthFormat: null);
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