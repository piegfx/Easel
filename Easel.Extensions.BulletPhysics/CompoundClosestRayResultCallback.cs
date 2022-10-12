using System.Numerics;
using BulletSharp;

namespace Easel.Extensions.BulletPhysics;

public class CompoundClosestRayResultCallback : ClosestRayResultCallback
{
    public int ChildIndex;
    
    public CompoundClosestRayResultCallback(ref Vector3 rayFromWorld, ref Vector3 rayToWorld) : base(ref rayFromWorld, ref rayToWorld) { }

    public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
    {
        ChildIndex = rayResult.LocalShapeInfo?.TriangleIndex ?? -1;
        
        return base.AddSingleResult(ref rayResult, normalInWorldSpace);
    }
}