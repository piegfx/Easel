using System;
using Easel.Core;
using Easel.Math;
using Pie;

namespace Easel.Graphics;

public class Texture2D : IDisposable
{
    public readonly Texture PieTexture;

    public readonly Size<int> Size;

    public Texture2D(string path, int mipLevels = 0) : this(new Bitmap(path), mipLevels) { }

    public Texture2D(Bitmap bitmap, int mipLevels = 0) : this(bitmap.Data, bitmap.Size, bitmap.Format, mipLevels) { }

    public Texture2D(byte[] data, Size<int> size, Format format = Format.R8G8B8A8_UNorm, int mipLevels = 0)
    {
        Size = size;

        TextureDescription description = TextureDescription.Texture2D(size.Width, size.Height, format, mipLevels, 1,
            TextureUsage.ShaderResource);

        GraphicsDevice device = Renderer.Instance.Device;
        PieTexture = device.CreateTexture(description, data);
        
        if (mipLevels != 1)
            device.GenerateMipmaps(PieTexture);
    }

    public Texture2D(Texture pieTexture)
    {
        if (pieTexture.Description.TextureType != TextureType.Texture2D)
            throw new EaselException("Texture type must be Texture2D!");
    }

    public void SetData<T>(int x, int y, int width, int height, T[] data) where T : unmanaged
    {
        Renderer.Instance.Device.UpdateTexture(PieTexture, 0, 0, x, y, 0, width, height, 0, data);
    }

    public void GenerateMipmaps()
    {
        Renderer.Instance.Device.GenerateMipmaps(PieTexture);
    }

    public void Dispose()
    {
        PieTexture.Dispose();
    }

    static Texture2D()
    {
        White = new Texture2D(new byte[] { 255, 255, 255, 255 }, new Size<int>(1), mipLevels: 1);
        Black = new Texture2D(new byte[] { 0, 0, 0, 255 }, new Size<int>(1), mipLevels: 1);
    }
    
    public static Texture2D White { get; }
    
    public static Texture2D Black { get; }
}