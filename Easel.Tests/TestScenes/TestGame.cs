using System.Numerics;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Headless.Entities;
using Easel.Headless.Scenes;
using Camera = Easel.Entities.Camera;

namespace Easel.Tests.TestScenes;

public class TestGameScene : Scene
{
    private Vector2 _velocity;
    
    protected override void Initialize()
    {
        base.Initialize();

        Camera.Main.UseOrtho2D();
        
        Entity entity = new Entity("thing", new Transform()
        {
            Position = new Vector3(100, 100, 0)
        });
        entity.AddComponent(new Sprite(EaselGame.Instance.Content.Load<Texture2D>("awesomeface")));
        AddEntity(entity);
    }

    protected override void Update()
    {
        base.Update();

        Entity entity = GetEntity("thing");

        _velocity.X = 0;
        
        while (entity.Transform.Position.X >= 0)
            _velocity.X = -3;

        entity.Transform.Position.X += _velocity.X;
    }
}