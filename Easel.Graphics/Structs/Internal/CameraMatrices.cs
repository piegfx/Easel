using System.Numerics;

namespace Easel.Graphics.Structs.Internal;

internal struct CameraMatrices
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;

    public CameraMatrices()
    {
        Projection = Matrix4x4.Identity;
        View = Matrix4x4.Identity;
    }

    public CameraMatrices(Matrix4x4 projection, Matrix4x4 view)
    {
        Projection = projection;
        View = view;
    }
}