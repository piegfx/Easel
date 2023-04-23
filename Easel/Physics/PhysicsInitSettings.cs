using System.Numerics;

namespace Easel.Physics;

public struct PhysicsInitSettings
{
    public Vector3 Gravity;

    public PhysicsInitSettings()
    {
        Gravity = new Vector3(0, -9.81f, 0);
    }

    public PhysicsInitSettings(Vector3 gravity)
    {
        Gravity = gravity;
    }
}