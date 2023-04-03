using System.Numerics;

namespace Easel.Physics;

public struct PhysicsInitializeSettings
{
    public Vector3 Gravity;

    public PhysicsInitializeSettings()
    {
        Gravity = new Vector3(0, -9.81f, 0);
    }
}