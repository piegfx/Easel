using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public sealed class DeferredRenderer : IRenderer
{
    private bool _inPass;
    private Renderer _renderer;

    private Effect _gbufferEffect;

    private Texture _albedoTexture;
    private Texture _posTexture;
    private Texture _depthTexture;
    
    private Framebuffer _gbuffer;

    public RenderTarget2D MainTarget { get; private set; }
    
    public bool InPass => _inPass;

    public DeferredRenderer(Size<int> size, Renderer renderer)
    {
        Logger.Debug("Creating deferred renderer.");
        _renderer = renderer;
        
        Logger.Debug("Creating main target.");
        MainTarget = new RenderTarget2D(size);

        GraphicsDevice device = renderer.Device;
        
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
    }
    
    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in Vector3 cameraPosition, in SceneInfo sceneInfo)
    {
        if (_inPass)
            throw new EaselException("Renderer is already in a pass!");

        _inPass = true;
        
        _renderer.SetRenderTarget(MainTarget);
    }

    public void End3DPass()
    {
        if (!_inPass)
            throw new EaselException("Renderer is not in a pass!");

        _inPass = false;
    }

    public void DrawRenderable(in Renderable renderable, in Matrix4x4 world)
    {
        
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