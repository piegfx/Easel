using System.Numerics;

namespace Tests.Engine.TestScenes;

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
        
        Graphics.SpriteRenderer.Begin();
        Graphics.SpriteRenderer.Draw(_texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteFlip.None);
        Graphics.SpriteRenderer.End();
    }
}