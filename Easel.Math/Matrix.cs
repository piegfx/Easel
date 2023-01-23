using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Matrix<T> where T : INumber<T>
{
    public static Matrix<T> Identity =>
        new Matrix<T>(Vector4T<T>.UnitX, Vector4T<T>.UnitY, Vector4T<T>.UnitZ, Vector4T<T>.UnitW);
    
    public Vector4T<T> Row0;

    public Vector4T<T> Row1;

    public Vector4T<T> Row2;

    public Vector4T<T> Row3;

    public Matrix(Vector4T<T> row0, Vector4T<T> row1, Vector4T<T> row2, Vector4T<T> row3)
    {
        Row0 = row0;
        Row1 = row1;
        Row2 = row2;
        Row3 = row3;
    }

    #region Operators

    public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
    {
        Vector4T<T> rightColumn0 = new Vector4T<T>(right.Row0.X, right.Row1.X, right.Row2.X, right.Row3.X);
        Vector4T<T> rightColumn1 = new Vector4T<T>(right.Row0.Y, right.Row1.Y, right.Row2.Y, right.Row3.Y);
        Vector4T<T> rightColumn2 = new Vector4T<T>(right.Row0.Z, right.Row1.Z, right.Row2.Z, right.Row3.Z);
        Vector4T<T> rightColumn3 = new Vector4T<T>(right.Row0.W, right.Row1.W, right.Row2.W, right.Row3.W);
        
        Vector4T<T> row0 = new Vector4T<T>(Vector4T.Dot(left.Row0, rightColumn0), Vector4T.Dot(left.Row0, rightColumn1), Vector4T.Dot(left.Row0, rightColumn2), Vector4T.Dot(left.Row0, rightColumn3));
        Vector4T<T> row1 = new Vector4T<T>(Vector4T.Dot(left.Row1, rightColumn0), Vector4T.Dot(left.Row1, rightColumn1), Vector4T.Dot(left.Row1, rightColumn2), Vector4T.Dot(left.Row1, rightColumn3));
        Vector4T<T> row2 = new Vector4T<T>(Vector4T.Dot(left.Row2, rightColumn0), Vector4T.Dot(left.Row2, rightColumn1), Vector4T.Dot(left.Row2, rightColumn2), Vector4T.Dot(left.Row2, rightColumn3));
        Vector4T<T> row3 = new Vector4T<T>(Vector4T.Dot(left.Row3, rightColumn0), Vector4T.Dot(left.Row3, rightColumn1), Vector4T.Dot(left.Row3, rightColumn2), Vector4T.Dot(left.Row3, rightColumn3));

        return new Matrix<T>(row0, row1, row2, row3);
    }

    #endregion

    public Vector3T<T> Translation => Row3.XYZ;

    public override string ToString()
    {
        return "Matrix<" + typeof(T) + ">(Row0: " + Row0 + ", Row1: " + Row1 + ", Row2: " + Row2 + ", Row3: " + Row3 + ")";
    }
}

public static class Matrix
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix<T> Translate<T>(Vector3T<T> translation) where T : INumber<T>
    {
        return Matrix<T>.Identity with { Row3 = new Vector4T<T>(translation, T.CreateChecked(1)) };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix<T> Scale<T>(Vector3T<T> scale) where T : INumber<T>
    {
        Matrix<T> result = Matrix<T>.Identity;
        result.Row0.X = scale.X;
        result.Row1.Y = scale.Y;
        result.Row2.Z = scale.Z;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix<TFloat> RotateX<TFloat>(TFloat rotation)
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);
        
        Matrix<TFloat> result = Matrix<TFloat>.Identity;
        result.Row1.Y = cosTheta;
        result.Row1.Z = sinTheta;
        result.Row2.Y = -sinTheta;
        result.Row2.Z = cosTheta;
        return result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix<TFloat> RotateY<TFloat>(TFloat rotation)
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);
        
        Matrix<TFloat> result = Matrix<TFloat>.Identity;
        result.Row0.X = cosTheta;
        result.Row0.Z = -sinTheta;
        result.Row2.X = sinTheta;
        result.Row2.Z = cosTheta;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix<TFloat> RotateZ<TFloat>(TFloat rotation) 
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);

        Matrix<TFloat> result = Matrix<TFloat>.Identity;
        result.Row0.X = cosTheta;
        result.Row0.Y = sinTheta;
        result.Row1.X = -sinTheta;
        result.Row1.Y = cosTheta;
        return result;
    }
}