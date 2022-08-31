using System;
using System.Collections.Generic;
using Easel.Renderers;
using Easel.Utilities;
using Pie;
using Pie.Utils;

namespace Easel.Graphics;

/// <summary>
/// Contains a large list of built-in effects/shaders and their <see cref="InputLayout"/>s. These effects are lazily
/// initialized, they are only created when used, saving unnecessary memory usage. However, currently, once initialized,
/// they remain created until the program is exited. (This may change.)
/// </summary>
public static class BuiltinEffects
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
    }
    
    private static Dictionary<string, Lazy<EffectLayout>> _effects;

    private const string Assembly = "Easel.Graphics.Shaders.";

    static BuiltinEffects()
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        
        _effects = new Dictionary<string, Lazy<EffectLayout>>();
        _effects.Add("Forward/Standard",
            new Lazy<EffectLayout>(() =>
                new EffectLayout(
                    new Effect(Assembly + "Forward.Standard.vert", Assembly + "Forward.Standard.frag"),
                    device.CreateInputLayout(VertexPositionTextureNormal.SizeInBytes,
                        new InputLayoutDescription("aPosition", AttributeType.Vec3),
                        new InputLayoutDescription("aTexCoords", AttributeType.Vec2)))));
    }

    /// <summary>
    /// Get the <see cref="EffectLayout"/> with the given name, and is initialized if it has not been initialized already.
    /// </summary>
    /// <param name="name">The name of the <see cref="EffectLayout"/>.</param>
    /// <returns>The <see cref="EffectLayout"/>.</returns>
    public static EffectLayout GetEffectLayout(string name)
    {
        Lazy<EffectLayout> value = _effects[name];
        if (!value.IsValueCreated)
            Console.WriteLine($"Effect \"{name}\" is used for first time: Creating...");
        return value.Value;
    }
}