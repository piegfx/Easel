using System.Numerics;
using System.Runtime.CompilerServices;
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

    public override bool NeedsCollision(BroadphaseProxy proxy0)
    {
        // Ignore triggers
        if (((CollisionObject) proxy0.ClientObject).InternalType == CollisionObjectTypes.GhostObject)
            return false;

        return base.NeedsCollision(proxy0);
    }
}