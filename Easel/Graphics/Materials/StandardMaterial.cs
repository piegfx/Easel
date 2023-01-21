using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Easel.Utilities;
using Pie;

namespace Easel.Graphics.Materials;

/// <summary>
/// The standard material. Objects with this material receive lighting and shadows, as well as casting shadows.
/// </summary>
public class StandardMaterial : Material
{
    public Texture Albedo;
    public Texture Normal;
    public Texture Metallic;
    public Texture Roughness;
    public Texture Ao;

    public StandardMaterial(Texture2D albedo, Texture2D normal, Texture2D metallic, Texture2D roughness, Texture2D ao)
    {
        Albedo = albedo;
        Normal = normal;
        Metallic = metallic;
        Roughness = roughness;
        Ao = ao;

        // TODO hey do the darn caching!!!
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        InputLayout layout = device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Float3, 0, 0, InputType.PerVertex),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2, 12, 0, InputType.PerVertex),
            new InputLayoutDescription("aNormals", AttributeType.Float3, 20, 0, InputType.PerVertex),
            new InputLayoutDescription("aTangents", AttributeType.Float3, 32, 0, InputType.PerVertex)
        );

        EffectLayout =
            new EffectLayout(
                new Effect("Easel.Graphics.Shaders.Standard.vert", "Easel.Graphics.Shaders.Forward.Standard.frag",
                    defines: "LIGHTING"), layout, VertexPositionTextureNormalTangent.SizeInBytes);
    }

    public override ShaderMaterial ShaderMaterial => new ShaderMaterial()
    {
        Albedo = new Color(0),
        Metallic = 0,
        Roughness = 0,
        Ao = 1,
        Tiling = new Vector4(Tiling, 0, 0)
    };

    protected internal override void ApplyTextures(GraphicsDevice device)
    {
        device.SetTexture(TextureBindingLoc + 0, Albedo.PieTexture, Albedo.SamplerState.PieSamplerState);
        device.SetTexture(TextureBindingLoc + 1, Normal.PieTexture, Normal.SamplerState.PieSamplerState);
        device.SetTexture(TextureBindingLoc + 2, Metallic.PieTexture, Albedo.SamplerState.PieSamplerState);
        device.SetTexture(TextureBindingLoc + 3, Roughness.PieTexture, Albedo.SamplerState.PieSamplerState);
        device.SetTexture(TextureBindingLoc + 4, Ao.PieTexture, Albedo.SamplerState.PieSamplerState);
    }
}