using System;
using System.Collections.Generic;
using Easel.Core;
using Pie;

namespace Easel.Graphics;

public class BlendState : IDisposable
{
    private static Dictionary<BlendStateDescription, BlendState> _cachedStates;

    static BlendState()
    {
        _cachedStates = new Dictionary<BlendStateDescription, BlendState>();
    }

    public Pie.BlendState PieBlendState;

    private BlendState(in BlendStateDescription description)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        PieBlendState = device.CreateBlendState(description);
    }

    public static BlendState FromDescription(in BlendStateDescription description)
    {
        if (!_cachedStates.TryGetValue(description, out BlendState state))
        {
            Logger.Debug("Creating new blend state.");
            state = new BlendState(description);
            _cachedStates.Add(description, state);
        }

        return state;
    }

    public static BlendState Disabled => FromDescription(BlendStateDescription.Disabled);

    public static BlendState DisabledRgbMask => FromDescription(BlendStateDescription.Disabled with
    {
        ColorWriteMask = ColorWriteMask.Red | ColorWriteMask.Green | ColorWriteMask.Blue
    });

    public static BlendState AlphaBlend => FromDescription(BlendStateDescription.AlphaBlend);

    public static BlendState Additive => FromDescription(BlendStateDescription.Additive);
    
    public static BlendState Opaque => FromDescription(BlendStateDescription.Opaque);

    public static BlendState NonPremultiplied => FromDescription(BlendStateDescription.NonPremultiplied);

    public void Dispose()
    {
        _cachedStates.Remove(PieBlendState.Description);
        PieBlendState.Dispose();
    }
}