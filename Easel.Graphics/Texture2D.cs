using System;
using Easel.Math;
using Pie;

namespace Easel.Graphics;

public class Texture2D : IDisposable
{
    public Pie.Texture DeviceTexture;

    public Size<int> Size => new Size<int>(DeviceTexture.Description.Width, DeviceTexture.Description.Height);

    public Texture2D(string path, int mipLevels = 0, bool generateMipmaps = true) : this(new Bitmap(path), mipLevels,
        generateMipmaps) { }

    public Texture2D(Bitmap bitmap, int mipLevels = 0, bool generateMipmaps = true) : this(bitmap.Data, bitmap.Size,
        bitmap.Format, mipLevels, generateMipmaps) { }

    public Texture2D(byte[] data, Size<int> size, Format format = Format.R8G8B8A8_UNorm, int mipLevels = 0, bool generateMipmaps = true)
    {
        TextureDescription description = new TextureDescription(size.Width, size.Height, format, mipLevels, 1,
            TextureUsage.ShaderResource);
        CreateTexture(description, data, generateMipmaps);
    }
    
    protected Texture2D() { }

    protected void CreateTexture(TextureDescription description, byte[] data, bool generateMipmaps = true)
    {
        GraphicsDevice device = Renderer.Instance.Device;
        DeviceTexture = device.CreateTexture(description, data);
        
        if (generateMipmaps)
            device.GenerateMipmaps(DeviceTexture);
    }

    public void GenerateMipmaps()
    {
        GraphicsDevice device = Renderer.Instance.Device;
        device.GenerateMipmaps(DeviceTexture);
    }

    public virtual void Dispose()
    {
        DeviceTexture.Dispose();
    }

    public static readonly Texture2D White = new Texture2D(new byte[] { 255, 255, 255, 255 }, new Size<int>(1));
    
    public static readonly Texture2D Black = new Texture2D(new byte[] { 0, 0, 0, 255 }, new Size<int>(1));

    public static readonly Texture2D EmptyNormal = new Texture2D(new byte[] { 128, 128, 255, 255 }, new Size<int>(1));
}