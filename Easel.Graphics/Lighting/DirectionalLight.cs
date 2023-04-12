using System;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Easel.Graphics.Lighting;

public struct DirectionalLight
{
    private Vector2T<float> _direction;
    private Vector3 _position;

    private Matrix4x4 _projection;
    private Matrix4x4 _view;

    public Vector2T<float> Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            float theta = -value.Y;
            float phi = value.X;
            _position = new Vector3(MathF.Cos(phi) * MathF.Cos(theta), MathF.Cos(phi) * MathF.Sin(theta),
                MathF.Sin(phi));
        }
    }
    
    public Color Color;

    public ShadowMap ShadowMap;

    public DirectionalLight(Vector2T<float> direction, Color color, int numShadowCascades = 4)
    {
        Direction = direction;
        Color = color;
        if (numShadowCascades > 0)
            ShadowMap = new ShadowMap(new Size<int>(1024), numShadowCascades);
    }

    public ShaderDirLight ShaderDirLight => new ShaderDirLight()
    {
        Direction = new Vector4(_position, 0),
        Color = Color
    };

    private void CalculateViewProjMatrices()
    {
        // TODO: Move these to user-adjustable values.
        const float near = 0.1f;
        const float far = 1000.0f;
    }
}