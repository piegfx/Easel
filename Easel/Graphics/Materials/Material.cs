using System.Numerics;
using Easel.Graphics.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

public abstract class Material
{
    public EffectLayout EffectLayout { get; protected set; }
    
    public Color Color;

    public Vector2 Tiling;

    public RasterizerState RasterizerState;

    protected Material()
    {
        Color = Color.White;
        Tiling = Vector2.One;
        RasterizerState = RasterizerState.CullClockwise;
    }

    internal abstract void Apply(GraphicsDevice device);
}