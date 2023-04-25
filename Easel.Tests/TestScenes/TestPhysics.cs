using System;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Primitives;
using Easel.Math;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;
using Easel.Scenes;
using Pie.Windowing;

namespace Easel.Tests.TestScenes;

public class TestPhysics : Scene
{
    protected override void Initialize()
    {
        base.Initialize();
        
        Camera.Main.AddComponent(new NoClipCamera() { MoveSpeed = 10 });
        
        Camera.Main.ClearColor = Color.CornflowerBlue;

        Entity cube = new Entity("cube", new Transform()
        {
            Position = new Vector3(0, 0, -3)
        });
        cube.AddComponent(new ModelRenderer(new Cube(), new StandardMaterial() { RasterizerState = RasterizerState.CullClockwise }));
        cube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), new RigidbodyInitSettings() { Restitution = 1 }));
        AddEntity(cube);
        
        Entity cube2 = new Entity("cube2", new Transform()
        {
            Position = new Vector3(0, -5, -3),
        });
        cube2.AddComponent(new ModelRenderer(new Cube(), new StandardMaterial() { RasterizerState = RasterizerState.CullClockwise }));
        cube2.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), new RigidbodyInitSettings() { BodyType = BodyType.Static, Restitution = 1 }));
        AddEntity(cube2);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.KeyDown(Key.Up))
            GetEntity("cube").GetComponent<Rigidbody>().LinearVelocity = new Vector3(0, 1, 0);

        if (Input.KeyDown(Key.Right))
            GetEntity("cube").GetComponent<Rigidbody>().AngularVelocity = new Vector3(1, 0, 0);
    }
}