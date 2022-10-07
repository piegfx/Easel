using System.Numerics;
using BulletSharp;

namespace Easel.Extensions.BulletPhysics;

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

    public static bool Raycast(Vector3 position, Vector3 direction, float distance, out RayHit hit)
    {
        Vector3 dir = position + direction * distance;
        using ClosestRayResultCallback cb = new ClosestRayResultCallback(ref position, ref dir);
        cb.Flags |= (uint) TriangleRaycastCallback.EFlags.DisableHeightfieldAccelerator;
        World.RayTest(position, dir, cb);

        if (!cb.HasHit)
        {
            hit = default;
            return false;
        }

        Matrix4x4.Decompose(cb.CollisionObject.WorldTransform, out _, out Quaternion rotation, out _);
        Quaternion invert = Quaternion.Inverse(rotation);
        Vector3 normal = Vector3.Transform(cb.HitNormalWorld, invert);
        normal = new Vector3((int) MathF.Round(normal.X), (int) MathF.Round(normal.Y), (int) MathF.Round(normal.Z));

        hit.Position = cb.CollisionObject.WorldTransform.Translation;
        hit.Normal = normal;
        hit.CollisionObject = cb.CollisionObject;
        hit.Rotation = rotation;

        return true;
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