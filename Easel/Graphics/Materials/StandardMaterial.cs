using Easel.Graphics.Renderers.Structs;
using Pie;

namespace Easel.Graphics.Materials;

/// <summary>
/// The standard non-PBR material. Objects with this material receive lighting and shadows, as well as casting shadows.
/// </summary>
public class StandardMaterial : Material
{
    public Texture Diffuse;

    public Texture Specular;

    //public Texture2D Normal;

    public float ShininessExponent;

    public StandardMaterial(Texture diffuse)
    {
        Diffuse = diffuse;
        Specular = null;
    }

    public StandardMaterial(Texture diffuse, float shininess)
    {
        Diffuse = diffuse;
        Specular = diffuse;
        ShininessExponent = shininess;
    }
    
    public StandardMaterial(Texture diffuse, Texture specular, float shininess)
    {
        Diffuse = diffuse;
        Specular = specular;
        ShininessExponent = shininess;
    }

    public override ShaderMaterial ShaderMaterial => new ShaderMaterial()
    {
        Color = Color,
        Tiling = Tiling,
        Shininess = ShininessExponent
    };

    protected internal override void ApplyTextures(GraphicsDevice device)
    {
        device.SetTexture(TextureBindingLoc, Diffuse.PieTexture, Diffuse.SamplerState.PieSamplerState);
        // Disable specular if the texture is null.
        if (Specular != null)
            device.SetTexture(TextureBindingLoc + 1, Specular.PieTexture, Specular.SamplerState.PieSamplerState);
        else
            device.SetTexture(TextureBindingLoc + 1, Texture2D.Void.PieTexture, SamplerState.LinearClamp.PieSamplerState);
    }
}