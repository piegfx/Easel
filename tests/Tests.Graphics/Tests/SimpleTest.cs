using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Tests.Graphics.Tests;

public class SimpleTest : TestBase
{
    private Texture2D _texture;
    
    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D(new Bitmap(Utils.LoadEmbeddedResource(Assembly.GetAssembly(typeof(Texture2D)),
            "Easel.Graphics.DEBUG.png")));
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.Device.ClearColorBuffer(System.Drawing.Color.CornflowerBlue);

        Rectangle<int>? source = new Rectangle<int>(64, 0, 64, 64);

        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.DrawSprite(_texture, new Vector2(0, 0), source, Color.White,
            0, new Vector2(2), Vector2.Zero, SpriteRenderer.Flip.FlipX);
        Renderer.SpriteRenderer.End();
    }

    public SimpleTest() : base("Simple test") { }
}