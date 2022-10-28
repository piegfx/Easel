using System;
using Easel.Graphics;
using Easel.Math;

namespace Easel;

public class PostProcessor
{
    public RenderTarget MainTarget;
    private Effect _effect;
    
    public PostProcessor(ref PostProcessorSettings settings, EaselGraphics graphics)
    {
        Console.WriteLine(graphics.Viewport.Size);
        MainTarget = new RenderTarget(graphics.Viewport.Size, false);
        CreateResources(ref settings);
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
        //graphics.SpriteRenderer.End();
        //graphics.SpriteRenderer.Begin(effect: _effect);
        graphics.SpriteRenderer.Draw(MainTarget, graphics.Viewport, Color.White);
        //graphics.SpriteRenderer.End();
        //graphics.SpriteRenderer.Begin();
        //graphics.SpriteRenderer.End();
    }
    
    public struct PostProcessorSettings
    {
        public bool Fxaa;
    }
}