using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class GaussianBlur : UIElement
{
    private static Effect _effect;

    private RenderTarget _readBuffer;
    private RenderTarget _writeBuffer;

    public float Radius;
    public int Iterations;

    public GaussianBlur(Position position, Size<int> size, float radius, int iterations) : base(position, size)
    {
        _effect ??= new Effect("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert",
            "Easel.Graphics.Shaders.SpriteRenderer.Sprite.frag", defines: "BLUR");
        
        EaselGraphics graphics = EaselGame.Instance.GraphicsInternal;
        
        // TODO TODO TODO This is incredibly wasteful, fix the damn render target stuff so we only need to create a 
        // render target of the right size instead of blurring the *entire* target.
        _readBuffer = new RenderTarget(size);
        _writeBuffer = new RenderTarget(size);

        Radius = radius;
        Iterations = iterations;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        EaselGraphics graphics = EaselGame.Instance.GraphicsInternal;
        RenderTarget rt = graphics.Renderer.MainTarget;
        
        renderer.End();
        
        graphics.SetRenderTarget(_readBuffer);
        graphics.Viewport = new Rectangle<int>(new Vector2<int>(0, rt.Size.Height - Size.Height), rt.Size);
        graphics.Clear(Color.CornflowerBlue);
        renderer.Begin();
        renderer.Draw(rt, new Vector2<float>(0, 000), new Rectangle<int>(CalculatedScreenPos, Size), Color.White, 0, Vector2<float>.Zero, Vector2<float>.One);
        renderer.End();
        graphics.SetRenderTarget(null);

        for (int i = 0; i < Iterations; i++)
        {
            float radius = (Iterations - i - 1) * Radius;
            Vector2<float> direction = i % 2 == 0 ? new Vector2<float>(radius, 0) : new Vector2<float>(0, radius);
            
            graphics.SetRenderTarget(_writeBuffer);
            graphics.Viewport = new Rectangle<int>(new Vector2<int>(0, rt.Size.Height - Size.Height), rt.Size);
            renderer.Begin(effect: _effect);

            renderer.Draw(_readBuffer, Vector2<float>.Zero, null, Color.White, 0, Vector2<float>.Zero, Vector2<float>.One,
                meta1: new Vector4((System.Numerics.Vector2) direction, Size.Width, Size.Height));
            
            renderer.End();
            graphics.SetRenderTarget(null);
            (_readBuffer, _writeBuffer) = (_writeBuffer, _readBuffer);
        }
        
        renderer.Begin();

        // TODO: Fix whatever the heck is going on here, I just have no idea
        renderer.Draw(_readBuffer, (Vector2<float>) CalculatedScreenPos, null, Color.White,
            0, Vector2<float>.Zero, Vector2<float>.One);
        
        renderer.DrawRectangle((Vector2<float>) CalculatedScreenPos, Size, 0, 0, new Color(Color.White, 0.15f), Color.Transparent, 0, Vector2<float>.Zero);
    }
}