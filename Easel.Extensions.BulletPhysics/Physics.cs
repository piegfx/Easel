using System.Numerics;
using BulletSharp;

namespace Easel.Utilities;

public static class Physics
{
    public static readonly DiscreteDynamicsWorld World;
    public static readonly CollisionDispatcher Dispatcher;
    public static readonly DbvtBroadphase Broadphase;
    public static readonly CollisionConfiguration Configuration;

    public static float SimulationSpeed = 1;

    static Physics()
    {
        Configuration = new DefaultCollisionConfiguration();
        Dispatcher = new CollisionDispatcher(Configuration);
        Broadphase = new DbvtBroadphase();
        World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, Configuration);
        World.Gravity = new Vector3(0, -9.81f, 0);
        
        World.PairCache.SetInternalGhostPairCallback(new GhostPairCallback());
    }

    public static RigidBody AddRigidBody(float mass, CollisionShape shape, Matrix4x4 startTransform)
    {
        Vector3 localInertia = shape.CalculateLocalInertia(mass);
        using RigidBodyConstructionInfo info =
            new RigidBodyConstructionInfo(mass, new DefaultMotionState(startTransform), shape, localInertia);
        RigidBody rb = new RigidBody(info);
        World.AddRigidBody(rb);
        return rb;
    }

    public static RigidBody AddStaticBody(CollisionShape shape, Matrix4x4 startTransform)
    {
        using RigidBodyConstructionInfo info =
            new RigidBodyConstructionInfo(0, new DefaultMotionState(startTransform), shape);
        RigidBody rb = new RigidBody(info);
        rb.CollisionFlags |= CollisionFlags.StaticObject | CollisionFlags.CustomMaterialCallback;
        World.AddRigidBody(rb);
        return rb;
    }

    public static GhostObject AddTrigger(CollisionShape shape, Matrix4x4 startTransform)
    {
        GhostObject obj = new PairCachingGhostObject();
        obj.CollisionShape = shape;
        obj.WorldTransform = startTransform;
        obj.CollisionFlags |= CollisionFlags.NoContactResponse;
        World.AddCollisionObject(obj);
        return obj;
    }

    public static void Update()
    {
        World.StepSimulation(Time.DeltaTime * SimulationSpeed, fixedTimeStep: (1 / 60f) * SimulationSpeed);
    }
}