using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Button : UIElement
{
    public string Text;
    
    public uint FontSize;

    public Button(Position position, Size size, string text, uint fontSize = 24) : base(position, size)
    {
        Text = text;
        FontSize = fontSize;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        Color color = Theme.BackgroundColor;
        if (IsHovering)
            color = Theme.HoverColor;
        if (IsMouseButtonHeld)
            color = Theme.ClickedColor;

        if (Theme.DropShadow.HasValue)
        {
            DropShadow shadow = Theme.DropShadow.Value;
            renderer.DrawRectangle((Vector2) CalculatedScreenPos + shadow.Offset, Size, 0, Theme.BorderRadius, shadow.Color, Color.Transparent, 0, Vector2.Zero);
        }

        renderer.DrawRectangle((Vector2) CalculatedScreenPos, Size, Theme.BorderWidth, Theme.BorderRadius, color, Theme.BorderColor, 0, Vector2.Zero);

        Size size = Theme.Font.MeasureStringBBCode(FontSize, Text);
        Theme.Font.DrawBBCode(FontSize, Text, (Vector2) (CalculatedScreenPos + (Point) Size / 2 - (Point) size / 2), Theme.FontColor);
    }
}