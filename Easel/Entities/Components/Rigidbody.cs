using System;
using System.Numerics;
using Easel.Physics.Internal;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;
using JoltPhysicsSharp;

namespace Easel.Entities.Components;

public class Rigidbody : Component
{
    private ShapeSettings _settings;
    private RigidbodyInitSettings _initSettings;
    public BodyID Handle;

    public Vector3 LinearVelocity
    {
        get => Simulation.BodyInterface.GetLinearVelocity(Handle);
        set => Simulation.BodyInterface.SetLinearVelocity(Handle, value);
    }

    // TODO: Angular velocity in PR
    public Vector3 AngularVelocity
    {
        get => Simulation.BodyInterface.GetAngularVelocity(Handle);
        set => Simulation.BodyInterface.SetAngularVelocity(Handle, value);
    }

    public float Restitution
    {
        get => Simulation.BodyInterface.GetRestitution(Handle);
        set => Simulation.BodyInterface.SetRestitution(Handle, value);
    }

    public Rigidbody(IShape shape, RigidbodyInitSettings? settings = null)
    {
        _settings = shape.ShapeSettings;
        _initSettings = settings ?? new RigidbodyInitSettings();
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        BodyCreationSettings settings = new BodyCreationSettings(_settings, Transform.Position, Transform.Rotation,
            (MotionType) _initSettings.BodyType,
            (ObjectLayer) (_initSettings.BodyType == BodyType.Static ? Layers.NonMoving : Layers.Moving));

        Body body = Simulation.BodyInterface.CreateBody(settings);
        if (_initSettings.BodyType != BodyType.Static)
        {
            body.SetLinearVelocity(_initSettings.LinearVelocity);
            body.SetAngularVelocity(_initSettings.AngularVelocity);
        }

        body.Restitution = Restitution;
        
        Simulation.BodyInterface.AddBody(body, ActivationMode.Activate);
        Handle = body.ID;
    }

    protected internal override void AfterUpdate()
    {
        base.AfterUpdate();

        Simulation.BodyInterface.SetPositionAndRotationWhenChanged(Handle, Transform.Position, Transform.Rotation,
            ActivationMode.Activate);
    }

    protected internal override void FixedUpdate()
    {
        base.FixedUpdate();

        Transform.Position = Simulation.BodyInterface.GetPosition(Handle);
        Transform.Rotation = Simulation.BodyInterface.GetRotation(Handle);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Simulation.BodyInterface.RemoveBody(Handle);
        Simulation.BodyInterface.DestroyBody(Handle);
    }

    public void ApplyForce(Vector3 force)
    {
        Simulation.BodyInterface.AddForce(Handle, force);
    }
}