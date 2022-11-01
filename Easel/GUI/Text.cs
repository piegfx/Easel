namespace Easel.GUI;

public class Text : UIElement
{
    public static void Draw(Position position, string text, uint size)
    {
        UI.AddToDrawList(new UI.TextDrawListInstruction(position, text, size));
    }
}