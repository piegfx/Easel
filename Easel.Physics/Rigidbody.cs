using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Entities.Components;

namespace Easel.Physics;

public class Rigidbody : Component
{
    private float _iMass;
    private CollisionShape _iShape;
    
    private RigidBody _rb;

    private Transform _lastTransform;

    public bool LockX, LockY, LockZ;

    public Vector3 LinearVelocity
    {
        get => _rb.LinearVelocity;
        set => _rb.LinearVelocity = value;
    }

    public Vector3 AngularVelocity
    {
        get => _rb.AngularVelocity;
        set => _rb.AngularVelocity = value;
    }

    public float Friction
    {
        get => _rb.Friction;
        set => _rb.Friction = value;
    }

    public float Restitution
    {
        get => _rb.Restitution;
        set => _rb.Restitution = value;
    }

    public Rigidbody(float mass, CollisionShape shape)
    {
        _iMass = mass;
        _iShape = shape;
        Physics.FixedUpdate += FixedUpdate;
    }

    private void FixedUpdate()
    {
        Matrix4x4.Decompose(_rb.InterpolationWorldTransform, out Vector3 scale, out Quaternion rotation,
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
            _rb = Physics.AddStaticBody(_iShape, transform);
        else
            _rb = Physics.AddRigidBody(_iMass, _iShape, transform);

        _rb.UserObject = Entity;
    }

    protected override void Update()
    {
        base.Update();
        _rb.AngularFactor = new Vector3(LockX ? 0 : 1, LockY ? 0 : 1, LockZ ? 0 : 1);
        _rb.WorldTransform = Transform.TransformMatrix;
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Physics.Remove(_rb);
        _rb.Dispose();
    }
}