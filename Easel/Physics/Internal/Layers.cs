using JoltPhysicsSharp;

namespace Easel.Physics.Internal;

internal static class Layers
{
    public const int NonMoving = 0;
    public const int Moving = 1;
    public const int NumLayers = 2;
}

internal static class BroadPhaseLayers
{
    public static BroadPhaseLayer NonMoving = new BroadPhaseLayer(0);
    public static BroadPhaseLayer Moving = new BroadPhaseLayer(1);
    public static int NumLayers = 2;
}