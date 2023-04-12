using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4T<T> : IEquatable<Vector4T<T>> where T : INumber<T>
{
    public static Vector4T<T> Zero => new Vector4T<T>(T.Zero, T.Zero, T.Zero, T.Zero);

    public static Vector4T<T> One => new Vector4T<T>(T.One, T.One, T.One, T.Zero);

    public static Vector4T<T> UnitX => new Vector4T<T>(T.One, T.Zero, T.Zero, T.Zero);

    public static Vector4T<T> UnitY => new Vector4T<T>(T.Zero, T.One, T.Zero, T.Zero);

    public static Vector4T<T> UnitZ => new Vector4T<T>(T.Zero, T.Zero, T.One, T.Zero);

    public static Vector4T<T> UnitW => new Vector4T<T>(T.Zero, T.Zero, T.Zero, T.One);

    public T X;
    public T Y;
    public T Z;
    public T W;

    public Vector4T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
        W = scalar;
    }

    public Vector4T(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4T(Vector2T<T> xy, T z, T w)
    {
        X = xy.X;
        Y = xy.Y;
        Z = z;
        W = w;
    }

    public Vector4T(Vector3T<T> xyz, T w)
    {
        X = xyz.X;
        Y = xyz.Y;
        Z = xyz.Z;
        W = w;
    }

    public static Vector4T<T> operator +(Vector4T<T> left, Vector4T<T> right)
    {
        return new Vector4T<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
    }

    public static Vector4T<T> operator -(Vector4T<T> left, Vector4T<T> right)
    {
        return new Vector4T<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
    }
    
    public static Vector4T<T> operator -(Vector4T<T> vector)
    {
        return new Vector4T<T>(-vector.X, -vector.Y, -vector.Z, -vector.W);
    }

    public static Vector4T<T> operator *(Vector4T<T> left, Vector4T<T> right)
    {
        return new Vector4T<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
    }

    public static Vector4T<T> operator *(Vector4T<T> left, T right)
    {
        return new Vector4T<T>(left.X * right, left.Y * right, left.Z * right, left.W * right);
    }

    public static Vector4T<T> operator *(T left, Vector4T<T> right)
    {
        return new Vector4T<T>(left * right.X, left * right.Y, left * right.Z, left * right.W);
    }

    public static Vector4T<T> operator /(Vector4T<T> left, Vector4T<T> right)
    {
        return new Vector4T<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
    }

    public static Vector4T<T> operator /(Vector4T<T> left, T right)
    {
        return new Vector4T<T>(left.X / right, left.Y / right, left.Z / right, left.W / right);
    }

    public static bool operator ==(Vector4T<T> left, Vector4T<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector4T<T> left, Vector4T<T> right)
    {
        return !left.Equals(right);
    }

    public static explicit operator System.Numerics.Vector4(Vector4T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        float z = Convert.ToSingle(vector.Z);
        float w = Convert.ToSingle(vector.W);
        return new Vector4(x, y, z, w);
    }

    public static explicit operator Vector4T<T>(System.Numerics.Vector4 vector)
    {
        T x = T.CreateChecked(vector.X);
        T y = T.CreateChecked(vector.Y);
        T z = T.CreateChecked(vector.Z);
        T w = T.CreateChecked(vector.W);
        return new Vector4T<T>(x, y, z, w);
    }

    [MethodImpl(Vector4T.Options)]
    public readonly Vector4T<TOther> As<TOther>() where TOther : INumber<TOther>
    {
        TOther x = TOther.CreateChecked(X);
        TOther y = TOther.CreateChecked(Y);
        TOther z = TOther.CreateChecked(Z);
        TOther w = TOther.CreateChecked(W);
        return new Vector4T<TOther>(x, y, z, w);
    }

    public override string ToString()
    {
        return $"Vector4T<{typeof(T).FullName}>(X: {X}, Y: {Y}, Z: {Z}, W: {W})";
    }

    public bool Equals(Vector4T<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y) &&
               EqualityComparer<T>.Default.Equals(Z, other.Z) && EqualityComparer<T>.Default.Equals(W, other.W);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector4T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }
}

