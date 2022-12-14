
using System;
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
    public Matrix4x4 ViewMatrix
    {
        get
        {
            Matrix4x4 parent = Matrix4x4.Identity;
            if (Parent != null)
                Matrix4x4.Invert(Parent.Transform.TransformMatrix, out parent);
            return parent * Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
        }
    }

    public Frustum? Frustum
    {
        get
        {
            float halfVSide = _far * MathF.Tan(_fov * 0.5f);
            float halfHSize = halfVSide * _aspectRatio;
            //return new Frustum(
            //    new Frustum.Plane(Transform.Position, ),
            //    new Frustum.Plane());
            return new Frustum?();
        }
    }

    private float _fov;
    private float _aspectRatio;
    private float _near;
    private float _far;
    private CameraType _cameraType;

    /// <summary>
    /// The type of camera this is, a projection (typically for 3D), or an orthographic (typically for 2D) camera.
    /// </summary>
    /// <remarks>Certain camera properties will have no effect depending on which mode is selected.</remarks>
    public CameraType CameraType
    {
        get => _cameraType;
        set
        {
            _cameraType = value;
            GenerateProjectionMatrix();
        }
    }
    
    #region Perspective

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
    
    #endregion

    #region Orthographic

    private Vector2 _orthoSize;
    
    /// <summary>
    /// The size of the orthographic matrix, in normalized 0-1 coordinates. (1, 1) will be the size of the current
    /// viewport. (0.5, 0.5) will be half the size of the current viewport, and all objects will be twice the size.
    /// Use this for camera zoom functionality.
    /// </summary>
    /// <returns></returns>
    public Vector2 OrthoSize
    {
        get => _orthoSize;
        set
        {
            _orthoSize = value;
            GenerateProjectionMatrix();
        }
    }

    #endregion

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
        CameraType = CameraType.Perspective;
        _orthoSize = Vector2.One;
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
        ProjectionMatrix = CameraType switch
        {
            CameraType.Perspective => Matrix4x4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _near, _far),
            CameraType.Orthographic => Matrix4x4.CreateOrthographicOffCenter(0, Graphics.Viewport.Width * _orthoSize.X, Graphics.Viewport.Height * _orthoSize.Y, 0, -1, 1),
            _ => throw new ArgumentOutOfRangeException()
        };
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

public enum CameraType
{
    Perspective,
    Orthographic
}