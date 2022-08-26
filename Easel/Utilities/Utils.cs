using System;

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
}