using Easel.Math;
using Pie;

namespace Easel.Graphics;

public class RenderTarget2D : Texture2D
{
    public Framebuffer PieFramebuffer;
    public Texture PieDepthTexture;

    public RenderTarget2D(Size<int> size, Format format = Format.R8G8B8A8_UNorm, Format? depthFormat = Format.D32_Float,
        int mipLevels = 1)
        : this(null, size, format, depthFormat, mipLevels) { }

    public RenderTarget2D(string path, Format? depthFormat = Format.D32_Float, int mipLevels = 1)
        : this(new Bitmap(path), depthFormat, mipLevels) { }
    
    public RenderTarget2D(Bitmap bitmap, Format? depthFormat = Format.D32_Float, int mipLevels = 1)
        : this(bitmap.Data, bitmap.Size, bitmap.Format, depthFormat, mipLevels) { }
    
    public RenderTarget2D(byte[] data, Size<int> size, Format format = Format.R8G8B8A8_UNorm, Format? depthFormat = Format.D32_Float, int mipLevels = 1)
        : this(TextureDescription.Texture2D(size.Width, size.Height, format, mipLevels, 1, 
            TextureUsage.Framebuffer | TextureUsage.ShaderResource), data, depthFormat) { }

    public RenderTarget2D(in TextureDescription description, byte[] data, Format? depthFormat = Format.D32_Float) :
        base(in description, data)
    {
        FramebufferAttachment[] attachments;

        GraphicsDevice device = Renderer.Instance.Device;
        
        if (depthFormat != null)
        {
            PieDepthTexture = device.CreateTexture(TextureDescription.Texture2D(description.Width, description.Height,
                depthFormat.Value, 1, 1, TextureUsage.Framebuffer));

            attachments = new[]
            {
                new FramebufferAttachment(PieTexture),
                new FramebufferAttachment(PieDepthTexture)
            };
        }
        else
        {
            attachments = new[]
            {
                new FramebufferAttachment(PieTexture)
            };
        }

        PieFramebuffer = device.CreateFramebuffer(attachments);
    }

    public RenderTarget2D(Framebuffer pieBuffer, Texture colorTexture, Texture depthTexture, Size<int> size)
        : base(colorTexture, size)
    {
        PieFramebuffer = pieBuffer;
        PieDepthTexture = depthTexture;
    }

    public override void Dispose()
    {
        base.Dispose();
        
        PieDepthTexture?.Dispose();
        PieFramebuffer.Dispose();
    }
}