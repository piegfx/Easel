using System;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

/// <summary>
/// The standard material. Objects with this material receive lighting and shadows, as well as casting shadows.
/// </summary>
public class StandardMaterial : Material
{
    /// <summary>
    /// The albedo texture of this material, if any.
    /// </summary>
    public Texture Albedo;
    
    /// <summary>
    /// The normal texture of this material, if any.
    /// </summary>
    public Texture Normal;
    
    /// <summary>
    /// The metallic texture of this material, if any.
    /// </summary>
    public Texture Metallic;
    
    /// <summary>
    /// The roughness texture of this material, if any.
    /// </summary>
    public Texture Roughness;
    
    /// <summary>
    /// The ambient occlusion texture of this material, if any.
    /// </summary>
    public Texture Ao;

    /// <summary>
    /// The albedo color of this material. If <see cref="Albedo"/> is set, this acts like a tint.
    /// </summary>
    public Color AlbedoColor;

    /// <summary>
    /// The metallic value of this material. If <see cref="Metallic"/> is set, this value has no effect.
    /// </summary>
    public float MetallicValue;

    /// <summary>
    /// The roughness value of this material. If <see cref="Roughness"/> is set, this value has no effect.
    /// </summary>
    public float RoughnessValue;

    /// <summary>
    /// The ambient occlusion value of this material. If <see cref="Ao"/> is set, this value has no effect.
    /// </summary>
    public float AoValue;
    
    public StandardMaterial() : this(Texture2D.White) { }
    
    public StandardMaterial(Texture albedo) : this(albedo, Texture2D.EmptyNormal, Texture2D.Black, Texture2D.White, Texture2D.White) { }

    public StandardMaterial(Texture albedo, Texture normal, Texture metallicRoughnessAo) : this(albedo, normal,
        metallicRoughnessAo, metallicRoughnessAo, metallicRoughnessAo, new[] { "LIGHTING", "COMBINE_TEXTURES" }) { }
    
    public StandardMaterial(Texture albedo, Texture normal, Texture metallic, Texture roughness, Texture ao) : this(
        albedo, normal, metallic, roughness, ao, new[] { "LIGHTING" }) { }

    protected StandardMaterial(Texture albedo, Texture normal, Texture metallic, Texture roughness, Texture ao,
        string[] shaderDefines)
    {
        // TODO: Dynamic recompilation of shader based on the values?
        // Aka: If metallic texture is set to null, recompile the shader to remove the texture altogether, instead of
        // checking to see if it is null, at which point the MetallicValue is used.
        
        Albedo = albedo;
        Normal = normal;
        Metallic = metallic;
        Roughness = roughness;
        Ao = ao;
        
        AlbedoColor = Color.White;

        InputLayoutDescription[] descriptions = new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32_Float, 20, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32_Float, 32, 0, InputType.PerVertex)
        };

        EffectLayout = GetEffectLayout("Easel.Graphics.Shaders.Standard.vert",
            "Easel.Graphics.Shaders.Forward.Standard.frag", shaderDefines, descriptions,
            VertexPositionTextureNormalTangent.SizeInBytes);
    }

    public override ShaderMaterial ShaderMaterial => new ShaderMaterial()
    {
        Albedo = AlbedoColor,
        // Since the values currently act like multipliers in the shader, we must set them all to 1 for them to have
        // no effect. We ignore the value if the texture has been set.
        Metallic = Metallic != null ? 1 : MetallicValue,
        Roughness = Roughness != null ? 1 : RoughnessValue,
        Ao = Ao != null ? 1 : AoValue,
        Tiling = new Vector4((System.Numerics.Vector2) Tiling, 0, 0)
    };

    protected internal override void ApplyTextures(GraphicsDevice device)
    {
        if (Albedo != null)
            device.SetTexture(TextureBindingLoc + 0, Albedo.PieTexture, Albedo.SamplerState.PieSamplerState);
        if (Normal != null)
            device.SetTexture(TextureBindingLoc + 1, Normal.PieTexture, Normal.SamplerState.PieSamplerState);
        if (Metallic != null)
            device.SetTexture(TextureBindingLoc + 2, Metallic.PieTexture, Metallic.SamplerState.PieSamplerState);
        if (Roughness != null)
            device.SetTexture(TextureBindingLoc + 3, Roughness.PieTexture, Roughness.SamplerState.PieSamplerState);
        if (Ao != null)
            device.SetTexture(TextureBindingLoc + 4, Ao.PieTexture, Ao.SamplerState.PieSamplerState);
    }
}