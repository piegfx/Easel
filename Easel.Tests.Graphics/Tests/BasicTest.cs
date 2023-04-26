using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Tests.Graphics.Tests;

public class BasicTest : TestBase
{
    private Texture2D _texture;

    protected override void Initialize()
    {
        base.Initialize();

        _texture = new Texture2D(Bitmap.Debug);
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.BeginFrame();

        Renderer.Perform3DPass(new CameraInfo(Matrix4x4.Identity, Matrix4x4.Identity, Color.RebeccaPurple),
            new SceneInfo(0.1f), new Rectangle<float>(0, 0, 1, 1));
        
        Renderer.EndFrame();
    }
}