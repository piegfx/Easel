namespace Easel.Graphics.Materials;

public class TranslucentStandardMaterial : StandardMaterial
{
    public TranslucentStandardMaterial() : this(Texture2D.White) { }
    
    public TranslucentStandardMaterial(Texture albedo) : this(albedo, Texture2D.EmptyNormal, Texture2D.Black, Texture2D.White, Texture2D.White) { }

    public TranslucentStandardMaterial(Texture albedo, Texture normal, Texture metallicRoughnessAo) : base(albedo,
        normal, metallicRoughnessAo, metallicRoughnessAo, metallicRoughnessAo,
        new[] { "LIGHTING", "ALPHA", "COMBINE_TEXTURES" })
    {
        IsTranslucent = true;
    }

    public TranslucentStandardMaterial(Texture albedo, Texture normal, Texture metallic, Texture roughness, Texture ao)
        : base(albedo, normal, metallic, roughness, ao, new[] { "LIGHTING", "ALPHA" })
    {
        IsTranslucent = true;
    }
}