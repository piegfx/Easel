using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Headless.Scenes;
using Easel.Math;
using Pie.Text;

namespace Easel.Tests.TestScenes;

public class TestFont : Scene
{
    private Texture2D _texture;
    
    protected override void Initialize()
    {
        base.Initialize();

        using FreeType freeType = new FreeType();
        using Face face = freeType.CreateFace("Content/Abel-Regular.ttf", 24);
        Character character = face.Characters['A'];
        //_texture = new Texture2D(character.Width, character.Height, character.);
    }

    protected override void Draw()
    {
        base.Draw();
        
        EaselGame.Instance.Graphics.SpriteRenderer.Begin();
        EaselGame.Instance.Graphics.SpriteRenderer.Draw(_texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteFlip.None);
        EaselGame.Instance.Graphics.SpriteRenderer.End();
    }
}