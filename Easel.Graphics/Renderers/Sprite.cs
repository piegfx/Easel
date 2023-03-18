using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public struct Sprite
{
    public Texture Texture;
    public Vector3 Position;
    public Rectangle<int>? Source;
    public Color Tint;
    public float Rotation;
    public Vector2T<float> Origin;
    public Vector2T<float> Scale;
    public SpriteFlip Flip;

    public Sprite(Texture texture, Vector3 position, Rectangle<int>? source, Color tint, float rotation, Vector2T<float> origin, Vector2T<float> scale, SpriteFlip flip)
    {
        Texture = texture;
        Position = position;
        Source = source;
        Tint = tint;
        Rotation = rotation;
        Origin = origin;
        Scale = scale;
        Flip = flip;
    }
}