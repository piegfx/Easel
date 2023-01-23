using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Matrix<T> where T : INumber<T>
{
    public static Matrix<T> Identity =>
        new Matrix<T>(Vector4<T>.UnitX, Vector4<T>.UnitY, Vector4<T>.UnitZ, Vector4<T>.UnitW);
    
    public Vector4<T> Row0;

    public Vector4<T> Row1;

    public Vector4<T> Row2;

    public Vector4<T> Row3;

    public Matrix(Vector4<T> row0, Vector4<T> row1, Vector4<T> row2, Vector4<T> row3)
    {
        Row0 = row0;
        Row1 = row1;
        Row2 = row2;
        Row3 = row3;
    }
}