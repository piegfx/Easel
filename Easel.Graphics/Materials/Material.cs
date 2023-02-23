using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Core;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

/// <summary>
/// A material represents a set of parameters that tells Easel how to render an object.
/// </summary>
public abstract class Material : IDisposable
{
    public const int TextureBindingLoc = 2;

    private static Dictionary<int, MaterialCache> _cache;
    private int _hash;

    static Material()
    {
        _cache = new Dictionary<int, MaterialCache>();
    }

    /// <summary>
    /// The <see cref="Easel.Graphics.EffectLayout"/> of this material.
    /// Each material will contain its own <see cref="Easel.Graphics.EffectLayout"/>.
    /// </summary>
    public EffectLayout EffectLayout { get; protected set; }
    
    /// <summary>
    /// Is <see langword="true" /> when using a translucent material, such as <see cref="TranslucentStandardMaterial"/>.
    /// </summary>
    public bool IsTranslucent { get; protected set; }
    
    public abstract ShaderMaterial ShaderMaterial { get; }

    /// <summary>
    /// How much the texture will tile. (Default: 1)
    /// </summary>
    public Vector2<float> Tiling;

    /// <summary>
    /// The rasterizer state of this material. (Default: CullClockwise)
    /// </summary>
    public RasterizerState RasterizerState;

    protected Material()
    {
        Tiling = Vector2<float>.One;
        RasterizerState = RasterizerState.CullCounterClockwise;
    }

    protected internal abstract void ApplyTextures(GraphicsDevice device);

    protected EffectLayout GetEffectLayout(string vShader, string fShader, string[] defines, InputLayoutDescription[] descriptions, uint stride)
    {
        // TODO: Potentially a better way of getting a hash code?
        
        _hash = (vShader + fShader + string.Join(' ', defines)).GetHashCode();
        if (!_cache.TryGetValue(_hash, out MaterialCache cache))
        {
            Logger.Debug($"Creating new material cache. (ID: {_hash})");
            InputLayout layout = EaselGraphics.Instance.PieGraphics.CreateInputLayout(descriptions);

            cache = new MaterialCache()
            {
                EffectLayout = new EffectLayout(new Effect(vShader, fShader, defines: defines), layout, stride),
                NumReferences = 0
            };
            
            _cache.Add(_hash, cache);
        }

        cache.NumReferences++;
        return cache.EffectLayout;
    }

    public virtual void Dispose()
    {
        MaterialCache cache = _cache[_hash];
        cache.NumReferences--;
        Console.WriteLine(cache.NumReferences);
        if (cache.NumReferences <= 0)
        {
            Logger.Debug($"Disposing of material cache. (ID: {_hash})");
            cache.EffectLayout.Dispose();
            _cache.Remove(_hash);
        }
        
        Logger.Debug("Material disposed.");
    }
    
    private class MaterialCache
    {
        public EffectLayout EffectLayout;
        public int NumReferences;
    }
}