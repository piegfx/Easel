using Easel.Graphics;
using Easel.Math;

namespace Easel.GUI;

public class Image : UIElement
{
    public Image(Position position, Size size) : base(position, size) { }
    
    public static Image Place(Position position, Size size, Texture texture)
    {
        Image image = new Image(position, size);
        UI.AddElement(image, new UI.TextureDrawListInstruction(texture, position, size));

        return image;
    }
}