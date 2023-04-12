using System;
using System.Numerics;
using Easel.Core;
using Easel.Math;

namespace Easel.Entities;

/// <summary>
/// Describes position, rotation, and scale, and provides some helper functions and properties for determining things such
/// as the forward vector and model matrix. Typically used in entities, however can be used anywhere.
/// </summary>
public sealed class Transform : IEquatable<Transform>, ICloneable
{
    /// <summary>
    /// The position of this transform. (Default: Zero)
    /// </summary>
    public Vector3T<float> Position;

    /// <summary>
    /// The rotation of this transform. (Default: Identity)
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// The scale of this transform. (Default: One)
    /// </summary>
    public Vector3T<float> Scale;

    public Vector3T<float> Origin;

    /// <summary>
    /// The Sprite (Z) rotation of this transform. This is helpful when dealing with 2D objects.
    /// </summary>
    public float SpriteRotation
    {
        get => Rotation.ToEulerAngles().Z;
        set => Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, value);
    }

    /// <summary>
    /// Create a new default transform.
    /// </summary>
    public Transform()
    {
        Position = Vector3T<float>.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3T<float>.One;
        Origin = Vector3T<float>.Zero;
    }

    /// <summary>
    /// The forward vector of this transform.
    /// </summary>
    public Vector3T<float> Forward => (Vector3T<float>) Vector3.Transform(-Vector3.UnitZ, Rotation);

    /// <summary>
    /// The backward vector of this transform.
    /// </summary>
    public Vector3T<float> Backward => (Vector3T<float>) Vector3.Transform(Vector3.UnitZ, Rotation);

    /// <summary>
    /// The right vector of this transform.
    /// </summary>
    public Vector3T<float> Right => (Vector3T<float>) Vector3.Transform(Vector3.UnitX, Rotation);

    /// <summary>
    /// The left vector of this transform.
    /// </summary>
    public Vector3T<float> Left => (Vector3T<float>) Vector3.Transform(-Vector3.UnitX, Rotation);

    /// <summary>
    /// The up vector of this transform.
    /// </summary>
    public Vector3T<float> Up => (Vector3T<float>) Vector3.Transform(Vector3.UnitY, Rotation);

    /// <summary>
    /// The down vector of this transform.
    /// </summary>
    public Vector3T<float> Down => (Vector3T<float>) Vector3.Transform(-Vector3.UnitY, Rotation);

    /// <summary>
    /// Calculates and returns the matrix for this transform.
    /// </summary>
    public Matrix4x4 TransformMatrix => Matrix4x4.CreateTranslation(-(Vector3) Origin) *
                                        Matrix4x4.CreateScale((Vector3) Scale) *
                                        Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(Rotation)) *
                                        Matrix4x4.CreateTranslation((Vector3) Position);

    public void RotateAroundLocalPoint(Vector3T<float> point, Vector3T<float> axis, float angle)
    {
        Quaternion rotation = Quaternion.CreateFromAxisAngle((Vector3) axis, angle);
        Rotation *= rotation;
        Position = (Vector3T<float>) Vector3.Transform((Vector3) Position, Rotation) + point;
    }

    public bool Equals(Transform other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Position.Equals(other.Position) && Rotation.Equals(other.Rotation) && Scale.Equals(other.Scale);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is Transform other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Rotation, Scale);
    }

    public object Clone()
    {
        return new Transform()
        {
            Position = Position,
            Rotation = Rotation,
            Scale = Scale
        };
    }

    public static bool operator ==(Transform left, Transform right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Transform left, Transform right)
    {
        return !Equals(left, right);
    }

    public static Transform Lerp(Transform a, Transform b, float amount)
    {
        return new Transform()
        {
            Position = Vector3T.Lerp(a.Position, b.Position, amount),
            Rotation = Quaternion.Lerp(a.Rotation, b.Rotation, amount),
            Scale = Vector3T.Lerp(a.Scale, b.Scale, amount),
            Origin = Vector3T.Lerp(a.Origin, b.Origin, amount)
        };
    }
}