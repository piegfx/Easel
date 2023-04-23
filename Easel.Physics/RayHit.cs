using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Math;

namespace Easel.Physics;

public struct RayHit
{
    public Vector3 WorldPosition;
    public Vector3 HitPosition;
    public Vector3T<int> CubeNormal;
    public Vector3 RealNormal;
    public Quaternion Rotation;
    public int ChildIndex;
    public Entity Entity;

    public CollisionObject CollisionObject;
}