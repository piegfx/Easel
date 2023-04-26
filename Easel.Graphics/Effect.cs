using System;
using Pie;
using Pie.ShaderCompiler;

namespace Easel.Graphics;

public sealed class Effect : IDisposable
{
    public Pie.Shader DeviceShader;

    public Effect(byte[] vertexSpirv, byte[] fragmentSpirv, SpecializationConstant[] constants = null)
    {
        GraphicsDevice device = Renderer.Instance.Device;

        // Easel (and pie) can only natively understand SPIR-V, and Easel heavily encourages usage of SPIR-V compiled
        // shaders, instead of runtime compiling GLSL or HLSL.
        DeviceShader =
            device.CreateShader(
                new[]
                {
                    new ShaderAttachment(ShaderStage.Vertex, vertexSpirv),
                    new ShaderAttachment(ShaderStage.Fragment, fragmentSpirv)
                }, constants);
    }

    public void Dispose()
    {
        DeviceShader.Dispose();
    }
}