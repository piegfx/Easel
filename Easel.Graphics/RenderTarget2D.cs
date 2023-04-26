using Easel.Math;
using Pie;

namespace Easel.Graphics;

public class RenderTarget2D : Texture2D
{
    public Framebuffer DeviceBuffer;
    public Pie.Texture DepthTexture;
    
    public RenderTarget2D(Size<int> size, Format format = Format.R8G8B8A8_UNorm, Format? depthFormat = Format.D24_UNorm_S8_UInt)
    {
        GraphicsDevice device = Renderer.Instance.Device;

        DeviceTexture = device.CreateTexture(new TextureDescription(size.Width, size.Height, format, 1, 1,
            TextureUsage.ShaderResource | TextureUsage.Framebuffer));
        if (depthFormat != null)
        {
            DepthTexture = device.CreateTexture(new TextureDescription(size.Width, size.Height, depthFormat.Value, 1, 1,
                TextureUsage.Framebuffer | TextureUsage.ShaderResource));
        }

        DeviceBuffer = device.CreateFramebuffer(new FramebufferAttachment(DeviceTexture),
            new FramebufferAttachment(DepthTexture));
    }

    public override void Dispose()
    {
        DeviceBuffer.Dispose();
        if (DepthTexture != null)
            DepthTexture.Dispose();
        
        base.Dispose();
    }
}