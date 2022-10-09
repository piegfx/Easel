using System.Numerics;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Graphics;

public class Material
{
    private string _effect;
    
    public string Effect
    {
        get => _effect;
        set
        {
            EffectLayout = EaselGame.Instance.GraphicsInternal.EffectManager.GetEffectLayout(value);
            _effect = value;
        }
    }

    public EffectLayout EffectLayout;
    
    public Texture Albedo;

    public Texture Specular;

    public Texture Normal;

    public Color Color;

    public float Shininess;

    public Vector2 Tiling;

    public float AlphaCutoff;

    public ShaderMaterial ShaderMaterial => new ShaderMaterial()
    {
        Color = Color,
        Specular = new Vector4(Shininess),
        Tiling = new Vector4(Tiling, 0, 0),
        AlphaCutoff = new Vector4(AlphaCutoff)
    };

    public Material(Texture texture) : this(texture, texture, null, Color.White, 32) { }
    
    public Material(Texture texture, Color color) : this(texture, texture, null, color, 32) { }
    
    public Material(Texture texture, Color color, int shininess) : this(texture, texture, null, color, shininess) { }

    public Material(Texture albedo, Texture specular, Texture2D normal, Color color, float shininess)
    {
        Albedo = albedo;
        Specular = specular ?? albedo;
        Normal = normal;
        Color = color;
        Shininess = shininess;
        Tiling = new Vector2(1);
        AlphaCutoff = 0;
        Effect = normal != null ? EffectManager.Forward.Normal : EffectManager.Forward.Diffuse;
    }

    public Material(Material clone)
    {
        Albedo = clone.Albedo;
        Specular = clone.Specular;
        Normal = clone.Normal;
        Color = new Color(clone.Color.R, clone.Color.G, clone.Color.B, clone.Color.A);
        Shininess = clone.Shininess;
        Tiling = new Vector2(clone.Tiling.X, clone.Tiling.Y);
        AlphaCutoff = clone.AlphaCutoff;
        Effect = clone.Effect;
    }
}