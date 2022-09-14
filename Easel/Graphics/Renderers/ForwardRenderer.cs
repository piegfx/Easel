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
public static class ForwardRenderer
{
    private static EffectLayout _effectLayout;

    private static GraphicsBuffer _projViewModelBuffer;
    private static ProjViewModel _projViewModel;

    private static CameraInfo _cameraInfo;
    private static GraphicsBuffer _cameraBuffer;

    private static List<Renderable> _translucents;

    private static List<Renderable> _opaques;

    private static RasterizerState _rasterizerState;
    private static DepthState _depthState;
    private static SamplerState _samplerState;

    static ForwardRenderer()
    {
        _translucents = new List<Renderable>();
        _opaques = new List<Renderable>();
        
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;

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

        _effectLayout = BuiltinEffects.GetEffectLayout(BuiltinEffects.Forward.Standard);
    }

    /// <summary>
    /// Draw a translucent object. These objects are drawn back-to-front to allow transparency to work.
    /// </summary>
    /// <param name="renderable">The renderable object.</param>
    public static void DrawTranslucent(Renderable renderable)
    {
        _translucents.Add(renderable);
    }

    /// <summary>
    /// Draw an opaque object. These objects are draw front-to-back so the GPU won't process fragments that are covered
    /// by other fragments.
    /// </summary>
    /// <param name="renderable"></param>
    public static void DrawOpaque(Renderable renderable)
    {
        _opaques.Add(renderable);
    }

    /// <summary>
    /// Clear all draw lists and prepare the renderer for a new frame.
    /// </summary>
    public static void ClearAll()
    {
        _translucents.Clear();
        _opaques.Clear();
    }

    /// <summary>
    /// Render all draw lists and perform post-processing.
    /// </summary>
    public static void Render(Camera camera)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        
        _projViewModel.ProjView = camera.ViewMatrix * camera.ProjectionMatrix;

        _cameraInfo.Sun = SceneManager.ActiveScene.World.Sun.ShaderDirectionalLight;
        _cameraInfo.CameraPos = new Vector4(camera.Transform.Position, 1);

        foreach (Renderable renderable in _opaques)
        {
            _projViewModel.Model = renderable.ModelMatrix;
            device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);

            _cameraInfo.Material = renderable.Material.ShaderMaterial;
            device.UpdateBuffer(_cameraBuffer, 0, _cameraInfo);

            device.SetShader(_effectLayout.Effect.PieShader);
            device.SetRasterizerState(_rasterizerState);
            device.SetDepthState(_depthState);
            device.SetUniformBuffer(0, _projViewModelBuffer);
            device.SetUniformBuffer(1, _cameraBuffer);
            device.SetTexture(2, renderable.Material.Albedo?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
            device.SetTexture(3, renderable.Material.Specular?.PieTexture ?? Texture2D.Missing.PieTexture, _samplerState);
            device.SetPrimitiveType(PrimitiveType.TriangleList);
            device.SetVertexBuffer(renderable.VertexBuffer, _effectLayout.Layout);
            device.SetIndexBuffer(renderable.IndexBuffer);
            device.Draw(renderable.IndicesLength);
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
}