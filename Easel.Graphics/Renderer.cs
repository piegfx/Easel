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
        // Acts as a guard to make sure you don't screw up any rendering bits. Each render frame is standalone and can't
        // interact with other frames.
        if (InFrame)
            throw new EaselException("A frame is already active!");

        InFrame = true;
        
        Device.SetFramebuffer(MainBuffer.DeviceBuffer);
        
        // Reset everything per frame.
        // Note that the directional light is also reset. While it does persist for the entire duration of the frame, in
        // keeping with the rest of the API it is reset on a new frame.
        _opaques.Clear();
        _transluscents.Clear();
        _directionalLight = null;
    }

    public void EndFrame()
    {
        if (!InFrame)
            throw new EaselException("No active frame!");

        InFrame = false;
        
        // Reset the device viewport back to the main buffer size so that we don't have any weird viewport stuff to deal
        // with.
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
        // Most cameras won't want to clear the screen, so we only clear for any cameras that have the clear color set.
        // Note that currently setting the clear color in the camera will clear the *entire* screen, so you should only
        // set the value for the first camera in the scene.
        // TODO: Depth-stencil clear only option?
        // TODO: Use scissor rectangle/RTs to allow every camera to clear the color buffer?
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