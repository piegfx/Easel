using System.Drawing;
using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Color = Easel.Math.Color;

namespace Tests.Graphics.Tests;

public class SimpleTest : TestBase
{
    private Texture2D _texture;
    private float _rotation;
    
    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D(new Bitmap(Utils.LoadEmbeddedResource(Assembly.GetAssembly(typeof(Texture2D)),
            "Easel.Graphics.DEBUG.png")));
    }

    protected override void Update(double dt)
    {
        base.Update(dt);

        _rotation += 1 * (float) dt;
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.NewFrame();
        
        Renderer.Device.ClearColorBuffer(System.Drawing.Color.CornflowerBlue);

        Rectangle<int>? source = null;

        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.DrawSprite(_texture, new Vector2(200, 200), source, Color.White, 
            _rotation, new Vector2(2, 2), new Vector2(0));
        Renderer.SpriteRenderer.End();
        
        Renderer.EndFrame();
    }

    public SimpleTest() : base("Simple test") { }
}