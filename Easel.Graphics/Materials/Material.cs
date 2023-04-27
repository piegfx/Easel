using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

public class Material
{
    public Texture2D Albedo;
    public Texture2D Normal;
    public Texture2D Metallic;
    public Texture2D Roughness;
    public Texture2D AmbientOcclusion;
    public Texture2D Emissive;

    public Color AlbedoColor;

    public PrimitiveType PrimitiveType;
    
    public AlphaMode AlphaMode { get; }
    
    public bool CastShadows { get; }
    public bool ReceiveShadows { get; }

    public Material(in MaterialDescription description)
    {
        Albedo = description.Albedo ?? Texture2D.White;
        Normal = description.Normal ?? Texture2D.EmptyNormal;
        Metallic = description.Metallic ?? Texture2D.White;
        Roughness = description.Roughness ?? Texture2D.Black;
        AmbientOcclusion = description.AmbientOcclusion ?? Texture2D.White;
        Emissive = description.Emissive ?? Texture2D.Black;

        AlbedoColor = description.AlbedoColor;

        AlphaMode = description.AlphaMode;

        CastShadows = description.CastShadows;
        ReceiveShadows = description.CastShadows;
    }
}