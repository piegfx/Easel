using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2T<T> : IEquatable<Vector2T<T>> where T : INumber<T>
{
    public static Vector2T<T> Zero => new Vector2T<T>(T.Zero, T.Zero);

    public static Vector2T<T> One => new Vector2T<T>(T.One, T.One);

    public static Vector2T<T> UnitX => new Vector2T<T>(T.One, T.Zero);

    public static Vector2T<T> UnitY => new Vector2T<T>(T.Zero, T.One);

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

    public static Vector2T<T> operator +(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X + right.X, left.Y + right.Y);
    }

    public static Vector2T<T> operator -(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X - right.X, left.Y - right.Y);
    }
    
    public static Vector2T<T> operator -(Vector2T<T> vector)
    {
        return new Vector2T<T>(-vector.X, -vector.Y);
    }

    public static Vector2T<T> operator *(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X * right.X, left.Y * right.Y);
    }

    public static Vector2T<T> operator *(Vector2T<T> left, T right)
    {
        return new Vector2T<T>(left.X * right, left.Y * right);
    }

    public static Vector2T<T> operator *(T left, Vector2T<T> right)
    {
        return new Vector2T<T>(left * right.X, left * right.Y);
    }

    public static Vector2T<T> operator /(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X / right.X, left.Y / right.Y);
    }

    public static Vector2T<T> operator /(Vector2T<T> left, T right)
    {
        return new Vector2T<T>(left.X / right, left.Y / right);
    }

    public static bool operator ==(Vector2T<T> left, Vector2T<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2T<T> left, Vector2T<T> right)
    {
        return !left.Equals(right);
    }

    public static explicit operator System.Numerics.Vector2(Vector2T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        return new Vector2(x, y);
    }

    public static explicit operator Vector2T<T>(System.Numerics.Vector2 vector)
    {
        T x = T.CreateChecked(vector.X);
        T y = T.CreateChecked(vector.Y);
        return new Vector2T<T>(x, y);
    }

    [MethodImpl(Vector2T.Options)]
    public readonly Vector2T<TOther> As<TOther>() where TOther : INumber<TOther>
    {
        TOther x = TOther.CreateChecked(X);
        TOther y = TOther.CreateChecked(Y);
        return new Vector2T<TOther>(x, y);
    }

    public override string ToString()
    {
        return $"Vector2T<{typeof(T).FullName}>(X: {X}, Y: {Y})";
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
}

public static class Vector2T
{
    internal const MethodImplOptions Options =
        MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
    
    [MethodImpl(Options)]
    public static T MagnitudeSquared<T>(in Vector2T<T> vector) where T : INumber<T>
    {
        return vector.X * vector.X + vector.Y * vector.Y;
    }

    [MethodImpl(Options)]
    public static T Magnitude<T>(in Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(vector));
    }

    [MethodImpl(Options)]
    public static void Normalize<T>(ref Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T magnitude = Magnitude(vector);
        vector.X /= magnitude;
        vector.Y /= magnitude;
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Normalize<T>(Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        Normalize(ref vector);
        return vector;
    }

    [MethodImpl(Options)]
    public static T Dot<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>
    {
        return T.CreateChecked(a.X * b.X + a.Y * b.Y);
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Abs<T>(in Vector2T<T> vector) where T : INumber<T>
    {
        return new Vector2T<T>(T.Abs(vector.X), T.Abs(vector.Y));
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Clamp<T>(in Vector2T<T> vector, in Vector2T<T> min, in Vector2T<T> max)
        where T : INumber<T>
    {
        T x = T.Clamp(vector.X, min.X, max.X);
        T y = T.Clamp(vector.Y, min.Y, max.Y);
        return new Vector2T<T>(x, y);
    }

    [MethodImpl(Options)]
    public static T DistanceSquared<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static T Distance<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>, IRootFunctions<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Lerp<T>(in Vector2T<T> a, in Vector2T<T> b, T amount) where T : INumber<T>
    {
        T x = EaselMath.Lerp(a.X, b.X, amount);
        T y = EaselMath.Lerp(a.X, b.Y, amount);
        return new Vector2T<T>(x, y);
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Max<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>
    {
        T x = T.Max(a.X, b.X);
        T y = T.Max(a.Y, b.Y);
        return new Vector2T<T>(x, y);
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Min<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>
    {
        T x = T.Min(a.X, b.X);
        T y = T.Max(a.Y, b.Y);
        return new Vector2T<T>(x, y);
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Reflect<T>(in Vector2T<T> surface, in Vector2T<T> normal) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Sqrt<T>(in Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T x = T.Sqrt(vector.X);
        T y = T.Sqrt(vector.Y);
        return new Vector2T<T>(x, y);
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Transform<T>(in Vector2T<T> vector, in MatrixT<T> matrix) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector2T<T> Transform<T>(in Vector2T<T> vector, in QuaternionT<T> quaternion) where T : INumber<T>
    {
        throw new NotImplementedException();
    }
}

public static class Vector2TExtensions
{
    [MethodImpl(Vector2T.Options)]
    public static T LengthSquared<T>(this Vector2T<T> vector) where T : INumber<T>
    {
        return Vector2T.MagnitudeSquared(vector);
    }

    [MethodImpl(Vector2T.Options)]
    public static T Length<T>(this Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return Vector2T.Magnitude(vector);
    }

    [MethodImpl(Vector2T.Options)]
    public static Vector2T<T> Normalize<T>(this Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return Vector2T.Normalize(vector);
    }
}