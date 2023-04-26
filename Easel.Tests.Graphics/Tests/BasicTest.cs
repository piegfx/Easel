using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Color = System.Drawing.Color;

namespace Easel.Tests.Graphics.Tests;

public class BasicTest : TestBase
{
    private Texture2D _texture;

    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D(Bitmap.Debug);
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.BeginFrame();
        
        Renderer.Device.Clear(Color.CornflowerBlue);

        Renderer.SpriteRenderer.Begin();
        Renderer.SpriteRenderer.Draw(_texture, new Vector2(0, 0), new Rectangle<int>(100, 64, 128, 64), Math.Color.White,
            0, Vector2.Zero, new Vector2(3, 1.5f));
        //Renderer.SpriteRenderer.Draw(_texture2, new Vector2(100, 100), null, Math.Color.White, 0, Vector2.Zero, new Vector2(2, 1));
        Renderer.SpriteRenderer.End();
    }
}