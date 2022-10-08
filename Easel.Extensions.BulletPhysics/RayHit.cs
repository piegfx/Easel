using System.Numerics;
using BulletSharp;

namespace Easel.Extensions.BulletPhysics;

public struct RayHit
{
    public Vector3 WorldPosition;
    public Vector3 HitPosition;
    public Vector3 CubeNormal;
    public Vector3 RealNormal;
    public Quaternion Rotation;

    public CollisionObject CollisionObject;
}