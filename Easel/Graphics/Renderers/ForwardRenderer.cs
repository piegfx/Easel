using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Entities;
using Easel.Graphics.Structs;
using Easel.Math;
using Easel.Scenes;
using Pie;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Forward rendering is the "traditional" way to render objects. It has its advantages, but also has many disadvantages
/// compared to deferred rendering. As such, deferred rendering is usually preferred for most rendering tasks.
/// </summary>
public sealed class ForwardRenderer : I3DRenderer
{
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

    public ForwardRenderer(EaselGraphics graphics)
    {
        _graphics = graphics;
        GraphicsDevice device = graphics.PieGraphics;
        
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
    }

    /// <inheritdoc />
    public void DrawTranslucent(Renderable renderable, Matrix4x4 world)
    {
        _translucents.Add((renderable, world));
    }

    /// <inheritdoc />
    public void DrawOpaque(Renderable renderable, Matrix4x4 world)
    {
        _opaques.Add((renderable, world));
    }

    /// <inheritdoc />
    public void ClearAll()
    {
        _translucents.Clear();
        _opaques.Clear();
    }

    /// <inheritdoc />
    public void Render(Camera camera, World world)
    {
        GraphicsDevice device = _graphics.PieGraphics;
        _graphics.Clear(world.ClearColor);
        world.Skybox?.Draw(camera);
        _projViewModel.ProjView = camera.ViewMatrix * camera.ProjectionMatrix;

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
        
        foreach ((Renderable renderable, Matrix4x4 mWorld) in _translucents.OrderBy(renderable => -Vector3.Distance(renderable.Item2.Translation, camera.Transform.Position)))
            DrawRenderable(renderable, mWorld);
    }

    private void DrawRenderable(in Renderable renderable, in Matrix4x4 world)
    {
        GraphicsDevice device = _graphics.PieGraphics;
        
        _projViewModel.Model = world;
        device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);

        _cameraInfo.Material = renderable.Material.ShaderMaterial;
        device.UpdateBuffer(_cameraBuffer, 0, _cameraInfo);

        device.SetShader(renderable.Material.EffectLayout.Effect.PieShader);
        device.SetTexture(2, renderable.Material.Albedo?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
        device.SetTexture(3, renderable.Material.Specular?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
        device.SetTexture(4, renderable.Material.Normal?.PieTexture ?? Texture2D.Void.PieTexture, _samplerState);
        device.SetVertexBuffer(0, renderable.VertexBuffer, renderable.Material.EffectLayout.Stride, renderable.Material.EffectLayout.Layout);
        device.SetIndexBuffer(renderable.IndexBuffer, IndexType.UInt);
        device.DrawIndexed(renderable.IndicesLength);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ProjViewModel
    {
        public Matrix4x4 ProjView;
        public Matrix4x4 Model;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CameraInfo
    {
        public ShaderMaterial Material;
        public ShaderDirectionalLight Sun;
        public Vector4 CameraPos;
    }

    public void Dispose()
    {
        _projViewModelBuffer.Dispose();
        _cameraBuffer.Dispose();
        _rasterizerState.Dispose();
        _depthState.Dispose();
        _samplerState.Dispose();
    }
}