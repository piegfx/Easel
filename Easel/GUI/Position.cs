using System;
using System.Numerics;
using Easel.Math;

namespace Easel.GUI;

public struct Position
{
    /// <summary>
    /// The anchor point of the element.
    /// </summary>
    public Anchor Anchor;

    /// <summary>
    /// The offset from the anchor point of the element.
    /// </summary>
    public Vector2T<int> Offset;

    public Position(Anchor anchor, Vector2T<int> offset)
    {
        Anchor = anchor;
        Offset = offset;
    }

    public Position(Anchor anchor) : this(anchor, Vector2T<int>.Zero) { }

    public Position(Vector2T<int> offset) : this(Anchor.TopLeft, offset) { }

    /// <summary>
    /// Calculate the screen position of the UI element.
    /// </summary>
    /// <param name="viewport">The bounds that the element will abide by.</param>
    /// <param name="elementSize">The size in pixels of this UI element.</param>
    /// <returns>The calculated position.</returns>
    public readonly Vector2T<int> CalculatePosition(Rectangle<int> viewport, Size<int> elementSize)
    {
        float scale = UI.Scale;
        
        Vector2T<int> pos = Vector2T<int>.Zero;

        elementSize = new Size<int>((int) (elementSize.Width * scale), (int) (elementSize.Height * scale));

        switch (Anchor)
        {
            case Anchor.TopLeft:
                break;
            case Anchor.TopCenter:
                pos.X = (viewport.Width / 2) - (elementSize.Width / 2);
                break;
            case Anchor.TopRight:
                pos.X = viewport.Width - elementSize.Width;
                break;
            case Anchor.CenterLeft:
                pos.Y = (viewport.Height / 2) - (elementSize.Height / 2);
                break;
            case Anchor.CenterCenter:
                pos.X = (viewport.Width / 2) - (elementSize.Width / 2);
                pos.Y = (viewport.Height / 2) - (elementSize.Height / 2);
                break;
            case Anchor.CenterRight:
                pos.X = viewport.Width - elementSize.Width;
                pos.Y = (viewport.Height / 2) - (elementSize.Height / 2);
                break;
            case Anchor.BottomLeft:
                pos.Y = viewport.Height - elementSize.Height;
                break;
            case Anchor.BottomCenter:
                pos.X = (viewport.Width / 2) - (elementSize.Width / 2);
                pos.Y = viewport.Height - elementSize.Height;
                break;
            case Anchor.BottomRight:
                pos.X = viewport.Width - elementSize.Width;
                pos.Y = viewport.Height - elementSize.Height;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        pos += (Offset.As<float>() * scale).As<int>();

        return pos;
    }
}