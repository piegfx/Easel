using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential, Size = 48)]
public struct ShaderDirLight
{
    public Color DiffuseColor;
    public Color SpecularColor;
    public Vector3 Direction;
}