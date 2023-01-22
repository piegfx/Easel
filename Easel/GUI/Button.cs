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

    public Point TextOffset;

    public Justification Justification;

    public Texture Image;

    public Button(Position position, Size size, string text, uint fontSize = 24, Justification justification = Justification.Center) : base(position, size)
    {
        Text = text;
        FontSize = fontSize;
        Justification = justification;
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
        
        renderer.DrawRectangle(Image ?? Texture2D.White, (Vector2) CalculatedScreenPos, Size, Theme.BorderWidth, Theme.BorderRadius, color, Theme.BorderColor, 0, Vector2.Zero);
        Size size = Theme.Font.MeasureStringBBCode(FontSize, Text);

        int posX = Justification switch
        {
            Justification.Left => CalculatedScreenPos.X,
            Justification.Center => CalculatedScreenPos.X + Size.Width / 2 - size.Width / 2,
            Justification.Right => CalculatedScreenPos.X + Size.Width - size.Width,
            _ => throw new ArgumentOutOfRangeException()
        };
        Theme.Font.DrawBBCode(renderer, FontSize, Text, new Vector2(posX, CalculatedScreenPos.Y + Size.Height / 2 - size.Height / 2) + (Vector2) TextOffset, Theme.FontColor);
    }
}