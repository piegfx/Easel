using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Math;

/// <summary>
/// Represents an integer-based 3D vector.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vector3I
{
    public int X;

    public int Y;

    public int Z;

    public Vector3I(int xyz)
    {
        X = xyz;
        Y = xyz;
        Z = xyz;
    }
    
    public Vector3I(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static explicit operator Vector3(Vector3I value) => new Vector3(value.X, value.Y, value.Z);

    public static explicit operator Vector3I(Vector3 value) =>
        new Vector3I((int) value.X, (int) value.Y, (int) value.Z);

    public static Vector3 operator +(Vector3I value1, Vector3 value2) =>
        new Vector3(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);

    public static Vector3I operator +(Vector3I value1, Vector3I value2) =>
        new Vector3I(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
}