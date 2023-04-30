using System;
using Easel.Headless;
using Easel.Headless.Entities;
using Easel.Headless.Scenes;

namespace Easel.Tests.TestScenes;

public class TestServer : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        Entity entity = new Entity("test");
        AddEntity(entity);
    }

    protected override void Update()
    {
        base.Update();

        Entity test = GetEntity("test");
        test.Transform.Position.X -= 1 * Time.DeltaTime;
        Console.WriteLine(Time.DeltaTime);
    }
}