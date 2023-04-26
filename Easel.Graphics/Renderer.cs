using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Core;
using Easel.Graphics.Lighting;
using Easel.Graphics.Renderers;
using Easel.Graphics.Structs;
using Easel.Math;
using Pie;
using Color = System.Drawing.Color;

namespace Easel.Graphics;

public sealed class Renderer : IDisposable
{
    private List<TransformedRenderable> _opaques;
    private List<TransformedRenderable> _transluscents;
    private DirectionalLight _directionalLight;

    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

    public RenderTarget2D MainBuffer;
    
    public bool InFrame { get; private set; }

    public Renderer(GraphicsDevice device, in RendererSettings settings)
    {
        Instance = this;
        
        Device = device;

        MainBuffer = new RenderTarget2D(settings.Resolution ?? (Size<int>) device.Swapchain.Size);

        SpriteRenderer = new SpriteRenderer(this);

        _opaques = new List<TransformedRenderable>();
        _transluscents = new List<TransformedRenderable>();
    }

    public void BeginFrame()
    {
        if (InFrame)
            throw new EaselException("A frame is already active!");

        InFrame = true;
        
        Device.SetFramebuffer(MainBuffer.DeviceBuffer);
        
        _opaques.Clear();
        _transluscents.Clear();
        _directionalLight = null;
    }

    public void EndFrame()
    {
        if (!InFrame)
            throw new EaselException("No active frame!");

        InFrame = false;
        
        Device.Viewport = new System.Drawing.Rectangle(0, 0, MainBuffer.Size.Width, MainBuffer.Size.Height);
    }

    public void SetDirectionalLight(DirectionalLight light)
    {
        _directionalLight = light;
    }

    public void Draw(in Renderable renderable, Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    public void Perform3DPass(in CameraInfo cameraInfo, in SceneInfo sceneInfo, in Rectangle<float> viewport)
    {
        if (cameraInfo.ClearColor != null)
            Device.Clear((Vector4) cameraInfo.ClearColor.Value, ClearFlags.Depth | ClearFlags.Stencil);

        Size<int> targetSize = MainBuffer.Size;
        // The viewport is normalized 0-1 so we must convert it to screen coordinates as pie expects.
        Device.Viewport = new System.Drawing.Rectangle((int) (viewport.X * targetSize.Width),
            (int) (viewport.Y * targetSize.Height), (int) (viewport.Width * targetSize.Width),
            (int) (viewport.Height * targetSize.Height));
    }

    public void Present()
    {
        // Draw main target to screen.
        Device.SetFramebuffer(null);
        Device.Clear(Color.Black);
        SpriteRenderer.Begin();
        SpriteRenderer.Draw(MainBuffer, Vector2.Zero, null, Math.Color.White, 0, Vector2.Zero, Vector2.One);
        SpriteRenderer.End();

        // TODO: VSync controls.
        Device.Present(1);
    }

    public void Resize(in Size<int> size)
    {
        // TODO: Check to see if `Resolution` is null, in which case do not resize the RT.
        MainBuffer.Dispose();
        MainBuffer = new RenderTarget2D(size);
    }

    public void Dispose()
    {
        MainBuffer.Dispose();
        SpriteRenderer.Dispose();
        Device.Dispose();
    }
    
    public static Renderer Instance { get; private set; }

    internal const string AssemblyName = "Easel.Graphics.Shaders";
}