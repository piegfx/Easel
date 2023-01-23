using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector3<T> : IEquatable<Vector3<T>> where T : INumber<T>
{
    public static Vector3<T> Zero => new Vector3<T>(T.Zero);

    public static Vector3<T> One => new Vector3<T>(T.One);

    public static Vector3<T> UnitX => new Vector3<T>(T.One, T.Zero, T.Zero);

    public static Vector3<T> UnitY => new Vector3<T>(T.Zero, T.One, T.Zero);

    public static Vector3<T> UnitZ => new Vector3<T>(T.Zero, T.Zero, T.One);
    
    public T X;
    public T Y;
    public T Z;

    public Vector3(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
    }
    
    public Vector3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) =>
        new Vector3<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) =>
        new Vector3<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) =>
        new Vector3<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(Vector3<T> left, T right) =>
        new Vector3<T>(left.X * right, left.Y * right, left.Z * right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(T left, Vector3<T> right) =>
        new Vector3<T>(left * right.X, left * right.Y, left * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) =>
        new Vector3<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator /(Vector3<T> left, T right) =>
        new Vector3<T>(left.X / right, left.Y / right, left.Z / right);

    #endregion

    #region Swizzle
    
    // TODO: Swizzle auto generator

    #endregion

    public bool Equals(Vector3<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y) && EqualityComparer<T>.Default.Equals(Z, other.Z);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(Vector3<T> left, Vector3<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector3<T> left, Vector3<T> right)
    {
        return !left.Equals(right);
    }
}

public static class Vector3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(Vector3<T> value) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(Vector3<T> value) where T : INumber<T>, IRootFunctions<T>
    {
        return Dot(value, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector3<T> left, Vector3<T> right) where T : INumber<T>, IRootFunctions<T>
    {
        return T.CreateChecked(left.X * right.X + left.Y * right.Y + left.Z * right.Z);
    }
}