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
        Size size = Theme.Font.MeasureStringBBCode(FontSize, Text);
        Point pos = (Point) Input.MousePosition + new Point(0, -20);
        renderer.DrawRectangle((Vector2) pos, size + new Size(0, 10), Theme.BorderWidth, Theme.BorderRadius, Theme.BackgroundColor, Theme.BorderColor, 0, Vector2.Zero);
        Theme.Font.DrawBBCode(FontSize, Text, (Vector2) (pos + (Point) size / 2 - (Point) size / 2), Theme.FontColor);
    }
}