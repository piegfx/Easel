using System;
using System.Numerics;
using Easel.Headless.Physics.Internal;
using JoltPhysicsSharp;

namespace Easel.Headless.Physics;

public class Simulation
{
    public PhysicsSystem PhysicsSystem;
    public BodyInterface BodyInterface => PhysicsSystem.BodyInterface;
    
    private TempAllocator _allocator;
    private JobSystemThreadPool _jobSystem;

    private BroadPhaseLayerInterfaceImpl _broadPhaseInterface;
    private ObjectLayerPairFilterImpl _objectLayerPair;
    private ObjectVsBroadPhaseLayerFilterImpl _objectVsBroadPhase;
    
    public Simulation(PhysicsInitSettings initSettings)
    {
        Foundation.Init();
        
        _allocator = new TempAllocator(10 * 1024 * 1024);

        _jobSystem = new JobSystemThreadPool(Foundation.MaxPhysicsJobs, Foundation.MaxPhysicsBarriers,
            Environment.ProcessorCount);

        _broadPhaseInterface = new BroadPhaseLayerInterfaceImpl();
        _objectLayerPair = new ObjectLayerPairFilterImpl();
        _objectVsBroadPhase = new ObjectVsBroadPhaseLayerFilterImpl();

        PhysicsSystem = new PhysicsSystem();
        PhysicsSystem.Init(initSettings.MaxPhysicsObjects, 0, initSettings.MaxPhysicsObjects, 1024,
            _broadPhaseInterface, _objectVsBroadPhase, _objectLayerPair);
        PhysicsSystem.Gravity = initSettings.Gravity;
    }

    public void Timestep(float deltaTime)
    {
        PhysicsSystem.Update(deltaTime, 1, 1, _allocator, _jobSystem);
    }
}