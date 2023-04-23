using System.Numerics;
using Easel.Math;

namespace Easel.Graphics;

public struct DropShadow
{
    public Color Color;

    public Vector2 Offset;

    public DropShadow(Color color, Vector2 offset)
    {
        Color = color;
        Offset = offset;
    }
}