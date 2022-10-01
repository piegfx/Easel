using System;
using System.Collections.Generic;
using Easel.Utilities;
using Pie;
using Pie.Utils;

namespace Easel.Graphics;

/// <summary>
/// Contains a large list of built-in effects/shaders and their <see cref="InputLayout"/>s. These effects are lazily
/// initialized, they are only created when used, saving unnecessary memory usage. However, currently, once initialized,
/// they remain created until the program is exited. (This may change.)
/// </summary>
public class EffectManager
{
    /// <summary>
    /// Contains all the shaders that can be used for Forward rendering.
    /// </summary>
    public static class Forward
    {
        /// <summary>
        /// The standard shader.
        /// </summary>
        public const string Standard = "Forward/Standard";
        
        public const string StandardUnlit = "Forward/StandardUnlit";
    }
    
    private Dictionary<string, Lazy<EffectLayout>> _effects;

    private const string Assembly = "Easel.Graphics.Shaders.";

    internal EffectManager(GraphicsDevice device)
    {
        InputLayout standardVertexShader = device.CreateInputLayout(VertexPositionTextureNormal.SizeInBytes,
            new InputLayoutDescription("aPosition", AttributeType.Float3),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2),
            new InputLayoutDescription("aNormals", AttributeType.Float3));
        
        _effects = new Dictionary<string, Lazy<EffectLayout>>();
        _effects.Add("Forward/Standard", new Lazy<EffectLayout>(() =>
        {
            return new EffectLayout(new Effect(Assembly + "Standard.vert", Assembly + "Forward.Standard.frag", defines: "LIGHTING"),
                standardVertexShader);
        }));
        
        _effects.Add("Forward/StandardUnlit", new Lazy<EffectLayout>(() =>
        {
            return new EffectLayout(new Effect(Assembly + "Standard.vert", Assembly + "Forward.Standard.frag"),
                standardVertexShader);
        }));
    }

    /// <summary>
    /// Get the <see cref="EffectLayout"/> with the given name, and is initialized if it has not been initialized already.
    /// </summary>
    /// <param name="name">The name of the <see cref="EffectLayout"/>.</param>
    /// <returns>The <see cref="EffectLayout"/>.</returns>
    public EffectLayout GetEffectLayout(string name)
    {
        Lazy<EffectLayout> value = _effects[name];
        if (!value.IsValueCreated)
            Logging.Info($"Effect \"{name}\" is used for first time: Creating...");
        return value.Value;
    }
}