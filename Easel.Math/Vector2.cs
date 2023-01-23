using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2T<T> : IEquatable<Vector2T<T>> where T : INumber<T>
{
    public static Vector2T<T> Zero => new Vector2T<T>(T.CreateChecked(0));

    public static Vector2T<T> One => new Vector2T<T>(T.CreateChecked(1));

    public static Vector2T<T> UnitX => new Vector2T<T>(T.CreateChecked(1), T.CreateChecked(0));

    public static Vector2T<T> UnitY => new Vector2T<T>(T.CreateChecked(0), T.CreateChecked(1));

    public T X;

    public T Y;

    public Vector2T(T scalar)
    {
        X = scalar;
        Y = scalar;
    }

    public Vector2T(T x, T y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Vector2T<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector2T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2T<T> left, Vector2T<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2T<T> left, Vector2T<T> right)
    {
        return !left.Equals(right);
    }

    public static implicit operator Vector2(Vector2T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        return new Vector2(x, y);
    }
    
    public static implicit operator Vector2T<T>(Vector2 vector)
    {
        return new Vector2T<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));
    }
}

public static class Vector2T
{
}