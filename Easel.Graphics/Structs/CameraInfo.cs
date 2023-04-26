using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Structs;

public struct CameraInfo
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;

    public Vector3 CameraPosition;

    public Color? ClearColor;

    // public Skybox Skybox

    public CameraInfo(Matrix4x4 projection, Matrix4x4 view, Color? clearColor)
    {
        Projection = projection;
        View = view;
        ClearColor = clearColor;

        CameraPosition = Matrix4x4.Invert(view, out Matrix4x4 inverted) ? inverted.Translation : Vector3.Zero;
    }
}