using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Renderers;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Tests.Graphics.Tests;

public class Basic3D : TestBase
{
    private Texture2D _texture;
    private Renderable _renderable;
    
    protected override void Initialize()
    {
        base.Initialize();
        
        _texture = new Texture2D(new Bitmap(Utils.LoadEmbeddedResource(Assembly.GetAssembly(typeof(Texture2D)),
            "Easel.Graphics.DEBUG.png")));

        VertexPositionTextureNormalTangent[] vertices = new []
        {
            new VertexPositionTextureNormalTangent(new Vector3T<float>(-0.5f, -0.5f, 0), new Vector2T<float>(0, 0), Vector3T<float>.UnitZ, Vector3T<float>.Zero),
            new VertexPositionTextureNormalTangent(new Vector3T<float>(0.5f, -0.5f, 0), new Vector2T<float>(1, 0), Vector3T<float>.UnitZ, Vector3T<float>.Zero),
            new VertexPositionTextureNormalTangent(new Vector3T<float>(0.5f, 0.5f, 0), new Vector2T<float>(1, 1), Vector3T<float>.UnitZ, Vector3T<float>.Zero),
            new VertexPositionTextureNormalTangent(new Vector3T<float>(-0.5f, 0.5f, 0), new Vector2T<float>(0, 1), Vector3T<float>.UnitZ, Vector3T<float>.Zero),
        };

        uint[] indices = new[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        MaterialDescription description = new MaterialDescription()
        {
            AlbedoTexture = _texture
        };
        
        _renderable = new Renderable(vertices, indices, new Material(description));
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        Renderer.NewFrame();
        
        Renderer.Draw(_renderable, Matrix4x4.CreateTranslation(0, 0, 3));

        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(EaselMath.ToRadians(75),
            Window.Size.Width / (float) Window.Size.Height, 0.1f, 1000f);

        Matrix4x4 view = Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

        Renderer.Perform3DPass(new CameraInfo(projection, view, Color.Orange, Vector3.Zero), new SceneInfo(0.1f),
            new Rectangle<float>(0, 0, 1, 1));
        
        Renderer.EndFrame();
    }

    public Basic3D() : base("Basic 3D test") { }
}