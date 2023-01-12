using Easel.Graphics.Renderers.Structs;
using Easel.Utilities;
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

    public StandardMaterial(Texture diffuse) : this(diffuse, null, 1) { }

    public StandardMaterial(Texture diffuse, float shininess) : this(diffuse, diffuse, shininess) { }
    
    public StandardMaterial(Texture diffuse, Texture specular, float shininess)
    {
        Diffuse = diffuse;
        Specular = specular;
        ShininessExponent = shininess;
        
        // TODO hey do the darn caching!!!
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        InputLayout layout = device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Float3, 0, 0, InputType.PerVertex),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2, 12, 0, InputType.PerVertex),
            new InputLayoutDescription("aNormals", AttributeType.Float2, 20, 0, InputType.PerVertex)
        );

        EffectLayout =
            new EffectLayout(
                new Effect("Easel.Graphics.Shaders.Standard.vert", "Easel.Graphics.Shaders.Forward.Standard.frag",
                    defines: "LIGHTING"), layout, VertexPositionTextureNormalTangent.SizeInBytes);
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