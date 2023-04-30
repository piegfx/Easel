using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Headless.Entities;
using Easel.Headless.Scenes;
using Easel.Math;

namespace Easel.Events; 

public class InitializeSceneEvent
{
    public static void Initialize()
    {
        Scene.InitializeScene += (obj, args) => 
        {
            Size<int> size = (Size<int>) EaselGame.Instance.Window.Size;
            Camera camera = new Camera("Main Camera", EaselMath.ToRadians(70), size.Width / (float) size.Height);
            camera.Tag = Tags.MainCamera;
            args.Scene.AddEntity(camera);

            Entity directionalLight = new Entity("Sun");
            directionalLight.AddComponent(new DirectionalLight(new Vector2(EaselMath.ToRadians(0), EaselMath.ToRadians(75)),
                Color.White));
            args.Scene.AddEntity(directionalLight);
        };
    }
}