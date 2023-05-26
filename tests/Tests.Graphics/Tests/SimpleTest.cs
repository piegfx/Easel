using System.Numerics;
using Easel.Graphics;
using Easel.Math;

namespace Tests.Graphics.Tests;

public class SimpleTest : TestBase
{
    private Texture2D _texture;
    
    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D("/home/skye/Pictures/awesomeface.png");
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.Device.ClearColorBuffer(System.Drawing.Color.CornflowerBlue);
        
        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.DrawSprite(_texture, new Vector2(0, 0), null, Color.White, 0, Vector2.One, Vector2.Zero);
        Renderer.SpriteRenderer.DrawSprite(_texture, new Vector2(100, 100), null, Color.White, 0, Vector2.One, Vector2.Zero);
        Renderer.SpriteRenderer.End();
    }

    public SimpleTest() : base("Simple test") { }
}