using System;

namespace Easel.Utilities;

public static class Utils
{
    public static float NextFloat(this Random random, float min, float max)
    {
        return EaselMath.Lerp(min, max, random.NextSingle());
    }
}