using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Panel : UIElement
{
    public Panel(Position position, Size size) : base(position, size) { }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        throw new System.NotImplementedException();
    }
}