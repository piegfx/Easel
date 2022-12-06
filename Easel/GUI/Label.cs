using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Label : UIElement
{
    public string Text;

    public uint FontSize;
    
    public Label(Position position, string text, uint fontSize) : base(position, Size.Zero)
    {
        Size = Theme.Font.MeasureStringBBCode(fontSize, text);
        Text = text;
        FontSize = fontSize;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        Size = Theme.Font.MeasureStringBBCode(FontSize, Text);
        
        Theme.Font.DrawBBCode(FontSize, Text, (Vector2) CalculatedScreenPos, Color.White);
    }
}