using System.Numerics;
using JoltPhysicsSharp;

namespace Easel.Headless.Physics.Shapes;

public struct BoxShape : IShape
{
    public Vector3 HalfExtents;
    
    public BoxShape(Vector3 halfExtents)
    {
        HalfExtents = halfExtents;
    }

    public ShapeSettings ShapeSettings => new BoxShapeSettings(HalfExtents);
}