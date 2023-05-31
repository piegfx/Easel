using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Graphics.Structs;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct RenderInfo
{
    public Matrix4x4 WorldMatrix;
    public SceneInfo SceneInfo;
    public Material Material;
}