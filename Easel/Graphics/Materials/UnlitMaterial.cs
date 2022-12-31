using Easel.Utilities;
using Pie;

namespace Easel.Graphics.Materials;

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

    internal override void Apply(GraphicsDevice device)
    {
        device.SetTexture(1, Diffuse.PieTexture, Diffuse.SamplerState.PieSamplerState);
    }
}