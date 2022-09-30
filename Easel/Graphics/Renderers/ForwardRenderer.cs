using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Entities;
using Easel.Graphics.Structs;
using Easel.Scenes;
using Pie;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Forward rendering is the "traditional" way to render objects. It has its advantages, but also has many disadvantages
/// compared to deferred rendering. As such, deferred rendering is usually preferred for most rendering tasks.
/// </summary>
public sealed class ForwardRenderer : I3DRenderer
{
    private GraphicsDevice _device;
    
    private EffectLayout _effectLayout;

    private GraphicsBuffer _projViewModelBuffer;
    private ProjViewModel _projViewModel;

    private CameraInfo _cameraInfo;
    private GraphicsBuffer _cameraBuffer;

    private List<Renderable> _translucents;

    private List<Renderable> _opaques;

    private RasterizerState _rasterizerState;
    private DepthState _depthState;
    private SamplerState _samplerState;

    public ForwardRenderer(GraphicsDevice device, EffectManager manager)
    {
        _device = device;
        
        _translucents = new List<Renderable>();
        _opaques = new List<Renderable>();

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

        _effectLayout = manager.GetEffectLayout(EffectManager.Forward.Standard);
    }

    /// <inheritdoc />
    public void DrawTranslucent(Renderable renderable)
    {
        _translucents.Add(renderable);
    }

    /// <inheritdoc />
    public void DrawOpaque(Renderable renderable)
    {
        _opaques.Add(renderable);
    }

    /// <inheritdoc />
    public void ClearAll()
    {
        _translucents.Clear();
        _opaques.Clear();
    }

    /// <inheritdoc />
    public void Render(Camera camera)
    {
        _projViewModel.ProjView = camera.ViewMatrix * camera.ProjectionMatrix;

        _cameraInfo.Sun = SceneManager.ActiveScene.World.Sun.ShaderDirectionalLight;
        _cameraInfo.CameraPos = new Vector4(camera.Transform.Position, 1);

        foreach (Renderable renderable in _opaques)
        {
            _projViewModel.Model = renderable.ModelMatrix;
            _device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);

            _cameraInfo.Material = renderable.Material.ShaderMaterial;
            _device.UpdateBuffer(_cameraBuffer, 0, _cameraInfo);

            _device.SetShader(_effectLayout.Effect.PieShader);
            _device.SetRasterizerState(_rasterizerState);
            _device.SetDepthState(_depthState);
            _device.SetUniformBuffer(0, _projViewModelBuffer);
            _device.SetUniformBuffer(1, _cameraBuffer);
            _device.SetTexture(2, renderable.Material.Albedo?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
            _device.SetTexture(3, renderable.Material.Specular?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
            _device.SetPrimitiveType(PrimitiveType.TriangleList);
            _device.SetVertexBuffer(renderable.VertexBuffer, _effectLayout.Layout);
            _device.SetIndexBuffer(renderable.IndexBuffer, IndexType.UInt);
            _device.DrawIndexed(renderable.IndicesLength);
        }
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
        _effectLayout.Dispose();
        _projViewModelBuffer.Dispose();
        _cameraBuffer.Dispose();
        _rasterizerState.Dispose();
        _depthState.Dispose();
        _samplerState.Dispose();
    }
}