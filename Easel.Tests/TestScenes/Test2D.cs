using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class Test2D : Scene
{
    private Font _font;
    private RenderTarget _rt;

    private SpriteRenderer.SpriteVertex[] _vertices;
    private uint[] _indices;

    protected override void Initialize()
    {
        base.Initialize();

        _font = Content.Load<Font>("Abel-Regular");

        const uint size = 128;
        const string text = "piegfx.";
        
        _rt = new RenderTarget(_font.MeasureString(size, text) + new Size(0, 10));
        
        Graphics.SetRenderTarget(_rt);
        Graphics.SpriteRenderer.Begin();
        _font.Draw(Graphics.SpriteRenderer, 128, "piegfx.", Vector2.Zero, Color.White);
        Graphics.SpriteRenderer.End();
        Graphics.SetRenderTarget(null);

        _vertices = new SpriteRenderer.SpriteVertex[]
        {
            new(new Vector2(0, 0), new Vector2(0, 1), Color.Red),
            new(new Vector2(_rt.Size.Width, 0), new Vector2(1, 1), Color.Green),
            new(new Vector2(_rt.Size.Width, _rt.Size.Height), new Vector2(1, 0), Color.Blue),
            new(new Vector2(0, _rt.Size.Height), new Vector2(0, 0), Color.White),
        };

        _indices = new uint[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };
    }

    protected override void Draw()
    {
        base.Draw();

        Graphics.SpriteRenderer.Begin();
        Graphics.SpriteRenderer.DrawVertices(_rt, _vertices, _indices);
        Graphics.SpriteRenderer.End();
    }
}