using System.Numerics;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Graphics;

public class Material
{
    public Texture2D Albedo;

    public Texture2D Specular;

    public Color Color;

    public int Shininess;

    public Vector2 Tiling;

    public ShaderMaterial ShaderMaterial => new ShaderMaterial()
    {
        Color = Color,
        Specular = Shininess,
        //Tiling = new Vector4(Tiling, 0, 0)
    };

    public Material(Texture2D texture) : this(texture, texture, Color.White, 32) { }
    
    public Material(Texture2D texture, Color color) : this(texture, texture, color, 32) { }
    
    public Material(Texture2D texture, Color color, int shininess) : this(texture, texture, color, shininess) { }

    public Material(Texture2D albedo, Texture2D specular, Color color, int shininess)
    {
        Albedo = albedo;
        Specular = specular;
        Color = color;
        Shininess = shininess;
        Tiling = new Vector2(0);
    }
}