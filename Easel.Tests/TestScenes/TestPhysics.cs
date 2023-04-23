using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Primitives;
using Easel.Math;
using Easel.Physics.Shapes;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestPhysics : Scene
{
    protected override void Initialize()
    {
        base.Initialize();
        
        Camera.Main.ClearColor = Color.CornflowerBlue;

        Entity cube = new Entity("cube", new Transform()
        {
            Position = new Vector3(0, 0, -3)
        });
        cube.AddComponent(new ModelRenderer(new Cube(), new StandardMaterial() { RasterizerState = RasterizerState.CullClockwise }));
        cube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f))));
        AddEntity(cube);
    }
}