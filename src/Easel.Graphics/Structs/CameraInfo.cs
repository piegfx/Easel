using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Structs;

public struct CameraInfo
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;
    public Color? ClearColor;
    // public Skybox Skybox;

    public Vector3 WorldPosition;

    public CameraInfo(Matrix4x4 projection, Matrix4x4 view, Color? clearColor, Vector3 worldPosition)
    {
        Projection = projection;
        View = view;
        ClearColor = clearColor;
        WorldPosition = worldPosition;
    }
}