using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Graphics.Structs;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct CameraInfo
{
    public ShaderMaterial Material;
    public ShaderDirectionalLight Sun;
    public Vector4 CameraPos;
}