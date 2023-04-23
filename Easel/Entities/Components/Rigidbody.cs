using Easel.Physics.Internal;
using Easel.Physics.Shapes;
using JoltPhysicsSharp;

namespace Easel.Entities.Components;

public class Rigidbody : Component
{
    private ShapeSettings _settings;
    private Body _body;
    
    public Rigidbody(IShape shape)
    {
        _settings = shape.ShapeSettings;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        BodyCreationSettings settings = new BodyCreationSettings(_settings, Transform.Position, Transform.Rotation,
            MotionType.Dynamic, Layers.Moving);
        
        _body = EaselGame.Instance.Simulation.BodyInterface.CreateBody(settings);
        
        EaselGame.Instance.Simulation.BodyInterface.AddBody(_body, ActivationMode.Activate);
    }

    protected internal override void Update()
    {
        base.Update();
        
        //_body.
    }

    protected internal override void AfterUpdate()
    {
        base.AfterUpdate();

        Transform.Position = _body.Position;
        Transform.Rotation = _body.Rotation;
    }
}