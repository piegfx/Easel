using System;
using System.Reflection;
using Easel.Core;
using Pie;
using Pie.ShaderCompiler;

namespace Easel.Graphics;

public sealed class Effect : IDisposable
{
    public readonly Shader PieShader;
    public readonly InputLayout InputLayout;
    public readonly uint Stride;

    public Effect(byte[] vertexSpirv, byte[] fragmentSpirv, InputLayoutDescription[] inputLayout, uint stride,
        SpecializationConstant[] specializationConstants = null)
    {
        GraphicsDevice device = Renderer.Instance.Device;
        
        Logger.Debug("Compiling shader.");

        PieShader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, vertexSpirv),
            new ShaderAttachment(ShaderStage.Fragment, fragmentSpirv)
        }, specializationConstants);

        if (inputLayout == null)
        {
            Logger.Debug("Input layout is null - one will not be created.");
            return;
        }

        Logger.Debug("Creating input layout.");
        InputLayout = device.CreateInputLayout(inputLayout);
        Stride = stride;
    }

    public static Effect FromSpirv(string vertex, string fragment, InputLayoutDescription[] inputLayout, uint stride,
        Assembly assembly, SpecializationConstant[] specializationConstants = null)
    {
        byte[] vShader = Utils.LoadEmbeddedResource(assembly, vertex);
        byte[] fShader = Utils.LoadEmbeddedResource(assembly, fragment);

        return new Effect(vShader, fShader, inputLayout, stride, specializationConstants);
    }

    public void Dispose()
    {
        PieShader.Dispose();
        InputLayout?.Dispose();
    }
}