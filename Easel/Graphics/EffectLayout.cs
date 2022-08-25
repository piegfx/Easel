using Pie;

namespace Easel.Graphics;

public class EffectLayout
{
    public readonly Effect Effect;
    public readonly InputLayout Layout;

    public EffectLayout(Effect effect, InputLayout layout)
    {
        Effect = effect;
        Layout = layout;
    }
}