using System.Numerics;
using BulletSharp;
using Easel.Math;

namespace Easel.Physics;

public struct RayHit
{
    public Vector3 WorldPosition;
    public Vector3 HitPosition;
    public Vector3I CubeNormal;
    public Vector3 RealNormal;
    public Quaternion Rotation;
    public int ChildIndex;

    public CollisionObject CollisionObject;
}