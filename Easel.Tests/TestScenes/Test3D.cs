using System.IO;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Math;
using Easel.Primitives;
using Easel.Scenes;
using Easel.Utilities;

namespace Easel.Tests.TestScenes;

public class Test3D : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        Camera.Main.ClearColor = Color.CornflowerBlue;
        Camera.Main.Skybox = new Skybox(EaselTexture.Deserialize(
            File.ReadAllBytes("/home/ollie/Documents/C#/SpaceBox/SpaceBox/Content/Textures/Environment/Skybox.etf")));
        
        Entity entity = new Entity();
        entity.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), new Material(Content.Load<Texture2D>("awesomeface.png")))));
        AddEntity("cube", entity);
    }

    protected override void Update()
    {
        base.Update();
        
        Camera.Main.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1 * Time.DeltaTime);
    }
}