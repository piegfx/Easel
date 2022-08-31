using System;
using System.Numerics;

namespace Easel.Math;

public struct Point : IEquatable<Point>
{
    public static readonly Point Zero = new Point(0, 0);
    
    public int X;
    
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point(int xy)
    {
        X = xy;
        Y = xy;
    }
    
    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point left, Point right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right)
    {
        return !left.Equals(right);
    }

    public static Point operator +(Point left, Point right)
    {
        return new Point(left.X + right.X, left.Y + right.Y);
    }

    public static Point operator -(Point left, Point right)
    {
        return new Point(left.X - right.X, left.Y - right.Y);
    }

    public static Point operator *(Point left, Point right)
    {
        return new Point(left.X * right.X, left.Y * right.Y);
    }

    public static Point operator *(Point left, float right)
    {
        return new Point((int) (left.X * right), (int) (left.Y * right));
    }

    public static Point operator /(Point left, Point right)
    {
        return new Point(left.X / right.X, left.Y / right.Y);
    }

    public static Point operator /(Point left, float right)
    {
        return new Point((int) (left.X / right), (int) (left.Y / right));
    }
    
    public static explicit operator Vector2(Point point) => new Vector2(point.X, point.Y);

    public static explicit operator Point(Vector2 vector) => new Point((int) vector.X, (int) vector.Y);
    
    public static explicit operator System.Drawing.Point(Point point) => new System.Drawing.Point(point.X, point.Y);

    public override string ToString()
    {
        return "Point(X: " + X + ", Y: " + Y + ")";
    }
}