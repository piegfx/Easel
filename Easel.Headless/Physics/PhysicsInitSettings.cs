using System.Numerics;

namespace Easel.Headless.Physics;

public struct PhysicsInitSettings
{
    public Vector3 Gravity;

    public uint MaxPhysicsObjects;

    public PhysicsInitSettings()
    {
        Gravity = new Vector3(0, -9.81f, 0);
        MaxPhysicsObjects = 65536;
    }

    public PhysicsInitSettings(Vector3 gravity, uint maxPhysicsObjects)
    {
        Gravity = gravity;
        MaxPhysicsObjects = maxPhysicsObjects;
    }
}