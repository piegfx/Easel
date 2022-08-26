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

public static class ForwardRenderer
{
    public const string TempVertex = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjView;
    mat4 uModel;
};

void main()
{
    gl_Position = uProjView * uModel * vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

    public const string TempFragment = @"
#version 450

layout (location = 0) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
}";

    private static EffectLayout _effectLayout;

    private static GraphicsBuffer _projViewModelBuffer;
    private static ProjViewModel _projViewModel;

    private static List<Renderable> _translucents;

    private static List<Renderable> _opaques;

    private static RasterizerState _rasterizerState;

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

        _rasterizerState = device.CreateRasterizerState();

        _effectLayout = BuiltinEffects.GetEffectLayout(BuiltinEffects.Forward.StandardUnlit);
    }

    public static void DrawTranslucent(Renderable renderable)
    {
        _translucents.Add(renderable);
    }

    public static void DrawOpaque(Renderable renderable)
    {
        _opaques.Add(renderable);
    }

    public static void ClearAll()
    {
        _translucents.Clear();
        _opaques.Clear();
    }

    public static void Render()
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        
        Camera main = Camera.Main;
        _projViewModel.ProjView = main.ViewMatrix * main.ProjectionMatrix;

        foreach (Renderable renderable in _opaques)
        {
            _projViewModel.Model = renderable.ModelMatrix;
            _projViewModelBuffer.Update(0, _projViewModel);

            device.SetShader(_effectLayout.Effect.PieShader);
            device.SetRasterizerState(_rasterizerState);
            device.SetUniformBuffer(0, _projViewModelBuffer);
            device.SetTexture(1, renderable.Texture.PieTexture);
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