using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Easel.Math;

public struct TVector2
{
    public float X;
    public float Y;

    public static TVector2 Zero = new TVector2(0);
    public static TVector2 One = new TVector2(0);

    public static TVector2 UnitX = new TVector2(1, 0);
    public static TVector2 UnitY = new TVector2(0, 1);

    public TVector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public TVector2(float xy)
    {
        X = xy;
        Y = xy;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe TVector2 Add(TVector2 a, TVector2 b)
    {
        /*if (Sse2.IsSupported)
        {
            Vector128<float> result = Sse2.Add(
                Vector128.Create(a.X, a.Y, 0, 0),
                Vector128.Create(b.X, b.Y, 0, 0));
            return new TVector2(result.GetElement(0), result.GetElement(1));
        }*/
        
        return new TVector2(a.X + b.X, a.Y + b.Y);
    }

    public override string ToString()
    {
        return "Vector2(X: " + X + ", Y: " + Y + ")";
    }
}