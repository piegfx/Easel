using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2T<T> : IEquatable<Vector2T<T>> where T : INumber<T>
{
    public static Vector2T<T> Zero => new Vector2T<T>(T.Zero);

    public static Vector2T<T> One => new Vector2T<T>(T.One);

    public static Vector2T<T> UnitX => new Vector2T<T>(T.One, T.Zero);

    public static Vector2T<T> UnitY => new Vector2T<T>(T.Zero, T.One);

    [XmlAttribute]
    public T X;

    [XmlAttribute]
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
    
    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator +(Vector2T<T> left, Vector2T<T> right) =>
        new Vector2T<T>(left.X + right.X, left.Y + right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator -(Vector2T<T> left, Vector2T<T> right) =>
        new Vector2T<T>(left.X - right.X, left.Y - right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator -(Vector2T<T> negate)
    {
        return new Vector2T<T>(-negate.X, -negate.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator *(Vector2T<T> left, Vector2T<T> right) =>
        new Vector2T<T>(left.X * right.X, left.Y * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator *(Vector2T<T> left, T right) =>
        new Vector2T<T>(left.X * right, left.Y * right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator *(T left, Vector2T<T> right) =>
        new Vector2T<T>(left * right.X, left * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator /(Vector2T<T> left, Vector2T<T> right) =>
        new Vector2T<T>(left.X / right.X, left.Y / right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator /(Vector2T<T> left, T right) =>
        new Vector2T<T>(left.X / right, left.Y / right);

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2T<TOther> As<TOther>() where TOther : INumber<TOther> =>
        Vector2T.Cast<T, TOther>(this);

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

    public static explicit operator Vector2(Vector2T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        return new Vector2(x, y);
    }
    
    public static explicit operator Vector2T<T>(Vector2 vector)
    {
        return new Vector2T<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));
    }
    
    #region Quick conversions

    public static explicit operator Vector2T<float>(Vector2T<T> value)
    {
        return Vector2T.Cast<T, float>(value);
    }

    public static explicit operator Vector2T<int>(Vector2T<T> value)
    {
        return Vector2T.Cast<T, int>(value);
    }

    public static explicit operator Vector2T<double>(Vector2T<T> value)
    {
        return Vector2T.Cast<T, double>(value);
    }
    
    #endregion
}

public static class Vector2T
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<TOther> Cast<T, TOther>(Vector2T<T> vector) where T : INumber<T> where TOther : INumber<TOther>
    {
        return new Vector2T<TOther>(TOther.CreateChecked(vector.X), TOther.CreateChecked(vector.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(vector));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(Vector2T<T> vector) where T : INumber<T>
    {
        return Dot(vector, vector);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector2T<T> left, Vector2T<T> right) where T : INumber<T>
    {
        return T.CreateChecked(left.X * right.X + left.Y * right.Y);
    }

    public static Vector2T<T> Lerp<T>(Vector2T<T> start, Vector2T<T> end, T value) where T : INumber<T>
    {
        T x = EaselMath.Lerp(start.X, end.X, value);
        T y = EaselMath.Lerp(start.Y, end.Y, value);

        return new Vector2T<T>(x, y);
    }
}