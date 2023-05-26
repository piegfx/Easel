using System;
using Easel.Core;
using Easel.Math;
using Pie;
using Color = System.Drawing.Color;

namespace Easel.Graphics.Lighting;

public class ShadowMap : IDisposable
{
    internal Framebuffer[] Framebuffers;
    internal Pie.Texture[] Textures;
    public Pie.SamplerState SamplerState;
    
    public ShadowMap(Size<int> size, int numCascades)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        
        // TODO: USE TEXTURE ARRAYS!!!!!!!!!!!!!11111111111

        Textures = new Pie.Texture[numCascades];
        Framebuffers = new Framebuffer[numCascades];
        
        for (int i = 0; i < numCascades; i++)
        {
            Textures[i] = device.CreateTexture(TextureDescription.Texture2D(size.Width, size.Height, Format.D32_Float,
                1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer));
            Framebuffers[i] = device.CreateFramebuffer(new FramebufferAttachment(Textures[i]));
        }

        SamplerState = device.CreateSamplerState(new SamplerStateDescription(TextureFilter.MinMagMipLinear,
            TextureAddress.ClampToBorder, TextureAddress.ClampToBorder, TextureAddress.ClampToBorder, 0, Color.White, 0,
            float.MaxValue));
    }

    public void Dispose()
    {
        for (int i = 0; i < Framebuffers.Length; i++)
        {
            Framebuffers[i].Dispose();
            Textures[i].Dispose();
        }

        SamplerState.Dispose();
    }
}