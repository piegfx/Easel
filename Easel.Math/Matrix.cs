using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct MatrixT<T> where T : INumber<T>
{
    public static MatrixT<T> Identity =>
        new MatrixT<T>(Vector4T<T>.UnitX, Vector4T<T>.UnitY, Vector4T<T>.UnitZ, Vector4T<T>.UnitW);
    
    public Vector4T<T> Row0;

    public Vector4T<T> Row1;

    public Vector4T<T> Row2;

    public Vector4T<T> Row3;

    public MatrixT(Vector4T<T> row0, Vector4T<T> row1, Vector4T<T> row2, Vector4T<T> row3)
    {
        Row0 = row0;
        Row1 = row1;
        Row2 = row2;
        Row3 = row3;
    }

    #region Operators

    public static MatrixT<T> operator *(MatrixT<T> left, MatrixT<T> right)
    {
        Vector4T<T> rightColumn0 = new Vector4T<T>(right.Row0.X, right.Row1.X, right.Row2.X, right.Row3.X);
        Vector4T<T> rightColumn1 = new Vector4T<T>(right.Row0.Y, right.Row1.Y, right.Row2.Y, right.Row3.Y);
        Vector4T<T> rightColumn2 = new Vector4T<T>(right.Row0.Z, right.Row1.Z, right.Row2.Z, right.Row3.Z);
        Vector4T<T> rightColumn3 = new Vector4T<T>(right.Row0.W, right.Row1.W, right.Row2.W, right.Row3.W);
        
        Vector4T<T> row0 = new Vector4T<T>(Vector4T.Dot(left.Row0, rightColumn0), Vector4T.Dot(left.Row0, rightColumn1), Vector4T.Dot(left.Row0, rightColumn2), Vector4T.Dot(left.Row0, rightColumn3));
        Vector4T<T> row1 = new Vector4T<T>(Vector4T.Dot(left.Row1, rightColumn0), Vector4T.Dot(left.Row1, rightColumn1), Vector4T.Dot(left.Row1, rightColumn2), Vector4T.Dot(left.Row1, rightColumn3));
        Vector4T<T> row2 = new Vector4T<T>(Vector4T.Dot(left.Row2, rightColumn0), Vector4T.Dot(left.Row2, rightColumn1), Vector4T.Dot(left.Row2, rightColumn2), Vector4T.Dot(left.Row2, rightColumn3));
        Vector4T<T> row3 = new Vector4T<T>(Vector4T.Dot(left.Row3, rightColumn0), Vector4T.Dot(left.Row3, rightColumn1), Vector4T.Dot(left.Row3, rightColumn2), Vector4T.Dot(left.Row3, rightColumn3));

        return new MatrixT<T>(row0, row1, row2, row3);
    }

    #endregion

    public Vector3T<T> Translation => Row3.XYZ;

    public override string ToString()
    {
        return "MatrixT<" + typeof(T) + ">(Row0: " + Row0 + ", Row1: " + Row1 + ", Row2: " + Row2 + ", Row3: " + Row3 + ")";
    }
}

public static class MatrixT
{
    public static MatrixT<T> Translate<T>(Vector3T<T> translation) where T : INumber<T>
    {
        return MatrixT<T>.Identity with { Row3 = new Vector4T<T>(translation, T.CreateChecked(1)) };
    }
    
    public static MatrixT<T> Scale<T>(Vector3T<T> scale) where T : INumber<T>
    {
        MatrixT<T> result = MatrixT<T>.Identity;
        result.Row0.X = scale.X;
        result.Row1.Y = scale.Y;
        result.Row2.Z = scale.Z;
        return result;
    }
    
    public static MatrixT<TFloat> RotateX<TFloat>(TFloat rotation)
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);
        
        MatrixT<TFloat> result = MatrixT<TFloat>.Identity;
        result.Row1.Y = cosTheta;
        result.Row1.Z = sinTheta;
        result.Row2.Y = -sinTheta;
        result.Row2.Z = cosTheta;
        return result;
    }
    
    public static MatrixT<TFloat> RotateY<TFloat>(TFloat rotation)
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);
        
        MatrixT<TFloat> result = MatrixT<TFloat>.Identity;
        result.Row0.X = cosTheta;
        result.Row0.Z = -sinTheta;
        result.Row2.X = sinTheta;
        result.Row2.Z = cosTheta;
        return result;
    }
    
    public static MatrixT<TFloat> RotateZ<TFloat>(TFloat rotation) 
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);

        MatrixT<TFloat> result = MatrixT<TFloat>.Identity;
        result.Row0.X = cosTheta;
        result.Row0.Y = sinTheta;
        result.Row1.X = -sinTheta;
        result.Row1.Y = cosTheta;
        return result;
    }

    public static MatrixT<T> FromQuaternion<T>(QuaternionT<T> quaternion) where T : INumber<T>
    {
        T x = quaternion.X;
        T y = quaternion.Y;
        T z = quaternion.Z;
        T w = quaternion.W;

        T one = T.One;
        T two = T.CreateChecked(2);
        
        //Vector4T<T> row0 = new Vector4T<T>(one - two * y * y - two * z * z, two * x * y - two * z * w, two * x * z + two * y * w, T.Zero);
        //Vector4T<T> row1 = new Vector4T<T>(two * x * y + two * z * w, one - two * x * x - two * z * z, two * y * z - two * x * w, T.Zero);
        //Vector4T<T> row2 = new Vector4T<T>(two * x * z + two * y * w, two * y * z + two * x * w, one - two * x * x - two * y * y, T.Zero);
        
        Vector4T<T> row0 = new Vector4T<T>(one - two * y * y - two * z * z, two * x * y + two * z * w, two * x * z - two * y * w, T.Zero);
        Vector4T<T> row1 = new Vector4T<T>(two * x * y - two * z * w, one - two * x * x - two * z * z, two * y * z + two * x * w, T.Zero);
        Vector4T<T> row2 = new Vector4T<T>(two * x * z + two * y * w, two * y * z - two * x * w, one - two * x * x - two * y * y, T.Zero);
        Vector4T<T> row3 = Vector4T<T>.UnitW;

        return new MatrixT<T>(row0, row1, row2, row3);
    }
}