using System;
using System.Collections.Generic;
using Easel.Renderers;
using Pie;
using Pie.Utils;

namespace Easel.Graphics;

public static class BuiltinEffects
{
    public static class Forward
    {
        public const string StandardUnlit = "Forward/StandardUnlit";
    }
    
    private static Dictionary<string, Lazy<EffectLayout>> _effects;

    static BuiltinEffects()
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        
        _effects = new Dictionary<string, Lazy<EffectLayout>>();
        _effects.Add("Forward/StandardUnlit",
            new Lazy<EffectLayout>(() =>
                new EffectLayout(new Effect(ForwardRenderer.TempVertex, ForwardRenderer.TempFragment),
                    device.CreateInputLayout(VertexPositionTextureNormal.SizeInBytes,
                        new InputLayoutDescription("aPosition", AttributeType.Vec3),
                        new InputLayoutDescription("aTexCoords", AttributeType.Vec2)))));
    }

    public static EffectLayout GetEffectLayout(string name)
    {
        Lazy<EffectLayout> value = _effects[name];
        if (!value.IsValueCreated)
            Console.WriteLine($"Effect \"{name}\" is used for first time: Creating...");
        return value.Value;
    }
}