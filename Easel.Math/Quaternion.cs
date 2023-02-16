using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Quaternion<T> where T : INumber<T>
{
    public static Quaternion<T> Identity => new Quaternion<T>(T.Zero, T.Zero, T.Zero, T.One);

    [XmlAttribute]
    public T X;

    [XmlAttribute]
    public T Y;

    [XmlAttribute]
    public T Z;

    [XmlAttribute]
    public T W;

    public Quaternion(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public override string ToString()
    {
        return "Quaternion<" + typeof(T) + ">(X: " + X + ", Y: " + Y + ", Z: " + Z + ", W: " + W + ")";
    }
}

public static class Quaternion
{
    public static Quaternion<T> FromEuler<T>(Vector3T<T> euler) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        return FromEuler(euler.X, euler.Y, euler.Z);
    }

    public static Quaternion<T> FromEuler<T>(T yaw, T pitch, T roll) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        T half = T.CreateChecked(0.5);
        T cy = T.Cos(yaw * half);
        T sy = T.Sin(yaw * half);
        T cp = T.Cos(pitch * half);
        T sp = T.Sin(pitch * half);
        T cr = T.Cos(roll * half);
        T sr = T.Sin(roll * half);

        T x = cr * sp * cy + sr * cp * sy;
        T y = cr * cp * sy - sr * sp * cy;
        T z = sr * cp * cy - cr * sp * sy;
        T w = cr * cp * cy + sr * sp * sy;
        return new Quaternion<T>(x, y, z, w);
    }
}