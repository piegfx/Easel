using System.Numerics;
using Easel.Core;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.Entities.Components;

public class Sprite : Component
{
    public Texture Texture;

    public Rectangle<int>? SourceRectangle;

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

        Graphics.Renderer.DrawSprite(new Graphics.Renderers.Sprite(Texture, (Vector3) Transform.Position, SourceRectangle,
            Tint, Transform.SpriteRotation, Transform.Origin.ToVector2(), Transform.Scale.ToVector2(), Flip));
    }
}