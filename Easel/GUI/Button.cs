using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Button : UIElement
{
    public string Text;

    public Button(Position position, Size size, string text) : base(position, size)
    {
        Text = text;
    }

    protected internal override void Draw(SpriteRenderer renderer)
    {
        base.Draw(renderer);

        Color color = Theme.BackgroundColor;

        if (IsHovered)
            color = Theme.HoverColor;
        if (IsClicked)
            color = Theme.ClickedColor;

        if (Theme.DropShadow.HasValue)
        {
            renderer.DrawRoundedRect((Vector2) ResolvedPosition + Theme.DropShadow.Value.Offset, Size, 0, Theme.BorderRadius,
                Theme.DropShadow.Value.Color, Color.Transparent, 0, Vector2.Zero);
        }

        //if (Theme.BorderRadius > 0)
            renderer.DrawRoundedRect((Vector2) ResolvedPosition, Size, Theme.BorderWidth, Theme.BorderRadius, color, Theme.BorderColor, 0, Vector2.Zero);

        Size textSize = Theme.Font.MeasureString(24, Text);
        Theme.Font.Draw(24, Text, ((Vector2) ResolvedPosition + (Vector2) (Size / 2)) - (Vector2) (textSize / 2), Color.Black);
    }
}