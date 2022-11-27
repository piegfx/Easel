using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Label : UIElement
{
    public string Text;

    public uint FontSize;
    
    public Label(Position position, string text, uint fontSize) : base(position, Size.Zero)
    {
        Size = Theme.Font.MeasureString(fontSize, text);
        Text = text;
        FontSize = fontSize;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        Theme.Font.MeasureStringBBCode(FontSize, Text);
    }
}