using Easel.Physics.Shapes;
using JoltPhysicsSharp;

namespace Easel.Entities.Components;

public class Rigidbody : Component
{
    public Rigidbody(IShape shape)
    {
        ShapeSettings settings = shape.ShapeSettings;
        
        BodyCreationSettings bodySettings = new BodyCreationSettings(settings, )
    }
}