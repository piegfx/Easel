using System;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public class PostProcessor
{
    public RenderTarget MainTarget;
    private Effect _effect;
    
    public PostProcessor(ref PostProcessorSettings settings, EaselGraphics graphics)
    {
        MainTarget = new RenderTarget(graphics.Viewport.Size, false);
        graphics.ViewportResized += GraphicsOnViewportResized;
        CreateResources(ref settings);
    }

    private void GraphicsOnViewportResized(Rectangle viewport)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget(viewport.Size);
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
        graphics.SpriteRenderer.Draw(MainTarget, graphics.Viewport, Color.White);
        graphics.SpriteRenderer.End();
    }
    
    public struct PostProcessorSettings
    {
        public bool Fxaa;
    }
}