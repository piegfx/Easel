using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ShaderDirectionalLight
{
    public Vector4 Direction;
    public Color Ambient;
    public Color Diffuse;
    public Color Specular;
}