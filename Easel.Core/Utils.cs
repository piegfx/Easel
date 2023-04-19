using System;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Text;
using Easel.Math;
using Mth = System.Math;

namespace Easel.Core;

/// <summary>
/// Provides certain utilities and extension methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Get a random value between the min (inclusive) and max (exclusive) value.
    /// </summary>
    /// <param name="random">The random instance.</param>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <returns></returns>
    public static float NextFloat(this Random random, float min, float max)
    {
        return EaselMath.Lerp(min, max, random.NextSingle());
    }

    /// <summary>
    /// Load an embedded resource with the given name.
    /// </summary>
    /// <param name="assemblyName">The assembly name of the resource to load.</param>
    /// <returns>The loaded resource.</returns>
    public static byte[] LoadEmbeddedResource(Assembly assembly, string assemblyName)
    {
        using Stream stream = assembly.GetManifestResourceStream(assemblyName);
        using MemoryStream memoryStream = new MemoryStream();
        stream!.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static string LoadEmbeddedString(Assembly assembly, string assemblyName, Encoding encoding)
    {
        return encoding.GetString(LoadEmbeddedResource(assembly, assemblyName));
    }

    public static string LoadEmbeddedString(Assembly assembly, string assemblyName) => LoadEmbeddedString(assembly, assemblyName, Encoding.UTF8);

    public static byte[] Compress(byte[] data, CompressionLevel level = CompressionLevel.Optimal)
    {
        using MemoryStream stream = new MemoryStream();
        using DeflateStream deflate = new DeflateStream(stream, level);
        deflate.Write(data, 0, data.Length);
        return stream.ToArray();
    }
    
    public static byte[] Decompress(byte[] data)
    {
        using MemoryStream stream = new MemoryStream(data);
        using MemoryStream output = new MemoryStream();
        using DeflateStream deflate = new DeflateStream(stream, CompressionMode.Decompress);
        deflate.CopyTo(output);
        return output.ToArray();
    }
}