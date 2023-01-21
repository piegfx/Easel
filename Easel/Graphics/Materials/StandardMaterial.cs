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
    public Color Albedo;
    public float Metallic;
    public float Roughness;


    public StandardMaterial(Color albedo) : this(albedo, 0, 0.4f) { }

    public StandardMaterial(Color albedo, float metallic, float roughness)
    {
        Albedo = albedo;
        Metallic = metallic;
        Roughness = roughness;

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
        Albedo = Albedo,
        Metallic = Metallic,
        Roughness = Roughness,
        Ao = 1,
        Tiling = new Vector4(Tiling, 0, 0)
    };

    protected internal override void ApplyTextures(GraphicsDevice device)
    {
        //device.SetTexture(TextureBindingLoc, Diffuse.PieTexture, Diffuse.SamplerState.PieSamplerState);
    }
}