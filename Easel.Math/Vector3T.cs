using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector3T<T> : IEquatable<Vector3T<T>> where T : INumber<T>
{
    public static Vector3T<T> Zero => new Vector3T<T>(T.Zero, T.Zero, T.Zero);

    public static Vector3T<T> One => new Vector3T<T>(T.One, T.One, T.One);

    public static Vector3T<T> UnitX => new Vector3T<T>(T.One, T.Zero, T.Zero);

    public static Vector3T<T> UnitY => new Vector3T<T>(T.Zero, T.One, T.Zero);

    public static Vector3T<T> UnitZ => new Vector3T<T>(T.Zero, T.Zero, T.One);

    public T X;
    public T Y;
    public T Z;

    public Vector3T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
    }

    public Vector3T(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3T(Vector2T<T> xy, T z)
    {
        X = xy.X;
        Y = xy.Y;
        Z = z;
    }

    public static Vector3T<T> operator +(Vector3T<T> left, Vector3T<T> right)
    {
        return new Vector3T<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static Vector3T<T> operator -(Vector3T<T> left, Vector3T<T> right)
    {
        return new Vector3T<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
    
    public static Vector3T<T> operator -(Vector3T<T> vector)
    {
        return new Vector3T<T>(-vector.X, -vector.Y, -vector.Z);
    }

    public static Vector3T<T> operator *(Vector3T<T> left, Vector3T<T> right)
    {
        return new Vector3T<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    public static Vector3T<T> operator *(Vector3T<T> left, T right)
    {
        return new Vector3T<T>(left.X * right, left.Y * right, left.Z * right);
    }

    public static Vector3T<T> operator *(T left, Vector3T<T> right)
    {
        return new Vector3T<T>(left * right.X, left * right.Y, left * right.Z);
    }

    public static Vector3T<T> operator /(Vector3T<T> left, Vector3T<T> right)
    {
        return new Vector3T<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }

    public static Vector3T<T> operator /(Vector3T<T> left, T right)
    {
        return new Vector3T<T>(left.X / right, left.Y / right, left.Z / right);
    }

    public static bool operator ==(Vector3T<T> left, Vector3T<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector3T<T> left, Vector3T<T> right)
    {
        return !left.Equals(right);
    }

    public static explicit operator System.Numerics.Vector3(Vector3T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        float z = Convert.ToSingle(vector.Z);
        return new Vector3(x, y, z);
    }

    public static explicit operator Vector3T<T>(System.Numerics.Vector3 vector)
    {
        T x = T.CreateChecked(vector.X);
        T y = T.CreateChecked(vector.Y);
        T z = T.CreateChecked(vector.Z);
        return new Vector3T<T>(x, y, z);
    }

    [MethodImpl(Vector3T.Options)]
    public readonly Vector3T<TOther> As<TOther>() where TOther : INumber<TOther>
    {
        TOther x = TOther.CreateChecked(X);
        TOther y = TOther.CreateChecked(Y);
        TOther z = TOther.CreateChecked(Z);
        return new Vector3T<TOther>(x, y, z);
    }

    public override string ToString()
    {
        return $"Vector3T<{typeof(T).FullName}>(X: {X}, Y: {Y}, Z: {Z})";
    }

    public bool Equals(Vector3T<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y) &&
               EqualityComparer<T>.Default.Equals(Z, other.Z);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}

public static class Vector3T
{
    internal const MethodImplOptions Options =
        MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
    
    [MethodImpl(Options)]
    public static T MagnitudeSquared<T>(in Vector3T<T> vector) where T : INumber<T>
    {
        return Dot(vector, vector);
    }

    [MethodImpl(Options)]
    public static T Magnitude<T>(in Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(vector));
    }

    [MethodImpl(Options)]
    public static void Normalize<T>(ref Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T magnitude = Magnitude(vector);
        vector.X /= magnitude;
        vector.Y /= magnitude;
        vector.Z /= magnitude;
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Normalize<T>(Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        Normalize(ref vector);
        return vector;
    }

    [MethodImpl(Options)]
    public static T Dot<T>(in Vector3T<T> a, in Vector3T<T> b) where T : INumber<T>
    {
        return T.CreateChecked(a.X * b.X + a.Y * b.Y + a.Z * b.Z);
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Abs<T>(in Vector3T<T> vector) where T : INumber<T>
    {
        return new Vector3T<T>(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z));
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Clamp<T>(in Vector3T<T> vector, in Vector3T<T> min, in Vector3T<T> max)
        where T : INumber<T>
    {
        T x = T.Clamp(vector.X, min.X, max.X);
        T y = T.Clamp(vector.Y, min.Y, max.Y);
        T z = T.Clamp(vector.Z, min.Z, max.Z);
        return new Vector3T<T>(x, y, z);
    }

    [MethodImpl(Options)]
    public static T DistanceSquared<T>(in Vector3T<T> a, in Vector3T<T> b) where T : INumber<T>
    {
        Vector3T<T> res = a - b;
        return Dot(res, res);
    }

    [MethodImpl(Options)]
    public static T Distance<T>(in Vector3T<T> a, in Vector3T<T> b) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(DistanceSquared(a, b));
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Lerp<T>(in Vector3T<T> a, in Vector3T<T> b, T amount) where T : INumber<T>
    {
        T x = EaselMath.Lerp(a.X, b.X, amount);
        T y = EaselMath.Lerp(a.Y, b.Y, amount);
        T z = EaselMath.Lerp(a.Z, b.Z, amount);
        return new Vector3T<T>(x, y, z);
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Max<T>(in Vector3T<T> a, in Vector3T<T> b) where T : INumber<T>
    {
        T x = T.Max(a.X, b.X);
        T y = T.Max(a.Y, b.Y);
        T z = T.Max(a.Z, b.Z);
        return new Vector3T<T>(x, y, z);
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Min<T>(in Vector3T<T> a, in Vector3T<T> b) where T : INumber<T>
    {
        T x = T.Min(a.X, b.X);
        T y = T.Min(a.Y, b.Y);
        T z = T.Min(a.Z, b.Z);
        return new Vector3T<T>(x, y, z);
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Reflect<T>(in Vector3T<T> surface, in Vector3T<T> normal) where T : INumber<T>
    {
        return surface - (T.CreateChecked(2) * Dot(surface, normal)) * normal;
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Sqrt<T>(in Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T x = T.Sqrt(vector.X);
        T y = T.Sqrt(vector.Y);
        T z = T.Sqrt(vector.Z);
        return new Vector3T<T>(x, y, z);
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Cross<T>(in Vector3T<T> a, in Vector3T<T> b) where T : INumber<T>
    {
        return new Vector3T<T>(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Transform<T>(in Vector3T<T> vector, in MatrixT<T> matrix) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector3T<T> Transform<T>(in Vector3T<T> vector, in QuaternionT<T> quaternion) where T : INumber<T>
    {
        throw new NotImplementedException();
    }
    
    [MethodImpl(Options)]
    public static Vector3T<T> TransformNormal<T>(in Vector3T<T> vector, in MatrixT<T> matrix) where T : INumber<T>
    {
        throw new NotImplementedException();
    }

    [MethodImpl(Options)]
    public static Vector3T<T> TransformNormal<T>(in Vector3T<T> vector, in QuaternionT<T> quaternion) where T : INumber<T>
    {
        throw new NotImplementedException();
    }
}

public static class Vector3TExtensions
{
    [MethodImpl(Vector3T.Options)]
    public static T LengthSquared<T>(this Vector3T<T> vector) where T : INumber<T>
    {
        return Vector3T.MagnitudeSquared(vector);
    }

    [MethodImpl(Vector3T.Options)]
    public static T Length<T>(this Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return Vector3T.Magnitude(vector);
    }

    [MethodImpl(Vector3T.Options)]
    public static Vector3T<T> Normalize<T>(this Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return Vector3T.Normalize(vector);
    }
}