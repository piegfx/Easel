using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct CameraMatrices
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;
}