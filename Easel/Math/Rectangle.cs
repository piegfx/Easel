using System;

namespace Easel.Math;

public struct Rectangle : IEquatable<Rectangle>
{
    public int X;
    
    public int Y;
    
    public int Width;
    
    public int Height;

    public Point Location => new Point(X, Y);

    public Size Size => new Size(Width, Height);

    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Rectangle(Point location, Size size) : this(location.X, location.Y, size.Width, size.Height) { }

    public bool Equals(Rectangle other)
    {
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object obj)
    {
        return obj is Rectangle other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    public static bool operator ==(Rectangle left, Rectangle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rectangle left, Rectangle right)
    {
        return !left.Equals(right);
    }

    public static explicit operator System.Drawing.Rectangle(Rectangle rectangle) =>
        new System.Drawing.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

    public static explicit operator Rectangle(System.Drawing.Rectangle rectangle) =>
        new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

    public override string ToString()
    {
        return "Rectangle(X: " + X + ", Y: " + Y + ", Width: " + Width + ", Height: " + Height + ")";
    }
}