using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Entities.Components;

namespace Easel.Physics;

public class Rigidbody : Component
{
    private float _iMass;
    private CollisionShape _iShape;
    
    public RigidBody BulletBody;

    private Transform _lastTransform;

    public bool LockX, LockY, LockZ;

    public Vector3 LinearVelocity
    {
        get => BulletBody.LinearVelocity;
        set => BulletBody.LinearVelocity = value;
    }

    public Vector3 AngularVelocity
    {
        get => BulletBody.AngularVelocity;
        set => BulletBody.AngularVelocity = value;
    }

    public float Friction
    {
        get => BulletBody.Friction;
        set => BulletBody.Friction = value;
    }

    public float Restitution
    {
        get => BulletBody.Restitution;
        set => BulletBody.Restitution = value;
    }

    public Rigidbody(float mass, CollisionShape shape)
    {
        _iMass = mass;
        _iShape = shape;
        Physics.FixedUpdate += FixedUpdate;
    }

    private void FixedUpdate()
    {
        Matrix4x4.Decompose(BulletBody.InterpolationWorldTransform, out Vector3 scale, out Quaternion rotation,
            out Vector3 translation);

        Transform.Position = translation;
        Transform.Rotation = rotation;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Matrix4x4 transform = Matrix4x4.CreateFromQuaternion(Transform.Rotation) *
                              Matrix4x4.CreateTranslation(Transform.Position);
        
        _iShape.LocalScaling = Transform.Scale;

        if (_iMass == 0)
            BulletBody = Physics.AddStaticBody(_iShape, transform);
        else
            BulletBody = Physics.AddRigidBody(_iMass, _iShape, transform);

        BulletBody.UserObject = Entity;
    }

    protected override void Update()
    {
        base.Update();
        BulletBody.AngularFactor = new Vector3(LockX ? 0 : 1, LockY ? 0 : 1, LockZ ? 0 : 1);
        BulletBody.WorldTransform = Transform.TransformMatrix;
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Physics.Remove(BulletBody);
        BulletBody.Dispose();
    }
}