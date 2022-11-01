using Easel.Graphics;
using Easel.Math;

namespace Easel.GUI;

public class Image : UIElement
{
    public static void Draw(Position position, Size size, Texture texture)
    {
        UI.AddToDrawList(new UI.TextureDrawListInstruction(texture, position, size));
    }
}