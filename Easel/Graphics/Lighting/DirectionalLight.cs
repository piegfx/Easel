using System;
using System.Numerics;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Graphics.Lighting;

public class DirectionalLight
{
    public Vector2 Direction;
    
    public Color Ambient;
    
    public Color Diffuse;
    
    public Color Specular;

    public ShaderDirectionalLight ShaderDirectionalLight => new ShaderDirectionalLight()
    {
        Direction = new Vector4(new Vector3(MathF.Cos(Direction.X) * MathF.Cos(-Direction.Y), MathF.Cos(Direction.X) * MathF.Sin(-Direction.Y), MathF.Sin(Direction.X)), 1),
        Ambient = Ambient,
        Diffuse = Diffuse,
        Specular = Specular
    };

    public DirectionalLight(Vector2 direction, Color ambient, Color diffuse, Color specular)
    {
        Direction = direction;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
    }
}