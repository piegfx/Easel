using Easel.Graphics.Renderers.Structs;
using Easel.Utilities;
using Pie;

namespace Easel.Graphics.Materials;

/// <summary>
/// An unlit/unshaded material. Objects using this material do not receive lighting and shadow information, however
/// <i>do</i> cast shadows.
/// </summary>
public sealed class UnlitMaterial : Material
{
    public Texture Diffuse;

    public UnlitMaterial(Texture diffuse)
    {
        Diffuse = diffuse;

        // TODO: Temporary - material caching system needs doing!
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        InputLayout layout = device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Float3, 0, 0, InputType.PerVertex),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2, 12, 0, InputType.PerVertex)
        );
        
        EffectLayout = new EffectLayout(new Effect("Easel.Graphics.Shaders.Standard.vert",
            "Easel.Graphics.Shaders.Forward.Standard.frag"), layout, VertexPositionTextureNormalTangent.SizeInBytes);
    }

    public override ShaderMaterial ShaderMaterial => new ShaderMaterial()
    {
        Color = Color,
        Tiling = Tiling
    };

    protected internal override void ApplyTextures(GraphicsDevice device)
    {
        device.SetTexture(TextureBindingLoc, Diffuse.PieTexture, Diffuse.SamplerState.PieSamplerState);
    }
}