using Easel.Math;
using Pie;

namespace Easel.Graphics;

public class RenderTarget : TextureObject
{
    public Framebuffer PieBuffer;
    private Texture _depth;
    
    public RenderTarget(Size size, bool autoDispose = true) : base(autoDispose)
    {
        TextureDescription description = new TextureDescription(TextureType.Texture2D, size.Width, size.Height,
            PixelFormat.B8G8R8A8_UNorm, false, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer);

        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        PieTexture = device.CreateTexture<byte>(description, null);

        description.Format = PixelFormat.D24_UNorm_S8_UInt;
        description.Usage = TextureUsage.DepthStencil;

        _depth = device.CreateTexture<byte>(description, null);

        PieBuffer = device.CreateFramebuffer(new FramebufferAttachment(PieTexture, AttachmentType.Color),
            new FramebufferAttachment(_depth, AttachmentType.DepthStencil));
    }

    public override void Dispose()
    {
        PieBuffer.Dispose();
        _depth.Dispose();
        
        base.Dispose();
    }
}