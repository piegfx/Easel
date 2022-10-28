using Easel.Entities;
using Easel.Math;
using Pie;
using PTex = Pie.Texture;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Deferred rendering is Easel's primary rendering method. It provides a way to quickly render thousands of lights,
/// and is currently the only renderer that supports any form of lighting, outside of the directional light (called Sun
/// in Easel).
/// </summary>
public class DeferredRenderer : I3DRenderer
{
    public readonly Framebuffer GBuffer;
    public readonly PTex PositionTexture;
    public readonly PTex NormalTexture;
    public readonly PTex AlbedoTexture;
    public readonly PTex SpecularTexture;

    public DeferredRenderer()
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        int texWidth = 1280;
        int texHeight = 720;
        
        PositionTexture = device.CreateTexture<byte>(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R8G8B8A8_UNorm, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer), null);
        NormalTexture = device.CreateTexture<byte>(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R8G8B8A8_UNorm, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer), null);
        AlbedoTexture = device.CreateTexture<byte>(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R8G8B8A8_UNorm, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer), null);
        SpecularTexture = device.CreateTexture<byte>(new TextureDescription(TextureType.Texture2D, texWidth, texHeight, PixelFormat.R8G8B8A8_UNorm, 1, 1, TextureUsage.ShaderResource | TextureUsage.Framebuffer), null);
        
        GBuffer = device.CreateFramebuffer(new FramebufferAttachment(PositionTexture, AttachmentType.Color),
            new FramebufferAttachment(NormalTexture, AttachmentType.Color),
            new FramebufferAttachment(AlbedoTexture, AttachmentType.Color),
            new FramebufferAttachment(SpecularTexture, AttachmentType.Color));
    }

    public PostProcessor PostProcessor { get; }

    public void DrawTranslucent(Renderable renderable)
    {
        throw new System.NotImplementedException();
    }

    public void DrawOpaque(Renderable renderable)
    {
        throw new System.NotImplementedException();
    }

    public void ClearAll()
    {
        throw new System.NotImplementedException();
    }

    public void Render(Camera camera, Color clearColor)
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}