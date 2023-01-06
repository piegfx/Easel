using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

/// <summary>
/// Contains all the possible material parameters that can be sent to the shader.
/// Some shaders will completely ignore certain parameters.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 32)]
public struct ShaderMaterial
{
    public Color Color;
    
    public Vector2 Tiling;
    
    public float Shininess;
}