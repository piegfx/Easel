using System.Drawing;
using System.Numerics;
using Easel.Scenes;

namespace Easel.Entities;

public class Camera : Entity
{
    public Matrix4x4 ProjectionMatrix { get; private set; }

    public Matrix4x4 ViewMatrix =>
        Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);

    private float _fov;
    private float _aspectRatio;
    private float _near;
    private float _far;

    public float FieldOfView
    {
        get => _fov;
        set
        {
            _fov = value;
            GenerateProjectionMatrix();
        }
    }

    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            GenerateProjectionMatrix();
        }
    }

    public float NearPlane
    {
        get => _near;
        set
        {
            _near = value;
            GenerateProjectionMatrix();
        }
    }

    public float FarPlane
    {
        get => _far;
        set
        {
            _far = value;
            GenerateProjectionMatrix();
        }
    }

    public Camera(float fov, float aspectRatio, float near = 0.1f, float far = 1000f)
    {
        _fov = fov;
        _aspectRatio = aspectRatio;
        _near = near;
        _far = far;
        GenerateProjectionMatrix();
    }

    protected internal override void Initialize()
    {
        base.Initialize();
        
        Graphics.ViewportResized += GraphicsOnViewportResized;
    }

    private void GraphicsOnViewportResized(Rectangle viewport)
    {
        _aspectRatio = viewport.Width / (float) viewport.Height;
        GenerateProjectionMatrix();
    }

    private void GenerateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _near, _far);
    }

    public override void Dispose()
    {
        base.Dispose();

        Graphics.ViewportResized -= GraphicsOnViewportResized;
    }

    public static Camera Main => (Camera) SceneManager.ActiveScene.GetEntitiesWithTag(Tags.MainCamera)[0];
}