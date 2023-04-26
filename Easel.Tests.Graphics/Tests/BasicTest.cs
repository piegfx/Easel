using System.Drawing;
using System.Numerics;
using Easel.Graphics;

namespace Easel.Tests.Graphics.Tests;

public class BasicTest : TestBase
{
    private Texture2D _texture;
    private Texture2D _texture2;
    
    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D("/home/ollie/Pictures/awesomeface.png");
        _texture2 = new Texture2D("/home/ollie/Pictures/E V I L.png");
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.BeginFrame();
        
        Renderer.Device.Clear(Color.CornflowerBlue);

        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.Draw(_texture, new Vector2(0, 0), null, Math.Color.Orange, 0, Vector2.Zero, Vector2.One);
        Renderer.SpriteRenderer.Draw(_texture2, new Vector2(100, 100), null, Math.Color.White, 0, Vector2.Zero, new Vector2(2, 1));
        Renderer.SpriteRenderer.End();
    }
}