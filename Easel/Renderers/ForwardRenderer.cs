using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Entities;
using Easel.Graphics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Easel.Renderers;

/// <summary>
/// Forward rendering is the "traditional" way to render objects. It has its advantages, but also has many disadvantages
/// compared to deferred rendering. As such, deferred rendering is usually preferred for most rendering tasks.
/// </summary>
public static class ForwardRenderer
{
    private static EffectLayout _effectLayout;

    private static GraphicsBuffer _projViewModelBuffer;
    private static ProjViewModel _projViewModel;

    private static GraphicsBuffer _tilingBuffer;

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

        _tilingBuffer = device.CreateBuffer(BufferType.UniformBuffer, Vector4.One, true);

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
    public static void Render()
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        
        Camera main = Camera.Main;
        _projViewModel.ProjView = main.ViewMatrix * main.ProjectionMatrix;

        foreach (Renderable renderable in _opaques)
        {
            _projViewModel.Model = renderable.ModelMatrix;
            device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);
            device.UpdateBuffer(_tilingBuffer, 0, renderable.TilingAmount);

            device.SetShader(_effectLayout.Effect.PieShader);
            device.SetRasterizerState(_rasterizerState);
            device.SetDepthState(_depthState);
            device.SetUniformBuffer(0, _projViewModelBuffer);
            device.SetUniformBuffer(1, _tilingBuffer);
            device.SetTexture(2, renderable.Texture.PieTexture, _samplerState);
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
}