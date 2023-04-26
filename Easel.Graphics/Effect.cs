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