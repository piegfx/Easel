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

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Vector3 Backward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    public Vector3 Left => Vector3.Transform(-Vector3.UnitX, Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);
}