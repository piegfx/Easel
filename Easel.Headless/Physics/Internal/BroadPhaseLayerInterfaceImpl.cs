using System.Diagnostics;
using JoltPhysicsSharp;

namespace Easel.Headless.Physics.Internal;

internal class BroadPhaseLayerInterfaceImpl : BroadPhaseLayerInterface
{
    private BroadPhaseLayer[] _objectToBroadPhaseLayers;
    
    public BroadPhaseLayerInterfaceImpl()
    {
        _objectToBroadPhaseLayers = new BroadPhaseLayer[Layers.NumLayers];
        _objectToBroadPhaseLayers[Layers.NonMoving] = BroadPhaseLayers.NonMoving;
        _objectToBroadPhaseLayers[Layers.Moving] = BroadPhaseLayers.Moving;
    }
    
    protected override int GetNumBroadPhaseLayers()
    {
        return BroadPhaseLayers.NumLayers;
    }

    protected override BroadPhaseLayer GetBroadPhaseLayer(ObjectLayer layer)
    {
        Debug.Assert(layer < Layers.NumLayers);
        return _objectToBroadPhaseLayers[layer];
    }

    protected override string GetBroadPhaseLayerName(BroadPhaseLayer layer)
    {
        return string.Empty;
    }
}