using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ProjViewModel
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;
    public Matrix4x4 Model;
    public Matrix4x4 LightSpace;

    public ProjViewModel()
    {
        Projection = Matrix4x4.Identity;
        View = Matrix4x4.Identity;
        Model = Matrix4x4.Identity;
        LightSpace = Matrix4x4.Identity;
    }
}