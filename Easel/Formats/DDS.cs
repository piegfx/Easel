using System;
using System.IO;
using System.Runtime.InteropServices;
using Easel.Core;
using Easel.Graphics;
using Pie;

namespace Easel.Formats;

public class DDS
{
    public Bitmap Test;
    
    public DDS(byte[] file)
    {
        using MemoryStream stream = new MemoryStream(file);
        using BinaryReader reader = new BinaryReader(stream);
        
        // 'DDS '
        if (reader.ReadUInt32() != 0x20534444)
            throw new EaselException("Given file is not a DDS file (identifier missing).");

        if (reader.ReadUInt32() != 124)
            throw new EaselException("Invalid DDS file (size of structure is not 124).");

        uint flags = reader.ReadUInt32();

        uint height = reader.ReadUInt32();
        uint width = reader.ReadUInt32();

        uint pitchOrLinearSize = reader.ReadUInt32();

        reader.ReadUInt32(); // depth, not supported

        uint numMipmaps = reader.ReadUInt32();

        #region DDS_PIXELFORMAT
        
        reader.ReadBytes(11 * sizeof(uint)); // 11 byte DWORD reserved

        if (reader.ReadUInt32() != 32)
            throw new EaselException("An error occurred while reading the DDS file (invalid pixel format).");

        uint fFlags = reader.ReadUInt32();
        bool isCompressed = (fFlags & 0x4) == 0x4;

        uint fourCc = reader.ReadUInt32();

        bool containsDx10Header = fourCc == 0x30315844;

        // TODO: The rest of the pixel format.
        reader.ReadBytes(5 * sizeof(uint));
        
        // TODO: The rest of the header.
        reader.ReadBytes(5 * sizeof(uint));
        
        #endregion
        
        if (isCompressed)
        {
            byte[] initData = reader.ReadBytes((int) (pitchOrLinearSize));
            Test = new Bitmap((int) width, (int) height, Format.BC3_UNorm, initData);
        }
        else
        {
            byte[] initData = reader.ReadBytes((int) (pitchOrLinearSize * width));
            Test = new Bitmap((int) width, (int) height, Format.B8G8R8A8_UNorm, initData);
        }
    }
}