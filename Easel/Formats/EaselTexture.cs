using System;
using System.Collections.Generic;
using System.IO;
using Easel.Core;
using Easel.Graphics;
using Easel.Math;
using Pie;

namespace Easel.Formats;

public struct EaselTexture
{
    public TextureType Type;
    
    public Size Size;

    public Dictionary<BitmapLayer, Bitmap> Bitmaps;

    public Bitmap[] Cubemap;

    public EaselTexture(Bitmap[] cubemap)
    {
        Type = TextureType.Cubemap;
        Size = cubemap[0].Size;
        foreach (Bitmap bp in cubemap)
        {
            if (bp.Size != Size)
                throw new EaselException("Cubemap bitmaps must all be the same size!");
        }

        Cubemap = cubemap;

        Bitmaps = null;
    }

    public EaselTexture(params (BitmapLayer, Bitmap)[] bitmaps)
    {
        Type = TextureType.Bitmap;
        Size = bitmaps[0].Item2.Size;
        Bitmaps = new Dictionary<BitmapLayer, Bitmap>();
        foreach ((BitmapLayer layer, Bitmap bp) in bitmaps)
        {
            if (bp.Size != Size)
                throw new EaselException("Bitmaps must all be the same size!");
            Bitmaps.Add(layer, bp);
        }

        Cubemap = null;
    }

    public Bitmap GetBitmap(BitmapLayer layer)
    {
        Bitmaps.TryGetValue(layer, out Bitmap bp);
        return bp;
    }

    public bool TryGetBitmap(BitmapLayer layer, out Bitmap bitmap)
    {
        bitmap = GetBitmap(layer);
        return bitmap != null;
    }

    public static EaselTexture Deserialize(byte[] data)
    {
        using MemoryStream stream = new MemoryStream(data);
        using BinaryReader reader = new BinaryReader(stream);

        if (new string(reader.ReadChars(4)) != "ETF ")
            throw new EaselException("Given file is not an Easel texture.");

        reader.ReadUInt16(); // Version

        bool compress = reader.ReadByte() == 1;
        if (compress)
            Logger.Debug("Decompressing Easel Texture...");

        TextureType type = (TextureType) reader.ReadByte();

        Size size = new Size(reader.ReadInt32(), reader.ReadInt32());

        byte[] compData = reader.ReadBytes((int) (reader.BaseStream.Length - reader.BaseStream.Position));
        using MemoryStream compStream = new MemoryStream(compress ? Utils.Decompress(compData) : compData);
        using BinaryReader compReader = new BinaryReader(compStream);

        switch (type)
        {
            case TextureType.Bitmap:
                byte bLength = compReader.ReadByte();
                (BitmapLayer, Bitmap)[] bitmaps = new (BitmapLayer, Bitmap)[bLength];

                for (int i = 0; i < bLength; i++)
                {
                    if (new string(compReader.ReadChars(4)) != "TEXB")
                        throw new EaselException("\"TEXB\" expected, was not found.");
                    
                    BitmapLayer layer = (BitmapLayer) compReader.ReadByte();
                    PixelFormat format = (PixelFormat) compReader.ReadByte();

                    int length = compReader.ReadInt32();
                    byte[] bData = compReader.ReadBytes(length);
                    bitmaps[i] = (layer, new Bitmap(size.Width, size.Height, format, bData));
                }

                return new EaselTexture(bitmaps);

            case TextureType.Cubemap:
                PixelFormat cFmt = (PixelFormat) compReader.ReadByte();

                byte cLength = compReader.ReadByte();

                Bitmap[] cubemaps = new Bitmap[cLength];
                
                for (int i = 0; i < cLength; i++)
                {
                    if (new string(compReader.ReadChars(4)) != "TEXC")
                        throw new EaselException("\"TEXC\" expected, was not found.");

                    int dataLength = compReader.ReadInt32();
                    byte[] cData = compReader.ReadBytes(dataLength);
                    cubemaps[i] = new Bitmap(size.Width, size.Height, cFmt, cData);
                }

                return new EaselTexture(cubemaps);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public byte[] Serialize(bool compress)
    {
        const ushort version = 1;

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        
        // HEADER
        
        writer.Write("ETF ".ToCharArray());
        writer.Write(version);
        writer.Write((byte) (compress ? 1 : 0));
        writer.Write((byte) Type);
        writer.Write(Size.Width);
        writer.Write(Size.Height);

        using MemoryStream compStream = new MemoryStream();
        using BinaryWriter compWriter = new BinaryWriter(compStream);
        
        switch (Type)
        {
            case TextureType.Bitmap:
                compWriter.Write((byte) Bitmaps.Count);
                foreach ((BitmapLayer layer, Bitmap bp) in Bitmaps)
                {
                    compWriter.Write("TEXB".ToCharArray());
                    compWriter.Write((byte) layer);
                    compWriter.Write((byte) bp.Format);
                    
                    compWriter.Write(bp.Data.Length);
                    compWriter.Write(bp.Data);
                }
                
                break;
            
            case TextureType.Cubemap:
                compWriter.Write((byte) Cubemap[0].Format);
                
                compWriter.Write((byte) Cubemap.Length);
                for (int i = 0; i < Cubemap.Length; i++)
                {
                    compWriter.Write("TEXC".ToCharArray());
                    compWriter.Write(Cubemap[i].Data.Length);
                    compWriter.Write(Cubemap[i].Data);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (compress)
            writer.Write(Utils.Compress(compStream.ToArray()));
        else
            writer.Write(compStream.ToArray());

        return stream.ToArray();
    }

    public enum BitmapLayer : byte
    {
        Albedo,
        Specular,
        Normal,
        Roughness
    }

    public enum TextureType : byte
    {
        Bitmap,
        Cubemap
    }
}