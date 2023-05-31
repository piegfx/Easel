using System;
using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics.Renderers.Structs;
using Easel.Graphics.Structs;
using Easel.Math;
using Pie;
using Color = System.Drawing.Color;

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

    private CameraMatrices _cameraMatrices;
    private GraphicsBuffer _cameraMatricesBuffer;

    private RenderInfo _renderInfo;
    private GraphicsBuffer _renderInfoBuffer;

    private RasterizerState _rasterizerState;
    private DepthStencilState _depthStencilState;
    private BlendState _blendState;

    // TODO: Engine sampler states.
    private SamplerState _samplerState;

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

        _cameraMatrices = new CameraMatrices()
        {
            Projection = Matrix4x4.Identity,
            View = Matrix4x4.Identity,
        };

        _cameraMatricesBuffer = device.CreateBuffer(BufferType.UniformBuffer, _cameraMatrices, true);

        Logger.Debug("Creating render info buffer.");
        _renderInfo = new RenderInfo();
        _renderInfoBuffer = device.CreateBuffer(BufferType.UniformBuffer, _renderInfo, true);
        
        Logger.Debug("Creating various device states.");
        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _depthStencilState = device.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);
        _blendState = device.CreateBlendState(BlendStateDescription.Disabled);

        _samplerState = device.CreateSamplerState(SamplerStateDescription.LinearClamp);
    }
    
    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in Vector3 cameraPosition, in SceneInfo sceneInfo)
    {
        if (_inPass)
            throw new EaselException("Renderer is already in a pass!");

        _inPass = true;
        
        // TODO in pie, Framebuffer.Size returns 0.
        
        _device.SetFramebuffer(_gbuffer);
        _device.Viewport = new System.Drawing.Rectangle(0, 0, _albedoTexture.Description.Width, _albedoTexture.Description.Height);
        Console.WriteLine(_gbuffer.Size);
        _device.ClearColorBuffer(Color.Black);
        _device.ClearDepthStencilBuffer(ClearFlags.Depth, 1, 0);

        _cameraMatrices.Projection = projection;
        _cameraMatrices.View = view;
        _device.UpdateBuffer(_cameraMatricesBuffer, 0, _cameraMatrices);

        _renderInfo.SceneInfo = sceneInfo;
    }

    public void End3DPass()
    {
        if (!_inPass)
            throw new EaselException("Renderer is not in a pass!");

        _inPass = false;
    }

    public void DrawRenderable(in Renderable renderable, in Matrix4x4 world)
    {
        _renderInfo.WorldMatrix = world;
        _renderInfo.Material = renderable.Material.ShaderMaterial;
        _device.UpdateBuffer(_renderInfoBuffer, 0, _renderInfo);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        
        _device.SetShader(_gbufferEffect.PieShader);
        
        _device.SetUniformBuffer(0, _cameraMatricesBuffer);
        _device.SetUniformBuffer(1, _renderInfoBuffer);
        
        _device.SetTexture(2, renderable.Material.AlbedoTexture.PieTexture, _samplerState);
        
        _device.SetRasterizerState(_rasterizerState);
        _device.SetDepthStencilState(_depthStencilState);
        _device.SetBlendState(_blendState);
        
        _device.SetVertexBuffer(0, renderable.VertexBuffer, _gbufferEffect.Stride, _gbufferEffect.InputLayout);
        _device.SetIndexBuffer(renderable.IndexBuffer, IndexType.UInt);
        
        _device.DrawIndexed(renderable.NumIndices);
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