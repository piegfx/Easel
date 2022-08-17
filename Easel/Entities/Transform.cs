using System.Numerics;

namespace Easel.Entities;

public sealed class Transform
{
    public Vector3 Position;

    public Quaternion Rotation;

    public Vector3 Scale;

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }
}