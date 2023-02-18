using System.IO;
using Easel.Graphics;
using Pie;

namespace Easel.Formats;

public class ETF
{
    public static byte[] CreateEtf(Bitmap bitmap, Format? desiredFormat = null)
    {
        int mipLevels = 1;
        int arraySize = 1;
        
        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(0x20465445);
        
        writer.Write(1);

        #region EtfHeader

        writer.Write(bitmap.Size.Width);
        
        writer.Write(bitmap.Size.Height);

        byte flags = 0;
        if (mipLevels > 1)
            flags |= 0x1;
        if (arraySize > 1)
            flags |= 0x2;
        
        writer.Write(flags);

        Format format = desiredFormat ?? bitmap.Format;
        writer.Write((byte) format);
        
        if (mipLevels > 1)
            writer.Write((byte) mipLevels);
        if (arraySize > 1)
            writer.Write(arraySize);
        
        writer.Write(bitmap.Size.Width * bitmap.Size.Height * PieUtils.CalculateBitsPerPixel(format));

        #endregion



        for (int m = 0; m < mipLevels; m++)
        {
            writer.Write(bitmap.Data);
        }

        return stream.ToArray();
    }
}