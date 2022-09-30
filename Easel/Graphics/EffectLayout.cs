using System;
using Pie;

namespace Easel.Graphics;

/// <summary>
/// Represents a combined <see cref="Graphics.Effect"/> and <see cref="InputLayout"/>.
/// </summary>
public class EffectLayout : IDisposable
{
    /// <summary>
    /// The <see cref="Effect"/> of this effect layout.
    /// </summary>
    public readonly Effect Effect;
    
    /// <summary>
    /// The <see cref="InputLayout"/> of this effect layout.
    /// </summary>
    public readonly InputLayout Layout;

    /// <summary>
    /// Create a new <see cref="EffectLayout"/> with the given <see cref="Graphics.Effect"/> and <see cref="InputLayout"/>.
    /// </summary>
    /// <param name="effect">The <see cref="Graphics.Effect"/> to use.</param>
    /// <param name="layout">The <see cref="InputLayout"/> to use.</param>
    public EffectLayout(Effect effect, InputLayout layout)
    {
        Effect = effect;
        Layout = layout;
    }

    public void Dispose()
    {
        Effect.Dispose();
        Layout.Dispose();
    }
}