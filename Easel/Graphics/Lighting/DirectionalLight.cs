using System.Numerics;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Graphics.Lighting;

public class DirectionalLight
{
    public Vector3 Direction;
    
    public Color Ambient;
    
    public Color Diffuse;
    
    public Color Specular;

    public ShaderDirectionalLight ShaderDirectionalLight => new ShaderDirectionalLight()
    {
        Direction = new Vector4(Direction, 1),
        Ambient = Ambient,
        Diffuse = Diffuse,
        Specular = Specular
    };

    public DirectionalLight(Vector3 direction, Color ambient, Color diffuse, Color specular)
    {
        Direction = direction;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
    }
}