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
        _readBuffer = new RenderTarget(graphics.Renderer.MainTarget.Size);
        _writeBuffer = new RenderTarget(graphics.Renderer.MainTarget.Size);

        Radius = radius;
        Iterations = iterations;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        EaselGraphics graphics = EaselGame.Instance.GraphicsInternal;
        RenderTarget rt = graphics.Renderer.MainTarget;
        
        renderer.End();
        
        graphics.SetRenderTarget(_readBuffer);
        graphics.Viewport = new Rectangle(Point.Zero, graphics.Renderer.MainTarget.Size);
        graphics.Clear(Color.CornflowerBlue);
        renderer.Begin();
        renderer.Draw(rt, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One);
        renderer.End();
        graphics.SetRenderTarget(null);

        for (int i = 0; i < Iterations; i++)
        {
            float radius = (Iterations - i - 1) * Radius;
            Vector2 direction = i % 2 == 0 ? new Vector2(radius, 0) : new Vector2(0, radius);
            
            graphics.SetRenderTarget(_writeBuffer);
            renderer.Begin(effect: _effect);

            renderer.Draw(_readBuffer, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One,
                meta1: new Vector4(direction, graphics.Renderer.MainTarget.Size.Width, graphics.Renderer.MainTarget.Size.Height));
            
            renderer.End();
            graphics.SetRenderTarget(null);
            (_readBuffer, _writeBuffer) = (_writeBuffer, _readBuffer);
        }
        
        renderer.Begin();

        // TODO: Fix whatever the heck is going on here, I just have no idea
        renderer.Draw(_readBuffer, (Vector2) CalculatedScreenPos, new Rectangle(CalculatedScreenPos, Size), Color.White,
            0, Vector2.Zero, Vector2.One);
    }
}