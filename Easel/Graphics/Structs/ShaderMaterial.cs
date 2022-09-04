using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ShaderMaterial
{
    public Color Color;
    //public Vector4 Tiling;
    public int Specular;
}