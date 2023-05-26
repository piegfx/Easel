using System.Diagnostics;
using JoltPhysicsSharp;

namespace Easel.Physics.Internal;

internal class ObjectVsBroadPhaseLayerFilterImpl : ObjectVsBroadPhaseLayerFilter
{
    protected override bool ShouldCollide(ObjectLayer layer1, BroadPhaseLayer layer2)
    {
        switch (layer1)
        {
            case Layers.NonMoving:
                return layer2 == BroadPhaseLayers.Moving;
            case Layers.Moving:
                return true;
            default:
                Debug.Assert(false);
                return false;
        }
    }
}