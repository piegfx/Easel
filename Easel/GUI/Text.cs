using Easel.Math;

namespace Easel.GUI;

public class Text : UIElement
{
    public Text(Position position, Size size) : base(position, size) { }
    
    public static Text Place(Position position, string text, uint size)
    {
        Text txt = new Text(position, UI.Theme.Font.MeasureStringBBCode(size, text));
        UI.AddElement(txt, new UI.TextDrawListInstruction(position, text, size));

        return txt;
    }
}