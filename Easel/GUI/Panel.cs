using System;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Panel : UIElement
{
    public Panel(string name, Position position, Size<int> size) : base(name, position, size) { }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        Color bg = Style.Panel.BackgroundColor;
        Color border = Style.Panel.BorderColor;
        float radius = Style.Panel.BorderRadius;
        int width = Style.Panel.BorderWidth;

        float scale = UI.Scale;
        Size<int> size = new Size<int>((int) (Size.Width * scale), (int) (Size.Height * scale));

        /*if (Style.BackgroundTexture != null)
        {
            renderer.Draw(Style.BackgroundTexture, (Vector2T<float>) CalculatedScreenPos,
                new Rectangle<int>(CalculatedScreenPos, size), Color.White, 0, Vector2T<float>.Zero,
                Vector2T<float>.One);
        }*/

        renderer.DrawRectangle(CalculatedScreenPos.As<float>(), size, width, radius, bg, border, 0,
            Vector2T<float>.Zero);
    }
}