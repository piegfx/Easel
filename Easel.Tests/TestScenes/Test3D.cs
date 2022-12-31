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
        Camera.Main.Skybox = new Skybox(Content.Load<EaselTexture>("Skybox"));
        Camera.Main.Viewport = new Vector4(0, 0, 0.5f, 1f);

        Camera second = new Camera(EaselMath.ToRadians(75), 640 / 360f);
        second.ClearColor = Color.RebeccaPurple;
        Bitmap awesomeface = Content.Load<Bitmap>("awesomeface");
        // lol
        second.Skybox = new Skybox(awesomeface, awesomeface, awesomeface, awesomeface, awesomeface, awesomeface);
        second.Tag = Tags.MainCamera;
        second.Viewport = new Vector4(0.5f, 0, 1.0f, 0.5f);
        AddEntity("second", second);

        Entity entity = new Entity();
        entity.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), new Material(Content.Load<Texture2D>("awesomeface")))));
        AddEntity("cube", entity);
    }

    protected override void Update()
    {
        base.Update();
        
        Camera.Main.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1 * Time.DeltaTime);
        GetEntity<Camera>("second").Transform.Rotation *= Quaternion.CreateFromAxisAngle(-Vector3.UnitY, 1 * Time.DeltaTime);
    }
}