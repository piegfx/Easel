using System.Numerics;
using Easel.Math;

namespace Easel.Graphics;

public struct DropShadow
{
    public Color Color;

    public Vector2T<float> Offset;

    public DropShadow(Color color, Vector2T<float> offset)
    {
        Color = color;
        Offset = offset;
    }
}