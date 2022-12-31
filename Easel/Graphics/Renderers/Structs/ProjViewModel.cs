using System.Numerics;

namespace Easel.Graphics.Renderers.Structs;

public struct ProjViewModel
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;
    public Matrix4x4 Model;

    public ProjViewModel()
    {
        Projection = Matrix4x4.Identity;
        View = Matrix4x4.Identity;
        Model = Matrix4x4.Identity;
    }
}