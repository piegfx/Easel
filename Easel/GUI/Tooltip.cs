using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Tooltip
{
    public UITheme Theme;

    public string Text;
    public uint FontSize;
    
    public Tooltip(string text, uint fontSize = 24, UITheme? theme = null)
    {
        Text = text;
        FontSize = fontSize;
        Theme = theme ?? UI.Theme;
    }

    public void Draw(SpriteRenderer renderer)
    {
        Vector2T<int> size = (Vector2T<int>) Theme.Font.MeasureStringBBCode(FontSize, Text);
        Vector2T<int> pos = (Vector2T<int>) Input.MousePosition + new Vector2T<int>(0, -20);
        renderer.DrawRectangle(pos, (Size<int>) (size + new Vector2T<int>(0, 10)), Theme.BorderWidth, Theme.BorderRadius, Theme.BackgroundColor, Theme.BorderColor, 0, Vector2.Zero);
        Theme.Font.DrawBBCode(renderer, FontSize, Text, (pos + size / 2 - size / 2), Theme.FontColor);
    }
}