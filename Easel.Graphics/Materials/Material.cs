using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

/// <summary>
/// A material represents a set of parameters that tells Easel how to render an object.
/// </summary>
public abstract class Material
{
    public const int TextureBindingLoc = 2;
    
    /// <summary>
    /// The <see cref="Easel.Graphics.EffectLayout"/> of this material.
    /// Each material will contain its own <see cref="Easel.Graphics.EffectLayout"/>.
    /// </summary>
    public EffectLayout EffectLayout { get; protected set; }
    
    public abstract ShaderMaterial ShaderMaterial { get; }
    
    /// <summary>
    /// The tint color of this material. (Default: White)
    /// </summary>
    public Color Tint;

    /// <summary>
    /// How much the texture will tile. (Default: 1)
    /// </summary>
    public Vector2 Tiling;

    /// <summary>
    /// The rasterizer state of this material. (Default: CullClockwise)
    /// </summary>
    public RasterizerState RasterizerState;

    protected Material()
    {
        Tint = Color.White;
        Tiling = Vector2.One;
        RasterizerState = RasterizerState.CullClockwise;
    }

    protected internal abstract void ApplyTextures(GraphicsDevice device);
}