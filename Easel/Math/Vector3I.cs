using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

/// <summary>
/// Represents an integer-based 3D vector.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vector3I
{
    public static readonly Vector3I Zero = new Vector3I(0);

    public static readonly Vector3I One = new Vector3I(1);

    public static readonly Vector3I UnitX = new Vector3I(1, 0, 0);

    public static readonly Vector3I UnitY = new Vector3I(0, 1, 0);

    public static readonly Vector3I UnitZ = new Vector3I(0, 0, 1);
    
    [XmlAttribute]
    public int X;

    [XmlAttribute]
    public int Y;

    [XmlAttribute]
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

    public static Vector3I Clamp(Vector3I value, Vector3I min, Vector3I max)
    {
        value.X = EaselMath.Clamp(value.X, min.X, max.X);
        value.Y = EaselMath.Clamp(value.Y, min.Y, max.Y);
        value.Z = EaselMath.Clamp(value.Z, min.Z, max.Z);
        return value;
    }

    public static Vector3I Wrap(Vector3I value, Vector3I min, Vector3I max)
    {
        value.X = EaselMath.Wrap(value.X, min.X, max.X);
        value.Y = EaselMath.Wrap(value.Y, min.Y, max.Y);
        value.Z = EaselMath.Wrap(value.Z, min.Z, max.Z);
        return value;
    }

    public override string ToString()
    {
        return "Vector3I(X: " + X + ", Y: " + Y + ", Z: " + Z + ")";
    }
}