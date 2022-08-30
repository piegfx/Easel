using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Easel.Utilities;

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
    /// Normalize the given color to a Vector4 value.
    /// </summary>
    /// <param name="color">The color to normalize.</param>
    /// <returns>The normalized color.</returns>
    public static Vector4 Normalize(this Color color)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    /// <summary>
    /// Load an embedded resource with the given name.
    /// </summary>
    /// <param name="assemblyName">The assembly name of the resource to load.</param>
    /// <returns>The loaded resource.</returns>
    public static byte[] LoadEmbeddedResource(string assemblyName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyName);
        using MemoryStream memoryStream = new MemoryStream();
        stream!.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static string LoadEmbeddedString(string assemblyName)
    {
        return Encoding.UTF8.GetString(LoadEmbeddedResource(assemblyName));
    }
}