public static class Vector4T
{
    internal const MethodImplOptions Options =
        MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
    
    [MethodImpl(Options)]
    public static T MagnitudeSquared<T>(in Vector4T<T> vector) where T : INumber<T>
    {
        return Dot(vector, vector);
    }

    [MethodImpl(Options)]
    public static T Magnitude<T>(in Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(vector));
    }

    [MethodImpl(Options)]
    public static void Normalize<T>(ref Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T magnitude = Magnitude(vector);
        vector.X /= magnitude;
        vector.Y /= magnitude;
        vector.Z /= magnitude;
        vector.W /= magnitude;
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Normalize<T>(Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        Normalize(ref vector);
        return vector;
    }

    [MethodImpl(Options)]
    public static T Dot<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>
    {
        return T.CreateChecked(a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W);
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Abs<T>(in Vector4T<T> vector) where T : INumber<T>
    {
        return new Vector4T<T>(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z), T.Abs(vector.W));
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Clamp<T>(in Vector4T<T> vector, in Vector4T<T> min, in Vector4T<T> max)
        where T : INumber<T>
    {
        T x = T.Clamp(vector.X, min.X, max.X);
        T y = T.Clamp(vector.Y, min.Y, max.Y);
        T z = T.Clamp(vector.Z, min.Z, max.Z);
        T w = T.Clamp(vector.W, min.W, max.W);
        return new Vector4T<T>(x, y, z, w);
    }

    [MethodImpl(Options)]
    public static T DistanceSquared<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>
    {
        Vector4T<T> res = a - b;
        return Dot(res, res);
    }

    [MethodImpl(Options)]
    public static T Distance<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(DistanceSquared(a, b));
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Lerp<T>(in Vector4T<T> a, in Vector4T<T> b, T amount) where T : INumber<T>
    {
        T x = EaselMath.Lerp(a.X, b.X, amount);
        T y = EaselMath.Lerp(a.Y, b.Y, amount);
        T z = EaselMath.Lerp(a.Z, b.Z, amount);
        T w = EaselMath.Lerp(a.W, b.W, amount);
        return new Vector4T<T>(x, y, z, w);
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Max<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>
    {
        T x = T.Max(a.X, b.X);
        T y = T.Max(a.Y, b.Y);
        T z = T.Max(a.Z, b.Z);
        T w = T.Max(a.W, b.W);
        return new Vector4T<T>(x, y, z, w);
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Min<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>
    {
        T x = T.Min(a.X, b.X);
        T y = T.Min(a.Y, b.Y);
        T z = T.Min(a.Z, b.Z);
        T w = T.Min(a.W, b.W);
        return new Vector4T<T>(x, y, z, w);
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Sqrt<T>(in Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T x = T.Sqrt(vector.X);
        T y = T.Sqrt(vector.Y);
        T z = T.Sqrt(vector.Z);
        T w = T.Sqrt(vector.W);
        return new Vector4T<T>(x, y, z, w);
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Transform<T>(in Vector4T<T> vector, in MatrixT<T> matrix) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector4T<T> Transform<T>(in Vector4T<T> vector, in QuaternionT<T> quaternion) where T : INumber<T>
    {
        throw new NotImplementedException();
    }
    
    [MethodImpl(Options)]
    public static Vector4T<T> TransformNormal<T>(in Vector4T<T> vector, in MatrixT<T> matrix) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector4T<T> TransformNormal<T>(in Vector4T<T> vector, in QuaternionT<T> quaternion) where T : INumber<T>
    {
        throw new NotImplementedException();
    }
}

public static class Vector4TExtensions
{
    [MethodImpl(Vector4T.Options)]
    public static T LengthSquared<T>(this Vector4T<T> vector) where T : INumber<T>
    {
        return Vector4T.MagnitudeSquared(vector);
    }

    [MethodImpl(Vector4T.Options)]
    public static T Length<T>(this Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return Vector4T.Magnitude(vector);
    }

    [MethodImpl(Vector4T.Options)]
    public static Vector4T<T> Normalize<T>(this Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return Vector4T.Normalize(vector);
    }
}