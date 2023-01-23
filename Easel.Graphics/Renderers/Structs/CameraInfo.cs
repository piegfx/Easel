using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

public struct CameraInfo
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;
    public Vector3 Position;

    public Color ClearColor;
    public Skybox Skybox;
}