using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

public struct MaterialDescription
{
    public Texture2D Albedo;
    public Texture2D Normal;
    public Texture2D Metallic;
    public Texture2D Roughness;
    public Texture2D AmbientOcclusion;
    public Texture2D Emissive;

    public Color AlbedoColor;

    public AlphaMode AlphaMode;

    public bool CastShadows;
    public bool ReceiveShadows;

    public PrimitiveType PrimitiveType;

    public MaterialDescription()
    {
        Albedo = null;
        Normal = null;
        Metallic = null;
        Roughness = null;
        AmbientOcclusion = null;
        Emissive = null;
        
        AlbedoColor = Color.White;

        AlphaMode = AlphaMode.Opaque;

        CastShadows = true;
        ReceiveShadows = true;

        PrimitiveType = PrimitiveType.TriangleList;
    }
}