using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using Easel.Math;

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
    
    public static Vector3 ToEulerAngles(this Quaternion quat)
    {
        // Convert our values to euler angles.
        // https://math.stackexchange.com/questions/2975109/how-to-convert-euler-angles-to-quaternions-and-get-the-same-euler-angles-back-fr

        float yaw = MathF.Asin(EaselMath.Clamp(2f * (quat.W * quat.Y - quat.Z * quat.X), -1f, 1f));
        
        float pitch = MathF.Atan2(2f * (quat.W * quat.X + quat.Y * quat.Z),
            1f - 2f * (quat.X * quat.X + quat.Y * quat.Y));

        float roll = MathF.Atan2(2f * (quat.W * quat.Z + quat.X * quat.Y),
            1f - 2f * (quat.Y * quat.Y + quat.Z * quat.Z));

        return new Vector3(yaw, pitch, roll);
    }

    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2(vector3.X, vector3.Y);
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