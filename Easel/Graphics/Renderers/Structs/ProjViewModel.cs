using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ProjViewModel
{
    public Matrix4x4 ProjView;
    public Matrix4x4 Model;
}