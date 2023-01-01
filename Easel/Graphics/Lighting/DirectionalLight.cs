using System;
using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Lighting;

public class DirectionalLight
{
    public Vector2 Direction;
    
    public Color Ambient;
    
    public Color Diffuse;
    
    public Color Specular;

    public DirectionalLight(Vector2 direction, Color ambient, Color diffuse, Color specular)
    {
        Direction = direction;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
    }
}