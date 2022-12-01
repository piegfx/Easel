using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Easel.Utilities;

namespace Easel.Entities.Components;

public class Sprite : Component
{
    public Texture Texture;

    public Rectangle? SourceRectangle;

    public Color Tint;

    public SpriteFlip Flip;

    public Sprite(Texture texture)
    {
        Texture = texture;
        SourceRectangle = null;
        Tint = Color.White;
        Flip = SpriteFlip.None;
    }

    protected internal override void Draw()
    {
        base.Draw();

        Graphics.Renderer2D.Draw(Texture, Transform.Position, SourceRectangle, Tint,
            Transform.Rotation.ToEulerAngles().Z, Transform.Origin.ToVector2(), Transform.Scale.ToVector2(), Flip);
    }
}