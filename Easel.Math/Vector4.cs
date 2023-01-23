using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4<T> where T : INumber<T>
{
    public static Vector4<T> Zero => new Vector4<T>(T.CreateChecked(0));

    public static Vector4<T> One => new Vector4<T>(T.CreateChecked(1));

    public static Vector4<T> UnitX =>
        new Vector4<T>(T.CreateChecked(1), T.CreateChecked(0), T.CreateChecked(0), T.CreateChecked(0));
    
    public static Vector4<T> UnitY =>
        new Vector4<T>(T.CreateChecked(0), T.CreateChecked(1), T.CreateChecked(0), T.CreateChecked(0));
    
    public static Vector4<T> UnitZ =>
        new Vector4<T>(T.CreateChecked(0), T.CreateChecked(0), T.CreateChecked(1), T.CreateChecked(0));
    
    public static Vector4<T> UnitW =>
        new Vector4<T>(T.CreateChecked(0), T.CreateChecked(0), T.CreateChecked(0), T.CreateChecked(1));

    public T X;

    public T Y;

    public T Z;

    public T W;

    public Vector4(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
        W = scalar;
    }

    public Vector4(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
}

public static class Vector4
{
    
}