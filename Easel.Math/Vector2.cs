using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2<T> : IEquatable<Vector2<T>> where T : INumber<T>
{
    public T X;

    public T Y;

    public Vector2(T scalar)
    {
        X = scalar;
        Y = scalar;
    }

    public Vector2(T x, T y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Vector2<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector2<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2<T> left, Vector2<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2<T> left, Vector2<T> right)
    {
        return !left.Equals(right);
    }
}