using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics.Renderers.Structs;
using Easel.Graphics.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public sealed class DeferredRenderer : IRenderer
{
    private bool _inPass;
    private GraphicsDevice _device;

    private Effect _gbufferEffect;

    private Texture _albedoTexture;
    private Texture _posTexture;
    private Texture _depthTexture;
    
    private Framebuffer _gbuffer;

    private WorldMatrices _worldMatrices;
    private GraphicsBuffer _worldMatricesBuffer;

    public RenderTarget2D MainTarget { get; private set; }
    
    public bool InPass => _inPass;

    public DeferredRenderer(Size<int> size, GraphicsDevice device)
    {
        Logger.Debug("Creating deferred renderer.");
        _device = device;
        
        Logger.Debug("Creating main target.");
        MainTarget = new RenderTarget2D(size);

        Logger.Debug("Compiling G-Buffer shader.");

        InputLayoutDescription[] layout = new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex), // texCoord
            new InputLayoutDescription(Format.R32G32B32_Float, 20, 0, InputType.PerVertex), // normal
            new InputLayoutDescription(Format.R32G32B32_Float, 32, 0, InputType.PerVertex), // tangent
        };

        _gbufferEffect = Effect.FromSpirv(Renderer.ShaderNamespace + ".Deferred.GBuffer_vert.spv",
            Renderer.ShaderNamespace + ".Deferred.GBuffer_frag.spv", layout,
            VertexPositionTextureNormalTangent.SizeInBytes, Assembly.GetAssembly(typeof(Effect)));
        
        Logger.Debug("Creating G-Buffer.");

        TextureDescription desc = TextureDescription.Texture2D(size.Width, size.Height, Format.R32G32B32_Float, 1, 1,
            TextureUsage.Framebuffer | TextureUsage.ShaderResource);

        _albedoTexture = device.CreateTexture(desc);
        _posTexture = device.CreateTexture(desc);

        _depthTexture = device.CreateTexture(TextureDescription.Texture2D(size.Width, size.Height, Format.D32_Float, 1,
            1, TextureUsage.Framebuffer));

        _gbuffer = device.CreateFramebuffer(new[]
        {
            new FramebufferAttachment(_albedoTexture),
            new FramebufferAttachment(_posTexture),
            new FramebufferAttachment(_depthTexture),
        });
        
        Logger.Debug("Creating matrices buffer.");

        _worldMatrices = new WorldMatrices()
        {
            Projection = Matrix4x4.Identity,
            View = Matrix4x4.Identity,
            Model = Matrix4x4.Identity
        };

        _worldMatricesBuffer = device.CreateBuffer(BufferType.UniformBuffer, _worldMatrices, true);
    }
    
    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in Vector3 cameraPosition, in SceneInfo sceneInfo)
    {
        if (_inPass)
            throw new EaselException("Renderer is already in a pass!");

        _inPass = true;
        
        _device.SetFramebuffer(MainTarget.PieFramebuffer);

        _worldMatrices.Projection = projection;
        _worldMatrices.View = view;
    }

    public void End3DPass()
    {
        if (!_inPass)
            throw new EaselException("Renderer is not in a pass!");

        _inPass = false;
    }

    public void DrawRenderable(in Renderable renderable, in Matrix4x4 world)
    {
        _worldMatrices.Model = world;

        _device.UpdateBuffer(_worldMatricesBuffer, 0, _worldMatrices);
    }

    public void Resize(Size<int> newSize)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget2D(newSize);
    }

    public void Dispose()
    {
        _gbufferEffect.Dispose();
        _albedoTexture.Dispose();
        _posTexture.Dispose();
        _depthTexture.Dispose();
        _gbuffer.Dispose();
        MainTarget.Dispose();
    }
}