
using System.Numerics;
using Easel.Math;
using Easel.Scenes;

namespace Easel.Entities;

/// <summary>
/// A perspective camera used for 3D scenes.
/// </summary>
public class Camera : Entity
{
    public Rectangle? Viewport;
    
    /// <summary>
    /// The projection matrix of this camera.
    /// </summary>
    public Matrix4x4 ProjectionMatrix { get; private set; }

    /// <summary>
    /// Calculates and returns the view matrix of this camera.
    /// </summary>
    public Matrix4x4 ViewMatrix =>
        Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);

    private float _fov;
    private float _aspectRatio;
    private float _near;
    private float _far;

    /// <summary>
    /// Get or set the field of view (FOV), in radians, of this camera.
    /// </summary>
    public float FieldOfView
    {
        get => _fov;
        set
        {
            _fov = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// Get or set the aspect ratio (typically width / height) of this camera. You won't normally need to set this value.
    /// </summary>
    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// The near plane distance of this camera.
    /// </summary>
    public float NearPlane
    {
        get => _near;
        set
        {
            _near = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// The far plane distance of this camera.
    /// </summary>
    public float FarPlane
    {
        get => _far;
        set
        {
            _far = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// Create a new camera for use in 3D scenes.
    /// </summary>
    /// <param name="fov">The field of view, in radians, of this camera.</param>
    /// <param name="aspectRatio">The aspect ratio of this camera (typically width / height).</param>
    /// <param name="near">The near plane distance of this camera.</param>
    /// <param name="far">The far plane distance of this camera.</param>
    /// <remarks>Multiple cameras are not currently supported.</remarks>
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

    /// <summary>
    /// Get the main camera for the current scene. This is the first camera in the scene with the
    /// <see cref="Tags.MainCamera"/> tag.
    /// </summary>
    public static Camera Main => (Camera) SceneManager.ActiveScene.GetEntitiesWithTag(Tags.MainCamera)[0];
}