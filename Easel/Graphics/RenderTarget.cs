using Easel.Math;
using Pie;

namespace Easel.Graphics;

public class RenderTarget : Texture
{
    public readonly Framebuffer PieBuffer;
    private Pie.Texture _depth;
    
    public RenderTarget(Size size, SamplerState samplerState = null, bool autoDispose = true) 
        : base(samplerState ?? SamplerState.LinearRepeat, autoDispose)
    {
        // TODO: RGB render targets that draw by ignoring the alpha value in the frag shader (since D3D doesn't support RGB)
        
        TextureDescription description = new TextureDescription(TextureType.Texture2D, size.Width, size.Height,
            PixelFormat.B8G8R8A8_UNorm, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer);

        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        PieTexture = device.CreateTexture(description);

        description.Format = PixelFormat.D24_UNorm_S8_UInt;
        description.Usage = TextureUsage.Framebuffer;

        _depth = device.CreateTexture(description);

        PieBuffer = device.CreateFramebuffer(new FramebufferAttachment(PieTexture, AttachmentType.Color),
            new FramebufferAttachment(_depth, AttachmentType.DepthStencil));
    }

    public override void Dispose()
    {
        PieBuffer.Dispose();
        //_depth.Dispose();
        
        base.Dispose();
    }
}