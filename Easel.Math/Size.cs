using System;
using System.Numerics;

namespace Easel.Math;

public struct Size : IEquatable<Size>
{
    public static readonly Size Zero = new Size(0, 0);
    
    public int Width;
    
    public int Height;

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public Size(int wh)
    {
        Width = wh;
        Height = wh;
    }
    
    public bool Equals(Size other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object obj)
    {
        return obj is Size other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(Size left, Size right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size left, Size right)
    {
        return !left.Equals(right);
    }

    public static Size operator +(Size left, Size right)
    {
        return new Size(left.Width + right.Width, left.Height + right.Height);
    }

    public static Size operator -(Size left, Size right)
    {
        return new Size(left.Width - right.Width, left.Height - right.Height);
    }

    public static Size operator *(Size left, Size right)
    {
        return new Size(left.Width * right.Width, left.Height * right.Height);
    }

    public static Size operator *(Size left, float right)
    {
        return new Size((int) (left.Width * right), (int) (left.Height * right));
    }

    public static Size operator /(Size left, Size right)
    {
        return new Size((int) (left.Width / (float) right.Width), (int) (left.Height / (float) right.Height));
    }

    public static Size operator /(Size left, float right)
    {
        return new Size((int) (left.Width / right), (int) (left.Height / right));
    }
    
    public static explicit operator Vector2(Size size) => new Vector2(size.Width, size.Height);

    public static explicit operator Size(Vector2 vector) => new Size((int) vector.X, (int) vector.Y);

    public static explicit operator System.Drawing.Size(Size size) => new System.Drawing.Size(size.Width, size.Height);

    public static explicit operator Size(System.Drawing.Size size) => new Size(size.Width, size.Height);

    public override string ToString()
    {
        return Width + "x" + Height;
    }
}