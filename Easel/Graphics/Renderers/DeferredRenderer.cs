using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Easel.Entities;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Easel.Scenes;
using Easel.Utilities;
using Pie;
using PTex = Pie.Texture;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Deferred rendering is Easel's primary rendering method. It provides a way to quickly render thousands of lights,
/// and is currently the only renderer that supports any form of lighting, outside of the directional light (called Sun
/// in Easel).
/// </summary>
public class DeferredRenderer : I3DRenderer
{
    public readonly Framebuffer GBuffer;
    public readonly PTex PositionTexture;
    public readonly PTex NormalTexture;
    public readonly PTex AlbedoTexture;
    public readonly PTex SpecularTexture;
    
    private EaselGraphics _graphics;

    private GraphicsBuffer _projViewModelBuffer;
    private ProjViewModel _projViewModel;

    private CameraInfo _cameraInfo;
    private GraphicsBuffer _cameraBuffer;

    private List<(Renderable, Matrix4x4)> _translucents;
    private List<(Renderable, Matrix4x4)> _opaques;

    private RasterizerState _rasterizerState;
    private DepthState _depthState;
    private SamplerState _samplerState;
    private BlendState _blendState;

    private EffectLayout _el;

    public DeferredRenderer(EaselGraphics graphics)
    {
        _graphics = graphics;
        GraphicsDevice device = graphics.PieGraphics;
        
        int texWidth = 1280;
        int texHeight = 720;
        
        PositionTexture = device.CreateTexture(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R16G16B16A16_Float, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer));
        NormalTexture = device.CreateTexture(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R16G16B16A16_Float, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer));
        AlbedoTexture = device.CreateTexture(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R8G8B8A8_UNorm, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer));
        
        GBuffer = device.CreateFramebuffer(new FramebufferAttachment(PositionTexture, AttachmentType.Color),
            new FramebufferAttachment(NormalTexture, AttachmentType.Color),
            new FramebufferAttachment(AlbedoTexture, AttachmentType.Color));
        
        _translucents = new List<(Renderable, Matrix4x4)>();
        _opaques = new List<(Renderable, Matrix4x4)>();

        _projViewModel = new ProjViewModel()
        {
            ProjView = Matrix4x4.Identity,
            Model = Matrix4x4.Identity
        };
        _projViewModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projViewModel, true);

        _cameraInfo = new CameraInfo();
        _cameraBuffer = device.CreateBuffer(BufferType.UniformBuffer, _cameraInfo, true);

        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullClockwise);
        _depthState = device.CreateDepthState(DepthStateDescription.LessEqual);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.AnisotropicRepeat);
        _blendState = device.CreateBlendState(BlendStateDescription.NonPremultiplied);

        _el = new EffectLayout(
            new Effect("Easel.Graphics.Shaders.Standard.vert", "Easel.Graphics.Shaders.Deferred.GBuffer.frag"),
            device.CreateInputLayout(
                new InputLayoutDescription("aPosition", AttributeType.Float3, 0, 0, InputType.PerVertex),
                new InputLayoutDescription("aTexCoords", AttributeType.Float2, 12, 0, InputType.PerVertex),
                new InputLayoutDescription("aNormals", AttributeType.Float3, 20, 0, InputType.PerVertex),
                new InputLayoutDescription("aTangent", AttributeType.Float3, 32, 0, InputType.PerVertex)),
            VertexPositionTextureNormalTangent.SizeInBytes);
    }

    public void DrawTranslucent(Renderable renderable, Matrix4x4 world)
    {
        throw new System.NotImplementedException();
    }

    public void DrawOpaque(Renderable renderable, Matrix4x4 world)
    {
        _opaques.Add((renderable, world));
    }

    public void ClearAll()
    {
        _opaques.Clear();
    }

    public void Render(Camera camera, World world)
    {
        GraphicsDevice device = _graphics.PieGraphics;
        _graphics.Clear(world.ClearColor);
        world.Skybox?.Draw(camera);
        _projViewModel.ProjView = camera.ViewMatrix * camera.ProjectionMatrix;
        
        device.SetFramebuffer(GBuffer);

        _cameraInfo.Sun = SceneManager.ActiveScene.World.Sun.ShaderDirectionalLight;
        _cameraInfo.CameraPos = new Vector4(camera.Transform.Position, 1);

        device.SetRasterizerState(_rasterizerState);
        device.SetDepthState(_depthState);
        device.SetBlendState(_blendState);
        device.SetUniformBuffer(0, _projViewModelBuffer);
        device.SetUniformBuffer(1, _cameraBuffer);
        device.SetPrimitiveType(PrimitiveType.TriangleList);

        foreach ((Renderable renderable, Matrix4x4 mWorld) in _opaques.OrderBy(renderable => Vector3.Distance(renderable.Item2.Translation, camera.Transform.Position)))
        {
            // TODO move to array and convert to ref
            DrawRenderable(renderable, mWorld);
        }
    }
    
    private void DrawRenderable(in Renderable renderable, in Matrix4x4 world)
    {
        GraphicsDevice device = _graphics.PieGraphics;
        
        _projViewModel.Model = world;
        device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);

        _cameraInfo.Material = renderable.Material.ShaderMaterial;
        device.UpdateBuffer(_cameraBuffer, 0, _cameraInfo);

        device.SetShader(_el.Effect.PieShader);
        device.SetTexture(2, renderable.Material.Albedo?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
        device.SetTexture(3, renderable.Material.Specular?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
        device.SetTexture(4, renderable.Material.Normal?.PieTexture ?? Texture2D.Void.PieTexture, _samplerState);
        device.SetVertexBuffer(0, renderable.VertexBuffer, _el.Stride, _el.Layout);
        device.SetIndexBuffer(renderable.IndexBuffer, IndexType.UInt);
        device.DrawIndexed(renderable.IndicesLength);
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}