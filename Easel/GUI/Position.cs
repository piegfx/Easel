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
    public Point Offset;

    public Position(Anchor anchor, Point offset)
    {
        Anchor = anchor;
        Offset = offset;
    }

    public Position(Anchor anchor) : this(anchor, Point.Zero) { }

    public Position(Point offset) : this(Anchor.TopLeft, Point.Zero) { }

    /// <summary>
    /// Calculate the screen position of the UI element.
    /// </summary>
    /// <param name="viewport">The bounds that the element will abide by.</param>
    /// <param name="elementSize">The size in pixels of this UI element.</param>
    /// <returns>The calculated position.</returns>
    public Point CalculatePosition(Rectangle viewport, Size elementSize)
    {
        Point pos = Point.Zero;

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

        pos += Offset;

        return pos;
    }
}