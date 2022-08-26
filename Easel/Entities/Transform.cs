using System.Numerics;

namespace Easel.Entities;

/// <summary>
/// Describes position, rotation, and scale, and provides some helper functions and properties for determining things such
/// as the forward vector and model matrix. Typically used in entities, however can be used anywhere.
/// </summary>
public sealed class Transform
{
    /// <summary>
    /// The position of this transform. (Default: Zero)
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The rotation of this transform. (Default: Identity)
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// The scale of this transform. (Default: One)
    /// </summary>
    public Vector3 Scale;

    /// <summary>
    /// Create a new default transform.
    /// </summary>
    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    /// <summary>
    /// The forward vector of this transform.
    /// </summary>
    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);

    /// <summary>
    /// The backward vector of this transform.
    /// </summary>
    public Vector3 Backward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    /// <summary>
    /// The right vector of this transform.
    /// </summary>
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    /// <summary>
    /// The left vector of this transform.
    /// </summary>
    public Vector3 Left => Vector3.Transform(-Vector3.UnitX, Rotation);

    /// <summary>
    /// The up vector of this transform.
    /// </summary>
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    /// <summary>
    /// The down vector of this transform.
    /// </summary>
    public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);

    /// <summary>
    /// Calculates and returns the model matrix for this transform.
    /// </summary>
    public Matrix4x4 ModelMatrix => Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) *
                                    Matrix4x4.CreateTranslation(Position);
}