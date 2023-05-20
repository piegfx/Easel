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

        Material material = new StandardMaterial() { RasterizerState = RasterizerState.CullClockwise };
        
        Entity cube = new Entity($"cube", new Transform()
        {
            Position = new Vector3(0, 5, -5),
            Rotation = Quaternion.CreateFromYawPitchRoll(0, 1, 0)
        });
        cube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f))));
        cube.AddComponent(new ModelRenderer(new Cube(), material));
        AddEntity(cube);

        Entity cube2 = new Entity("cubes", new Transform()
        {
            Position = new Vector3(0, -5, -3),
            Scale = new Vector3(20, 0.25f, 20)
        });
        cube2.AddComponent(new ModelRenderer(new Cube(), material));
        cube2.AddComponent(new Rigidbody(new BoxShape(new Vector3(10, 0.25f / 2f, 10)), new RigidbodyInitSettings() { BodyType = BodyType.Static }));
        AddEntity(cube2);
    }
}