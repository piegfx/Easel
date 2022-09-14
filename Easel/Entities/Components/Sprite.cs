using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Easel.Utilities;

namespace Easel.Entities.Components;

public class Sprite : Component
{
    public TextureObject Texture;

    public Rectangle? SourceRectangle;

    public Color Tint;

    public Vector2 Origin;

    public SpriteFlip Flip;

    public Sprite(TextureObject texture)
    {
        Texture = texture;
        SourceRectangle = null;
        Tint = Color.White;
        Origin = Vector2.Zero;
        Flip = SpriteFlip.None;
    }

    protected internal override void Draw()
    {
        base.Draw();

        SpriteRenderer.Draw(Texture, Transform.Position.ToVector2(), SourceRectangle, Tint,
            Transform.Rotation.ToEulerAngles().Z, Origin, Transform.Scale.ToVector2(), Flip);
    }
}