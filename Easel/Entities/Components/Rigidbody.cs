using System;
using System.Numerics;
using Easel.Physics.Internal;
using Easel.Physics.Shapes;
using JoltPhysicsSharp;

namespace Easel.Entities.Components;

public class Rigidbody : Component
{
    private ShapeSettings _settings;
    private bool _staticBody;
    private BodyID _id;

    public Vector3 LinearVelocity
    {
        get => Simulation.BodyInterface.GetLinearVelocity(_id);
        set => Simulation.BodyInterface.SetLinearVelocity(_id, value);
    }

    // TODO: Angular velocity in PR
    public Vector3 AngularVelocity
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Rigidbody(IShape shape, bool staticBody)
    {
        _settings = shape.ShapeSettings;
        _staticBody = staticBody;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        BodyCreationSettings settings = new BodyCreationSettings(_settings, Transform.Position, Transform.Rotation,
            _staticBody ? MotionType.Static : MotionType.Dynamic, (ObjectLayer) (_staticBody ? Layers.NonMoving : Layers.Moving));

        _id = Simulation.BodyInterface.CreateAndAddBody(settings, ActivationMode.Activate);
    }

    protected internal override void AfterUpdate()
    {
        base.AfterUpdate();

        Simulation.BodyInterface.SetPositionAndRotationWhenChanged(_id, Transform.Position, Transform.Rotation,
            ActivationMode.Activate);
    }

    protected internal override void FixedUpdate()
    {
        base.AfterUpdate();

        Transform.Position = Simulation.BodyInterface.GetPosition(_id);
        Transform.Rotation = Simulation.BodyInterface.GetRotation(_id);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Simulation.BodyInterface.RemoveBody(_id);
        Simulation.BodyInterface.DestroyBody(_id);
    }
}