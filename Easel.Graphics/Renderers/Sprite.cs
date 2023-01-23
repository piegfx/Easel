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
    public Vector2 Origin;
    public Vector2 Scale;
    public SpriteFlip Flip;

    public Sprite(Texture texture, Vector3 position, Rectangle<int>? source, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteFlip flip)
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