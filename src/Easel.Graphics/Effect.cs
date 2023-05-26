using System;
using Easel.Core;
using Pie;
using Pie.ShaderCompiler;

namespace Easel.Graphics;

public sealed class Effect : IDisposable
{
    public readonly Shader PieShader;
    public readonly InputLayout InputLayout;

    public Effect(byte[] vertexSpirv, byte[] fragmentSpirv, InputLayoutDescription[] inputLayout,
        SpecializationConstant[] specializationConstants = null)
    {
        GraphicsDevice device = Renderer.Instance.Device;
        
        Logger.Debug("Compiling shader.");

        PieShader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, vertexSpirv),
            new ShaderAttachment(ShaderStage.Fragment, fragmentSpirv)
        }, specializationConstants);
        
        Logger.Debug("Creating input layout.");
        InputLayout = device.CreateInputLayout(inputLayout);
    }

    public void Dispose()
    {
        PieShader.Dispose();
        InputLayout.Dispose();
    }
}