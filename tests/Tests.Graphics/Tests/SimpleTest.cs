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
    private RenderTarget2D _renderTarget;
    private float _rotation;
    
    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D(new Bitmap(Utils.LoadEmbeddedResource(Assembly.GetAssembly(typeof(Texture2D)),
            "Easel.Graphics.DEBUG.png")));

        _renderTarget = new RenderTarget2D(new Size<int>(512, 512));
    }

    protected override void Update(double dt)
    {
        base.Update(dt);

        _rotation += 1 * (float) dt;
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);

        Renderer.Device.SetFramebuffer(_renderTarget.PieFramebuffer);
        Renderer.Device.Viewport = new Rectangle(0, 0, _renderTarget.Size.Width, _renderTarget.Size.Height);
        Renderer.Device.ClearColorBuffer(System.Drawing.Color.RebeccaPurple);
        
        Rectangle<int>? source = null;

        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.DrawSprite(_texture, new Vector2(200, 200), source, Color.White, 
            _rotation, new Vector2(2, 2), new Vector2(0));
        Renderer.SpriteRenderer.End();
        
        Renderer.Device.SetFramebuffer(null);
        Renderer.Device.Viewport = new Rectangle(0, 0, 1280, 720);
        
        Renderer.Device.ClearColorBuffer(System.Drawing.Color.CornflowerBlue);
        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.DrawSprite(_renderTarget, new Vector2(0, 0), null, Color.White, 0, new Vector2(2, 1),
            Vector2.Zero);
        Renderer.SpriteRenderer.End();
    }

    public SimpleTest() : base("Simple test") { }
}