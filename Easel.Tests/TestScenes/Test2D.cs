using System;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Sprite = Easel.Entities.Components.Sprite;

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
        const string text = "docs.piegfx.com";
        
        _rt = new RenderTarget(_font.MeasureString(size, text) + new Size(0, 10));
        
        Graphics.SetRenderTarget(_rt);
        Graphics.Clear(Color.Transparent);
        Graphics.SpriteRenderer.Begin();
        _font.Draw(Graphics.SpriteRenderer, size, text, Vector2.Zero, Color.White);
        Graphics.SpriteRenderer.End();
        Graphics.SetRenderTarget(null);

        _vertices = new SpriteRenderer.SpriteVertex[]
        {
            new(new Vector2(0, 0), new Vector2(0, 1), Color.Cyan),
            new(new Vector2(_rt.Size.Width, 0), new Vector2(1, 1), Color.RebeccaPurple),
            new(new Vector2(_rt.Size.Width, _rt.Size.Height), new Vector2(1, 0), Color.Purple),
            new(new Vector2(0, _rt.Size.Height), new Vector2(0, 0), Color.Blue),
        };

        _indices = new uint[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        Texture2D texture = Content.Load<Texture2D>("awesomeface");
        
        Entity sprite = new Entity();
        sprite.AddComponent(new Sprite(texture));
        Console.WriteLine(sprite.TryAddComponent(new Sprite(texture)));
    }

    protected override void Draw()
    {
        base.Draw();

        Graphics.SpriteRenderer.Begin();
        Graphics.SpriteRenderer.DrawVertices(_rt, _vertices, _indices);
        Graphics.SpriteRenderer.End();
    }
}