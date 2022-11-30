using System;
using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public class PostProcessor
{
    public RenderTarget MainTarget;
    private Effect _effect;
    private bool _shouldAutomaticallyAdjust;
    private Size? _resolution;

    public Size? Resolution
    {
        get => _resolution;
        set
        {
            if (value != null)
            {
                _resolution = value;
                _shouldAutomaticallyAdjust = false;
                ResizeTarget(value.Value);
            }
        }
    }
    
    public PostProcessor(ref PostProcessorSettings settings, EaselGraphics graphics, Size? resolution = null)
    {
        _resolution = resolution;
        _shouldAutomaticallyAdjust = resolution == null;
        MainTarget = new RenderTarget(resolution ?? graphics.Viewport.Size, false);
        graphics.SwapchainResized += GraphicsOnSwapchainResized;
        CreateResources(ref settings);
    }

    private void GraphicsOnSwapchainResized(Size size)
    {
        if (_shouldAutomaticallyAdjust)
            ResizeTarget(size);
    }

    private void ResizeTarget(Size size)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget(size);
    }

    public void ApplyPostProcessorSettings(ref PostProcessorSettings settings)
    {
        Clear();
        CreateResources(ref settings);
    }

    private void Clear()
    {
        
    }

    private void CreateResources(ref PostProcessorSettings settings)
    {
        _effect = new Effect("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert",
            "Easel.Graphics.Shaders.PostProcessor.frag");
    }

    public void Process(EaselGraphics graphics)
    {
        graphics.SpriteRenderer.Begin(effect: _effect);
        graphics.SpriteRenderer.Draw(MainTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One);
        graphics.SpriteRenderer.End();
    }
    
    public struct PostProcessorSettings
    {
        public bool Fxaa;
    }
}