using System;
using Easel.Core;
using Easel.Math;
using Pie;
using Color = System.Drawing.Color;

namespace Easel.Graphics.Lighting;

public class ShadowMap : IDisposable
{
    internal Framebuffer Framebuffer;
    private Pie.Texture _texture;
    public Pie.SamplerState SamplerState;
    
    public ShadowMap(Size<int> size, int numCascades)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        _texture = device.CreateTexture(new TextureDescription(size.Width, size.Height, Format.D32_Float, 1, 1,
            TextureUsage.ShaderResource | TextureUsage.Framebuffer));
        Framebuffer = device.CreateFramebuffer(new FramebufferAttachment(_texture));

        SamplerState = device.CreateSamplerState(new SamplerStateDescription(TextureFilter.MinMagMipLinear,
            TextureAddress.ClampToBorder, TextureAddress.ClampToBorder, TextureAddress.ClampToBorder, 0, Color.Black, 0,
            float.MaxValue));
    }

    public void Dispose()
    {
        Framebuffer.Dispose();
        _texture.Dispose();
        SamplerState.Dispose();
    }
}