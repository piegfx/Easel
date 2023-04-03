using System;
using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Primitives;
using Easel.Physics;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestPhysics : Scene
{
    protected override void Initialize()
    {
        base.Initialize();
        
        Physics.Physics.Initialize(new PhysicsInitializeSettings());
        
        Camera.Main.AddComponent(new NoClipCamera());

        Entity cube = new Entity("cube", new Transform()
        {
            Position = new Vector3(0, 0, -3)
        });
        cube.AddComponent(new Rigidbody(0, new BoxShape(0.5f)));
        cube.AddComponent(new ModelRenderer(new Cube(), new StandardMaterial() { RasterizerState = RasterizerState.CullClockwise }));
        AddEntity(cube);
    }

    protected override void Update()
    {
        Physics.Physics.Timestep(Time.DeltaTime);
        base.Update();

        if (Physics.Physics.Raycast(Camera.Main.Transform.Position, Camera.Main.Transform.Forward, 100, out RayHit hit))
        {
            Console.WriteLine("Hit!");
            Console.WriteLine(hit.Entity.Name);
        }
        else
        {
            Console.WriteLine("boooo");
        }
    }
}