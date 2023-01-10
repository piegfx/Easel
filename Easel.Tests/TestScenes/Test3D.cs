using System.IO;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Math;
using Easel.Primitives;
using Easel.Scenes;
using Easel.Utilities;
using Pie.Windowing;

namespace Easel.Tests.TestScenes;

public class Test3D : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        Input.MouseState = MouseState.Locked;

        DDS dds = new DDS(File.ReadAllBytes("/home/ollie/Pictures/RubberFloor.dds"));

        Texture2D texture = Content.Load<Texture2D>("awesomeface");
        texture.SamplerState = SamplerState.AnisotropicRepeat;
        
        Camera.Main.ClearColor = Color.CornflowerBlue;
        Camera.Main.Skybox = new Skybox(Content.Load<EaselTexture>("Skybox"));
        Camera.Main.Viewport = new Vector4(0, 0, 0.5f, 1f);
        Camera.Main.AddComponent(new NoClipCamera()
        {
            MoveSpeed = 10
        });
        Camera.Main.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), new UnlitMaterial(texture))));

        Camera second = new Camera(EaselMath.ToRadians(75), 640 / 360f);
        second.Transform.Position = new Vector3(0, 0, -5);
        second.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(EaselMath.ToRadians(180), 0, 0);
        second.ClearColor = Color.RebeccaPurple;
        //Bitmap awesomeface = Content.Load<Bitmap>("awesomeface");
        // lol
        //second.Skybox = new Skybox(awesomeface, awesomeface, awesomeface, awesomeface, awesomeface, awesomeface);
        second.Skybox = Camera.Main.Skybox;
        second.Tag = Tags.MainCamera;
        second.Viewport = new Vector4(0.5f, 0, 1.0f, 0.5f);
        AddEntity("second", second);

        Camera third = new Camera(type: CameraType.Camera2D);
        // TODO: Better camera solution than forcing every camera to use main camera tag.
        third.Tag = Tags.MainCamera;
        //AddEntity("third", third);

        Entity entity = new Entity(new Transform()
        {
            Position = new Vector3(0, 0, -3)
        });

        entity.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), new StandardMaterial(texture, 32)
        {
            Tiling = new Vector2(2),
            Color = Color.Orange with { A = 0.5f }
        })));
        AddEntity("cube", entity);

        Entity thingy = new Entity();
        thingy.AddComponent(new Sprite(texture));
        AddEntity(thingy);
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.KeyPressed(Key.Escape))
            Game.Close();
        
        //Camera.Main.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1 * Time.DeltaTime);
        //GetEntity<Camera>("second").Transform.Rotation *= Quaternion.CreateFromAxisAngle(-Vector3.UnitY, 1 * Time.DeltaTime);

        GetEntity("cube").Transform.Rotation *=
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1 * Time.DeltaTime) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.75f * Time.DeltaTime) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.34f * Time.DeltaTime);
    }
}