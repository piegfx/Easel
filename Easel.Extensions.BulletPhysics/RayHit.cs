using System.Numerics;
using BulletSharp;

namespace Easel.Extensions.BulletPhysics;

public struct RayHit
{
    public Vector3 Position;
    public Vector3 Normal;
    public Quaternion Rotation;

    public CollisionObject CollisionObject;
}