using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Graphics.Renderers.Structs;

/// <summary>
/// Contains all the possible material parameters that can be sent to the shader.
/// Some shaders will completely ignore certain parameters.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct ShaderMaterial
{
    public Vector2 Tiling;
    
    public float Shininess;
}