using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Panel : UIElement
{
    public Panel(string name, Position position, Size<int> size) : base(name, position, size) { }
    
    protected internal override void Draw(SpriteRenderer renderer, double scale)
    {
        Color bg = Style.Panel.BackgroundColor;
        Color border = Style.Panel.BorderColor;
        float radius = Style.Panel.BorderRadius;
        int width = Style.Panel.BorderWidth;

        if (Style.BackgroundTexture != null)
        {
            renderer.Draw(Style.BackgroundTexture, (Vector2T<float>) CalculatedScreenPos,
                new Rectangle<int>(CalculatedScreenPos, Size), Color.White, 0, Vector2T<float>.Zero,
                Vector2T<float>.One);
        }

        renderer.DrawRectangle((Vector2T<float>) CalculatedScreenPos, Size, width, radius, bg, border, 0,
            Vector2T<float>.Zero);
    }
}