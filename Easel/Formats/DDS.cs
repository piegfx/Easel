using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Easel.Formats;

public class DDS
{
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

        reader.ReadUInt32(); // Pitch, not needed as Pie works this out automatically.

        reader.ReadUInt32(); // depth, not supported

        uint numMipmaps = reader.ReadUInt32();

        reader.ReadBytes(44); // 11 byte DWORD reserved

        if (reader.ReadUInt32() != 32)
            throw new EaselException("An error occurred while reading the DDS file (invalid pixel format).");
    }
}