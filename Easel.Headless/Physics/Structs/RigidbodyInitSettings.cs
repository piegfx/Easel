using System.Numerics;

namespace Easel.Headless.Physics.Structs;

public struct RigidbodyInitSettings
{
    public BodyType BodyType;

    public float Restitution;

    public Vector3 LinearVelocity;

    public Vector3 AngularVelocity;

    public RigidbodyInitSettings()
    {
        BodyType = BodyType.Dynamic;
        Restitution = 0;
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
    }
    
    public RigidbodyInitSettings(BodyType bodyType, float restitution, Vector3 linearVelocity, Vector3 angularVelocity)
    {
        BodyType = bodyType;
        Restitution = restitution;
        LinearVelocity = linearVelocity;
        AngularVelocity = angularVelocity;
    }
}